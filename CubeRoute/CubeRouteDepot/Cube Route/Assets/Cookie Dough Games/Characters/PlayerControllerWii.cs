using BeardedManStudios.Network;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VR;

[RequireComponent( typeof( Player ) )]
public class PlayerControllerWii : SimpleNetworkedMonoBehavior
{
	[Header( "Player Controller" )]
	public float BaseSpeed = 200;

	public float RotationSpeed = 7;
	public float speedH = 2.0f;
	public float speedV = 2.0f;

	// Wii controller info, if we've got one.
	public WiiControllerInfo wiiInfo;

	private Player player;
	private float JumpForce = 30;
	private Rigidbody rb;

	private Quaternion prevQuaternion;
	private Quaternion targetQuaternion;
	private bool rotating;
	private float time;
	private float CamSpeed = .2f;

	private Vector3 Gravity;

	public void ApplyMovementForce( float forward, float strafe )
	{
		float yVel = Vector3.Dot( rb.velocity, Gravity.normalized );
		rb.velocity = ( rb.transform.forward * forward / 20 ) + ( yVel * Gravity.normalized ) + rb.transform.right * strafe / 20;
	}

	public void ApplyRotation( float angle )
	{
		rb.transform.Rotate( new Vector3( 0, angle, 0 ) );
	}

	public void OnGravityChange( GravityEvent e )
	{
		// Whooooosh!
		AudioManager.Instance.PlaySound( AudioManager.SoundType.Gameplay_Gravity_0 );

		if ( IsClient && e.GetPlayer() == 2 )
		{
			SetGravity( e.GetGravityVec() );
		}
		// If server/offline
		else if ( e.GetPlayer() == 1 )
		{
			SetGravity( e.GetGravityVec() );
		}

		//		NetworkType nwp = GetComponent<Player>().NetworkType;
		//
		//		// Note: We cannot just use "IsSinglePlayerOrOwner" here, since the server may fire gravity events for both server + client
		//		if ( e.GetPlayer() == 1 && ( nwp == NetworkType.Server || nwp == NetworkType.Offline ) || e.GetPlayer() == 2 && nwp == NetworkType.Client )
		//		{
		//			SetGravity( e.GetGravityVec() );
		//		}
	}

	public void SetGravity( Vector3 NewGravity )
	{
		//Quaternion rot = Quaternion.FromToRotation( -rb.transform.up, NewGravity.normalized );
		//rb.transform.rotation = rot * rb.transform.rotation;
		//targetQuaternion = rot * rb.transform.rotation;
		//prevQuaternion = rb.transform.rotation;
		rotating = true;
		time = 0;

		Gravity = NewGravity;

		rb.freezeRotation = true;
	}

	public void Jump()
	{
		Vector3 down = transform.TransformDirection( Vector3.down );
		Vector3 offset = transform.up * .5f;
		// Is the raycast pointing down from the feet colliding with something?
		if ( Physics.Raycast( transform.position + offset, down, 1f ) )
		{
			rb.AddForce( -Gravity * JumpForce );
		}
	}

	public void OnEnable()
	{
		rb = GetComponent<Rigidbody>();
		EventManager.Grav += OnGravityChange;
	}

	public void OnDisable()
	{
		EventManager.Grav -= OnGravityChange;
	}

	protected void FixedUpdate()
	{
		if ( IsSinglePlayerOrOwner )
		{
			if ( rotating )
			{
				time += CamSpeed;
				Quaternion rot = Quaternion.FromToRotation( -rb.transform.up, Gravity.normalized );
				targetQuaternion = rot * rb.transform.rotation;
				prevQuaternion = rb.transform.rotation;
				if ( time >= 6 )
				{
					time = 0;
					rotating = false;
					rb.transform.rotation = targetQuaternion;
				}
				else {
					rb.transform.rotation = Quaternion.Slerp( prevQuaternion, targetQuaternion, CamSpeed );
				}
			}

			Vector3 u = rb.transform.up;
			Vector3 c = Camera.main.transform.forward;
			Vector3 target = c - ( u * ( Vector3.Dot( u, c ) ) / u.sqrMagnitude );
			Quaternion rotatef = Quaternion.FromToRotation( rb.transform.forward, target );
			Quaternion rotateb = Quaternion.FromToRotation( target, rb.transform.forward );
			rb.transform.rotation = rotatef * rb.transform.rotation;
			player.CameraPivot.transform.rotation = rotateb * Camera.main.transform.rotation;

			// Controller movement.
			float moveFwd = wiiInfo.GetWiimoteData().GetAxisVertical();
			float strafe = wiiInfo.GetWiimoteData().GetAxisHorizontal();

			float rotate = 0;
			if ( wiiInfo.GetWiimoteData().GetButton( WiimoteData.Buttons.Left ) )
			{
				rotate = -.5f;
			}
			else if ( wiiInfo.GetWiimoteData().GetButton( WiimoteData.Buttons.Right ) )
			{
				rotate = .5f;
			}

			if ( rotating )
			{
				moveFwd = 0;
				strafe = 0;
			}
			ApplyMovementForce( moveFwd * BaseSpeed, strafe * BaseSpeed );
			ApplyRotation( rotate * RotationSpeed );

			rb.AddForce( Gravity );
		}
	}

	private void Start()
	{
		player = GetComponent<Player>();

		Gravity = new Vector3( 0, -GravityScript.GravityConst, 0 );
		SetGravity( new Vector3( 0, -GravityScript.GravityConst, 0 ) );

		if ( IsSinglePlayerOrOwner )
		{
			Camera.main.transform.SetParent( player.CameraPivot, true );
			Camera.main.transform.ResetTransformation();
			Camera.main.transform.localPosition = new Vector3( 0, 1.75f, VRDevice.isPresent ? .45f : 0 );

			if ( VRDevice.isPresent )
			{
				Camera.main.fieldOfView = 100; // TODO Figure out ideal FOV for VR and non-VR
			}
		}
	}

	private void Update()
	{
		if ( IsSinglePlayerOrOwner && wiiInfo.GetWiimoteData().GetButtonDown( WiimoteData.Buttons.Home ) )
		{
			InGameMenu.Instance.PauseGame( player.transform.position + player.transform.forward, player.transform.rotation );
		}

		// Jump is in Update (rather than FixedUpdate) to make sure we always capture input
		if ( IsSinglePlayerOrOwner && wiiInfo.GetWiimoteData().GetButtonDown( WiimoteData.Buttons.A ) )
		{
			Jump();
		}

		Vector3 down = transform.TransformDirection( Vector3.down );
		Vector3 offset = transform.up * .5f;
		Debug.DrawRay( transform.position + offset, down * 1f, Color.red );
	}
}