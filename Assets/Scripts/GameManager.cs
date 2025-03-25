using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    // Following Singleton pattern for persistent game manager
    public static GameManager Instance;

    [Tooltip("Target frame rate of game")]
    [Range(30, 300)]
    public int targetFrameRate = 60;


    // Enumerate for game mode
    public enum GameMode { Menu, Create, Simulate, Unknown }
    public GameMode currentGameMode = GameMode.Menu;

    public struct Constants
    {
        public const float GravityConstant = 6.674f;
        public const float TimeStep = 0.01f;
        public const float ScaleFactor = 1.0f;
    }

    public Constants ProgramConstants;

    public bool tempPause = false;


    // TODO: Testing
    [Tooltip("Target frame rate of game")]
    public float targetRate = 30.0f;
    private float _currentFrameTime;

    private void Awake()
    {
        // Ensure one instance
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameManager instance initialized");
            // TODO: JSON CONVERT STUFF
        }
        else 
        { 
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Disable V-Sync
        QualitySettings.vSyncCount = 0;

        // Set target frame rate
        Application.targetFrameRate = targetFrameRate;
    }
    

    public Camera GetMainCamera()
    {
        return Camera.main;
    }


    public void LoadScene(int index)
    {
        switch (index)
        {
            case 0:
                currentGameMode = GameMode.Menu; 
                break;
            case 1:
                currentGameMode = GameMode.Create;
                break;
            case 2:
                currentGameMode = GameMode.Simulate;
                break;
            default:
                currentGameMode = GameMode.Unknown;
                break;
        }
    }
}
