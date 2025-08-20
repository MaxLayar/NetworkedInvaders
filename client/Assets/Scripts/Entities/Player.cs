using UnityEngine;

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

        private float lastBulletTime;

        private void Update()
        {
            MovePlayer();
            Shoot();
        }

        private void MovePlayer()
        {
            float moveInput = Input.GetAxis("Horizontal");
            transform.Translate(Vector3.right * moveInput * speed * Time.deltaTime);
        }

        private void Shoot()
        {
            if (Input.GetKeyDown(KeyCode.Space) && Time.time > lastBulletTime + bulletCooldown)
            {
                GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
                Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
                bulletRigidbody.velocity = Vector2.up * bulletSpeed;
                lastBulletTime = Time.time;
            }
        }
    }
}
