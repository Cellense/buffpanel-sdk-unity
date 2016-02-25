using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public Text campaignNameInputFieldText;
    public Text playerTokenInputFieldText;

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
            this._responseText.text = "Server Response:\n" + www.text;
        }

        public void error(WWW www)
        {
            this._responseText.color = Color.red;
            this._responseText.text = "The following error occurred:\n" + www.error + "\n"
                + "Server Response:\n" + www.text;            
        }
    }

    public void OnSubmitClick()
    {
        Tributit.Tributit.Track(this.campaignNameInputFieldText.text, this.playerTokenInputFieldText.text,
            new DebugCallback(this.responseText));
    }
}