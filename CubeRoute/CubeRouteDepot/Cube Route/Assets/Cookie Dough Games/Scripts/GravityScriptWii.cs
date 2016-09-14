using BeardedManStudios.Network;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent( typeof( Rigidbody ) )]
public class GravityScriptWii : SimpleNetworkedMonoBehavior
{
	/* Gravity scalar. */
	public static float GravityConst = 20;

	[Header( "Gravity Script" )]
	public float m_GravityCooldown = 3;

	public GameObject m_Indicator;
	private float m_timeSinceGravityShift = 9999;
	/* Normal to the surface we're looking at. */
	private Vector3 m_ForwardNormal;
	private GravityState m_Gravity;
	private Rigidbody m_RigidBody;

	// Wii controller info, if we've got one.
	public WiiControllerInfo wiiInfo;

	public void Reset()
	{
		m_Gravity = new GravityState();

		// TODO: This is fine if we always call reset on spawn only. Otherwise it needs to move.
		AudioManager.Instance.PlaySound( AudioManager.SoundType.Gameplay_Spawn_0 );
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

		// TODO: Cache camera when it is attached to the player (boxes don't have cameras).
		Camera cam = GetComponentInChildren<Camera>();
		if ( cam != null )
		{
			// Get a normal to the surface we're looking at.
			RaycastHit hit;
			bool rayHitSomething = Physics.Raycast( cam.transform.position, cam.transform.forward, out hit, 500f, LayerMask.NameToLayer( "Player" ) );
			if ( rayHitSomething )
			{
				m_ForwardNormal = hit.normal;

				// Position the indicator so its base touches the surface.
				m_Indicator.transform.position = hit.point + ( m_Indicator.transform.localScale.y * hit.normal );

				// Alight the indicator to the surface normal.
				m_Indicator.transform.rotation = Quaternion.FromToRotation( m_Indicator.transform.up, hit.normal ) * m_Indicator.transform.rotation;

				// Scale indicator based on how far away the surface is. TODO: Store/expose original indicator scale elsewhere.
				Vector3 indicatorSize = new Vector3( .05f, .2f, .05f ) * hit.distance;
				m_Indicator.transform.localScale = indicatorSize;
			}
			m_Indicator.SetActive( rayHitSomething );

			// Pressing B shifts gravity (if it's off-cooldown).
			bool wiiShift = wiiInfo != null && wiiInfo.GetWiimoteData().GetButtonDown(WiimoteData.Buttons.B);
			if ( wiiShift && m_timeSinceGravityShift >= m_GravityCooldown )
			{
				// TODO HACK: Clean this up
				// If this is competitive level, make sure it's our turn
				if ( !SceneManager.GetActiveScene().name.Equals( "Spires" ) || CompetitveManager.Instance.IsOurTurn() )
				{
					m_Gravity.setCurrentVec( m_ForwardNormal * -1 );
					m_timeSinceGravityShift = 0;

					EventManager.RotateGravity( new GravityEvent( m_Gravity.getCurrentVec() * GravityConst, OurPlayerType() ) );

					// TODO HACK: Clean this up -- Move inside OnEvent in CompetitveManager?
					if ( SceneManager.GetActiveScene().name.Equals( "Spires" ) )
					{
						CompetitveManager.Instance.UpdateRemotePlayersGravity( m_Gravity.getCurrentVec() * GravityConst, OurPlayerType() == 1 ? 2 : 1 );
					}
				}
			}
			else
			{
				m_timeSinceGravityShift += Time.deltaTime;
			}
		}

		m_RigidBody.AddForce( m_Gravity.getCurrentVec() * GravityConst );
	}

	private int OurPlayerType()
	{
		return IsClient ? 2 : 1;
	}
}