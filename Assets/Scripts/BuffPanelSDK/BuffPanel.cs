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
        public static string serviceHostname = "buffpanel.com";
        public static string servicePath = "/api/run";
        public static List<string> redirectURIs = new List<string>();

        private static float initialRetryOffset = 0.25f;
		private static int maxRetryCount = 10;

        private static Dictionary<string, string> httpHeaders = new Dictionary<string, string> {
            { "Content-type", "application/json" }
        };

		public static void Track(string gameToken, string playerToken, Callback callback = null)
		{
			Track(gameToken, new Dictionary<string, object> {
				{ "registered", playerToken }
			}, callback);
		}

		public static void Track(string gameToken, Dictionary<string, object> playerTokens, Callback callback = null)
        {
            string url = "http://" + serviceHostname + servicePath;
            AddAlias(gameToken + ".trbt.it");

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

            string httpBody = CreateHttpBody(gameToken, playerTokens);
            byte[] httpBodyBytes = Encoding.UTF8.GetBytes(httpBody);

            GameObject gameObject = new GameObject("BuffPanel Sender Coroutine");
            Object.DontDestroyOnLoad(gameObject);
            MonoBehaviour coroutineObject = gameObject.AddComponent<BuffPanelMonoBehaviour>();
			coroutineObject.StartCoroutine(Send(url, httpBodyBytes, callback, gameObject));
        }

        public static void AddAlias(string alias)
        {
            if (!redirectURIs.Contains(alias)) {
                redirectURIs.Add(alias);
            }
        }

        private static string CreateHttpBody(string gameToken, Dictionary<string, object> playerTokens)
        {
            Dictionary<string, object> playerTokensDict = new Dictionary<string, object>();
            if (playerTokens.ContainsKey("registered"))
            {
                playerTokensDict.Add("registered", playerTokens["registered"]);
            }
            if (playerTokens.ContainsKey("user_id"))
            {
                playerTokensDict.Add("user_id", playerTokens["user_id"]);
            }
            if (playerTokensDict.Count == 0)
            {
                return null;
            }

            Dictionary<string, string> cookies = new Dictionary<string, string>();
            try {
                cookies = CookieExtractor.ReadCookies(gameToken);
            } catch (System.Exception e) {
                Debug.LogException(e);
            }
            return Json.Serialize(new Dictionary<string, object>
            {
                { "game_token", gameToken },
                { "player_tokens", playerTokensDict },
                { "browser_cookies", cookies }
            });
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
