using UnityEngine;
using CelestialBodies.Config;


public class PhysicsTestScript : MonoBehaviour
{
    //public CelestialBody celestialBody;

    public GameObject centralBody;
    public GameObject[] orbitingBodies;

    public bodySimulation simulation;

    [Header("Physics Properties")]
    public float centralmass = 10f;
    public float orbitDistance = 10f;
    public float orbitSpeed = 5f;


    private void Start()
    {
        if (!centralBody || orbitingBodies.Length == 0)
        {
            Debug.LogError("Central body or orbiting bodies not set");
            return;
        }
    
        if (simulation == null)
        {
            Debug.LogError("Simulation not set in PhysicsTestScript");
            return;
        }
    
        Debug.Log("Starting physics setup");
    
        centralBody.transform.position = Vector3.zero;
        CelestialBody central = FakeBodyWrapper(centralBody, Vector3.zero, Vector3.zero, centralmass);

        Debug.Log("Central body setup complete");

        // Setup orbiting bodies
        for (int i = 0; i < orbitingBodies.Length; i++)
        {
            float dist = orbitDistance + i * 5f; // slight offset for spacing
            Vector3 position = new Vector3(dist, 0, 0);

            // Calculate proper orbital velocity for circular orbit
            // v = sqrt(GM/r) where G is gravitational constant, M is central mass, r is distance
            float orbitalVelocity = Mathf.Sqrt(centralmass * 6.674f / dist);
            Vector3 velocity = new Vector3(0, orbitalVelocity, 0);

            // No need to keep the return unless you want to use the CelestialBody later
            _ = FakeBodyWrapper(orbitingBodies[i], position, velocity, 1f);
        }

//        KeplerianPhysics.OrbitalElements.mu = centralmass;


        // Start sim with keplerian physics
        simulation.centralBody = central;
        simulation.SetPhysicsModel(new KeplerMotion());
//        simulation.SetPhysicsModel(new NBodyPhysics());
  
        simulation.StartSimulation();
    }

    private CelestialBody FakeBodyWrapper(GameObject go, Vector3 pos, Vector3 vel, float mass)
    {

        // Check for NaN values
        if (float.IsNaN(pos.x) || float.IsNaN(pos.y) || float.IsNaN(pos.z))
        {
            Debug.LogError($"Invalid position detected for {go.name}. Using default position.");
            pos = Vector3.zero;
        }

        if (float.IsNaN(vel.x) || float.IsNaN(vel.y) || float.IsNaN(vel.z))
        {
            Debug.LogError($"Invalid velocity detected for {go.name}. Using default velocity.");
            vel = Vector3.zero;
        }

        mass = Mathf.Max(0.1f, mass); // Ensure mass is never zero or negative
        
        go.transform.position = pos;

        // Remove any existing components to avoid conflicts
        var existingBody = go.GetComponent<CelestialBody>();
        if (existingBody != null) DestroyImmediate(existingBody);
        
        var existingGen = go.GetComponent<CelestialBodyGenerator>();
        if (existingGen != null) DestroyImmediate(existingGen);

        // Create and setup the body first
        var body = go.AddComponent<CelestialBody>();
        body.mass = mass;
        body.position = pos;
        body.velocity = vel;

        // Create and setup the generator
        var gen = go.AddComponent<CelestialBodyGenerator>();
        body.celestiaBodyGenerator = gen;
        gen.body = body;

        // Create and setup the configs
        var bodyConfig = ScriptableObject.CreateInstance<CelestialBodyConfig>();
        bodyConfig.Init(CelestialBodyConfig.CelestialBodyType.Planet); // This should set up all required components

        var physicsWrapper = ScriptableObject.CreateInstance<Physics>();
        var settings = new Physics.PhysicsSettings();

        settings.initialPosition = pos;
        settings.initialVelocity = vel;

        physicsWrapper.SetSettings(settings);

        gen.bodyConfig = bodyConfig;
        gen.bodyConfig.physics = physicsWrapper;

        return body;
    }
}

        