using UnityEngine;
 
/// <summary>
/// Be aware this will not prevent a non singleton constructor
///   such as `T myT = new T();`
/// To prevent that, add `protected T () {}` to your singleton class.
/// 
/// As a note, this is made as MonoBehaviour because we need Coroutines.
/// </summary>

public class GameSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private volatile static T s_Instance = null; // Should be volatile for doubly-checked lock, via https://msdn.microsoft.com/en-us/library/ff650316.aspx
	private static object s_Lock = new object();

	public static bool HasInstance {
		get {
			try{
				return Instance != null;
			}catch (UnityException e){
				return false;
			}
		}
	}

	public static T Instance
	{
		get
		{
			// Doubly-checked lock, copied from https://msdn.microsoft.com/en-us/library/ff650316.aspx
			if (s_Instance != null)
			{
				return s_Instance;
			}

			lock (s_Lock)
			{
				if (s_Instance == null)
				{
					Object[] objects = FindObjectsOfType(typeof(T));

					if (objects.Length == 0)
					{
						throw new UnityException("Missing singleton component " + typeof(T) + "; be sure an instance of this object has been loaded");
					}
					if (objects.Length > 1)
					{
						Debug.LogError("[Singleton] Found " + objects.Length + " components of type " + typeof(T)
							+ "There should never be more than 1 singleton object!");
					}

					s_Instance = (T)objects[0];
				}
			}

			return s_Instance;
		}
	}
}