using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace WinFormsGraphicsDevice
{
    partial class MainForm
    {
        public static World world;
        public static Room selectedRoom = null;
        public static Sector selectedSector = null;
        public static Room zoomRoom = null;
        public static string currentFileName = null;
        public static Face selectedFace = null;
        public static Vector3 currentUp = Vector3.UnitY;

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

            #region initializations
            this.save = new System.Windows.Forms.Button();
            this.saveAs = new System.Windows.Forms.Button();
            this.load = new System.Windows.Forms.Button();
            this.clear = new System.Windows.Forms.Button();            

            this.sectorGroup = new System.Windows.Forms.GroupBox();
            this.sectorDropdown = new System.Windows.Forms.ComboBox();
            this.sectorNewButton = new System.Windows.Forms.Button();
            this.sectorNameField = new System.Windows.Forms.TextBox();
            this.sectorDelete = new System.Windows.Forms.Button();
            this.sectorView = new System.Windows.Forms.Button();

            this.roomDelete = new System.Windows.Forms.Button();
            this.roomEdit = new System.Windows.Forms.Button();
            this.roomGroup = new System.Windows.Forms.GroupBox();
            this.roomDropdown = new System.Windows.Forms.ComboBox();
            this.roomNewButton = new System.Windows.Forms.Button();
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
            this.roomSizeXUp = new System.Windows.Forms.Button();
            this.roomSizeYUp = new System.Windows.Forms.Button();
            this.roomSizeZUp = new System.Windows.Forms.Button();
            this.roomCenterXUp = new System.Windows.Forms.Button();
            this.roomCenterYUp = new System.Windows.Forms.Button();
            this.roomCenterZUp = new System.Windows.Forms.Button();
            this.roomCenterXDown = new System.Windows.Forms.Button();
            this.roomCenterYDown = new System.Windows.Forms.Button();
            this.roomCenterZDown = new System.Windows.Forms.Button();
            this.roomSizeXDown = new System.Windows.Forms.Button();
            this.roomSizeYDown = new System.Windows.Forms.Button();
            this.roomSizeZDown = new System.Windows.Forms.Button();
            #endregion

            this.WorldPreviewControl = new WinFormsGraphicsDevice.WorldPreviewControl();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();

            #region sectorObjects
            //
            // sectorObjects
            //
            this.sectorNewButton = new System.Windows.Forms.Button();
            this.sectorNewButton.Text = "New Sector";
            this.sectorNewButton.Location = new System.Drawing.Point(210, 19);
            this.sectorNewButton.Size = new System.Drawing.Size(80, 23);
            this.sectorNewButton.Click += this.world_create_new;

            this.sectorNameField = new System.Windows.Forms.TextBox();
            this.sectorNameField.Text = "New Sector";
            this.sectorNameField.Location = new System.Drawing.Point(10, 45);
            this.sectorNameField.Size = new System.Drawing.Size(160, 23);
            this.sectorNameField.TextChanged += this.world_rename;  

            this.sectorDropdown.DropDownHeight = 500;
            this.sectorDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sectorDropdown.FormattingEnabled = true;
            this.sectorDropdown.IntegralHeight = false;
            this.sectorDropdown.Location = new System.Drawing.Point(10, 20);
            this.sectorDropdown.Name = "SectorList";
            this.sectorDropdown.Size = new System.Drawing.Size(200, 20);
            this.sectorDropdown.TabIndex = 3;
            this.sectorDropdown.SelectedIndexChanged += new System.EventHandler(this.world_selected_change);

            this.sectorDelete.Location = new System.Drawing.Point(10, 120);
            this.sectorDelete.Size = new System.Drawing.Size(60, 25);
            this.sectorDelete.Text = "Delete";

            this.sectorView.Location = new System.Drawing.Point(220, 120);
            this.sectorView.Size = new System.Drawing.Size(60, 25);
            this.sectorView.Text = "View";
            this.sectorView.Click+=new System.EventHandler(world_zoom);

            this.roomDelete.Location = new System.Drawing.Point(10, 170);
            this.roomDelete.Size = new System.Drawing.Size(60, 25);
            this.roomDelete.Text = "Delete";

            //
            // sectorGroup
            //
            this.sectorGroup.Size = new System.Drawing.Size(300, 150);
            this.sectorGroup.Location = new System.Drawing.Point(10, 100);
            this.sectorGroup.Controls.Add(this.sectorNewButton);
            this.sectorGroup.Controls.Add(this.sectorDropdown);
            this.sectorGroup.Controls.Add(this.sectorNameField);
            this.sectorGroup.MouseHover += new System.EventHandler(this.world_mouse_hover);
            this.sectorGroup.MouseLeave += new System.EventHandler(this.world_mouse_leave);
            this.sectorGroup.Controls.Add(this.sectorDelete);
            this.sectorGroup.Controls.Add(this.sectorView);
            #endregion

            #region roomObjects

            #region roomDataFields
            // 
            // roomObjects
            //

            this.roomNewButton = new System.Windows.Forms.Button();
            this.roomNewButton.Text = "New Room";
            this.roomNewButton.Location = new System.Drawing.Point(210, 19);
            this.roomNewButton.Size = new System.Drawing.Size(80, 23);
            this.roomNewButton.Click += this.world_create_new;

            this.roomNameField = new System.Windows.Forms.TextBox();
            this.roomNameField.Text = "New Sector";
            this.roomNameField.Location = new System.Drawing.Point(10, 45);
            this.roomNameField.Size = new System.Drawing.Size(160, 23);
            this.roomNameField.TextChanged += this.world_rename;

            this.roomCenterX.Size = new System.Drawing.Size(50, 21);
            this.roomCenterX.Location = new System.Drawing.Point(30, 70);
            this.roomCenterX.Text = "0";
            this.roomCenterX.TextChanged += new System.EventHandler(world_data_change);

            this.roomCenterXUp.Text = "+";
            this.roomCenterXUp.Size = new System.Drawing.Size(20, 20);
            this.roomCenterXUp.Location = new System.Drawing.Point(80, 70);
            this.roomCenterXUp.Click += new System.EventHandler(world_value_increment);
            this.roomCenterXDown.Text = "-";
            this.roomCenterXDown.Size = new System.Drawing.Size(20, 20);
            this.roomCenterXDown.Location = new System.Drawing.Point(10, 70);
            this.roomCenterXDown.Click += new System.EventHandler(world_value_increment);

            this.roomCenterY.Size = new System.Drawing.Size(50, 21);
            this.roomCenterY.Location = new System.Drawing.Point(120, 70);
            this.roomCenterY.Text = "0";
            this.roomCenterY.TextChanged += new System.EventHandler(world_data_change);

            this.roomCenterYUp.Text = "+";
            this.roomCenterYUp.Size = new System.Drawing.Size(20, 20);
            this.roomCenterYUp.Location = new System.Drawing.Point(170, 70);
            this.roomCenterYUp.Click += new System.EventHandler(world_value_increment);
            this.roomCenterYDown.Text = "-";
            this.roomCenterYDown.Size = new System.Drawing.Size(20, 20);
            this.roomCenterYDown.Location = new System.Drawing.Point(100, 70);
            this.roomCenterYDown.Click += new System.EventHandler(world_value_increment);

            this.roomCenterZ.Size = new System.Drawing.Size(50, 21);
            this.roomCenterZ.Location = new System.Drawing.Point(210, 70);
            this.roomCenterZ.Text = "0";
            this.roomCenterZ.TextChanged += new System.EventHandler(world_data_change);

            this.roomCenterZUp.Text = "+";
            this.roomCenterZUp.Size = new System.Drawing.Size(20, 20);
            this.roomCenterZUp.Location = new System.Drawing.Point(260, 70);
            this.roomCenterZUp.Click += new System.EventHandler(world_value_increment);
            this.roomCenterZDown.Text = "-";
            this.roomCenterZDown.Size = new System.Drawing.Size(20, 20);
            this.roomCenterZDown.Location = new System.Drawing.Point(190, 70);
            this.roomCenterZDown.Click += new System.EventHandler(world_value_increment);

            this.roomSizeX.Size = new System.Drawing.Size(50, 21);
            this.roomSizeX.Location = new System.Drawing.Point(30, 90);
            this.roomSizeX.Text = "10";
            this.roomSizeX.TextChanged += new System.EventHandler(world_data_change);

            this.roomSizeXUp.Text = "+";
            this.roomSizeXUp.Size = new System.Drawing.Size(20, 20);
            this.roomSizeXUp.Location = new System.Drawing.Point(80, 90);
            this.roomSizeXUp.Click += new System.EventHandler(world_value_increment);
            this.roomSizeXDown.Text = "-";
            this.roomSizeXDown.Size = new System.Drawing.Size(20, 20);
            this.roomSizeXDown.Location = new System.Drawing.Point(10, 90);
            this.roomSizeXDown.Click += new System.EventHandler(world_value_increment);

            this.roomSizeY.Size = new System.Drawing.Size(50, 21);
            this.roomSizeY.Location = new System.Drawing.Point(120, 90);
            this.roomSizeY.Text = "10";
            this.roomSizeY.TextChanged += new System.EventHandler(world_data_change);

            this.roomSizeYUp.Text = "+";
            this.roomSizeYUp.Size = new System.Drawing.Size(20, 20);
            this.roomSizeYUp.Location = new System.Drawing.Point(170, 90);
            this.roomSizeYUp.Click += new System.EventHandler(world_value_increment);
            this.roomSizeYDown.Text = "-";
            this.roomSizeYDown.Size = new System.Drawing.Size(20, 20);
            this.roomSizeYDown.Location = new System.Drawing.Point(100, 90);
            this.roomSizeYDown.Click += new System.EventHandler(world_value_increment);

            this.roomSizeZ.Size = new System.Drawing.Size(50, 21);
            this.roomSizeZ.Location = new System.Drawing.Point(210, 90);
            this.roomSizeZ.Text = "10";
            this.roomSizeZ.TextChanged += new System.EventHandler(world_data_change);

            this.roomSizeZUp.Text = "+";
            this.roomSizeZUp.Size = new System.Drawing.Size(20, 20);
            this.roomSizeZUp.Location = new System.Drawing.Point(260, 90);
            this.roomSizeZUp.Click += new System.EventHandler(world_value_increment);
            this.roomSizeZDown.Text = "-";
            this.roomSizeZDown.Size = new System.Drawing.Size(20, 20);
            this.roomSizeZDown.Location = new System.Drawing.Point(190, 90);
            this.roomSizeZDown.Click += new System.EventHandler(world_value_increment);

            this.roomColorR.Size = new System.Drawing.Size(50, 21);
            this.roomColorR.Location = new System.Drawing.Point(30, 110);
            this.roomColorR.Text = "10";
            this.roomColorR.TextChanged += new System.EventHandler(world_data_change);

            this.roomColorRUp.Text = "+";
            this.roomColorRUp.Size = new System.Drawing.Size(20, 20);
            this.roomColorRUp.Location = new System.Drawing.Point(80, 110);
            this.roomColorRUp.Click += new System.EventHandler(world_value_increment);
            this.roomColorRDown.Text = "-";
            this.roomColorRDown.Size = new System.Drawing.Size(20, 20);
            this.roomColorRDown.Location = new System.Drawing.Point(10, 110);
            this.roomColorRDown.Click += new System.EventHandler(world_value_increment);

            this.roomColorG.Size = new System.Drawing.Size(50, 21);
            this.roomColorG.Location = new System.Drawing.Point(120, 110);
            this.roomColorG.Text = "10";
            this.roomColorG.TextChanged += new System.EventHandler(world_data_change);

            this.roomColorGUp.Text = "+";
            this.roomColorGUp.Size = new System.Drawing.Size(20, 20);
            this.roomColorGUp.Location = new System.Drawing.Point(170, 110);
            this.roomColorGUp.Click += new System.EventHandler(world_value_increment);
            this.roomColorGDown.Text = "-";
            this.roomColorGDown.Size = new System.Drawing.Size(20, 20);
            this.roomColorGDown.Location = new System.Drawing.Point(100, 110);
            this.roomColorGDown.Click += new System.EventHandler(world_value_increment);

            this.roomColorB.Size = new System.Drawing.Size(50, 21);
            this.roomColorB.Location = new System.Drawing.Point(210, 110);
            this.roomColorB.Text = "10";
            this.roomColorB.TextChanged += new System.EventHandler(world_data_change);

            this.roomColorBUp.Text = "+";
            this.roomColorBUp.Size = new System.Drawing.Size(20, 20);
            this.roomColorBUp.Location = new System.Drawing.Point(260, 110);
            this.roomColorBUp.Click += new System.EventHandler(world_value_increment);
            this.roomColorBDown.Text = "-";
            this.roomColorBDown.Size = new System.Drawing.Size(20, 20);
            this.roomColorBDown.Location = new System.Drawing.Point(190, 110);
            this.roomColorBDown.Click += new System.EventHandler(world_value_increment);
#endregion

            this.roomDropdown.DropDownHeight = 500;
            this.roomDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.roomDropdown.FormattingEnabled = true;
            this.roomDropdown.IntegralHeight = false;
            this.roomDropdown.Location = new System.Drawing.Point(10, 20);
            this.roomDropdown.Name = "testBox";
            this.roomDropdown.Size = new System.Drawing.Size(200, 20);
            this.roomDropdown.TabIndex = 3;
            this.roomDropdown.SelectedIndexChanged += new System.EventHandler(this.world_selected_change);
            this.roomDropdown.TextChanged += new System.EventHandler(this.world_rename);
            this.roomDropdown.MouseMove += new System.Windows.Forms.MouseEventHandler(this.world_highlight_room);

            this.roomEdit.Location = new System.Drawing.Point(210, 170);
            this.roomEdit.Text = "Edit";
            this.roomEdit.Size = new System.Drawing.Size(80, 25);
            this.roomEdit.Click += new System.EventHandler(this.world_zoom);

            #endregion

            //
            // roomGroup
            //
            this.roomGroup.Size = new System.Drawing.Size(300, 200);
            this.roomGroup.Location = new System.Drawing.Point(10, 250);
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
            this.roomGroup.MouseHover += new System.EventHandler(this.world_mouse_hover);
            this.roomGroup.MouseLeave += new System.EventHandler(this.world_mouse_leave);


            this.save.Text = "Save";
            this.save.Location = new System.Drawing.Point(10, 10);
            this.save.Size = new System.Drawing.Size(70, 20);
            this.save.Click += new System.EventHandler(this.editor_save);
            this.saveAs.Text = "Save As";
            this.saveAs.Location = new System.Drawing.Point(90, 10);
            this.saveAs.Size = new System.Drawing.Size(70, 20);
            this.saveAs.Click += new System.EventHandler(this.editor_saveAs);
            this.load.Text = "Load";
            this.load.Location = new System.Drawing.Point(170, 10);
            this.load.Size = new System.Drawing.Size(70, 20);
            this.load.Click += new System.EventHandler(this.editor_load);
            this.clear.Text = "New";
            this.clear.Location = new System.Drawing.Point(250, 10);
            this.clear.Size = new System.Drawing.Size(70, 20);
            this.clear.Click += new System.EventHandler(this.editor_clear);
           
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Panel1.Controls.Add(this.save);
            this.splitContainer1.Panel1.Controls.Add(this.saveAs);
            this.splitContainer1.Panel1.Controls.Add(this.load);
            this.splitContainer1.Panel1.Controls.Add(this.clear);
            this.splitContainer1.Panel1.Controls.Add(this.sectorGroup);            
            this.splitContainer1.Panel1.Controls.Add(this.roomGroup);
            
            // 
            // splitContainer1.Panel2
            // 

            this.splitContainer1.Panel2.Controls.Add(this.WorldPreviewControl);
            this.splitContainer1.Size = new System.Drawing.Size(792, 573);
            this.splitContainer1.SplitterDistance = 320;
            this.splitContainer1.TabIndex = 0;
            
            // 
            // WorldPreviewControl
            // 
            this.WorldPreviewControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WorldPreviewControl.Location = new System.Drawing.Point(0, 0);
            this.WorldPreviewControl.Name = "WorldPreviewControl";
            this.WorldPreviewControl.Size = new System.Drawing.Size(392, 573);
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
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private WorldPreviewControl WorldPreviewControl;

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
    }
}

