using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

public class ShootRequestManager : MonoBehaviour
{
    Queue<ShootRequest> shootRequests = new Queue<ShootRequest>();
    ShootRequest currentShootRequest;
    public static ShootRequestManager Instance;
    BoidManager boidManager;
    bool isProcessingRequest = false;
    static int maxRequests;
    static readonly ProfilerMarker pShootRequest = new ProfilerMarker("ShootRequest");
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
        boidManager = GetComponent<BoidManager>();
        maxRequests = boidManager.GetNumberOfBullets();
    }
    struct ShootRequest
    {
        public Transform shootOrigin;
        public GameObject bulletPrefab;
        public ShootRequest(Transform shootOrigin, GameObject bulletPrefab)
        {
            this.shootOrigin = shootOrigin;
            this.bulletPrefab = bulletPrefab;
        }
    }

    public static void RequestShoot(Transform shootOrigin, GameObject bulletPrefab)
    {

        if (Instance.shootRequests.Count >= maxRequests)
        {
            return;
        }
        ShootRequest newRequest = new ShootRequest(shootOrigin, bulletPrefab);
        Instance.shootRequests.Enqueue(newRequest);
        Instance.TryProcessNext();
    }
    public void FinishProcessingShoot()
    {
        isProcessingRequest = false;
        TryProcessNext();
    }
    void TryProcessNext()
    {
        using (pShootRequest.Auto())
        {
            if (!isProcessingRequest && shootRequests.Count > 0)
            {
                currentShootRequest = shootRequests.Dequeue();
                isProcessingRequest = true;
                if (!currentShootRequest.shootOrigin.gameObject.activeSelf)
                {
                    FinishProcessingShoot();
                    return;
                }
                boidManager.Shoot(currentShootRequest.shootOrigin);
            }
        }
    }
}
