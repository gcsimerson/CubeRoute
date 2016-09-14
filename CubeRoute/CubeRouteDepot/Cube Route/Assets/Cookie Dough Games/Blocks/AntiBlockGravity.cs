using System.Collections;
using UnityEngine;

public class AntiBlockGravity : BlockGravity
{
	protected override void FixedUpdate()
	{
		// The server is responsible for moving both the server and client cubes
		if ( IsSinglePlayerOrOwner )
		{
			rb.AddForce( -blockGravity ); // TODO: removed "* rb.mass" for quicker calculations
		}
	}
}