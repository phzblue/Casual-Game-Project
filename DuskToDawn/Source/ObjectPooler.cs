using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public GameObject pooledObject;

    public Transform targetParent;

    public int pooledAmount;

    List<GameObject> pooledObjects;

    // Start is called before the first frame update
    void Awake()
    {
        pooledObjects = new List<GameObject>();

        for (int i = 0; i < pooledAmount; i++)
        {
            GameObject obj;

            if(targetParent == null)
            {
                obj = (GameObject)Instantiate(pooledObject, transform);
            }
            else
            {
                obj = (GameObject)Instantiate(pooledObject, targetParent);
            }

            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        GameObject obj;

        if (targetParent == null)
        {
            obj = (GameObject)Instantiate(pooledObject, transform);
        }
        else
        {
            obj = (GameObject)Instantiate(pooledObject, targetParent);
        }

        pooledAmount++;
        obj.SetActive(false);
        pooledObjects.Add(obj);
        return obj;
    }

    public GameObject AddToObjectPool()
    {
        GameObject obj = (GameObject)Instantiate(pooledObject, targetParent);
        pooledAmount++;
        obj.SetActive(false);
        pooledObjects.Add(obj);
        return obj;
    }
}
