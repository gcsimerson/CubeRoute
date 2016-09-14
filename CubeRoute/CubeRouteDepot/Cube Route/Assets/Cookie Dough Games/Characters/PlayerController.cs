using BeardedManStudios.Network;
using BeardedManStudios.Network.Unity;
using UnityEngine;
using UnityEngine.VR;

[RequireComponent( typeof( Player ) )]
public class PlayerController : SimpleNetworkedMonoBehavior
{
	[Header( "Player Controller" )]

	// The script that handles gravity controls and updates.
	public GravityScript gravityScript;

	// Wii controller info, if we've got one.
	public WiiControllerInfo wiiInfo;

	private const float BaseSpeed = 100;
	private const float Sensitivity = 10;
	private const float RotationSpeed = 7;
	private const float SpeedH = 2.0f;
	private const float SpeedV = 2.0f;
	private const float VRCamFOV = 120f;
	private const float RotateDuration = 0.5f;
	private const float MinJumpInterval = 0.5f;
	private const float MinRotateAngle = 30;
	private const float MaxRotateAngle = 60;
	private const float ViewRotationSpeed = 2;
	private const float JumpForce = 30;
	private float rotateX = 0;
	private float rotateY = 0;
	private Player player;
	private Rigidbody rb;
	private Animator anim;
	private float startRotateTime = -RotateDuration;
	private float lastJumpTime = -MinJumpInterval;
	private Vector3 gravity;
	private Camera secondaryCamera;
	private Quaternion startPlayerRot, startCamRot, destPlayerRot, destCamRot;
	private bool canMove;

	public void OnEnable()
	{
		rb = GetComponent<Rigidbody>();
		anim = GetComponent<Animator>();
		EventManager.Grav += OnGravityChange;
	}

	public void OnDisable()
	{
		EventManager.Grav -= OnGravityChange;
	}

	public void OnGravityChange( GravityEvent e )
	{
		// Whooooosh!
		AudioManager.Instance.PlaySound( AudioManager.SoundType.Gameplay_Gravity_0 );

		if ( ( IsClient && e.GetPlayer() == 2 ) || e.GetPlayer() == 1 )
		{
			SetGravity( e.GetGravityVec() );
		}
	}

	public void SetGravity( Vector3 newGravity )
	{
		gravity = newGravity;
		startRotateTime = Time.time;

		startPlayerRot = transform.rotation;
		startCamRot = player.CameraPivot.transform.rotation;

		Quaternion playerRotationAmount = Quaternion.FromToRotation( -transform.up, gravity.normalized );
		Quaternion cameraRotationAmount = Quaternion.FromToRotation( -player.CameraPivot.transform.up, gravity.normalized );

		destPlayerRot = playerRotationAmount * startPlayerRot;
		destCamRot = cameraRotationAmount * startCamRot;
	}

	public void ApplyMovementForce( float forward, float strafe )
	{
		anim.SetFloat( "SpeedForward", forward );
		anim.SetFloat( "SpeedStrafe", strafe );
		float yVel = Vector3.Dot( rb.velocity, gravity.normalized );
		rb.velocity = ( rb.transform.forward * forward / 20 ) + ( yVel * gravity.normalized ) + rb.transform.right * strafe / 20;
	}

	public void Jump()
	{
		Vector3 down = transform.TransformDirection( Vector3.down );
		Vector3 offset = transform.up * .5f;

		// Is the raycast pointing down from the feet colliding with something?
		if ( Physics.Raycast( transform.position + offset, down, 1f ) )
		{
			anim.SetTrigger( "Jump" );
			player.GetComponent<NetworkedAnimator>().SendJumpTrigger();

			rb.AddForce( -gravity * JumpForce );
		}
	}

