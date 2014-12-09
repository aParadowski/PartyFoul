using UnityEngine;
using System.Collections;

public class Assassin : MonoBehaviour {

	// Use this for initialization
	public GameObject assassin;
	public GameObject assassinCam;
	public GameObject gun;
	
	void Start() {

		Debug.Log("Assasin is here");
		assassinCam = GameObject.Find("AssassinView");
		assassin = GameObject.FindWithTag("Assassin");
		gun = GameObject.Find("Gun");
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(networkView.isMine)
		{
			enabled = true;
		}
		else
		{
			enabled = false;	
		}

		if(Input.GetMouseButton(1))//if they right click, zoom in and add laser sight
		{
			assassinCam.GetComponent<MouseLook>().isFiring = true;
		}
		else
		{
			assassinCam.GetComponent<Camera>().fieldOfView = 60.0f;
			assassinCam.GetComponent<MouseLook>().isFiring = false;
		}
	}
	[RPC]
	public void AssassinWins()
	{
		Application.LoadLevel("AssassinWin");
	}
	
	[RPC]
	public void SpyWinsTheGame()
	{
		Application.LoadLevel("SpyWin");
	}
}
