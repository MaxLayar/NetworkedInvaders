using UnityEngine;

namespace NetworkedInvaders.Manager
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        // Can be overriden if some singletons doesn't need to be scene persistant.
        public virtual bool isDontDestroyOnLoad => true;

        protected static T instance;
        public static T Instance
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying && !instance)
                    instance = FindInstance();
#endif
                
                return instance ??= FindInstance();

                static T FindInstance()
                {
                    T found = FindFirstObjectByType<T>();

                    if (!found)
                        Debug.LogError($"Your singleton '{typeof(T).Name}' doesn't exist.");
                    return found;
                }
            }
        }

        protected virtual void Awake()
        {
            if (instance && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this as T;

            if (isDontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);

            OnAwake();
        }

        protected virtual void OnAwake() { }
    }
}