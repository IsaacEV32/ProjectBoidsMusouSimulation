using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] float lifeTime = 5f;

    public void LaunchBullet()
    {
        StartCoroutine(MoveBulletLoop());
        
    }
    IEnumerator MoveBulletLoop()
    {
        float timer = 0;
        while (lifeTime > timer)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }
        ShootRequestManager.Instance.FinishProcessingShoot();
        transform.gameObject.SetActive(false);
    }
}
