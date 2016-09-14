using UnityEngine;
using System.Collections;

/// <summary>
/// Just a DontDestroyOnLoad for now.
/// </summary>
public class SingletonBucketScript : MonoBehaviour
{
	private static SingletonBucketScript instance;

	void Awake ()
	{
		if ( instance != null )
			DestroyImmediate( gameObject );
		else
		{
			DontDestroyOnLoad( gameObject );
			instance = this;
		}
	}
}
