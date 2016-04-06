using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Tributit
{
    public class Tributit
    {
        private static string _serviceHostname = "trbt.it";
        private static string _servicePath = "/api/installation";

        private static Dictionary<string, string> _httpHeaders = new Dictionary<string, string> {
            { "Content-type", "application/json" }
        };

        public static void Track(string gameToken, string playerToken, Callback callback = null)
        {
            string url = "http://" + _serviceHostname + _servicePath;

            string httpBody = Json.Serialize(new Dictionary<string, object> {
                { "game_token", gameToken },
                { "player_token", playerToken }
            });
            byte[] httpBodyBytes = Encoding.UTF8.GetBytes(httpBody);

            GameObject gameObject = new GameObject("Tributit Sender Coroutine");
            Object.DontDestroyOnLoad(gameObject);
            MonoBehaviour coroutineObject = gameObject.AddComponent<MonoBehaviour>();
            coroutineObject.StartCoroutine(Send(url, httpBodyBytes, callback, gameObject));
        }

        private static IEnumerator Send(string url, byte[] httpBodyBytes, Callback callback, GameObject gameObject)
        {
            WWW www = new WWW(url, httpBodyBytes, _httpHeaders);

            yield return www;

            Object.Destroy(gameObject);

            if (callback != null) {
                if (www.error == null) {
                    callback.success(www);
                } else {
                    callback.error(www);
                }
            }
            
        }
    }
}
