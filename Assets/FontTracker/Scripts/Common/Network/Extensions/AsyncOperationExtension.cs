using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace Voodoo.Sauce.Common.Font.Network
{
	public static class AsyncOperationExtension
	{
		public static TaskAwaiter GetAwaiter(this AsyncOperation asyncOp)
		{
			var tcs = new TaskCompletionSource<object>();
			asyncOp.completed += obj => { tcs.SetResult(null); };
			return ((Task)tcs.Task).GetAwaiter();
		}
	}
}