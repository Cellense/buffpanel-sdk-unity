﻿using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Tributit
{
    class TributitMonoBehaviour : MonoBehaviour
    {
    }

    public class Tributit
    {
		private static float initialRetryOffset = 0.25f;
		private static int maxRetryCount = 10;

        private static string _serviceHostname = "trbt.it";
        private static string _servicePath = "/api/installation";

        private static Dictionary<string, string> _httpHeaders = new Dictionary<string, string> {
            { "Content-type", "application/json" }
        };

		public static void Track(string gameToken, Dictionary<string, object> playerTokens, Callback callback = null)
        {
            string url = "http://" + _serviceHostname + _servicePath;

			Dictionary<string, object> playerTokensDict = new Dictionary<string, object> ();
			if (playerTokens.ContainsKey("registered")) {
				playerTokensDict.Add("registered", playerTokens["registered"]);
			}
			if (playerTokens.ContainsKey("user_id")) {
				playerTokensDict.Add("user_id", playerTokens["user_id"]);
			}
			if (playerTokensDict.Count == 0) {
				callback.error(null);
				return;
			}

            string httpBody = Json.Serialize(new Dictionary<string, object> {
                { "game_token", gameToken },
				{ "player_tokens", playerTokensDict }
            });
            byte[] httpBodyBytes = Encoding.UTF8.GetBytes(httpBody);
            
            GameObject gameObject = new GameObject("Tributit Sender Coroutine");
            Object.DontDestroyOnLoad(gameObject);
            MonoBehaviour coroutineObject = gameObject.AddComponent<TributitMonoBehaviour>();
			coroutineObject.StartCoroutine(Send(url, httpBodyBytes, callback, gameObject, 0, initialRetryOffset));
        }

		private static IEnumerator Send(string url, byte[] httpBodyBytes, Callback callback, GameObject gameObject, int retryCount, float retryOffset)
        {
            WWW www = new WWW(url, httpBodyBytes, _httpHeaders);

            yield return www;
            if (www.error == null) {
				if (callback != null) {
					callback.success(www);
				}
			} else if (retryCount < maxRetryCount) {
				yield return new WaitForSeconds(retryOffset);

				GameObject newGameObject = new GameObject("Tributit Sender Coroutine");
				Object.DontDestroyOnLoad(newGameObject);
				MonoBehaviour coroutineObject = newGameObject.AddComponent<TributitMonoBehaviour>();
				coroutineObject.StartCoroutine(Send(url, httpBodyBytes, callback, newGameObject, retryCount + 1, retryOffset * 2));
			} else {
				if (callback != null) {
					callback.error(www);
				}
            }

			Object.Destroy(gameObject);
        }
    }
}
