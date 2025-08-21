using System;
using NetworkedInvaders.Network;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace NetworkedInvaders.Manager
{
	public class UIManager : Singleton<UIManager>
	{
		[SerializeField] private Text scoreText;
		[SerializeField] private Text highscoreText;
		[SerializeField] private Text timerText;
		[SerializeField] private InputField playerNameInput;
		[SerializeField] private Text playerInputResponseText;
		[SerializeField] private GameObject gameOverPanel;

		public static event Action<string> OnLoginSubmission;
		public static event Action OnGameOverSkip;
		
		private bool isSubmitting;
		private bool isScoreActive;
		private bool isGameOver;

		protected override void OnAwake()
		{
			GameManager.OnActivateScoring += OnActivateScoring;
			GameManager.OnGameOver += OnGameOver;
			GameManager.OnStartGameplay += OnStartGameplay;
			NetworkRegistry.OnLoginResult += OnLoginResult;
			NetworkRegistry.OnScoresReceived += OnScoresReceived;
			NetworkRegistry.OnTimerReceived += OnTimerReceived;
		}

		private void OnDestroy()
		{
			GameManager.OnActivateScoring -= OnActivateScoring;
			GameManager.OnGameOver -= OnGameOver;
			GameManager.OnStartGameplay -= OnStartGameplay;
			NetworkRegistry.OnLoginResult -= OnLoginResult;
			NetworkRegistry.OnScoresReceived -= OnScoresReceived;
			NetworkRegistry.OnTimerReceived -= OnTimerReceived;
			
			GameManager.OnScoreChanged -= OnScoreChanged;
		}

		private void OnScoresReceived(string playerName, int currentScore, int highscore)
		{
			if (!highscoreText.isActiveAndEnabled)
				highscoreText.gameObject.SetActive(true);
			highscoreText.text = "Highscore: " + highscore;
		}

		private void OnTimerReceived(int newTimer)
		{
			timerText.text = newTimer.ToString();
		}

		private void OnActivateScoring()
		{
			isScoreActive = true;
			GameManager.OnScoreChanged += OnScoreChanged;
		}

		private void OnScoreChanged(int newScore)
		{
			scoreText.text = newScore.ToString();
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
			if (!success)
				playerInputResponseText.text = message;
		}

		private void OnStartGameplay()
		{
			playerNameInput.transform.parent.gameObject.SetActive(false);
			timerText.gameObject.SetActive(true);
			if (isScoreActive)
				scoreText.gameObject.SetActive(true);
		}

		private void OnGameOver()
		{
			Time.timeScale = 0f;
			gameOverPanel.SetActive(true);
			isGameOver = true;
		}

		public void OnSubmit()
		{
			if (isGameOver)
			{
				OnGameOverSkip?.Invoke();
				isGameOver = false;
			}
		}
	}
}
