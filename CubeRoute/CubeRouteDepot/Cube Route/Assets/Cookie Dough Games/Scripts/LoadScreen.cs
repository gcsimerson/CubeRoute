using BeardedManStudios.Network;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadScreen : Singleton<LoadScreen>
{
	public bool UseWaitScreen = true;
	public Image LoadImage;
	public Text LoadText;
	public Text WaitingText;

	private Coroutine activeCoroutine;

	// TODO Consider making it so it displays loading before beginning scene change
	private void OnLevelWasLoaded( int level )
	{
		// If we loaded main menu (or if this is a single player level), make sure that all UI elements are disabled
		if ( NetworkingSceneChanger.IsMainMenu() || NetworkingSceneChanger.IsSoloScene() )
		{
			HideAll();
		}
		else // We just loaded a coop or competitive scene
		{
			if ( activeCoroutine != null )
			{
				StopCoroutine( activeCoroutine );
			}
			CancelInvoke();

			//			LoadImage.GetComponent<CanvasRenderer>().SetAlpha( 1 );
			LoadImage.CrossFadeAlpha( 1, 0, false );
			LoadImage.gameObject.SetActive( true );

			//			LoadText.GetComponent<CanvasRenderer>().SetAlpha( 1 );
			LoadText.CrossFadeAlpha( 1, 0, false );
			LoadText.gameObject.SetActive( true );

			if ( UseWaitScreen && Networking.PrimarySocket.IsServer && Networking.PrimarySocket.Players.Count == 0 )
			{
				activeCoroutine = StartCoroutine( WaitForOtherPlayer() );
			}

			Invoke( "FadeIn", PlayerSpawner.FixedSpawnDelay );
		}
	}

	private IEnumerator WaitForOtherPlayer()
	{
		WaitingText.gameObject.SetActive( true );

		GameObject player = null;

		while ( Networking.PrimarySocket == null || Networking.PrimarySocket.Players.Count == 0 )
		{
			if ( player == null )
			{
				player = GameObject.FindGameObjectWithTag( "Player" );
				if ( player != null )
				{
					player.GetComponent<PlayerController>().enabled = false;
					player.GetComponent<GravityScript>().enabled = false;
				}
			}

			yield return null;
		}

		if ( player != null )
		{
			player.GetComponent<PlayerController>().enabled = true;
			player.GetComponent<GravityScript>().enabled = true;
		}

		WaitingText.gameObject.SetActive( false );

		activeCoroutine = null;
	}

	private void FadeIn()
	{
		LoadImage.CrossFadeAlpha( 0, 1.5f, false );
		LoadText.CrossFadeAlpha( 0, 1.5f, false );
	}

	private void HideAll()
	{
		LoadImage.gameObject.SetActive( false );
		LoadText.gameObject.SetActive( false );
		WaitingText.gameObject.SetActive( false );
	}
}