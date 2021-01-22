using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiasGames.ThirdPersonSystem
{
    public class ObjectPooler : MonoBehaviour
    {
        private Dictionary<int, Queue<GameObject>> PoolDictionary;       // Main pool controller

        [SerializeField] private int m_DefaultMaximumObjects = 50;               // Maximum amount of object that should be instantiate

        #region Singleton

        public static ObjectPooler Instance;

        #endregion

        private void Awake()
        {
            if (Instance != null)       // Check if static instance already exists to avoid more than one instance
                Instance = null;

            Instance = this;            // Set instance to this

            PoolDictionary = new Dictionary<int, Queue<GameObject>>();   // Starts a new pool
        }


        /// <summary>
        /// Create pool for a specific type of object
        /// </summary>
        /// <param name="objectToPool"></param>
        /// <param name="amount"></param>
        public void CreatePool(GameObject objectToPool, int amount)
        {
            if (!PoolDictionary.ContainsKey(objectToPool.GetInstanceID()))     // check if object already exists in the pool
            {
                // Add this type of object to the list
                Queue<GameObject> objectQueue = new Queue<GameObject>();

                for (int i = 0; i < amount; i++)
                {
                    GameObject obj = Instantiate(objectToPool, transform);

                    obj.SetActive(false);
                    objectQueue.Enqueue(obj);
                }

                PoolDictionary.Add(objectToPool.GetInstanceID(), objectQueue);         // Add this type of Object to the pool
            }
        }


        public GameObject Spawn(GameObject objectToSpawn, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            CreatePool(objectToSpawn, m_DefaultMaximumObjects);     // Create pool for this gameobject, if it already exists, nothing will happen

            GameObject spawnedObject = PoolDictionary[objectToSpawn.GetInstanceID()].Dequeue();

            spawnedObject.SetActive(true);
            spawnedObject.transform.position = position;
            spawnedObject.transform.rotation = rotation;
            
            if (parent != null)
            {
                spawnedObject.transform.parent = parent;
            }

            PoolDictionary[objectToSpawn.GetInstanceID()].Enqueue(spawnedObject);
            return spawnedObject;
        }



        public GameObject Spawn(GameObject spawnObject)
        {
            return Spawn(spawnObject, Vector3.zero, Quaternion.identity);
        }


        public void Remove(GameObject target)
        {
            target.SetActive(false);
        }
    }
}