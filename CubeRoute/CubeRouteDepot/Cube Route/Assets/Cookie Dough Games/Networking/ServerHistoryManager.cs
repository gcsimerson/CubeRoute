using BeardedManStudios.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script is responsible for remembering IP addresses previously connected to, as well as pinging them to see if they are active.
/// </summary>
public class ServerHistoryManager : Singleton<ServerHistoryManager>
{
	//    [Header( "Server History Manager" )]

	[Header( "Object References" )]
	public VerticalLayoutGroup HistoryPanel;

	public ServerHistoryEntry ServerHistoryEntryPrefab;

	public VerticalLayoutGroup ServerContainer;

	/// <summary>A list of previously-connected to IP addresses. The key is the IP address and the value is the alias (which may be null, if none was given).</summary>
	private Dictionary<string, string> history;

	/// <summary>A list of ServerHistoryEntry objects that have been instantiated.</summary>
	private List<ServerHistoryEntry> entries = new List<ServerHistoryEntry>();

	public void OnEnable()
	{
		LoadSettings();

		//		ClearAllSavedIpAddresses();

		//		SaveIpAddress( "152.7.74.241", "BBL Desktop" );
		//		SaveIpAddress( "152.7.74.22", "BBL Laptop" );
		//		SaveIpAddress( "152.7.224.2", "NCSU Wireless" );
		//		SaveIpAddress( "152.7.224.4" );

		PopulateServerList();
	}

	public void SaveIpAddress( string ipAddress, string alias = null )
	{
		ipAddress = ipAddress.ToLower();

		if ( ipAddress != "127.0.0.1" && ipAddress != "localhost" && !history.ContainsKey( ipAddress ) )
		{
			history.Add( ipAddress, alias );
			SaveSettings();
		}
	}

	public void RemoveIpAddress( string ipAddress )
	{
		ipAddress = ipAddress.ToLower();

		if ( history.ContainsKey( ipAddress ) )
		{
			history.Remove( ipAddress );
			SaveSettings();
		}
	}

	/// <summary>
	/// Use with caution. This is not reversable.
	/// </summary>
	public void ClearAllSavedIpAddresses()
	{
		foreach ( ServerHistoryEntry entry in entries )
		{
			Destroy( entry.gameObject );
		}
		entries.Clear();

		history.Clear();
		SaveSettings();

		HistoryPanel.gameObject.SetActive( false );
	}

	/// <summary>
	/// Sort the server history based on which are available.
	/// </summary>
	public void SortEntries()
	{
		// Sort by server availability and then by alphabetical order
		entries.Sort( ( a, b ) => a.Button.IsInteractable() != b.Button.IsInteractable() ?
		b.Button.IsInteractable().CompareTo( a.Button.IsInteractable() ) :
			string.Compare( b.IpAddress, a.IpAddress, StringComparison.OrdinalIgnoreCase ) );

		// Reorder the hierarchical sorting in the vertical layout group
		for ( int i = 0; i < entries.Count; i++ )
			entries[i].transform.SetSiblingIndex( i );
	}

	private void PopulateServerList()
	{
		foreach ( ServerHistoryEntry entry in entries )
		{
			Destroy( entry.gameObject );
		}
		entries.Clear();

		foreach ( KeyValuePair<string, string> kvp in history )
		{
			ServerHistoryEntry entry = Instantiate( ServerHistoryEntryPrefab );
			entries.Add( entry );
			entry.Initialize( kvp.Key, kvp.Value );
			entry.transform.SetParent( ServerContainer.transform );
		}

		HistoryPanel.gameObject.SetActive( entries.Count > 0 );
	}

	#region Serialization

	private void SaveSettings()
	{
		string filePath = Application.persistentDataPath + "/ServerHistory.dat";

		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create( filePath );

		ServerHistoryManagerData data = new ServerHistoryManagerData( history );

		bf.Serialize( file, data );
		file.Close();
	}

	private void LoadSettings()
	{
		string filePath = Application.persistentDataPath + "/ServerHistory.dat";

		if ( File.Exists( filePath ) )
		{
			FileStream file = File.Open( filePath, FileMode.Open );
			try
			{
				BinaryFormatter bf = new BinaryFormatter();
				ServerHistoryManagerData data = (ServerHistoryManagerData)bf.Deserialize( file );
				history = data.ConvertToDictionary();
				file.Close();
			}
			catch ( Exception )
			{
				Debug.LogWarning( "Failed to deserialize. Deleting: " + filePath );
				file.Close();
				File.Delete( filePath );
			}
		}

		if ( history == null )
		{
			history = new Dictionary<string, string>();
		}
	}

	#endregion Serialization
}

[Serializable]
internal class ServerHistoryManagerData
{
	private Hashtable history;

	public ServerHistoryManagerData( Dictionary<string, string> history )
	{
		this.history = new Hashtable( history );
	}

	public Dictionary<string, string> ConvertToDictionary()
	{
		return history.Cast<DictionaryEntry>().ToDictionary( kvp => (string)kvp.Key, kvp => (string)kvp.Value );
	}
}