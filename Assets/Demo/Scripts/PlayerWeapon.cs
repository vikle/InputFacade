using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerWeapon : MonoBehaviour
{
    [Header("Input")]
    public InputActionData fireEvent;
    public InputActionData fireModeEvent;

    [Header("Settings")]
    public Transform cameraTransform;
    public float bulletLifetime = 3.5f;
    public int shotsPerMinute = 650;
    public Transform muzzle;

    readonly List<Bullet> m_bullets = new(20);
    readonly Queue<Bullet> m_pool = new(20);
    Transform m_poolParent;
    bool m_autoFire;
    bool m_autoFireReady;
    float m_fireInterval;

    void Awake()
    {
        m_autoFireReady = true;
        InputObserverTool.Bind(ref fireEvent);
        InputObserverTool.Bind(ref fireModeEvent);
    }

    void Update()
    {
        if (fireModeEvent.IsStarted)
        {
            m_autoFire = !m_autoFire;
        }

        if (m_autoFire)
        {
            UpdateAutoFire();
        }
        else if (fireEvent.IsStarted)
        {
            Fire();
        }
        
        UpdateBullets();
    }

    private void UpdateAutoFire()
    {
        if (m_autoFireReady == false)
        {
            m_fireInterval -= Time.deltaTime;
            if (m_fireInterval > 0f) return;
            m_autoFireReady = true;
            return;
        }

        if (fireEvent.IsPerformed == false) return;
        Fire();
        m_autoFireReady = false;
        m_fireInterval = (60f / shotsPerMinute);
    }

    private void Fire()
    {
        Bullet bullet;

        if (m_pool.Count > 0)
        {
            bullet = m_pool.Dequeue();
        }
        else
        {
            if (m_poolParent == null) m_poolParent = new GameObject("BulletsPool").transform;

            var bullet_obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bullet_obj.name = $"Bullet ({m_poolParent.childCount.ToString()})";

            bullet = new()
            {
                target = bullet_obj.AddComponent<Rigidbody>(),
                transform = bullet_obj.transform
            };

            m_bullets.Add(bullet);

            bullet.transform.SetParent(m_poolParent);
            bullet.transform.localScale = muzzle.localScale;
            bullet.target.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        bullet.transform.position = muzzle.position;
        
        bullet.Activate(bulletLifetime);

        bullet.target.AddForce(cameraTransform.forward * Random.Range(25f, 35f)
                             + cameraTransform.right * Random.Range(-2f, 2f)
                             + cameraTransform.up * Random.Range(-2f, 2f),
                               ForceMode.Impulse);
    }

    private void UpdateBullets()
    {
        for (int i = 0, i_max = m_bullets.Count; i < i_max; i++)
        {
            var bullet = m_bullets[i];
            if (bullet.isActive == false) continue;
            bullet.OnUpdate();
            if (bullet.isActive == false) m_pool.Enqueue(bullet);
        }
    }


    private sealed class Bullet
    {
        public Rigidbody target;
        public Transform transform;
        public bool isActive;
        float m_lifetime;

        public void Activate(float lifetime)
        {
            isActive = true;
            m_lifetime = lifetime;
            target.velocity = Vector3.zero;
            target.gameObject.SetActive(true);
        }

        public void OnUpdate()
        {
            m_lifetime -= Time.deltaTime;
            if (m_lifetime > 0f) return;
            isActive = false;
            target.velocity = Vector3.zero;
            target.gameObject.SetActive(false);
        }
    };
};
