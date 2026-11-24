using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;

namespace AbuDardaApp
{
    public class MainForm : Form
    {
        private WebView2 webView;
        private string appDir;

        [DllImport("shell32.dll", SetLastError = true)]
        private static extern void SetCurrentProcessExplicitAppUserModelID([MarshalAs(UnmanagedType.LPWStr)] string AppID);

        public MainForm()
        {
            // Set AppUserModelID so Windows Taskbar pins and groups it under its own identity and custom logo
            try
            {
                SetCurrentProcessExplicitAppUserModelID("AbuDarda.StrategicOS.v4.2");
            }
            catch { }

            appDir = AppDomain.CurrentDomain.BaseDirectory;

            // Form properties
            this.Text = "Abu Darda v4.2 - Personal Strategy & Development Consultant";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(1300, 850);
            this.MinimumSize = new Size(950, 650);
            this.BackColor = Color.FromArgb(7, 24, 23); // Matches theme #071817

            // Load and apply application icon
            string iconPath = Path.Combine(appDir, "app_icon.ico");
            if (File.Exists(iconPath))
            {
                try
                {
                    this.Icon = new Icon(iconPath);
                }
                catch { }
            }

            // Initialize WebView2
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            webView = new WebView2();
            webView.Dock = DockStyle.Fill;
            webView.DefaultBackgroundColor = Color.FromArgb(7, 24, 23);
            this.Controls.Add(webView);

            try
            {
                string userDataFolder = Path.Combine(appDir, "app_data", "user_profile");
                Directory.CreateDirectory(userDataFolder);

                CoreWebView2EnvironmentOptions options = new CoreWebView2EnvironmentOptions();
                CoreWebView2Environment env = await CoreWebView2Environment.CreateAsync(null, userDataFolder, options);

                await webView.EnsureCoreWebView2Async(env);

                // Configure settings for dedicated app experience
                webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
                webView.CoreWebView2.Settings.IsStatusBarEnabled = false;
                webView.CoreWebView2.Settings.AreDevToolsEnabled = false;
                webView.CoreWebView2.Settings.IsZoomControlEnabled = true;

                // Sync window title if page updates
                webView.CoreWebView2.DocumentTitleChanged += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(webView.CoreWebView2.DocumentTitle))
                    {
                        this.Text = webView.CoreWebView2.DocumentTitle;
                    }
                };

                // Load application
                string htmlPath = Path.Combine(appDir, "app_data", "index.html");
                if (File.Exists(htmlPath))
                {
                    Uri fileUri = new Uri(htmlPath);
                    webView.CoreWebView2.Navigate(fileUri.AbsoluteUri);
                }
                else
                {
                    MessageBox.Show("Cannot locate application file:\n" + htmlPath, "Abu Darda v4.2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("WebView2 initialization error:\n" + ex.Message, "Abu Darda v4.2", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
