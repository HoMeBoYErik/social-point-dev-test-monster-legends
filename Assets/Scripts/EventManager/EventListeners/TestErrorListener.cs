using UnityEngine;
using System.Collections;
using SocialPoint;

public class TestErrorListener : MonoBehaviour, IEventListener {

	public GameObject NoInternetPopup;
	public GameObject YouAreOnline;
	public bool doVerifyInternet;
	public float verificationInterval = 15.0f;

	void OnEnable()
	{
		// When this object is enabled start listening to Errors
		// If you want to stop listening just disable the object the script is attached to
		EventManager.Instance.AddListener(this as IEventListener, "ErrorEvent");
		this.doVerifyInternet = true;
		this.StartCoroutine("coVerifyInternet"); // start a coroutine that force internet connection every verificationInterval seconds
		// this is just an example, the right way is to check connection when a net error occurr.

	}

	void OnDisable()
	{
		// When this object is disabled stop listening to errors and we will not managing it
		EventManager.Instance.DetachListener(this as IEventListener, "ErrorEvent");
		this.doVerifyInternet = false;
		this.StopCoroutine("coVerifyInternet");

	}


	bool IEventListener.HandleEvent(IEvent evt)
	{
		// Check if the type of Event is correct and it's what we are looking for
		// It's useful when we register for more than one type of event.
		if( evt.GetName() == "ErrorEvent")
		{
			ErrorObject errorObj = evt.GetData() as ErrorObject;

			switch ( errorObj.errorCode )
			{
				case ERROR_CODES.NO_INTERNET_CONNECTION:
					this.YouAreOnline.SetActive(false);
					this.NoInternetPopup.SetActive(true);
				break;

				case ERROR_CODES.INTERNET_VERIFIED:
					this.NoInternetPopup.SetActive(false);
					this.YouAreOnline.SetActive(true);
					break;
			}
		}

		return false; // we want to propagate event if anyone else is listening to the same error
	}


	IEnumerator coVerifyInternet( )
	{
		while ( this.doVerifyInternet )
		{
			// start a coroutine that force internet connection every verificationInterval seconds
			yield return new WaitForSeconds(verificationInterval);
			// this is just an example, the right way is to check connection when a net error occurr.
			if( InternetConnectionController.Instance.iStatus != InternetReachabilityVerifier.Status.PendingVerification )
			{
				InternetConnectionController.Instance.VerifyConnection();
			}

		}

		yield return null;

	}
}
	