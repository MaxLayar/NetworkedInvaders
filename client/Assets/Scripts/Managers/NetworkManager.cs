using System;
using UnityEngine;
using System.Text;
using UnityEngine.Networking;
using System.Net.Http;

namespace NetworkedInvaders.Manager
{
	public class NetworkManager : Singleton<NetworkManager>
	{
		[SerializeField] private string serverBaseUrl = "http://localhost:4444";

		internal void Login(System.Action<string> callback)
		{
			Request("POST","/join", "{\"username\":\"Player\"}", callback);
		}
		
		internal void RetrieveRoundTime(System.Action<string> callback)
		{
			Request("GET","/timeleft", string.Empty, callback);
		}

		private async void Request(string method, string endpoint, string content, System.Action<string> callback)
		{
			try
			{
				string url = serverBaseUrl + endpoint;
				
				UnityWebRequest request = new UnityWebRequest(url, method);
				
				request.uploadHandler = new UploadHandlerRaw(await (new StringContent(content, Encoding.UTF8, "application/json")).ReadAsByteArrayAsync());
				request.uploadHandler.contentType = "application/json";
				request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
				request.SetRequestHeader("Content-Type", "application/json");
				request.timeout = 5;

				request.SendWebRequest().completed += x => callback(request.downloadHandler.text);
			}
			catch (Exception e)
			{
				Debug.LogWarning(e.Message);
			}
		}
	}
}
