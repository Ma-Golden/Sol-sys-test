using UnityEngine;
using UnityEngine.UI;
using CelestialBodies.Config;
using System.Collections.Generic;
using CelestialBodies.Config.Shading;

public class SystemCreator : MonoBehaviour
{
    [Header("References")]
    public Transform bodiesContainer;
    public GameObject bodyPrefab;
    public Dropdown templateDropdown;
    public Button createButton;
    public Button clearButton;

    [Header("Star Settings")]
    public float defaultStarMass = 100f;
    public float defaultStarRadius = 10f;
    public Color defaultStarColor = Color.yellow;

    private Dictionary<string, System.Action> systemTemplates = new Dictionary<string, System.Action>();

    void Start()
    {
        // Initialize dropdown
        InitializeDropdown();

        // Set up button listeners
        if (createButton != null)
            createButton.onClick.AddListener(OnCreateButtonPressed);

        if (clearButton != null)
            clearButton.onClick.AddListener(OnClearButtonPressed);
    }

    private void InitializeDropdown()
    {
        if (templateDropdown == null)
            return;

        // Register system templates
        RegisterSystemTemplates();

        // Clear existing options
        templateDropdown.ClearOptions();

        // Add template names to dropdown
        List<string> options = new List<string>(systemTemplates.Keys);
        templateDropdown.AddOptions(options);
    }

    private void RegisterSystemTemplates()
    {
        // Add various solar system templates
        systemTemplates.Add("Basic Solar System", CreateBasicSolarSystem);
        systemTemplates.Add("Two-Planet System", CreateTwoPlanetSystem);
        systemTemplates.Add("Binary Star System", CreateBinaryStarSystem);
        systemTemplates.Add("Planet with Moon", CreatePlanetWithMoon);
        systemTemplates.Add("Four-Planet System", CreateFourPlanetSystem);
    }

    private void OnCreateButtonPressed()
    {
        if (templateDropdown == null || templateDropdown.options.Count == 0)
            return;

        string selectedTemplate = templateDropdown.options[templateDropdown.value].text;

        if (systemTemplates.ContainsKey(selectedTemplate))
        {
            // Clear existing system
            ClearSystem();

            // Create selected template
            systemTemplates[selectedTemplate]?.Invoke();

            Debug.Log($"Created {selectedTemplate}");
        }
    }

    private void OnClearButtonPressed()
    {
        ClearSystem();
    }

    private void ClearSystem()
    {
        if (StarSystemManager.Instance != null)
        {
            StarSystemManager.Instance.StopSimulation();
        }

        // Clear bodies container
        if (bodiesContainer != null)
        {
            foreach (Transform child in bodiesContainer)
            {
                Destroy(child.gameObject);
            }
        }

        // Clear system in manager
        if (StarSystemManager.Instance != null)
        {
            var systemConfig = ScriptableObject.CreateInstance<StarSystemConfig>();
            StarSystemManager.Instance.LoadSystemConfig(systemConfig);
        }

        Debug.Log("System cleared");
    }

    #region System Templates

    private void CreateBasicSolarSystem()
    {
        // Create a star
        CelestialBody star = CreateStar("Sun", defaultStarMass, defaultStarRadius, defaultStarColor, Vector3.zero, Vector3.zero);

        // Create Earth-like planet
        float earthDistance = 50f;
        float earthMass = 1f;
        float earthRadius = 3f;

        var (earthPos, earthVel) = OrbitalCalculator.CalculateOrbitPositionVelocity(
            star, earthDistance, 0f, 0f, earthMass, earthRadius);

        CelestialBody earth = CreatePlanet("Earth", earthMass, earthRadius, Color.blue, earthPos, earthVel);

        // Set star as central body
        if (StarSystemManager.Instance != null)
        {
            StarSystemManager.Instance.SetCentralBody(star);
        }
    }

