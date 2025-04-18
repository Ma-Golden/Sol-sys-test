using UnityEngine;
using TMPro; // TextMeshPro namespace
using CelestialBodies.Config;
using System.Collections.Generic;
using CelestialBodies.Config.Shape;
using CelestialBodies.Config.Shading;
using UnityEngine.UI; // For Button

public class BodyEditorPanel : MonoBehaviour
{
    [Header("Body Creation")]
    public TMP_Dropdown bodyTypeDropdown;
    public Button createBodyButton;
    public Button deselectBodyButton;
    public Button deleteBodyButton;

    [Header("Body Properties")]
    public TMP_InputField nameInput;
    public TMP_InputField massInput;
    public TMP_InputField radiusInput;
    public TMP_InputField positionXInput;
    public TMP_InputField positionYInput;
    public TMP_InputField positionZInput;
    public TMP_InputField velocityXInput;
    public TMP_InputField velocityYInput;
    public TMP_InputField velocityZInput;

    [Header("Body Appearance")]
    public Button randomizeShapeButton;
    public Button randomizeShadingButton;
    public Button randomizeBothButton;

    [Header("Generation")]
    public GameObject bodyPrefab;
    public Transform bodiesContainer;

    // Currently selected body
    private CelestialBody selectedBody;
    private CelestialBodyGenerator selectedGenerator;

    private void Start()
    {
        InitializeDropdowns();
        SetupListeners();
    }

    private void InitializeDropdowns()
    {
        // Initialize body type dropdown
        if (bodyTypeDropdown != null)
        {
            bodyTypeDropdown.ClearOptions();

            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>
            {
                new TMP_Dropdown.OptionData("Planet"),
                new TMP_Dropdown.OptionData("Moon"),
                new TMP_Dropdown.OptionData("Star")
            };

            bodyTypeDropdown.AddOptions(options);
        }
    }

    private void SetupListeners()
    {
        if (bodyTypeDropdown != null)
            bodyTypeDropdown.onValueChanged.AddListener(OnBodyTypeChanged);

        if (createBodyButton != null)
            createBodyButton.onClick.AddListener(CreateNewBody);

        if (deselectBodyButton != null)
            deselectBodyButton.onClick.AddListener(DeselectBody);

        if (deleteBodyButton != null)
            deleteBodyButton.onClick.AddListener(DeleteSelectedBody);

        if (randomizeShapeButton != null)
            randomizeShapeButton.onClick.AddListener(RandomizeShape);

        if (randomizeShadingButton != null)
            randomizeShadingButton.onClick.AddListener(RandomizeShading);

        if (randomizeBothButton != null)
            randomizeBothButton.onClick.AddListener(() => {
                RandomizeShape();
                RandomizeShading();
            });

        // Set up input field listeners
        SetupInputFieldListeners();
    }

    private void SetupInputFieldListeners()
    {
        // Name
        if (nameInput != null)
            nameInput.onEndEdit.AddListener(UpdateBodyName);

        // Mass and radius
        if (massInput != null)
            massInput.onEndEdit.AddListener(value => UpdateBodyParameter("mass", value));

        if (radiusInput != null)
            radiusInput.onEndEdit.AddListener(value => UpdateBodyParameter("radius", value));

        // Position
        if (positionXInput != null)
            positionXInput.onEndEdit.AddListener(value => UpdateBodyParameter("position.x", value));

        if (positionYInput != null)
            positionYInput.onEndEdit.AddListener(value => UpdateBodyParameter("position.y", value));

        if (positionZInput != null)
            positionZInput.onEndEdit.AddListener(value => UpdateBodyParameter("position.z", value));

        // Velocity
        if (velocityXInput != null)
            velocityXInput.onEndEdit.AddListener(value => UpdateBodyParameter("velocity.x", value));

        if (velocityYInput != null)
            velocityYInput.onEndEdit.AddListener(value => UpdateBodyParameter("velocity.y", value));

        if (velocityZInput != null)
            velocityZInput.onEndEdit.AddListener(value => UpdateBodyParameter("velocity.z", value));
    }

    public void SelectBody(CelestialBody body)
    {
        selectedBody = body;

        if (selectedBody == null)
        {
            Debug.LogError("SelectBody called with null body");
            DeselectBody();
            return;
        }

        selectedGenerator = body.GetComponent<CelestialBodyGenerator>();

        if (selectedGenerator == null)
        {
            Debug.LogError($"Selected body {body.name} has no CelestialBodyGenerator component");
            DeselectBody();
            return;
        }

        if (selectedGenerator.bodyConfig == null)
        {
            Debug.LogError($"Selected body {body.name} has generator but null bodyConfig");
            DeselectBody();
            return;
        }

        gameObject.SetActive(true);
        UpdateUI();
    }

