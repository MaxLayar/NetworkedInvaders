using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NetworkedInvaders.Entity
{
    public class Player : MonoBehaviour
    {
        [Header("Player Control")]
        [SerializeField] private float speed = 5f;
        
        [Header("Bullet Settings")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform bulletSpawnPoint;
        [SerializeField] private float bulletSpeed = 10f;
        [SerializeField] private float bulletCooldown = 0.5f;
        [SerializeField] private float bulletLifeTime = 5f;

        private float lastBulletTime;
        private Vector2 moveVector;

        private void Update()
        {
            if (moveVector != Vector2.zero)
            {
                transform.Translate(Vector3.right * moveVector * speed * Time.deltaTime);
            }
        }

        public void OnMove(InputValue iv)
        {
            moveVector = iv.Get<Vector2>();
        }

        public void OnShoot(InputValue iv)
        {
            if (Time.time <= lastBulletTime + bulletCooldown) 
                return;
            
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().velocity = Vector2.up * bulletSpeed;
            lastBulletTime = Time.time;

            Destroy(bullet, bulletLifeTime);
        }
    }
}
