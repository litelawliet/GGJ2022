using System.Collections;
using UnityEngine;

public class BeatGenerator : MonoBehaviour
{
    #region SerializeField
    [SerializeField] private float[] beatsPattern = { 1f, 1f, 1f, 1f };
    #endregion

    /// <summary>
    /// OnBeat() event to subscribe and unsubscribe to.
    /// </summary>
    public delegate void OnBeat(float nextBeatDuration);
    public OnBeat OnBeatHandler;

    private int beatsPatternIndex = 0;
    private bool invokeNextBeat = false;
    private bool coroutineThrown = false;

    void Start()
    {
        OnBeatHandler += InvokeBeat;
    }

    void Update()
    {
        if (!coroutineThrown && !invokeNextBeat)
        {
            StartCoroutine(WaitNextBeat(beatsPattern[beatsPatternIndex]));
        }

        if (invokeNextBeat)
        {
            OnBeatHandler?.Invoke(beatsPattern[beatsPatternIndex]);
        }
    }

    IEnumerator WaitNextBeat(float duration)
    {
        coroutineThrown = true;
        invokeNextBeat = false;

        do
        {
            duration -= Time.deltaTime;

            yield return null;
        } while (duration >= 0.0f);

        invokeNextBeat = true;
        coroutineThrown = false;
        beatsPatternIndex = ++beatsPatternIndex % beatsPattern.Length;
    }

    private void InvokeBeat(float nextBeatDuration)
    {
        invokeNextBeat = false;
    }

    private void OnDestroy()
    {
        OnBeatHandler -= InvokeBeat;
    }
}
