using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using Mono.Data.Sqlite;
using UnityEngine;

namespace BuffPanel
{
    public class CookieExtractor
    {
        public static Dictionary<string, string> ReadCookies(string gameToken)
        {
            var result = new Dictionary<string, string>();
            var chrome = getChromeCookies(gameToken);
            foreach (var x in chrome)
            {
                result[x.Key] = x.Value;
            }
            var firefox = getFirefoxCookies(gameToken);
            foreach (var x in firefox)
            {
                result[x.Key] = x.Value;
            }
            var edge = getEdgeCookies(gameToken);
            foreach (var x in edge)
            {
                result[x.Key] = x.Value;
            }
            var IEWin7 = getIEWin7Cookies(gameToken);
            foreach (var x in IEWin7)
            {
                result[x.Key] = x.Value;
            }
            var IEWin8 = getIEWin8Cookies(gameToken);
            foreach (var x in IEWin8)
            {
                result[x.Key] = x.Value;
            }
            var IEWin10 = getIEWin10Cookies(gameToken);
            foreach (var x in IEWin10)
            {
                result[x.Key] = x.Value;
            }
            return result;
        }

        private static Dictionary<string, string> getChromeCookies(string gameToken)
        {
            var chromePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome\User Data\";
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (!Directory.Exists(chromePath))
            {
                return result;
            }
            var cookieStores = Directory.GetFiles(chromePath, "Cookies", SearchOption.AllDirectories);
            foreach (var cookieStorePath in cookieStores)
            {
                if (!File.Exists(cookieStorePath))
                {
                    Debug.LogWarning("No Google Chrome cookies.");
                    continue;
                }
                Debug.Log(cookieStorePath);
                string conn = "URI=file:" + cookieStorePath;
                IDbConnection connection = (IDbConnection)new SqliteConnection(conn);
                connection.Open();
                foreach (string x in BuffPanel.redirectURIs) {
                    IDbCommand command = connection.CreateCommand();
                    command.CommandText = "SELECT name, encrypted_value FROM cookies WHERE host_key LIKE '%" + x + "%';";
                    IDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var encryptedData = (byte[])reader[1];
                        var decodedData = System.Security.Cryptography.ProtectedData.Unprotect(encryptedData, null, System.Security.Cryptography.DataProtectionScope.CurrentUser);
                        var plainText = System.Text.Encoding.ASCII.GetString(decodedData); // Looks like ASCII
                        var clickId = reader.GetString(0);
                        result.Add(clickId, plainText);
                    }
                }
                connection.Close();
            }
            return result;
        }

        private static Dictionary<string, string> getFirefoxCookies(string gameToken)
        {
            var firefoxPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Mozilla\Firefox\Profiles\";
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (!Directory.Exists(firefoxPath))
            {
                return result;
            }
            var cookieStores = Directory.GetFiles(firefoxPath, "cookies.sqlite", SearchOption.AllDirectories);
            foreach (var cookieStorePath in cookieStores)
            {
                if (!File.Exists(cookieStorePath))
                {
                    Debug.LogWarning("No Mozzila Firefox cookies.");
                    continue;
                }
                Debug.Log(cookieStorePath);
                string conn = "URI=file:" + cookieStorePath;
                IDbConnection connection = (IDbConnection)new SqliteConnection(conn);
                connection.Open();
                foreach (string x in BuffPanel.redirectURIs) {
                    IDbCommand command = connection.CreateCommand();
                    command.CommandText = "SELECT name, value FROM moz_cookies WHERE host LIKE '%" + x + "%';";
                    IDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var clickId = reader.GetString(0);
                        var campaignId = reader.GetString(1);
                        result.Add(clickId, campaignId);
                    }

                }
                connection.Close();
            }
            return result;
        }

        private static Dictionary<string, string> getEdgeCookies(string gameToken)
        {
            var edgePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Packages\Microsoft.MicrosoftEdge_8wekyb3d8bbwe\";
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (!Directory.Exists(edgePath))
            {
                Debug.LogWarning("No Microsoft Edge cookies.");
                return result;
            }
            var cookieStores = Directory.GetFiles(edgePath, "*.cookie", SearchOption.AllDirectories);
            foreach (var cookieStorePath in cookieStores)
            {
                var text = File.ReadAllLines(cookieStorePath);
                for (int i = 0; i < text.Length; i++)
                {

                    foreach (string x in BuffPanel.redirectURIs) {
                        if (text[i].Contains(x))
                        {
                            Debug.Log(cookieStorePath);
                            result.Add(text[i - 2], text[i - 1]);
                        }
                    }
                }
            }
            return result;
        }

        private static Dictionary<string, string> getIEWin7Cookies(string gameToken)
        {
            var IEPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Windows\Cookies\";
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (!Directory.Exists(IEPath))
            {
                Debug.LogWarning("No Internet Explorer Windows 7 cookies.");
                return result;
            }
            var cookieStores = Directory.GetFiles(IEPath, "*.txt", SearchOption.AllDirectories);
            foreach (var cookieStorePath in cookieStores)
            {
                var text = File.ReadAllLines(cookieStorePath);
                for (int i = 0; i < text.Length; i++)
                {
                    foreach (string x in BuffPanel.redirectURIs) {
                        if (text[i].Contains(x))
                        {
                            Debug.Log(cookieStorePath);
                            result.Add(text[i - 2], text[i - 1]);
                        }
                    }
                }
            }
            return result;
        }

        private static Dictionary<string, string> getIEWin8Cookies(string gameToken)
        {
            var IEPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Microsoft\Windows\INetCookies\";
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (!Directory.Exists(IEPath))
            {
                Debug.LogWarning("No Internet Explorer Windows 8 cookies.");
                return result;
            }
            var cookieStores = Directory.GetFiles(IEPath, "*.txt", SearchOption.AllDirectories);
            foreach (var cookieStorePath in cookieStores)
            {
                var text = File.ReadAllLines(cookieStorePath);
                for (int i = 0; i < text.Length; i++)
                {
                    foreach (string x in BuffPanel.redirectURIs) {
                        if (text[i].Contains(x))
                        {
                            Debug.Log(cookieStorePath);
                            result.Add(text[i - 2], text[i - 1]);
                        }
                    }
                }
            }
            return result;
        }

        private static Dictionary<string, string> getIEWin10Cookies(string gameToken)
        {
            var IEPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Microsoft\Windows\INetCookies\";
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (!Directory.Exists(IEPath))
            {
                Debug.LogWarning("No Internet Explorer Windows 10 cookies.");
                return result;
            }
            var cookieStores = Directory.GetFiles(IEPath, "*.cookie", SearchOption.AllDirectories);
            foreach (var cookieStorePath in cookieStores)
            {
                var text = File.ReadAllLines(cookieStorePath);
                for (int i = 0; i < text.Length; i++)
                {
                    foreach (string x in BuffPanel.redirectURIs) {
                        if (text[i].Contains(x))
                        {
                            Debug.Log(cookieStorePath);
                            result.Add(text[i - 2], text[i - 1]);
                        }
                    }
                }
            }
            return result;
        }
    }
}
