using UnityEngine;

namespace Teston.Utils
{ 
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        [SerializeField]
        private bool _dontKillMe = false;

        private static T _instance;
        public static T Instance
        {
            get { return _instance; }
        }

        public static bool IsInitialized
        {
            get { return _instance != null; }
        }

        protected virtual void Awake()
        {
            if (_instance != null)
            {
                Debug.LogWarning("[Singleton] Trying to instantiate a second instance of a singleton");
                Destroy(gameObject);
            }
            else
            {
                _instance = (T)this;

                if(_dontKillMe)
                    DontDestroyOnLoad(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}
