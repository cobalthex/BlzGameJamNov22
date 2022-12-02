// Singleton Pattern
// See http://wiki.unity3d.com/index.php/Singleton
using UnityEngine;

public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T m_instance;

    private static object m_lock = new object();

    public static T Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                return null;
            }

            lock (m_lock)
            {
                if (m_instance == null)
                {
                    m_instance = (T)FindObjectOfType(typeof(T));
                }

                return m_instance;
            }
        }
    }

    private static bool applicationIsQuitting = false;
    /// <summary>
    /// When Unity quits, it destroys objects in a random order.
    /// In principle, a Singleton is only destroyed when application quits.
    /// If any script calls Instance after it have been destroyed, 
    ///   it will create a buggy ghost object that will stay on the Editor scene
    ///   even after stopping playing the Application. Really bad!
    /// So, this was made to be sure we're not creating that buggy ghost object.
    /// </summary>
    private void OnApplicationQuit()
    {
        applicationIsQuitting = true;
    }

    public virtual void OnAwake()
    {
    }

    private void Awake()
    {
        // If we already have an instance and it's not this
        if (m_instance != null && m_instance != this)
            Destroy(gameObject);

        m_instance = GetComponent<T>();
        OnAwake();
    }
}
