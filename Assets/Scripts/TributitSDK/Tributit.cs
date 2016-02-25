using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Tributit
{
    public class Tributit
    {
        private static string[] _serviceHostnames = {
            "trbt.it",
            "trbtit-cellense.rhcloud.com"
        };
        private static string _servicePath = "/api/installation";

        private static Dictionary<string, string> _httpHeaders = new Dictionary<string, string> {
            { "Content-type", "application/json" }
        };

        public static void Track(string campaignName, string playerToken, Callback callback = null)
        {
            string httpBody = Json.Serialize(new Dictionary<string, object> {
                { "campaign_name", campaignName },
                { "player_token", playerToken }
            });
            byte[] httpBodyBytes = Encoding.UTF8.GetBytes(httpBody);

            Init(0, httpBodyBytes, callback);
        }

        private static void Init(int hostnameId, byte[] httpBodyBytes, Callback callback)
        {
            GameObject gameObject = new GameObject("Tributit Coroutines");
            Object.DontDestroyOnLoad(gameObject);
            MonoBehaviour coroutineObject = gameObject.AddComponent<MonoBehaviour>();
            coroutineObject.StartCoroutine(Send(hostnameId, httpBodyBytes, callback, gameObject));
        }

        private static IEnumerator Send(int hostnameId, byte[] httpBodyBytes, Callback callback, GameObject gameObject)
        {
            string url = "http://" + _serviceHostnames[hostnameId] + _servicePath;

            WWW www = new WWW(url, httpBodyBytes, _httpHeaders);

            yield return www;

            Object.Destroy(gameObject);

            if (www.error == null) {
                callback.success(www);
            } else {
                callback.error(www);

                if (hostnameId < _serviceHostnames.Length) {
                    Init(hostnameId + 1, httpBodyBytes, callback);
                }
            }
        }
    }
}
