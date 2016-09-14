using BeardedManStudios.Network;
using UnityEngine;

/// <summary>
/// Which machines should be informed of the event?
/// </summary>
public enum EventReceivers { LocalOnly, LocalAndRemote, RemoteOnly };

/// <summary>
/// Represents one of the two different player types available in Cube Route.
/// </summary>
public enum PlayerType { Server, Client };

/// <summary>
/// This class is used to synchronize events across the network.  It can be used for non-networked events as well.
///
/// Example of how to raise an event: NetworkedEventManager.Instance.RaiseNewGravityChangeEvent( EventReceivers.LocalAndRemote, PlayerType.Server, Vector3.zero );
/// Example of how to register to an event: NetworkedEventManager.Instance.GravityChangeEvent += MyLocalMethod;
///
/// To modify this class to support a new event type, please copy an entire existing "region" as a base to start from.
///
/// When choosing which EventReceivers to use, pay careful attention to how each machine may perceive the game events locally.
/// For example, if raising an event on pressing "E", only that single machine will possibly detect that E input, so it may be relevant to raise it for other machines if needed.
/// However, if raising an event on a player colliding with a death zone, realize that both machines will potentially detect this happening locally, and sending it across the network may result in the raising of duplicate events.
/// </summary>
public class NetworkedEventManager : SimpleNetworkedMonoBehaviorSingleton<NetworkedEventManager>
{
	#region Gravity Change Event

	public delegate void GravityChange( PlayerType eventRaiser, PlayerType playerAffected, Vector3 newGravity );

	public event GravityChange GravityChangeEvent;

	public void RaiseNewGravityChangeEvent( EventReceivers receivers, PlayerType playerAffected, Vector3 newGravity )
	{
		PlayerType eventRaiser = IsServer ? PlayerType.Server : PlayerType.Client;

		if ( receivers == EventReceivers.LocalAndRemote || receivers == EventReceivers.LocalOnly )
		{
			DistributeGravityChangeEvent( (int)eventRaiser, (int)playerAffected, newGravity );
		}

		if ( IsOnline && ( receivers == EventReceivers.LocalAndRemote || receivers == EventReceivers.RemoteOnly ) )
		{
			RPC( "DistributeGravityChangeEvent", NetworkReceivers.Others, (int)eventRaiser, (int)playerAffected, newGravity );
		}
	}

	[BRPC]
	private void DistributeGravityChangeEvent( int eventRaiser, int playerAffected, Vector3 newGravity )
	{
		if ( GravityChangeEvent != null ) GravityChangeEvent( (PlayerType)eventRaiser, (PlayerType)playerAffected, newGravity );
	}

	#endregion Gravity Change Event

	#region Turn Change Event

	public delegate void TurnChange( PlayerType eventRaiser, PlayerType whoseTurn );

	public event TurnChange TurnChangeEvent;

	public void RaiseNewTurnChangeEvent( EventReceivers receivers, PlayerType whoseTurn )
	{
		PlayerType eventRaiser = IsServer ? PlayerType.Server : PlayerType.Client;

		if ( receivers == EventReceivers.LocalAndRemote || receivers == EventReceivers.LocalOnly )
		{
			DistributeTurnChangeEvent( (int)eventRaiser, (int)whoseTurn );
		}

		if ( IsOnline && ( receivers == EventReceivers.LocalAndRemote || receivers == EventReceivers.RemoteOnly ) )
		{
			RPC( "DistributeTurnChangeEvent", NetworkReceivers.Others, (int)eventRaiser, (int)whoseTurn );
		}
	}

	[BRPC]
	private void DistributeTurnChangeEvent( int eventRaiser, int whoseTurn )
	{
		if ( TurnChangeEvent != null ) TurnChangeEvent( (PlayerType)eventRaiser, (PlayerType)whoseTurn );
	}

	#endregion Turn Change Event
}