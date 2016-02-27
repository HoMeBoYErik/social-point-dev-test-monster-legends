using UnityEngine;
using System.Collections;

public class ButtonDebug : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnClick()
	{
		Debug.Log(this.gameObject.name + " Button was clicked");
	}
}
