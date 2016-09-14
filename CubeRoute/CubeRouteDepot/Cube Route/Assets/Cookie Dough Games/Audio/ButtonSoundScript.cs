using UnityEngine;
using System.Collections;

public class ButtonSoundScript : MonoBehaviour
{
	public void PlayHover()
	{
		AudioManager.Instance.PlaySound(AudioManager.SoundType.Menu_Hover_0);
	}

	public void PlayClick()
	{
		AudioManager.Instance.PlaySound(AudioManager.SoundType.Menu_Select_0);
	}

	public void PlayStart()
	{
		AudioManager.Instance.PlaySound(AudioManager.SoundType.Menu_Select_1);
	}
}
