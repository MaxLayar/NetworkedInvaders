using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NetworkedInvaders.Entity;

namespace NetworkedInvaders.Manager
{
	public class GameManager : Singleton<GameManager>
	{
		public GameObject player;
		
		[Header("Invader Settings")]
		[SerializeField] private GameObject invaderPrefab;
		[SerializeField] private Transform invaderSpawnPoint;
		[SerializeField] private int numberOfInvaders = 20;
		
		[Header("Grid Settings")]
		[SerializeField] private  int invadersColumns = 10;
		[SerializeField] private float spacingX = 1.5f;
		[SerializeField] private float spacingY = 1.5f;
		
		[Header("Game Screens")]
		[SerializeField] private GameObject gameOverUI;

		private bool moveRight = true;
		private bool loggedIn = false;

		private List<Invader> invaders;
		
		private enum EdgeSide
		{
			Left,
			Right
		}

		private void Start()
		{
			invaders = new List<Invader>();
			Invader.OnTriggerEnter2DEvent += HandleInvaderCollision;
			
			loggedIn = false;
			NetworkManager.Instance.Login(OnLoginSuccess);
		}

		private void OnDestroy()
		{
			Invader.OnTriggerEnter2DEvent -= HandleInvaderCollision;
		}

		#region GameState
		private void OnLoginSuccess(string loginResponse)
		{
			loggedIn = true;
			SpawnInvaders();
			gameOverUI.SetActive(false);
		}
				
		private void EndRound()
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
		
		private void GameOver()
		{
			gameOverUI.SetActive(true);
		}
		#endregion GameState

		#region Invaders
		private void SpawnInvaders()
		{
			for (int i = 0; i < numberOfInvaders; i++)
			{
				int col = i % invadersColumns;
				int row = i / invadersColumns;
				Vector3 invaderPosition = invaderSpawnPoint.position + new Vector3(col * spacingX, -row * spacingY, 0);
				
				GameObject go = Instantiate(invaderPrefab, invaderPosition, Quaternion.identity, invaderSpawnPoint);
				invaders.Add(go.GetComponent<Invader>());
			}
		}
		
		private void HandleInvaderCollision(Invader invader, Collider2D collider)
		{
			switch (collider.tag)
			{
				case "ScreenEdgeLeft":
					InvaderHitEdge(EdgeSide.Left);
					break;
				case "ScreenEdgeRight":
					InvaderHitEdge(EdgeSide.Right);
					break;
				case "ScreenBottom":
					GameOver();
					break;
				case "Bullet":
					Destroy(collider.gameObject);
					Destroy(invader?.gameObject);
					break;
			}
		}
		
		private void InvaderHitEdge(EdgeSide side)
		{
			// Right = 1, Left = -1
			int dirSign = moveRight ? 1 : -1;
			int sideSign = side == EdgeSide.Right ? 1 : -1;

			// Only change direction if invaders trigger the side they're moving toward.
			if (dirSign != sideSign) return;
			
			moveRight = !moveRight;
			invaders.ForEach(invader => invader?.ChangeDirection(moveRight));
		}

		internal void RemoveInvader(Invader invader)
		{
			invaders.Remove(invader);
			
			if (invaders.Count == 0)
			{
				EndRound();
			}
		}
		#endregion Invaders
	}
}
