using CelestialBodies;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;


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
    public float lineWidth = 100; // TODO: CHECK

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
            _lineRenderers[i].positionCount = 0;
            // TODO: SET COLOUR BASED ON BODY COLOUR
            _lineRenderers[i].startColor = Color.white;
            _lineRenderers[i].endColor = _bodies[i].celestiaBodyGenerator.bodyConfig.shading.GetConfig().mainColor;
            _lineRenderers[i].widthMultiplier = lineWidth;
        }


        // CONTINUE HERE
        // CONTINUE HERE
        // CONTINUE HERE
        // CONTINUE HERE
        // START COROUTINE SIM LOOP
        // CONTINUE HERE
        // CONTINUE HERE
        // CONTINUE HERE
        // CONTINUE HERE

    }

    // TODO:EXPLAIN
    private int ComputeOrbitSize()
    {
        return Mathf.Clamp(Mathf.CeilToInt(400 / timeStep), 100, 4000);
    }


    private IEnumerator SimulationLoop()
    {
        return null;
    }



    


    public void StopSimulation()
    {
        StopAllCoroutines();
        foreach (var body in _bodies)
        {
            body.transform.position = body.celestiaBodyGenerator.bodyConfig.physics.GetPhysicalConfig().initialPosition;
        }

        foreach (var line in _lineRenderers)
        {
            if (line != null) line.positionCount = 0; // (If active) reset line renderers
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
        //void InitializeBodies(VirtualBody[] bodies);
    }


    // virtual body class to decouple gravity calculations from GameObjects

    // CLASS FOR KEPLERIAN PHYSICS

    



    public class VirtualBody
    {
        public Vector3 Position;
        public Vector3 Velocity;
        
        // TODO: MASS EDITING
        public readonly float Mass;

        public VirtualBody(CelestialBody body)
        {
            Physics.PhysicsSettings physicalConfig = body.celestiaBodyGenerator.bodyConfig.physics.GetPhysicalConfig();
            Position = physicalConfig.initialPosition;
            Velocity = physicalConfig.initialVelocity;
            Mass = body.mass;
        }
    }