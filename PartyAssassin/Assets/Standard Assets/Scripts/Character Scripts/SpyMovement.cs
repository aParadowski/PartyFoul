using UnityEngine;
using System.Collections;

public class SpyMovement : MonoBehaviour {
	
	
    public AnimationClip idleAnimation;
    public AnimationClip walkAnimation;
	public float movementSpeed = 10;
    public float turningSpeed = 600;
	//int gravity = 5;
	public bool hasSpawned = false;
	public bool inSights;
	public GameObject myCam;

	public GameObject assassin;

	public bool markedSpy = false;
	public bool markedInnocent = false;

 	enum CharacterState 
	{
        Idle = 0,
        Walking = 1,
    }
	
	private CharacterState _characterState;
	
	// Use this for initialization
	void Awake () {

		myCam = GameObject.FindGameObjectWithTag("SpyView");
		assassin = GameObject.FindGameObjectWithTag("Assassin");
		hasSpawned= true;
		Debug.Log("Spy has entered the game");
		
	}
	
	// Update is called once per frame
	void Update () {

		float horizontal = Input.GetAxis("Horizontal") * turningSpeed * Time.deltaTime;
        transform.Rotate(0, horizontal, 0);

         
        float vertical = Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;
        transform.Translate(0, 0, vertical);

	}
	
	[RPC]
	public void MarkAsSpy()
	{

		if(!markedSpy)
		{
			Debug.Log("Turning them red");
			foreach(Transform child in transform)
			{
				if(child.tag == "CharTexture")
				{
					for(int i = 0; i < child.GetComponent<SkinnedMeshRenderer>().materials.Length; i++)
					{
						child.GetComponent<SkinnedMeshRenderer>().materials[i].color = Color.red;
					}
				}
			}

			markedInnocent= false;
			markedSpy = true;	

		}
		else 
		{
			Debug.Log("Turning them white");
			foreach(Transform child in transform)
			{
				if(child.tag == "CharTexture")
				{
					//child.GetComponent<SkinnedMeshRenderer>().material.color = Color.red;

					for(int i = 0; i < child.GetComponent<SkinnedMeshRenderer>().materials.Length; i++)
					{
						child.GetComponent<SkinnedMeshRenderer>().materials[i].color = Color.white;
					}
				}
			}

			markedSpy = false;
			markedInnocent = false;
		}

	}
	[RPC]
	public void MarkAsInnocent()
	{
		
		if(!markedInnocent)
		{
			foreach( Transform child in transform)
			{
				if(child.tag == "CharTexture")
				{
					for(int i = 0; i < child.GetComponent<SkinnedMeshRenderer>().materials.Length; i++)
					{
						child.GetComponent<SkinnedMeshRenderer>().materials[i].color = Color.green;
					}

				}

			}
			markedInnocent= true;
			markedSpy = false;
		}
		else 
		{
			foreach( Transform child in transform)
			{
				if(child.tag == "CharTexture")
				{
					for(int i = 0; i < child.GetComponent<SkinnedMeshRenderer>().materials.Length; i++)
					{
						child.GetComponent<SkinnedMeshRenderer>().materials[i].color = Color.white;
					}
					
				}
				
			}
			markedSpy = false;
			markedInnocent=false;

		}
	}

}