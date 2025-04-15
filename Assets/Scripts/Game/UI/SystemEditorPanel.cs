using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SystemEditorPanel : MonoBehaviour
{
    [Header("System Properties")]
    public Dropdown physicsModelDropdown;
    public Slider simulationSpeedSlider;
    public Toggle relativeToStarToggle;

    [Header("Templates")]
    public Dropdown templateDropdown;
    public Button generateButton;
    public Button clearSystemButton;

    private void Start()
    {
        InitializeDropdowns();
        SetupListeners();
    }

    private void InitializeDropdowns()
    {
        // Set up physics model dropdown
        if (physicsModelDropdown != null)
        {
            physicsModelDropdown.ClearOptions();
            physicsModelDropdown.AddOptions(StarSystemManager.GetPhysicsModelNames());

            if (StarSystemManager.Instance != null)
            {
                physicsModelDropdown.value = (int)StarSystemManager.Instance.currentPhysicsModelType;
            }
        }

        // Set up templates dropdown (implement as needed)
    }

    private void SetupListeners()
    {
        if (physicsModelDropdown != null)
            physicsModelDropdown.onValueChanged.AddListener(OnPhysicsModelChanged);

        if (simulationSpeedSlider != null)
            simulationSpeedSlider.onValueChanged.AddListener(OnSimulationSpeedChanged);

        if (relativeToStarToggle != null)
            relativeToStarToggle.onValueChanged.AddListener(OnRelativeToStarChanged);

        if (generateButton != null)
            generateButton.onClick.AddListener(GenerateTemplate);

        if (clearSystemButton != null)
            clearSystemButton.onClick.AddListener(ClearSystem);
    }

    private void OnPhysicsModelChanged(int index)
    {
        if (StarSystemManager.Instance != null)
        {
            StarSystemManager.Instance.SwitchPhysicsModelByIndex(index);
        }
    }

    private void OnSimulationSpeedChanged(float value)
    {
        if (StarSystemManager.Instance != null &&
            StarSystemManager.Instance.simulationController != null)
        {
            StarSystemManager.Instance.simulationController.simulationSpeed = Mathf.RoundToInt(value);
        }
    }

    private void OnRelativeToStarChanged(bool value)
    {
        if (StarSystemManager.Instance != null)
        {
            StarSystemManager.Instance.simulateRelativeToStar = value;
        }
    }

    private void GenerateTemplate()
    {
        // Generate selected template
        if (templateDropdown != null)
        {
            int templateIndex = templateDropdown.value;
            // Implement template generation
        }
    }

    private void ClearSystem()
    {
        if (StarSystemManager.Instance != null)
        {
            StarSystemManager.Instance.StopSimulation();

            // Clear existing bodies
            var bodies = new List<CelestialBody>(StarSystemManager.Instance.systemBodies);
            foreach (var body in bodies)
            {
                if (body != null)
                {
                    StarSystemManager.Instance.RemoveBody(body);
                    Destroy(body.gameObject);
                }
            }
        }
    }
}