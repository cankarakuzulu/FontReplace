using UnityEngine.Networking;

namespace Voodoo.Sauce.Common.Font.Network
{
    public interface IWebRequestHandler
    {
        void OnSuccess(UnityWebRequest webRequest);

        void OnError(UnityWebRequest webRequest);
    }
}