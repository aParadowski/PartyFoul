using UnityEngine;
using System.Collections;

public class assassinCamScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Debug.Log("AssassinCam here, Looking for assassin");
		//GameObject.FindGameObjectWithTag("Assassin")
		this.transform.parent = GameObject.FindGameObjectWithTag("Assassin").transform;
		//GameObject.FindGameObjectWithTag("Assassin").transform = this.transform;
		Debug.Log("Assassin Cam parent is " + this.transform.parent);
		if(!networkView.isMine)
		{
			Debug.Log("assassin Cam Network view is not mine");	
			enabled = false;
		}
		else
			enabled = true;
	}
	
	// Update is called once per frame
	void Update () 
	{

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		Debug.DrawLine(ray.origin, Camera.main.transform.forward * 1000, Color.red,20,true);
		//Debug.DrawRay(
		if(Physics.Raycast(ray, out hit, 1000))
		{
			Debug.Log("Im aiming at the player");
		}
	}
}
