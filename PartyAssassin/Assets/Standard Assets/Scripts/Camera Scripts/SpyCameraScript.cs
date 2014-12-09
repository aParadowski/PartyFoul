using UnityEngine;
using System.Collections;

public class SpyCameraScript : MonoBehaviour {
	
	private Transform spyPlayer;

	public GameObject spyChar;
	public float maxDistanceFromPlayer;
	public float minDistanceToPlayer;
	public float heightFromChar = 3.0f;
	public float transitionSpeed;
	public float moveSpeed;
	public float maxRotation;
	public float rotateSpeed;
	public float damping = 6.0f;
	public bool smooth = true;
	public bool followSpy;
	public Camera spyCam;
	public float seconds;
	public string currentTime;
	

	public bool foundSpy = false;
	public bool inSights;
	GUITexture warning;
	
	
	private float distanceToPlayer;
	private float speed;
	private float startY;
	private float defaultFOV;
	private float angle;
	
	Vector3 offset;
	private bool moving;
	// Use this for initialization
	void Start() 
	{
		Debug.Log("Spy Cam is here");


		offset = spyChar.transform.position - transform.position;
		if(foundSpy == false)
		{

			if(GameObject.FindWithTag("Spy").transform == null)
			{
				foundSpy = false;
				FindSpy();
			}
			else
			foundSpy = true;
			spyPlayer = GameObject.FindWithTag("Spy").transform;
		}
		if(foundSpy == true)
		{
			
			startY = spyPlayer.transform.position.y;
		}
		defaultFOV = camera.fieldOfView;
	}
	
	// Update is called once per frame
	void Update()
	{
	

		networkView.RPC("UpdateTime", RPCMode.All);

		if(!networkView.isMine)
   	 	{
    		enabled = false;
    	}

	}

	void LateUpdate()
	{
		float desiredAngle = spyChar.transform.eulerAngles.y;
		Quaternion rotation = Quaternion.Euler(0, desiredAngle, 0);

		transform.position = spyChar.transform.position - (rotation * offset);
		transform.LookAt(spyChar.transform);


	}
	public void FindSpy()
	{

		if(foundSpy == false)
		{
			//spyPlayer = GameObject.Find("SpyPlayer").transform;
			if(GameObject.Find("SpyPlayer").transform == null)
			{
				foundSpy = false;
				FindSpy();
			}
			else
			{
				spyPlayer = GameObject.Find("SpyPlayer").transform;
				foundSpy = true;
			}
		}
	}
	[RPC]
	public void UpdateTime()
	{
		seconds = seconds - Time.deltaTime;
		currentTime = string.Format("{0:0.0}", seconds);
		if(seconds <= 0.0f)
		{
			networkView.RPC("SpyWinsTheGame", RPCMode.All);
		}
	}

	//rpc function for gunscript to tell me im in his sights
	[RPC]
	public void SignalSpyInSight()
	{
		inSights = true;	
		
	}
	[RPC]
	public void StopSpyInSight()
	{
		inSights = false;	
	}
	[RPC]
	public void SpyWinsTheGame()
	{
		Application.LoadLevel("SpyWin");
	}
	void OnGUI()
	{
		warning = transform.GetComponent<GUITexture>();
		GUI.Label(new Rect( 0, Screen.height - 20, 100, 20), "Time left: " + currentTime);
	
		if(inSights)
		{
			//Debug.Log("IM GETTIN SHAWT AT");
			GUI.DrawTexture(new Rect(0,0,Screen.width, Screen.height), warning.texture);
		}
		
	}
}
