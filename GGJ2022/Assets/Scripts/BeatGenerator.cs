using System.Collections;
using UnityEngine;

public class BeatGenerator : MonoBehaviour
{
    #region SerializeField
    [SerializeField] GameObject cube; // Test purpose
    [SerializeField] private float[] beatsPattern = { 1f, 0.5f, 2f, 0.5f };
    [SerializeField] private float beatDuration = 0.1f;
    #endregion

    /// <summary>
    /// OnBeat() event to subscribe and unsubscribe to.
    /// </summary>
    public delegate void OnBeat();
    private OnBeat onBeatHandler;

    private MeshRenderer meshRenderer; // Test purpose
    private int beatsPatternIndex = 0;
    private bool invokeNextBeat = false;
    private bool coroutineThrown = false;

    void Start()
    {
        if (cube)
        {
            meshRenderer = cube.GetComponent<MeshRenderer>();
        }

        onBeatHandler = InvokeBeat;
    }

    void Update()
    {
        if (!coroutineThrown && !invokeNextBeat)
        {
            StartCoroutine(WaitNextBeat(beatsPattern[beatsPatternIndex]));
        }
        else
        {
            if (meshRenderer)
            {
                meshRenderer.material.color = Color.white;
            }
        }

        if (invokeNextBeat)
        {
            if (meshRenderer)
            {
                onBeatHandler?.Invoke();
            }
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

    IEnumerator WaitBeatDuration(float duration)
    {
        do
        {
            duration -= Time.deltaTime;

            yield return null;
        } while (duration >= 0.0f);

        invokeNextBeat = false;
    }

    private void InvokeBeat()
    {
        meshRenderer.material.color = Color.red;
        StartCoroutine(WaitBeatDuration(beatDuration));
    }
}
