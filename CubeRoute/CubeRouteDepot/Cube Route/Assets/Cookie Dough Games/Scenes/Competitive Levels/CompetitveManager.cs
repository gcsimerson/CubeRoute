using BeardedManStudios.Network;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CompetitveManager : SimpleNetworkedMonoBehaviorSingleton<CompetitveManager>
{
	[Header( "Competitve Manager" )]
	public int currentPlayer;

	public int p1Score;

	public int p2Score;

	public bool gameOver;

	public int winner;

	[Header( "Object References" )]
	public ParticleSystem DeathExplosionPrefab;

	// timing values
	private float startTime;

	private float currentTime;
	private float currentSec;
	private float prevSec;
	private int milSec;
	private int sec;

	public void ChangeTurn()
	{
		if ( IsServer )
		{
			currentPlayer = currentPlayer == 1 ? 2 : 1;

			milSec = 0;
			sec = 11;
			EventManager.RaiseTurnEvent( currentPlayer );
			print( "Turn Changed. Player " + currentPlayer + "'s turn" );
		}

		RPC( "SyncTurns", NetworkReceivers.Others, currentPlayer );
	}

	public bool IsOurTurn()
	{
		return IsServer ? currentPlayer == 1 : currentPlayer == 2;
	}

	public void PlayerDeath( int player )
	{
		if ( player == 1 )
		{
			p2Score++;
		}
		else
		{
			p1Score++;
		}

		RPC( "SyncScore", NetworkReceivers.Others, p1Score, p2Score );
	}

	public void UpdateRemotePlayersGravity( Vector3 newGravity, int playerType )
	{
		print( "Attempting to update gravity for remote end." );
		RPC( "ReceiveGravityUpdate", NetworkReceivers.Others, newGravity, playerType );
	}

	// Use this for initialization
	private void Start()
	{
		gameOver = false;
		winner = 0;
		startTime = Time.time;
		milSec = 0;
		sec = 10;
		p1Score = 0;
		p2Score = 0;

		currentPlayer = 1;
	}

	// add on death events
	private void OnEnable()
	{
		//EventManager.Grav += GravInterrupt;
		EventManager.Death += PlayerDeath;
	}

	// add on death events
	private void OnDisable()
	{
		//EventManager.Grav -= GravInterrupt;
		EventManager.Death -= PlayerDeath;
	}

	// Update is called once per frame
	private void FixedUpdate()
	{
		//if ( Networking.PrimarySocket.Players.Count == 0 ) return;

		if ( IsServer && ScoreReached() )
		{
			print( "Score reached!" );
			if ( !gameOver )
			{
				if ( p1Score > p2Score )
				{
					winner = 1;
				}
				else if ( p2Score > p1Score )
				{
					winner = 2;
				}
				gameOver = true;
				sec = 5;
				milSec = 0;

				RPC( "SyncWinner", NetworkReceivers.Others, winner, gameOver );
			}
		}

		currentTime = Time.time - startTime;
		currentSec = currentTime % 60f;

		if ( ( currentSec - prevSec ) >= 0 )
		{
			milSec++;
			if ( milSec == 60 )
			{
				if ( sec == 1 )
				{
					if ( !gameOver )
					{
						ChangeTurn();
					}
					else {
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;
						NetworkingSceneChanger.ExitToMainMenu();
					}
				}

				sec--;
				milSec = 0;
			}
		}

		prevSec = currentTime % 60f;
	}

	[BRPC]
	private void SyncWinner( int winner, bool gameOver )
	{
		this.winner = winner;
		this.gameOver = gameOver;
	}

	[BRPC]
	private void SyncTurns( int currentPlayer )
	{
		this.currentPlayer = currentPlayer;

		//EventManager.RaiseTurnEvent( currentPlayer );
	}

	[BRPC]
	private void SyncScore( int p1Score, int p2Score )
	{
		this.p1Score = p1Score;
		this.p2Score = p2Score;
	}

	[BRPC]
	private void ReceiveGravityUpdate( Vector3 newGravity, int playerType )
	{
		print( "Received new forced gravity from remote end." );
		EventManager.RotateGravity( new GravityEvent( newGravity, playerType ) );
	}

	private bool ScoreReached()
	{
		return p1Score >= 5 || p2Score >= 5;
	}
}