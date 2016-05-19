using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

public class MenuController : MonoBehaviour
{
    public Text gameTokenInputFieldText;
    public Text playerRegisteredInputFieldText;
	public Text playerUserIdInputFieldText;

    public Text responseText;

    private class DebugCallback : Tributit.Callback
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
		Tributit.Tributit.Track(this.gameTokenInputFieldText.text, new Dictionary<string, object> {
			{ "registered", this.playerRegisteredInputFieldText.text }, { "user_id", this.playerUserIdInputFieldText.text } },
			new DebugCallback(this.responseText));
    }
}