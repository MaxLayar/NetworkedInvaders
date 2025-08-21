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

		public static event Action<string> OnLoginSubmission;
		
		private bool isSubmitting;
		private bool isScoreActive;

		protected override void OnAwake()
		{
			GameManager.OnActivateScoring += OnActivateScoring;
			GameManager.OnGameOver += OnGameOver;
			NetworkRegistry.OnLoginResult += OnLoginResult;
		}

		private void OnDestroy()
		{
			GameManager.OnActivateScoring -= OnActivateScoring;
			GameManager.OnGameOver -= OnGameOver;
			NetworkRegistry.OnLoginResult -= OnLoginResult;
		}

		private void OnActivateScoring()
		{
			isScoreActive = true;
			GameManager.OnScoreChanged += OnScoreChanged;
		}

		private void OnScoreChanged(int newScore)
		{
			score.text = newScore.ToString();
		}

		public void OnSubmitPlayerName(string playerName)
		{
			if (isSubmitting) return;
			
			isSubmitting = true;

			OnLoginSubmission?.Invoke(playerName);
		}

		private void OnLoginResult(bool success, string message)
		{
			isSubmitting = false;
			if (success)
			{
				playerNameInput.transform.parent.gameObject.SetActive(false);
				if (isScoreActive)
					score.gameObject.SetActive(true);
			}
			else
			{
				playerInputResponseText.text = message;
			}
		}

		private void OnGameOver()
		{
			Time.timeScale = 0f;
			gameOverPanel.SetActive(true);
		}
	}
}