    private void DeselectBody()
    {
        selectedBody = null;
        selectedGenerator = null;

        // Clear input fields
        if (nameInput != null) nameInput.text = "";
        if (massInput != null) massInput.text = "";
        if (radiusInput != null) radiusInput.text = "";
        if (positionXInput != null) positionXInput.text = "";
        if (positionYInput != null) positionYInput.text = "";
        if (positionZInput != null) positionZInput.text = "";
        if (velocityXInput != null) velocityXInput.text = "";
        if (velocityYInput != null) velocityYInput.text = "";
        if (velocityZInput != null) velocityZInput.text = "";
    }

    private void UpdateUI()
    {
        if (selectedBody == null || selectedGenerator == null || selectedGenerator.bodyConfig == null)
            return;

        // Set body type dropdown
        if (bodyTypeDropdown != null)
            bodyTypeDropdown.value = (int)selectedGenerator.bodyConfig.bodyType;

        // Set name
        if (nameInput != null)
            nameInput.text = selectedBody.name;

        // Set mass and radius
        if (massInput != null)
            massInput.text = selectedBody.mass.ToString("F2");

        if (radiusInput != null)
            radiusInput.text = selectedGenerator.bodyConfig.radius.ToString("F2");

        // Set position
        if (positionXInput != null)
            positionXInput.text = selectedBody.position.x.ToString("F2");

        if (positionYInput != null)
            positionYInput.text = selectedBody.position.y.ToString("F2");

        if (positionZInput != null)
            positionZInput.text = selectedBody.position.z.ToString("F2");

        // Set velocity
        if (velocityXInput != null)
            velocityXInput.text = selectedBody.velocity.x.ToString("F2");

        if (velocityYInput != null)
            velocityYInput.text = selectedBody.velocity.y.ToString("F2");

        if (velocityZInput != null)
            velocityZInput.text = selectedBody.velocity.z.ToString("F2");
    }

    private void UpdateBodyName(string newName)
    {
        if (selectedBody == null || string.IsNullOrEmpty(newName))
            return;

        selectedBody.name = newName;
        selectedBody.gameObject.name = newName;
    }

    private void UpdateBodyParameter(string parameter, string value)
    {
        if (selectedBody == null || selectedGenerator == null)
            return;

        float floatValue;
        if (!float.TryParse(value, out floatValue))
            return;

        switch (parameter)
        {
            case "mass":
                selectedBody.mass = floatValue;
                break;

            case "radius":
                selectedGenerator.bodyConfig.radius = floatValue;
                selectedGenerator.OnShapeUpdate();
                break;

            case "position.x":
                Vector3 pos = selectedBody.position;
                pos.x = floatValue;
                selectedBody.position = pos;
                UpdatePhysicsPosition(pos);
                break;

            case "position.y":
                pos = selectedBody.position;
                pos.y = floatValue;
                selectedBody.position = pos;
                UpdatePhysicsPosition(pos);
                break;

            case "position.z":
                pos = selectedBody.position;
                pos.z = floatValue;
                selectedBody.position = pos;
                UpdatePhysicsPosition(pos);
                break;

            case "velocity.x":
                Vector3 vel = selectedBody.velocity;
                vel.x = floatValue;
                selectedBody.velocity = vel;
                UpdatePhysicsVelocity(vel);
                break;

            case "velocity.y":
                vel = selectedBody.velocity;
                vel.y = floatValue;
                selectedBody.velocity = vel;
                UpdatePhysicsVelocity(vel);
                break;

            case "velocity.z":
                vel = selectedBody.velocity;
                vel.z = floatValue;
                selectedBody.velocity = vel;
                UpdatePhysicsVelocity(vel);
                break;
        }

        // Apply changes to transform
        selectedBody.transform.position = selectedBody.position;
    }

    private void UpdatePhysicsPosition(Vector3 position)
    {
        if (selectedGenerator.bodyConfig.physics != null)
        {
            Physics.PhysicsSettings settings = selectedGenerator.bodyConfig.physics.GetPhysicalConfig();
            settings.initialPosition = position;
            selectedGenerator.bodyConfig.physics.SetSettings(settings);
            selectedGenerator.OnPhysicsUpdate();
        }
    }

