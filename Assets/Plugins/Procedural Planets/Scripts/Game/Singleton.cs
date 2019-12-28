using UnityEngine;

namespace ProceduralPlanets
{
    /// <summary>
    /// Singleton class that the Manager derives from to ensure only one peristent instance exists of the Manager.
    /// 
    /// Version 1.0 - 2018 (c) Imphenzia AB - Author: Stefan Persson
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                    if (instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = typeof(T).Name;
                        instance = obj.AddComponent<T>();
                    }
                }
                return instance;
            }
        }

        public virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
#if !UNITY_EDITOR
            DontDestroyOnLoad(this.gameObject);
#endif
            }
            else
            {
#if !UNITY_EDITOR
            Destroy(gameObject);
#endif
            }
        }
    }
}
