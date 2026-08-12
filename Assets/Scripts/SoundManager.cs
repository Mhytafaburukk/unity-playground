using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public AudioSource impactVoice;
    private Coroutine currentRoutine;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    public void SesiCal(float sure)
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        impactVoice.Stop(); 
        impactVoice.time = 0;
        currentRoutine = StartCoroutine(SureliCal(sure));
    }

    private IEnumerator SureliCal(float sure)
    {
        impactVoice.Play();
        yield return new WaitForSeconds(sure);
        impactVoice.Stop();
        currentRoutine = null;
    }
}
