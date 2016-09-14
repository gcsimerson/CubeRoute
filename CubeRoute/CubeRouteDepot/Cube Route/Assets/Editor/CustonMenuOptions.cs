using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class CustomMenuOptions : EditorWindow
{
	[MenuItem( "Edit/Delete PlayerPrefs" )]
	public static void DeletePlayerPrefs()
	{
		PlayerPrefs.DeleteAll();
	}

	[MenuItem( "Build/Play Main Menu" )]
	public static void PlayInEditor()
	{
		// If the editor is already running, stop it and do nothing else
		if ( EditorApplication.isPlaying )
		{
			EditorApplication.isPlaying = false;
			return;
		}

		// Save the current scene and load the server-specific launcher scene
		EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
		EditorSceneManager.OpenScene( "Assets/Cookie Dough Games/Scenes/MainMenu.unity" );
		EditorApplication.isPlaying = true;
	}
}