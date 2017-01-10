using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public Text gameTokenInputFieldText;
    public Text playerTokenInputFieldText;

    public Text responseText;

    private class DebugCallback : BuffPanel.Callback
    {
        private Text _responseText;

        public DebugCallback(Text responseText)
        {
            this._responseText = responseText;
        }

        public void success(WWW www)
		{
			this._responseText.color = Color.green;
			if (www == null) {
				this._responseText.text = "Success";
			} else {
	            this._responseText.text = "Server Response:\n" + www.text;
			}
        }

        public void error(WWW www)
		{
			this._responseText.color = Color.red;
			if (www == null) {
				this._responseText.text = "An error occured";
			} else {
				this._responseText.text = "The following error occurred:\n" + www.error;
			}
        }
    }

    public void OnSubmitClick()
    {
		BuffPanel.BuffPanel.Track(
			this.gameTokenInputFieldText.text,
			this.playerTokenInputFieldText.text,
			new DebugCallback(this.responseText)
		);
    }

    public void OnCookieClick()
    {
        responseText.color = Color.yellow;
		responseText.text =
			"Chrome cookies:\n" + BuffPanel.Json.Serialize(BuffPanel.CookieExtractor.ReadChromeCookies()) + "\n"
			+ "Firefox cookies:\n" + BuffPanel.Json.Serialize(BuffPanel.CookieExtractor.ReadChromeCookies()) + "\n"
			+ "Internet explorer 10 cookies:\n" + BuffPanel.Json.Serialize(BuffPanel.CookieExtractor.ReadChromeCookies()) + "\n";
	}
}