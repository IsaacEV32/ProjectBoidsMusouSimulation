using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    [Header("Prefab of boid")]
    [SerializeField] GameObject boidPrefab;
    [SerializeField] GameObject boid2Prefab;
    [Header("Spawn info")]
    [SerializeField] int capacity1 = 10;
    [SerializeField] float radioSpawn1 = 10;

    [SerializeField] int capacity2 = 10;
    [SerializeField] float radioSpawn2 = 10;

    [SerializeField] public float radiusZone = 50;
    public static BoidManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    public List<Boids> boids = new List<Boids>();
    private void Start()
    {
        for (int i = 0; i < capacity1; i++)
        {
            GameObject go = Instantiate(boidPrefab, transform.position + Random.insideUnitSphere * radioSpawn1, Random.rotation);
            boids.Add(go.GetComponent<Boids>());
        }
        for (int i = 0; i < capacity2; i++)
        {
            GameObject go = Instantiate(boid2Prefab, transform.position + Random.insideUnitSphere * radioSpawn2, Random.rotation);
            boids.Add(go.GetComponent<Boids>());
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position, radiusZone);
    }
}
