using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;
using System;
using System.Text.RegularExpressions;

namespace BuffPanel
{
	class BuffPanelMonoBehaviour : MonoBehaviour
	{
	}

	public class BuffPanel
	{
		public static string serviceHostname = "staging.api.buffpanel.com";
		public static string servicePath = "/run_event/create";

		public static string version = "unity_0.0.1";

		private static float initialRetryOffset = 0.25f;
		private static int maxRetryCount = 10;

		private static Dictionary<string, string> httpHeaders = new Dictionary<string, string> {
			{ "Content-type", "application/json" }
		};

		public static void Track(string gameToken, bool isExistingPlayer, Callback callback = null)
		{
            string playerToken = "";
            try
            {
                playerToken = GetPlayerToken(gameToken);
            }
            catch (Exception)
            {
                playerToken = "unknown_player";
            }
            string url = "http://" + serviceHostname + servicePath;

			string httpBody = Json.Serialize(new Dictionary<string, object> {
				{ "game_token", gameToken },
				{ "player_token", playerToken },
				{ "is_existing_player", isExistingPlayer },
				{ "version", version }
			});
			byte[] httpBodyBytes = Encoding.UTF8.GetBytes(httpBody);

			GameObject gameObject = new GameObject("BuffPanel Sender Coroutine");
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
			MonoBehaviour coroutineObject = gameObject.AddComponent<BuffPanelMonoBehaviour>();
			coroutineObject.StartCoroutine(Send(url, httpBodyBytes, callback, gameObject));
		}

		public static void Track(string gameToken, bool isExistingPlayer, Dictionary<string, string> attributes, Callback callback = null)
		{

            string playerToken = "";
            try
            {
                playerToken = GetPlayerToken(gameToken);
            }
            catch (Exception)
            {
                playerToken = "unknown_player";
            }
            

            Debug.Log(playerToken);


            string url = "http://" + serviceHostname + servicePath;

			string httpBody = Json.Serialize(new Dictionary<string, object> {
				{ "game_token", gameToken },
				{ "player_token", playerToken },
				{ "is_existing_player", isExistingPlayer },
				{ "attributes", attributes },
				{ "version", version }
			});
			byte[] httpBodyBytes = Encoding.UTF8.GetBytes(httpBody);

			GameObject gameObject = new GameObject("BuffPanel Sender Coroutine");
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
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

            UnityEngine.Object.Destroy(gameObject);
		}

        private static string GetUuidPersistPath()
        {
            OperatingSystem os = Environment.OSVersion;
            PlatformID platform = os.Platform;
            switch (platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\BuffPanel\";
                case PlatformID.Unix:
                    return Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)) + @"/BuffPanel/";
                case PlatformID.MacOSX:
                    return Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)) + @"/BuffPanel/";
                default:
                    return "";
            }
        }

        private static string ReadSavedUuid(string path)
        {
            if (File.Exists(path))
            {
                string uuid;
                try
                {
                    uuid = System.IO.File.ReadAllText(path);
                }
                catch (UnauthorizedAccessException)
                {
                    return "anonymous";
                }
                catch (Exception)
                {
                    return "";
                }
                if (!IsValidUuid(uuid))
                    return "";

                return uuid;
            }
            return "";             
        }

        private static void SaveUuid(string filePath, string folderPath, string uuid)
        {
            System.IO.Directory.CreateDirectory(folderPath);
            System.IO.File.WriteAllText(filePath, uuid);
        }

        private static string GetPlayerToken(string gameToken)
        {
            string folderPath = GetUuidPersistPath();
            string filePath = folderPath + "uuid_" + gameToken;
            string uuid = ReadSavedUuid(filePath);
            if (string.IsNullOrEmpty(uuid))
            {
                uuid = System.Guid.NewGuid().ToString("D").ToUpper();
                SaveUuid(filePath, folderPath, uuid);
            }
            return uuid;
        }

        private static bool IsValidUuid(string uuid)
        {
            Regex uuidRegex = new Regex(@"^[0-9A-F]{8}-[0-9A-F]{4}-4[0-9A-F]{3}-[89AB][0-9A-F]{3}-[0-9A-F]{12}$");
            return uuidRegex.IsMatch(uuid);
        }
    }
}
