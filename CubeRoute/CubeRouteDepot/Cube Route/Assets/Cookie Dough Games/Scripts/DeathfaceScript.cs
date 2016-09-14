using System.Collections;
using UnityEngine;

public class DeathfaceScript : MonoBehaviour
{
	private Vector3 p1StartPosition;
	private Vector3 p1StartRotation;
	private Vector3 p2StartPosition;
	private Vector3 p2StartRotation;

	private void Start()
	{
		p1StartPosition = new Vector3( 0, 3.5f, -8 );
		p1StartRotation = new Vector3( 0, 0, 0 );

		p2StartPosition = new Vector3( 0, 3.5f, 8 );
		p2StartRotation = new Vector3( 0, 0, 0 );
	}

	private void OnCollisionEnter( Collision c )
	{
		//Debug.Break ();
		Player player = c.gameObject.GetComponent<Player>();
		if ( player == null ) return;

		Instantiate( CompetitveManager.Instance.DeathExplosionPrefab, player.transform.position, player.transform.rotation );

		int playerType = player.NetworkType == NetworkType.Server ? 1 : 2;

		if ( player.CompareTag( "Player" ) )
		{
			GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag( "SpawnPoint" );

			if ( spawnPoints.Length == 0 )
			{
				Debug.LogError( "Spawn point could not be found!" );
				return;
			}

			//			GameObject spawnPoint = spawnPoints[Random.Range( 0, spawnPoints.Length )];
			//			Vector3 spawnGravity = spawnPoint.GetComponent<SpawnGravityInformation>().SpawnGravity;

			EventManager.RaiseDeathEvent( playerType );
			EventManager.RotateGravity( new GravityEvent( new Vector3( 0, -20, 0 ), playerType ) );
			//
			//			other.GetComponent<GravityScript>().Reset();
			//			other.transform.position = spawnPoint.transform.position;
			//			other.transform.rotation = spawnPoint.transform.rotation;

			if ( playerType == 1 )
			{
				player.GetComponent<GravityScript>().Reset();
				player.transform.position = p1StartPosition;
				//player.transform.eulerAngles = p1StartRotation;
			}
			else if ( playerType == 2 )
			{
				player.GetComponent<GravityScript>().Reset();
				player.transform.position = p2StartPosition;
				//player.transform.eulerAngles = p2StartRotation;
			}
		}
	}
}