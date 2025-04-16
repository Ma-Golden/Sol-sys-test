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
        if (editModeButton != null)
            editModeButton.onClick.AddListener(() => SwitchMode(GameManager.GameMode.Create));
        
        if (simulationModeButton != null)
            simulationModeButton.onClick.AddListener(() => SwitchMode(GameManager.GameMode.Simulate));
        
        if (newSystemButton != null)
            newSystemButton.onClick.AddListener(NewSystem);
        
        if (saveButton != null)
            saveButton.onClick.AddListener(SaveSystem);
        
        if (loadButton != null)
            loadButton.onClick.AddListener(LoadSystem);
        
        if (resetCameraButton != null)
            resetCameraButton.onClick.AddListener(ResetCamera);
    }
    
    private void SwitchMode(GameManager.GameMode mode)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadScene((int)mode);
            Debug.Log($"Switching to mode: {mode}");
        }
    }
    
    private void NewSystem()
    {
        if (StarSystemManager.Instance != null)
        {
            // Stop any running simulation
            StarSystemManager.Instance.StopSimulation();
            
            // Clear the system
            var bodies = new System.Collections.Generic.List<CelestialBody>(StarSystemManager.Instance.systemBodies);
            foreach (var body in bodies)
            {
                if (body != null)
                {
                    StarSystemManager.Instance.RemoveBody(body);
                    Destroy(body.gameObject);
                }
            }
            
            Debug.Log("Created new system");
        }
    }
    
    private void SaveSystem()
    {
        if (StarSystemManager.Instance != null)
        {
            StarSystemManager.Instance.SaveSystemConfig();
            Debug.Log("System saved");
        }
    }
    
    private void LoadSystem()
    {
        // Implementation depends on your saving system
        Debug.Log("Load System functionality - to be implemented");
    }
    
    private void ResetCamera()
    {
        // Reset camera to default position and rotation
        if (Camera.main != null)
        {
            Camera.main.transform.position = new Vector3(0, 20, -20);
            Camera.main.transform.rotation = Quaternion.Euler(45, 0, 0);
            Debug.Log("Camera reset to default position");
        }
    }
}