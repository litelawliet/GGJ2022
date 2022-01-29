using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Tooltip("Player's move range winthin next beat duration in percentage. Indicates in % the time frame for the next user input.")]
    [SerializeField] private float moveWindowPercentage = 50f;
    [Tooltip("Player's successful beat window in percentage. Indicates in % the time frame for the next successful beat.")]
    [SerializeField] private float successfulBeatWindowPercentage = 30f;
    [SerializeField] private int onFireComboNeeded = 3;

    private UserInput m_userInput;
    private GameObject playerGO;
    private BeatGenerator m_beatGenerator;
    
    private float nextInputDuration = 0.5f;
    private float nextSuccessfulBeatDuration = 0.3f;
    private Timer nextInputDurationTimer;
    private Timer nextSuccesfulBeatDurationTimer;
    
    private bool timeoutInputFinished = true;
    private bool inputAllowed = true;
    private bool successfulBeatMove = true;
    private bool onFire = false;

    private int m_successfulBeatMoveCount;
    private int SuccessfulBeatMoveCount
    {
        get { return m_successfulBeatMoveCount;}
        set
        {
            m_successfulBeatMoveCount = value;
            if (m_successfulBeatMoveCount == 0)
            {
                onFire = false;
            }
            else if (m_successfulBeatMoveCount > 2)
            {
                onFire = true;
            }
        }
    }

    private MeshRenderer playerMeshRenderer;

    // Start is called before the first frame update
    void Awake()
    {
        m_userInput = new UserInput();

        nextInputDurationTimer = new Timer();
        nextInputDurationTimer.Elapsed += NextInputDurationTimer_Elapsed;

        nextSuccesfulBeatDurationTimer = new Timer();
        nextSuccesfulBeatDurationTimer.Elapsed += NextSuccesfulBeatDurationTimer_Elapsed;
    }



    private void Start()
    {
        var bgGo = GameObject.FindGameObjectWithTag(tag:"BeatGenerator");
        m_beatGenerator = bgGo.GetComponent<BeatGenerator>();
        m_beatGenerator.OnBeatHandler += OnBeat;

        m_userInput.Player.Move.performed += Move_performed;

        playerGO = transform.gameObject;
        playerMeshRenderer = playerGO.GetComponent<MeshRenderer>();
        playerMeshRenderer.material.color = Color.white;
    }

    private void Move_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        nextInputDurationTimer.Stop();
        nextSuccesfulBeatDurationTimer.Stop();

        if ((timeoutInputFinished && inputAllowed) && playerGO)
        {
            if (successfulBeatMove)
            {
                ++SuccessfulBeatMoveCount;
            }
            else
            {
                SuccessfulBeatMoveCount = 0;
            }

            //Debug.Log("Input allowed: " + inputAllowed.ToString() + "\nSuccessful beat move: " + successfulBeatMove.ToString());

            inputAllowed = false;
            successfulBeatMove = false;

            var value = obj.ReadValue<Vector2>();
            playerGO.transform.Translate(value.x, 0.0f, value.y);

            playerMeshRenderer.material.color = Color.white;
            
            StartCoroutine(TimeoutInput(nextInputDuration));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (successfulBeatMove)
        { 
            playerMeshRenderer.material.color = Color.yellow;
        }
        else if (inputAllowed)
        {
            playerMeshRenderer.material.color = Color.blue;
        }
        else
        {
            playerMeshRenderer.material.color = Color.white;
        }
    }

    #region Events
    IEnumerator TimeoutInput(float duration)
    {
        timeoutInputFinished = false;
        do
        {
            duration -= Time.deltaTime;

            yield return null;
        } while (duration >= 0.0f);

        timeoutInputFinished = true;
    }

    private void NextInputDurationTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        inputAllowed = false;
    }

    private void NextSuccesfulBeatDurationTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        successfulBeatMove = false;
    }
    #endregion

    private void OnEnable()
    {
        m_userInput.Enable();
    }

    private void OnDisable()
    {
        m_userInput.Disable();
    }

    private void OnBeat(float nextBeatDuration)
    {
        nextInputDurationTimer.Stop();
        nextSuccesfulBeatDurationTimer.Stop();

        inputAllowed = true;
        successfulBeatMove = true;

        nextInputDuration = nextBeatDuration * NextInputWindowReduction(); // Start timer for next beat time
        nextSuccessfulBeatDuration = nextBeatDuration * NextSuccessfulBeatWindowReduction(); // Start timer for successful beat time

        nextInputDurationTimer.Interval = nextInputDuration * 1000f;
        nextSuccesfulBeatDurationTimer.Interval = nextSuccessfulBeatDuration * 1000f;

        playerMeshRenderer.material.color = Color.yellow;
        nextInputDurationTimer.Start();
        nextSuccesfulBeatDurationTimer.Start();

        //Debug.Log("Next input within " + nextInputDuration + " sec\nNext successful beat within " + nextSuccessfulBeatDuration + " sec\n");
    }

    private float NextSuccessfulBeatWindowReduction()
    {
        return successfulBeatWindowPercentage / 100f;
    }

    private float NextInputWindowReduction()
    {
        return moveWindowPercentage / 100f;
    }

    private void OnDestroy()
    {
        m_beatGenerator.OnBeatHandler -= OnBeat;
    }
}
