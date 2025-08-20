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
		}

		public void OnSubmitPlayerName(string playerName)
		{
			if (isSubmitting) return;
			
			isSubmitting = true;

			NetworkManager.Instance.SubmitPlayerName(playerName, (response) =>
			{
				playerInputResponseText.text = "TODO: " + response;
				isSubmitting = false;
			});
		}
		private void OnGameOver()
		{
			Time.timeScale = 0f;
			gameOverPanel.SetActive(true);
		}
	}
}
