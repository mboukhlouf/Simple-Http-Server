using System;
using System.IO;
using System.Net;

namespace Simple_Http_Server
{
    class Program
    {
        private static Config config;
        private static HttpServer server;

        static void Main(string[] args)
        {
            if (!LoadConfig())
                return;

            server = new HttpServer(config.Prefixes);

            server.Log += Server_Log;
            server.RequestReceived += Server_RequestReceived;
            server.Start();


            while (true)
            {
                String input = Console.ReadLine();
                if (input == "start")
                    server.Start();

                else if (input == "stop")
                    server.Stop();
            }
        }

        private static String GetAppropriateFile(String localPath)
        {
            localPath = localPath.TrimStart('/');

            String path = Path.Combine(config.HtmlDirectory, localPath);

            if (File.Exists(path))
            {
                return path;
            }
            else if (Directory.Exists(path))
            {
                path = Path.Combine(path, "index.html");
                if (File.Exists(path))
                {
                    return path;
                }
            }
            return null;
        }

        private static void Server_RequestReceived(HttpListenerRequest request, HttpListenerResponse response)
        {
            String path = GetAppropriateFile(request.RawUrl);

            byte[] buffer;
            int statusCode;
            String statusDescription;
            if (path != null)
            {
                buffer = File.ReadAllBytes(path);
                statusCode = 200;
                statusDescription = "OK";
            }
            else
            {
                // Not found
                buffer = File.ReadAllBytes("404.html");
                statusCode = 404;
                statusDescription = "Not Found";
            }

            response.Headers["Server"] = "Simple Http Server ";
            response.ContentLength64 = buffer.Length;
            response.StatusCode = statusCode;
            response.StatusDescription = statusDescription;
            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }

        private static void Server_Log(string message)
        {
            Console.WriteLine(message);
        }

        private static bool LoadConfig()
        {
            try
            {
                config = Config.ParseFromFile("config.json");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Coudln't find config.json file.");
                Config.InitFile("config.json");
                Console.WriteLine("A config.json file was just created, please fill it and run the program again.");
                Console.Read();
                return false;
            }
            return true;
        }
    }
}
