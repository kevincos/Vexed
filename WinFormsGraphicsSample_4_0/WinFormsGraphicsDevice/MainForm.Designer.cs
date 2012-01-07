using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using VL;

namespace WinFormsGraphicsDevice
{
    partial class MainForm
    {                
        public static World world;
        public static World undoWorld;
        public static Room selectedRoom = null;
        public static Sector selectedSector = null;
        public static Room zoomRoom = null;
        public static Doodad selectedDoodad = null;
        public static Monster selectedMonster = null;
        public static Decoration selectedDecoration = null;
        public static string currentFileName = null;
        public static Face selectedFace = null;
        public static Block selectedBlock = null;
        public static Edge selectedEdge = null;
        public static Vector3 currentUp = Vector3.UnitY;
        public static Vector2 translation = Vector2.Zero;
        public static EditMode editMode = EditMode.None;
        public static float animateSpeed = .001f;
        public static bool cameraReady = true;        

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
            #region ComponentInitializations
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.save = new System.Windows.Forms.Button();
            this.saveAs = new System.Windows.Forms.Button();
            this.load = new System.Windows.Forms.Button();
            this.clear = new System.Windows.Forms.Button();
            this.undo = new System.Windows.Forms.Button();
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
            this.faceClockwise = new System.Windows.Forms.Button();
            this.faceCounterClockwise = new System.Windows.Forms.Button();
            this.debug1 = new System.Windows.Forms.TextBox();
            this.debug2 = new System.Windows.Forms.TextBox();
            this.debug3 = new System.Windows.Forms.TextBox();
            this.debug4 = new System.Windows.Forms.TextBox();
            this.modeDraw = new System.Windows.Forms.RadioButton();
            this.modeLine = new System.Windows.Forms.RadioButton();
            this.modePoint = new System.Windows.Forms.RadioButton();
            this.modeDoodad = new System.Windows.Forms.RadioButton();
            this.modeMonster = new System.Windows.Forms.RadioButton();
            this.modeDecoration = new System.Windows.Forms.RadioButton();
            this.speedSlider = new System.Windows.Forms.TrackBar();
            this.elementGroup = new System.Windows.Forms.GroupBox();
            this.elementNameField = new System.Windows.Forms.TextBox();
            this.elementIDField = new System.Windows.Forms.TextBox();
            this.elementBehaviorDropdown = new System.Windows.Forms.ComboBox();
            this.elementBehaviorAdd = new System.Windows.Forms.Button();
            this.elementBehaviorDelete = new System.Windows.Forms.Button();
            this.elementDelete = new System.Windows.Forms.Button();
            this.behaviorNameField = new System.Windows.Forms.TextBox();
            
            this.behaviorPrimaryValue = new System.Windows.Forms.TextBox();
            this.behaviorPrimaryValueLabel = new System.Windows.Forms.Label();
            this.behaviorSecondaryValue = new System.Windows.Forms.TextBox();
            this.behaviorSecondaryValueLabel = new System.Windows.Forms.Label();
            this.behaviorPropertiesGroup = new System.Windows.Forms.GroupBox();
            this.behaviorDuration = new System.Windows.Forms.TextBox();
            this.behaviorDurationLabel = new System.Windows.Forms.Label();
            this.behaviorPeriod = new System.Windows.Forms.TextBox();
            this.behaviorPeriodLabel = new System.Windows.Forms.Label();
            this.behaviorNextBehavior = new System.Windows.Forms.TextBox();
            this.behaviorNextBehaviorLabel = new System.Windows.Forms.Label();
            this.behaviorOffset = new System.Windows.Forms.TextBox();
            this.behaviorOffsetLabel = new System.Windows.Forms.Label();
            this.behaviorToggle = new System.Windows.Forms.CheckBox();
            this.behaviorDestinationLabel = new System.Windows.Forms.Label();
            this.behaviorDestinationX = new System.Windows.Forms.TextBox();
            this.behaviorDestinationY = new System.Windows.Forms.TextBox();
            this.behaviorDestinationZ = new System.Windows.Forms.TextBox();

            this.doodadPropertiesGroup = new System.Windows.Forms.GroupBox();
            this.doodadFixed = new System.Windows.Forms.CheckBox();
            this.doodadAbilityDropdown = new System.Windows.Forms.ComboBox();
            this.doodadActivationCost = new System.Windows.Forms.TextBox();
            this.doodadActivationCostLabel = new System.Windows.Forms.Label();
            this.doodadExpectedBehavior = new System.Windows.Forms.TextBox();
            this.doodadExpectedBehaviorLabel = new System.Windows.Forms.Label();
            this.doodadTarget = new System.Windows.Forms.TextBox();
            this.doodadTargetLabel = new System.Windows.Forms.Label();
            this.doodadTargetBehavior = new System.Windows.Forms.TextBox();
            this.doodadTargetBehaviorLabel = new System.Windows.Forms.Label();
            this.doodadTypeDropdown = new System.Windows.Forms.ComboBox();

            this.monsterPropertiesGroup = new System.Windows.Forms.GroupBox();
            this.monsterFixedPath = new System.Windows.Forms.CheckBox();
            this.monsterMovementDropdown = new System.Windows.Forms.ComboBox();
            this.monsterArmorDropdown = new System.Windows.Forms.ComboBox();
            this.monsterSpeedDropdown = new System.Windows.Forms.ComboBox();
            this.monsterSizeDropdown = new System.Windows.Forms.ComboBox();
            this.monsterTrackingDropdown = new System.Windows.Forms.ComboBox();
            this.monsterHealthDropdown = new System.Windows.Forms.ComboBox();
            this.monsterWeaponDropdown = new System.Windows.Forms.ComboBox();
            this.monsterAIDropdown = new System.Windows.Forms.ComboBox();
            this.monsterWaypointID = new System.Windows.Forms.TextBox();
            this.monsterWaypointIDLabel = new System.Windows.Forms.Label();

