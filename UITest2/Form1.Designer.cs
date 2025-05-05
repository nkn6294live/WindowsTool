namespace UITest2
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.testButtonSync = new System.Windows.Forms.Button();
            this.testButtonAsync = new System.Windows.Forms.Button();
            this.resultTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // testButtonSync
            // 
            this.testButtonSync.Location = new System.Drawing.Point(236, 36);
            this.testButtonSync.Name = "testButtonSync";
            this.testButtonSync.Size = new System.Drawing.Size(75, 23);
            this.testButtonSync.TabIndex = 0;
            this.testButtonSync.Text = "TestSync";
            this.testButtonSync.UseVisualStyleBackColor = true;
            this.testButtonSync.Click += new System.EventHandler(this.TestButtonSync_Click);
            // 
            // testButtonAsync
            // 
            this.testButtonAsync.Location = new System.Drawing.Point(420, 36);
            this.testButtonAsync.Name = "testButtonAsync";
            this.testButtonAsync.Size = new System.Drawing.Size(75, 23);
            this.testButtonAsync.TabIndex = 1;
            this.testButtonAsync.Text = "TestAsync";
            this.testButtonAsync.UseVisualStyleBackColor = true;
            this.testButtonAsync.Click += new System.EventHandler(this.TestButtonAsync_Click);
            // 
            // resultTextBox
            // 
            this.resultTextBox.Location = new System.Drawing.Point(90, 127);
            this.resultTextBox.Name = "resultTextBox";
            this.resultTextBox.Size = new System.Drawing.Size(595, 20);
            this.resultTextBox.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.resultTextBox);
            this.Controls.Add(this.testButtonAsync);
            this.Controls.Add(this.testButtonSync);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TestAsync";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button testButtonSync;
        private System.Windows.Forms.Button testButtonAsync;
        private System.Windows.Forms.TextBox resultTextBox;
    }
}

