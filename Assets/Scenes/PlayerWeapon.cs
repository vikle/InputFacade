using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public sealed class PlayerWeapon : MonoBehaviour
{
    public Transform muzzle;
    public float bulletLifetime = 3.5f;
    internal Transform cameraTransform;

    Stack<GameObject> m_pool;
    Transform m_poolParent;
    int m_bulletsCount;

    void Start()
    {
        m_pool = new(20);
        m_poolParent = new GameObject("BulletsPool").transform;
    }

    public void Fire()
        => StartCoroutine(IEFire());

    private IEnumerator IEFire()
    {
        GameObject bullet;
        Rigidbody r_body;

        if (m_pool.Count > 0)
        {
            bullet = m_pool.Pop();
            r_body = bullet.GetComponent<Rigidbody>();
            bullet.SetActive(true);
            
        }
        else
        {
            bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bullet.name = $"Bullet ({m_bulletsCount})";
            bullet.transform.SetParent(m_poolParent);
            bullet.transform.localScale = muzzle.localScale;
            r_body = bullet.AddComponent<Rigidbody>();

            m_bulletsCount++;
        }

        bullet.transform.position = muzzle.position;

        r_body.AddForce(cameraTransform.forward * Random.Range(25f, 35f)
                      + cameraTransform.right * Random.Range(-2f, 2f)
                      + cameraTransform.up * Random.Range(-2f, 2f),
                        ForceMode.Impulse);

        for (float t = 0f; t < bulletLifetime; t += Time.deltaTime)
        {
            yield return null;
        }

        r_body.velocity = Vector3.zero;
        bullet.SetActive(false);
        m_pool.Push(bullet);
    }
};