            this.decorationPropertiesGroup = new System.Windows.Forms.GroupBox();
            this.decorationTexture = new System.Windows.Forms.TextBox();
            this.decorationDepth = new System.Windows.Forms.TextBox();
            this.decorationWrap = new System.Windows.Forms.CheckBox();
            this.decorationStartFrame = new System.Windows.Forms.TextBox();
            this.decorationSpin = new System.Windows.Forms.CheckBox();
            this.decorationR = new System.Windows.Forms.TextBox();
            this.decorationG = new System.Windows.Forms.TextBox();
            this.decorationB = new System.Windows.Forms.TextBox();

            this.viewControlsGroup = new System.Windows.Forms.GroupBox();
            this.WorldPreviewControl = new WinFormsGraphicsDevice.WorldPreviewControl();
            this.edgePropertiesGroup = new System.Windows.Forms.GroupBox();
            this.edgeTypeDropdown = new System.Windows.Forms.ComboBox();

            this.blockPropertiesGroup = new System.Windows.Forms.GroupBox();
            this.blockColorR = new System.Windows.Forms.TextBox();
            this.blockColorG = new System.Windows.Forms.TextBox();
            this.blockColorB = new System.Windows.Forms.TextBox();
            this.blockType = new System.Windows.Forms.ComboBox();
            this.blockScale = new System.Windows.Forms.CheckBox();
            this.blockDepth = new System.Windows.Forms.TextBox();

            #endregion

            #region splitcontainer setup
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
            this.splitContainer1.Panel1.Controls.Add(this.undo);
            this.splitContainer1.Panel1.Controls.Add(this.sectorGroup);
            this.splitContainer1.Panel1.Controls.Add(this.roomGroup);
            this.splitContainer1.Panel1.Controls.Add(this.viewControlsGroup);
            this.splitContainer1.Panel1.Controls.Add(this.elementGroup);
            this.splitContainer1.Panel1.Controls.Add(this.edgePropertiesGroup);
            this.splitContainer1.Panel1.Controls.Add(this.blockPropertiesGroup);
            this.splitContainer1.Panel1.Controls.Add(this.doodadPropertiesGroup);
            this.splitContainer1.Panel1.Controls.Add(this.decorationPropertiesGroup);
            this.splitContainer1.Panel1.Controls.Add(this.monsterPropertiesGroup);
            this.splitContainer1.Panel1.Controls.Add(this.behaviorPropertiesGroup);
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
            this.splitContainer1.SplitterDistance = 275;
            this.splitContainer1.TabIndex = 0;
            #endregion

            #region saveloadedit
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
            // clear
            // 
            this.undo.Location = new System.Drawing.Point(250, 40);
            this.undo.Size = new System.Drawing.Size(70, 20);
            this.undo.TabIndex = 3;
            this.undo.Text = "Undo";
            this.undo.Click += new System.EventHandler(this.editor_undo);

            //
            // speedSlider
            //
            this.speedSlider.Location = new System.Drawing.Point(200, 60);
            this.speedSlider.Scroll += new System.EventHandler(this.editor_change_speed);
            #endregion

            #region sectorinfo
            // 
            // sectorGroup
            // 
            this.sectorGroup.Controls.Add(this.sectorNewButton);
            this.sectorGroup.Controls.Add(this.sectorDropdown);
            this.sectorGroup.Controls.Add(this.sectorNameField);
            this.sectorGroup.Controls.Add(this.sectorDelete);
            this.sectorGroup.Controls.Add(this.sectorView);
            this.sectorGroup.Location = new System.Drawing.Point(10, 85);
            this.sectorGroup.Name = "sectorGroup";
            this.sectorGroup.Size = new System.Drawing.Size(300, 100);
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
            this.sectorNewButton.Click += new System.EventHandler(this.world_create_new);
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
            this.sectorNameField.TextChanged += new System.EventHandler(world_rename);
            // 
            // sectorDelete
            // 
            this.sectorDelete.Location = new System.Drawing.Point(10, 70);
            this.sectorDelete.Name = "sectorDelete";
            this.sectorDelete.Size = new System.Drawing.Size(60, 25);
            this.sectorDelete.TabIndex = 5;
            this.sectorDelete.Text = "Delete";
            this.sectorDelete.Click += new System.EventHandler(this.world_delete);
            // 
            // sectorView
            // 
            this.sectorView.Location = new System.Drawing.Point(220, 70);
            this.sectorView.Name = "sectorView";
            this.sectorView.Size = new System.Drawing.Size(60, 25);
            this.sectorView.TabIndex = 6;
            this.sectorView.Text = "View";
            this.sectorView.Click += new System.EventHandler(this.world_zoom);
            #endregion

            #region elementproperties
            //
            // elemenetProperties Group
            //
            this.elementGroup.Location = new System.Drawing.Point(10, 470);
            this.elementGroup.Size = new System.Drawing.Size(220, 145);
            this.elementGroup.Visible = false;
            this.elementNameField.Location = new System.Drawing.Point(10, 10);
            this.elementNameField.Size = new System.Drawing.Size(100, 20);
            this.elementNameField.TextChanged += new System.EventHandler(this.world_rename);
            this.elementIDField.Location = new System.Drawing.Point(110, 10);
            this.elementIDField.Size = new System.Drawing.Size(100, 20);            
            this.elementIDField.TextChanged += new System.EventHandler(this.world_rename);
            this.elementIDField.ReadOnly = true;
            
