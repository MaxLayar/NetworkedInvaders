using UnityEngine;
using System.Text;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Net.Http;
using NetworkedInvaders.Entity;

namespace NetworkedInvaders.Manager
{
	public class GameController : Singleton<GameController>
	{
		public GameObject player;
		public GameObject invaderPrefab;
		public Transform invaderSpawnPoint;
		public int numberOfInvaders = 20;

		public GameObject gameOverUI;

		private bool moveRight = true;
		private bool loggedIn = false;

		private void Start()
		{
			loggedIn = false;
			NetworkManager.Instance.Login(StartGame);
		}

		private void StartGame(string loginResponse)
		{
			loggedIn = true;
			SpawnInvaders();
			gameOverUI.SetActive(false);
		}

		private void SpawnInvaders()
		{
			for (int i = 0; i < numberOfInvaders; i++)
			{
				Instantiate(invaderPrefab, invaderSpawnPoint.position + new Vector3(i % 10, -i / 10, 0), Quaternion.identity, invaderSpawnPoint);
			}
		}

		public void Update()
		{
			if (!loggedIn)
				return;

			if (GameObject.FindGameObjectsWithTag("Invader").Length == 0)
			{
				EndRound();
			}
		}

		public void InvaderHitEdge()
		{
			moveRight = !moveRight;
			GameObject[] invaders = GameObject.FindGameObjectsWithTag("Invader");
			foreach (GameObject invader in invaders)
			{
				Invader invaderScript = invader.GetComponent<Invader>();
				if (invaderScript != null)
				{
					invaderScript.ChangeDirection(moveRight);
				}
			}
		}

		private void EndRound()
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}

		public void GameOver()
		{
			gameOverUI.SetActive(true);
		}
	}
}
