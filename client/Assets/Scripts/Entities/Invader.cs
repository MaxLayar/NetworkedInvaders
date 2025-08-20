using System;
using UnityEngine;
using NetworkedInvaders.Manager;

namespace NetworkedInvaders.Entity
{
    public class Invader : MonoBehaviour
    {
        [SerializeField] private float speed = 1f;
        [SerializeField] private float moveDownDistance = 1f;
        [SerializeField] private float moveInterval = 1f;

        private float moveTimer = 0f;
        private bool moveRight = true;
        private static float lowestPosition = Mathf.Infinity;

        public static event Action<Invader, Collider2D> OnTriggerEnter2DEvent;

        private void Update()
        {
            moveTimer += Time.deltaTime;
            if (moveTimer >= moveInterval)
            {
                Move();
                moveTimer = 0f;
            }
        }

        private void Move()
        {
            Vector3 movement = moveRight ? Vector3.right : Vector3.left;
            transform.Translate(movement * speed);

            if (transform.position.y < lowestPosition)
            {
                lowestPosition = transform.position.y;
            }
        }

        internal void ChangeDirection(bool newDirection)
        {
            moveRight = newDirection;
            transform.Translate(Vector3.down * moveDownDistance);
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            OnTriggerEnter2DEvent?.Invoke(this, col);
        }

        private void OnDestroy()
        {
            GameManager.Instance.RemoveInvader(this);
        }
    }
}