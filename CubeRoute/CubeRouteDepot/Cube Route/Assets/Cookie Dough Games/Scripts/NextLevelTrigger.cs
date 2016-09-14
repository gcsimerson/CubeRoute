using System.Collections.Generic;
using UnityEngine;

public class NextLevelTrigger : MonoBehaviour
{
	private HashSet<Player> collidingPlayers = new HashSet<Player>();

	private void OnTriggerEnter( Collider other )
	{
		Player p = other.GetComponent<Player>();
		if ( p != null )
		{
			// Require 2 players for coop
			if ( NetworkingSceneChanger.IsCoopScene() )
			{
				collidingPlayers.Add( p );

				if ( collidingPlayers.Count >= 2 )
				{
					NetworkingSceneChanger.Instance.LoadNextLevel();
				}
			}
			else
			{
				NetworkingSceneChanger.Instance.LoadNextLevel();
			}
		}
	}

	private void OnTriggerExit( Collider other )
	{
		Player p = other.GetComponent<Player>();
		if ( p != null )
		{
			if ( NetworkingSceneChanger.IsCoopScene() )
			{
				collidingPlayers.Remove( p );
			}
		}
	}
}