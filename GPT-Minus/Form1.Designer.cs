
namespace GPT_Minus_App
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox txtApiKey;
        private System.Windows.Forms.Button btnSaveApiKey;
        private System.Windows.Forms.TextBox txtUserInput;
        private System.Windows.Forms.Button btnSend;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.txtApiKey = new System.Windows.Forms.TextBox();
            this.btnSaveApiKey = new System.Windows.Forms.Button();
            this.txtUserInput = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.cmbModel = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtResponse = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // txtApiKey
            // 
            this.txtApiKey.Location = new System.Drawing.Point(502, 39);
            this.txtApiKey.Name = "txtApiKey";
            this.txtApiKey.PasswordChar = '*';
            this.txtApiKey.Size = new System.Drawing.Size(194, 22);
            this.txtApiKey.TabIndex = 0;
            // 
            // btnSaveApiKey
            // 
            this.btnSaveApiKey.Location = new System.Drawing.Point(702, 38);
            this.btnSaveApiKey.Name = "btnSaveApiKey";
            this.btnSaveApiKey.Size = new System.Drawing.Size(100, 23);
            this.btnSaveApiKey.TabIndex = 1;
            this.btnSaveApiKey.Text = "Save API Key";
            this.btnSaveApiKey.UseVisualStyleBackColor = true;
            this.btnSaveApiKey.Click += new System.EventHandler(this.btnSaveApiKey_Click);
            // 
            // txtUserInput
            // 
            this.txtUserInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUserInput.Location = new System.Drawing.Point(502, 79);
            this.txtUserInput.Multiline = true;
            this.txtUserInput.Name = "txtUserInput";
            this.txtUserInput.Size = new System.Drawing.Size(300, 204);
            this.txtUserInput.TabIndex = 2;
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(502, 289);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(300, 28);
            this.btnSend.TabIndex = 3;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // cmbModel
            // 
            this.cmbModel.FormattingEnabled = true;
            this.cmbModel.Items.AddRange(new object[] {
            "google/gemini-2.5-flash-preview-05-20",
            "deepseek/deepseek-chat-v3-0324:free",
            "openai/gpt-4.1-nano"});
            this.cmbModel.Location = new System.Drawing.Point(502, 12);
            this.cmbModel.Name = "cmbModel";
            this.cmbModel.Size = new System.Drawing.Size(194, 21);
            this.cmbModel.TabIndex = 5;
            this.cmbModel.Text = "select a model from the menu ->";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(702, 10);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Help";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 7.25F, System.Drawing.FontStyle.Italic);
            this.label1.ForeColor = System.Drawing.Color.Gray;
            this.label1.Location = new System.Drawing.Point(502, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(275, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "make an api key for free at https://openrouter.ai/settings/keys";
            // 
            // txtResponse
            // 
            this.txtResponse.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtResponse.Location = new System.Drawing.Point(12, 10);
            this.txtResponse.Name = "txtResponse";
            this.txtResponse.ReadOnly = true;
            this.txtResponse.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.txtResponse.Size = new System.Drawing.Size(484, 307);
            this.txtResponse.TabIndex = 8;
            this.txtResponse.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(814, 329);
            this.Controls.Add(this.txtResponse);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cmbModel);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.txtUserInput);
            this.Controls.Add(this.btnSaveApiKey);
            this.Controls.Add(this.txtApiKey);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "GPT-Minus";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.ComboBox cmbModel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox txtResponse;
    }
}
