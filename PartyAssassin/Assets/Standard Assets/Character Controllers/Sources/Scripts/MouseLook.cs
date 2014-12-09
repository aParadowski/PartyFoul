using UnityEngine;
using System.Collections;

/// MouseLook rotates the transform based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character:
/// - Create a capsule.
/// - Add the MouseLook script to the capsule.
///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
/// - Add FPSInputController script to the capsule
///   -> A CharacterMotor and a CharacterController component will be automatically added.

/// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
/// - Add a MouseLook script to the camera.
///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour {

	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;

	private Quaternion originalRotation;

	public float minimumX = -360F;
	public float maximumX = 360F;

	public float minimumY = -60F;
	public float maximumY = 60F;

	float rotationY = 0F;
	float rotationX = 0F;
	
	public GameObject assassinCam;
	public GameObject assassin;

	public GameObject spyCam;
	public string secondsLeft;
	//public LineRenderer laser;
	public bool isFiring = false;
	public GameObject zoomSight;
	public GameObject gun;

	void Awake ()
	{
		spyCam = GameObject.FindGameObjectWithTag("SpyView");
		gun = GameObject.Find("Gun");
		assassin = GameObject.FindGameObjectWithTag("Assassin");
		zoomSight = GameObject.Find("SniperZoom");
		zoomSight.guiTexture.enabled = false;
		assassinCam = GameObject.FindGameObjectWithTag("AssassinView");

		if(networkView.isMine)
		{
			enabled = true;
			//Debug.Log("Reading from mouselook");
			
			//Debug.Log("Network view is mine in spy movement");
			//cc.Move(new Vector3(Input.GetAxis("Horizontal") * speed * Time.deltaTime, -gravity * Time.deltaTime, Input.GetAxis("Vertical") * speed * Time.deltaTime));
		}
		else
		{
			enabled = false;	
		}
		// Make the rigid body not change rotation
		if (rigidbody)
			rigidbody.freezeRotation = true;
	//	originalRotation = transform.localRotation;
	}
	
	void Update ()
	{

		secondsLeft = GameObject.FindGameObjectWithTag("SpyView").GetComponent<SpyCameraScript>().currentTime;
		if(isFiring == true)
		{

			StartLaser();
		}
		else
			GameObject.Find("Gun").transform.GetComponent<GunScript>().networkView.RPC("StopLaser",RPCMode.All);

		

		if (axes == RotationAxes.MouseXAndY)
		{
			// Read the mouse input axis
			rotationX += Input.GetAxis("Mouse X") * sensitivityX;
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			
			rotationX = Mathf.Clamp (rotationX, minimumX, maximumX);
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
			Quaternion xQuaternion = Quaternion.AngleAxis (rotationX, Vector3.up);
			Quaternion yQuaternion = Quaternion.AngleAxis (rotationY, -Vector3.right);
			
			transform.localRotation = originalRotation * xQuaternion * yQuaternion;
		

			/*rotationX += Input.GetAxis("Mouse X") * sensitivityX;
			rotationX = Mathf.Clamp (rotationX, minimumX, maximumX);
			
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
			transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);*/
		}
		else if (axes == RotationAxes.MouseX)
		{
			transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
		}
		else
		{
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
			transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
		}

		if(Input.GetKeyDown(KeyCode.F1)&& this.gameObject.tag=="Assassin")
		{
			//NetworkView. MarkAsSpy();
			//this.networkView.RPC("MarkAsSpy", RPCMode.All);
			SpyMark();
		}
		if(Input.GetKeyDown(KeyCode.F2)&& this.gameObject.tag=="Assassin")
		{
			//this.networkView.RPC ("MarkAsInnocent",RPCMode.All);
			InnocentMark();
		}
	}

	public static float ClampAngle(float angle, float min,float max  ) 
	{
		if (angle < -360.0f)
			angle += 360.0f;
		if (angle > 360.0f)
			angle -= 360.0f;
		return Mathf.Clamp (angle, min, max);
	}
	
	
	public void StartLaser()
	{	
		RaycastHit hit;
	
		if(Physics.Raycast(Camera.main.transform.position,Camera.main.transform.forward * 1000,out hit))
		{
		
			if(hit.collider.transform.tag =="Spy")
			{
				GameObject.FindGameObjectWithTag("SpyView").GetComponent<SpyCameraScript>().networkView.RPC ("SignalSpyInSight", RPCMode.All);
				gun.GetComponent<GunScript>().onSpy=true;
			}
			else
			{
				GameObject.FindGameObjectWithTag("SpyView").GetComponent<SpyCameraScript>().networkView.RPC ("StopSpyInSight", RPCMode.All);
				gun.GetComponent<GunScript>().onSpy=false;
			}
			GameObject.Find("Gun").transform.GetComponent<GunScript>().networkView.RPC("ShootLaser",RPCMode.All,hit.point);
			Camera.main.fieldOfView = 10.0f;
		}
	}

	//[RPC]
	public void SpyMark()//allows the assassin to mark the current target as a possible spy
	{
		RaycastHit hit;

		if(Physics.Raycast(Camera.main.transform.position,Camera.main.transform.forward * 1000,out hit))
		{
			if(hit.collider != null)
			{
			if(hit.collider.transform.tag == "NPC" || hit.collider.transform.tag == "Spy" )
			{

				hit.collider.transform.GetComponent<SpyMovement>().networkView.RPC ("MarkAsSpy", RPCMode.All);
			}
		}
		}
	}

	//[RPC] 
	public void InnocentMark()//allows the assassin to mark the current target as a safe NPC
	{
		RaycastHit hit;

		if(Physics.Raycast(Camera.main.transform.position,Camera.main.transform.forward * 1000,out hit))
		{
			if(hit.collider!=null)
			{
			if(hit.collider.transform.tag == "NPC" || hit.collider.transform.tag == "Spy" )
			{
				hit.collider.transform.GetComponent<SpyMovement>().networkView.RPC ("MarkAsInnocent", RPCMode.All);
				//hit.collider.transform.GetComponent<SampleAI>().networkView.RPC ("MarkAsInnocent", RPCMode.All);
			}
		}
		}
	}


	//only displays the sight if assassin is zoomed in
	void OnGUI()
	{

		GUI.Label(new Rect(Screen.width - 100, Screen.height - 20, 100, 20), "Shots left: " + gun.GetComponent<GunScript>().shotsLeft);
		GUI.Label(new Rect( 0, Screen.height - 20, 100, 20), "Time left: " + secondsLeft);

		if(isFiring)
		{
			//zoomSight.guiTexture.enabled = true;
			sensitivityY = 3.0f;
			sensitivityX = 0.1f;
			//assassin.GetComponent<MouseLook>().sensitivityX = 2.0f;
			GUI.DrawTexture(new Rect(0,0,Screen.width, Screen.height), zoomSight.guiTexture.texture);

		}
		else
		{
			sensitivityY = 5.0f;
			sensitivityX = 5.0f;
			//assassin.GetComponent<MouseLook>().sensitivityX = 5.0f;
		}
	}
}