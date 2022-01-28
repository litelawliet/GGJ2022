using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float waitNexInputDuration = 0.5f;

    private UserInput m_userInput;
    private GameObject playerGO;

    private bool inputAllowed = true;

    // Start is called before the first frame update
    void Awake()
    {
        m_userInput = new UserInput();
    }

    private void Start()
    {
        m_userInput.Player.Move.performed += Move_performed;
        playerGO = transform.gameObject;
    }

    private void Move_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (inputAllowed && playerGO)
        {
            var value = obj.ReadValue<Vector2>();
            playerGO.transform.Translate(value.x, 0.0f, value.y);
            StartCoroutine(TimeoutInput(waitNexInputDuration));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator TimeoutInput(float duration)
    {
        inputAllowed = false;
        do
        {
            duration -= Time.deltaTime;

            yield return null;
        } while (duration >= 0.0f);

        inputAllowed = true;
    }

    private void OnEnable()
    {
        m_userInput.Enable();
    }

    private void OnDisable()
    {
        m_userInput.Disable();
    }
}
