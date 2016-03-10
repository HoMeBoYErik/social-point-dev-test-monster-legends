using UnityEngine;
using System.Collections;
using Deprecated;

namespace SocialPoint
{
	[RequireComponent(typeof(InternetReachabilityVerifier))]
	public class InternetConnectionController : MonoBehaviour 
	{
		// Static singleton reference
		public static InternetConnectionController Instance { get; private set; }
		// whether or not this object should persist accross scenes (DontDestroyOnLoad)
		public bool makePersistent = true;

		// Protected reference to InternetReachabilityVerifier script that reside in the same GO
		private InternetReachabilityVerifier irv;

		// Timeout parameters for automated connections checks
		public float defaultReachabilityCheckPeriodSeconds = 15.0f;
		public float netVerificationErrorRetryTimeSeconds = 15.0f;
		public float netVerificationMismatchRetryTimeSeconds = 7.0f;

		public InternetReachabilityVerifier.Status iStatus;


		bool verifyNetCheckData(WWW www, string customMethodExpectedData)
		{
			// Example validation - require that given custom string is not empty
			// and that it appears at some place in the returned document.
			if (customMethodExpectedData == null ||
			    customMethodExpectedData.Length == 0)
			{
#if _DEBUG	
				Debug.Log("[InternetConnectionController]:Custom verifyNetCheckData - Null or empty customMethodExpectedData!");
#endif
				return false;
			}
			bool result = www.text.Contains(customMethodExpectedData);
#if _DEBUG	
			Debug.Log("[InternetConnectionController]:Custom verifyNetCheckData - result:" + result + ", customMethodExpectedData:" + customMethodExpectedData + ", www.text:" + www.text);
#endif
			return result;
		}



		// Called everytime the actual connection state change (the check is every 15 seconds for default.
		
		void netStatusChanged(InternetReachabilityVerifier.Status newStatus)
		{
			// Update Internet Connection Status
			this.iStatus = newStatus;

#if _DEBUG
			Debug.Log("[InternetConnectionController]:Net status changed: " + newStatus);
#endif
			if (newStatus == InternetReachabilityVerifier.Status.Error)
			{
				string error = irv.lastError;

				// Queue a new error event
				EventManager.Instance.QueueEvent(new ErrorEvent(ERROR_CODES.NO_INTERNET_CONNECTION, error));
#if _DEBUG	
				Debug.Log("[InternetConnectionController]:Error: " + error);
#endif
				if (error.Contains("no crossdomain.xml"))
				{
#if _DEBUG	
					Debug.Log("[InternetConnectionController]: No crossdomain.xml, check WWW Security Emulation Host URL of Unity Editor in Edit->Project Settings->Editor");
#endif				
				}
			}

			else if ( newStatus == InternetReachabilityVerifier.Status.NetVerified )
			{
				EventManager.Instance.QueueEvent(new ErrorEvent(ERROR_CODES.INTERNET_VERIFIED, "Internet OK"));
			}


		}


		// One Time Initializations
		void Awake()
		{
			// First we check if there are any other instances conflicting
			if(Instance != null && Instance != this)
			{
				// If that is the case, we destroy other instances
				Destroy(gameObject);
				
			}
			
			// Here we save our singleton instance
			Instance = this;
			
			// Furthermore we make sure that we don't destroy between scenes (this is optional)
			if( makePersistent )
			{
				DontDestroyOnLoad(this.gameObject);
			}
			
			// Others "one time" initializations...
			
			
			// Just a demo on how to encapsule all Debug code in a pre-compiler #if
			// to strip out debug code when compiling for release
			#if _DEBUG	
			Debug.Log ("[Internet Connection Controller]: is Awake ");
			#endif
			
		}
	

		// Use this for initialization
		void Start () 
		{
			// Attach to IRV and setup default customized params
			irv = GetComponent<InternetReachabilityVerifier>();
			irv.captivePortalDetectionMethod = InternetReachabilityVerifier.CaptivePortalDetectionMethod.Custom;
			irv.setNetActivityTimes( this.defaultReachabilityCheckPeriodSeconds, 
			                         this.netVerificationErrorRetryTimeSeconds, 
			                         this.netVerificationMismatchRetryTimeSeconds );

			// Customized function to check if the remote content is correct
			irv.customMethodVerifierDelegate = verifyNetCheckData;
			irv.statusChangedDelegate += netStatusChanged;


		}

		// This function change back the state of IRV to pending verification
		// This will shoot a coroutine that will try to connect back to our server
		// The point is that this verification is not instantaneous but will shoot an event
		public void VerifyConnection()
		{
			irv.status = InternetReachabilityVerifier.Status.PendingVerification;
		}

		public bool HasInternet()
		{
			if( this.iStatus == InternetReachabilityVerifier.Status.NetVerified )
			{
				return true;
			}
			return false;
		}

	}
}
