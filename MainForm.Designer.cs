namespace APRSWXUDPSender
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.csl = new System.Windows.Forms.TextBox();
            this.txprops = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader33 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader34 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.updateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateSendToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.runsingleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runmultiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.conmnu = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.savepresToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.setdefsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.savelogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBox1.Location = new System.Drawing.Point(0, 244);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(951, 59);
            this.textBox1.TabIndex = 0;
            this.textBox1.Text = "NOCALL-WX>APRS,TCPIP*:>Testing software";
            this.textBox1.WordWrap = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.csl);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 303);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(951, 275);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Лог:";
            // 
            // csl
            // 
            this.csl.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.csl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.csl.Location = new System.Drawing.Point(3, 16);
            this.csl.Multiline = true;
            this.csl.Name = "csl";
            this.csl.ReadOnly = true;
            this.csl.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.csl.Size = new System.Drawing.Size(945, 256);
            this.csl.TabIndex = 13;
            this.csl.WordWrap = false;
            // 
            // txprops
            // 
            this.txprops.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txprops.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader33,
            this.columnHeader34,
            this.columnHeader2,
            this.columnHeader3});
            this.txprops.ContextMenuStrip = this.contextMenuStrip1;
            this.txprops.Dock = System.Windows.Forms.DockStyle.Top;
            this.txprops.FullRowSelect = true;
            this.txprops.GridLines = true;
            this.txprops.Location = new System.Drawing.Point(0, 0);
            this.txprops.MultiSelect = false;
            this.txprops.Name = "txprops";
            this.txprops.Size = new System.Drawing.Size(951, 244);
            this.txprops.TabIndex = 15;
            this.txprops.UseCompatibleStateImageBehavior = false;
            this.txprops.View = System.Windows.Forms.View.Details;
            this.txprops.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.txprops_MouseDoubleClick);
            this.txprops.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txprops_KeyPress);
            this.txprops.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txprops_KeyDown);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "ID";
            this.columnHeader1.Width = 32;
            // 
            // columnHeader33
            // 
            this.columnHeader33.Text = "Параметр";
            this.columnHeader33.Width = 260;
            // 
            // columnHeader34
            // 
            this.columnHeader34.Text = "Значение";
            this.columnHeader34.Width = 350;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Последнее";
            this.columnHeader2.Width = 100;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Получено";
            this.columnHeader3.Width = 170;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.updateToolStripMenuItem,
            this.sendToolStripMenuItem,
            this.updateSendToolStripMenuItem,
            this.toolStripMenuItem5,
            this.runsingleToolStripMenuItem,
            this.runmultiToolStripMenuItem,
            this.toolStripMenuItem1,
            this.conmnu,
            this.setdefsToolStripMenuItem,
            this.toolStripMenuItem4,
            this.savelogToolStripMenuItem,
            this.clearToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(319, 242);
            // 
            // updateToolStripMenuItem
            // 
            this.updateToolStripMenuItem.Name = "updateToolStripMenuItem";
            this.updateToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.updateToolStripMenuItem.Size = new System.Drawing.Size(318, 22);
            this.updateToolStripMenuItem.Text = "Сформировать пакет";
            this.updateToolStripMenuItem.Click += new System.EventHandler(this.updateToolStripMenuItem_Click);
            // 
            // sendToolStripMenuItem
            // 
            this.sendToolStripMenuItem.Name = "sendToolStripMenuItem";
            this.sendToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F6;
            this.sendToolStripMenuItem.Size = new System.Drawing.Size(318, 22);
            this.sendToolStripMenuItem.Text = "Отправить пакет ...";
            this.sendToolStripMenuItem.Click += new System.EventHandler(this.sendToolStripMenuItem_Click);
            // 
            // updateSendToolStripMenuItem
            // 
            this.updateSendToolStripMenuItem.Name = "updateSendToolStripMenuItem";
            this.updateSendToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F6)));
            this.updateSendToolStripMenuItem.Size = new System.Drawing.Size(318, 22);
            this.updateSendToolStripMenuItem.Text = "Сформировать и отправить пакет ...";
            this.updateSendToolStripMenuItem.Click += new System.EventHandler(this.updateSendToolStripMenuItem_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(315, 6);
            // 
            // runsingleToolStripMenuItem
            // 
            this.runsingleToolStripMenuItem.Name = "runsingleToolStripMenuItem";
            this.runsingleToolStripMenuItem.Size = new System.Drawing.Size(318, 22);
            this.runsingleToolStripMenuItem.Text = "Запуск в одиночном режиме";
            this.runsingleToolStripMenuItem.Click += new System.EventHandler(this.runsingleToolStripMenuItem_Click);
            // 
            // runmultiToolStripMenuItem
            // 
            this.runmultiToolStripMenuItem.Name = "runmultiToolStripMenuItem";
            this.runmultiToolStripMenuItem.Size = new System.Drawing.Size(318, 22);
            this.runmultiToolStripMenuItem.Text = "Запуск в селекторном режиме";
            this.runmultiToolStripMenuItem.Click += new System.EventHandler(this.runmultiToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(315, 6);
            // 
            // conmnu
            // 
            this.conmnu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.toolStripMenuItem2,
            this.savepresToolStripMenuItem,
            this.toolStripMenuItem3});
            this.conmnu.Name = "conmnu";
            this.conmnu.Size = new System.Drawing.Size(318, 22);
            this.conmnu.Text = "Настройки и список селекторов";
            this.conmnu.DropDownOpening += new System.EventHandler(this.conmnu_DropDownOpening);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(425, 22);
            this.loadToolStripMenuItem.Text = "Загрузить из файла...";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(425, 22);
            this.saveToolStripMenuItem.Text = "Сохранить в файл ...";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(422, 6);
            // 
            // savepresToolStripMenuItem
            // 
            this.savepresToolStripMenuItem.Name = "savepresToolStripMenuItem";
            this.savepresToolStripMenuItem.Size = new System.Drawing.Size(425, 22);
            this.savepresToolStripMenuItem.Text = "Сохранить текущие настройки и добавить в список селекторов...";
            this.savepresToolStripMenuItem.Click += new System.EventHandler(this.savepresToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(422, 6);
            // 
            // setdefsToolStripMenuItem
            // 
            this.setdefsToolStripMenuItem.Name = "setdefsToolStripMenuItem";
            this.setdefsToolStripMenuItem.Size = new System.Drawing.Size(318, 22);
            this.setdefsToolStripMenuItem.Text = "Установить настройки по умолчанию";
            this.setdefsToolStripMenuItem.Click += new System.EventHandler(this.setdefsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(315, 6);
            // 
            // savelogToolStripMenuItem
            // 
            this.savelogToolStripMenuItem.Name = "savelogToolStripMenuItem";
            this.savelogToolStripMenuItem.Size = new System.Drawing.Size(318, 22);
            this.savelogToolStripMenuItem.Text = "Сохранить лог в файл...";
            this.savelogToolStripMenuItem.Click += new System.EventHandler(this.savelogToolStripMenuItem_Click);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(318, 22);
            this.clearToolStripMenuItem.Text = "Очистить лог";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(951, 578);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.txprops);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "APRS WX UDP Sender by milokz@gmail.com";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox csl;
        private System.Windows.Forms.ListView txprops;
        private System.Windows.Forms.ColumnHeader columnHeader33;
        private System.Windows.Forms.ColumnHeader columnHeader34;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem updateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sendToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateSendToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem conmnu;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem savepresToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem savelogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        public System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ToolStripMenuItem setdefsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem runmultiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runsingleToolStripMenuItem;
    }
}

