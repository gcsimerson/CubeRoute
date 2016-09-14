using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : Singleton<AudioManager>
{
	// Used to make music clips accessible to outside functions.
	public enum MusicType
	{
		Music_Menu_0,
		Music_Puzzle_0,
		Music_Puzzle_1,
		Music_Battle_0
	}

	// Used to make audio clips accessible to outside functions.
	public enum AmbientType
	{
		Ambient_Hum_0,
		Ambient_Lava_0
	}

	// Used to make audio clips accessible to outside functions.
	public enum SoundType
	{
		Gameplay_ElevatorMove_0,
		Gameplay_ElevatorOpen_0,
		Gameplay_ElevatorStart_0,
		Gameplay_ElevatorStop_0,
		Menu_Fail_0,
		Menu_Hover_0,
		Menu_ModalDialog_0,
		Menu_Select_0,
		Menu_Select_1,
		Gameplay_Gravity_0,
		Gameplay_Spawn_0,
		Gameplay_Death_0,
		Gameplay_LevelComplete_0
	}

	public AudioClip[] MusicClips;
	public AudioClip[] AmbientClips;
	public AudioClip[] SoundClips;
	private AudioSource MusicSource;
	private Dictionary<MusicType, AudioClip> Songs = new Dictionary<MusicType, AudioClip>();

	private AudioSource AmbientSource;
	private Dictionary<AmbientType, AudioClip> Ambience = new Dictionary<AmbientType, AudioClip>();

	private AudioSource SFXSource;
	private Dictionary<SoundType, AudioClip> Sounds = new Dictionary<SoundType, AudioClip>();

	public void ApplyVolumeSettings()
	{
		MusicSource.volume = PlayerPrefs.GetFloat( "Master Volume" ) * PlayerPrefs.GetFloat( "Music Volume" );
		AmbientSource.volume = PlayerPrefs.GetFloat( "Master Volume" ) * PlayerPrefs.GetFloat( "Sound Volume" );
		SFXSource.volume = PlayerPrefs.GetFloat( "Master Volume" ) * PlayerPrefs.GetFloat( "Sound Volume" );
	}

	public void PlaySound( SoundType type )
	{
		if ( Sounds.ContainsKey( type ) && Sounds[type] != null )
		{
			SFXSource.PlayOneShot( Sounds[type] );
		}
	}

	public void PlayMusic( MusicType type )
	{
		if ( MusicSource.isPlaying )
		{
			MusicSource.Stop();
		}
		MusicSource.clip = Songs[type];
		MusicSource.Play();
	}

	public void StopMusic()
	{
		if ( MusicSource.isPlaying )
		{
			MusicSource.Stop();
		}
	}

	public void PlayAmbient( AmbientType type )
	{
		if ( AmbientSource.isPlaying )
		{
			AmbientSource.Stop();
		}
		AmbientSource.clip = Ambience[type];
		AmbientSource.Play();
	}

	public void StopAmbient()
	{
		if ( AmbientSource.isPlaying )
		{
			AmbientSource.Stop();
		}
	}

	private void Awake()
	{
		AudioSource[] sources = GetComponents<AudioSource>();
		MusicSource = sources[0];
		AmbientSource = sources[1];
		SFXSource = sources[2];

		ApplyVolumeSettings();

		// Music.
		Songs.Add( MusicType.Music_Menu_0, MusicClips[0] );
		Songs.Add( MusicType.Music_Puzzle_0, MusicClips[1] );
		Songs.Add( MusicType.Music_Puzzle_1, MusicClips[2] );
		Songs.Add( MusicType.Music_Battle_0, MusicClips[3] );

		// Ambience.
		Ambience.Add( AmbientType.Ambient_Hum_0, AmbientClips[0] );
		Ambience.Add( AmbientType.Ambient_Lava_0, AmbientClips[1] );

		// SFX.
		Sounds.Add( SoundType.Menu_Fail_0, SoundClips[4] );
		Sounds.Add( SoundType.Menu_Hover_0, SoundClips[5] );
		Sounds.Add( SoundType.Menu_ModalDialog_0, SoundClips[6] );
		Sounds.Add( SoundType.Menu_Select_0, SoundClips[7] );
		Sounds.Add( SoundType.Menu_Select_1, SoundClips[8] );
		Sounds.Add( SoundType.Gameplay_Gravity_0, SoundClips[9] );
		Sounds.Add( SoundType.Gameplay_Spawn_0, SoundClips[10] );
		Sounds.Add( SoundType.Gameplay_Death_0, SoundClips[11] );
	}

	private void OnLevelWasLoaded()
	{
		// Music.
		if ( NetworkingSceneChanger.IsMainMenu() )
		{
			PlayMusic( MusicType.Music_Menu_0 );
			StopAmbient();
		}
		else if ( NetworkingSceneChanger.IsCompetitiveScene() )
		{
			PlayMusic( MusicType.Music_Battle_0 );
			PlayAmbient( AmbientType.Ambient_Lava_0 );
		}
		else if ( NetworkingSceneChanger.IsCoopScene() )
		{
			PlayMusic( MusicType.Music_Puzzle_1 );
			StopAmbient();
		}
		else if ( NetworkingSceneChanger.IsSoloScene() )
		{
			PlayMusic( MusicType.Music_Puzzle_0 );
			StopAmbient();
		}
	}
}