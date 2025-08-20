using UnityEngine;

namespace NetworkedInvaders.Manager
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public virtual bool isDontDestroyOnLoad => true;

        protected static T instance;
        public static T Instance
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying && instance == null)
                    instance = FindFirstObjectByType<T>();
#endif

                if (instance == null)
                {
                    instance = FindFirstObjectByType<T>();

                    if (instance == null)
                        Debug.LogError($"Your singleton '{typeof(T).Name}' doesn't exist.");
                }

                return instance;
            }
        }

        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this as T;

            if (isDontDestroyOnLoad)
                DontDestroyOnLoad(instance);

            OnAwake();
        }

        protected virtual void OnAwake() { }
    }
}