    private void UpdatePhysicsVelocity(Vector3 velocity)
    {
        if (selectedGenerator.bodyConfig.physics != null)
        {
            Physics.PhysicsSettings settings = selectedGenerator.bodyConfig.physics.GetPhysicalConfig();
            settings.initialVelocity = velocity;
            selectedGenerator.bodyConfig.physics.SetSettings(settings);
            selectedGenerator.OnPhysicsUpdate();
        }
    }

    private void OnBodyTypeChanged(int index)
    {
        if (selectedGenerator == null || selectedGenerator.bodyConfig == null)
            return;

        CelestialBodyConfig.CelestialBodyType bodyType = (CelestialBodyConfig.CelestialBodyType)index;
        selectedGenerator.bodyConfig.bodyType = bodyType;

        // Update features based on body type using SystemSavingUtils
        if (SystemSavingUtils.Instance != null)
        {
            var (shape, shading, ocean, physics) = SystemSavingUtils.Instance.CreateFeatures(bodyType);

            // Apply new features to config
            selectedGenerator.bodyConfig.shape = shape;
            selectedGenerator.bodyConfig.shading = shading;
            selectedGenerator.bodyConfig.ocean = ocean;

            // Don't override physics if already set
            if (selectedGenerator.bodyConfig.physics == null)
            {
                selectedGenerator.bodyConfig.physics = physics;
            }

            // Trigger updates
            selectedGenerator.OnShapeUpdate();
            selectedGenerator.OnShadingUpdate();
        }
    }

    private void RandomizeShape()
    {
        if (selectedGenerator == null || selectedGenerator.bodyConfig == null ||
            selectedGenerator.bodyConfig.shape == null)
            return;

        Shape.ShapeConfig shapeConfig = selectedGenerator.bodyConfig.shape.GetConfig();

        // Randomize shape parameters
        shapeConfig.perturbStrength = Random.Range(0.1f, 2.0f);
        
        // TODO: fix this
        // shapeConfig.perturbScale = Random.Range(1.0f, 5.0f);  // Fixed variable name
        // shapeConfig.noiseStrength = Random.Range(0.01f, 0.3f); // Added missing property
        
        
        shapeConfig.perturbVertices = Random.value > 0.3f; // 70% chance of true

        selectedGenerator.bodyConfig.shape.SetConfig(shapeConfig);
        selectedGenerator.OnShapeUpdate();

        Debug.Log("Randomized shape for " + selectedBody.name);
    }

    private void RandomizeShading()
    {
        if (selectedGenerator == null || selectedGenerator.bodyConfig == null ||
            selectedGenerator.bodyConfig.shading == null)
            return;

        // Generic randomization that should work for most cases
        switch (selectedGenerator.bodyConfig.bodyType)
        {
            case CelestialBodyConfig.CelestialBodyType.Planet:
                if (selectedGenerator.bodyConfig.shading is PlanetShading planetShading)
                {
                    // Randomize planet-specific shading properties
                    var shadingConfig = planetShading.GetConfig();
                    shadingConfig.mainColor = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);
                    planetShading.SetConfig(shadingConfig);
                }
                break;

            case CelestialBodyConfig.CelestialBodyType.Moon:
                if (selectedGenerator.bodyConfig.shading is MoonShading moonShading)
                {
                    // Randomize moon-specific shading properties
                    var shadingConfig = moonShading.GetConfig();
                    shadingConfig.mainColor = Random.ColorHSV(0f, 1f, 0.1f, 0.5f, 0.3f, 0.7f);
                    moonShading.SetConfig(shadingConfig);
                }
                break;

            case CelestialBodyConfig.CelestialBodyType.Star:
                if (selectedGenerator.bodyConfig.shading is StarShading starShading)
                {
                    // Randomize star-specific shading properties
                    var shadingConfig = starShading.GetConfig();
                    shadingConfig.mainColor = Random.ColorHSV(0f, 0.2f, 0.8f, 1f, 0.8f, 1f);
                    starShading.SetConfig(shadingConfig);
                }
                break;
        }

        selectedGenerator.OnShadingUpdate();
        Debug.Log("Randomized shading for " + selectedBody.name);
    }

