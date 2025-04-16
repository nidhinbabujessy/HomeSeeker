using UnityEngine;
using System.Collections.Generic;

public class ObjectPoolScript
{
    private GameObject prefab;
    private Queue<GameObject> pool;

    public ObjectPoolScript(GameObject prefab, int initialSize)
    {
        this.prefab = prefab;
        pool = new Queue<GameObject>();

        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Object.Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject Get(Vector3 position, Quaternion rotation, Transform parent = null)
    {
        GameObject obj = pool.Count > 0 ? pool.Dequeue() : Object.Instantiate(prefab);
        obj.transform.SetParent(parent);
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);
        return obj;
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
