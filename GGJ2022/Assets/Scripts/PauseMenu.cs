using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameManager gm;

    public void TogglePause()
    {
        gm.TogglePause();
    }
    
    public void BackToMenu()
    {
        SceneManager.LoadScene(0); // We load build Index 0, our main menu -> Magic value because Unity is stupid
    }

    public void QuitGame()
    {
        Debug.Log("We Quit");
        Application.Quit();
    }
}
