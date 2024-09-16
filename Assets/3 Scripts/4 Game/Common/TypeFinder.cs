using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TypeFinder : MonoBehaviour
{
    [SerializeField]
    private string typeName;
    private Type type;

    public UnityEvent<GameObject> OnEnter { get; private set; }
    public UnityEvent<GameObject> OnExit { get; private set; }

    private List<GameObject> gameObjects;

    private void Awake()
    {
        type = Type.GetType(typeName);

        OnEnter = new UnityEvent<GameObject>();
        OnExit = new UnityEvent<GameObject>();

        gameObjects = new List<GameObject>();
    }

    private void OnEnable()
    {
        OnEnter.RemoveAllListeners();
        OnExit.RemoveAllListeners();

        gameObjects.Clear();
    }

    public GameObject[] GetGameObjects()
    {
        return gameObjects.ToArray();
    }

    public GameObject GetClosestGameObject()
    {
        return gameObjects
                    .OrderBy(gobj => Vector3.Distance(transform.position, gobj.transform.position))
                    .FirstOrDefault();
    }

    public GameObject GetClosestGameObjectWithout(GameObject without)
    {
        return gameObjects
                    .Where(gobj => gobj != without)
                    .OrderBy(gobj => Vector3.Distance(transform.position, gobj.transform.position))
                    .FirstOrDefault();
    }

    public GameObject GetClosestGameObjectWithout(List<GameObject> without)
    {
        return gameObjects
                    .Where(gobj => !without.Contains(gobj))
                    .OrderBy(gobj => Vector3.Distance(transform.position, gobj.transform.position))
                    .FirstOrDefault();
    }

    public GameObject GetRandomGameObejectWithout(List<GameObject> without)
    {
        List<GameObject> list = gameObjects
                                .Where(gobj => !without.Contains(gobj))
                                .ToList();

        return list.Count == 0 ? null : list[UnityEngine.Random.Range(0, list.Count)];
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(type, out Component component))
        {
            OnEnter.Invoke(component.gameObject);
            gameObjects.Add(component.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(type, out Component component))
        {
            OnExit.Invoke(component.gameObject);
            gameObjects.Remove(component.gameObject);
        }
    }
}
