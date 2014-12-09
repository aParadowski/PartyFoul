/*
 * Script to spawn NPC's at predetermined locations (ie the spy spawn locations)
 * should pick a random character prefab to load which will already have AI script
 * attached to it
 * 
 * 
 */


using UnityEngine;
using System.Collections;

public class NPCSpawnScript : MonoBehaviour {
	public Transform[] NPCSpawnLocations = new Transform[4];
	public GameObject[] NPCList = new GameObject[2];
	public int numNPC;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SpawnNPCs()
	{
		int prefabNum;
		int spawnLocation;

		Debug.Log("Calling NPCSpawn");

		//for as many npcs are in numNPC
		for(int i =0; i < numNPC; i++)
		{
			Debug.Log("Spawning PEOPLE!");
			prefabNum = Random.Range(0, NPCList.Length);
			spawnLocation = Random.Range(0, NPCSpawnLocations.Length);
			Network.Instantiate(NPCList[prefabNum],NPCSpawnLocations[spawnLocation].position,Quaternion.identity,0);

		}
	}
}
