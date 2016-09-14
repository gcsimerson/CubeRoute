using BeardedManStudios.Network;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

[RequireComponent( typeof( Rigidbody ) )]
public class GravityScript : SimpleNetworkedMonoBehavior
{
	public enum FlipDirection
	{
		Up,
		Down,
		Left,
		Right
	}

	[Header( "Gravity Script" )]
	/* Gravity scalar. */
	public static float GravityConst = 20;

	[FormerlySerializedAs( "Indicator" )]
	public GameObject Indicator;

	[Header( "Wii" )]
	public WiiControllerInfo wiiInfo;

	public float flipThreshold;

	private const float GravityCooldown = 1;

	/* Normal to the surface we're looking at. */
	private Vector3 m_ForwardNormal;
	private GravityState m_Gravity;
	private Rigidbody m_RigidBody;
	private float lastGravChangeTime = -GravityCooldown;

	// Used to restrict flipping when the game is paused.
	private bool canMove = true;

	private int ignorePlayerLayer = ~( 1 << LayerMask.NameToLayer( "Player" ) );

	public void SetCanMove( bool can )
	{
		canMove = can;
	}

	public void Reset()
	{
		m_Gravity = new GravityState();

		// TODO: This is fine if we always call reset on spawn only. Otherwise it needs to move.
		AudioManager.Instance.PlaySound( AudioManager.SoundType.Gameplay_Spawn_0 );
	}

	private bool CooldownReady()
	{
		return lastGravChangeTime.TimeElapsedSince() > GravityCooldown;
	}

	// Use this for initialization
	private void Start()
	{
		m_Gravity = new GravityState();
		m_RigidBody = GetComponent<Rigidbody>();
	}

	// TODO We need to prevent the player from rotating again until all block have finished moving.
	// Note: Right now, it is set to not rotate again until the player releases the key.

	private void Update()
	{
		if ( !IsSinglePlayerOrOwner ) return;

		// Get a normal to the surface we're looking at.
		RaycastHit hit;
		bool rayHitSomething = Physics.Raycast( Camera.main.transform.position, Camera.main.transform.forward, out hit, 500f, ignorePlayerLayer );
		bool rayHitInvalidAngle = false;
		if ( rayHitSomething )
		{
			m_ForwardNormal = hit.normal;

			float offset = -5;//( Mathf.Cos( Time.realtimeSinceStartup * 10 ) + 1 ) / 5;
							 // Position the indicator so its base touches the surface.

			Vector3 hit_point = hit.point;
			float hit_distance = hit.distance;

			Indicator.transform.position = hit_point + ( ( Indicator.transform.localScale.y + offset * Indicator.transform.localScale.y ) * hit.normal );

			// Align the indicator to the surface normal.
			Indicator.transform.rotation = Quaternion.FromToRotation( Indicator.transform.up, hit.normal ) * Indicator.transform.rotation;

			// Scale indicator based on how far away the surface is. TODO: Store/expose original indicator scale elsewhere.
			Vector3 indicatorSize = new Vector3( .05f, -.1f, .05f ) * hit_distance;
			//				print( indicatorSize );
			Indicator.transform.localScale = indicatorSize;

			float cosTheta = Vector3.Dot( m_Gravity.getCurrentVec(), m_ForwardNormal ) / ( m_Gravity.getCurrentVec().magnitude * m_ForwardNormal.magnitude );
			if ( cosTheta > .1 || cosTheta < -.1 )
			{
				rayHitInvalidAngle = true;
			}
		}

		if ( NetworkingSceneChanger.IsCompetitiveScene() )
		{
			Indicator.SetActive( CooldownReady() && rayHitSomething && !rayHitInvalidAngle && CompetitveManager.Instance.IsOurTurn() );
		}
		else {
			Indicator.SetActive( CooldownReady() && rayHitSomething && !rayHitInvalidAngle );
		}

		if ( canMove )
		{
			HandleFlips();
		}

		m_RigidBody.AddForce( m_Gravity.getCurrentVec() * GravityConst );
	}

	/// <summary>
	/// Interprets Wii gestures or simple buttons (keyboard) to control
	/// rotating gravity in the room. Wii controller supports bidirectional
	/// rotations.
	/// </summary>
	private void HandleFlips()
	{
		if ( wiiInfo != null && wiiInfo.GetWiimoteData() != null ) // Wii controls.
		{
			if ( wiiInfo.GetWiimoteData().GetButtonUp( WiimoteData.Buttons.B ) )
			{
				float pitch = wiiInfo.GetWiimoteData().GetPitch(); //
				float yaw = wiiInfo.GetWiimoteData().GetYaw(); // - right, + left

				float pitchMag = Mathf.Abs( pitch );
				float yawMag = Mathf.Abs( yaw );

				if ( pitchMag > flipThreshold || yawMag > flipThreshold )
				{
					FlipDirection gestureDir = FlipDirection.Up;

					if ( pitchMag >= yawMag )
					{
						gestureDir = pitch > 0 ? FlipDirection.Down : FlipDirection.Up;
					}
					else
					{
						gestureDir = yaw > 0 ? FlipDirection.Left : FlipDirection.Right;
					}

					DoFlip( gestureDir );
				}
			}
		}
		else // Keyboard controls.
		{
			// Pressing E shifts gravity (if it's off-cooldown).
			if ( Input.GetAxis( "Activate" ) != 0 && CooldownReady() )
			{
				DoFlip( FlipDirection.Up );
			}
		}
	}

	/// <summary>
	/// Sets new gravity vector and rotates the currect gravity and view.
	/// </summary>
	/// <param name="direction">The direction we want gravity to be after the flip.</param>
	private void DoFlip( FlipDirection direction )
	{
		// If this is competitive level, make sure it's our turn
		if ( !NetworkingSceneChanger.IsCompetitiveScene() || CompetitveManager.Instance.IsOurTurn() )
		{
			float cosTheta = Vector3.Dot( m_Gravity.getCurrentVec(), m_ForwardNormal ) / ( m_Gravity.getCurrentVec().magnitude * m_ForwardNormal.magnitude );
			if ( cosTheta < .9f && cosTheta > -.9f )
			{
				// Determine desired gravity after flip.
				Vector3 newVec = m_ForwardNormal * -1;
				switch ( direction )
				{
					case FlipDirection.Down:
						newVec *= -1;
						break;

					case FlipDirection.Left:
						newVec = Vector3.Cross( newVec, -transform.up ).normalized;
						break;

					case FlipDirection.Right:
						newVec = Vector3.Cross( newVec, transform.up ).normalized;
						break;

					default:
						break;
				}

				m_Gravity.setCurrentVec( newVec );
				lastGravChangeTime = Time.time;

				EventManager.RotateGravity( new GravityEvent( m_Gravity.getCurrentVec() * GravityConst, OurPlayerType() ) );

				if ( NetworkingSceneChanger.IsCompetitiveScene() )
				{
					CompetitveManager.Instance.UpdateRemotePlayersGravity( m_Gravity.getCurrentVec() * GravityConst, OurPlayerType() == 1 ? 2 : 1 );
				}
			}
		}
	}

	private int OurPlayerType()
	{
		return IsClient ? 2 : 1;
	}
}