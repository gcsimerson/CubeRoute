using BeardedManStudios.Network;
using BeardedManStudios.Network.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VR;

public class InGameMenu : Singleton<InGameMenu>
{
	[Header( "In Game Menu" )]
	public RectTransform Panel;

    // Wii controller info, if we've got one.
    public WiiControllerInfo wiiInfo;

	public void PauseGame( Vector3 newTransform, Quaternion rotation )
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = !Cursor.visible;
        Cursor.lockState = CursorLockMode.Locked;
		Panel.gameObject.SetActive( !Panel.gameObject.activeSelf );
        if (VRDevice.isPresent)
        {
            Canvas canvas = gameObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            Panel.transform.position = newTransform;
            Panel.localScale = new Vector3( .005f, .005f );
            Panel.transform.rotation = rotation;
        }
	}

	private void Update()
	{
	    bool wiiPause = wiiInfo != null && wiiInfo.GetWiimoteData().GetButtonDown(WiimoteData.Buttons.Home);
        bool pause = Input.GetKeyDown( KeyCode.Escape );

        if ( pause )
        {
            InGameMenu.Instance.PauseGame( Camera.main.transform.position + Camera.main.transform.forward, Camera.main.transform.rotation );
        }

//        if ( Input.GetKeyDown( KeyCode.Escape ) ) // TODO wii pause
//            {
//                PauseGame(null);
//            }
	}

	public void ReturnToMainMenu()
	{
		Panel.gameObject.SetActive( false );
		NetworkingSceneChanger.ExitToMainMenu();
	}

	public void ResetScene()
	{
		Panel.gameObject.SetActive( false );
		NetworkingSceneChanger.Instance.ReloadLevel();
	}

	public void NextLevel()
	{
		Panel.gameObject.SetActive( false );
		NetworkingSceneChanger.Instance.LoadNextLevel();
	}

	public void ExitGame()
	{
		Networking.Disconnect();
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
	}
}