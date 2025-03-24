using CelestialBodies;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.InputSystem;


public class bodySimulation : MonoBehaviour, ICelestialObserver
{
    public bool simulating; // Start/pause simulation

    [Tooltip("Length of line")]
    [UnityEngine.Range(1, 5)] public int lengthIncrement = 1;

    [Tooltip("Simulation time step")]
    [UnityEngine.Range(0.001f, 1)] public float timeStep = 0.01f;

    [Tooltip("Number of points computed per update")]
    [UnityEngine.Range(1, 25)] public int simulationSpeed = 1;

    public bool relativeToBody;
    public CelestialBody centralBody;   // Centre of simulation, most often the sun
    public int centralBodyIndex;        // Index of central body in celestialBodies array
    public float lineWidth = 1f; // TODO: CHECK

    private VirtualBody[] _virtualBodies; // Virtual bodies for physics calculations

    private List<List<Vector3>> _orbitPoints;
    private List<int> _orbitSizes;
    private int _referenceFrameIndex;
    private Vector3 _referenceBodyInitialPosition;
    private IPhysicsModel _physicsModel;


    // check if needed
    private CelestialBody[] _bodies;
    private LineRenderer[] _lineRenderers;


    // Allow settings of physics model
    public void SetPhysicsModel(IPhysicsModel model)
    {
        _physicsModel = model;
    }


    public void StartSimulation()
    {
        StopSimulation();
        
        // TODO check deselect body

        _bodies = FindObjectsOfType<CelestialBody>();
        int numBodies = _bodies.Length;
        _virtualBodies = new VirtualBody[numBodies];
        _orbitPoints = new List<List<Vector3>>(numBodies);
        _orbitSizes = new List<int>(numBodies);
        _lineRenderers = new LineRenderer[numBodies];

        _referenceFrameIndex = 0;
        _referenceBodyInitialPosition = Vector3.zero;


        for (var i = 0; i < numBodies; i++)
        {
            _orbitSizes.Add(ComputeOrbitSize());
            _orbitPoints.Add(new List<Vector3>(_orbitSizes[i]));

            _virtualBodies[i] = new VirtualBody(_bodies[i]);

            if (_bodies[i] == centralBody && relativeToBody)
            {
                _referenceFrameIndex = i;
                _referenceBodyInitialPosition = _virtualBodies[i].Position;
            }

            // Setup orbit visualization
            _lineRenderers[i] = _bodies[i].gameObject.AddComponent<LineRenderer>();
            _lineRenderers[i].material = new Material(Shader.Find("Sprites/Default"));
            
            _lineRenderers[i].alignment = LineAlignment.TransformZ;

            _lineRenderers[i].positionCount = 0;
            // TODO: SET COLOUR BASED ON BODY COLOUR
            _lineRenderers[i].startColor = Color.white;

            // TODO: SET COLOUR BASED ON BODY COLOUR -> DUMMY CONFIG NEEDED
            _lineRenderers[i].endColor = Color.red;
            //_lineRenderers[i].endColor = _bodies[i].celestiaBodyGenerator.bodyConfig.shading.GetConfig().mainColor;

            _lineRenderers[i].widthMultiplier = lineWidth;
        }

        // Begin simulating
        StartCoroutine(SimulationLoop());

    }

    // TODO:EXPLAIN
    private int ComputeOrbitSize()
    {
        return Mathf.Clamp(Mathf.CeilToInt(400 / timeStep), 100, 4000);
    }

    private IEnumerator SimulationLoop()
    {
        _physicsModel.InitializeBodies(_virtualBodies);

        int stepCount = 0;

        while (true)
        {
            // IMPLEMENT PAUSING HERE
            while (GameManager.Instance.tempPause)
            {
                yield return null;
            }

            // CHECK SIMULATION SPEED
            _physicsModel.UpdateBodies(_virtualBodies, timeStep * simulationSpeed);

            for (var i = 0; i < _virtualBodies.Length; i++)
            {
                Vector3 newPos = _virtualBodies[i].Position;

                // Check values


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
                _bodies[i].transform.position = newPos;

                // Update orbbit visualisation
                _lineRenderers[i].positionCount = _orbitPoints[i].Count;
                _lineRenderers[i].SetPositions(_orbitPoints[i].ToArray());
            }

            stepCount++;
            if (stepCount % simulationSpeed != 0) continue;

            stepCount = 0;
            yield return null;
        }
    }

    // public void StopSimulation()
    // {
    //     StopAllCoroutines();

    //     if (_bodies == null)
    //     {
            
    //     } 

    //     foreach (var body in _bodies)
    //     {
    //         body.transform.position = body.celestiaBodyGenerator.bodyConfig.physics.GetPhysicalConfig().initialPosition;
    //     }

    //     foreach (var line in _lineRenderers)
    //     {
    //         if (line != null) line.positionCount = 0; // (If active) reset line renderers
    //     }
    // }

    public void StopSimulation()
    {
        StopAllCoroutines();
        
        // Add null checks
        if (_bodies != null)
        {
            foreach (var body in _bodies)
            {
                if (body != null && body.celestiaBodyGenerator != null && 
                    body.celestiaBodyGenerator.bodyConfig != null &&
                    body.celestiaBodyGenerator.bodyConfig.physics != null)
                {
                    body.transform.position = body.celestiaBodyGenerator.bodyConfig.physics.GetPhysicalConfig().initialPosition;
                }
            }
        }

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
    }

    public void OnPhysicsUpdate()
    {
        // STOP SIMULATION AND UPDATE PHYSICS THIGNS
    }

    // SHAPE, shaing , intial update
    public void OnShapeUpdate()
    {
    }

    public void OnShadingUpdate()
    {
        // TODO: Implement this method
    }

    public void OnInitialUpdate()
    {
        // TODO: Implement this method
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