using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject BeatGeneratorPrefab;
    private PauseInput pauseInput;
    bool m_Paused = false;

    private GameObject m_Generator;

    private void Awake()
    {
        m_Generator = Instantiate(BeatGeneratorPrefab, new Vector3(0, 0, 0), Quaternion.identity);
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
    /// Toggle the paused state of the game.
    /// </summary>
    public void TogglePause()
    {
        if ( m_Paused )
            ResumeGame();
        else
            PauseGame();
    }

    /// <summary>
    /// Pause the game and timeScale.
    /// </summary>
    private void PauseGame()
    {
        Debug.Log("We paused the game");
        Time.timeScale = 0f;
        AudioListener.pause = true;
        if (pauseMenu)
        {
            pauseMenu.SetActive(true);
        }
        m_Paused = true;
    }

    /// <summary>
    /// Resume the game and timeScale.
    /// </summary>
    private void ResumeGame()
    {
        Debug.Log("We resume the gameS");
        Time.timeScale = 1f;
        AudioListener.pause = false;
        if (pauseMenu)
        {
            pauseMenu.SetActive(false);
        }
        m_Paused = false;
    }

    GameObject GetBeatGenerator()
    {
        return m_Generator;
    }
}
