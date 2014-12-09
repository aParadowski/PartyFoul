/*
 *Script used for basic AI and to practice simple AI skills
 *Written: AJ, March 2014
 * 
 *
 */

using UnityEngine;
using System.Collections;

public class SampleAI : MonoBehaviour {

	public float timeAtTarget;
	public bool atTarget;
	public Transform target;
	public GameObject TargetHolder;
	public NavMeshAgent agent;
	public bool hasTarget;
	public Vector3 velocity;
	public Vector3 previous;

	
	Animator animator;
	// Use this for initialization
	void Start () 
	{
		animator = GetComponent<Animator>();
		TargetHolder = GameObject.Find("TargetHolder");
		agent = GetComponent<NavMeshAgent>();
		FindNextTarget();

	}
	
	// Update is called once per frame
	void Update () 
	{
		velocity = (transform.position - previous) / Time.deltaTime; 
		previous = transform.position;
			// We are grounded, so recalculate
			// move direction directly from axes
			//moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			
			animator.SetFloat("MovementX", velocity.x);
			animator.SetFloat("MovementZ", velocity.z);
			
		if(hasTarget)
		{
			agent.SetDestination(target.position);
			CheckPath();
			if(atTarget)
			{
				StartCoroutine(WaitAtTarget(timeAtTarget));
			}
		}
		if(!hasTarget)
		{
			FindNextTarget();
		}
	}

	//after x amount of seconds, find a new thing to do
	IEnumerator WaitAtTarget(float waitTime)
	{
		//Debug.Log("At Current Target");
		yield return new WaitForSeconds(waitTime);
		//Debug.Log("Leaving Target");
		setAtTargetFalse();

	}

	void FindNextTarget()
	{
		int randTarget;

		//Debug.Log("Getting a new target");
		randTarget = Random.Range(0,TargetHolder.GetComponent<TargetHolderScript>().targetList.Length);

		if(TargetHolder.GetComponent<TargetHolderScript>().targetList[randTarget].GetComponent<ObjectManager>().isInUse == false && hasTarget == false)
			{
				//Debug.Log("I have a new target");
				target = TargetHolder.GetComponent<TargetHolderScript>().targetList[randTarget];
				target.GetComponent<ObjectManager>().isInUse = true;
				//CURRENTLY AFFECTING ALL TARGETS might have to use the targetholder to access it at the index instead of what is happening now
				hasTarget = true;
			}

	}
	void setAtTargetFalse()
	{
		//Debug.Log("Setting at target false");
		hasTarget = false;
		atTarget = false;
		target.GetComponent<ObjectManager>().isInUse = false;
	}
	void CheckPath()
	{
		//float dist = agent.remainingDistance;
		Vector3 dist = transform.position - target.transform.position;
		float sqrLen = dist.sqrMagnitude;


		//Debug.Log("dist = " + sqrLen);
		//if(dist!= Mathf.Infinity && agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance == 0)
		if(sqrLen <= 3.5f)
		{
			atTarget = true;
		}
	}
}
