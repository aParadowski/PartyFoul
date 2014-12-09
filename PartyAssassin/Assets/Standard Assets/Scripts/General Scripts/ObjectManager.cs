//Attached to interactable objects in the game area for their manipulation


using UnityEngine;
using System.Collections;

public class ObjectManager : MonoBehaviour {
	public bool isInteractable;
	public bool isObjective;
	public bool isBugged;
	public bool isInUse;
	

	// Use this for initialization
	void Start () 
	{
		if(transform.gameObject.tag == "Objective")
		{
			isObjective = true;	
		}
		else
		{
			isObjective = false;
		}
		isBugged = false;
		isInUse = false;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
