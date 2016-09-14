using BeardedManStudios.Network;
using UnityEngine;

public class PlayerSpawner : SimpleNetworkedMonoBehaviorSingleton<PlayerSpawner>
{
	public const float FixedSpawnDelay = 5;

	[Header( "Player Spawner" )]
	public NetworkedMonoBehavior PlayerPrefab;

	public Material BluePlayerMaterial;

	public Material YellowPlayerMaterial;
	private bool initialized = false; // Hackish work-around due to PrimarySocket being null during load

	private void FixedUpdate()
	{
		if ( !initialized && IsSetup && IsOnline )
		{
			initialized = true;

			Invoke( "SpawnPlayer", FixedSpawnDelay );
		}
	}

	private void SpawnPlayer()
	{
		print( "Spawning player." );

		GameObject spawnPoint = GameObject.FindGameObjectWithTag( IsServer ? "P1Spawn" : "P2Spawn" );

		Networking.Instantiate( PlayerPrefab.gameObject, spawnPoint.transform.position, spawnPoint.transform.rotation, NetworkReceivers.AllBuffered, OnCreated );
	}

	private void OnCreated( SimpleNetworkedMonoBehavior newObject )
	{
		print( "Successfully created " + ( IsServer ? NetworkType.Server : NetworkType.Client ) + " player." );

		newObject.GetComponent<Player>().UpdateNetworkType( IsServer ? NetworkType.Server : NetworkType.Client );
	}
}