            this.elementBehaviorDropdown.Location = new System.Drawing.Point(10, 60);
            this.elementBehaviorDropdown.Size = new System.Drawing.Size(200, 20);
            this.elementBehaviorDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.elementBehaviorDropdown.SelectedIndexChanged += new System.EventHandler(this.behavior_select);
            this.elementDelete.Location = new System.Drawing.Point(140, 35);
            this.elementDelete.Size = new System.Drawing.Size(70, 20);
            this.elementDelete.Click += new System.EventHandler(this.world_delete);
            this.elementDelete.Text = "Delete";
            this.elementBehaviorAdd.Location = new System.Drawing.Point(10, 85);
            this.elementBehaviorAdd.Size = new System.Drawing.Size(70, 20);
            this.elementBehaviorAdd.Click += new System.EventHandler(this.world_create_new);
            this.elementBehaviorAdd.Text = "New";
            this.elementBehaviorDelete.Location = new System.Drawing.Point(140, 85);
            this.elementBehaviorDelete.Size = new System.Drawing.Size(70, 20);
            this.elementBehaviorDelete.Click += new System.EventHandler(this.world_delete);
            this.elementBehaviorDelete.Text = "Delete";
            this.behaviorNameField.Location = new System.Drawing.Point(10, 115);
            this.behaviorNameField.Size = new System.Drawing.Size(200, 20);
            this.behaviorNameField.TextChanged += new System.EventHandler(this.world_rename);

            this.edgePropertiesGroup.Location = new System.Drawing.Point(10, 605);
            this.edgePropertiesGroup.Size = new System.Drawing.Size(220, 300);
            this.edgePropertiesGroup.Controls.Add(this.edgeTypeDropdown);
            this.edgePropertiesGroup.Visible = false;

            this.blockPropertiesGroup.Location = new System.Drawing.Point(10, 605);
            this.blockPropertiesGroup.Size = new System.Drawing.Size(220, 300);
            this.blockPropertiesGroup.Controls.Add(this.blockColorR);
            this.blockPropertiesGroup.Controls.Add(this.blockColorG);
            this.blockPropertiesGroup.Controls.Add(this.blockColorB);
            this.blockPropertiesGroup.Controls.Add(this.blockDepth);
            this.blockPropertiesGroup.Controls.Add(this.blockScale);
            this.blockPropertiesGroup.Controls.Add(this.blockType);
            this.blockPropertiesGroup.Visible = false;

            this.blockColorR.Location = new System.Drawing.Point(10, 10);
            this.blockColorR.Size = new System.Drawing.Size(50, 20);
            this.blockColorR.TextChanged += new System.EventHandler(this.block_change);
            this.blockColorG.Location = new System.Drawing.Point(70, 10);
            this.blockColorG.Size = new System.Drawing.Size(50, 20);
            this.blockColorG.TextChanged += new System.EventHandler(this.block_change);
            this.blockColorB.Location = new System.Drawing.Point(130, 10);
            this.blockColorB.Size = new System.Drawing.Size(50, 20);
            this.blockColorB.TextChanged += new System.EventHandler(this.block_change);

            this.blockDepth.Location = new System.Drawing.Point(140, 30);
            this.blockDepth.Size = new System.Drawing.Size(50, 20);
            this.blockDepth.TextChanged += new System.EventHandler(this.block_change);

            this.blockScale.Location = new System.Drawing.Point(80, 30);
            this.blockScale.Size = new System.Drawing.Size(60, 20);
            this.blockScale.Text = "Scales";
            this.blockScale.CheckStateChanged += new System.EventHandler(this.block_change);

            this.blockType.Location = new System.Drawing.Point(10, 30);
            this.blockType.Size = new System.Drawing.Size(60, 20);
            this.blockType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            for (int i = 0; i < 11; i++)
            {
                this.blockType.Items.Add((WallType)i);
            }
            this.blockType.SelectedIndex = 0;
            this.blockType.SelectedIndexChanged += new System.EventHandler(this.block_change);

            this.behaviorPropertiesGroup.Location = new System.Drawing.Point(230, 470);
            this.behaviorPropertiesGroup.Size = new System.Drawing.Size(110, 300);
            this.behaviorPropertiesGroup.Controls.Add(this.behaviorToggle);
            this.behaviorPropertiesGroup.Controls.Add(this.behaviorNextBehavior);
            this.behaviorPropertiesGroup.Controls.Add(this.behaviorNextBehaviorLabel);
            this.behaviorPropertiesGroup.Controls.Add(this.behaviorDuration);
            this.behaviorPropertiesGroup.Controls.Add(this.behaviorDurationLabel);
            this.behaviorPropertiesGroup.Controls.Add(this.behaviorOffset);
            this.behaviorPropertiesGroup.Controls.Add(this.behaviorOffsetLabel);
            this.behaviorPropertiesGroup.Controls.Add(this.behaviorPeriod);
            this.behaviorPropertiesGroup.Controls.Add(this.behaviorPeriodLabel);
            this.behaviorPropertiesGroup.Controls.Add(this.behaviorDestinationLabel);
            this.behaviorPropertiesGroup.Controls.Add(this.behaviorDestinationX);
            this.behaviorPropertiesGroup.Controls.Add(this.behaviorDestinationY);
            this.behaviorPropertiesGroup.Controls.Add(this.behaviorDestinationZ);
            this.behaviorPropertiesGroup.Controls.Add(this.behaviorPrimaryValue);
            this.behaviorPropertiesGroup.Controls.Add(this.behaviorPrimaryValueLabel);
            this.behaviorPropertiesGroup.Controls.Add(this.behaviorSecondaryValue);
            this.behaviorPropertiesGroup.Controls.Add(this.behaviorSecondaryValueLabel);
            this.behaviorPropertiesGroup.Visible = false;

