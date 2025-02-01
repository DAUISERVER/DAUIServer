namespace DAUIServer
{
    partial class FrmMain
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.SnapGrid = new System.Windows.Forms.DataGridView();
            this.color2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SceneGrid = new System.Windows.Forms.DataGridView();
            this.color = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Flash = new System.Windows.Forms.Timer(this.components);
            this.UIWeb = new System.Windows.Forms.WebBrowser();
            this.Connect = new System.Windows.Forms.Timer(this.components);
            this.DauiMenu = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MenuPanel = new System.Windows.Forms.Panel();
            this.InfoMessage = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.button_menu = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.SnapGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SceneGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DauiMenu)).BeginInit();
            this.MenuPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // SnapGrid
            // 
            this.SnapGrid.AllowUserToAddRows = false;
            this.SnapGrid.AllowUserToDeleteRows = false;
            this.SnapGrid.AllowUserToResizeColumns = false;
            this.SnapGrid.AllowUserToResizeRows = false;
            this.SnapGrid.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.SnapGrid.BackgroundColor = System.Drawing.Color.Black;
            this.SnapGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.SnapGrid.ColumnHeadersVisible = false;
            this.SnapGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.color2,
            this.col1});
            this.SnapGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.SnapGrid.EnableHeadersVisualStyles = false;
            this.SnapGrid.GridColor = System.Drawing.Color.Black;
            this.SnapGrid.Location = new System.Drawing.Point(641, 2);
            this.SnapGrid.Margin = new System.Windows.Forms.Padding(0);
            this.SnapGrid.MultiSelect = false;
            this.SnapGrid.Name = "SnapGrid";
            this.SnapGrid.ReadOnly = true;
            this.SnapGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.SnapGrid.RowHeadersVisible = false;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.BlueViolet;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.SnapGrid.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.SnapGrid.RowTemplate.Height = 50;
            this.SnapGrid.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.SnapGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.SnapGrid.ShowCellErrors = false;
            this.SnapGrid.ShowCellToolTips = false;
            this.SnapGrid.ShowEditingIcon = false;
            this.SnapGrid.ShowRowErrors = false;
            this.SnapGrid.Size = new System.Drawing.Size(160, 164);
            this.SnapGrid.StandardTab = true;
            this.SnapGrid.TabIndex = 1;
            this.SnapGrid.SelectionChanged += new System.EventHandler(this.SnapGrid_SelectionChanged);
            // 
            // color2
            // 
            this.color2.HeaderText = "color2";
            this.color2.Name = "color2";
            this.color2.ReadOnly = true;
            this.color2.Width = 5;
            // 
            // col1
            // 
            this.col1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Olive;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.col1.DefaultCellStyle = dataGridViewCellStyle1;
            this.col1.HeaderText = "Snaps";
            this.col1.MinimumWidth = 99;
            this.col1.Name = "col1";
            this.col1.ReadOnly = true;
            this.col1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.col1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // SceneGrid
            // 
            this.SceneGrid.AllowUserToAddRows = false;
            this.SceneGrid.AllowUserToDeleteRows = false;
            this.SceneGrid.AllowUserToResizeColumns = false;
            this.SceneGrid.AllowUserToResizeRows = false;
            this.SceneGrid.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.SceneGrid.BackgroundColor = System.Drawing.Color.Black;
            this.SceneGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.SceneGrid.ColumnHeadersVisible = false;
            this.SceneGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.color,
            this.dataGridViewTextBoxColumn1});
            this.SceneGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.SceneGrid.EnableHeadersVisualStyles = false;
            this.SceneGrid.GridColor = System.Drawing.Color.Black;
            this.SceneGrid.Location = new System.Drawing.Point(641, 246);
            this.SceneGrid.Margin = new System.Windows.Forms.Padding(0);
            this.SceneGrid.MultiSelect = false;
            this.SceneGrid.Name = "SceneGrid";
            this.SceneGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.SceneGrid.RowHeadersVisible = false;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.DarkGray;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.BlueViolet;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.SceneGrid.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.SceneGrid.RowTemplate.Height = 50;
            this.SceneGrid.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.SceneGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.SceneGrid.Size = new System.Drawing.Size(160, 164);
            this.SceneGrid.StandardTab = true;
            this.SceneGrid.TabIndex = 2;
            this.SceneGrid.SelectionChanged += new System.EventHandler(this.SceneGrid_SelectionChanged);
            // 
            // color
            // 
            this.color.HeaderText = "color";
            this.color.Name = "color";
            this.color.Width = 5;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.Olive;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewTextBoxColumn1.HeaderText = "Snaps";
            this.dataGridViewTextBoxColumn1.MinimumWidth = 99;
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Flash
            // 
            this.Flash.Interval = 800;
            this.Flash.Tick += new System.EventHandler(this.slash_Tick);
            // 
            // UIWeb
            // 
            this.UIWeb.AllowWebBrowserDrop = false;
            this.UIWeb.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.UIWeb.IsWebBrowserContextMenuEnabled = false;
            this.UIWeb.Location = new System.Drawing.Point(1, 2);
            this.UIWeb.MinimumSize = new System.Drawing.Size(20, 20);
            this.UIWeb.Name = "UIWeb";
            this.UIWeb.ScrollBarsEnabled = false;
            this.UIWeb.Size = new System.Drawing.Size(637, 447);
            this.UIWeb.TabIndex = 3;
            this.UIWeb.Url = new System.Uri("", System.UriKind.Relative);
            this.UIWeb.WebBrowserShortcutsEnabled = false;
            // 
            // Connect
            // 
            this.Connect.Enabled = true;
            this.Connect.Interval = 1000;
            this.Connect.Tick += new System.EventHandler(this.Connect_Tick);
            // 
            // DauiMenu
            // 
            this.DauiMenu.AllowUserToAddRows = false;
            this.DauiMenu.AllowUserToDeleteRows = false;
            this.DauiMenu.AllowUserToResizeColumns = false;
            this.DauiMenu.AllowUserToResizeRows = false;
            this.DauiMenu.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCellsExceptHeader;
            this.DauiMenu.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            this.DauiMenu.BackgroundColor = System.Drawing.Color.Red;
            this.DauiMenu.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.DauiMenu.ColumnHeadersVisible = false;
            this.DauiMenu.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3});
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DauiMenu.DefaultCellStyle = dataGridViewCellStyle7;
            this.DauiMenu.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.DauiMenu.EnableHeadersVisualStyles = false;
            this.DauiMenu.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.DauiMenu.Location = new System.Drawing.Point(0, -1);
            this.DauiMenu.Margin = new System.Windows.Forms.Padding(0);
            this.DauiMenu.MultiSelect = false;
            this.DauiMenu.Name = "DauiMenu";
            this.DauiMenu.ReadOnly = true;
            this.DauiMenu.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.DauiMenu.RowHeadersVisible = false;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Arial Rounded MT Bold", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DauiMenu.RowsDefaultCellStyle = dataGridViewCellStyle8;
            this.DauiMenu.RowTemplate.DividerHeight = 10;
            this.DauiMenu.RowTemplate.Height = 90;
            this.DauiMenu.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.DauiMenu.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DauiMenu.ShowCellErrors = false;
            this.DauiMenu.ShowCellToolTips = false;
            this.DauiMenu.ShowEditingIcon = false;
            this.DauiMenu.ShowRowErrors = false;
            this.DauiMenu.Size = new System.Drawing.Size(366, 330);
            this.DauiMenu.StandardTab = true;
            this.DauiMenu.TabIndex = 7;
            this.DauiMenu.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DauiMenu_CellContentClick);
            this.DauiMenu.SelectionChanged += new System.EventHandler(this.Menu_SelectionChanged);
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.Red;
            this.dataGridViewTextBoxColumn2.DefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridViewTextBoxColumn2.HeaderText = "Info";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn2.Width = 15;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.Gray;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewTextBoxColumn3.DefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridViewTextBoxColumn3.HeaderText = "Snaps";
            this.dataGridViewTextBoxColumn3.MinimumWidth = 99;
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewTextBoxColumn3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // MenuPanel
            // 
            this.MenuPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.MenuPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MenuPanel.Controls.Add(this.DauiMenu);
            this.MenuPanel.Controls.Add(this.InfoMessage);
            this.MenuPanel.Location = new System.Drawing.Point(193, 54);
            this.MenuPanel.Name = "MenuPanel";
            this.MenuPanel.Size = new System.Drawing.Size(367, 330);
            this.MenuPanel.TabIndex = 9;
            this.MenuPanel.Visible = false;
            // 
            // InfoMessage
            // 
            this.InfoMessage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.InfoMessage.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InfoMessage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.InfoMessage.Location = new System.Drawing.Point(-150, -34);
            this.InfoMessage.Name = "InfoMessage";
            this.InfoMessage.Padding = new System.Windows.Forms.Padding(5);
            this.InfoMessage.Size = new System.Drawing.Size(333, 110);
            this.InfoMessage.TabIndex = 10;
            this.InfoMessage.Text = "label1";
            this.InfoMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.InfoMessage.Visible = false;
            // 
            // button_menu
            // 
            this.button_menu.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button_menu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.button_menu.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_menu.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.button_menu.FlatAppearance.BorderSize = 2;
            this.button_menu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_menu.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_menu.ForeColor = System.Drawing.Color.Gray;
            this.button_menu.Location = new System.Drawing.Point(482, 2);
            this.button_menu.Margin = new System.Windows.Forms.Padding(0);
            this.button_menu.Name = "button_menu";
            this.button_menu.Size = new System.Drawing.Size(78, 69);
            this.button_menu.TabIndex = 12;
            this.button_menu.Text = "Menu";
            this.button_menu.UseVisualStyleBackColor = false;
            this.button_menu.Click += new System.EventHandler(this.button_menu_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.MenuPanel);
            this.Controls.Add(this.button_menu);
            this.Controls.Add(this.SceneGrid);
            this.Controls.Add(this.SnapGrid);
            this.Controls.Add(this.UIWeb);
            this.KeyPreview = true;
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "DAUIServer [WAIT Initialisation]";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmMain_FormClosed);
            this.Resize += new System.EventHandler(this.FrmMain_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.SnapGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SceneGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DauiMenu)).EndInit();
            this.MenuPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView SnapGrid;
        private System.Windows.Forms.DataGridView SceneGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn color;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn color2;
        private System.Windows.Forms.DataGridViewTextBoxColumn col1;
        private System.Windows.Forms.Timer Flash;
        private System.Windows.Forms.WebBrowser UIWeb;
        private System.Windows.Forms.Timer Connect;
        private System.Windows.Forms.DataGridView DauiMenu;
        private System.Windows.Forms.Panel MenuPanel;
        private System.Windows.Forms.Label InfoMessage;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Button button_menu;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
    }
}

