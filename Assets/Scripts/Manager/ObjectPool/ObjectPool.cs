using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;
    
    [SerializeField] private int poolSize = 10;
    
    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new();

    [Header("To Initialize")] [SerializeField]
    private GameObject weaponPickup;
    [SerializeField] private GameObject ammoPickup;
    
    private void Awake()
    {
        if(!instance)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        InitializeNewPool(weaponPickup);
        InitializeNewPool(ammoPickup);
    }

    public GameObject GetObject(GameObject prefab, Transform target)
    {
        if(!poolDictionary.ContainsKey(prefab)){
        {
            InitializeNewPool(prefab);
        }}
        
        if(poolDictionary[prefab].Count == 0)
            CreateNewObject(prefab);
        
        GameObject objToGet = poolDictionary[prefab].Dequeue();
        
        objToGet.transform.position = target.position;
        objToGet.SetActive(true);
        objToGet.transform.parent = null;
        return objToGet;
    }

    public void ReturnObject(GameObject objToReturn, float delay = 0.001f)
    {
        StartCoroutine(DelayReturn(delay, objToReturn));
    }
    
    private IEnumerator DelayReturn(float delay, GameObject objToReturn)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPool(objToReturn);
    }

    private void ReturnToPool(GameObject objToReturn)
    {
        GameObject originalPrefab = objToReturn.GetComponent<PooledObject>().originalPrefab;
        objToReturn.SetActive(false);
        objToReturn.transform.parent = transform;
        poolDictionary[originalPrefab].Enqueue(objToReturn);
    }
    
    private void InitializeNewPool(GameObject prefab)
    {
        poolDictionary[prefab] = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewObject(prefab);
        }
    }

    private void CreateNewObject(GameObject prefab)
    {
        GameObject newObj = Instantiate(prefab, transform);
        newObj.AddComponent<PooledObject>().originalPrefab = prefab;
        newObj.SetActive(false);
        poolDictionary[prefab].Enqueue(newObj);
    }
}