            this.monsterPropertiesGroup.Location = new System.Drawing.Point(10, 605);
            this.monsterPropertiesGroup.Size = new System.Drawing.Size(300, 300);
            this.monsterPropertiesGroup.Visible = false;
            this.monsterPropertiesGroup.Controls.Add(this.monsterMovementDropdown);
            this.monsterPropertiesGroup.Controls.Add(this.monsterArmorDropdown);
            this.monsterPropertiesGroup.Controls.Add(this.monsterHealthDropdown);
            this.monsterPropertiesGroup.Controls.Add(this.monsterSizeDropdown);
            this.monsterPropertiesGroup.Controls.Add(this.monsterSpeedDropdown);
            this.monsterPropertiesGroup.Controls.Add(this.monsterTrackingDropdown);
            this.monsterPropertiesGroup.Controls.Add(this.monsterWeaponDropdown);
            this.monsterPropertiesGroup.Controls.Add(this.monsterAIDropdown);
            this.monsterPropertiesGroup.Controls.Add(this.monsterFixedPath);
            this.monsterPropertiesGroup.Controls.Add(this.monsterWaypointID);
            this.monsterPropertiesGroup.Controls.Add(this.monsterWaypointIDLabel);

            

            this.monsterMovementDropdown.Location = new System.Drawing.Point(10, 10);
            this.monsterMovementDropdown.Size = new System.Drawing.Size(180, 20);
            this.monsterMovementDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            for (int i = 0; i < 12; i++ )
            {
                this.monsterMovementDropdown.Items.Add((MovementType)i);
            }
            this.monsterMovementDropdown.SelectedIndex = 0;
            this.monsterMovementDropdown.SelectedIndexChanged += new System.EventHandler(this.monster_change);

            this.monsterArmorDropdown.Location = new System.Drawing.Point(10, 35);
            this.monsterArmorDropdown.Size = new System.Drawing.Size(180, 20);
            this.monsterArmorDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            for (int i = 0; i < 7; i++)
            {
                this.monsterArmorDropdown.Items.Add((ArmorType)i);
            }
            this.monsterArmorDropdown.SelectedIndex = 0;
            this.monsterArmorDropdown.SelectedIndexChanged += new System.EventHandler(this.monster_change);

            this.monsterHealthDropdown.Location = new System.Drawing.Point(10, 130);
            this.monsterHealthDropdown.Size = new System.Drawing.Size(60, 20);
            this.monsterHealthDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            for (int i = 0; i < 3; i++)
            {
                this.monsterHealthDropdown.Items.Add((MonsterHealth)i);
            }
            this.monsterHealthDropdown.SelectedIndex = 0;
            this.monsterHealthDropdown.SelectedIndexChanged += new System.EventHandler(this.monster_change);

            this.monsterSizeDropdown.Location = new System.Drawing.Point(70, 130);
            this.monsterSizeDropdown.Size = new System.Drawing.Size(60, 20);
            this.monsterSizeDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            for (int i = 0; i < 3; i++)
            {
                this.monsterSizeDropdown.Items.Add((MonsterSize)i);
            }
            this.monsterSizeDropdown.SelectedIndex = 0;
            this.monsterSizeDropdown.SelectedIndexChanged += new System.EventHandler(this.monster_change);

            this.monsterSpeedDropdown.Location = new System.Drawing.Point(130, 130);
            this.monsterSpeedDropdown.Size = new System.Drawing.Size(60, 20);
            this.monsterSpeedDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            for (int i = 0; i < 3; i++)
            {
                this.monsterSpeedDropdown.Items.Add((MonsterSpeed)i);
            }
            this.monsterSpeedDropdown.SelectedIndex = 0;
            this.monsterSpeedDropdown.SelectedIndexChanged += new System.EventHandler(this.monster_change);

            this.monsterTrackingDropdown.Location = new System.Drawing.Point(190, 130);
            this.monsterTrackingDropdown.Size = new System.Drawing.Size(100, 20);
            this.monsterTrackingDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            for (int i = 0; i < 8; i++)
            {
                this.monsterTrackingDropdown.Items.Add((TrackType)i);
            }
            this.monsterTrackingDropdown.SelectedIndex = 0;
            this.monsterTrackingDropdown.SelectedIndexChanged += new System.EventHandler(this.monster_change);

            this.monsterWeaponDropdown.Location = new System.Drawing.Point(10, 60);
            this.monsterWeaponDropdown.Size = new System.Drawing.Size(180, 20);
            this.monsterWeaponDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            for (int i = 0; i < 6; i++)
            {
                this.monsterWeaponDropdown.Items.Add((GunType)i);
            }
            this.monsterWeaponDropdown.SelectedIndex = 0;
            this.monsterWeaponDropdown.SelectedIndexChanged += new System.EventHandler(this.monster_change);

