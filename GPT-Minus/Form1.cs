using GPT_Minus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace GPT_Minus_App
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private extern static void SendMessge(IntPtr hwnd, int wmsg, int wparam, int lparam);

        private string apiKey = "";
        private string selectedModel = "deepseek/deepseek-r1-0528-qwen3-8b:free";
        private Task coreWebView2InitializationTask;

        private List<ChatMessage> messages = new List<ChatMessage>();
        private string currentLogFilePath = "";

        public Form1()
        {
            InitializeComponent();
            _ = EnsureWebViewInitializedAsync();
            InitializeChatLogFile();
        }

        private void InitializeChatLogFile()
        {
            string chatlogDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "chatlogs");
            if (!Directory.Exists(chatlogDir))
            {
                Directory.CreateDirectory(chatlogDir);
            }

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            currentLogFilePath = Path.Combine(chatlogDir, timestamp + "_chatlog.json");
        }

        /* this is coming later as it doesnt work as intented refer to line 212 for info
        private string BuildChatHtmlWithLoading(string userInput)
        {
            var historyBuilder = new StringBuilder();
            for (int i = 0; i < messages.Count; i++)
            {
                var msg = messages[i];
                string sender = msg.role == "user" ? "You" : "AI";
                string mdEscaped = JsonSerializer.Serialize($"**{sender}:**\n\n{msg.content}");
                historyBuilder.AppendLine($"chatHistory.push({mdEscaped});");
            }

            string inputEscaped = JsonSerializer.Serialize($"**You:**\n\n{userInput}");

            string html = $@"
<!DOCTYPE html>
<html>
<head>
<meta charset='utf-8'>
<script src='https://cdn.jsdelivr.net/npm/marked/marked.min.js'></script>
<style>
    body {{
        background-color: #1e1e1e;
        color: #ffffff;
        font-family: 'Segoe UI', sans-serif;
        padding: 20px;
    }}
    pre {{
        background-color: #2d2d2d;
        padding: 10px;
        border-radius: 6px;
        overflow-x: auto;
    }}
    code {{
        font-family: Consolas, monospace;
    }}
    .loading {{
        font-style: italic;
        color: #aaa;
        animation: pulse 1.2s infinite;
        margin-top: 12px;
    }}
    @keyframes pulse {{
        0% {{ opacity: 0.2; }}
        50% {{ opacity: 1; }}
        100% {{ opacity: 0.2; }}
    }}
</style>
</head>
<body>
<div id='chat'></div>
<script>
    const chatHistory = [];
    {historyBuilder}

    chatHistory.push({inputEscaped});

    const chatContainer = document.getElementById('chat');
    chatHistory.forEach(md => {{
        const div = document.createElement('div');
        div.innerHTML = marked.parse(md);
        chatContainer.appendChild(div);
        const hr = document.createElement('hr');
        chatContainer.appendChild(hr);
    }});

    // Show loading animation
    const loadingDiv = document.createElement('div');
    loadingDiv.className = 'loading';
    loadingDiv.textContent = 'AI is thinking...';
    chatContainer.appendChild(loadingDiv);
    window.scrollTo(0, document.body.scrollHeight);
</script>
</body>
</html>";
            return html;
        }

 */
        private async Task EnsureWebViewInitializedAsync()
        {
            if (webViewResponse.CoreWebView2 == null)
            {
                if (coreWebView2InitializationTask == null)
                {
                    coreWebView2InitializationTask = webViewResponse.EnsureCoreWebView2Async();
                }
                await coreWebView2InitializationTask;
                ShowInitialPlaceholder();
            }
        }

        private void ShowInitialPlaceholder()
        {
            string placeholderHtml = @"
    <html style='background-color:#1e1e1e;'>
    <head>
        <style>
            body {
                margin: 0;
                background-color: #1e1e1e;
                color: gray;
                font-family: 'Segoe UI', sans-serif;
                display: flex;
                justify-content: center;
                align-items: center;
                height: 100vh;
                user-select: none;
            }
        </style>
    </head>
    <body>
        <h1>GPT-Minus</h1>
    </body>
    </html>";

            webViewResponse.NavigateToString(placeholderHtml);
        }


        private void btnSaveApiKey_Click(object sender, EventArgs e)
        {
            apiKey = txtApiKey.Text.Trim();
            GPT_Minus.Properties.Settings.Default.ApiKey = apiKey;
            GPT_Minus.Properties.Settings.Default.Save();
            MessageBox.Show("API Key saved.");
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            SetControlsEnabled(false); // Disable controls during processing

            await EnsureWebViewInitializedAsync();

            string newSelectedModel = cmbModel.SelectedItem?.ToString();
            if (!string.IsNullOrWhiteSpace(newSelectedModel) && newSelectedModel != selectedModel)
            {
                selectedModel = newSelectedModel;
                messages.Clear();
                InitializeChatLogFile();
            }

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                MessageBox.Show("Please enter your API key. You can get one at https://openrouter.ai/settings/keys");
                SetControlsEnabled(true); // Re-enable controls before exit
                return;
            }

            string input = txtUserInput.Text.Trim();
            if (string.IsNullOrEmpty(input))
            {
                SetControlsEnabled(true);
                return;
            }

            messages.Add(new ChatMessage { role = "user", content = input });

            webViewResponse.NavigateToString($@"
            <html><body style='background-color:#1e1e1e; color:gray; font-family:Segoe UI; padding:10px;'>
            <p>⏳ Loading response from <b>{selectedModel}</b>...</p></body></html>");
            

            //Build chat UI with temporary "thinking..." message
            //disabled due to not working (it shows user message 2 times? wtf)

            // WARNING: if enabled comment lines 205, 206, 207 or the app will probally glitch out idk

            //var loadingHtml = BuildChatHtmlWithLoading(input);
            //webViewResponse.NavigateToString(loadingHtml);


            string response = await GetChatGPTResponse();
            messages.Add(new ChatMessage { role = "assistant", content = response });
            SaveChatLog();

            // ⬇️ Build full chat as Markdown
            StringBuilder markdownBuilder = new StringBuilder();
            foreach (var msg in messages)
            {
                string roleLabel = msg.role == "user" ? "**You:**" : "**AI:**";
                markdownBuilder.AppendLine($"{roleLabel}\n\n{msg.content}\n\n---\n");
            }

            string fullMarkdown = JsonSerializer.Serialize(markdownBuilder.ToString());

            string html = $@"
<!DOCTYPE html>
<html>
<head>
<meta charset='utf-8'>
<script src='https://cdn.jsdelivr.net/npm/marked/marked.min.js'></script>
<style>
    body {{
        background-color: #1e1e1e;
        color: #ffffff;
        font-family: 'Segoe UI', sans-serif;
        padding: 20px;
    }}
    pre {{
        background-color: #2d2d2d;
        padding: 10px;
        border-radius: 6px;
        overflow-x: auto;
    }}
    code {{
        font-family: Consolas, monospace;
    }}
    table {{
        border-collapse: collapse;
        margin-top: 10px;
    }}
    th, td {{
        border: 1px solid #555;
        padding: 6px 12px;
    }}
    a {{
        color: #4eaaff;
    }}
</style>
</head>
<body>
<div id='content'><em>Rendering full chat history...</em></div>
<script>
    const markdown = {fullMarkdown};
    window.addEventListener('DOMContentLoaded', function () {{
        const html = marked.parse(markdown);
        document.getElementById('content').innerHTML = html;
    }});
</script>
</body>
</html>";

            webViewResponse.NavigateToString(html);
            txtUserInput.Clear();
            SetControlsEnabled(true); // Re-enable controls
        }


        private void SetControlsEnabled(bool enabled)
        {
            btnSend.Enabled = enabled;
            txtUserInput.Enabled = enabled;
            cmbModel.Enabled = enabled;
            btnSaveApiKey.Enabled = enabled;
            btnClearChat.Enabled = enabled; // if you have this button for clearing chat
        }

        public async Task<string> GetChatGPTResponse()
        {
            var url = "https://openrouter.ai/api/v1/chat/completions";
            var client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            client.DefaultRequestHeaders.Add("HTTP-Referer", "http://localhost");
            client.DefaultRequestHeaders.Add("X-Title", "GPT-Minus-App");

            var requestBody = new
            {
                model = selectedModel,
                messages = messages
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync(url, content);
                var responseString = await response.Content.ReadAsStringAsync();
                using (JsonDocument json = JsonDocument.Parse(responseString))
                {
                    if (json.RootElement.TryGetProperty("error", out JsonElement error))
                    {
                        return "API Error: " + error.GetProperty("message").GetString();
                    }
                    return json.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
                }
            }
            catch (Exception ex)
            {
                return "Error processing the response: " + ex.Message;
            }
        }

        private void SaveChatLog()
        {
            File.WriteAllText(currentLogFilePath, JsonSerializer.Serialize(messages, new JsonSerializerOptions { WriteIndented = true }));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            apiKey = GPT_Minus.Properties.Settings.Default.ApiKey;
            txtApiKey.Text = apiKey;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 helpme = new Form2();
            helpme.ShowDialog();
        }

        private void txtResponse_TextChanged(object sender, EventArgs e) { }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessge(this.Handle, 0x112, 0xf012, 0);
        }

        private void label4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label5_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        public class ChatMessage
        {
            public string role { get; set; }
            public string content { get; set; }
        }

        private void btnClearChat_Click(object sender, EventArgs e)
        {
            messages.Clear();
            InitializeChatLogFile(); // Fresh log file
            ShowInitialPlaceholder();
            txtUserInput.Focus(); // UX nicety
        }
    }
}
