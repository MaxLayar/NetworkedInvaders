using System;
using UnityEngine;
using UnityEngine.UI;

namespace NetworkedInvaders.Manager
{
	public class UIManager : Singleton<UIManager>
	{
		[SerializeField] private Text score;
		[SerializeField] private Text lives;
		[SerializeField] private InputField playerNameInput;
		[SerializeField] private Text playerInputResponseText;
		[SerializeField] private GameObject gameOverPanel;
		
		private bool isSubmitting;

		private void Start()
		{
			GameManager.OnGameOver += OnGameOver;
			NetworkManager.OnLoggedIn += OnLoggedIn;
		}

		private void OnDestroy()
		{
			GameManager.OnGameOver -= OnGameOver;
			NetworkManager.OnLoggedIn -= OnLoggedIn;
		}

		public void OnSubmitPlayerName(string playerName)
		{
			if (isSubmitting) return;
			
			isSubmitting = true;

			NetworkManager.Instance.Login(playerName);
		}

		private void OnLoggedIn(NetworkManager.ServerMessage response)
		{
			isSubmitting = false;
			if (response.IsSuccess())
				playerNameInput.transform.parent.gameObject.SetActive(false);
			else
				playerInputResponseText.text = response.data.ToString();
		}

		private void OnGameOver()
		{
			Time.timeScale = 0f;
			gameOverPanel.SetActive(true);
		}
	}
}