            this.monsterAIDropdown.Location = new System.Drawing.Point(10, 85);
            this.monsterAIDropdown.Size = new System.Drawing.Size(180, 20);
            this.monsterAIDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            for (int i = 0; i < 4; i++)
            {
                this.monsterAIDropdown.Items.Add((AIType)i);
            }
            this.monsterAIDropdown.SelectedIndex = 0;
            this.monsterAIDropdown.SelectedIndexChanged += new System.EventHandler(this.monster_change);

            this.monsterFixedPath.Location = new System.Drawing.Point(200, 95);
            this.monsterFixedPath.Text = "Fixed Path";
            this.monsterFixedPath.CheckedChanged += new System.EventHandler(this.monster_change);

            this.monsterWaypointID.Location = new System.Drawing.Point(10, 110);
            this.monsterWaypointID.Size = new System.Drawing.Size(180, 20);
            this.monsterWaypointID.TextChanged += new System.EventHandler(this.monster_change);

            this.decorationPropertiesGroup.Location = new System.Drawing.Point(10, 605);
            this.decorationPropertiesGroup.Size = new System.Drawing.Size(300, 300);
            this.decorationPropertiesGroup.Visible = false;
            this.decorationPropertiesGroup.Controls.Add(this.decorationTexture);
            this.decorationPropertiesGroup.Controls.Add(this.decorationDepth);
            this.decorationPropertiesGroup.Controls.Add(this.decorationWrap);
            this.decorationPropertiesGroup.Controls.Add(this.decorationSpin);
            this.decorationPropertiesGroup.Controls.Add(this.decorationStartFrame);
            this.decorationPropertiesGroup.Controls.Add(this.decorationR);
            this.decorationPropertiesGroup.Controls.Add(this.decorationG);
            this.decorationPropertiesGroup.Controls.Add(this.decorationB);

            this.decorationTexture.Location = new System.Drawing.Point(10, 110);
            this.decorationTexture.Size = new System.Drawing.Size(200, 20);
            this.decorationTexture.TextChanged += new System.EventHandler(this.decoration_change);

            this.decorationDepth.Location = new System.Drawing.Point(10, 85);
            this.decorationDepth.Size = new System.Drawing.Size(80, 20);
            this.decorationDepth.TextChanged += new System.EventHandler(this.decoration_change);

            this.decorationStartFrame.Location = new System.Drawing.Point(110, 85);
            this.decorationStartFrame.Size = new System.Drawing.Size(60, 20);
            this.decorationStartFrame.TextChanged += new System.EventHandler(this.decoration_change);

            this.decorationR.Location = new System.Drawing.Point(10, 60);
            this.decorationR.Size = new System.Drawing.Size(30, 20);
            this.decorationR.TextChanged += new System.EventHandler(this.decoration_change);

            this.decorationG.Location = new System.Drawing.Point(50, 60);
            this.decorationG.Size = new System.Drawing.Size(30, 20);
            this.decorationG.TextChanged += new System.EventHandler(this.decoration_change);

            this.decorationB.Location = new System.Drawing.Point(90, 60);
            this.decorationB.Size = new System.Drawing.Size(30, 20);
            this.decorationB.TextChanged += new System.EventHandler(this.decoration_change);

            this.decorationWrap.Location = new System.Drawing.Point(10, 35);
            this.decorationWrap.Size = new System.Drawing.Size(60, 20);
            this.decorationWrap.CheckedChanged += new System.EventHandler(this.decoration_change);
            this.decorationWrap.Text = "Wrap";

            this.decorationSpin.Location = new System.Drawing.Point(80, 35);
            this.decorationSpin.Size = new System.Drawing.Size(60, 20);
            this.decorationSpin.CheckedChanged += new System.EventHandler(this.decoration_change);
            this.decorationSpin.Text = "Spin";


            this.doodadPropertiesGroup.Location = new System.Drawing.Point(10, 605);
            this.doodadPropertiesGroup.Size = new System.Drawing.Size(300, 300);
            this.doodadPropertiesGroup.Visible = false;
            this.doodadPropertiesGroup.Controls.Add(this.doodadAbilityDropdown);
            this.doodadPropertiesGroup.Controls.Add(this.doodadTypeDropdown);
            this.doodadPropertiesGroup.Controls.Add(this.doodadActivationCost);
            this.doodadPropertiesGroup.Controls.Add(this.doodadActivationCostLabel);
            this.doodadPropertiesGroup.Controls.Add(this.doodadExpectedBehavior);
            this.doodadPropertiesGroup.Controls.Add(this.doodadExpectedBehaviorLabel);
            this.doodadPropertiesGroup.Controls.Add(this.doodadFixed);
            this.doodadPropertiesGroup.Controls.Add(this.doodadTargetBehavior);
            this.doodadPropertiesGroup.Controls.Add(this.doodadTargetBehaviorLabel);
            this.doodadPropertiesGroup.Controls.Add(this.doodadTarget);
            this.doodadPropertiesGroup.Controls.Add(this.doodadTargetLabel);

            this.doodadTypeDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            for (int i = 0; i < 52; i++)
            {
                this.doodadTypeDropdown.Items.Add((DoodadType)i);
            }
            this.doodadTypeDropdown.Location = new System.Drawing.Point(10, 10);
            this.doodadTypeDropdown.Size = new System.Drawing.Size(180, 20);
            this.doodadTypeDropdown.SelectedIndex = (int)DoodadType.PowerOrb;
            this.doodadTypeDropdown.SelectedIndexChanged += new System.EventHandler(this.doodad_change);

