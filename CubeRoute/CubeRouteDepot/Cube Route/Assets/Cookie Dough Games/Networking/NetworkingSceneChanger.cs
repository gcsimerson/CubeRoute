using BeardedManStudios.Network;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkingSceneChanger : SimpleNetworkedMonoBehaviorSingleton<NetworkingSceneChanger>
{
	/// <summary>
	/// The name of the main menu scene.
	/// </summary>
	public static readonly string MainMenuScene = "MainMenu";

	/// <summary>
	/// The name of the competitive scene.
	/// </summary>
	public static readonly string CompetitiveScene = "Spires";

	/// <summary>
	/// A list of all single player scenes.  Note that the ordering of these scenes is crucial.
	/// </summary>
	public static readonly List<string> SoloScenes = new List<string>() { "Level 1 - Roam", "Level 3 - Bridge", "Level 4 - Slide", "Level 2 - Fiddle" };

	/// <summary>
	/// A list of all cooperative scenes.  Note that the ordering of these scenes is crucial.
	/// </summary>
	public static readonly List<string> CoopScenes = new List<string>() { "MP Coop Level 01", "MP Coop Level 02", "MP Coop Level 03", "MP Coop Level 04", "MP Coop Level 05" };

	/// <summary>
	/// In the case of the server: force disconnect all connected clients, shut down the server connection, and return to main menu.
	/// In the case of the client: disconnect from server and return to main menu.
	/// </summary>
	public static void ExitToMainMenu()
	{
		// TODO This may not be necessary, but adding it to try to make sure client are kicked to main menu
		if ( Networking.PrimarySocket != null && Networking.PrimarySocket.IsServer )
		{
			Debug.Log( "[SERVER] Shutting down server for " + Networking.PrimarySocket.Players.Count + " clients" );

			Cursor.visible = true;

			foreach ( NetworkingPlayer player in Networking.PrimarySocket.Players )
			{
				player.PlayerObject.OwningNetWorker.Disconnect( player, "Server is being shut down." );
			}
		}

		Networking.Disconnect();
		SceneManager.LoadScene( MainMenuScene );
	}

	public static bool IsMainMenu()
	{
		return SceneManager.GetActiveScene().name == MainMenuScene;
	}

	public static bool IsSoloScene()
	{
		return ( SoloScenes.Contains( SceneManager.GetActiveScene().name ) );
	}

	public static bool IsCoopScene()
	{
		return ( CoopScenes.Contains( SceneManager.GetActiveScene().name ) );
	}

	public static bool IsCompetitiveScene()
	{
		return SceneManager.GetActiveScene().name == CompetitiveScene;
	}

	public void ReloadLevel()
	{
		if ( IsOfflineOrServer )
			SceneManager.LoadScene( SceneManager.GetActiveScene().name );
	}

	/// <summary>
	/// This method will automatically determine if we're on a single player or coop level, and then automatically load the next sequential level.
	/// In the case of this level being the last, it will return to the main menu.
	/// </summary>
	public void LoadNextLevel()
	{
		if ( IsSoloScene() )
		{
			int index = SoloScenes.FindIndex( ele => ele == SceneManager.GetActiveScene().name );
			index++;
			if ( index == SoloScenes.Count )
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
				ExitToMainMenu();
			}
			else
			{
				SceneManager.LoadScene( SoloScenes[index] );
			}
		}
		else if ( IsCoopScene() && IsServer )
		{
			int index = CoopScenes.FindIndex( ele => ele == SceneManager.GetActiveScene().name );
			index++;
			if ( index == CoopScenes.Count )
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
				ExitToMainMenu();
			}
			else
			{
				SceneManager.LoadScene( CoopScenes[index] );
			}
		}
	}

	public void Start()
	{
		if ( IsMainMenu() )
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
		else
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		if ( IsSoloScene() && SoloScenes.FindIndex( ele => ele == SceneManager.GetActiveScene().name ) != 0 )
		{
			AudioManager.Instance.PlaySound( AudioManager.SoundType.Gameplay_LevelComplete_0 );
		}
		else if ( IsCoopScene() && CoopScenes.FindIndex( ele => ele == SceneManager.GetActiveScene().name ) != 0 )
		{
			AudioManager.Instance.PlaySound( AudioManager.SoundType.Gameplay_LevelComplete_0 );
		}

		if ( IsServer && ( IsCoopScene() || IsCompetitiveScene() ) ) // Added CompetitiveScene check in case of hitting 'reset'
		{
			Debug.Log( "[SERVER] Informing " + Networking.PrimarySocket.Players.Count + " clients to switch to scene: " + SceneManager.GetActiveScene().name );

			foreach ( NetworkingPlayer player in Networking.PrimarySocket.Players )
			{
				Networking.ChangeClientScene( Networking.PrimarySocket, player, SceneManager.GetActiveScene().name );
			}
		}
	}
}