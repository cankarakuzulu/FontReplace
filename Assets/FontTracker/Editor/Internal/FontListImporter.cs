using System.Threading.Tasks;
using UnityEngine.Networking;
using Voodoo.Sauce.Common.Font.Network;

namespace Voodoo.Sauce.Font
{
    public static class FontListImporter
    {
        const string GetURL = "https://script.google.com/macros/s/AKfycbzCaANxiEX26i3MCWbyuOmvt0Yq9uwZD49edfHR6J_7gb92ilEfrqBdc-9DoCMfT5U/exec"; 

        [System.Serializable]
        public class WhitelistedFonts 
        {
            public string[] result;
        }

        public async static Task<string[]> ImportGoogleSheetAsync()
        {
            var request = WebRequest.GetAsync(GetURL);
            await request;

#if UNITY_2020_1_OR_NEWER
            if (request.Result.result == UnityWebRequest.Result.ProtocolError || request.Result.result == UnityWebRequest.Result.ConnectionError)
#else
            if (request.Result.isHttpError || request.Result.isNetworkError)
#endif
            {
                UnityEngine.Debug.LogError($"Fail retrieving whitelisted fonts at {GetURL}");
                return new string[0];
            }

            return UnityEngine.JsonUtility.FromJson<WhitelistedFonts>(request.Result.downloadHandler.text)?.result;
        }
    }
}
