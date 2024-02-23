using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerWeapon : MonoBehaviour
{
    [Header("Input")]
    public InputActionData fireEvent;
    
    [Header("Settings")]
    public Transform cameraTransform;
    public float bulletLifetime = 3.5f;
    public Transform muzzle;

    readonly Stack<Rigidbody> m_pool = new(20);
    Transform m_poolParent;
    int m_bulletsCount;

    void Awake()
    {
        InputObserverTool.Bind(ref fireEvent);
    }

    void Update()
    {
        if (fireEvent.IsStarted) Fire();
    }
    
    private void Fire()
        => StartCoroutine(IEFire());

    private IEnumerator IEFire()
    {
        Rigidbody bullet;
        Transform bullet_tr;
        
        if (m_pool.Count > 0)
        {
            bullet = m_pool.Pop();
            bullet_tr = bullet.transform;
            bullet.gameObject.SetActive(true);
        }
        else
        {
            if (m_poolParent == null) m_poolParent = new GameObject("BulletsPool").transform;
            
            bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<Rigidbody>();
            bullet.name = $"Bullet ({m_bulletsCount++})";
            
            bullet_tr = bullet.transform;
            bullet_tr.SetParent(m_poolParent);
            bullet_tr.localScale = muzzle.localScale;
        }

        bullet_tr.position = muzzle.position;

        bullet.AddForce(cameraTransform.forward * Random.Range(25f, 35f)
                      + cameraTransform.right * Random.Range(-2f, 2f)
                      + cameraTransform.up * Random.Range(-2f, 2f),
                        ForceMode.Impulse);

        for (float t = 0f; t < bulletLifetime; t += Time.deltaTime)
        {
            yield return null;
        }

        bullet.velocity = Vector3.zero;
        bullet.gameObject.SetActive(false);
        m_pool.Push(bullet);
    }
};
