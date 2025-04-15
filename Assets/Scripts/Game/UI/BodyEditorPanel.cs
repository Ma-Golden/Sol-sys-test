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
        selectedGenerator = body?.GetComponent<CelestialBodyGenerator>();

        if (selectedBody != null && selectedGenerator != null)
        {
            gameObject.SetActive(true);
            UpdateUI();
        }
        else
        {
            DeselectBody();
        }
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


        // TODO: CHECK THIS!!!!!1
        // Randomize shape parameters
        shapeConfig.perturbStrength = Random.Range(0.1f, 2.0f);
        shapeConfig.perturbStrength = Random.Range(1.0f, 5.0f);
        shapeConfig.perturbStrength = Random.Range(0.01f, 0.3f);
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

        // Note: This implementation depends on your specific shading setup
        // You may need to cast to specific shading types and modify their properties

        // Generic randomization that should work for most cases
        switch (selectedGenerator.bodyConfig.bodyType)
        {
            case CelestialBodyConfig.CelestialBodyType.Planet:
                if (selectedGenerator.bodyConfig.shading is PlanetShading planetShading)
                {
                    // Randomize planet-specific shading properties
                    // Example: planetShading.SetColor(Random.ColorHSV());
                }
                break;

            case CelestialBodyConfig.CelestialBodyType.Moon:
                if (selectedGenerator.bodyConfig.shading is MoonShading moonShading)
                {
                    // Randomize moon-specific shading properties
                }
                break;

            case CelestialBodyConfig.CelestialBodyType.Star:
                if (selectedGenerator.bodyConfig.shading is StarShading starShading)
                {
                    // Randomize star-specific shading properties
                }
                break;
        }

        selectedGenerator.OnShadingUpdate();
        Debug.Log("Randomized shading for " + selectedBody.name);
    }

    private void CreateNewBody()
    {
        if (bodyPrefab == null || bodiesContainer == null)
            return;

        // Create a new body GameObject
        GameObject newBodyObject = Instantiate(bodyPrefab, Vector3.zero, Quaternion.identity, bodiesContainer);
        newBodyObject.name = "CelestialBody_" + System.Guid.NewGuid().ToString().Substring(0, 8);

        // Add CelestialBody component
        CelestialBody newBody = newBodyObject.AddComponent<CelestialBody>();
        newBody.mass = 1.0f;
        newBody.position = Vector3.zero;
        newBody.velocity = Vector3.zero;

        // Add CelestialBodyGenerator component
        CelestialBodyGenerator generator = newBodyObject.AddComponent<CelestialBodyGenerator>();
        generator.body = newBody;
        newBody.celestiaBodyGenerator = generator;

        // Create body config
        CelestialBodyConfig bodyConfig = ScriptableObject.CreateInstance<CelestialBodyConfig>();

        // Get selected body type from dropdown if available
        CelestialBodyConfig.CelestialBodyType bodyType = CelestialBodyConfig.CelestialBodyType.Planet;
        if (bodyTypeDropdown != null)
        {
            bodyType = (CelestialBodyConfig.CelestialBodyType)bodyTypeDropdown.value;
        }

        bodyConfig.Init(bodyType);
        bodyConfig.radius = 5.0f;

        // Create default physics config
        Physics physicsConfig = ScriptableObject.CreateInstance<Physics>();
        Physics.PhysicsSettings physicsSettings = new Physics.PhysicsSettings();
        physicsSettings.initialPosition = Vector3.zero;
        physicsSettings.initialVelocity = Vector3.zero;
        physicsConfig.SetSettings(physicsSettings);
        bodyConfig.physics = physicsConfig;

        // Set config
        generator.bodyConfig = bodyConfig;

        // Apply default features from SystemSavingUtils
        if (SystemSavingUtils.Instance != null)
        {
            var (shape, shading, ocean, physics) = SystemSavingUtils.Instance.CreateFeatures(bodyConfig.bodyType);
            bodyConfig.shape = shape;
            bodyConfig.shading = shading;
            bodyConfig.ocean = ocean;
        }

        // Initialize generator
        generator.OnInitialUpdate();

        // Add to system
        if (StarSystemManager.Instance != null)
        {
            StarSystemManager.Instance.AddBody(newBody);
        }

        // Select the new body
        SelectBody(newBody);

        Debug.Log("Created new " + bodyType.ToString());
    }

    private void DeleteSelectedBody()
    {
        if (selectedBody == null)
            return;

        string bodyName = selectedBody.name;

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