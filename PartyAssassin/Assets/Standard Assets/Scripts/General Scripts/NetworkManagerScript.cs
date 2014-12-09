using UnityEngine;
using System.Collections;

public class NetworkManagerScript : MonoBehaviour {
	
	//public GameObject SpyPrefab;
	public GameObject assassinPrefab;
	//public GameObject spyCamera;
	public GameObject assassinCam;
	public Transform[] spySpawns = new Transform[4];
	public GameObject[] spyPrefabs = new GameObject[3];

	public Transform AssassinSpawn;
	
	float btnX;
	float btnY;
	float btnWidth;
	float btnHeight;
	
	private string gameName = "PartyFoul_SpyVsAssassin";
	private bool refreshing = false;
	private HostData[] hostData;

	// Use this for initialization
	void Start () {
		btnX = Screen.width * 0.05f;
		btnY = Screen.width * 0.05f;
		btnHeight = Screen.width * 0.1f;
		btnWidth = Screen.width * 0.1f;

	}
	
	// Update is called once per frame
	void Update () 
	{
		if(refreshing)
		{
			if(MasterServer.PollHostList().Length>0)
			{
				refreshing = false;
				Debug.Log(MasterServer.PollHostList().Length);
				hostData = MasterServer.PollHostList();
			}
		}
	}
	void StartServer()
	{
		Network.InitializeServer(2,25000, !Network.HavePublicAddress());	
		//register to unity master server, free to use
		MasterServer.RegisterHost(gameName,"PartyFoul","Host for 2-Player Game Party Foul");
	}
	
	void RefreshHostList()
	{	
		MasterServer.RequestHostList(gameName);
		refreshing = true;
		//Invoke("MasterServer.PollHostList()",1.5f);
		//Debug.Log(MasterServer.PollHostList().Length);
		
	}

	void SpawnSpy()
	{
		int spawnNum;
		int spyPrefNum;

		if(GameObject.FindWithTag("Spy")==null)
		{
			Debug.Log("Spawning Spy");
			spawnNum = Random.Range(0,spySpawns.Length);
			spyPrefNum = Random.Range(0,spyPrefabs.Length);

			Network.Instantiate(spyPrefabs[spyPrefNum], spySpawns[spawnNum].position, Quaternion.identity, 0);
		}
		else if(GameObject.FindWithTag("Spy")!=null)
		{
				Debug.Log("Spy already exists");
		}

		//Debug.Log("spawned player");
		//Debug.Log("Player prefab name: " + SpyPrefab.name);
		//Network.Instantiate(spyCamera, SpyPrefab.transform.position, Quaternion.identity, 0);
		//Debug.Log("spawned Camera");

	}
	
	void SpawnAssassin()
	{
		if(GameObject.FindWithTag("Assassin")==null)
		{
			Debug.Log("Spawning Assassin");
			Network.Instantiate(assassinPrefab, AssassinSpawn.position, Quaternion.identity, 0);
			
		}
		else if(GameObject.FindWithTag("Assassin")!=null)
			{
				Debug.Log("assassin already exists");
			}
	}
	
	

	//Messages
	void OnServerInitialized()
	{
		Debug.Log("Server is ALIVE");
		SpawnAssassin();//only to test camera scripts and what not
		//SpawnSpy();
		GameObject.Find("NPCSpawner").GetComponent<NPCSpawnScript>().SpawnNPCs();
	}
	
	void OnConnectedToServer()
	{
		Debug.Log("Connected to server");
		//SpawnAssassin();	
		SpawnSpy();
		//GameObject.Find("NPCSpawner").GetComponent<NPCSpawnScript>().SpawnNPCs();

	}


	void OnMasterServerEvent(MasterServerEvent mse)
	{
		if(mse== MasterServerEvent.RegistrationSucceeded)
		{
			Debug.Log("Registered the server");
		}
	}
	
	//GUI Buttons
	void OnGUI()
	{
		if(!Network.isClient && !Network.isServer)
		{
			if(GUI.Button(new Rect(btnX,btnY,btnWidth,btnHeight), "Start Server"))
			{
				Debug.Log("starting server");
				StartServer();
			}
			if(GUI.Button(new Rect(btnX,btnY * 5.0f,btnWidth,btnHeight), "Refresh Hosts"))
			{
				Debug.Log("Refreshing");
				RefreshHostList();
			}
			if(hostData!=null){
				for(int i = 0; i < hostData.Length; i++)
				{
					if(GUI.Button(new Rect(btnX * 1.5f + btnWidth, btnY * 1.2f + (btnHeight *i), btnWidth*3.0f, btnHeight* 0.5f), hostData[i].gameName))
					{
						Network.Connect(hostData[i]);
					}
				}
			}
		}
	}
}
