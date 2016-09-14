using BeardedManStudios.Network;
using BeardedManStudios.Network.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkingMainMenu : Singleton<NetworkingMainMenu>
{
	[Header( "Networking Main Menu" )]
	public InputField ipAddressInput = null;

	public GameObject MenuPanel;

	private string host = "127.0.0.1";                                                                       // IP address
	private ushort port = 15987;                                                                                // Port number (Forge uses 15937 by default)
	private Networking.TransportationProtocolType protocolType = Networking.TransportationProtocolType.UDP;  // Communication protocol
	private int playerCount = 31;                                                                            // Maximum player count -- excluding this server
	private NetWorker socket = null;                                                                        // The initial connection socket
	private string selectedScene = null;

	/// <summary>
	/// Determine if the current system is within the "WinRT" ecosystem
	/// </summary>
	private bool IsWinRT
	{
		get
		{
#if UNITY_4_6
				return Application.platform == RuntimePlatform.MetroPlayerARM ||
					Application.platform == RuntimePlatform.MetroPlayerX86 ||
					Application.platform == RuntimePlatform.MetroPlayerX64;
#else
			return Application.platform == RuntimePlatform.WSAPlayerARM ||
				Application.platform == RuntimePlatform.WSAPlayerX86 ||
				Application.platform == RuntimePlatform.WSAPlayerX64;
#endif
		}
	}

	private void OnLevelWasLoaded()
	{
		MenuPanel.SetActive( NetworkingSceneChanger.IsMainMenu() );
	}

	public void Start()
	{
		// Assign the text for the input to be whatever is set by default
		//        ipAddressInput.text = host;

		// These devices have no reason to fire off a firewall check as they are not behind a local firewall
#if !UNITY_IPHONE && !UNITY_WP_8_1 && !UNITY_ANDROID
		// Check to make sure that the user is allowing this connection through the local OS firewall
		Networking.InitializeFirewallCheck( port );
#endif

#if UNITY_STANDALONE_LINUX
			if (autoStartServer)
				StartServer();
#endif
	}

	public void StartServerCooperative()
	{
		selectedScene = NetworkingSceneChanger.CoopScenes[0];
		StartServer();
	}

	public void StartServerCompetitive()
	{
		selectedScene = NetworkingSceneChanger.CompetitiveScene;
		StartServer();
	}

	private void StartServer()
	{
		if ( Networking.IsConnected( port ) )
		{
			Debug.LogError( "Cannot start server, port is already active." );
			return;
		}

		// Create a host connection
		socket = Networking.Host( port, protocolType, playerCount, IsWinRT );

		WaitForSocketConnect();
	}

	public void StartClientWithIpInputField()
	{
		host = ipAddressInput.text;

		if ( string.IsNullOrEmpty( host.Trim() ) )
		{
			Debug.Log( "No IP address provided to connect to" );
			return;
		}

		StartClient( host );
	}

	/// <summary>
	/// Called from within this same script, or by clicking a server history button.
	/// </summary>
	/// <param name="host"></param>
	public void StartClient( string host )
	{
		this.host = host;
		selectedScene = null;

		socket = Networking.Connect( host, port, protocolType, IsWinRT );

		if ( !socket.Connected )
		{
			socket.ConnectTimeout = 5000;
			socket.connectTimeout += OnConnectTimeout;
		}

		WaitForSocketConnect();
	}

	private void OnConnectTimeout()
	{
		Debug.LogWarning( "Connection could not be established." );
		Networking.Disconnect();
	}

	public void StartClientSinglePlayer()
	{
		selectedScene = NetworkingSceneChanger.SoloScenes[0];
		UnitySceneManager.LoadScene( selectedScene );
	}

	public void StartClientLan()
	{
#if !NETFX_CORE
		System.Net.IPEndPoint endpoint = Networking.LanDiscovery( port, 5000, protocolType, IsWinRT );
#else
			IPEndPointWinRT endpoint = Networking.LanDiscovery(port, 5000, protocolType, IsWinRT);
#endif
		if ( endpoint == null )
		{
			Debug.Log( "No server found on LAN" );
			return;
		}

		string ipAddress = string.Empty;
		ushort targetPort = 0;

#if !NETFX_CORE
		ipAddress = endpoint.Address.ToString();
		targetPort = (ushort)endpoint.Port;
#else
			ipAddress = endpoint.ipAddress;
			targetPort = (ushort)endpoint.port;
#endif

		socket = Networking.Connect( ipAddress, targetPort, protocolType, IsWinRT );

		WaitForSocketConnect();
	}

	private void WaitForSocketConnect()
	{
		Networking.SetPrimarySocket( socket );
		Networking.networkReset += RemoveSocketReference;

		if ( socket.Connected )
		{
			MainThreadManager.Run( OnSocketConnect );
		}
		else
		{
			socket.connected += OnSocketConnect;
		}
	}

	private void OnSocketConnect()
	{
		socket.connected -= OnSocketConnect;

		if ( socket.IsServer )
		{
			// When a client connects, instruct it to change to the scene that the server is currently on
			socket.playerConnected += delegate ( NetworkingPlayer player )
			{
				MainThreadManager.Run( () =>
				{
					Networking.ChangeClientScene( Networking.PrimarySocket, player, SceneManager.GetActiveScene().name );
				} );
			};

			SceneManager.LoadScene( selectedScene );
		}
		else // is client
		{
			ServerHistoryManager.Instance.SaveIpAddress( host );
			print( "Saving IP: " + host );

			socket.serverDisconnected += delegate ( string reason )
			{
				MainThreadManager.Run( () =>
				{
					Debug.Log( "The server kicked you for reason: " + reason );
					NetworkingSceneChanger.ExitToMainMenu();
				} );
			};
			// Wait for server to instruct scene change
		}
	}

	private void RemoveSocketReference()
	{
		socket = null;
		Networking.networkReset -= RemoveSocketReference;
	}

	public void ExitGame()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
	}

	public void ToggleSettingsDisplay()
	{
		SettingsMenu.instance.ToggleDisplay();
	}
}