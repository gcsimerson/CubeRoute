using BeardedManStudios.Network;
using UnityEngine;

[RequireComponent( typeof( Rigidbody ) )]
public class BlockGravity : NetworkedMonoBehavior
{
	[Header( "Block Gravity" )]
	public NetworkType NetworkType;

	[Header( "Audio" )]
	// Play thud sounds when landing.
	public AudioClip m_audioClipThud;

	public AudioSource m_audioSource;
	protected Rigidbody rb;
	protected Vector3 blockGravity;
	private Vector3 InitialPosition;

	protected virtual void FixedUpdate()
	{
		// The server is responsible for moving both the server and client cubes
		if ( IsSinglePlayerOrOwner )
		{
			rb.AddForce( blockGravity ); // TODO: removed "* rb.mass" for quicker calculations
		}
	}

	private void OnEnable()
	{
		blockGravity = new Vector3( 0, -GravityScript.GravityConst, 0 );
		EventManager.Grav += OnGravityChange;
	}

	private void OnDisable()
	{
		EventManager.Grav -= OnGravityChange;
	}

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		InitialPosition = rb.position; //TODO OnEnable is called before Start, so rb is not set yet
	}

	private void OnCollisionEnter( Collision collision )
	{
		if ( collision != null && !collision.gameObject.tag.Equals( "Block" ) && !collision.gameObject.tag.Equals( "Player" ) )
		{
			//m_audioSource.PlayOneShot( m_audioClipThud );
		}
	}

	private void OnGravityChange( GravityEvent e )
	{
		if ( e.GetPlayer() == 1 && ( NetworkType == NetworkType.Server || NetworkType == NetworkType.Offline ) || e.GetPlayer() == 2 && NetworkType == NetworkType.Client )
		{
			blockGravity = e.GetGravityVec();
		}

		InitialPosition = rb.position;
	}

	private void LateUpdate()
	{
		if ( IsSinglePlayerOrOwner )
		{
			Vector3 Grav = blockGravity;
			if ( Grav.magnitude < .01 || rb == null )
			{
				return;
			}
			//			//print (Grav);
			//			//print (InitialPosition);
			//			//print (Vector3.Dot (rb.position, InitialPosition));
			//			//print (Vector3.Dot((rb.position - InitialPosition), Grav) / Grav.sqrMagnitude);
			//			//print (Grav * (Vector3.Dot ((rb.position - InitialPosition), Grav) / Grav.sqrMagnitude));
			//			//print (InitialPosition + Grav * (Vector3.Dot((rb.position - InitialPosition), Grav) / Grav.sqrMagnitude));
			rb.velocity = Grav.normalized * Vector3.Dot(rb.velocity, Grav.normalized);
			rb.MovePosition( InitialPosition + Grav * ( Vector3.Dot( ( rb.position - InitialPosition ), Grav ) / Grav.sqrMagnitude ) );
		}
	}
}