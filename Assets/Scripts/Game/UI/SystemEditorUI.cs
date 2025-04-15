using UnityEngine;
using UnityEngine.UI;
using CelestialBodies.Config;
using System.Collections.Generic;
using CelestialBodies.Config.Shape;


//
//
//
//  TODO: REPLACE 'PERTURB STRENGTH' WITH VALID NOISE STRENGTH EQUIVALENT
//
//
//
//
//


public class SystemEditorUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject bodySelectionPanel;
    public GameObject bodyEditPanel;
    public Button createBodyButton;
    public Button deleteBodyButton;
    public Button startSimulationButton;
    public Button stopSimulationButton;
    public Button setAsStarButton;
    public Dropdown bodyTypeDropdown;
    
    [Header("Edit Parameters")]
    public InputField massInput;
    public InputField radiusInput;
    public InputField positionXInput;
    public InputField positionYInput;
    public InputField positionZInput;
    public InputField velocityXInput;
    public InputField velocityYInput;
    public InputField velocityZInput;
    public Slider noiseStrengthSlider;
    public Slider noiseScaleSlider;
    
    [Header("Physics Settings")]
    public Toggle keplerPhysicsToggle;
    public Toggle nBodyPhysicsToggle;
    
    [Header("Generation")]
    public GameObject bodyPrefab;
    public Transform bodiesContainer;
    
    // Currently selected body
    private CelestialBody selectedBody;
    private CelestialBodyGenerator selectedGenerator;
    
    void Start()
    {
        SetupUI();
        InitializeDropdowns();
    }
    
    private void SetupUI()
    {
        // Button listeners
        if (createBodyButton != null)
            createBodyButton.onClick.AddListener(CreateNewBody);
        
        if (deleteBodyButton != null)
            deleteBodyButton.onClick.AddListener(DeleteSelectedBody);
        
        if (startSimulationButton != null)
            startSimulationButton.onClick.AddListener(StartSimulation);
        
        if (stopSimulationButton != null)
            stopSimulationButton.onClick.AddListener(StopSimulation);
        
        if (setAsStarButton != null)
            setAsStarButton.onClick.AddListener(SetAsCentralBody);
        
        // Physics toggles
        if (keplerPhysicsToggle != null)
            keplerPhysicsToggle.onValueChanged.AddListener(OnPhysicsToggleChanged);
        
        if (nBodyPhysicsToggle != null)
            nBodyPhysicsToggle.onValueChanged.AddListener(OnPhysicsToggleChanged);
        
        // Parameter inputs
        if (massInput != null)
            massInput.onEndEdit.AddListener(delegate { UpdateBodyParameter("mass"); });
        
        if (radiusInput != null)
            radiusInput.onEndEdit.AddListener(delegate { UpdateBodyParameter("radius"); });
        
        // Position inputs
        if (positionXInput != null)
            positionXInput.onEndEdit.AddListener(delegate { UpdateBodyParameter("position.x"); });
        
        if (positionYInput != null)
            positionYInput.onEndEdit.AddListener(delegate { UpdateBodyParameter("position.y"); });
        
        if (positionZInput != null)
            positionZInput.onEndEdit.AddListener(delegate { UpdateBodyParameter("position.z"); });
        
        // Velocity inputs
        if (velocityXInput != null)
            velocityXInput.onEndEdit.AddListener(delegate { UpdateBodyParameter("velocity.x"); });
        
        if (velocityYInput != null)
            velocityYInput.onEndEdit.AddListener(delegate { UpdateBodyParameter("velocity.y"); });
        
        if (velocityZInput != null)
            velocityZInput.onEndEdit.AddListener(delegate { UpdateBodyParameter("velocity.z"); });
        
        // Sliders
        if (noiseStrengthSlider != null)
            noiseStrengthSlider.onValueChanged.AddListener(UpdateNoiseStrength);
        
        if (noiseScaleSlider != null)
            noiseScaleSlider.onValueChanged.AddListener(UpdateNoiseScale);
    }
    
    private void InitializeDropdowns()
    {
        if (bodyTypeDropdown != null)
        {
            bodyTypeDropdown.ClearOptions();
            
            List<string> options = new List<string>
            {
                "Planet",
                "Moon",
                "Star"
            };
            
            bodyTypeDropdown.AddOptions(options);
            bodyTypeDropdown.onValueChanged.AddListener(OnBodyTypeChanged);
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
    
    public void SelectBody(CelestialBody body)
    {
        selectedBody = body;
        selectedGenerator = body?.GetComponent<CelestialBodyGenerator>();
        
        if (selectedBody != null && selectedGenerator != null)
        {
            // Update UI to reflect current body parameters
            UpdateUI();
            
            // Show edit panel
            if (bodyEditPanel != null)
                bodyEditPanel.SetActive(true);
        }
        else
        {
            // Hide edit panel if no body is selected
            if (bodyEditPanel != null)
                bodyEditPanel.SetActive(false);
        }
    }
    
    private void UpdateUI()
    {
        if (selectedBody == null || selectedGenerator == null || selectedGenerator.bodyConfig == null)
            return;
            
        // Set body type dropdown
        if (bodyTypeDropdown != null)
            bodyTypeDropdown.value = (int)selectedGenerator.bodyConfig.bodyType;
            
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
            
        // Set noise parameters if available
        if (selectedGenerator.bodyConfig.shape != null)
        {
            Shape.ShapeConfig shapeConfig = selectedGenerator.bodyConfig.shape.GetConfig();
            
            if (noiseStrengthSlider != null)
                noiseStrengthSlider.value = shapeConfig.perturbStrength;
                
            if (noiseScaleSlider != null)
                noiseScaleSlider.value = shapeConfig.perturbStrength;
        }
    }
    
    private void UpdateBodyParameter(string parameter)
    {
        if (selectedBody == null || selectedGenerator == null)
            return;
            
        float value;
        
        switch (parameter)
        {
            case "mass":
                if (float.TryParse(massInput.text, out value))
                {
                    selectedBody.mass = value;
                }
                break;
                
            case "radius":
                if (float.TryParse(radiusInput.text, out value))
                {
                    selectedGenerator.bodyConfig.radius = value;
                    selectedGenerator.OnShapeUpdate();
                }
                break;
                
            case "position.x":
                if (float.TryParse(positionXInput.text, out value))
                {
                    Vector3 pos = selectedBody.position;
                    pos.x = value;
                    selectedBody.position = pos;
                    
                    // Update physics config
                    UpdatePhysicsPosition(pos);
                }
                break;
                
            case "position.y":
                if (float.TryParse(positionYInput.text, out value))
                {
                    Vector3 pos = selectedBody.position;
                    pos.y = value;
                    selectedBody.position = pos;
                    
                    // Update physics config
                    UpdatePhysicsPosition(pos);
                }
                break;
                
            case "position.z":
                if (float.TryParse(positionZInput.text, out value))
                {
                    Vector3 pos = selectedBody.position;
                    pos.z = value;
                    selectedBody.position = pos;
                    
                    // Update physics config
                    UpdatePhysicsPosition(pos);
                }
                break;
                
            case "velocity.x":
                if (float.TryParse(velocityXInput.text, out value))
                {
                    Vector3 vel = selectedBody.velocity;
                    vel.x = value;
                    selectedBody.velocity = vel;
                    
                    // Update physics config
                    UpdatePhysicsVelocity(vel);
                }
                break;
                
            case "velocity.y":
                if (float.TryParse(velocityYInput.text, out value))
                {
                    Vector3 vel = selectedBody.velocity;
                    vel.y = value;
                    selectedBody.velocity = vel;
                    
                    // Update physics config
                    UpdatePhysicsVelocity(vel);
                }
                break;
                
            case "velocity.z":
                if (float.TryParse(velocityZInput.text, out value))
                {
                    Vector3 vel = selectedBody.velocity;
                    vel.z = value;
                    selectedBody.velocity = vel;
                    
                    // Update physics config
                    UpdatePhysicsVelocity(vel);
                }
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
    
    private void UpdateNoiseStrength(float value)
    {
        if (selectedGenerator == null || selectedGenerator.bodyConfig == null || selectedGenerator.bodyConfig.shape == null)
            return;
            
        Shape.ShapeConfig shapeConfig = selectedGenerator.bodyConfig.shape.GetConfig();
        shapeConfig.perturbStrength = value;
        selectedGenerator.bodyConfig.shape.SetConfig(shapeConfig);
        selectedGenerator.OnShapeUpdate();
    }
    
    private void UpdateNoiseScale(float value)
    {
        if (selectedGenerator == null || selectedGenerator.bodyConfig == null || selectedGenerator.bodyConfig.shape == null)
            return;
            
        Shape.ShapeConfig shapeConfig = selectedGenerator.bodyConfig.shape.GetConfig();
        shapeConfig.perturbStrength = value;
        selectedGenerator.bodyConfig.shape.SetConfig(shapeConfig);
        selectedGenerator.OnShapeUpdate();
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
        bodyConfig.Init(CelestialBodyConfig.CelestialBodyType.Planet);
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
    }
    
    private void DeleteSelectedBody()
    {
        if (selectedBody == null)
            return;
            
        // Remove from system manager
        if (StarSystemManager.Instance != null)
        {
            StarSystemManager.Instance.RemoveBody(selectedBody);
        }
        
        // Destroy the GameObject
        Destroy(selectedBody.gameObject);
        
        // Clear selection
        selectedBody = null;
        selectedGenerator = null;
        
        // Hide edit panel
        bodyEditPanel?.SetActive(false);
    }
    
    private void StartSimulation()
    {
        if (StarSystemManager.Instance != null)
        {
            StarSystemManager.Instance.StartSimulation();
        }
    }
    
    private void StopSimulation()
    {
        if (StarSystemManager.Instance != null)
        {
            StarSystemManager.Instance.StopSimulation();
        }
    }
    
    private void SetAsCentralBody()
    {
        if (selectedBody != null && StarSystemManager.Instance != null)
        {
            StarSystemManager.Instance.SetCentralBody(selectedBody);
        }
    }
    
    private void OnPhysicsToggleChanged(bool isOn)
    {
        if (!isOn || StarSystemManager.Instance == null)
            return;
            
        // Update physics model in system manager
        if (keplerPhysicsToggle != null && keplerPhysicsToggle.isOn)
        {
            StarSystemManager.Instance.SwitchPhysicsModel(true);
            if (nBodyPhysicsToggle != null)
                nBodyPhysicsToggle.isOn = false;
        }
        else if (nBodyPhysicsToggle != null && nBodyPhysicsToggle.isOn)
        {
            StarSystemManager.Instance.SwitchPhysicsModel(false);
            if (keplerPhysicsToggle != null)
                keplerPhysicsToggle.isOn = false;
        }
    }
}