            this.doodadAbilityDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            for (int i = 0; i < 32; i++)
            {
                this.doodadAbilityDropdown.Items.Add((AbilityType)i);
            }
            this.doodadAbilityDropdown.Location = new System.Drawing.Point(10, 40);
            this.doodadAbilityDropdown.Size = new System.Drawing.Size(180, 20);
            this.doodadAbilityDropdown.SelectedIndex = (int)AbilityType.DoubleJump;
            this.doodadAbilityDropdown.SelectedIndexChanged += new System.EventHandler(this.doodad_change);

            this.doodadActivationCost.Location = new System.Drawing.Point(50, 65);
            this.doodadActivationCost.Size = new System.Drawing.Size(80, 20);
            this.doodadActivationCost.TextChanged+= new System.EventHandler(this.doodad_change);
            this.doodadActivationCostLabel.Location = new System.Drawing.Point(10, 65);
            this.doodadActivationCostLabel.Text = "Cost";

            this.doodadFixed.Location = new System.Drawing.Point(150, 65);
            this.doodadFixed.Text = "Fixed";
            this.doodadFixed.Checked = true;
            this.doodadFixed.CheckedChanged += new System.EventHandler(this.doodad_change);

            this.doodadTarget.Location = new System.Drawing.Point(80, 90);
            this.doodadTarget.Size = new System.Drawing.Size(130, 20);
            this.doodadTarget.TextChanged += new System.EventHandler(this.doodad_change);
            this.doodadTargetLabel.Location = new System.Drawing.Point(10, 90);
            this.doodadTargetLabel.Text = "TargetID";

            this.doodadExpectedBehavior.Location = new System.Drawing.Point(80, 115);
            this.doodadExpectedBehavior.Size = new System.Drawing.Size(130, 20);
            this.doodadExpectedBehavior.TextChanged += new System.EventHandler(this.doodad_change);
            this.doodadExpectedBehaviorLabel.Location = new System.Drawing.Point(10, 115);
            this.doodadExpectedBehaviorLabel.Text = "Expected";

            this.doodadTargetBehavior.Location = new System.Drawing.Point(80, 140);
            this.doodadTargetBehavior.Size = new System.Drawing.Size(130, 20);
            this.doodadTargetBehavior.TextChanged += new System.EventHandler(this.doodad_change);
            this.doodadTargetBehaviorLabel.Location = new System.Drawing.Point(10, 140);
            this.doodadTargetBehaviorLabel.Text = "Behavior";

            this.edgeTypeDropdown.Location = new System.Drawing.Point(10, 10);
            this.edgeTypeDropdown.Size = new System.Drawing.Size(90, 20);
            this.edgeTypeDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            for (int i = 0; i < 8; i++)
            {
                this.edgeTypeDropdown.Items.Add((EdgeType)i);
            }
            this.edgeTypeDropdown.SelectedIndexChanged+=new System.EventHandler(this.edge_change);

            #region behaviorProperties

            this.behaviorToggle.Location = new System.Drawing.Point(50, 10);
            this.behaviorToggle.Text = "On";
            this.behaviorToggle.CheckedChanged += new System.EventHandler(this.properties_data_change);
            this.behaviorToggle.Checked = true;

            this.behaviorPeriod.Location = new System.Drawing.Point(50, 35);
            this.behaviorPeriod.Text = "Period";
            this.behaviorPeriod.TextChanged+=new System.EventHandler(this.properties_data_change);
            this.behaviorPeriodLabel.Location = new System.Drawing.Point(10, 35);
            this.behaviorPeriodLabel.Text = "Period";

            this.behaviorOffset.Location = new System.Drawing.Point(50, 60);
            this.behaviorOffset.Text = "Offset";
            this.behaviorOffset.TextChanged += new System.EventHandler(this.properties_data_change);
            this.behaviorOffsetLabel.Location = new System.Drawing.Point(10, 60);
            this.behaviorOffsetLabel.Text = "Offset";

            this.behaviorDuration.Location = new System.Drawing.Point(50, 85);
            this.behaviorDuration.Text = "Duration";
            this.behaviorDuration.TextChanged += new System.EventHandler(this.properties_data_change);
            this.behaviorDurationLabel.Location = new System.Drawing.Point(10, 85);
            this.behaviorDurationLabel.Text = "Duration";

            this.behaviorNextBehavior.Location = new System.Drawing.Point(50, 110);
            this.behaviorNextBehavior.Text = "Next Behavior ID";
            this.behaviorNextBehavior.TextChanged += new System.EventHandler(this.properties_data_change);
            this.behaviorNextBehaviorLabel.Location = new System.Drawing.Point(10, 110);
            this.behaviorNextBehaviorLabel.Text = "NextID";

            this.behaviorDestinationLabel.Location = new System.Drawing.Point(10, 135);
            this.behaviorDestinationLabel.Text = "Destination";
            this.behaviorDestinationX.Location = new System.Drawing.Point(10, 155);
            this.behaviorDestinationX.Size = new System.Drawing.Size(30, 20);
            this.behaviorDestinationX.TextChanged += new System.EventHandler(this.properties_data_change);
            this.behaviorDestinationY.Location = new System.Drawing.Point(45, 155);
            this.behaviorDestinationY.Size = new System.Drawing.Size(30, 20);
            this.behaviorDestinationY.TextChanged += new System.EventHandler(this.properties_data_change);
            this.behaviorDestinationZ.Location = new System.Drawing.Point(80, 155);
            this.behaviorDestinationZ.Size = new System.Drawing.Size(30, 20);
            this.behaviorDestinationZ.TextChanged += new System.EventHandler(this.properties_data_change);

