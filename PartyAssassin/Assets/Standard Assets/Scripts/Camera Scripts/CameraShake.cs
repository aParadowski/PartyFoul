using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour {
	public GameObject assassinCam;

	public float shake = 0.0f;
	public float shakeAmt = 1.7f;

	Vector3 originalPos;
	// Use this for initialization
	void Start () 
	{
		assassinCam = GameObject.FindWithTag("MainCamera");
	}

	void onEnable()
	{
		originalPos = assassinCam.transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () 
	{
		//assassinCam = GameObject.FindWithTag("AssassinView");
		if(shake > 0)
		{
			assassinCam.transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmt;
		}
		else
		{
			shake = 0.0f;
			assassinCam.transform.localPosition = originalPos;

		}
	}

}
