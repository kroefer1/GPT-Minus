
using GPT_Minus;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
//merci chatgpt
//made in switzerland (land mit grosse stein mit schnee drauf)
namespace GPT_Minus_App
{
    public partial class Form1 : Form
    {
        private string apiKey = "";
        private string selectedModel = "google/gemini-2.5-flash-preview-05-20";

        public Form1()
        {
            InitializeComponent();
        }

        private void btnSaveApiKey_Click(object sender, EventArgs e)
        {
            apiKey = txtApiKey.Text.Trim();
            MessageBox.Show("API Key saved."); // es tut der api-key nicht mal speichern aber egallll
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            selectedModel = cmbModel.SelectedItem.ToString();
            if (string.IsNullOrWhiteSpace(apiKey))
            {                   //machet mal gpt plus für alle gratis :herz:
                MessageBox.Show("Please enter your API key. You can get one at https://openrouter.ai/settings/keys");
                return;
            }

            txtResponse.Text = $"Loading response from {selectedModel}";

            string input = txtUserInput.Text;
            string response = await GetChatGPTResponse(input);
            txtResponse.Text = response;
        }

        public async Task<string> GetChatGPTResponse(string userInput)
        {
            var url = "https://openrouter.ai/api/v1/chat/completions";
            var client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            client.DefaultRequestHeaders.Add("HTTP-Referer", "http://localhost"); //die scheiss leute wollen ein http addresse, wieso? weiss ich nicht.
            client.DefaultRequestHeaders.Add("X-Title", "GPT-Minus-App");
            //raste aus wieso macht diser scheisse nit gud
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



        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 helpme = new Form2(); // offnen dise helping scheise
            helpme.ShowDialog();
        }

        private void txtResponse_TextChanged(object sender, EventArgs e)
        {

        }
    }
}


// Danke an alle diese leute

// mein lehrer
// mein mitschüler
// Pythagoras
// chatgpt