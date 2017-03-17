using UnityEngine;

namespace BuffPanel
{
	public interface Callback
	{
		void success(WWW www);
		void error(WWW www);
	}
}
