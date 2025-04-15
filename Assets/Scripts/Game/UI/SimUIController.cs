using CelestialBodies.Config;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject topNavigationPanel;
    public GameObject systemEditorPanel;
    public GameObject bodyEditorPanel;

    [Header("Navigation Buttons")]
    public Button editModeButton;
    public Button simulationModeButton;
    public Button newSystemButton;
    public Button saveButton;
    public Button loadButton;
    public Button resetCameraButton;

    private void Start()
    {
        // Set up button listeners
        editModeButton.onClick.AddListener(() => SwitchMode(GameManager.GameMode.Create));
        simulationModeButton.onClick.AddListener(() => SwitchMode(GameManager.GameMode.Simulate));
        newSystemButton.onClick.AddListener(NewSystem);
        saveButton.onClick.AddListener(SaveSystem);
        loadButton.onClick.AddListener(LoadSystem);
        resetCameraButton.onClick.AddListener(ResetCamera);
    }

    private void SwitchMode(GameManager.GameMode mode)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadScene((int)mode);
        }
    }

    private void NewSystem()
    {
        if (StarSystemManager.Instance != null)
        {
            // Clear the system
            StarSystemManager.Instance.LoadSystemConfig(ScriptableObject.CreateInstance<StarSystemConfig>());
        }
    }

    private void SaveSystem()
    {
        if (StarSystemManager.Instance != null)
        {
            StarSystemManager.Instance.SaveSystemConfig();
        }
    }

    private void LoadSystem()
    {
        // Implementation depends on your saving system
        // Could open a file dialog or load a predefined config
    }

    private void ResetCamera()
    {
        // Reset camera to default position
        Camera.main.transform.position = new Vector3(0, 20, -20);
        Camera.main.transform.rotation = Quaternion.Euler(45, 0, 0);
    }
}