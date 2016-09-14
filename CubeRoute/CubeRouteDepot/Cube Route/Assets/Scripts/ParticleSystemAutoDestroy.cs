using System.Collections;
using UnityEngine;

[RequireComponent( typeof( ParticleSystem ) )]
public class ParticleSystemAutoDestroy : MonoBehaviour
{
	private ParticleSystem ps;

	private void Start()
	{
		ps = GetComponent<ParticleSystem>();
	}

	private void FixedUpdate()
	{
		if ( !ps.IsAlive() )
		{
			Destroy( gameObject );
		}
	}
}