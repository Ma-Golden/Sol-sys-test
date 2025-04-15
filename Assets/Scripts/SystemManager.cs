using System.Collections.Generic;
using System.Net;
using CelestialBodies.Config;
using UnityEngine;

public class StarSystemManager : MonoBehaviour
{
    public static StarSystemManager Instance;


    [Header("Simulation")]
    public bodySimulation simulationController;
    public bool autoStartSimulation = false;
    
    [Header("System Configuration")]
    public StarSystemConfig systemConfig;
    private List<CelestialBody> systemBodies = new List<CelestialBody>();

    [Header("Physics Model")]
    public bool useKeplerianPhysics = true;
    private IPhysicsModel currentPhysicsModel;


    [Header("Reference Body")]
    public bool simulateRelativeToStar = true;
    public CelestialBody centralStar;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void Start()
    {
        // Initialize physics model based on selection
        currentPhysicsModel = useKeplerianPhysics ? new KeplerMotion() : new NBodyPhysics();

        // Set the physcis model on the simulation controller
        if (simulationController != null)
        {
            simulationController.SetPhysicsModel(currentPhysicsModel); // update physics model
            simulationController.relativeToBody = simulateRelativeToStar;

            if (centralStar != null)
            {
                simulationController.centralBody = centralStar;
            }
        }

        // Auto-start if enabled
        if (autoStartSimulation && systemBodies.Count > 0)
        {
            StartSimulation();
        }
    } // Start ()

    public void AddBody(CelestialBody body)
    {
        if (body != null && !systemBodies.Contains(body))
        {
            systemBodies.Add(body);
            Debug.Log($"Added {body.name} to system. Total bodies: {systemBodies.Count}");
        }
    }

    public void RemoveBody(CelestialBody body)
    {
        if (systemBodies.Contains(body))
        {
            systemBodies.Remove(body);
            Debug.Log($"Removed {body.name} from system. Total bodies: {systemBodies.Count}");
        }
    }

    public void SetCentralBody(CelestialBody body)
    {
        centralStar = body;
        if (simulationController != null)
        {
            simulationController.centralBody = body;
        }
    }

    public void StartSimulation()
    {

        if (simulationController != null && systemBodies.Count > 0)
        {
            // Configure simulation
            simulationController.SetPhysicsModel(currentPhysicsModel);
            simulationController.relativeToBody = simulateRelativeToStar;
            simulationController.centralBody = centralStar;

            // Convert to array for simulation
            CelestialBody[] bodiesArray = systemBodies.ToArray();

            // Start simulation
            simulationController.StartSimulation(bodiesArray);
            simulationController.simulating = true;

            Debug.Log($"Started simulation with {systemBodies.Count} bodies");
        }
        else 
        {
            Debug.LogWarning("Cannot start simulation: No bodies in system or simulation controller not assigned");
        }
    }

    public void StopSimulation()
    {
        if (simulationController != null)
        {
            simulationController.StopSimulation();
            simulationController.simulating = false;
            Debug.Log("Simulation stoppped");
        }
    }

    public void ToggleSimulation()
    {
        if (simulationController.simulating)
        {
            StopSimulation();
        }
        else
        {
            StartSimulation();
        }
    }

    // TODO: change this to multiple other possible models

    public void SwitchPhysicsModel(bool useKepler)
    {
        useKeplerianPhysics = useKepler;
        currentPhysicsModel = useKeplerianPhysics ? new KeplerMotion() : new NBodyPhysics();

        if (simulationController != null)
        {
            simulationController.SetPhysicsModel(currentPhysicsModel);

            // If simulation is running, restart it with new physics
            if (simulationController.simulating)
            {
                StopSimulation();
                StartSimulation();
            }
        }
    }

    public void LoadSystemConfig(StarSystemConfig config)
    {
        if (config == null)
        {
            Debug.LogError("Cannot load null system config");
            return;
        }

        // Clear existing bodies
        systemBodies.Clear();
        StopSimulation();

        // Set the sytem config
        systemConfig = config;

        // TODO: Instantiate bodies from config
        // This would involve creating GameObjects and adding CelestialBody components
        // based on the configuration in the StarSystemConfig

        Debug.Log("System loaded from config");
    }

    public void SaveSystemConfig()
    {
        if (SystemSavingUtils.Instance != null)
        {
            // Create new config if none exists
            if (systemConfig == null)
            {
                systemConfig = ScriptableObject.CreateInstance<StarSystemConfig>();
            }

            // TODO: Save all body configurations to the system config
            // Extract all relevant data from each Celestialbody
            // and its CelestialBody generator and creating appropriate config objects

            SystemSavingUtils.Instance.currentSystemConfig = systemConfig;
            Debug.Log("System saved to config");;
        }
    }
}

