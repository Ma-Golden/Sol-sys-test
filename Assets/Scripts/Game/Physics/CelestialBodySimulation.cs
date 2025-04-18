using CelestialBodies;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CelestialBodySimulation : MonoBehaviour, ICelestialObserver
{
    [Header("Simulation Controls")]
    public bool simulating = false; // Start/pause simulation

    [Tooltip("Length of line")]
    [UnityEngine.Range(1, 5)] public int lengthIncrement = 1;

    [Tooltip("Simulation time step")]
    [UnityEngine.Range(0.00001f, 1)] public float timeStep = 0.0001f;

    [Tooltip("Number of points computed per update")]
    [UnityEngine.Range(1, 25)] public int simulationSpeed = 1;

    [Header("Reference Frame")]
    public bool relativeToBody;
    public CelestialBody centralBody;   // Centre of simulation, most often the sun
    public int centralBodyIndex;        // Index of central body in celestialBodies array
    public float lineWidth = 1f;

    private List<List<Vector3>> _orbitPoints;
    private List<int> _orbitSizes;
    private int _referenceFrameIndex;
    private Vector3 _referenceBodyInitialPosition;
    public IPhysicsModel _physicsModel;

    private CelestialBody[] _bodies;
    private LineRenderer[] _lineRenderers;
    private VirtualBody[] _virtualBodies; // Virtual bodies for physics calculations
    private Coroutine _simulationCoroutine;
    private bool _isInitialized = false;

    // Allow settings of physics model
    public void SetPhysicsModel(IPhysicsModel model)
    {
        _physicsModel = model;
        Debug.Log($"Physics model set to: {model.GetType().Name}");
    }

    public void StartSimulation(CelestialBody[] orderedBodies)
    {
        StopSimulation();

        // Safety check for physics model
        if (_physicsModel == null)
        {
            Debug.LogError("Cannot start simulation: Physics model is null!");
            return;
        }

        // Safety check for bodies
        if (orderedBodies == null || orderedBodies.Length == 0)
        {
            Debug.LogError("Cannot start simulation: No bodies provided!");
            return;
        }

        Debug.Log($"Starting simulation with {orderedBodies.Length} bodies");
        
        _bodies = orderedBodies;
        int numBodies = _bodies.Length;
        _virtualBodies = new VirtualBody[numBodies];
        _orbitPoints = new List<List<Vector3>>(numBodies);
        _orbitSizes = new List<int>(numBodies);
        _lineRenderers = new LineRenderer[numBodies];

        _referenceFrameIndex = 0;
        _referenceBodyInitialPosition = Vector3.zero;

        for (var i = 0; i < numBodies; i++)
        {
            if (_bodies[i] == null)
            {
                Debug.LogError($"Body at index {i} is null. Aborting simulation start.");
                StopSimulation();
                return;
            }

            _orbitSizes.Add(ComputeOrbitSize());
            _orbitPoints.Add(new List<Vector3>(_orbitSizes[i]));

            try
            {
                _virtualBodies[i] = new VirtualBody(_bodies[i]);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to create VirtualBody for {_bodies[i].name}: {e.Message}");
                StopSimulation();
                return;
            }

            if (_bodies[i] == centralBody && relativeToBody)
            {
                _referenceFrameIndex = i;
                _referenceBodyInitialPosition = _virtualBodies[i].Position;
            }

            // Setup orbit visualization
            if (_bodies[i].gameObject.GetComponent<LineRenderer>() != null)
            {
                // Reuse existing line renderer if there is one
                _lineRenderers[i] = _bodies[i].gameObject.GetComponent<LineRenderer>();
            }
            else
            {
                // Create new line renderer
                _lineRenderers[i] = _bodies[i].gameObject.AddComponent<LineRenderer>();
            }
            
            _lineRenderers[i].material = new Material(Shader.Find("Sprites/Default"));
            _lineRenderers[i].alignment = LineAlignment.TransformZ;
            _lineRenderers[i].positionCount = 0;
            _lineRenderers[i].widthMultiplier = lineWidth;

            // Set color based on body color (if available)
            _lineRenderers[i].startColor = Color.white;
            _lineRenderers[i].endColor = Color.red;
        }

        _isInitialized = true;
        simulating = true;
        
        // Begin simulating
        _simulationCoroutine = StartCoroutine(SimulationLoop());
        Debug.Log("Simulation coroutine started.");
    }

    private int ComputeOrbitSize()
    {
        return Mathf.Clamp(Mathf.CeilToInt(400 / timeStep), 100, 4000);
    }

    private IEnumerator SimulationLoop()
    {
        if (!_isInitialized || _physicsModel == null)
        {
            Debug.LogError("Cannot start simulation loop: Not initialized or physics model is null!");
            simulating = false;
            yield break;
        }

        Debug.Log("Initializing physics model...");
        _physicsModel.InitializeBodies(_virtualBodies);

        int stepCount = 0;
        Debug.Log("Entering simulation loop...");

        while (simulating) // This is the key change - check the simulating flag
        {
            // Check for pause
            if (GameManager.Instance != null && GameManager.Instance.tempPause)
            {
                yield return null;
                continue;
            }

            // Update physics
            _physicsModel.UpdateBodies(_virtualBodies, timeStep * simulationSpeed);

            // Update positions and orbit trails
            for (var i = 0; i < _virtualBodies.Length; i++)
            {
                Vector3 newPos = _virtualBodies[i].Position;

                if (relativeToBody)
                {
                    Vector3 referenceFrameOffset = _virtualBodies[_referenceFrameIndex].Position - _referenceBodyInitialPosition;
                    newPos -= referenceFrameOffset;
                }

                if (relativeToBody && i == _referenceFrameIndex)
                {
                    newPos = _referenceBodyInitialPosition;
                }

                if (_orbitPoints[i].Count >= _orbitSizes[i])
                {
                    _orbitPoints[i].RemoveAt(0);
                }

                _orbitPoints[i].Add(newPos);
                
                // Update body position
                if (_bodies[i] != null)
                {
                    _bodies[i].transform.position = newPos;
                    _bodies[i].position = newPos; // Update the position field as well
                }

                // Update orbit visualization
                if (_lineRenderers[i] != null)
                {
                    _lineRenderers[i].positionCount = _orbitPoints[i].Count;
                    _lineRenderers[i].SetPositions(_orbitPoints[i].ToArray());
                }
            }

            stepCount++;
            if (stepCount % simulationSpeed != 0) continue;

            stepCount = 0;
            yield return null;
        }

        Debug.Log("Simulation loop ended");
    }

    public void StopSimulation()
    {
        if (_simulationCoroutine != null)
        {
            StopCoroutine(_simulationCoroutine);
            _simulationCoroutine = null;
        }
        
        simulating = false;
        
        // Reset bodies to initial positions
        if (_bodies != null)
        {
            foreach (var body in _bodies)
            {
                if (body != null && body.celestiaBodyGenerator != null && 
                    body.celestiaBodyGenerator.bodyConfig != null &&
                    body.celestiaBodyGenerator.bodyConfig.physics != null)
                {
                    Vector3 initialPos = body.celestiaBodyGenerator.bodyConfig.physics.GetPhysicalConfig().initialPosition;
                    body.transform.position = initialPos;
                    body.position = initialPos;
                }
            }
        }

        // Clear orbit trails
        if (_lineRenderers != null)
        {
            foreach (var line in _lineRenderers)
            {
                if (line != null) 
                {
                    line.positionCount = 0;
                }
            }
        }

        _isInitialized = false;
        Debug.Log("Simulation stopped");
    }

    public void OnPhysicsUpdate()
    {
        // Stop the simulation when physics settings change
        if (simulating)
        {
            StopSimulation();
        }
    }

    public void OnShapeUpdate()
    {
        // Not needed for simulation
    }

    public void OnShadingUpdate()
    {
        // Not needed for simulation
    }

    public void OnInitialUpdate()
    {
        // Not needed for simulation
    }

    private void Update()
    {
        // Handle simulation toggle from editor
        if (simulating && !_isInitialized)
        {
            if (StarSystemManager.Instance != null && StarSystemManager.Instance.systemBodies.Count > 0)
            {
                StartSimulation(StarSystemManager.Instance.systemBodies.ToArray());
            }
            else
            {
                Debug.LogWarning("Cannot start simulation: No bodies in system or StarSystemManager not properly initialized");
                simulating = false;
            }
        }
    }
}

public interface IPhysicsModel
{
    void InitializeBodies(VirtualBody[] bodies);
    void UpdateBodies(VirtualBody[] bodies, float timeStep);
}

public class VirtualBody
{
    public Vector3 Position;
    public Vector3 Velocity;
    public readonly float Mass;

    public VirtualBody(CelestialBody body)
    {
        if (body == null)
            throw new ArgumentNullException(nameof(body), "CelestialBody is null");
            
        if (body.celestiaBodyGenerator == null)
            throw new ArgumentNullException("CelestialBodyGenerator is null");
            
        if (body.celestiaBodyGenerator.bodyConfig == null)
            throw new ArgumentNullException("BodyConfig is null");
            
        if (body.celestiaBodyGenerator.bodyConfig.physics == null)
            throw new ArgumentNullException("Physics config is null");

        var physicalConfig = body.celestiaBodyGenerator.bodyConfig.physics.GetPhysicalConfig();
        if (physicalConfig == null)
            throw new ArgumentNullException("PhysicalConfig is null");

        Position = physicalConfig.initialPosition;
        Velocity = physicalConfig.initialVelocity;
        Mass = body.mass;
    }
}