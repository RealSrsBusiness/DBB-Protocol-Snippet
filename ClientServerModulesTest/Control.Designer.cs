namespace ClientServerModulesTest
{
    partial class Control
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
            this.buttonListen = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.buttonSnd1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.labelState = new System.Windows.Forms.Label();
            this.path1 = new System.Windows.Forms.Label();
            this.buttonClear = new System.Windows.Forms.Button();
            this.path2 = new System.Windows.Forms.Label();
            this.buttonSnd2 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.path3 = new System.Windows.Forms.Label();
            this.buttonSnd3 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.stat1 = new System.Windows.Forms.Label();
            this.stat2 = new System.Windows.Forms.Label();
            this.stat3 = new System.Windows.Forms.Label();
            this.buttonSendAll = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonListen
            // 
            this.buttonListen.Location = new System.Drawing.Point(12, 12);
            this.buttonListen.Name = "buttonListen";
            this.buttonListen.Size = new System.Drawing.Size(120, 23);
            this.buttonListen.TabIndex = 0;
            this.buttonListen.Text = "Listen";
            this.buttonListen.UseVisualStyleBackColor = true;
            this.buttonListen.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 68);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(77, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "Select";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // buttonSnd1
            // 
            this.buttonSnd1.Enabled = false;
            this.buttonSnd1.Location = new System.Drawing.Point(95, 68);
            this.buttonSnd1.Name = "buttonSnd1";
            this.buttonSnd1.Size = new System.Drawing.Size(75, 23);
            this.buttonSnd1.TabIndex = 2;
            this.buttonSnd1.Text = "Send";
            this.buttonSnd1.UseVisualStyleBackColor = true;
            this.buttonSnd1.Click += new System.EventHandler(this.button3_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Status:";
            // 
            // labelState
            // 
            this.labelState.AutoSize = true;
            this.labelState.Location = new System.Drawing.Point(63, 38);
            this.labelState.Name = "labelState";
            this.labelState.Size = new System.Drawing.Size(85, 13);
            this.labelState.TabIndex = 4;
            this.labelState.Text = "<Disconnected>";
            // 
            // path1
            // 
            this.path1.AutoSize = true;
            this.path1.Location = new System.Drawing.Point(12, 94);
            this.path1.Name = "path1";
            this.path1.Size = new System.Drawing.Size(34, 13);
            this.path1.TabIndex = 5;
            this.path1.Text = "path1";
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(138, 12);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(92, 23);
            this.buttonClear.TabIndex = 6;
            this.buttonClear.Text = "clear";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.button4_Click);
            // 
            // path2
            // 
            this.path2.AutoSize = true;
            this.path2.Location = new System.Drawing.Point(12, 143);
            this.path2.Name = "path2";
            this.path2.Size = new System.Drawing.Size(34, 13);
            this.path2.TabIndex = 9;
            this.path2.Text = "path2";
            // 
            // buttonSnd2
            // 
            this.buttonSnd2.Enabled = false;
            this.buttonSnd2.Location = new System.Drawing.Point(95, 117);
            this.buttonSnd2.Name = "buttonSnd2";
            this.buttonSnd2.Size = new System.Drawing.Size(75, 23);
            this.buttonSnd2.TabIndex = 8;
            this.buttonSnd2.Text = "Send";
            this.buttonSnd2.UseVisualStyleBackColor = true;
            this.buttonSnd2.Click += new System.EventHandler(this.buttonSnd2_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(12, 117);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(77, 23);
            this.button6.TabIndex = 7;
            this.button6.Text = "Select";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // path3
            // 
            this.path3.AutoSize = true;
            this.path3.Location = new System.Drawing.Point(12, 191);
            this.path3.Name = "path3";
            this.path3.Size = new System.Drawing.Size(34, 13);
            this.path3.TabIndex = 12;
            this.path3.Text = "path3";
            // 
            // buttonSnd3
            // 
            this.buttonSnd3.Enabled = false;
            this.buttonSnd3.Location = new System.Drawing.Point(95, 165);
            this.buttonSnd3.Name = "buttonSnd3";
            this.buttonSnd3.Size = new System.Drawing.Size(75, 23);
            this.buttonSnd3.TabIndex = 11;
            this.buttonSnd3.Text = "Send";
            this.buttonSnd3.UseVisualStyleBackColor = true;
            this.buttonSnd3.Click += new System.EventHandler(this.buttonSnd3_Click);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(12, 165);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(77, 23);
            this.button8.TabIndex = 10;
            this.button8.Text = "Select";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // stat1
            // 
            this.stat1.AutoSize = true;
            this.stat1.Location = new System.Drawing.Point(176, 73);
            this.stat1.Name = "stat1";
            this.stat1.Size = new System.Drawing.Size(13, 13);
            this.stat1.TabIndex = 13;
            this.stat1.Text = "1";
            // 
            // stat2
            // 
            this.stat2.AutoSize = true;
            this.stat2.Location = new System.Drawing.Point(176, 122);
            this.stat2.Name = "stat2";
            this.stat2.Size = new System.Drawing.Size(13, 13);
            this.stat2.TabIndex = 14;
            this.stat2.Text = "2";
            // 
            // stat3
            // 
            this.stat3.AutoSize = true;
            this.stat3.Location = new System.Drawing.Point(176, 170);
            this.stat3.Name = "stat3";
            this.stat3.Size = new System.Drawing.Size(13, 13);
            this.stat3.TabIndex = 15;
            this.stat3.Text = "3";
            // 
            // buttonSendAll
            // 
            this.buttonSendAll.Location = new System.Drawing.Point(236, 12);
            this.buttonSendAll.Name = "buttonSendAll";
            this.buttonSendAll.Size = new System.Drawing.Size(95, 23);
            this.buttonSendAll.TabIndex = 16;
            this.buttonSendAll.Text = "send all";
            this.buttonSendAll.UseVisualStyleBackColor = true;
            this.buttonSendAll.Click += new System.EventHandler(this.button3_Click_1);
            // 
            // Control
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(343, 215);
            this.Controls.Add(this.buttonSendAll);
            this.Controls.Add(this.stat3);
            this.Controls.Add(this.stat2);
            this.Controls.Add(this.stat1);
            this.Controls.Add(this.path3);
            this.Controls.Add(this.buttonSnd3);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.path2);
            this.Controls.Add(this.buttonSnd2);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.path1);
            this.Controls.Add(this.labelState);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonSnd1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.buttonListen);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Control";
            this.Text = "Control";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonListen;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelState;
        private System.Windows.Forms.Label path1;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.Label path2;
        private System.Windows.Forms.Button buttonSnd2;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Label path3;
        private System.Windows.Forms.Button buttonSnd3;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Label stat1;
        private System.Windows.Forms.Label stat2;
        private System.Windows.Forms.Label stat3;
        private System.Windows.Forms.Button buttonSnd1;
        private System.Windows.Forms.Button buttonSendAll;
    }
}