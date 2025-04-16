using System.Collections.Generic;
using System.Net;
using CelestialBodies.Config;
using UnityEngine;

public class StarSystemManager : MonoBehaviour
{
    public enum PhysicsModelType
    {
        Keplerian,
        NBody,
        // Add more physics models here in the future
    }


    public static StarSystemManager Instance;

    [Header("Simulation")]
    public CelestialBodySimulation simulationController;
    public bool autoStartSimulation = false;

    [Header("System Configuration")]
    public StarSystemConfig systemConfig;
    public List<CelestialBody> systemBodies = new List<CelestialBody>();

    [Tooltip("Current physics model used for simulation")]
    public PhysicsModelType currentPhysicsModelType = PhysicsModelType.Keplerian;
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
        UpdatePhysicsModel();

        // Set the physics model on the simulation controller
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
    }

    /// <summary>
    /// Updates the physics model based on the selected model type
    /// </summary>
    private void UpdatePhysicsModel()
    {
        switch (currentPhysicsModelType)
        {
            case PhysicsModelType.Keplerian:
                currentPhysicsModel = new KeplerMotion();
                break;
            case PhysicsModelType.NBody:
                currentPhysicsModel = new NBodyPhysics();
                break;
            // Add cases for additional physics models here
            default:
                currentPhysicsModel = new KeplerMotion();
                break;
        }
    }

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
            Debug.Log("Simulation stopped");
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

    /// <summary>
    /// Switch to a different physics model
    /// </summary>
    /// <param name="modelType">The physics model type to use</param>
    public void SwitchPhysicsModel(PhysicsModelType modelType)
    {
        // Only switch if it's a different model
        if (currentPhysicsModelType != modelType)
        {
            currentPhysicsModelType = modelType;
            UpdatePhysicsModel();

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

            Debug.Log($"Switched physics model to {modelType}");
        }
    }

    /// <summary>
    /// Switch physics model by index (useful for UI dropdowns)
    /// </summary>
    /// <param name="index">Index of the physics model in the enum</param>
    public void SwitchPhysicsModelByIndex(int index)
    {
        if (index >= 0 && index < System.Enum.GetValues(typeof(PhysicsModelType)).Length)
        {
            PhysicsModelType newModel = (PhysicsModelType)index;
            SwitchPhysicsModel(newModel);
        }
        else
        {
            Debug.LogError($"Invalid physics model index: {index}");
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

        // Set the system config
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
            Debug.Log("System saved to config");
        }
    }

    /// <summary>
    /// Returns a list of available physics model names for UI display
    /// </summary>
    /// <returns>List of physics model names</returns>
    public static List<string> GetPhysicsModelNames()
    {
        List<string> modelNames = new List<string>();
        System.Array values = System.Enum.GetValues(typeof(PhysicsModelType));

        foreach (var value in values)
        {
            modelNames.Add(value.ToString());
        }

        return modelNames;
    }
}