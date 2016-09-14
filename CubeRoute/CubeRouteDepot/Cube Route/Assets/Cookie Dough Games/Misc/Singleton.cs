using UnityEngine;

/// <summary>
/// This class offers a convenient way to declare any class a singleton by simply deriving from this class.
/// Example of correct syntax when using it:
/// "public class PlayerControls : Singleton<PlayerControls>"
/// Any singleton class can be accessed via its ClassName.Instance property.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance
    {
        get { return instance ?? ( instance = FindObjectOfType<T>() ); }
    }

    private static T instance { get; set; }
}