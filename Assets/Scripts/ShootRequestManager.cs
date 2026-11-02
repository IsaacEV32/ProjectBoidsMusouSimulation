using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ShootRequestManager : MonoBehaviour
{
    Queue<ShootRequest> shootRequests = new Queue<ShootRequest>();
    ShootRequest currentShootRequest;
    public static ShootRequestManager Instance;
    BoidManager boidManager;
    bool isProcessingRequest = false;
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
    }
    struct ShootRequest
    {
        public Transform shootOrigin;
        public GameObject bulletPrefab;
        public Action<bool> callback;
        public ShootRequest(Transform shootOrigin, GameObject bulletPrefab, Action<bool> callback)
        {
            this.shootOrigin = shootOrigin;
            this.bulletPrefab = bulletPrefab;
            this.callback = callback;
        }
    }
    public static void RequestShoot(Transform shootOrigin, GameObject bulletPrefab, Action<bool> callback)
    {
        ShootRequest newRequest = new ShootRequest(shootOrigin, bulletPrefab, callback);
        Instance.shootRequests.Enqueue(newRequest);
        Instance.TryProcessNext();
    }
    public void FinishProcessingShoot()
    {
        currentShootRequest.callback(true);
        isProcessingRequest = false;
        TryProcessNext();
    }
    void TryProcessNext()
    {
        if (!isProcessingRequest && shootRequests.Count > 0)
        {
            currentShootRequest = shootRequests.Dequeue();
            isProcessingRequest = true;
            boidManager.Shoot(currentShootRequest.shootOrigin);
        }
    }
}
