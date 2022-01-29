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

    private UserInput userInput;
    private GameObject playerGO;
    private BeatGenerator beatGenerator;
    
    private float nextInputDuration = 0.5f;
    private float nextSuccessfulBeatDuration = 0.3f;
    private Timer nextInputDurationTimer;
    private Timer nextSuccesfulBeatDurationTimer;
    
    private bool timeoutInputFinished = true;
    private bool inputAllowed = true;
    private bool successfulBeatMove = true;
    private bool onFire = false;

    private int totalMissedBeats = 0;
    private int missedBeatsCombo = 0;
    private int totalSucceededBeats = 0;
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
            else if (m_successfulBeatMoveCount >= onFireComboNeeded)
            {
                onFire = true;
            }
        }
    }

    private MeshRenderer playerMeshRenderer;

    private void Awake()
    {
        userInput = new UserInput();

        nextInputDurationTimer = new Timer();
        nextInputDurationTimer.Elapsed += NextInputDurationTimer_Elapsed;

        nextSuccesfulBeatDurationTimer = new Timer();
        nextSuccesfulBeatDurationTimer.Elapsed += NextSuccesfulBeatDurationTimer_Elapsed;
    }


    private void Start()
    {
        var bgGo = GameObject.FindGameObjectWithTag(tag:"BeatGenerator");
        beatGenerator = bgGo.GetComponent<BeatGenerator>();
        beatGenerator.OnBeatHandler += OnBeat;

        userInput.Player.Move.performed += Move_performed;

        playerGO = transform.gameObject;
        playerMeshRenderer = playerGO.GetComponent<MeshRenderer>();
        playerMeshRenderer.material.color = Color.white;
    }

    private void Update()
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

    #region Public API
    public int GetTotalBeatsSucceeded()
    {
        return totalSucceededBeats;
    }

    public int GetTotalBeatsMissed()
    {
        return totalMissedBeats;
    }

    public int GetMissedBeatCombo()
    {
        return missedBeatsCombo;
    }

    public int GetSuccessfulBeatMultiplicator()
    {
        return SuccessfulBeatMoveCount;
    }

    public bool IsOnFire()
    {
        return onFire;
    }
    #endregion

    #region Events
    private void Move_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        nextInputDurationTimer.Stop();
        nextSuccesfulBeatDurationTimer.Stop();

        if ((timeoutInputFinished && inputAllowed) && playerGO)
        {
            ++totalSucceededBeats;
            missedBeatsCombo = 0;

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
        ++totalMissedBeats;
        ++missedBeatsCombo; // TODO: This is a combo, do we need a maximum ?
    }

    private void NextSuccesfulBeatDurationTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        successfulBeatMove = false;
    }
    #endregion

    private void OnEnable()
    {
        userInput.Enable();
    }

    private void OnDisable()
    {
        userInput.Disable();
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
        beatGenerator.OnBeatHandler -= OnBeat;
    }
}
