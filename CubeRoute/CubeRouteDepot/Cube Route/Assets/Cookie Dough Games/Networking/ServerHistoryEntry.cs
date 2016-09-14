using BeardedManStudios.Network;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script is attached to every server prefab in the "server history" log of the multiplayer screen.
/// </summary>
public class ServerHistoryEntry : MonoBehaviour
{
	[Header( "Object References" )]
	public Button Button;

	public Text ButtonText;

	private float lastPingReceivedTime;

	private bool pingReceived;

	//    [Header( "Server History Entry" )]
	public string IpAddress { get; private set; }

	public string Alias { get; private set; }

	/// <summary>
	/// Called by Server History Manager immediately after being instantiated.
	/// </summary>
	public void Initialize( string ipAddress, string alias = null )
	{
		this.IpAddress = ipAddress;
		this.Alias = alias;

		ButtonText.text = ipAddress + ( !string.IsNullOrEmpty( alias ) ? " (" + alias + ")" : string.Empty );

		OnEnable();
	}

	public void OnEnable()
	{
		//        DisableButton();

		if ( IpAddress != null )
		{
			Networking.pingReceived += PingRecieved;
			InvokeRepeating( "SendPing", 0, 5 );
			//            SendPing();
		}
	}

	public void OnDisable()
	{
		CancelInvoke();
		Networking.pingReceived -= PingRecieved;
	}

	public void OnClick()
	{
		NetworkingMainMenu.Instance.StartClient( IpAddress );
	}

	private void Update()
	{
		if ( pingReceived )
		{
			lastPingReceivedTime = Time.time;
			pingReceived = false;

			EnableButton();
		}

		if ( lastPingReceivedTime.TimeElapsedSince() > 5 && Button.IsInteractable() )
		{
			DisableButton();
		}
	}

	private void EnableButton()
	{
		Button.interactable = true;
		ServerHistoryManager.Instance.SortEntries();
	}

	private void DisableButton()
	{
		Button.interactable = false;
		ServerHistoryManager.Instance.SortEntries();
	}

	private void SendPing()
	{
		HostInfo host = new HostInfo();

		host.ipAddress = this.IpAddress;
		host.port = 15987;

		Networking.Ping( host );

		Debug.Log( "Ping request sent to " + IpAddress );
	}

	private void PingRecieved( HostInfo host, int time )
	{
		if ( host.IpAddress == this.IpAddress )
		{
			//            CancelInvoke( "DisableButton" );
			//            Invoke( "SendPing", 5 );
			//            Invoke( "DisableButton", 10 );

			//            EnableButton(); // can't do inside call back
			//            lastPingReceivedTime = Time.time; // can't do inside call back

			pingReceived = true; // Have to use this bool due to thread constrictions

			Debug.Log( "Ping received from " + host.IpAddress + " in " + time + " ms" );
		}
	}
}