    private void CreateTwoPlanetSystem()
    {
        // Create a star
        CelestialBody star = CreateStar("Sun", defaultStarMass, defaultStarRadius, defaultStarColor, Vector3.zero, Vector3.zero);

        // Create inner planet
        float innerDistance = 30f;
        float innerMass = 0.8f;
        float innerRadius = 2.5f;

        var (innerPos, innerVel) = OrbitalCalculator.CalculateOrbitPositionVelocity(
            star, innerDistance, 0f, 0f, innerMass, innerRadius);

        CelestialBody innerPlanet = CreatePlanet("Mercury", innerMass, innerRadius, new Color(0.8f, 0.6f, 0.4f), innerPos, innerVel);

        // Create outer planet
        float outerDistance = 80f;
        float outerMass = 1.2f;
        float outerRadius = 3.5f;

        var (outerPos, outerVel) = OrbitalCalculator.CalculateOrbitPositionVelocity(
            star, outerDistance, 0f, 180f, outerMass, outerRadius);

        CelestialBody outerPlanet = CreatePlanet("Jupiter", outerMass, outerRadius, new Color(0.8f, 0.7f, 0.6f), outerPos, outerVel);

        // Set star as central body
        if (StarSystemManager.Instance != null)
        {
            StarSystemManager.Instance.SetCentralBody(star);
        }
    }

    private void CreateBinaryStarSystem()
    {
        // Create primary star
        float primaryMass = defaultStarMass;
        float primaryRadius = defaultStarRadius;
        CelestialBody primaryStar = CreateStar("Alpha", primaryMass, primaryRadius, Color.yellow, new Vector3(-20f, 0, 0), new Vector3(0, 0, -2f));

        // Create secondary star
        float secondaryMass = defaultStarMass * 0.7f;
        float secondaryRadius = defaultStarRadius * 0.8f;
        CelestialBody secondaryStar = CreateStar("Beta", secondaryMass, secondaryRadius, new Color(1f, 0.6f, 0.4f), new Vector3(20f, 0, 0), new Vector3(0, 0, 2f));

        // Create distant planet (orbiting the barycenter)
        float planetDistance = 100f;
        float planetMass = 1f;
        float planetRadius = 3f;

        // Approximate the barycenter (weighted average of star positions)
        float totalMass = primaryMass + secondaryMass;
        Vector3 barycenter = (primaryStar.position * primaryMass + secondaryStar.position * secondaryMass) / totalMass;

        CelestialBody planet = CreatePlanet("Tatooine", planetMass, planetRadius, Color.gray,
            barycenter + new Vector3(planetDistance, 0, 0), new Vector3(0, 0, 1.5f));

        // Note: For a true binary star simulation, you'd need custom physics logic
        // Here we're just creating a simplified visual representation

        // Set primary star as central reference
        if (StarSystemManager.Instance != null)
        {
            StarSystemManager.Instance.SetCentralBody(primaryStar);
        }
    }

    private void CreatePlanetWithMoon()
    {
        // Create a star
        CelestialBody star = CreateStar("Sun", defaultStarMass, defaultStarRadius, defaultStarColor, Vector3.zero, Vector3.zero);

        // Create planet
        float planetDistance = 60f;
        float planetMass = 5f;
        float planetRadius = 4f;

        var (planetPos, planetVel) = OrbitalCalculator.CalculateOrbitPositionVelocity(
            star, planetDistance, 0f, 0f, planetMass, planetRadius);

        CelestialBody planet = CreatePlanet("Planet", planetMass, planetRadius, Color.blue, planetPos, planetVel);

        // Create moon
        float moonDistance = 10f;
        float moonMass = 0.1f;
        float moonRadius = 1f;

        var (moonPos, moonVel) = OrbitalCalculator.SetupMoonOrbit(planet, moonDistance, moonMass, moonRadius, 45f);

        // Adjust moon velocity to account for planet's velocity
        moonVel += planetVel;

        CelestialBody moon = CreateMoon("Moon", moonMass, moonRadius, Color.gray, moonPos, moonVel);

        // Set star as central body
        if (StarSystemManager.Instance != null)
        {
            StarSystemManager.Instance.SetCentralBody(star);
        }
    }

