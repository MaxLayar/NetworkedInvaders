using System;
using NetworkedInvaders.Network;
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
			NetworkRegistry.OnLoginResult += OnLoginResult;
		}

		private void OnDestroy()
		{
			GameManager.OnGameOver -= OnGameOver;
			NetworkRegistry.OnLoginResult -= OnLoginResult;
		}

		public void OnSubmitPlayerName(string playerName)
		{
			if (isSubmitting) return;
			
			isSubmitting = true;

			NetworkRegistry.Login(playerName);
		}

		private void OnLoginResult(bool success, string message)
		{
			isSubmitting = false;
			if (success)
				playerNameInput.transform.parent.gameObject.SetActive(false);
			else
				playerInputResponseText.text = message;
		}

		private void OnGameOver()
		{
			Time.timeScale = 0f;
			gameOverPanel.SetActive(true);
		}
	}
}
