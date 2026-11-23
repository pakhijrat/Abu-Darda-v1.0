using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace AbuDardaApp
{
    public class Program
    {
        private static HttpListener listener;
        private static int port;
        private static string appDir;
        private static string htmlPath;

        [STAThread]
        public static void Main(string[] args)
        {
            appDir = AppDomain.CurrentDomain.BaseDirectory;
            htmlPath = Path.Combine(appDir, "app_data", "index.html");

            if (!File.Exists(htmlPath))
            {
                MessageBox.Show("Cannot find application files at:\n" + htmlPath, "Abu Darda v4.2 - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Find an available local port
            port = GetRandomUnusedPort();

            // Start embedded high-speed lightweight local server
            StartLocalServer();

            string appUrl = "http://127.0.0.1:" + port + "/";

            // Launch browser in app mode (Chromium / Edge app mode gives a 100% borderless, native desktop window)
            string edgePath = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe";
            string chromePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";

            string targetBrowser = null;
            if (File.Exists(edgePath)) targetBrowser = edgePath;
            else if (File.Exists(chromePath)) targetBrowser = chromePath;

            string userDataDir = Path.Combine(appDir, "app_data", "user_profile");
            Directory.CreateDirectory(userDataDir);

            if (targetBrowser != null)
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = targetBrowser;
                psi.Arguments = "--app=" + appUrl + " --user-data-dir=\"" + userDataDir + "\" --disable-features=Translate --disable-sync --window-size=1280,820";
                psi.UseShellExecute = false;

                try
                {
                    Process proc = Process.Start(psi);
                    if (proc != null)
                    {
                        proc.WaitForExit();
                        StopLocalServer();
                        return;
                    }
                }
                catch
                {
                    // Fallback to standard forms browser if needed
                }
            }

            // Fallback: Open in default system browser
            Process.Start(appUrl);
            Thread.Sleep(3000);
        }

        private static int GetRandomUnusedPort()
        {
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int p = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return p;
        }

        private static void StartLocalServer()
        {
            try
            {
                listener = new HttpListener();
                listener.Prefixes.Add("http://127.0.0.1:" + port + "/");
                listener.Start();

                Thread serverThread = new Thread(ServerWorker);
                serverThread.IsBackground = true;
                serverThread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Server Init Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private static void ServerWorker()
        {
            while (listener != null && listener.IsListening)
            {
                try
                {
                    HttpListenerContext context = listener.GetContext();
                    ThreadPool.QueueUserWorkItem(state => ProcessRequest(context));
                }
                catch
                {
                    break;
                }
            }
        }

        private static void ProcessRequest(HttpListenerContext context)
        {
            try
            {
                HttpListenerRequest req = context.Request;
                HttpListenerResponse resp = context.Response;

                string rawUrl = req.RawUrl.Split('?')[0].TrimStart('/');
                if (string.IsNullOrEmpty(rawUrl)) rawUrl = "index.html";

                string filePath = Path.Combine(appDir, "app_data", rawUrl);

                if (!File.Exists(filePath))
                {
                    resp.StatusCode = 404;
                    byte[] notFound = Encoding.UTF8.GetBytes("Not Found");
                    resp.OutputStream.Write(notFound, 0, notFound.Length);
                    resp.OutputStream.Close();
                    return;
                }

                string ext = Path.GetExtension(filePath).ToLower();
                switch (ext)
                {
                    case ".html": resp.ContentType = "text/html; charset=utf-8"; break;
                    case ".js": resp.ContentType = "application/javascript"; break;
                    case ".css": resp.ContentType = "text/css"; break;
                    case ".png": resp.ContentType = "image/png"; break;
                    case ".ico": resp.ContentType = "image/x-icon"; break;
                    default: resp.ContentType = "application/octet-stream"; break;
                }

                byte[] data = File.ReadAllBytes(filePath);
                resp.ContentLength64 = data.Length;
                resp.AddHeader("Cache-Control", "no-cache");
                resp.OutputStream.Write(data, 0, data.Length);
                resp.OutputStream.Close();
            }
            catch { }
        }

        private static void StopLocalServer()
        {
            try
            {
                if (listener != null)
                {
                    listener.Stop();
                    listener.Close();
                }
            }
            catch { }
        }
    }
}
