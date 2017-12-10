namespace tilt_reader
{
    partial class FormTiltReaderMain
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
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.testerTSMDetection = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testerTSMRecognition = new System.Windows.Forms.ToolStripMenuItem();
            this.detectionTesterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recognitionTesterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.detectionTrainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBoxVideo = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cb_bbNormal = new System.Windows.Forms.CheckBox();
            this.cb_TilBB = new System.Windows.Forms.CheckBox();
            this.rbDilated = new System.Windows.Forms.RadioButton();
            this.rbGrassfire = new System.Windows.Forms.RadioButton();
            this.rbEroded = new System.Windows.Forms.RadioButton();
            this.rbCanny = new System.Windows.Forms.RadioButton();
            this.rbNormal = new System.Windows.Forms.RadioButton();
            this.btnStartStop = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.labelFPS = new System.Windows.Forms.Label();
            this.checkBoxLogImg = new System.Windows.Forms.CheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rbAngle = new System.Windows.Forms.RadioButton();
            this.rbText = new System.Windows.Forms.RadioButton();
            this.cbOrientation = new System.Windows.Forms.CheckBox();
            this.timerImageLog = new System.Windows.Forms.Timer(this.components);
            this.cbLogAngle = new System.Windows.Forms.CheckBox();
            this.cbActivate180 = new System.Windows.Forms.CheckBox();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVideo)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testerTSMDetection,
            this.testerTSMRecognition});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(7, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(1169, 25);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // testerTSMDetection
            // 
            this.testerTSMDetection.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.testerTSMDetection.Name = "testerTSMDetection";
            this.testerTSMDetection.Size = new System.Drawing.Size(37, 19);
            this.testerTSMDetection.Text = "File";
            this.testerTSMDetection.Click += new System.EventHandler(this.testerToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            // 
            // testerTSMRecognition
            // 
            this.testerTSMRecognition.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.detectionTesterToolStripMenuItem,
            this.recognitionTesterToolStripMenuItem,
            this.detectionTrainToolStripMenuItem});
            this.testerTSMRecognition.Name = "testerTSMRecognition";
            this.testerTSMRecognition.Size = new System.Drawing.Size(50, 19);
            this.testerTSMRecognition.Text = "Tester";
            // 
            // detectionTesterToolStripMenuItem
            // 
            this.detectionTesterToolStripMenuItem.Name = "detectionTesterToolStripMenuItem";
            this.detectionTesterToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.detectionTesterToolStripMenuItem.Text = "Detection Tester";
            this.detectionTesterToolStripMenuItem.Click += new System.EventHandler(this.detectionTesterToolStripMenuItem_Click);
            // 
            // recognitionTesterToolStripMenuItem
            // 
            this.recognitionTesterToolStripMenuItem.Name = "recognitionTesterToolStripMenuItem";
            this.recognitionTesterToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.recognitionTesterToolStripMenuItem.Text = "Recognition Tester";
            // 
            // detectionTrainToolStripMenuItem
            // 
            this.detectionTrainToolStripMenuItem.Name = "detectionTrainToolStripMenuItem";
            this.detectionTrainToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.detectionTrainToolStripMenuItem.Text = "Detection Train";
            this.detectionTrainToolStripMenuItem.Click += new System.EventHandler(this.detectionTrainToolStripMenuItem_Click);
            // 
            // pictureBoxVideo
            // 
            this.pictureBoxVideo.BackColor = System.Drawing.Color.Black;
            this.pictureBoxVideo.Location = new System.Drawing.Point(41, 49);
            this.pictureBoxVideo.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.pictureBoxVideo.Name = "pictureBoxVideo";
            this.pictureBoxVideo.Size = new System.Drawing.Size(1084, 426);
            this.pictureBoxVideo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxVideo.TabIndex = 1;
            this.pictureBoxVideo.TabStop = false;
            this.pictureBoxVideo.Click += new System.EventHandler(this.pictureBoxVideo_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cb_bbNormal);
            this.panel1.Controls.Add(this.cb_TilBB);
            this.panel1.Controls.Add(this.rbDilated);
            this.panel1.Controls.Add(this.rbGrassfire);
            this.panel1.Controls.Add(this.rbEroded);
            this.panel1.Controls.Add(this.rbCanny);
            this.panel1.Controls.Add(this.rbNormal);
            this.panel1.Location = new System.Drawing.Point(198, 495);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(450, 78);
            this.panel1.TabIndex = 2;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // cb_bbNormal
            // 
            this.cb_bbNormal.AutoSize = true;
            this.cb_bbNormal.Location = new System.Drawing.Point(153, 49);
            this.cb_bbNormal.Name = "cb_bbNormal";
            this.cb_bbNormal.Size = new System.Drawing.Size(143, 20);
            this.cb_bbNormal.TabIndex = 14;
            this.cb_bbNormal.Text = "Normal Bound Box";
            this.cb_bbNormal.UseVisualStyleBackColor = true;
            // 
            // cb_TilBB
            // 
            this.cb_TilBB.AutoSize = true;
            this.cb_TilBB.Location = new System.Drawing.Point(25, 49);
            this.cb_TilBB.Name = "cb_TilBB";
            this.cb_TilBB.Size = new System.Drawing.Size(117, 20);
            this.cb_TilBB.TabIndex = 13;
            this.cb_TilBB.Text = "Tilt Bound Box";
            this.cb_TilBB.UseVisualStyleBackColor = true;
            // 
            // rbDilated
            // 
            this.rbDilated.AutoSize = true;
            this.rbDilated.Location = new System.Drawing.Point(256, 15);
            this.rbDilated.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.rbDilated.Name = "rbDilated";
            this.rbDilated.Size = new System.Drawing.Size(70, 20);
            this.rbDilated.TabIndex = 4;
            this.rbDilated.Text = "Dilated";
            this.rbDilated.UseVisualStyleBackColor = true;
            this.rbDilated.CheckedChanged += new System.EventHandler(this.rbDilated_CheckedChanged);
            // 
            // rbGrassfire
            // 
            this.rbGrassfire.AutoSize = true;
            this.rbGrassfire.Location = new System.Drawing.Point(330, 15);
            this.rbGrassfire.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.rbGrassfire.Name = "rbGrassfire";
            this.rbGrassfire.Size = new System.Drawing.Size(84, 20);
            this.rbGrassfire.TabIndex = 3;
            this.rbGrassfire.Text = "Contours";
            this.rbGrassfire.UseVisualStyleBackColor = true;
            this.rbGrassfire.CheckedChanged += new System.EventHandler(this.rbValue_CheckedChanged);
            // 
            // rbEroded
            // 
            this.rbEroded.AutoSize = true;
            this.rbEroded.Location = new System.Drawing.Point(176, 15);
            this.rbEroded.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.rbEroded.Name = "rbEroded";
            this.rbEroded.Size = new System.Drawing.Size(72, 20);
            this.rbEroded.TabIndex = 2;
            this.rbEroded.Text = "Eroded";
            this.rbEroded.UseVisualStyleBackColor = true;
            this.rbEroded.CheckedChanged += new System.EventHandler(this.rbEroded_CheckedChanged);
            // 
            // rbCanny
            // 
            this.rbCanny.AutoSize = true;
            this.rbCanny.Location = new System.Drawing.Point(97, 15);
            this.rbCanny.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.rbCanny.Name = "rbCanny";
            this.rbCanny.Size = new System.Drawing.Size(67, 20);
            this.rbCanny.TabIndex = 1;
            this.rbCanny.Text = "Canny";
            this.rbCanny.UseVisualStyleBackColor = true;
            this.rbCanny.CheckedChanged += new System.EventHandler(this.rbCanny_CheckedChanged);
            // 
            // rbNormal
            // 
            this.rbNormal.AutoSize = true;
            this.rbNormal.Checked = true;
            this.rbNormal.Location = new System.Drawing.Point(15, 15);
            this.rbNormal.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.rbNormal.Name = "rbNormal";
            this.rbNormal.Size = new System.Drawing.Size(70, 20);
            this.rbNormal.TabIndex = 0;
            this.rbNormal.TabStop = true;
            this.rbNormal.Text = "Normal";
            this.rbNormal.UseVisualStyleBackColor = true;
            this.rbNormal.CheckedChanged += new System.EventHandler(this.rbNormal_CheckedChanged);
            // 
            // btnStartStop
            // 
            this.btnStartStop.Location = new System.Drawing.Point(41, 495);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(151, 43);
            this.btnStartStop.TabIndex = 3;
            this.btnStartStop.Text = "Start";
            this.btnStartStop.UseVisualStyleBackColor = true;
            this.btnStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1067, 539);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 16);
            this.label1.TabIndex = 4;
            this.label1.Text = "FPS:";
            // 
            // labelFPS
            // 
            this.labelFPS.AutoSize = true;
            this.labelFPS.Location = new System.Drawing.Point(1112, 539);
            this.labelFPS.Name = "labelFPS";
            this.labelFPS.Size = new System.Drawing.Size(16, 16);
            this.labelFPS.TabIndex = 5;
            this.labelFPS.Text = "0";
            // 
            // checkBoxLogImg
            // 
            this.checkBoxLogImg.AutoSize = true;
            this.checkBoxLogImg.Location = new System.Drawing.Point(843, 540);
            this.checkBoxLogImg.Name = "checkBoxLogImg";
            this.checkBoxLogImg.Size = new System.Drawing.Size(93, 20);
            this.checkBoxLogImg.TabIndex = 8;
            this.checkBoxLogImg.Text = "Log Image";
            this.checkBoxLogImg.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rbAngle);
            this.panel2.Controls.Add(this.rbText);
            this.panel2.Location = new System.Drawing.Point(654, 495);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(174, 58);
            this.panel2.TabIndex = 9;
            // 
            // rbAngle
            // 
            this.rbAngle.AutoSize = true;
            this.rbAngle.Location = new System.Drawing.Point(89, 15);
            this.rbAngle.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.rbAngle.Name = "rbAngle";
            this.rbAngle.Size = new System.Drawing.Size(62, 20);
            this.rbAngle.TabIndex = 6;
            this.rbAngle.Text = "Angle";
            this.rbAngle.UseVisualStyleBackColor = true;
            // 
            // rbText
            // 
            this.rbText.AutoSize = true;
            this.rbText.Location = new System.Drawing.Point(14, 15);
            this.rbText.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.rbText.Name = "rbText";
            this.rbText.Size = new System.Drawing.Size(51, 20);
            this.rbText.TabIndex = 5;
            this.rbText.Text = "Text";
            this.rbText.UseVisualStyleBackColor = true;
            // 
            // cbOrientation
            // 
            this.cbOrientation.AutoSize = true;
            this.cbOrientation.Location = new System.Drawing.Point(843, 495);
            this.cbOrientation.Name = "cbOrientation";
            this.cbOrientation.Size = new System.Drawing.Size(125, 20);
            this.cbOrientation.TabIndex = 10;
            this.cbOrientation.Text = "Log Orientation";
            this.cbOrientation.UseVisualStyleBackColor = true;
            this.cbOrientation.CheckedChanged += new System.EventHandler(this.cbOrientation_CheckedChanged);
            // 
            // timerImageLog
            // 
            this.timerImageLog.Interval = 5000;
            this.timerImageLog.Tick += new System.EventHandler(this.timerImageLog_Tick);
            // 
            // cbLogAngle
            // 
            this.cbLogAngle.AutoSize = true;
            this.cbLogAngle.Location = new System.Drawing.Point(843, 518);
            this.cbLogAngle.Name = "cbLogAngle";
            this.cbLogAngle.Size = new System.Drawing.Size(97, 20);
            this.cbLogAngle.TabIndex = 11;
            this.cbLogAngle.Text = "Log Angles";
            this.cbLogAngle.UseVisualStyleBackColor = true;
            // 
            // cbActivate180
            // 
            this.cbActivate180.AutoSize = true;
            this.cbActivate180.Location = new System.Drawing.Point(1020, 495);
            this.cbActivate180.Name = "cbActivate180";
            this.cbActivate180.Size = new System.Drawing.Size(105, 20);
            this.cbActivate180.TabIndex = 12;
            this.cbActivate180.Text = "Activate 180";
            this.cbActivate180.UseVisualStyleBackColor = true;
            // 
            // FormTiltReaderMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1169, 578);
            this.Controls.Add(this.cbActivate180);
            this.Controls.Add(this.cbLogAngle);
            this.Controls.Add(this.cbOrientation);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.checkBoxLogImg);
            this.Controls.Add(this.labelFPS);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnStartStop);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pictureBoxVideo);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.Name = "FormTiltReaderMain";
            this.Text = "Tilt Scene OCR";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVideo)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem testerTSMDetection;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testerTSMRecognition;
        private System.Windows.Forms.ToolStripMenuItem detectionTesterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recognitionTesterToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureBoxVideo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rbCanny;
        private System.Windows.Forms.RadioButton rbNormal;
        private System.Windows.Forms.Button btnStartStop;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelFPS;
        private System.Windows.Forms.CheckBox checkBoxLogImg;
        private System.Windows.Forms.ToolStripMenuItem detectionTrainToolStripMenuItem;
        private System.Windows.Forms.RadioButton rbDilated;
        private System.Windows.Forms.RadioButton rbGrassfire;
        private System.Windows.Forms.RadioButton rbEroded;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton rbAngle;
        private System.Windows.Forms.RadioButton rbText;
        private System.Windows.Forms.CheckBox cbOrientation;
        private System.Windows.Forms.Timer timerImageLog;
        private System.Windows.Forms.CheckBox cbLogAngle;
        private System.Windows.Forms.CheckBox cbActivate180;
        private System.Windows.Forms.CheckBox cb_bbNormal;
        private System.Windows.Forms.CheckBox cb_TilBB;
    }
}

