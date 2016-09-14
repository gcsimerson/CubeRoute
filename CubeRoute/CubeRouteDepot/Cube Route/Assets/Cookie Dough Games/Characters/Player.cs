using BeardedManStudios.Network;
using UnityEngine;

public enum NetworkType { Offline, Server, Client };

[RequireComponent( typeof( Rigidbody ) )]
[RequireComponent( typeof( CapsuleCollider ) )]
public class Player : NetworkedMonoBehavior
{
	[Header( "Object References" )]
	public SkinnedMeshRenderer DummyMeshRenderer;

	public Transform HeadPivot;
	public NetworkType NetworkType = NetworkType.Offline;
	private Transform _cameraPivot = null;
	public Transform CameraPivot { get { return _cameraPivot ?? ( _cameraPivot = GameObject.FindGameObjectWithTag( "CameraPivot" ).transform ); } set { } }

	/// <summary>
	/// This will update the NetworkType variable of the player.
	/// It is automatically synced over the network.
	/// </summary>
	/// <param name="networkType"></param>
	public void UpdateNetworkType( NetworkType networkType )
	{
		ReceiveNewNetworkType( (int)networkType );
		RPC( "ReceiveNewNetworkType", NetworkReceivers.OthersBuffered, (int)networkType );
	}

	private static void SetLayers( GameObject go, int layerNumber )
	{
		foreach ( Transform trans in go.GetComponentsInChildren<Transform>( true ) )
		{
			trans.gameObject.layer = layerNumber;
		}
	}

	[BRPC]
	private void ReceiveNewNetworkType( int networkType )
	{
		NetworkType = (NetworkType)networkType;
		gameObject.name = NetworkType + " Player";

		if ( NetworkType == NetworkType.Client )
		{
			DummyMeshRenderer.material = PlayerSpawner.Instance.YellowPlayerMaterial;
		}
		else
		{
			DummyMeshRenderer.material = PlayerSpawner.Instance.BluePlayerMaterial;
		}
	}

	private void Start()
	{
		if ( IsSinglePlayerOrOwner )
			SetLayers( gameObject, 10 );
	}
}