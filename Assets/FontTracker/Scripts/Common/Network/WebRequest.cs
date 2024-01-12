using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Voodoo.Sauce.Common.Font.Network
{
    public static class WebRequest
    {
        private static int Timeout = 5;
        private static Dictionary<int, IWebRequestHandler> idToResult = new Dictionary<int, IWebRequestHandler>();

        public static Header requestHeader;

        private static CancellationTokenSource _cancellationTokenSource;

        public static bool IsCancellationRequested => _cancellationTokenSource != null && _cancellationTokenSource.IsCancellationRequested;

        public static void CancelAllTasks()
        {
            _cancellationTokenSource.Cancel();
        }

        public static void Get(string url, Action<UnityWebRequest> onSuccess = null, Action<UnityWebRequest> onError = null)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);
            SendAndCache(request, onSuccess, onError);
        }
        
        public static async Task<UnityWebRequest> GetAsync(string url)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);
            await SendAndCacheAsync(request);
            return request;
        }

        public static void Put(string url, string content, Action<UnityWebRequest> onSuccess = null, Action<UnityWebRequest> onError = null)
        {
            UnityWebRequest request = UnityWebRequest.Put(url, content);
            request.uploadHandler.contentType = "application/json";
            
            SendAndCache(request, onSuccess, onError);
        }

        public static async Task<UnityWebRequest> PutAsync(string url, string content)
        {
            UnityWebRequest request = UnityWebRequest.Put(url, content);
            request.uploadHandler.contentType = "application/json";

            await SendAndCacheAsync(request);
            return request;
        }

        public static void Post(string url, string content, Action<UnityWebRequest> onSuccess = null, Action<UnityWebRequest> onError = null)
        {
            UnityWebRequest request = new UnityWebRequest(url, "POST", new DownloadHandlerBuffer(), new UploadHandlerRaw(Encoding.UTF8.GetBytes(content)))
            {
                uploadHandler = {contentType = "application/json"}
            };

            SendAndCache(request, onSuccess, onError);
        }
        
        public static async Task<UnityWebRequest> PostAsync(string url, string content)
        {
            UnityWebRequest request = new UnityWebRequest(url, "POST", new DownloadHandlerBuffer(), new UploadHandlerRaw(Encoding.UTF8.GetBytes(content)))
            {
                uploadHandler = {contentType = "application/json"}
            };

            await SendAndCacheAsync(request);
            return request;
        }

        private static void SendAndCache(UnityWebRequest request, Action<UnityWebRequest> onSuccess = null, Action<UnityWebRequest> onError = null) 
        {
            ApplyHeader(request);

            UnityWebRequestAsyncOperation asyncOperation = request.SendWebRequest();

            if (asyncOperation == null)
            {
                return;
            }

            idToResult.Add(asyncOperation.GetHashCode(), new WebRequestHandler(onSuccess, onError));
            asyncOperation.completed += OnAsyncOperationComplete;
        }

        private static async Task SendAndCacheAsync(UnityWebRequest request) 
        {
            if (_cancellationTokenSource == null || _cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource = new CancellationTokenSource();
            }
            
            ApplyHeader(request);

            UnityWebRequestAsyncOperation asyncOperation = request.SendWebRequest();
            
            if (asyncOperation == null)
            {
                return;
            }

            while (asyncOperation.isDone == false && _cancellationTokenSource.IsCancellationRequested == false)
            {
                await Task.Yield();
            }

            if (_cancellationTokenSource.IsCancellationRequested)
            {
                Debug.Log("asyncOperation got canceled " + request.url);
                _cancellationTokenSource.Token.ThrowIfCancellationRequested();
            }
        }

        private static void ApplyHeader(UnityWebRequest request)
        {
            if (requestHeader?.value == null || requestHeader.value.Length <= 0)
            {
                return;
            }

            for (int i = 0; i < requestHeader.value.Length; i++)
            {
                char character = requestHeader.value[i];
                if (char.IsLetterOrDigit(character) == false && character != ' ' && character != '-')
                {
                    return;
                }
            }

            request.SetRequestHeader(requestHeader.name, requestHeader.value);
            request.timeout = Timeout;
        }

        private static void OnAsyncOperationComplete(AsyncOperation operation)
        {
            UnityWebRequestAsyncOperation webOperation = operation as UnityWebRequestAsyncOperation;
            if (webOperation == null)
            {
                return;
            }

            IWebRequestHandler handler = idToResult.ContainsKey(webOperation.GetHashCode()) ? idToResult[webOperation.GetHashCode()] : null;
            if (handler == null)
            {
                return;
            }
            
            if (string.IsNullOrEmpty(webOperation.webRequest.error))
            {
                handler.OnSuccess(webOperation.webRequest);
            }
            else
            {
                handler.OnError(webOperation.webRequest);
            }

            webOperation.webRequest.Dispose();
            idToResult.Remove(operation.GetHashCode());
        }
        
        public static bool HadErrors(Task<UnityWebRequest> request) => string.IsNullOrEmpty(request.Result.error) == false;
        public static bool HadErrors(UnityWebRequest request) => string.IsNullOrEmpty(request.error) == false;
    }
}