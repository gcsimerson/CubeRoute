using System.Collections;
using UnityEngine;

public class CompetitiveExplosion : MonoBehaviour
{
	private void Start()
	{
		AudioManager.Instance.PlaySound( AudioManager.SoundType.Gameplay_Death_0 );
	}
}