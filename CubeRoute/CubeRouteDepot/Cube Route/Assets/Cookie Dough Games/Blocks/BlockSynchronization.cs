using BeardedManStudios.Network;
using UnityEngine;

/// <summary>
/// This script is used exclusively to synchronize gravity settings for blocks across the network.
/// The server will keep track of the server's gravity and the client's gravity.
/// Every individual block's fixed update will apply the relevant gravity, only by the server.
/// </summary>
public class BlockSynchronization : SimpleNetworkedMonoBehaviorSingleton<BlockSynchronization>
{
	private void OnEnable()
	{
		EventManager.Grav += OnGravityChange;
	}

	private void OnDisable()
	{
		EventManager.Grav -= OnGravityChange;
	}

	private void OnGravityChange( GravityEvent e )
	{
		if ( IsClient )
		{
			RPC( "UpdateClientGravityOnServer", NetworkReceivers.Server, e.GetGravityVec() );
			Debug.Log( "Sending " + e.GetGravityVec() + " to server." );
		}
	}

	[BRPC]
	private void UpdateClientGravityOnServer( Vector3 newGravityDirection )
	{
		Debug.Log( "Server received new gravity from client: " + newGravityDirection );
		EventManager.RotateGravity( new GravityEvent( newGravityDirection, 2 ) ); // 2 for client
	}
}