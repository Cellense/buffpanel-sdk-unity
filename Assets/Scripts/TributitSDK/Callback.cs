using UnityEngine;

namespace Tributit
{
    public interface Callback
    {
        void success(WWW www);
        void error(WWW www);
    }
}
