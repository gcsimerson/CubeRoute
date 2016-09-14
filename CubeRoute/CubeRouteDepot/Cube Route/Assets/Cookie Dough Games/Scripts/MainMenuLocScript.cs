using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuLocScript : MonoBehaviour
{
	// .txt files for each item's string.
	public TextAsset[] MainMenuTexts;
	public TextAsset[] MultiplayerMenuTexts;
	public TextAsset[] SettingsMenuTexts;
	public TextAsset[] PauseMenuTexts;
	public TextAsset[] CompetitiveTexts;
	public TextAsset[] OtherTexts;

	// UI elements with strings in them.
	public Text[] MainMenuTextElements;
	public Text[] MultiplayerMenuTextElements;
	public Text[] SettingsMenuTextElements;
	public Text[] PauseMenuTextElements;
	public Text[] CompetitiveTextElements;
	public Text[] OtherTextElements;

	void Start ()
	{
		// Apply default strings.
		string lang = PlayerPrefs.GetString("Language");
		UpdateUIStringsForLanguage(lang);
	}

	/// <summary>
	/// Updates UI strings based on selected language.
	/// </summary>
	/// <param name="language">The chosen language, which should
	/// match the directory name under /Resources/Text/</param>
	public void UpdateUIStringsForLanguage(string language)
	{
		// Load Main Menu strings.
		MainMenuTexts[0] = Resources.Load("Text/" + language + "/Main Menu/single_player") as TextAsset;
		MainMenuTexts[1] = Resources.Load("Text/" + language + "/Main Menu/multiplayer") as TextAsset;
		MainMenuTexts[2] = Resources.Load("Text/" + language + "/Main Menu/settings") as TextAsset;
		MainMenuTexts[3] = Resources.Load("Text/" + language + "/Main Menu/exit") as TextAsset;

		// Load Multiplayer strings.
		MultiplayerMenuTexts[0] = Resources.Load("Text/" + language + "/Multiplayer Menu/host_a_new_game") as TextAsset;
		MultiplayerMenuTexts[1] = Resources.Load("Text/" + language + "/Multiplayer Menu/cooperative") as TextAsset;
		MultiplayerMenuTexts[2] = Resources.Load("Text/" + language + "/Multiplayer Menu/competitive") as TextAsset;
		MultiplayerMenuTexts[3] = Resources.Load("Text/" + language + "/Multiplayer Menu/join_by_ip_address") as TextAsset;
		MultiplayerMenuTexts[4] = Resources.Load("Text/" + language + "/Multiplayer Menu/connect") as TextAsset;
		MultiplayerMenuTexts[5] = Resources.Load("Text/" + language + "/Multiplayer Menu/search_on_local_area_network") as TextAsset;
		MultiplayerMenuTexts[6] = Resources.Load("Text/" + language + "/Multiplayer Menu/history") as TextAsset;

		// Load Settings strings.
		SettingsMenuTexts[0] = Resources.Load("Text/" + language + "/Settings Menu/master_volume") as TextAsset;
		SettingsMenuTexts[1] = Resources.Load("Text/" + language + "/Settings Menu/music_volume") as TextAsset;
		SettingsMenuTexts[2] = Resources.Load("Text/" + language + "/Settings Menu/sound_effects_volume") as TextAsset;
		SettingsMenuTexts[3] = Resources.Load("Text/" + language + "/Settings Menu/language") as TextAsset;
		SettingsMenuTexts[4] = Resources.Load("Text/" + language + "/Settings Menu/save") as TextAsset;

		// Load Pause strings.
		PauseMenuTexts[0] = Resources.Load("Text/" + language + "/Pause Menu/resume") as TextAsset;
		PauseMenuTexts[1] = Resources.Load("Text/" + language + "/Pause Menu/reset_level") as TextAsset;
		PauseMenuTexts[2] = Resources.Load("Text/" + language + "/Pause Menu/next_level") as TextAsset;
		PauseMenuTexts[3] = Resources.Load("Text/" + language + "/Pause Menu/settings") as TextAsset;
		PauseMenuTexts[4] = Resources.Load("Text/" + language + "/Pause Menu/exit_to_main_menu") as TextAsset;
		PauseMenuTexts[5] = Resources.Load("Text/" + language + "/Pause Menu/exit_game") as TextAsset;

		// Load Competitive strings.
		CompetitiveTexts[0] = Resources.Load("Text/" + language + "/Competitive/blue_score") as TextAsset;
		CompetitiveTexts[1] = Resources.Load("Text/" + language + "/Competitive/yellow_score") as TextAsset;
		CompetitiveTexts[2] = Resources.Load("Text/" + language + "/Competitive/blue_turn") as TextAsset;
		CompetitiveTexts[3] = Resources.Load("Text/" + language + "/Competitive/yellow_turn") as TextAsset;
		CompetitiveTexts[4] = Resources.Load("Text/" + language + "/Competitive/blue_wins") as TextAsset;
		CompetitiveTexts[5] = Resources.Load("Text/" + language + "/Competitive/yellow_wins") as TextAsset;

		// Load remaining strings.
		OtherTexts[0] = Resources.Load("Text/" + language + "/loading") as TextAsset;
		OtherTexts[1] = Resources.Load("Text/" + language + "/waiting_for_second_player_to_connect") as TextAsset;

		// Apply new strings to each panel.
		if (MainMenuTextElements != null && MainMenuTexts != null && MainMenuTextElements.Length == MainMenuTexts.Length)
		{
			for (int i = 0; i < MainMenuTexts.Length; i++)
			{
				if (MainMenuTextElements[i] != null)
				{
					MainMenuTextElements[i].text = MainMenuTexts[i].text;
				}
			}
		}
		if (MultiplayerMenuTextElements != null && MultiplayerMenuTexts != null && MultiplayerMenuTextElements.Length == MultiplayerMenuTexts.Length)
		{
			for (int i = 0; i < MultiplayerMenuTexts.Length; i++)
			{
				if (MultiplayerMenuTextElements[i] != null)
				{
					MultiplayerMenuTextElements[i].text = MultiplayerMenuTexts[i].text;
				}
			}
		}
		if (SettingsMenuTextElements != null && SettingsMenuTexts != null && SettingsMenuTextElements.Length == SettingsMenuTexts.Length)
		{
			for (int i = 0; i < SettingsMenuTexts.Length; i++)
			{
				if (SettingsMenuTextElements[i] != null)
				{
					SettingsMenuTextElements[i].text = SettingsMenuTexts[i].text;
				}
			}
		}
		if (PauseMenuTextElements != null && PauseMenuTexts != null && PauseMenuTextElements.Length == PauseMenuTexts.Length)
		{
			for (int i = 0; i < PauseMenuTexts.Length; i++)
			{
				if (PauseMenuTextElements[i] != null)
				{
					PauseMenuTextElements[i].text = PauseMenuTexts[i].text;
				}
			}
		}
		if (CompetitiveTextElements != null && CompetitiveTexts != null && CompetitiveTextElements.Length == CompetitiveTexts.Length)
		{
			for (int i = 0; i < CompetitiveTexts.Length; i++)
			{
				if (CompetitiveTextElements[i] != null)
				{
					CompetitiveTextElements[i].text = CompetitiveTexts[i].text;
				}
			}
		}
		if (OtherTextElements != null && OtherTexts != null && OtherTextElements.Length == OtherTexts.Length)
		{
			for (int i = 0; i < OtherTexts.Length; i++)
			{
				if (OtherTextElements[i] != null)
				{
					OtherTextElements[i].text = OtherTexts[i].text;
				}
			}
		}
	}
}
