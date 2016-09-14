using BeardedManStudios.Network;
using UnityEngine;

// ReSharper disable CompareOfFloatsByEqualityOperator

[RequireComponent( typeof( Animator ) )]
public class NetworkedAnimator : SimpleNetworkedMonoBehavior
{
	[Header( "Networked Animator" )]
	[Tooltip( "Animations will be synced over the network in this interval (in seconds)." )]
	public float UpdateFrequency = 0.1f;

	private readonly int jumpHash = Animator.StringToHash( "Jump" );
	private readonly int speedForwardHash = Animator.StringToHash( "SpeedForward" );
	private readonly int speedStrafeHash = Animator.StringToHash( "SpeedStrafe" );
	private readonly int fallingHash = Animator.StringToHash( "Falling" );

	private Animator anim;

	public void SendJumpTrigger()
	{
		RPC( "ReceiveJump", NetworkReceivers.Others );
	}

	private void Start()
	{
		anim = GetComponent<Animator>();

		if ( IsOnline && IsOwner )
		{
			InvokeRepeating( "SlowUpdate", 0, UpdateFrequency );
		}
	}

	private void SlowUpdate()
	{
		// IsOnline && IsOwner already assumed true

		RPC( "SyncParams", NetworkReceivers.Others, anim.GetFloat( speedForwardHash ), anim.GetFloat( speedStrafeHash ), anim.GetBool( fallingHash ) );
	}

	[BRPC]
	private void SyncParams( float speedForward, float speedStrafe, bool falling )
	{
		anim.SetFloat( speedForwardHash, speedForward );
		anim.SetFloat( speedStrafeHash, speedStrafe );
		anim.SetBool( fallingHash, falling );
	}

	[BRPC]
	private void ReceiveJump()
	{
		//		anim.ResetTrigger( jumpHash );
		anim.SetTrigger( jumpHash );
	}
}