	protected void LateUpdate()
	{
		if ( !IsSinglePlayerOrOwner ) return;

		if ( RecentlyRotated( 1.5f ) ) // transform.rotation != destPlayerRot
		{
			// print ("transform.rotation=["+transform.rotation+"] destPlayerRot=["+destPlayerRot+"] angle=["+Quaternion.Angle(transform.rotation, destPlayerRot)+"]");
			transform.rotation = Quaternion.Slerp( startPlayerRot, destPlayerRot, startRotateTime.TimeElapsedSince() / RotateDuration );
			player.CameraPivot.transform.rotation = Quaternion.Slerp( startCamRot, destCamRot, startRotateTime.TimeElapsedSince() / RotateDuration );
		}

		Vector3 playerUp = transform.up;
		Vector3 camForward = Camera.main.transform.forward;
		Vector3 target = camForward - ( playerUp * ( Vector3.Dot( playerUp, camForward ) ) / playerUp.sqrMagnitude );
		Quaternion rotateAmount = Quaternion.FromToRotation( transform.forward, target );

		float localAngle = Camera.main.transform.localEulerAngles.y;
		transform.rotation = rotateAmount * transform.rotation;
		player.CameraPivot.transform.position = player.HeadPivot.position + player.HeadPivot.rotation * new Vector3( 0, 0.05f, 0.14f );

		if ( VRDevice.isPresent )
		{
			if ( localAngle > MinRotateAngle && localAngle < 180 )
			{
				float diff = localAngle - MinRotateAngle;
				float val = Mathf.Min( diff / ( MaxRotateAngle - MinRotateAngle ), 1 );
				transform.Rotate( new Vector3( 0, val * ViewRotationSpeed, 0 ) );
				player.CameraPivot.transform.Rotate( new Vector3( 0, val * ViewRotationSpeed, 0 ) );
			}
			else if ( localAngle < 360 - MinRotateAngle && localAngle > 180 )
			{
				float diff = 360 - MinRotateAngle - localAngle;
				float val = Mathf.Min( diff / ( MaxRotateAngle - MinRotateAngle ), 1 );
				transform.Rotate( new Vector3( 0, -val * ViewRotationSpeed, 0 ) );
				player.CameraPivot.transform.Rotate( new Vector3( 0, -val * ViewRotationSpeed, 0 ) );
			}
		}
		else // !VRDevice.isPresent
		{
			if ( canMove )
			{
				rotateX = Mathf.Max( Mathf.Min( rotateX - Input.GetAxis( "Mouse Y" ) * Sensitivity, 80 ), -80 );
				rotateY += Input.GetAxis( "Mouse X" ) * Sensitivity;
				Camera.main.transform.localEulerAngles = new Vector3( rotateX, rotateY, 0 );
				secondaryCamera.transform.localEulerAngles = new Vector3( rotateX, rotateY, 0 );
			}
		}

		// Offset the camera slightly to be at the player's nose
		//player.CameraPivot.transform.position += player.HeadPivot.rotation * new Vector3( 0, 0.05f, 0.14f );

		if ( Input.GetKeyDown( KeyCode.Space ) && lastJumpTime.TimeElapsedSince() > MinJumpInterval && canMove )
		{
			lastJumpTime = Time.time;
			Jump();
		}
		if ( Input.GetKeyDown( KeyCode.L ) )
		{
			NetworkingSceneChanger.Instance.ReloadLevel();
		}
		if ( Input.GetKeyDown( KeyCode.M ) )
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			NetworkingSceneChanger.ExitToMainMenu();
		}
		if ( Input.GetKeyDown( KeyCode.N ) )
		{
			NetworkingSceneChanger.Instance.LoadNextLevel();
		}
	}

	private bool RecentlyRotated( float rotateDurationMult = 1 )
	{
		return startRotateTime.TimeElapsedSince() <= RotateDuration * rotateDurationMult;
	}

	private void FixedUpdate()
	{
		if ( !IsSinglePlayerOrOwner ) return;

		float rotate = Input.GetAxis( "Horizontal2" );

		float moveFwd = 0;
		float strafe = 0;
		if ( !RecentlyRotated() && canMove )
		{
			moveFwd = Input.GetAxis( "Vertical" );
			strafe = Input.GetAxis( "Horizontal" );
		}

		// Apply movements.
		ApplyMovementForce( moveFwd * BaseSpeed, strafe * BaseSpeed );
		transform.Rotate( new Vector3( 0, rotate * RotationSpeed, 0 ) );

		// Apply gravity.
		rb.AddForce( gravity );
	}

	private void Start()
	{
		player = GetComponent<Player>();
		rb.freezeRotation = true;
		gravity = new Vector3( 0, -GravityScript.GravityConst, 0 );

		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;

		canMove = true;

		if ( !IsSinglePlayerOrOwner ) return;

		secondaryCamera = GameObject.FindGameObjectWithTag( "SecondaryCamera" ).GetComponent<Camera>();

		if ( VRDevice.isPresent )
		{
			Camera.main.fieldOfView = VRCamFOV;
			secondaryCamera.fieldOfView = VRCamFOV;
		}
	}

	private void Update()
	{
		if ( !IsSinglePlayerOrOwner ) return;

		Camera.main.transform.localPosition = Vector3.zero;
		secondaryCamera.transform.localPosition = Vector3.zero;

		Vector3 down = transform.TransformDirection( Vector3.down );
		Vector3 offset = transform.up * .5f;
		//		Debug.DrawRay( transform.position + offset, down * 1f, Color.red );

		bool pause = Input.GetKeyDown( KeyCode.Escape );

		if ( pause )
		{
			canMove = !canMove;
		}

		anim.SetBool( "Falling", !Physics.Raycast( transform.position + offset, down, 1f ) );
	}
}