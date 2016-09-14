using BeardedManStudios.Network;

/// <summary>
/// Offers the same behavior as the normal Singleton class, but restricted specifically to SimpleNetworkedMonoBehaviors.
/// This class is needed since SimpleNetworkedMonoBehavior extends MonoBehavior, we cannot use the normal Singleton class which also extends MonoBehavior.
/// </summary>
/// <typeparam name="T"></typeparam>
public class SimpleNetworkedMonoBehaviorSingleton<T> : SimpleNetworkedMonoBehavior where T : SimpleNetworkedMonoBehavior
{
	public static T Instance
	{
		get { return instance ?? ( instance = FindObjectOfType<T>() ); }
	}

	private static T instance { get; set; }
}