    private void CreateFourPlanetSystem()
    {
        // Create a star
        CelestialBody star = CreateStar("Sun", defaultStarMass, defaultStarRadius, defaultStarColor, Vector3.zero, Vector3.zero);

        // Create planets at different distances
        CreatePlanetAtDistance(star, 30f, 0.7f, 2f, new Color(0.7f, 0.5f, 0.3f), "Mercury", 0f);
        CreatePlanetAtDistance(star, 50f, 0.9f, 2.5f, new Color(0.9f, 0.7f, 0.4f), "Venus", 90f);
        CreatePlanetAtDistance(star, 70f, 1f, 3f, new Color(0.2f, 0.4f, 0.8f), "Earth", 180f);
        CreatePlanetAtDistance(star, 100f, 0.8f, 2.8f, new Color(0.8f, 0.3f, 0.2f), "Mars", 270f);

        // Set star as central body
        if (StarSystemManager.Instance != null)
        {
            StarSystemManager.Instance.SetCentralBody(star);
        }
    }

    private void CreatePlanetAtDistance(CelestialBody star, float distance, float mass, float radius, Color color, string name, float angle)
    {
        var (pos, vel) = OrbitalCalculator.CalculateOrbitPositionVelocity(
            star, distance, 0f, angle, mass, radius);

        CreatePlanet(name, mass, radius, color, pos, vel);
    }

    #endregion

    #region Helper Methods

    private CelestialBody CreateStar(string name, float mass, float radius, Color color, Vector3 position, Vector3 velocity)
    {
        GameObject starObject = CreateBodyObject(name);

        // Add CelestialBody component
        CelestialBody body = starObject.AddComponent<CelestialBody>();
        body.mass = mass;
        body.position = position;
        body.velocity = velocity;

        // Add CelestialBodyGenerator component
        CelestialBodyGenerator generator = starObject.AddComponent<CelestialBodyGenerator>();
        generator.body = body;
        body.celestiaBodyGenerator = generator;

        // Create star config
        CelestialBodyConfig bodyConfig = ScriptableObject.CreateInstance<CelestialBodyConfig>();
        bodyConfig.Init(CelestialBodyConfig.CelestialBodyType.Star);
        bodyConfig.radius = radius;

        // Create physics config
        Physics physicsConfig = ScriptableObject.CreateInstance<Physics>();
        Physics.PhysicsSettings physicsSettings = new Physics.PhysicsSettings();
        physicsSettings.initialPosition = position;
        physicsSettings.initialVelocity = velocity;
        physicsConfig.SetSettings(physicsSettings);
        bodyConfig.physics = physicsConfig;

        // Apply features from SystemSavingUtils
        if (SystemSavingUtils.Instance != null)
        {
            var (shape, shading, ocean, physics) = SystemSavingUtils.Instance.CreateFeatures(bodyConfig.bodyType);
            bodyConfig.shape = shape;
            bodyConfig.shading = shading;
            bodyConfig.ocean = ocean;

            // Customize star color
            if (shading != null && shading is StarShading starShading)
            {
                // This assumes StarShading has a method to set color
                // If not, you'll need to adapt this based on your implementation
                // starShading.SetColor(color);
            }
        }

        // Set config
        generator.bodyConfig = bodyConfig;

        // Initialize generator
        generator.OnInitialUpdate();

        // Add to system
        if (StarSystemManager.Instance != null)
        {
            StarSystemManager.Instance.AddBody(body);
        }

        return body;
    }

