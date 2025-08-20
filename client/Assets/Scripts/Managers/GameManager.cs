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

		[SerializeField] private GameObject gameOverUI;

		private bool moveRight = true;
		private bool loggedIn = false;

		private List<Invader> invaders;

		private void Start()
		{
			invaders = new List<Invader>();
			
			loggedIn = false;
			NetworkManager.Instance.Login(OnLoginSuccess);
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
		
		public void GameOver()
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
		
		public void InvaderHitEdge()
		{
			moveRight = !moveRight;
			foreach (Invader invader in invaders)
			{
				invader?.ChangeDirection(moveRight);
			}
		}

		public void RemoveInvader(Invader invader)
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
