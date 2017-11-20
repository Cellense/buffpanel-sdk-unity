﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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
			false,
			new Dictionary<string, string>() {
				{ "dlc_651440", "1506599038"},
        { "dlc_647560", "1511176765"},
        { "dlc_707480", 1511076765.ToString()}
			},
			new DebugCallback(this.responseText)
		);
	}
}