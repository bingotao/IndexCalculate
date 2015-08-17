namespace IndexCalculate
{
    partial class indexCalculate
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.cb_type_complete = new System.Windows.Forms.CheckBox();
            this.cb_type_mixture = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.nudLambda = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dg_timeScaleRanage = new System.Windows.Forms.DataGridView();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.dg_timeLagRanage = new System.Windows.Forms.DataGridView();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btn_chooseOutputPath = new System.Windows.Forms.Button();
            this.btn_openDatabase = new System.Windows.Forms.Button();
            this.lb_outputPath = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lb_databasePath = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_create = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLambda)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dg_timeScaleRanage)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dg_timeLagRanage)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // cb_type_complete
            // 
            this.cb_type_complete.AutoSize = true;
            this.cb_type_complete.Location = new System.Drawing.Point(23, 23);
            this.cb_type_complete.Name = "cb_type_complete";
            this.cb_type_complete.Size = new System.Drawing.Size(72, 16);
            this.cb_type_complete.TabIndex = 0;
            this.cb_type_complete.Text = "complete";
            this.cb_type_complete.UseVisualStyleBackColor = true;
            // 
            // cb_type_mixture
            // 
            this.cb_type_mixture.AutoSize = true;
            this.cb_type_mixture.Location = new System.Drawing.Point(133, 23);
            this.cb_type_mixture.Name = "cb_type_mixture";
            this.cb_type_mixture.Size = new System.Drawing.Size(66, 16);
            this.cb_type_mixture.TabIndex = 1;
            this.cb_type_mixture.Text = "mixture";
            this.cb_type_mixture.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.nudLambda);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.cb_type_mixture);
            this.groupBox1.Controls.Add(this.cb_type_complete);
            this.groupBox1.Location = new System.Drawing.Point(14, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(545, 53);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "type";
            // 
            // nudLambda
            // 
            this.nudLambda.Location = new System.Drawing.Point(308, 21);
            this.nudLambda.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudLambda.Name = "nudLambda";
            this.nudLambda.Size = new System.Drawing.Size(120, 21);
            this.nudLambda.TabIndex = 3;
            this.nudLambda.Value = new decimal(new int[] {
            400,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(254, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "lambda:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dg_timeScaleRanage);
            this.groupBox2.Location = new System.Drawing.Point(14, 74);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(264, 139);
            this.groupBox2.TabIndex = 16;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "time scale";
            // 
            // dg_timeScaleRanage
            // 
            this.dg_timeScaleRanage.BackgroundColor = System.Drawing.Color.White;
            this.dg_timeScaleRanage.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dg_timeScaleRanage.Location = new System.Drawing.Point(16, 22);
            this.dg_timeScaleRanage.Name = "dg_timeScaleRanage";
            this.dg_timeScaleRanage.RowTemplate.Height = 23;
            this.dg_timeScaleRanage.Size = new System.Drawing.Size(229, 107);
            this.dg_timeScaleRanage.TabIndex = 15;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.dg_timeLagRanage);
            this.groupBox3.Location = new System.Drawing.Point(295, 74);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(264, 139);
            this.groupBox3.TabIndex = 17;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "time lag";
            // 
            // dg_timeLagRanage
            // 
            this.dg_timeLagRanage.BackgroundColor = System.Drawing.Color.White;
            this.dg_timeLagRanage.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dg_timeLagRanage.Location = new System.Drawing.Point(18, 22);
            this.dg_timeLagRanage.Name = "dg_timeLagRanage";
            this.dg_timeLagRanage.RowTemplate.Height = 23;
            this.dg_timeLagRanage.Size = new System.Drawing.Size(229, 107);
            this.dg_timeLagRanage.TabIndex = 15;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btn_chooseOutputPath);
            this.groupBox4.Controls.Add(this.btn_openDatabase);
            this.groupBox4.Controls.Add(this.lb_outputPath);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.lb_databasePath);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Location = new System.Drawing.Point(14, 229);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(545, 73);
            this.groupBox4.TabIndex = 16;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "path";
            // 
            // btn_chooseOutputPath
            // 
            this.btn_chooseOutputPath.Location = new System.Drawing.Point(86, 43);
            this.btn_chooseOutputPath.Name = "btn_chooseOutputPath";
            this.btn_chooseOutputPath.Size = new System.Drawing.Size(75, 23);
            this.btn_chooseOutputPath.TabIndex = 1;
            this.btn_chooseOutputPath.Text = "choose";
            this.btn_chooseOutputPath.UseVisualStyleBackColor = true;
            this.btn_chooseOutputPath.Click += new System.EventHandler(this.btn_chooseOutputPath_Click);
            // 
            // btn_openDatabase
            // 
            this.btn_openDatabase.Location = new System.Drawing.Point(86, 14);
            this.btn_openDatabase.Name = "btn_openDatabase";
            this.btn_openDatabase.Size = new System.Drawing.Size(75, 23);
            this.btn_openDatabase.TabIndex = 1;
            this.btn_openDatabase.Text = "open";
            this.btn_openDatabase.UseVisualStyleBackColor = true;
            this.btn_openDatabase.Click += new System.EventHandler(this.btn_openDatabase_Click);
            // 
            // lb_outputPath
            // 
            this.lb_outputPath.AutoSize = true;
            this.lb_outputPath.Location = new System.Drawing.Point(176, 48);
            this.lb_outputPath.Name = "lb_outputPath";
            this.lb_outputPath.Size = new System.Drawing.Size(23, 12);
            this.lb_outputPath.TabIndex = 0;
            this.lb_outputPath.Text = "...";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(33, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "output:";
            // 
            // lb_databasePath
            // 
            this.lb_databasePath.AutoSize = true;
            this.lb_databasePath.Location = new System.Drawing.Point(176, 19);
            this.lb_databasePath.Name = "lb_databasePath";
            this.lb_databasePath.Size = new System.Drawing.Size(23, 12);
            this.lb_databasePath.TabIndex = 0;
            this.lb_databasePath.Text = "...";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "database:";
            // 
            // btn_create
            // 
            this.btn_create.Location = new System.Drawing.Point(484, 309);
            this.btn_create.Name = "btn_create";
            this.btn_create.Size = new System.Drawing.Size(75, 23);
            this.btn_create.TabIndex = 18;
            this.btn_create.Text = "create";
            this.btn_create.UseVisualStyleBackColor = true;
            this.btn_create.Click += new System.EventHandler(this.btn_create_Click);
            // 
            // indexCalculate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(571, 339);
            this.Controls.Add(this.btn_create);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "indexCalculate";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "指数测算";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLambda)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dg_timeScaleRanage)).EndInit();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dg_timeLagRanage)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox cb_type_complete;
        private System.Windows.Forms.CheckBox cb_type_mixture;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dg_timeScaleRanage;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.DataGridView dg_timeLagRanage;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btn_chooseOutputPath;
        private System.Windows.Forms.Button btn_openDatabase;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lb_outputPath;
        private System.Windows.Forms.Label lb_databasePath;
        private System.Windows.Forms.Button btn_create;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nudLambda;
    }
}

