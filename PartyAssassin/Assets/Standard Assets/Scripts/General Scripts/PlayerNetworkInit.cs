using UnityEngine;
using System.Collections;

public class PlayerNetworkInit : MonoBehaviour {

	[SerializeField] Behaviour[] behavioursEnabledOnLocalClientsOnly;
	[SerializeField] Behaviour[] behavioursEnabledOnRemoteClientsOnly;
	[SerializeField] GameObject[] gameObjectsEnabledOnLocalClientsOnly;
	[SerializeField] GameObject[] gameObjectsEnabledOnRemoteClientsOnly;
	 
	void OnNetworkInstantiate( NetworkMessageInfo msg )
	{
		if( !networkView.isMine )
		{
			name += "(remote)";
		}
		 
		foreach( Behaviour behaviour in behavioursEnabledOnLocalClientsOnly )
		{
			behaviour.enabled = networkView.isMine;
		}
		foreach( Behaviour behaviour in behavioursEnabledOnRemoteClientsOnly )
		{
			behaviour.enabled = !networkView.isMine;
		}
		 
		foreach( GameObject go in gameObjectsEnabledOnLocalClientsOnly )
		{
			go.active = networkView.isMine;
		}
		foreach( GameObject go in gameObjectsEnabledOnRemoteClientsOnly )
		{
			go.active = !networkView.isMine;
		}
	}
}
