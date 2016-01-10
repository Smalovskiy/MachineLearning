namespace MachineLearning
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.simpleButton1 = new System.Windows.Forms.Button();
            this.perceChart = new AForge.Controls.Chart();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.nnChart = new AForge.Controls.Chart();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.clustersNum = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.iterationsNum = new System.Windows.Forms.NumericUpDown();
            this.thetaStepNum = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.statisticsView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lsChart = new AForge.Controls.Chart();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iterationsNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.thetaStepNum)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // simpleButton1
            // 
            this.simpleButton1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.simpleButton1.Location = new System.Drawing.Point(566, 287);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(200, 29);
            this.simpleButton1.TabIndex = 0;
            this.simpleButton1.Text = "Run";
            this.simpleButton1.UseVisualStyleBackColor = false;
            this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
            // 
            // perceChart
            // 
            this.perceChart.Location = new System.Drawing.Point(9, 19);
            this.perceChart.Name = "perceChart";
            this.perceChart.RangeX = ((AForge.Range)(resources.GetObject("perceChart.RangeX")));
            this.perceChart.RangeY = ((AForge.Range)(resources.GetObject("perceChart.RangeY")));
            this.perceChart.Size = new System.Drawing.Size(523, 349);
            this.perceChart.TabIndex = 2;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.perceChart);
            this.groupBox1.Location = new System.Drawing.Point(6, 16);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(538, 374);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Single Layer Perceptron";
            // 
            // nnChart
            // 
            this.nnChart.Location = new System.Drawing.Point(6, 19);
            this.nnChart.Name = "nnChart";
            this.nnChart.RangeX = ((AForge.Range)(resources.GetObject("nnChart.RangeX")));
            this.nnChart.RangeY = ((AForge.Range)(resources.GetObject("nnChart.RangeY")));
            this.nnChart.Size = new System.Drawing.Size(526, 359);
            this.nnChart.TabIndex = 4;
            this.nnChart.Text = "chart2";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.nnChart);
            this.groupBox2.Location = new System.Drawing.Point(6, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(538, 384);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Neural Network";
            // 
            // clustersNum
            // 
            this.clustersNum.AccessibleRole = System.Windows.Forms.AccessibleRole.TitleBar;
            this.clustersNum.Location = new System.Drawing.Point(94, 74);
            this.clustersNum.Name = "clustersNum";
            this.clustersNum.ReadOnly = true;
            this.clustersNum.Size = new System.Drawing.Size(100, 20);
            this.clustersNum.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Theta Step";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.iterationsNum);
            this.groupBox3.Controls.Add(this.thetaStepNum);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.clustersNum);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Location = new System.Drawing.Point(566, 322);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(200, 103);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "KMeans Clustering";
            // 
            // iterationsNum
            // 
            this.iterationsNum.Location = new System.Drawing.Point(109, 49);
            this.iterationsNum.Name = "iterationsNum";
            this.iterationsNum.Size = new System.Drawing.Size(69, 20);
            this.iterationsNum.TabIndex = 12;
            this.iterationsNum.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // thetaStepNum
            // 
            this.thetaStepNum.Location = new System.Drawing.Point(109, 23);
            this.thetaStepNum.Name = "thetaStepNum";
            this.thetaStepNum.Size = new System.Drawing.Size(69, 20);
            this.thetaStepNum.TabIndex = 11;
            this.thetaStepNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 77);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Clusters";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Iterations";
            // 
            // statisticsView
            // 
            this.statisticsView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
            this.statisticsView.FullRowSelect = true;
            this.statisticsView.GridLines = true;
            this.statisticsView.Location = new System.Drawing.Point(1, 3);
            this.statisticsView.Name = "statisticsView";
            this.statisticsView.Size = new System.Drawing.Size(546, 390);
            this.statisticsView.TabIndex = 10;
            this.statisticsView.UseCompatibleStateImageBehavior = false;
            this.statisticsView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Algorithms";
            this.columnHeader1.Width = 95;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Accuracy";
            this.columnHeader2.Width = 70;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "C1 Accuracy";
            this.columnHeader3.Width = 89;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "C2 Accuracy";
            this.columnHeader4.Width = 98;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "C1 Recall";
            this.columnHeader5.Width = 91;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "C2 Recall";
            this.columnHeader6.Width = 99;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lsChart);
            this.groupBox4.Location = new System.Drawing.Point(6, 6);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(538, 381);
            this.groupBox4.TabIndex = 11;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "LeastSquares";
            // 
            // lsChart
            // 
            this.lsChart.Location = new System.Drawing.Point(6, 19);
            this.lsChart.Name = "lsChart";
            this.lsChart.RangeX = ((AForge.Range)(resources.GetObject("lsChart.RangeX")));
            this.lsChart.RangeY = ((AForge.Range)(resources.GetObject("lsChart.RangeY")));
            this.lsChart.Size = new System.Drawing.Size(526, 356);
            this.lsChart.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(2, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(558, 422);
            this.tabControl1.TabIndex = 12;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(550, 396);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Perceptron";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox4);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(550, 396);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "LeastSquares";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.groupBox2);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(550, 396);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "NeuralNetwork";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.statisticsView);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(550, 396);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Statistics";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(566, 243);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(200, 23);
            this.progressBar1.TabIndex = 13;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(785, 466);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.simpleButton1);
            this.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Pattern Recognition Demo";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iterationsNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.thetaStepNum)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button simpleButton1;
        private AForge.Controls.Chart perceChart;
        private System.Windows.Forms.GroupBox groupBox1;
        private AForge.Controls.Chart nnChart;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox clustersNum;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.NumericUpDown iterationsNum;
        private System.Windows.Forms.NumericUpDown thetaStepNum;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListView statisticsView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.GroupBox groupBox4;
        private AForge.Controls.Chart lsChart;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}

