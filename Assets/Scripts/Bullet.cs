using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] float lifeTime = 5f;
    [SerializeField]LayerMask factionWhoShotTheBullet;

    public void LaunchBullet(LayerMask faction, Transform shootOrigin)
    {
        factionWhoShotTheBullet = faction;
        Physics.IgnoreCollision(this.gameObject.GetComponent<Collider>(), shootOrigin.GetComponent<Collider>());
        StartCoroutine(MoveBulletLoop());
    }
    void OnTriggerEnter(Collider other)
    {
        if ((factionWhoShotTheBullet.value & (1 << other.gameObject.layer)) == 0)
        {
            Debug.Log("Esta bala impacto con: " + other.gameObject.name);
            other.gameObject.SetActive(false);
            other.gameObject.GetComponent<Boids>().StopShootCoroutine();
            this.gameObject.transform.position = Vector3.zero;
            StopCoroutine(MoveBulletLoop());
            this.gameObject.SetActive(false);
        }
        else
        {
            Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), other);
            Debug.Log("Ignore: " + other.gameObject.name);
        }
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
        
        transform.gameObject.SetActive(false);
    }
}
