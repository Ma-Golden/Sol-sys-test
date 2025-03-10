using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    // Following Singleton pattern for persistent game manager
    public static GameManager Instance;

    // Enumerate for game mode
    public enum GameMode { Menu, Create, Simulate, Unknown }
    public GameMode currentGameMode = GameMode.Menu;


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
        }
        else 
        { 
            Destroy(gameObject);
        }
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
