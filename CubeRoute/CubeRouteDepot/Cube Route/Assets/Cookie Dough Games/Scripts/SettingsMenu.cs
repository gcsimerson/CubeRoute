using System.Collections;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
	public static SettingsMenu instance = null;

	[Header( "Settings Menu" )]
	public RectTransform Panel;

	public void ToggleDisplay()
	{
		Panel.gameObject.SetActive( !Panel.gameObject.activeSelf );
	}

	private void Awake()
	{
		if ( instance != null )
			DestroyImmediate( gameObject );
		else
		{
			instance = this;
		}
	}
}