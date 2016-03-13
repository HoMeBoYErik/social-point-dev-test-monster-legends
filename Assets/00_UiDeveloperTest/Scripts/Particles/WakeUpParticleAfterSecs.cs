using UnityEngine;
using System.Collections;

public class WakeUpParticleAfterSecs : MonoBehaviour {

    public GameObject ParticleSystem;
    public float WakeUpAfter = 1.0f;
    public float SleepAfter = 0.0f;

	// Use this for initialization
	IEnumerator Start () {

        if( WakeUpAfter > 0 )
        {
            yield return new WaitForSeconds(WakeUpAfter);
            ParticleSystem.SetActive(true);
        }       

        if( SleepAfter > 0)
        {
            yield return new WaitForSeconds(SleepAfter);
            ParticleSystem.SetActive(false);
        }
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