    private CelestialBody CreatePlanet(string name, float mass, float radius, Color color, Vector3 position, Vector3 velocity)
    {
        GameObject planetObject = CreateBodyObject(name);

        // Add CelestialBody component
        CelestialBody body = planetObject.AddComponent<CelestialBody>();
        body.mass = mass;
        body.position = position;
        body.velocity = velocity;

        // Add CelestialBodyGenerator component
        CelestialBodyGenerator generator = planetObject.AddComponent<CelestialBodyGenerator>();
        generator.body = body;
        body.celestiaBodyGenerator = generator;

        // Create planet config
        CelestialBodyConfig bodyConfig = ScriptableObject.CreateInstance<CelestialBodyConfig>();
        bodyConfig.Init(CelestialBodyConfig.CelestialBodyType.Planet);
        bodyConfig.radius = radius;

        // Create physics config
        Physics physicsConfig = ScriptableObject.CreateInstance<Physics>();
        Physics.PhysicsSettings physicsSettings = new Physics.PhysicsSettings();
        physicsSettings.initialPosition = position;
        physicsSettings.initialVelocity = velocity;
        physicsConfig.SetSettings(physicsSettings);
        bodyConfig.physics = physicsConfig;

        // Apply features from SystemSavingUtils
        if (SystemSavingUtils.Instance != null)
        {
            var (shape, shading, ocean, physics) = SystemSavingUtils.Instance.CreateFeatures(bodyConfig.bodyType);
            bodyConfig.shape = shape;
            bodyConfig.shading = shading;
            bodyConfig.ocean = ocean;

            // Customize planet color
            if (shading != null && shading is PlanetShading planetShading)
            {
                // This assumes PlanetShading has a method to set color
                // If not, you'll need to adapt this based on your implementation
                // planetShading.SetColor(color);
            }
        }

        // Set config
        generator.bodyConfig = bodyConfig;

        // Initialize generator
        generator.OnInitialUpdate();

        // Add to system
        if (StarSystemManager.Instance != null)
        {
            StarSystemManager.Instance.AddBody(body);
        }

        return body;
    }

    private CelestialBody CreateMoon(string name, float mass, float radius, Color color, Vector3 position, Vector3 velocity)
    {
        GameObject moonObject = CreateBodyObject(name);

        // Add CelestialBody component
        CelestialBody body = moonObject.AddComponent<CelestialBody>();
        body.mass = mass;
        body.position = position;
        body.velocity = velocity;

        // Add CelestialBodyGenerator component
        CelestialBodyGenerator generator = moonObject.AddComponent<CelestialBodyGenerator>();
        generator.body = body;
        body.celestiaBodyGenerator = generator;

        // Create moon config
        CelestialBodyConfig bodyConfig = ScriptableObject.CreateInstance<CelestialBodyConfig>();
        bodyConfig.Init(CelestialBodyConfig.CelestialBodyType.Moon);
        bodyConfig.radius = radius;

        // Create physics config
        Physics physicsConfig = ScriptableObject.CreateInstance<Physics>();
        Physics.PhysicsSettings physicsSettings = new Physics.PhysicsSettings();
        physicsSettings.initialPosition = position;
        physicsSettings.initialVelocity = velocity;
        physicsConfig.SetSettings(physicsSettings);
        bodyConfig.physics = physicsConfig;

        // Apply features from SystemSavingUtils
        if (SystemSavingUtils.Instance != null)
        {
            var (shape, shading, ocean, physics) = SystemSavingUtils.Instance.CreateFeatures(bodyConfig.bodyType);
            bodyConfig.shape = shape;
            bodyConfig.shading = shading;
            bodyConfig.ocean = ocean;

            // Customize moon color
            if (shading != null && shading is MoonShading moonShading)
            {
                // This assumes MoonShading has a method to set color
                // If not, you'll need to adapt this based on your implementation
                // moonShading.SetColor(color);
            }
        }

        // Set config
        generator.bodyConfig = bodyConfig;

        // Initialize generator
        generator.OnInitialUpdate();

        // Add to system
        if (StarSystemManager.Instance != null)
        {
            StarSystemManager.Instance.AddBody(body);
        }

        return body;
    }

    private GameObject CreateBodyObject(string name)
    {
        GameObject bodyObject = null;

        if (bodyPrefab != null)
        {
            bodyObject = Instantiate(bodyPrefab, Vector3.zero, Quaternion.identity, bodiesContainer);
        }
        else
        {
            bodyObject = new GameObject();
            bodyObject.transform.SetParent(bodiesContainer);
        }

        bodyObject.name = name;
        return bodyObject;
    }

    #endregion
}