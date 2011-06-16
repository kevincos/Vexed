using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace WinFormsGraphicsDevice
{
    public enum EditMode
    {
        None,
        New,
        Line
    }

    partial class MainForm
    {
        public static World world;
        public static Room selectedRoom = null;
        public static Sector selectedSector = null;
        public static Room zoomRoom = null;
        public static string currentFileName = null;
        public static Face selectedFace = null;
        public static Vector3 currentUp = Vector3.UnitY;
        public static Vector2 translation = Vector2.Zero;
        public static EditMode editMode = EditMode.None;
        public static float animateSpeed = .001f;

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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.save = new System.Windows.Forms.Button();
            this.saveAs = new System.Windows.Forms.Button();
            this.load = new System.Windows.Forms.Button();
            this.clear = new System.Windows.Forms.Button();
            this.sectorGroup = new System.Windows.Forms.GroupBox();
            this.sectorNewButton = new System.Windows.Forms.Button();
            this.sectorDropdown = new System.Windows.Forms.ComboBox();
            this.sectorNameField = new System.Windows.Forms.TextBox();
            this.sectorDelete = new System.Windows.Forms.Button();
            this.sectorView = new System.Windows.Forms.Button();
            this.roomGroup = new System.Windows.Forms.GroupBox();
            this.roomNewButton = new System.Windows.Forms.Button();
            this.roomDropdown = new System.Windows.Forms.ComboBox();
            this.roomNameField = new System.Windows.Forms.TextBox();
            this.roomCenterX = new System.Windows.Forms.TextBox();
            this.roomCenterY = new System.Windows.Forms.TextBox();
            this.roomCenterZ = new System.Windows.Forms.TextBox();
            this.roomSizeX = new System.Windows.Forms.TextBox();
            this.roomSizeY = new System.Windows.Forms.TextBox();
            this.roomSizeZ = new System.Windows.Forms.TextBox();
            this.roomColorR = new System.Windows.Forms.TextBox();
            this.roomColorG = new System.Windows.Forms.TextBox();
            this.roomColorB = new System.Windows.Forms.TextBox();
            this.roomColorRUp = new System.Windows.Forms.Button();
            this.roomColorRDown = new System.Windows.Forms.Button();
            this.roomColorGUp = new System.Windows.Forms.Button();
            this.roomColorGDown = new System.Windows.Forms.Button();
            this.roomColorBUp = new System.Windows.Forms.Button();
            this.roomColorBDown = new System.Windows.Forms.Button();
            this.roomCenterXUp = new System.Windows.Forms.Button();
            this.roomCenterXDown = new System.Windows.Forms.Button();
            this.roomCenterYUp = new System.Windows.Forms.Button();
            this.roomCenterYDown = new System.Windows.Forms.Button();
            this.roomCenterZUp = new System.Windows.Forms.Button();
            this.roomCenterZDown = new System.Windows.Forms.Button();
            this.roomSizeXUp = new System.Windows.Forms.Button();
            this.roomSizeXDown = new System.Windows.Forms.Button();
            this.roomSizeYUp = new System.Windows.Forms.Button();
            this.roomSizeYDown = new System.Windows.Forms.Button();
            this.roomSizeZUp = new System.Windows.Forms.Button();
            this.roomSizeZDown = new System.Windows.Forms.Button();
            this.roomEdit = new System.Windows.Forms.Button();
            this.roomDelete = new System.Windows.Forms.Button();
            this.faceDown = new System.Windows.Forms.Button();
            this.faceUp = new System.Windows.Forms.Button();
            this.faceLeft = new System.Windows.Forms.Button();
            this.faceRight = new System.Windows.Forms.Button();
            this.debug1 = new System.Windows.Forms.TextBox();
            this.debug2 = new System.Windows.Forms.TextBox();
            this.debug3 = new System.Windows.Forms.TextBox();
            this.debug4 = new System.Windows.Forms.TextBox();
            this.modeDraw = new System.Windows.Forms.RadioButton();
            this.modeLine = new System.Windows.Forms.RadioButton();
            this.speedSlider = new System.Windows.Forms.TrackBar();

            this.viewControlsGroup = new System.Windows.Forms.GroupBox();
            this.WorldPreviewControl = new WinFormsGraphicsDevice.WorldPreviewControl();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.sectorGroup.SuspendLayout();
            this.roomGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.save);
            this.splitContainer1.Panel1.Controls.Add(this.saveAs);
            this.splitContainer1.Panel1.Controls.Add(this.load);
            this.splitContainer1.Panel1.Controls.Add(this.clear);
            this.splitContainer1.Panel1.Controls.Add(this.sectorGroup);
            this.splitContainer1.Panel1.Controls.Add(this.roomGroup);
            this.splitContainer1.Panel1.Controls.Add(this.viewControlsGroup);
            this.splitContainer1.Panel1.Controls.Add(this.debug1);
            this.splitContainer1.Panel1.Controls.Add(this.debug2);
            this.splitContainer1.Panel1.Controls.Add(this.debug3);
            this.splitContainer1.Panel1.Controls.Add(this.debug4);
            this.splitContainer1.Panel1.Controls.Add(this.speedSlider);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.WorldPreviewControl);
            this.splitContainer1.Size = new System.Drawing.Size(792, 573);
            this.splitContainer1.SplitterDistance = 320;
            this.splitContainer1.TabIndex = 0;
            // 
            // save
            // 
            this.save.Location = new System.Drawing.Point(10, 10);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(70, 20);
            this.save.TabIndex = 0;
            this.save.Text = "Save";
            this.save.Click += new System.EventHandler(this.editor_save);
            // 
            // saveAs
            // 
            this.saveAs.Location = new System.Drawing.Point(90, 10);
            this.saveAs.Name = "saveAs";
            this.saveAs.Size = new System.Drawing.Size(70, 20);
            this.saveAs.TabIndex = 1;
            this.saveAs.Text = "Save As";
            this.saveAs.Click += new System.EventHandler(this.editor_saveAs);
            // 
            // load
            // 
            this.load.Location = new System.Drawing.Point(170, 10);
            this.load.Name = "load";
            this.load.Size = new System.Drawing.Size(70, 20);
            this.load.TabIndex = 2;
            this.load.Text = "Load";
            this.load.Click += new System.EventHandler(this.editor_load);
            // 
            // clear
            // 
            this.clear.Location = new System.Drawing.Point(250, 10);
            this.clear.Name = "clear";
            this.clear.Size = new System.Drawing.Size(70, 20);
            this.clear.TabIndex = 3;
            this.clear.Text = "New";
            this.clear.Click += new System.EventHandler(this.editor_clear);

            //
            // speedSlider
            //
            this.speedSlider.Location = new System.Drawing.Point(200, 40);
            this.speedSlider.Scroll += new System.EventHandler(this.editor_change_speed);
            
            // 
            // sectorGroup
            // 
            this.sectorGroup.Controls.Add(this.sectorNewButton);
            this.sectorGroup.Controls.Add(this.sectorDropdown);
            this.sectorGroup.Controls.Add(this.sectorNameField);
            this.sectorGroup.Controls.Add(this.sectorDelete);
            this.sectorGroup.Controls.Add(this.sectorView);
            this.sectorGroup.Location = new System.Drawing.Point(10, 100);
            this.sectorGroup.Name = "sectorGroup";
            this.sectorGroup.Size = new System.Drawing.Size(300, 150);
            this.sectorGroup.TabIndex = 4;
            this.sectorGroup.TabStop = false;
            this.sectorGroup.MouseLeave += new System.EventHandler(this.world_mouse_leave);
            this.sectorGroup.MouseHover += new System.EventHandler(this.world_mouse_hover);
            // 
            // sectorNewButton
            // 
            this.sectorNewButton.Location = new System.Drawing.Point(210, 19);
            this.sectorNewButton.Name = "sectorNewButton";
            this.sectorNewButton.Size = new System.Drawing.Size(80, 23);
            this.sectorNewButton.TabIndex = 0;
            this.sectorNewButton.Text = "New Sector";
            // 
            // sectorDropdown
            // 
            this.sectorDropdown.DropDownHeight = 500;
            this.sectorDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sectorDropdown.FormattingEnabled = true;
            this.sectorDropdown.IntegralHeight = false;
            this.sectorDropdown.Location = new System.Drawing.Point(10, 20);
            this.sectorDropdown.Name = "sectorDropdown";
            this.sectorDropdown.Size = new System.Drawing.Size(200, 21);
            this.sectorDropdown.TabIndex = 3;
            this.sectorDropdown.SelectedIndexChanged += new System.EventHandler(this.world_selected_change);
            // 
            // sectorNameField
            // 
            this.sectorNameField.Location = new System.Drawing.Point(10, 45);
            this.sectorNameField.Name = "sectorNameField";
            this.sectorNameField.Size = new System.Drawing.Size(160, 20);
            this.sectorNameField.TabIndex = 4;
            this.sectorNameField.Text = "New Sector";
            // 
            // sectorDelete
            // 
            this.sectorDelete.Location = new System.Drawing.Point(10, 120);
            this.sectorDelete.Name = "sectorDelete";
            this.sectorDelete.Size = new System.Drawing.Size(60, 25);
            this.sectorDelete.TabIndex = 5;
            this.sectorDelete.Text = "Delete";
            // 
            // sectorView
            // 
            this.sectorView.Location = new System.Drawing.Point(220, 120);
            this.sectorView.Name = "sectorView";
            this.sectorView.Size = new System.Drawing.Size(60, 25);
            this.sectorView.TabIndex = 6;
            this.sectorView.Text = "View";
            this.sectorView.Click += new System.EventHandler(this.world_zoom);
            // 
            // roomGroup
            // 
            this.roomGroup.Controls.Add(this.roomNewButton);
            this.roomGroup.Controls.Add(this.roomDropdown);
            this.roomGroup.Controls.Add(this.roomNameField);
            this.roomGroup.Controls.Add(this.roomCenterX);
            this.roomGroup.Controls.Add(this.roomCenterY);
            this.roomGroup.Controls.Add(this.roomCenterZ);
            this.roomGroup.Controls.Add(this.roomSizeX);
            this.roomGroup.Controls.Add(this.roomSizeY);
            this.roomGroup.Controls.Add(this.roomSizeZ);
            this.roomGroup.Controls.Add(this.roomColorR);
            this.roomGroup.Controls.Add(this.roomColorG);
            this.roomGroup.Controls.Add(this.roomColorB);
            this.roomGroup.Controls.Add(this.roomColorRUp);
            this.roomGroup.Controls.Add(this.roomColorRDown);
            this.roomGroup.Controls.Add(this.roomColorGUp);
            this.roomGroup.Controls.Add(this.roomColorGDown);
            this.roomGroup.Controls.Add(this.roomColorBUp);
            this.roomGroup.Controls.Add(this.roomColorBDown);
            this.roomGroup.Controls.Add(this.roomCenterXUp);
            this.roomGroup.Controls.Add(this.roomCenterXDown);
            this.roomGroup.Controls.Add(this.roomCenterYUp);
            this.roomGroup.Controls.Add(this.roomCenterYDown);
            this.roomGroup.Controls.Add(this.roomCenterZUp);
            this.roomGroup.Controls.Add(this.roomCenterZDown);
            this.roomGroup.Controls.Add(this.roomSizeXUp);
            this.roomGroup.Controls.Add(this.roomSizeXDown);
            this.roomGroup.Controls.Add(this.roomSizeYUp);
            this.roomGroup.Controls.Add(this.roomSizeYDown);
            this.roomGroup.Controls.Add(this.roomSizeZUp);
            this.roomGroup.Controls.Add(this.roomSizeZDown);
            this.roomGroup.Controls.Add(this.roomEdit);
            this.roomGroup.Controls.Add(this.roomDelete);
            this.roomGroup.Location = new System.Drawing.Point(10, 250);
            this.roomGroup.Name = "roomGroup";
            this.roomGroup.Size = new System.Drawing.Size(300, 200);
            this.roomGroup.TabIndex = 5;
            this.roomGroup.TabStop = false;
            this.roomGroup.MouseLeave += new System.EventHandler(this.world_mouse_leave);
            this.roomGroup.MouseHover += new System.EventHandler(this.world_mouse_hover);
            // 
            // roomNewButton
            // 
            this.roomNewButton.Location = new System.Drawing.Point(210, 19);
            this.roomNewButton.Name = "roomNewButton";
            this.roomNewButton.Size = new System.Drawing.Size(80, 23);
            this.roomNewButton.TabIndex = 0;
            this.roomNewButton.Text = "New Room";
            this.roomNewButton.Click += new System.EventHandler(this.world_create_new);
            // 
            // roomDropdown
            // 
            this.roomDropdown.DropDownHeight = 500;
            this.roomDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.roomDropdown.FormattingEnabled = true;
            this.roomDropdown.IntegralHeight = false;
            this.roomDropdown.Location = new System.Drawing.Point(10, 20);
            this.roomDropdown.Name = "roomDropdown";
            this.roomDropdown.Size = new System.Drawing.Size(200, 21);
            this.roomDropdown.TabIndex = 3;
            this.roomDropdown.SelectedIndexChanged += new System.EventHandler(this.world_selected_change);
            this.roomDropdown.TextChanged += new System.EventHandler(this.world_rename);
            this.roomDropdown.MouseMove += new System.Windows.Forms.MouseEventHandler(this.world_highlight_room);
            // 
            // roomNameField
            // 
            this.roomNameField.Location = new System.Drawing.Point(10, 45);
            this.roomNameField.Name = "roomNameField";
            this.roomNameField.Size = new System.Drawing.Size(160, 20);
            this.roomNameField.TabIndex = 4;
            this.roomNameField.Text = "New Sector";
            // 
            // roomCenterX
            // 
            this.roomCenterX.Location = new System.Drawing.Point(30, 70);
            this.roomCenterX.Name = "roomCenterX";
            this.roomCenterX.Size = new System.Drawing.Size(50, 20);
            this.roomCenterX.TabIndex = 5;
            this.roomCenterX.Text = "0";
            this.roomCenterX.TextChanged += new System.EventHandler(this.world_data_change);
            // 
            // roomCenterY
            // 
            this.roomCenterY.Location = new System.Drawing.Point(120, 70);
            this.roomCenterY.Name = "roomCenterY";
            this.roomCenterY.Size = new System.Drawing.Size(50, 20);
            this.roomCenterY.TabIndex = 6;
            this.roomCenterY.Text = "0";
            this.roomCenterY.TextChanged += new System.EventHandler(this.world_data_change);
            // 
            // roomCenterZ
            // 
            this.roomCenterZ.Location = new System.Drawing.Point(210, 70);
            this.roomCenterZ.Name = "roomCenterZ";
            this.roomCenterZ.Size = new System.Drawing.Size(50, 20);
            this.roomCenterZ.TabIndex = 7;
            this.roomCenterZ.Text = "0";
            this.roomCenterZ.TextChanged += new System.EventHandler(this.world_data_change);
            // 
            // roomSizeX
            // 
            this.roomSizeX.Location = new System.Drawing.Point(30, 90);
            this.roomSizeX.Name = "roomSizeX";
            this.roomSizeX.Size = new System.Drawing.Size(50, 20);
            this.roomSizeX.TabIndex = 8;
            this.roomSizeX.Text = "10";
            this.roomSizeX.TextChanged += new System.EventHandler(this.world_data_change);
            // 
            // roomSizeY
            // 
            this.roomSizeY.Location = new System.Drawing.Point(120, 90);
            this.roomSizeY.Name = "roomSizeY";
            this.roomSizeY.Size = new System.Drawing.Size(50, 20);
            this.roomSizeY.TabIndex = 9;
            this.roomSizeY.Text = "10";
            this.roomSizeY.TextChanged += new System.EventHandler(this.world_data_change);
            // 
            // roomSizeZ
            // 
            this.roomSizeZ.Location = new System.Drawing.Point(210, 90);
            this.roomSizeZ.Name = "roomSizeZ";
            this.roomSizeZ.Size = new System.Drawing.Size(50, 20);
            this.roomSizeZ.TabIndex = 10;
            this.roomSizeZ.Text = "10";
            this.roomSizeZ.TextChanged += new System.EventHandler(this.world_data_change);
            // 
            // roomColorR
            // 
            this.roomColorR.Location = new System.Drawing.Point(30, 110);
            this.roomColorR.Name = "roomColorR";
            this.roomColorR.Size = new System.Drawing.Size(50, 20);
            this.roomColorR.TabIndex = 11;
            this.roomColorR.Text = "10";
            this.roomColorR.TextChanged += new System.EventHandler(this.world_data_change);
            // 
            // roomColorG
            // 
            this.roomColorG.Location = new System.Drawing.Point(120, 110);
            this.roomColorG.Name = "roomColorG";
            this.roomColorG.Size = new System.Drawing.Size(50, 20);
            this.roomColorG.TabIndex = 12;
            this.roomColorG.Text = "10";
            this.roomColorG.TextChanged += new System.EventHandler(this.world_data_change);
            // 
            // roomColorB
            // 
            this.roomColorB.Location = new System.Drawing.Point(210, 110);
            this.roomColorB.Name = "roomColorB";
            this.roomColorB.Size = new System.Drawing.Size(50, 20);
            this.roomColorB.TabIndex = 13;
            this.roomColorB.Text = "10";
            this.roomColorB.TextChanged += new System.EventHandler(this.world_data_change);
            // 
            // roomColorRUp
            // 
            this.roomColorRUp.Location = new System.Drawing.Point(80, 110);
            this.roomColorRUp.Name = "roomColorRUp";
            this.roomColorRUp.Size = new System.Drawing.Size(20, 20);
            this.roomColorRUp.TabIndex = 14;
            this.roomColorRUp.Text = "+";
            this.roomColorRUp.Click += new System.EventHandler(this.world_value_increment);
            // 
            // roomColorRDown
            // 
            this.roomColorRDown.Location = new System.Drawing.Point(10, 110);
            this.roomColorRDown.Name = "roomColorRDown";
            this.roomColorRDown.Size = new System.Drawing.Size(20, 20);
            this.roomColorRDown.TabIndex = 15;
            this.roomColorRDown.Text = "-";
            this.roomColorRDown.Click += new System.EventHandler(this.world_value_increment);
            // 
            // roomColorGUp
            // 
            this.roomColorGUp.Location = new System.Drawing.Point(170, 110);
            this.roomColorGUp.Name = "roomColorGUp";
            this.roomColorGUp.Size = new System.Drawing.Size(20, 20);
            this.roomColorGUp.TabIndex = 16;
            this.roomColorGUp.Text = "+";
            this.roomColorGUp.Click += new System.EventHandler(this.world_value_increment);
            // 
            // roomColorGDown
            // 
            this.roomColorGDown.Location = new System.Drawing.Point(100, 110);
            this.roomColorGDown.Name = "roomColorGDown";
            this.roomColorGDown.Size = new System.Drawing.Size(20, 20);
            this.roomColorGDown.TabIndex = 17;
            this.roomColorGDown.Text = "-";
            this.roomColorGDown.Click += new System.EventHandler(this.world_value_increment);
            // 
            // roomColorBUp
            // 
            this.roomColorBUp.Location = new System.Drawing.Point(260, 110);
            this.roomColorBUp.Name = "roomColorBUp";
            this.roomColorBUp.Size = new System.Drawing.Size(20, 20);
            this.roomColorBUp.TabIndex = 18;
            this.roomColorBUp.Text = "+";
            this.roomColorBUp.Click += new System.EventHandler(this.world_value_increment);
            // 
            // roomColorBDown
            // 
            this.roomColorBDown.Location = new System.Drawing.Point(190, 110);
            this.roomColorBDown.Name = "roomColorBDown";
            this.roomColorBDown.Size = new System.Drawing.Size(20, 20);
            this.roomColorBDown.TabIndex = 19;
            this.roomColorBDown.Text = "-";
            this.roomColorBDown.Click += new System.EventHandler(this.world_value_increment);
            // 
            // roomCenterXUp
            // 
            this.roomCenterXUp.Location = new System.Drawing.Point(80, 70);
            this.roomCenterXUp.Name = "roomCenterXUp";
            this.roomCenterXUp.Size = new System.Drawing.Size(20, 20);
            this.roomCenterXUp.TabIndex = 20;
            this.roomCenterXUp.Text = "+";
            this.roomCenterXUp.Click += new System.EventHandler(this.world_value_increment);
            // 
            // roomCenterXDown
            // 
            this.roomCenterXDown.Location = new System.Drawing.Point(10, 70);
            this.roomCenterXDown.Name = "roomCenterXDown";
            this.roomCenterXDown.Size = new System.Drawing.Size(20, 20);
            this.roomCenterXDown.TabIndex = 21;
            this.roomCenterXDown.Text = "-";
            this.roomCenterXDown.Click += new System.EventHandler(this.world_value_increment);
            // 
            // roomCenterYUp
            // 
            this.roomCenterYUp.Location = new System.Drawing.Point(170, 70);
            this.roomCenterYUp.Name = "roomCenterYUp";
            this.roomCenterYUp.Size = new System.Drawing.Size(20, 20);
            this.roomCenterYUp.TabIndex = 22;
            this.roomCenterYUp.Text = "+";
            this.roomCenterYUp.Click += new System.EventHandler(this.world_value_increment);
            // 
            // roomCenterYDown
            // 
            this.roomCenterYDown.Location = new System.Drawing.Point(100, 70);
            this.roomCenterYDown.Name = "roomCenterYDown";
            this.roomCenterYDown.Size = new System.Drawing.Size(20, 20);
            this.roomCenterYDown.TabIndex = 23;
            this.roomCenterYDown.Text = "-";
            this.roomCenterYDown.Click += new System.EventHandler(this.world_value_increment);
            // 
            // roomCenterZUp
            // 
            this.roomCenterZUp.Location = new System.Drawing.Point(260, 70);
            this.roomCenterZUp.Name = "roomCenterZUp";
            this.roomCenterZUp.Size = new System.Drawing.Size(20, 20);
            this.roomCenterZUp.TabIndex = 24;
            this.roomCenterZUp.Text = "+";
            this.roomCenterZUp.Click += new System.EventHandler(this.world_value_increment);
            // 
            // roomCenterZDown
            // 
            this.roomCenterZDown.Location = new System.Drawing.Point(190, 70);
            this.roomCenterZDown.Name = "roomCenterZDown";
            this.roomCenterZDown.Size = new System.Drawing.Size(20, 20);
            this.roomCenterZDown.TabIndex = 25;
            this.roomCenterZDown.Text = "-";
            this.roomCenterZDown.Click += new System.EventHandler(this.world_value_increment);
            // 
            // roomSizeXUp
            // 
            this.roomSizeXUp.Location = new System.Drawing.Point(80, 90);
            this.roomSizeXUp.Name = "roomSizeXUp";
            this.roomSizeXUp.Size = new System.Drawing.Size(20, 20);
            this.roomSizeXUp.TabIndex = 26;
            this.roomSizeXUp.Text = "+";
            this.roomSizeXUp.Click += new System.EventHandler(this.world_value_increment);
            // 
            // roomSizeXDown
            // 
            this.roomSizeXDown.Location = new System.Drawing.Point(10, 90);
            this.roomSizeXDown.Name = "roomSizeXDown";
            this.roomSizeXDown.Size = new System.Drawing.Size(20, 20);
            this.roomSizeXDown.TabIndex = 27;
            this.roomSizeXDown.Text = "-";
            this.roomSizeXDown.Click += new System.EventHandler(this.world_value_increment);
            // 
            // roomSizeYUp
            // 
            this.roomSizeYUp.Location = new System.Drawing.Point(170, 90);
            this.roomSizeYUp.Name = "roomSizeYUp";
            this.roomSizeYUp.Size = new System.Drawing.Size(20, 20);
            this.roomSizeYUp.TabIndex = 28;
            this.roomSizeYUp.Text = "+";
            this.roomSizeYUp.Click += new System.EventHandler(this.world_value_increment);
            // 
            // roomSizeYDown
            // 
            this.roomSizeYDown.Location = new System.Drawing.Point(100, 90);
            this.roomSizeYDown.Name = "roomSizeYDown";
            this.roomSizeYDown.Size = new System.Drawing.Size(20, 20);
            this.roomSizeYDown.TabIndex = 29;
            this.roomSizeYDown.Text = "-";
            this.roomSizeYDown.Click += new System.EventHandler(this.world_value_increment);
            // 
            // roomSizeZUp
            // 
            this.roomSizeZUp.Location = new System.Drawing.Point(260, 90);
            this.roomSizeZUp.Name = "roomSizeZUp";
            this.roomSizeZUp.Size = new System.Drawing.Size(20, 20);
            this.roomSizeZUp.TabIndex = 30;
            this.roomSizeZUp.Text = "+";
            this.roomSizeZUp.Click += new System.EventHandler(this.world_value_increment);
            // 
            // roomSizeZDown
            // 
            this.roomSizeZDown.Location = new System.Drawing.Point(190, 90);
            this.roomSizeZDown.Name = "roomSizeZDown";
            this.roomSizeZDown.Size = new System.Drawing.Size(20, 20);
            this.roomSizeZDown.TabIndex = 31;
            this.roomSizeZDown.Text = "-";
            this.roomSizeZDown.Click += new System.EventHandler(this.world_value_increment);
            // 
            // roomEdit
            // 
            this.roomEdit.Location = new System.Drawing.Point(210, 170);
            this.roomEdit.Name = "roomEdit";
            this.roomEdit.Size = new System.Drawing.Size(80, 25);
            this.roomEdit.TabIndex = 32;
            this.roomEdit.Text = "Edit";
            this.roomEdit.Click += new System.EventHandler(this.world_zoom);
            // 
            // roomDelete
            // 
            this.roomDelete.Location = new System.Drawing.Point(10, 170);
            this.roomDelete.Name = "roomDelete";
            this.roomDelete.Size = new System.Drawing.Size(60, 25);
            this.roomDelete.TabIndex = 33;
            this.roomDelete.Text = "Delete";

            //
            // faceButtons
            //
            this.faceDown.Location = new System.Drawing.Point(50, 60);            
            this.faceDown.Size = new System.Drawing.Size(40, 20);
            this.faceDown.Click+=new System.EventHandler(this.room_rotate);
            this.faceUp.Location = new System.Drawing.Point(50, 10);           
            this.faceUp.Size = new System.Drawing.Size(40, 20);
            this.faceUp.Click += new System.EventHandler(this.room_rotate);
            this.faceLeft.Location = new System.Drawing.Point(10, 35);            
            this.faceLeft.Size = new System.Drawing.Size(40, 20);
            this.faceLeft.Click += new System.EventHandler(this.room_rotate);
            this.faceRight.Location = new System.Drawing.Point(90, 35);            
            this.faceRight.Size = new System.Drawing.Size(40, 20);
            this.faceRight.Click += new System.EventHandler(this.room_rotate);

            //
            // mode radio buttons
            //

            this.modeDraw.Location = new System.Drawing.Point(150, 10);
            this.modeDraw.Text = "Draw Mode";
            this.modeDraw.Click += new System.EventHandler(room_mode_change);
            this.modeDraw.PerformClick();
            this.modeLine.Location = new System.Drawing.Point(150, 30);
            this.modeLine.Text = "Line Edit Mode";
            this.modeLine.Click += new System.EventHandler(room_mode_change);

            this.viewControlsGroup.Location = new System.Drawing.Point(10, 450);
            this.viewControlsGroup.Size = new System.Drawing.Size(250, 100);
            this.viewControlsGroup.Controls.Add(this.faceDown);
            this.viewControlsGroup.Controls.Add(this.faceLeft);
            this.viewControlsGroup.Controls.Add(this.faceRight);
            this.viewControlsGroup.Controls.Add(this.faceUp);
            this.viewControlsGroup.Controls.Add(this.modeDraw);
            this.viewControlsGroup.Controls.Add(this.modeLine);

            //
            // debug
            //
            this.debug1.Location = new System.Drawing.Point(10, 40);
            this.debug1.Size = new System.Drawing.Size(80, 20);
            this.debug2.Location = new System.Drawing.Point(10, 65);
            this.debug2.Size = new System.Drawing.Size(80, 20);
            this.debug3.Location = new System.Drawing.Point(100, 40);
            this.debug3.Size = new System.Drawing.Size(80, 20);
            this.debug4.Location = new System.Drawing.Point(100, 65);
            this.debug4.Size = new System.Drawing.Size(80, 20);

            // 
            // WorldPreviewControl
            // 
            this.WorldPreviewControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WorldPreviewControl.Location = new System.Drawing.Point(0, 0);
            this.WorldPreviewControl.Name = "WorldPreviewControl";
            this.WorldPreviewControl.Size = new System.Drawing.Size(468, 573);
            this.WorldPreviewControl.TabIndex = 0;
            this.WorldPreviewControl.Text = "WorldPreviewControl";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 573);
            this.Controls.Add(this.splitContainer1);
            this.Name = "MainForm";
            this.Text = "WinForms Graphics Device";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.sectorGroup.ResumeLayout(false);
            this.sectorGroup.PerformLayout();
            this.roomGroup.ResumeLayout(false);
            this.roomGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private WorldPreviewControl WorldPreviewControl;

        private System.Windows.Forms.GroupBox viewControlsGroup;
        private System.Windows.Forms.Button faceUp;
        private System.Windows.Forms.Button faceDown;
        private System.Windows.Forms.Button faceLeft;
        private System.Windows.Forms.Button faceRight;
        private System.Windows.Forms.RadioButton modeDraw;
        private System.Windows.Forms.RadioButton modeLine;

        private System.Windows.Forms.GroupBox sectorGroup;
        private System.Windows.Forms.ComboBox sectorDropdown;

        private System.Windows.Forms.Button sectorNewButton;
        private System.Windows.Forms.TextBox sectorNameField;        

        private System.Windows.Forms.GroupBox roomGroup;
        private System.Windows.Forms.ComboBox roomDropdown;
        private System.Windows.Forms.Button roomNewButton;
        private System.Windows.Forms.TextBox roomNameField;
        private System.Windows.Forms.TextBox roomColorR, roomColorG, roomColorB;
        private System.Windows.Forms.Button roomColorRUp, roomColorRDown, roomColorGUp, roomColorGDown, roomColorBUp, roomColorBDown;
        private System.Windows.Forms.Button roomCenterXUp, roomCenterXDown, roomCenterYUp, roomCenterYDown, roomCenterZUp, roomCenterZDown;
        private System.Windows.Forms.Button roomSizeXUp, roomSizeXDown, roomSizeYUp, roomSizeYDown, roomSizeZUp, roomSizeZDown;
        private System.Windows.Forms.TextBox roomCenterX, roomCenterY, roomCenterZ;
        private System.Windows.Forms.TextBox roomSizeX, roomSizeY, roomSizeZ;

        private System.Windows.Forms.Button save, saveAs, load, clear;

        private System.Windows.Forms.Button roomEdit;
        private System.Windows.Forms.Button roomDelete;
        private System.Windows.Forms.Button sectorDelete;
        private System.Windows.Forms.Button sectorView;

        private System.Windows.Forms.TrackBar speedSlider;
        public System.Windows.Forms.TextBox debug1, debug2, debug3, debug4;
    }
}