            this.behaviorPrimaryValue.Location = new System.Drawing.Point(50, 190);
            this.behaviorPrimaryValue.Text = "Primary Value";
            this.behaviorPrimaryValue.TextChanged += new System.EventHandler(this.properties_data_change);
            this.behaviorPrimaryValueLabel.Location = new System.Drawing.Point(10, 190);
            this.behaviorPrimaryValueLabel.Text = "Primary";

            this.behaviorSecondaryValue.Location = new System.Drawing.Point(50, 215);
            this.behaviorSecondaryValue.Text = "Secondary Value";
            this.behaviorSecondaryValue.TextChanged += new System.EventHandler(this.properties_data_change);
            this.behaviorSecondaryValueLabel.Location = new System.Drawing.Point(10, 215);
            this.behaviorSecondaryValueLabel.Text = "Secondary";
            #endregion

            // element properties
            this.elementGroup.Controls.Add(this.elementNameField);
            this.elementGroup.Controls.Add(this.elementIDField);
            this.elementGroup.Controls.Add(this.elementBehaviorDropdown);
            this.elementGroup.Controls.Add(this.elementBehaviorAdd);
            this.elementGroup.Controls.Add(this.behaviorNameField);
            this.elementGroup.Controls.Add(this.elementBehaviorDelete);
            this.elementGroup.Controls.Add(this.elementDelete);

            #endregion

            #region roomcontrols
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
            this.roomGroup.Location = new System.Drawing.Point(10, 180);
            this.roomGroup.Name = "roomGroup";
            this.roomGroup.Size = new System.Drawing.Size(300, 170);
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
            this.roomNameField.TextChanged += new System.EventHandler(world_rename);
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
            this.roomEdit.Location = new System.Drawing.Point(210, 140);
            this.roomEdit.Name = "roomEdit";
            this.roomEdit.Size = new System.Drawing.Size(80, 25);
            this.roomEdit.TabIndex = 32;
            this.roomEdit.Text = "Edit";
            this.roomEdit.Click += new System.EventHandler(this.world_zoom);
            // 
            // roomDelete
            // 
            this.roomDelete.Location = new System.Drawing.Point(10, 140);
            this.roomDelete.Name = "roomDelete";
            this.roomDelete.Size = new System.Drawing.Size(60, 25);
            this.roomDelete.TabIndex = 33;
            this.roomDelete.Text = "Delete";
            this.roomDelete.Click += new System.EventHandler(this.world_delete);
            #endregion

            #region facecontrols
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
            this.faceClockwise.Location = new System.Drawing.Point(10, 10);
            this.faceClockwise.Size = new System.Drawing.Size(40, 20);
            this.faceClockwise.Click += new System.EventHandler(this.room_rotate);
            this.faceCounterClockwise.Location = new System.Drawing.Point(90, 10);
            this.faceCounterClockwise.Size = new System.Drawing.Size(40, 20);
            this.faceCounterClockwise.Click += new System.EventHandler(this.room_rotate);
            

            this.viewControlsGroup.Location = new System.Drawing.Point(10, 350);
            this.viewControlsGroup.Size = new System.Drawing.Size(260, 120);
            this.viewControlsGroup.Controls.Add(this.faceDown);
            this.viewControlsGroup.Controls.Add(this.faceLeft);
            this.viewControlsGroup.Controls.Add(this.faceRight);
            this.viewControlsGroup.Controls.Add(this.faceUp);
            this.viewControlsGroup.Controls.Add(this.faceClockwise);
            this.viewControlsGroup.Controls.Add(this.faceCounterClockwise);
            this.viewControlsGroup.Controls.Add(this.modeDraw);
            this.viewControlsGroup.Controls.Add(this.modeLine);
            this.viewControlsGroup.Controls.Add(this.modePoint);
            this.viewControlsGroup.Controls.Add(this.modeDoodad);
            this.viewControlsGroup.Controls.Add(this.modeMonster);
            this.viewControlsGroup.Controls.Add(this.modeDecoration);

            #endregion

            #region modechange
            //
            // mode radio buttons
            //

            this.modeDraw.Location = new System.Drawing.Point(150, 10);
            this.modeDraw.Text = "Block Draw";
            this.modeDraw.Click += new System.EventHandler(room_mode_change);
            this.modeDraw.PerformClick();
            this.modeLine.Location = new System.Drawing.Point(150, 30);
            this.modeLine.Text = "Line Draw";
            this.modeLine.Click += new System.EventHandler(room_mode_change);
            this.modePoint.Location = new System.Drawing.Point(150, 50);
            this.modePoint.Text = "Point Draw";
            this.modePoint.Click += new System.EventHandler(room_mode_change);
            this.modeDoodad.Location = new System.Drawing.Point(150, 70);
            this.modeDoodad.Text = "Doodad";
            this.modeDoodad.Click += new System.EventHandler(room_mode_change);
            this.modeMonster.Location = new System.Drawing.Point(150, 90);
            this.modeMonster.Text = "Monster";
            this.modeMonster.Click += new System.EventHandler(room_mode_change);
            this.modeDecoration.Location = new System.Drawing.Point(50, 90);
            this.modeDecoration.Text = "Decoration";
            this.modeDecoration.Click += new System.EventHandler(room_mode_change);
            #endregion 



            #region debug
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
            #endregion

            #region mainform
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
            //this.ClientSize = new System.Drawing.Size(792, 573);
            this.ClientSize = new System.Drawing.Size(1024, 768);
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
            #endregion

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private WorldPreviewControl WorldPreviewControl;

