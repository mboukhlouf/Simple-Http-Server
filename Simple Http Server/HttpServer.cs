using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Simple_Http_Server
{
    public class HttpServer
    {
        private String[] prefixes;
        private HttpListener listener;

        public delegate void LogHandler(String message);
        public event LogHandler Log;

        public delegate void RequestReceivedHandler(HttpListenerRequest request, HttpListenerResponse response);
        public event RequestReceivedHandler RequestReceived;

        public bool IsListening { get; private set; }

        private HttpServer()
        {
        }

        public HttpServer(String[] prefixes)
        {
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("There should be atleast one prefix");

            this.prefixes = (String[])prefixes.Clone();
            IsListening = false;
        }

        public void Start()
        {
            if (IsListening == false)
            {
                IsListening = true;
                listener = new HttpListener();
                foreach (String prefix in prefixes)
                    listener.Prefixes.Add(prefix);
                listener.Start();
                listener.BeginGetContext(OnContext, null);
                Handlelog("Listening...");
            }
        }

        public void Stop()
        {
            if (IsListening)
            {
                IsListening = false;
                listener.Close();
                Handlelog("Stopped");
            }
        }

        private void OnContext(IAsyncResult ar)
        {
            HttpListenerContext context = null;
            try
            {
                context = listener.EndGetContext(ar);
            }
            catch(ObjectDisposedException)
            {
            }
            catch(HttpListenerException)
            {
            }

            if (IsListening)
                listener.BeginGetContext(OnContext, null);

            if (context != null)
            {
                String logMessage = $"{context.Request.HttpMethod} {context.Request.Url.ToString()}";
                Handlelog(logMessage);
                HandleRequestReceived(context.Request, context.Response);
            }

        }

        private void Handlelog(String message)
        {
            Log?.Invoke(message);
        }

        private void HandleRequestReceived(HttpListenerRequest request, HttpListenerResponse response)
        {
            RequestReceived?.Invoke(request, response);
        }
    }
}
