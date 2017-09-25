using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BuffPanel
{
	class BuffPanelMonoBehaviour : MonoBehaviour
	{
	}

	public class BuffPanel
	{
		public static string serviceHostname = "api.buffpanel.com";
		public static string servicePath = "/run_event/create";

		private static float initialRetryOffset = 0.25f;
		private static int maxRetryCount = 10;

		private static Dictionary<string, string> httpHeaders = new Dictionary<string, string> {
			{ "Content-type", "application/json" }
		};

		public static void Track(string gameToken, string playerToken, bool isRepeated, Callback callback = null)
		{
			string url = "http://" + serviceHostname + servicePath;

			string httpBody = Json.Serialize(new Dictionary<string, object> {
				{ "game_token", gameToken },
				{ "player_token", playerToken },
				{ "is_existing_player", isRepeated },
			});
			byte[] httpBodyBytes = Encoding.UTF8.GetBytes(httpBody);

			GameObject gameObject = new GameObject("BuffPanel Sender Coroutine");
			Object.DontDestroyOnLoad(gameObject);
			MonoBehaviour coroutineObject = gameObject.AddComponent<BuffPanelMonoBehaviour>();
			coroutineObject.StartCoroutine(Send(url, httpBodyBytes, callback, gameObject));
		}

		private static IEnumerator Send(string url, byte[] httpBodyBytes, Callback callback, GameObject gameObject)
		{
			WWW www = null;

			bool isSuccessfull = false;
			float retryOffset = initialRetryOffset;

			for (int retryCount = 0; retryCount < maxRetryCount; ++retryCount) {
				www = new WWW(url, httpBodyBytes, httpHeaders);

				yield return www;

				if (www.error == null) {
					isSuccessfull = true;
					break;
				}

				yield return new WaitForSeconds(retryOffset);
				retryOffset *= 2;
			}

			if (callback != null) {
				if (isSuccessfull) {
					callback.success(www);
				} else {
					callback.error(www);
				}
			}

			Object.Destroy(gameObject);
		}
	}
}
