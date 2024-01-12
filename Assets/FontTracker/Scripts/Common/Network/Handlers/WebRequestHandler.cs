using System;
using UnityEngine.Networking;

namespace Voodoo.Sauce.Common.Font.Network
{
	public class WebRequestHandler : AbstractWebRequestHandler
	{
		public WebRequestHandler(Action<UnityWebRequest> onSuccess, Action<UnityWebRequest> onError) : base(onSuccess, onError) { }
	}
}