private void CreateNewBody()
{
    if (bodiesContainer == null)
    {
        Debug.LogError("Bodies container not assigned!");
        return;
    }

    // Get the StarSystemConfig from SystemSavingUtils
    StarSystemConfig systemConfig = null;
    if (SystemSavingUtils.Instance != null)
    {
        systemConfig = SystemSavingUtils.Instance.currentSystemConfig;
        if (systemConfig == null)
        {
            systemConfig = ScriptableObject.CreateInstance<StarSystemConfig>();
            SystemSavingUtils.Instance.currentSystemConfig = systemConfig;
        }
    }
    else
    {
        Debug.LogError("SystemSavingUtils.Instance is null!");
        return;
    }

    // Get selected body type from dropdown
    CelestialBodyConfig.CelestialBodyType bodyType = CelestialBodyConfig.CelestialBodyType.Planet;
    if (bodyTypeDropdown != null)
    {
        bodyType = (CelestialBodyConfig.CelestialBodyType)bodyTypeDropdown.value;
    }

    // Create a unique name for the new body
    string bodyName = bodyType.ToString() + "_" + System.Guid.NewGuid().ToString().Substring(0, 6);

    // Add new body settings to the system config
    int bodyIndex = systemConfig.AddNewCelestialBodySettings(bodyType);
    CelestialBodyConfig bodyConfig = systemConfig.celestialBodyConfigs[bodyIndex];

    // Create a new GameObject
    GameObject newBodyObject = new GameObject(bodyName);
    newBodyObject.transform.SetParent(bodiesContainer);
    newBodyObject.transform.localPosition = Vector3.zero;
    newBodyObject.transform.localRotation = Quaternion.identity;
    newBodyObject.transform.localScale = Vector3.one;

    // Add collider for selection
    SphereCollider collider = newBodyObject.AddComponent<SphereCollider>();
    collider.radius = 1f;

    // Add CelestialBody component
    CelestialBody newBody = newBodyObject.AddComponent<CelestialBody>();
    newBody.mass = 1.0f;
    
    // Get position from physics config or use default
    Vector3 position = Vector3.zero;
    Vector3 velocity = Vector3.zero;
    
    if (bodyConfig.physics != null && bodyConfig.physics.GetPhysicalConfig() != null)
    {
        position = bodyConfig.physics.GetPhysicalConfig().initialPosition;
        velocity = bodyConfig.physics.GetPhysicalConfig().initialVelocity;
    }
    
    newBody.position = position;
    newBody.velocity = velocity;
    newBody.transform.position = position;

    // Get reference to the generator that was automatically added in CelestialBody.Awake()
    CelestialBodyGenerator generator = newBody.celestiaBodyGenerator;
    
    // Set config reference
    generator.bodyConfig = bodyConfig;

    // Apply features if not already set
    if (bodyConfig.shape == null || bodyConfig.shading == null || bodyConfig.ocean == null)
    {
        if (SystemSavingUtils.Instance != null)
        {
            var (shape, shading, ocean, physics) = SystemSavingUtils.Instance.CreateFeatures(bodyType);
            
            if (bodyConfig.shape == null) bodyConfig.shape = shape;
            if (bodyConfig.shading == null) bodyConfig.shading = shading;
            if (bodyConfig.ocean == null) bodyConfig.ocean = ocean;
            if (bodyConfig.physics == null) bodyConfig.physics = physics;
        }
    }

    // Initialize generator
    generator.OnInitialUpdate();

    // Add to system manager
    if (StarSystemManager.Instance != null)
    {
        StarSystemManager.Instance.AddBody(newBody);
    }

    // Select the new body
    SelectBody(newBody);

    Debug.Log("Created new " + bodyType.ToString() + " with persistent config at index " + bodyIndex);
}
    private void DeleteSelectedBody()
    {
        if (selectedBody == null)
            return;

        string bodyName = selectedBody.name;

        // Remove from system config if applicable
        if (SystemSavingUtils.Instance != null && 
            SystemSavingUtils.Instance.currentSystemConfig != null)
        {
            // Find and remove the config
            // Note: This part assumes you have a way to identify which config belongs to which body
            // You might need to add an ID field to CelestialBodyConfig for this purpose
        }

        // Remove from system manager
        if (StarSystemManager.Instance != null)
        {
            StarSystemManager.Instance.RemoveBody(selectedBody);
        }

        // Destroy the GameObject
        Destroy(selectedBody.gameObject);

        // Clear selection
        DeselectBody();

        Debug.Log("Deleted " + bodyName);
    }

    // Method to check if panel has an active selection
    public bool HasSelection()
    {
        return selectedBody != null && selectedGenerator != null;
    }
}