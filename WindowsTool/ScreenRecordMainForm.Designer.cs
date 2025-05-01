
namespace WindowTool
{
    partial class ScreenRecordMainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScreenRecordMainForm));
            this.label1 = new System.Windows.Forms.Label();
            this.fpsNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.recordButton = new System.Windows.Forms.Button();
            this.captureScreen = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.fpsNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "FPS";
            // 
            // fpsNumericUpDown
            // 
            this.fpsNumericUpDown.Location = new System.Drawing.Point(45, 9);
            this.fpsNumericUpDown.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.fpsNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.fpsNumericUpDown.Name = "fpsNumericUpDown";
            this.fpsNumericUpDown.Size = new System.Drawing.Size(52, 20);
            this.fpsNumericUpDown.TabIndex = 1;
            this.fpsNumericUpDown.Value = new decimal(new int[] {
            24,
            0,
            0,
            0});
            this.fpsNumericUpDown.ValueChanged += new System.EventHandler(this.FPSNumericUpDown_ValueChanged);
            // 
            // recordButton
            // 
            this.recordButton.Location = new System.Drawing.Point(115, 9);
            this.recordButton.Name = "recordButton";
            this.recordButton.Size = new System.Drawing.Size(98, 23);
            this.recordButton.TabIndex = 10;
            this.recordButton.Text = "Quay màn hình";
            this.recordButton.UseVisualStyleBackColor = true;
            this.recordButton.Click += new System.EventHandler(this.RecordButton_Click);
            // 
            // captureScreen
            // 
            this.captureScreen.Location = new System.Drawing.Point(230, 9);
            this.captureScreen.Name = "captureScreen";
            this.captureScreen.Size = new System.Drawing.Size(99, 23);
            this.captureScreen.TabIndex = 11;
            this.captureScreen.Text = "Chụp màn hình";
            this.captureScreen.UseVisualStyleBackColor = true;
            this.captureScreen.Click += new System.EventHandler(this.CaptureScreen_Click);
            // 
            // ScreenRecordMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(341, 40);
            this.Controls.Add(this.captureScreen);
            this.Controls.Add(this.recordButton);
            this.Controls.Add(this.fpsNumericUpDown);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ScreenRecordMainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Simple Screen Record";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ScreenRecordMainForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.fpsNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown fpsNumericUpDown;
        private System.Windows.Forms.Button recordButton;
        private System.Windows.Forms.Button captureScreen;
    }
}

