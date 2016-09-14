using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class ProfileHandler : Singleton<ProfileHandler>
{
	// Profile Information
	private string currentLanguage;
	private int currentLangInt;
	private float currentMusicVolume;
	private float currentSoundVolume;
	private float currentMasterVolume;

	// Pointers to the sliders
	public Slider MasterVolumeSlider;
	public Slider MusicVolumeSlider;
	public Slider SoundEffectVolumeSlider;
	public Dropdown LanguageDropdown;

	// Reference to the localization script.
	private MainMenuLocScript locScript;

	void Start()
	{
		locScript = GetComponent<MainMenuLocScript>();

		int hasProfile = PlayerPrefs.GetInt ("HasProfile");
		if (hasProfile != 1)
		{
			PlayerPrefs.SetInt ("HasProfile", 1);
			SetToDefaults ();
		} 

		MasterVolumeSlider.value = PlayerPrefs.GetFloat ("Master Volume");
		currentMasterVolume = MasterVolumeSlider.value;
		MusicVolumeSlider.value = PlayerPrefs.GetFloat ("Music Volume");
		currentMasterVolume = MusicVolumeSlider.value;
		SoundEffectVolumeSlider.value = PlayerPrefs.GetFloat ("Sound Volume");
		currentMasterVolume = SoundEffectVolumeSlider.value;
		LanguageDropdown.value = PlayerPrefs.GetInt ("Language Int");
	}

	public void SetToDefaults()
	{
		PlayerPrefs.SetString ("Language", "English");
		PlayerPrefs.SetFloat ("Music Volume", 0.5f);
		PlayerPrefs.SetFloat ("Sound Volume", 0.5f);
		PlayerPrefs.SetFloat("Master Volume", 1.0f);
		PlayerPrefs.SetInt ("Language Int", 0);
		PlayerPrefs.Save ();
	}

	public void SetCurrentLanguage(int lang)
	{
		currentLangInt = lang;

		if (lang == 0)
		{
			currentLanguage = "English";
		}
		else if (lang == 1)
		{
			currentLanguage = "French";
		}
		else if (lang == 2)
		{
			currentLanguage = "Italian";
		}
		else if (lang == 3)
		{
			currentLanguage = "German";
		}
		else if (lang == 4)
		{
			currentLanguage = "Spanish";
		}
		else if (lang == 5)
		{
			currentLanguage = "Japanese";
		}
		else if (lang == 6)
		{
			currentLanguage = "Korean";
		}
		else if (lang == 7)
		{
			currentLanguage = "Chinese";
		}
	}

	public void SetCurrentMusicVolume(float percent)
	{
		currentMusicVolume = percent;
	}

	public void SetCurrentSoundVolume(float percent)
	{
		currentSoundVolume = percent;
	}

	public void SetCurrentMasterVolume(float percent)
	{
		currentMasterVolume = percent;
	}

	public void SaveAllChanges()
	{
		PlayerPrefs.SetString ("Language", currentLanguage);
		PlayerPrefs.SetFloat ("Music Volume", currentMusicVolume);
		PlayerPrefs.SetFloat ("Sound Volume", currentSoundVolume);
		PlayerPrefs.SetFloat("Master Volume", currentMasterVolume);
		PlayerPrefs.SetInt ("Language Int", currentLangInt);
		PlayerPrefs.Save ();

		AudioManager.Instance.ApplyVolumeSettings();

		if (locScript != null)
		{
			locScript.UpdateUIStringsForLanguage(currentLanguage);
		}
	}
}
