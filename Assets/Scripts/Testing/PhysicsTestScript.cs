using UnityEngine;
using CelestialBodies.Config;

public class PhysicsTestScript : MonoBehaviour
{
    public GameObject centralBody;
    public GameObject[] orbitingBodies;

    public bodySimulation simulation;

    [Header("Physics Properties")]
    public float centralmass = 10f;
    public float orbitDistance = 40f;
    public float orbitSpeed = 5f;

    private void Start()
    {
        if (!centralBody || orbitingBodies.Length == 0)
        {
            Debug.LogError("Central body or orbiting bodies not set");
            return;
        }
    
        // Position central body at origin
        centralBody.transform.position = Vector3.zero;
        
        // Get the actual CelestialBody component from the central body
        CelestialBody central = centralBody.GetComponent<CelestialBody>();
        if (central == null)
        {
            Debug.LogError("Central body doesn't have a CelestialBody component");
            return;
        }
        
        // Set central body properties
        central.position = Vector3.zero;
        central.velocity = Vector3.zero;
        central.mass = centralmass;

        // Setup all bodies array, add central body as first entry
        CelestialBody[] allBodies = new CelestialBody[orbitingBodies.Length + 1];
        allBodies[0] = central;

        // Setup orbiting bodies
        for (int i = 0; i < orbitingBodies.Length; i++)
        {
            // Get the actual CelestialBody component
            CelestialBody orbiting = orbitingBodies[i].GetComponent<CelestialBody>();
            if (orbiting == null)
            {
                Debug.LogError($"Orbiting body {orbitingBodies[i].name} doesn't have a CelestialBody component");
                continue;
            }

            // Test position with set distance
            float dist = orbitDistance + i * 10f; // slight offset for spacing
            Vector3 position = new Vector3(dist, 0, 0);

            // Calculate proper orbital velocity for circular orbit
            // v = sqrt(GM/r) where G is gravitational constant, M is central mass, r is distance
            float orbitalVelocity = Mathf.Sqrt(centralmass * 6.674f / dist);
            Vector3 velocity = new Vector3(0, 0, orbitalVelocity);

            // Set the orbiting body properties
            orbiting.position = position;
            orbiting.velocity = velocity;
            
            // Position the GameObject to match
            orbitingBodies[i].transform.position = position;
            
            allBodies[i + 1] = orbiting;
        }

        // Start sim with keplerian physics
        simulation.centralBody = central;
        simulation.SetPhysicsModel(new KeplerMotion());
        // Uncomment if you want to use N-body physics instead
        // simulation.SetPhysicsModel(new NBodyPhysics());
  
        simulation.StartSimulation(allBodies);
    }
}


// using UnityEngine;
// using CelestialBodies.Config;


// public class PhysicsTestScript : MonoBehaviour
// {
//     //public CelestialBody celestialBody;

//     public GameObject centralBody;
//     public GameObject[] orbitingBodies;

//     public bodySimulation simulation;

//     [Header("Physics Properties")]
//     public float centralmass = 10f;
//     public float orbitDistance = 40f;
//     public float orbitSpeed = 5f;


//     private void Start()
//     {
//         if (!centralBody || orbitingBodies.Length == 0)
//         {
//             Debug.LogError("Central body or orbiting bodies not set");
//             return;
//         }
    
//         centralBody.transform.position = Vector3.zero; // Central body at world origin
//         CelestialBody central = FakeBodyWrapper(centralBody, Vector3.zero, Vector3.zero, centralmass);

//         // Setup all bodies array, add central body as first entry
//         CelestialBody[] allBodies = new CelestialBody[orbitingBodies.Length + 1];
//         allBodies[0] = central;

//         // Setup orbiting bodies
//         for (int i = 0; i < orbitingBodies.Length; i++)
//         {
//             // Test position with set distance
//             float dist = orbitDistance + i * 10f; // slight offset for spacing
            
//             Vector3 position = new Vector3(dist, 0, 0);

//             // Calculate proper orbital velocity for circular orbit
//             // v = sqrt(GM/r) where G is gravitational constant, M is central mass, r is distance
//             float orbitalVelocity = Mathf.Sqrt(centralmass * 6.674f / dist);
//             Vector3 velocity = new Vector3(0, 0, orbitalVelocity);

//             // No need to keep the return unless you want to use the CelestialBody later
//             CelestialBody orbiting = FakeBodyWrapper(orbitingBodies[i], position, velocity, 100f);
//             allBodies[i + 1] = orbiting;
//             //_ = FakeBodyWrapper(orbitingBodies[i], position, velocity, 1f);
//         }

// //        KeplerianPhysics.OrbitalElements.mu = centralmass;


//         // Start sim with keplerian physics
//         simulation.centralBody = central;
//         simulation.SetPhysicsModel(new KeplerMotion());
// //        simulation.SetPhysicsModel(new NBodyPhysics());
  
//         simulation.StartSimulation(allBodies);
//     }

//     private CelestialBody FakeBodyWrapper(GameObject go, Vector3 pos, Vector3 vel, float mass)
//     {

//         // Check for NaN values
//         if (float.IsNaN(pos.x) || float.IsNaN(pos.y) || float.IsNaN(pos.z))
//         {
//             Debug.LogError($"Invalid position detected for {go.name}. Using default position.");
//             pos = Vector3.zero;
//         }

//         if (float.IsNaN(vel.x) || float.IsNaN(vel.y) || float.IsNaN(vel.z))
//         {
//             Debug.LogError($"Invalid velocity detected for {go.name}. Using default velocity.");
//             vel = Vector3.zero;
//         }

//         mass = Mathf.Max(0.1f, mass); // Ensure mass is never zero or negative
        
//         go.transform.position = pos;

//         // Remove any existing components to avoid conflicts
//         var existingBody = go.GetComponent<CelestialBody>();
//         if (existingBody != null) DestroyImmediate(existingBody);
        
//         var existingGen = go.GetComponent<CelestialBodyGenerator>();
//         if (existingGen != null) DestroyImmediate(existingGen);

//         // Create and setup the body first
//         var body = go.AddComponent<CelestialBody>();
//         body.mass = mass;
//         body.position = pos;
//         body.velocity = vel;

//         // Create and setup the generator
//         var gen = go.AddComponent<CelestialBodyGenerator>();
//         body.celestiaBodyGenerator = gen;
//         gen.body = body;

//         // Create and setup the configs
//         var bodyConfig = ScriptableObject.CreateInstance<CelestialBodyConfig>();
//         bodyConfig.Init(CelestialBodyConfig.CelestialBodyType.Planet); // This should set up all required components

//         var physicsWrapper = ScriptableObject.CreateInstance<Physics>();
//         var settings = new Physics.PhysicsSettings();

//         settings.initialPosition = pos;
//         settings.initialVelocity = vel;

//         physicsWrapper.SetSettings(settings);

//         gen.bodyConfig = bodyConfig;
//         gen.bodyConfig.physics = physicsWrapper;

//         return body;
//     }
// }

        