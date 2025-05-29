using GPT_Minus;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
namespace GPT_Minus_App
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private extern static void SendMessge(System.IntPtr hwnd, int wmsg, int wparam, int lparam);
        private string apiKey = "";
        private string selectedModel = "openai/gpt-4.1-nano";

        public Form1()
        {
            InitializeComponent();
        }

        private void btnSaveApiKey_Click(object sender, EventArgs e)
        {
            apiKey = txtApiKey.Text.Trim();
            MessageBox.Show("API Key saved.");
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            selectedModel = cmbModel.SelectedItem?.ToString() ?? selectedModel;

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                MessageBox.Show("Please enter your API key. You can get one at https://openrouter.ai/settings/keys");
                return;
            }

            string input = txtUserInput.Text;

            
            await webViewResponse.EnsureCoreWebView2Async();

            
            string loadingHtml = $"<html><body style='background-color:#1e1e1e; color:gray; font-family:Segoe UI; padding:10px;'>" +
                                 $"<p>⏳ Loading response from <b>{selectedModel}</b>...</p></body></html>";
            webViewResponse.NavigateToString(loadingHtml);

            string response = await GetChatGPTResponse(input);

            string markdownEscaped = JsonSerializer.Serialize(response);

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
    <div id='content'><em>Rendering...</em></div>
    <script>
        const markdown = {markdownEscaped};

        // Sicherstellen, dass JS im DOM läuft
        window.addEventListener('DOMContentLoaded', function () {{
            const html = marked.parse(markdown);
            document.getElementById('content').innerHTML = html;
        }});
    </script>
</body>
</html>
            ";



            webViewResponse.NavigateToString(html);
            txtUserInput.Clear();

        }


        public async Task<string> GetChatGPTResponse(string userInput)
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
                messages = new[]
                {
                    new { role = "user", content = userInput }
                }
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

                    string message = json.RootElement
                        .GetProperty("choices")[0]
                        .GetProperty("message")
                        .GetProperty("content")
                        .GetString();

                    return message;
                }
            }
            catch (Exception ex)
            {
                return "Error processing the response: " + ex.Message;
            }
        }

        private void Form1_Load(object sender, EventArgs e) { }

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
    }
}

//thanks to:
//my teacher
//my shit classmate
//pythagoras
//chatgpt
//stackoverflow
//google.com
//ich, du und der ander