        private System.Windows.Forms.GroupBox viewControlsGroup;
        private System.Windows.Forms.Button faceUp;
        private System.Windows.Forms.Button faceDown;
        private System.Windows.Forms.Button faceLeft;
        private System.Windows.Forms.Button faceRight;
        private System.Windows.Forms.Button faceClockwise;
        private System.Windows.Forms.Button faceCounterClockwise;

        private System.Windows.Forms.RadioButton modeDraw;
        private System.Windows.Forms.RadioButton modeLine;
        private System.Windows.Forms.RadioButton modePoint;
        private System.Windows.Forms.RadioButton modeDoodad;
        private System.Windows.Forms.RadioButton modeMonster;
        private System.Windows.Forms.RadioButton modeDecoration;

        private System.Windows.Forms.GroupBox elementGroup;
        private System.Windows.Forms.TextBox elementNameField;
        private System.Windows.Forms.TextBox elementIDField;
        private System.Windows.Forms.Button elementDelete;
        private System.Windows.Forms.ComboBox elementBehaviorDropdown;
        private System.Windows.Forms.Button elementBehaviorAdd;
        private System.Windows.Forms.Button elementBehaviorDelete;
        private System.Windows.Forms.TextBox behaviorNameField;

        private System.Windows.Forms.GroupBox edgePropertiesGroup;
        private System.Windows.Forms.ComboBox edgeTypeDropdown;

        private System.Windows.Forms.GroupBox blockPropertiesGroup;
        private System.Windows.Forms.TextBox blockColorR;
        private System.Windows.Forms.TextBox blockColorG;
        private System.Windows.Forms.TextBox blockColorB;
        private System.Windows.Forms.CheckBox blockScale;
        private System.Windows.Forms.ComboBox blockType;
        private System.Windows.Forms.TextBox blockDepth;

        private System.Windows.Forms.GroupBox doodadPropertiesGroup;
        private System.Windows.Forms.ComboBox doodadTypeDropdown;
        private System.Windows.Forms.ComboBox doodadAbilityDropdown;
        private System.Windows.Forms.CheckBox doodadFixed;
        private System.Windows.Forms.TextBox doodadActivationCost;
        private System.Windows.Forms.Label doodadActivationCostLabel;
        private System.Windows.Forms.TextBox doodadTarget;
        private System.Windows.Forms.Label doodadTargetLabel;
        private System.Windows.Forms.TextBox doodadExpectedBehavior;
        private System.Windows.Forms.Label doodadExpectedBehaviorLabel;
        private System.Windows.Forms.TextBox doodadTargetBehavior;
        private System.Windows.Forms.Label doodadTargetBehaviorLabel;

        private System.Windows.Forms.GroupBox monsterPropertiesGroup;
        private System.Windows.Forms.ComboBox monsterMovementDropdown;
        private System.Windows.Forms.ComboBox monsterArmorDropdown;
        private System.Windows.Forms.ComboBox monsterWeaponDropdown;
        private System.Windows.Forms.ComboBox monsterAIDropdown;
        private System.Windows.Forms.ComboBox monsterHealthDropdown;
        private System.Windows.Forms.ComboBox monsterSpeedDropdown;
        private System.Windows.Forms.ComboBox monsterSizeDropdown;
        private System.Windows.Forms.ComboBox monsterTrackingDropdown;
        private System.Windows.Forms.CheckBox monsterFixedPath;
        private System.Windows.Forms.TextBox monsterWaypointID;
        private System.Windows.Forms.Label monsterWaypointIDLabel;

        private System.Windows.Forms.GroupBox decorationPropertiesGroup;
        private System.Windows.Forms.TextBox decorationTexture;
        private System.Windows.Forms.TextBox decorationDepth;
        private System.Windows.Forms.TextBox decorationR;
        private System.Windows.Forms.TextBox decorationG;
        private System.Windows.Forms.TextBox decorationB;
        private System.Windows.Forms.CheckBox decorationWrap;
        private System.Windows.Forms.CheckBox decorationSpin;
        private System.Windows.Forms.TextBox decorationStartFrame;        

        private System.Windows.Forms.GroupBox behaviorPropertiesGroup;
        private System.Windows.Forms.CheckBox behaviorToggle;
        private System.Windows.Forms.TextBox behaviorOffset;
        private System.Windows.Forms.Label behaviorOffsetLabel;
        private System.Windows.Forms.TextBox behaviorPeriod;
        private System.Windows.Forms.Label behaviorPeriodLabel;
        private System.Windows.Forms.TextBox behaviorDuration;
        private System.Windows.Forms.Label behaviorDurationLabel;
        private System.Windows.Forms.TextBox behaviorNextBehavior;
        private System.Windows.Forms.Label behaviorNextBehaviorLabel;
        private System.Windows.Forms.Label behaviorDestinationLabel;
        private System.Windows.Forms.TextBox behaviorDestinationX;
        private System.Windows.Forms.TextBox behaviorDestinationY;
        private System.Windows.Forms.TextBox behaviorDestinationZ;
        private System.Windows.Forms.TextBox behaviorPrimaryValue;
        private System.Windows.Forms.Label behaviorPrimaryValueLabel;
        private System.Windows.Forms.TextBox behaviorSecondaryValue;
        private System.Windows.Forms.Label behaviorSecondaryValueLabel;
        
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
        private System.Windows.Forms.Button undo;
        public System.Windows.Forms.TextBox debug1, debug2, debug3, debug4;
    }
}

