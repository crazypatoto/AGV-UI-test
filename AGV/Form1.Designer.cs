namespace AGV_UI
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button_Clear = new System.Windows.Forms.Button();
            this.button_Send = new System.Windows.Forms.Button();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.button_sendAll = new System.Windows.Forms.Button();
            this.button_clearAll = new System.Windows.Forms.Button();
            this.listView_AGV = new AGV_UI.ListViewNF();
            this.columnHeader_hostName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader_IPA = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader_state = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.OutsetPartial;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 66.79281F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.20719F));
            this.tableLayoutPanel1.Controls.Add(this.pictureBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1060, 703);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // pictureBox
            // 
            this.pictureBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.Location = new System.Drawing.Point(6, 6);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(695, 691);
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            this.pictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox_Paint);
            this.pictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDown);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button_clearAll);
            this.panel1.Controls.Add(this.button_sendAll);
            this.panel1.Controls.Add(this.listView_AGV);
            this.panel1.Controls.Add(this.button_Clear);
            this.panel1.Controls.Add(this.button_Send);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Enabled = false;
            this.panel1.Location = new System.Drawing.Point(710, 6);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(344, 691);
            this.panel1.TabIndex = 1;
            // 
            // button_Clear
            // 
            this.button_Clear.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.button_Clear.Location = new System.Drawing.Point(177, 359);
            this.button_Clear.Name = "button_Clear";
            this.button_Clear.Size = new System.Drawing.Size(164, 68);
            this.button_Clear.TabIndex = 2;
            this.button_Clear.Text = "Clear Selected";
            this.button_Clear.UseVisualStyleBackColor = true;
            this.button_Clear.Click += new System.EventHandler(this.button_Clear_Click);
            // 
            // button_Send
            // 
            this.button_Send.Font = new System.Drawing.Font("微軟正黑體", 12F);
            this.button_Send.Location = new System.Drawing.Point(3, 359);
            this.button_Send.Name = "button_Send";
            this.button_Send.Size = new System.Drawing.Size(164, 68);
            this.button_Send.TabIndex = 1;
            this.button_Send.Text = "Send Selected";
            this.button_Send.UseVisualStyleBackColor = true;
            this.button_Send.Click += new System.EventHandler(this.button_Send_Click);
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 16;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // button_sendAll
            // 
            this.button_sendAll.Font = new System.Drawing.Font("微軟正黑體", 12F);
            this.button_sendAll.Location = new System.Drawing.Point(3, 433);
            this.button_sendAll.Name = "button_sendAll";
            this.button_sendAll.Size = new System.Drawing.Size(335, 68);
            this.button_sendAll.TabIndex = 5;
            this.button_sendAll.Text = "Send All";
            this.button_sendAll.UseVisualStyleBackColor = true;
            this.button_sendAll.Click += new System.EventHandler(this.button_sendAll_Click);
            // 
            // button_clearAll
            // 
            this.button_clearAll.Font = new System.Drawing.Font("微軟正黑體", 12F);
            this.button_clearAll.Location = new System.Drawing.Point(3, 507);
            this.button_clearAll.Name = "button_clearAll";
            this.button_clearAll.Size = new System.Drawing.Size(335, 68);
            this.button_clearAll.TabIndex = 6;
            this.button_clearAll.Text = "Clear All";
            this.button_clearAll.UseVisualStyleBackColor = true;
            this.button_clearAll.Click += new System.EventHandler(this.button_clearAll_Click);
            // 
            // listView_AGV
            // 
            this.listView_AGV.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader_hostName,
            this.columnHeader_IPA,
            this.columnHeader_state});
            this.listView_AGV.HideSelection = false;
            this.listView_AGV.Location = new System.Drawing.Point(3, 3);
            this.listView_AGV.Name = "listView_AGV";
            this.listView_AGV.Size = new System.Drawing.Size(338, 350);
            this.listView_AGV.TabIndex = 4;
            this.listView_AGV.UseCompatibleStateImageBehavior = false;
            this.listView_AGV.Click += new System.EventHandler(this.listView_AGV_Click);
            // 
            // columnHeader_hostName
            // 
            this.columnHeader_hostName.Text = "Hostname";
            this.columnHeader_hostName.Width = 80;
            // 
            // columnHeader_IPA
            // 
            this.columnHeader_IPA.Text = "IP Address";
            this.columnHeader_IPA.Width = 120;
            // 
            // columnHeader_state
            // 
            this.columnHeader_state.Text = "Current State";
            this.columnHeader_state.Width = 100;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1060, 703);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "AGV Test";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button_Send;
        private System.Windows.Forms.Button button_Clear;
        private System.Windows.Forms.ColumnHeader columnHeader_hostName;
        private System.Windows.Forms.ColumnHeader columnHeader_IPA;
        private System.Windows.Forms.ColumnHeader columnHeader_state;
        private System.Windows.Forms.Timer timer;
        private ListViewNF listView_AGV;
        private System.Windows.Forms.Button button_clearAll;
        private System.Windows.Forms.Button button_sendAll;
    }
}

