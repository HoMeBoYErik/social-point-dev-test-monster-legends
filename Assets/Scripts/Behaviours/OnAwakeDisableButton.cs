using UnityEngine;
using System.Collections;

namespace SocialPoint
{
	[RequireComponent(typeof(UIButton))]
	public class OnAwakeDisableButton : MonoBehaviour 
	{
		private UIButton button;


		void Awake()
		{
			button = this.GetComponent<UIButton>();

			button.isEnabled = false;
		}

		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			
		}
	}
}

