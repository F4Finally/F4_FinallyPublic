using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public bool isDontDestroyOnLoad; //얘가 트루일 때만 돈디스트로이

    private static T _instance;
    public static T Instance
    {
        get
        {

            if (_instance == null)
            {
                SetupInstance();
            }  

            return _instance;
        }
    }

    public virtual void Awake()
    {
        RemoveDuplicates();
    }

    private static void SetupInstance()
    {
        _instance = (T)FindAnyObjectByType(typeof(T), FindObjectsInactive.Include);

        if (_instance == null)
        {
            GameObject gameObject = new GameObject();
            gameObject.name = typeof(T).Name;
            _instance = gameObject.AddComponent<T>();
            
        }

        if (_instance.isDontDestroyOnLoad)
        {
            DontDestroyOnLoad(_instance.gameObject);
        }
    }

    private void RemoveDuplicates()
    {
        if (_instance == null)
        {
            _instance = this as T;

            if (isDontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else
        {
            if (_instance != this) 
            {
                Destroy(gameObject);
            }
            
        }
    }
}