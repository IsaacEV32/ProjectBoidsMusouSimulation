using System.Collections;
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

    [SerializeField] GameObject bulletPrefab;
    [SerializeField] int numberOfBullets = 5;
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
    public GameObject GetBulletPrefab()
    {
        return bulletPrefab;
    }
    public List<Boids> boids = new List<Boids>();
    List<GameObject> bullets = new List<GameObject>();
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
        for (int i = 0; i < numberOfBullets; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            bullet.SetActive(false);
            bullets.Add(bullet);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position, radiusZone);
    }
    public void Shoot(Transform shootOrigin)
    {
        foreach (GameObject bullet in bullets)
        {
            if (!bullet.activeInHierarchy)
            {
                bullet.SetActive(true);
                bullet.transform.position = shootOrigin.position;
                bullet.transform.forward = shootOrigin.forward;
                bullet.GetComponent<Bullet>().LaunchBullet();
                ShootRequestManager.Instance.FinishProcessingShoot();
                break;
            }
        }

    }
}
