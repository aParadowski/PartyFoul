using UnityEngine;
using System.Collections;

public class GunScript : MonoBehaviour {
	public LineRenderer laser;
	public bool isFiring = false;
	public GUITexture zoomSight;
	public GameObject cam;
	public int shotsLeft;
	public CameraShake shake;
	public bool onSpy;
	public GameObject assassin;

	// Use this for initialization
	void Start () 
	{
		cam = GameObject.FindWithTag("MainCamera");
		shake = cam.GetComponent<CameraShake>();
		shake.enabled = false;
		laser = transform.GetComponent<LineRenderer>();
		assassin = GameObject.FindGameObjectWithTag("Assassin");
		transform.position = GameObject.FindGameObjectWithTag("Assassin").transform.position;
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetMouseButtonDown(0) && shotsLeft>0)
		{
			if(onSpy)
			{
				assassin.GetComponent<Assassin>().networkView.RPC("AssassinWins", RPCMode.All);		
			}

			StartCoroutine(ShakeTime(0.2f));
		}

		
	}

	IEnumerator ShakeTime(float timeToShake)
	{

		shake.enabled = true;
		yield return new WaitForSeconds(timeToShake);
		RemoveShot(shotsLeft);
		shake.enabled = false;

	}

	public void RemoveShot(int shots)
	{
		shotsLeft = shots-1;

	}

	[RPC]
	public void ShootLaser(Vector3 endPoint)
	{	

		//RaycastHit hit;
		Vector3 end;
		end = endPoint;

		laser.SetColors(Color.red,Color.red);
		laser.enabled = true;
		laser.SetVertexCount(2);
		laser.SetWidth(1.0f,1.0f);

		laser.SetPosition(0,(cam.transform.position + Vector3.down));//  transform.position);
		laser.SetPosition(1, end);
		
	}
	[RPC]
	public void StopLaser()
	{
		laser = transform.GetComponent<LineRenderer>();
		laser.enabled = false;
		if(GameObject.FindGameObjectsWithTag("SpyView")== null)
		{
			Debug.Log("Wait...");
		}
		else
			GameObject.FindGameObjectWithTag("SpyView").GetComponent<SpyCameraScript>().networkView.RPC ("StopSpyInSight", RPCMode.All);
	}
}
