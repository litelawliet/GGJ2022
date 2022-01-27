using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    PauseInput pauseInput;
    public GameObject pauseMenu;
    bool m_Paused = false;

    private void Awake()
    {
        pauseInput = new PauseInput(); 
    }

    private void OnEnable()
    {
        pauseInput.Enable();
    }

    private void OnDisable()
    {
        pauseInput.Disable();
    }

    private void Start()
    {
        pauseInput.Pause.PauseGame.performed += _ => TogglePause();
    }

    /// <summary>
    /// Used to toggle the paused state of the game
    /// </summary>
    public void TogglePause()
    {
        if ( m_Paused )
            ResumeGame();
        else
            PauseGame();
    }

    // Stop time when in pause
    void PauseGame()
    {
        Debug.Log("We paused the game");
        Time.timeScale = 0;
        AudioListener.pause = true;
        pauseMenu.SetActive(true);
        m_Paused = true;
    }

    // Start time again when resuming game
    void ResumeGame()
    {
        Debug.Log("We resume the game");
        Time.timeScale = 1;
        AudioListener.pause = false;
        pauseMenu.SetActive(false);
        m_Paused = false;
    }
}
