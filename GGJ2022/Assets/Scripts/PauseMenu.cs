using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameManager gm;

    public void TogglePause()
    {
        gm.TogglePause();
    }
    
    public void BackToMenu()
    {
        // Back to main menu at idx 0
        SceneManager.LoadScene(0); 
    }

    public void QuitGame()
    {
        Debug.Log("We Quit");
        Application.Quit();
    }
}
