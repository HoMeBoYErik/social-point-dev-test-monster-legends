using UnityEngine;
using System.Collections;

namespace SocialPoint{

	[RequireComponent(typeof(UILabel))]
	public class OnDataReadySetLabel : MonoBehaviour, IEventListener  {

		public string StringKey;
		public GameDataManager gameDataManager;
		public bool Uppercase = false;
		private UILabel label;

		void Awake()
		{
			label = this.GetComponent<UILabel>();
		}

		// Use this for initialization
		void Start () 
		{
			gameDataManager = GameObject.FindGameObjectWithTag("DataManager").GetComponent<GameDataManager>();
			EventManager.Instance.AddListener(this as IEventListener, "JSONEvent");
		}
		
		bool IEventListener.HandleEvent(IEvent evt)
		{
			bool isEventConsumed = false;
			
			// Check if the type of Event is correct and it's what we are looking for
			// It's useful when we register for more than one type of event.
			if( evt.GetName() == "JSONEvent")
			{
				//JSONObject gameData = evt.GetData() as JSONObject;
				
				// Look for the Event Name and react properly. Use
				switch ( evt.GetCallName() )
				{
				case "LoadGameDataComplete":

					string text =  WebUtils.DecodeUnicodeString( gameDataManager.GetString(StringKey) );

					if( text != null )
					{
						if( Uppercase )
						{
							label.text = text.ToUpper();
						}
						else
						{
							label.text = text;
						}

					}					
					break;
				}
			}
			
			return isEventConsumed;
		}
	}
}
