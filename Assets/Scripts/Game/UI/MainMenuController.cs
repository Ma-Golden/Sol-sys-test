using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;


public class MainMenuController : MonoBehaviour
{
    public CanvasGroup OptionPanel;

    // Current build indeces
    // 0: MainMenu
    // 1: SampleScene
    // 2: TestScene

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Debug.Log("Mouse button registered");
    }


    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void OpenTestScene()
    {
        Debug.Log("Opening Test Scene");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
        //SceneManager.LoadScene(2);
    }
    
    public void OpenSystemEditor()
    {
        Debug.Log("Opening SystemEditing Scene");
        SceneManager.LoadScene("SystemEditingScene");
    }


    public void Option()
    {
        OptionPanel.alpha = 1;
        OptionPanel.blocksRaycasts = true;
    }

    public void Back()
    {
        OptionPanel.alpha = 0;
        OptionPanel.blocksRaycasts = false;
    }


    // Simple quit function
    public void QuitGame()
    {
        Debug.Log("Quitting Game");

        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                    Application.Quit();
        #endif
    }
}
