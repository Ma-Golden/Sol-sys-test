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
    public float orbitSpeed = 1f;


    private void Start()
    {
    
        // CONTINUE FROM HERE
        // CONTINUE FROM HERE
        // CONTINUE FROM HERE

        // NEED TO SET PARAMS IN THE EDITOR
        
        if (!centralBody || orbitingBodies.Length == 0)
        {
            Debug.LogError("Central body or orbiting bodies not set");
            return;
        }
    
        centralBody.transform.position = Vector3.zero;
        CelestialBody central = FakeBodyWrapper(centralBody, Vector3.zero, Vector3.zero, centralmass);

        // Setup orbiting bodies
        for (int i = 0; i < orbitingBodies.Length; i++)
        {
            float dist = orbitDistance + i * 5f; // slight offset for spacing
            Vector3 position = new Vector3(dist, 0, 0);
            Vector3 velocity = new Vector3(0, orbitSpeed / Mathf.Sqrt(i + 1), 0);

            // No need to keep the return unless you want to use the CelestialBody later
            _ = FakeBodyWrapper(orbitingBodies[i], position, velocity, 1f);
        }

        KeplerianPhysics.OrbitalElements.mu = centralmass;


        // Start sim with keplerian physics
        simulation.centralBody = central;
        simulation.SetPhysicsModel(new KeplerianPhysics());
        simulation.StartSimulation();
    }


    private CelestialBody FakeBodyWrapper(GameObject go, Vector3 pos, Vector3 vel, float mass)
    {
        go.transform.position = pos;

        var body = go.AddComponent<CelestialBody>();
        var gen = go.AddComponent<CelestialBodyGenerator>();

        body.celestiaBodyGenerator = gen;
        body.mass = mass;

        gen.body = body;
        gen.bodyConfig = ScriptableObject.CreateInstance<CelestialBodyConfig>();

        var physicsWrapper = new Physics();
        var settings = new Physics.PhysicsSettings();

        settings.initialPosition = pos;
        settings.initialVelocity = vel;

        physicsWrapper.SetSettings(settings);
        gen.bodyConfig.physics = physicsWrapper;
        
        return body;
    }
}