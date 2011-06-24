#region File Description
//-----------------------------------------------------------------------------
// MainForm.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
#endregion

namespace WinFormsGraphicsDevice
{
    // System.Drawing and the XNA Framework both define Color types.
    // To avoid conflicts, we define shortcut names for them both.
    using GdiColor = System.Drawing.Color;
    using XnaColor = Microsoft.Xna.Framework.Color;
        
    /// <summary>
    /// Custom form provides the main user interface for the program.
    /// In this sample we used the designer to add a splitter pane to the form,
    /// which contains a SpriteFontControl and a WorldPreviewControl.
    /// </summary>
    public partial class MainForm : Form
    {
        
        public MainForm()
        {
            InitializeComponent();
            editor_clear(null, null);
        }

        void editor_clear(object sender, System.EventArgs e)
        {
            currentFileName = null;
            world = new World();
            Sector s = new Sector();
            world.sectors.Add(s);
            Room r = new Room();
            s.rooms.Add(r);
            this.sectorDropdown.Items.Clear();
            this.roomDropdown.Items.Clear();
            this.sectorDropdown.Items.Add(s.IDString);
            this.sectorDropdown.SelectedIndex = 0;
        }

        void editor_save(object sender, System.EventArgs e)
        {
            if (currentFileName == null)
                editor_saveAs(sender, e);
            else
            {                
                Stream currentStream = new FileStream(currentFileName, FileMode.Create);
                XmlSerializer serializer = new XmlSerializer(typeof(World));
                serializer.Serialize(currentStream, world);
                // Code to write the stream goes here.
                currentStream.Close();
            }

        }

        void editor_saveAs(object sender, System.EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            Stream currentStream;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((currentStream = saveFileDialog1.OpenFile()) != null)
                {
                    currentFileName = saveFileDialog1.FileName;
                    XmlSerializer serializer = new XmlSerializer(typeof(World));
                    serializer.Serialize(currentStream, world);
                    // Code to write the stream goes here.
                    currentStream.Close();
                }
            }

        }

        void editor_change_speed(object sender, System.EventArgs e)
        {
            if (sender == this.speedSlider)
            {
                // pc = .03 -> .001
                // laptop = .15f -> .005
                animateSpeed = .001f + (.005f* this.speedSlider.Value / this.speedSlider.Maximum);
            }
        }

        void world_zoom(object sender, System.EventArgs e)
        {
            if (sender == this.sectorView)
            {
                this.WorldPreviewControl.ViewWorld();
                zoomRoom = null;
            }
            else
            {
                Sector s = world.FindSectorByIDString((string)this.sectorDropdown.Items[this.sectorDropdown.SelectedIndex]);
                Room r = s.FindRoomByIDString((string)this.roomDropdown.Items[this.roomDropdown.SelectedIndex]);
                zoomRoom = r;
                Face f = r.faceList[0];
                this.WorldPreviewControl.ViewFace(f);
                selectedFace = f;
                currentUp = Vector3.UnitZ;
            }
        }

        void room_rotate(object sender, System.EventArgs e)
        {
            if (MainForm.zoomRoom != null && MainForm.cameraReady == true)
            {
                if (sender == this.faceUp)
                {
                    Vector3 nextNormal = -MainForm.currentUp;
                    MainForm.currentUp = MainForm.selectedFace.normal;
                    this.WorldPreviewControl.FindFace(nextNormal, currentUp);
                }
                if (sender == this.faceDown)
                {
                    Vector3 nextNormal = MainForm.currentUp;
                    MainForm.currentUp = -MainForm.selectedFace.normal;
                    this.WorldPreviewControl.FindFace(nextNormal, currentUp);
                }
                if (sender == this.faceLeft)
                {
                    Vector3 nextNormal = -Vector3.Cross(MainForm.selectedFace.normal, MainForm.currentUp);
                    this.WorldPreviewControl.FindFace(nextNormal, currentUp);
                }
                if (sender == this.faceRight)
                {
                    Vector3 nextNormal = Vector3.Cross(MainForm.selectedFace.normal, MainForm.currentUp);
                    this.WorldPreviewControl.FindFace(nextNormal, currentUp);
                }
            }
        }

        void editor_load(object sender, System.EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            Stream currentStream;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if ((currentStream = openFileDialog.OpenFile()) != null)
                {
                    currentFileName = openFileDialog.FileName;
                    XmlSerializer serializer = new XmlSerializer(typeof(World));
                    world = (World)serializer.Deserialize(currentStream);
                    // Code to write the stream goes here.
                    currentStream.Close();

                    this.sectorDropdown.Items.Clear();
                    foreach (Sector s in world.sectors)
                    {
                        this.sectorDropdown.Items.Add(s.IDString);
                    }
                    this.sectorDropdown.SelectedIndex = 0;                    
                }
            }
        }

        void world_mouse_hover(object sender, System.EventArgs e)
        {
            Sector s = world.FindSectorByIDString((string)this.sectorDropdown.Items[this.sectorDropdown.SelectedIndex]);
            Room r = s.FindRoomByIDString((string)this.roomDropdown.Items[this.roomDropdown.SelectedIndex]);

            if (sender == this.sectorGroup)
            {
                selectedSector = s;
                selectedRoom = null;
            }
            if (sender == this.roomGroup)
            {
                selectedSector = null;
                selectedRoom = r;
            }
        }

        void world_mouse_leave(object sender, System.EventArgs e)
        {
            selectedSector = null;
            selectedRoom = null;            
        }

        void world_selected_change(object sender, System.EventArgs e)
        {
            if (sender == this.sectorDropdown)
            {
                Sector s = world.FindSectorByIDString((string)this.sectorDropdown.Items[this.sectorDropdown.SelectedIndex]);
                this.sectorNameField.Text = s.name;

                this.roomDropdown.Items.Clear();
                foreach (Room r in s.rooms)
                {
                    this.roomDropdown.Items.Add(r.IDString);
                }
                this.roomDropdown.SelectedIndex = 0;
                selectedSector = s;
            }
            if (sender == this.roomDropdown)
            {
                Sector s = world.FindSectorByIDString((string)this.sectorDropdown.Items[this.sectorDropdown.SelectedIndex]);
                Room r = s.FindRoomByIDString((string)this.roomDropdown.Items[this.roomDropdown.SelectedIndex]);
                this.roomNameField.Text = r.name;
                this.roomCenterX.Text = r.centerX.ToString();
                this.roomCenterY.Text = r.centerY.ToString();
                this.roomCenterZ.Text = r.centerZ.ToString();
                this.roomSizeX.Text = r.sizeX.ToString();
                this.roomSizeY.Text = r.sizeY.ToString();
                this.roomSizeZ.Text = r.sizeZ.ToString();
                this.roomColorR.Text = r.color.R.ToString();
                this.roomColorG.Text = r.color.G.ToString();
                this.roomColorB.Text = r.color.B.ToString();
                selectedRoom = r;
            }
        }

        void world_create_new(object sender, System.EventArgs e)
        {
            if (sender == this.sectorNewButton)
            {
                Sector s = new Sector();                
                world.sectors.Add(s);
                Room r = new Room();
                s.rooms.Add(r);
                this.sectorDropdown.Items.Add(s.IDString);
                this.sectorDropdown.SelectedIndex = this.sectorDropdown.Items.Count - 1;
            }
            if (sender == this.roomNewButton)
            {
                Room r = new Room();
                Sector s = world.FindSectorByIDString((string)this.sectorDropdown.Items[this.sectorDropdown.SelectedIndex]);
                s.rooms.Add(r);
                this.roomDropdown.Items.Add(r.IDString);
                this.roomDropdown.SelectedIndex = this.roomDropdown.Items.Count - 1;
            }
            if (sender == this.elementBehaviorAdd)
            {
                if (MainForm.editMode == EditMode.BlockSelect)
                {
                    Block block = MainForm.selectedBlock;
                    Behavior b = new Behavior();
                    block.behaviors.Add(b);
                    this.elementBehaviorDropdown.Items.Add(b.IDString);
                    this.elementBehaviorDropdown.SelectedIndex = this.elementBehaviorDropdown.Items.Count - 1;

                }
                if (MainForm.editMode == EditMode.LineSelect)
                {
                    Edge edge = MainForm.selectedEdge;
                    Behavior b = new Behavior();
                    edge.behaviors.Add(b);
                    this.elementBehaviorDropdown.Items.Add(b.IDString);
                    this.elementBehaviorDropdown.SelectedIndex = this.elementBehaviorDropdown.Items.Count - 1;
                }
                if (MainForm.editMode == EditMode.Doodad)
                {
                    Doodad d = MainForm.selectedDoodad;
                    Behavior b = new Behavior();
                    d.behaviors.Add(b);
                    this.elementBehaviorDropdown.Items.Add(b.IDString);
                    this.elementBehaviorDropdown.SelectedIndex = this.elementBehaviorDropdown.Items.Count - 1;
                }
            }
        }

        public void update_element_data()
        {
            if (selectedMonster != null && MainForm.editMode == EditMode.Monster)
            {
                this.elementGroup.Visible = true;
                this.edgePropertiesGroup.Visible = false;
                this.behaviorPropertiesGroup.Visible = false;
                this.doodadPropertiesGroup.Visible = false;
                this.monsterPropertiesGroup.Visible = true;
                
                this.elementNameField.Text = selectedMonster.name;
                this.elementIDField.Text = selectedMonster.IDString;
                this.elementBehaviorDropdown.Items.Clear();
                this.behaviorNameField.Text = "";

                this.monsterMovementDropdown.SelectedIndex = (int)selectedMonster.movement;
                this.monsterArmorDropdown.SelectedIndex = (int)selectedMonster.armor;
                this.monsterAIDropdown.SelectedIndex = (int)selectedMonster.behavior;
                this.monsterWeaponDropdown.SelectedIndex = (int)selectedMonster.weapon;
                this.monsterWaypointID.Text = selectedMonster.waypointId;
                this.monsterFixedPath.Checked = selectedMonster.fixedPath;
            }
            else if (selectedDoodad != null && MainForm.editMode == EditMode.Doodad)
            {
                this.elementGroup.Visible = true;
                this.edgePropertiesGroup.Visible = false;
                this.behaviorPropertiesGroup.Visible = true;
                this.doodadPropertiesGroup.Visible = true;
                this.elementNameField.Text = selectedDoodad.name;
                this.elementIDField.Text = selectedDoodad.IDString;
                this.elementBehaviorDropdown.Items.Clear();
                foreach (Behavior b in selectedDoodad.behaviors)
                {
                    this.elementBehaviorDropdown.Items.Add(b.IDString);
                }
                this.elementBehaviorDropdown.SelectedIndex = 0;
                this.behaviorNameField.Text = selectedDoodad.FindBehaviorByIDString((string)this.elementBehaviorDropdown.Items[this.elementBehaviorDropdown.SelectedIndex]).name;

                this.doodadAbilityDropdown.SelectedIndex = (int)selectedDoodad.ability;
                this.doodadTypeDropdown.SelectedIndex = (int)selectedDoodad.type;
                this.doodadFixed.Checked = selectedDoodad.fixedPosition;
                this.doodadTarget.Text = selectedDoodad.targetObject;
                this.doodadTargetBehavior.Text = selectedDoodad.targetBehavior;
                this.doodadExpectedBehavior.Text = selectedDoodad.expectBehavior;
                this.doodadActivationCost.Text = selectedDoodad.activationCost.ToString();
            }
            else if (selectedEdge != null && MainForm.editMode == EditMode.LineSelect)
            {
                this.elementGroup.Visible = true;
                this.edgePropertiesGroup.Visible = true;
                this.behaviorPropertiesGroup.Visible = true;
                this.doodadPropertiesGroup.Visible = false;
                this.elementNameField.Text = selectedEdge.name;
                this.elementIDField.Text = selectedEdge.IDString;
                this.elementBehaviorDropdown.Items.Clear();
                foreach (Behavior b in selectedEdge.behaviors)
                {
                    this.elementBehaviorDropdown.Items.Add(b.IDString);
                }
                this.edgeTypeDropdown.SelectedIndex = (int)selectedEdge.type;
                this.elementBehaviorDropdown.SelectedIndex = 0;
                this.behaviorNameField.Text = selectedEdge.FindBehaviorByIDString((string)this.elementBehaviorDropdown.Items[this.elementBehaviorDropdown.SelectedIndex]).name;
            }
            else if (selectedBlock != null && MainForm.editMode == EditMode.BlockSelect)
            {
                this.elementGroup.Visible = true;
                this.edgePropertiesGroup.Visible = false;
                this.behaviorPropertiesGroup.Visible = true;
                this.doodadPropertiesGroup.Visible = false;
                this.elementNameField.Text = selectedBlock.name;
                this.elementIDField.Text = selectedBlock.IDString;
                this.elementBehaviorDropdown.Items.Clear();
                foreach (Behavior b in selectedBlock.behaviors)
                {
                    this.elementBehaviorDropdown.Items.Add(b.IDString);
                }
                this.elementBehaviorDropdown.SelectedIndex = 0;
                this.behaviorNameField.Text = selectedBlock.FindBehaviorByIDString((string)this.elementBehaviorDropdown.Items[this.elementBehaviorDropdown.SelectedIndex]).name;                
            }
            else
            {
                this.edgePropertiesGroup.Visible = false;
                this.elementGroup.Visible = false;
                this.behaviorPropertiesGroup.Visible = false;
                this.doodadPropertiesGroup.Visible = false;
                this.monsterPropertiesGroup.Visible = false;
            }
        }

        void behavior_select(object sender, System.EventArgs e)
        {
            if (sender == elementBehaviorDropdown)
            {
                if (editMode == EditMode.BlockSelect)
                {
                    update_behavior_data(MainForm.selectedBlock.FindBehaviorByIDString((string)this.elementBehaviorDropdown.Items[this.elementBehaviorDropdown.SelectedIndex]));                    
                }
                if (editMode == EditMode.LineSelect)
                {
                    update_behavior_data(MainForm.selectedEdge.FindBehaviorByIDString((string)this.elementBehaviorDropdown.Items[this.elementBehaviorDropdown.SelectedIndex]));
                }
                if (editMode == EditMode.Doodad)
                {
                    update_behavior_data(MainForm.selectedDoodad.FindBehaviorByIDString((string)this.elementBehaviorDropdown.Items[this.elementBehaviorDropdown.SelectedIndex]));
                }
            }
        }

        public void update_behavior_data(Behavior b)
        {
            this.behaviorToggle.Checked = b.toggle;
            this.behaviorPrimaryValue.Text = b.primaryValue.ToString();
            this.behaviorSecondaryValue.Text = b.secondaryValue.ToString();
            this.behaviorOffset.Text = b.offset.ToString();
            this.behaviorPeriod.Text = b.period.ToString();
            this.behaviorDuration.Text = b.duration.ToString();
            this.behaviorNextBehavior.Text = b.nextBehavior;
            this.behaviorDestinationX.Text = b.destination.X.ToString();
            this.behaviorDestinationY.Text = b.destination.Y.ToString();
            this.behaviorDestinationZ.Text = b.destination.Z.ToString();
        }

        void room_mode_change(object sender, System.EventArgs e)
        {
            if (sender == this.modeDraw)
            {
                editMode = EditMode.Block;   
            }
            if (sender == this.modeLine)
            {
                editMode = EditMode.Line;
            }
            if (sender == this.modePoint)
            {
                editMode = EditMode.Point;
            }
            if (sender == this.modeEdgeSelect)
            {
                editMode = EditMode.LineSelect;
            }
            if (sender == this.modeBlockSelect)
            {
                editMode = EditMode.BlockSelect;
            }
            if (sender == this.modeDoodad)
            {
                editMode = EditMode.Doodad;
            }
            if (sender == this.modeMonster)
            {
                editMode = EditMode.Monster;
            }
        }

        void world_highlight_room(object sender, System.EventArgs e)
        {
            if (sender == this.roomDropdown)
            {

            }
        }

        void world_value_increment(object sender, System.EventArgs e)
        {
            Sector s = world.FindSectorByIDString((string)this.sectorDropdown.Items[this.sectorDropdown.SelectedIndex]);
            Room r = s.FindRoomByIDString((string)this.roomDropdown.Items[this.roomDropdown.SelectedIndex]);

            if (sender == this.roomColorBUp)            
                r.color.B += 25;
            if (sender == this.roomColorBDown)
                r.color.B -= 25;
            if (sender == this.roomColorGUp)
                r.color.G += 25;
            if (sender == this.roomColorGDown)
                r.color.G -= 25;
            if (sender == this.roomColorRUp)
                r.color.R += 25;
            if (sender == this.roomColorRDown)
                r.color.R -= 25;
            if (sender == this.roomCenterXUp)
                r.Move(new Vector3(15,0,0));
            if (sender == this.roomCenterXDown)
                r.Move(new Vector3(-15, 0, 0));
            if (sender == this.roomCenterYUp)
                r.Move(new Vector3(0, 15, 0));
            if (sender == this.roomCenterYDown)
                r.Move(new Vector3(0, -15, 0));
            if (sender == this.roomCenterZUp)
                r.Move(new Vector3(0, 0, 15));
            if (sender == this.roomCenterZDown)
                r.Move(new Vector3(0, 0, -15));
            if (sender == this.roomSizeXUp)
                r.Resize(new Vector3(10, 0, 0));
            if (sender == this.roomSizeXDown)
                r.Resize(new Vector3(-10, 0, 0));
            if (sender == this.roomSizeYUp)
                r.Resize(new Vector3(0, 10, 0));
            if (sender == this.roomSizeYDown)
                r.Resize(new Vector3(0, -10, 0));
            if (sender == this.roomSizeZUp)
                r.Resize(new Vector3(0, 0, 10));
            if (sender == this.roomSizeZDown)
                r.Resize(new Vector3(0, 0, -10));

            

            this.roomColorR.Text = r.color.R.ToString();
            this.roomColorG.Text = r.color.G.ToString();
            this.roomColorB.Text = r.color.B.ToString();
            this.roomCenterX.Text = r.centerX.ToString();
            this.roomCenterY.Text = r.centerY.ToString();
            this.roomCenterZ.Text = r.centerZ.ToString();
            this.roomSizeX.Text = r.sizeX.ToString();
            this.roomSizeY.Text = r.sizeY.ToString();
            this.roomSizeZ.Text = r.sizeZ.ToString();
            
        }

        void edge_change(object sender, System.EventArgs e)
        {
            if (sender == this.edgeTypeDropdown)
            {
                selectedEdge.type = (EdgeType)this.edgeTypeDropdown.SelectedIndex;                
            }
        }

        void monster_change(object sender, System.EventArgs e)
        {
            if (sender == this.monsterFixedPath)
            {
                selectedMonster.fixedPath = this.monsterFixedPath.Checked;
            }
            if (sender == this.monsterWaypointID)
            {
                selectedMonster.waypointId = this.monsterWaypointID.Text;
            }
            if (sender == this.monsterMovementDropdown)
            {
                selectedMonster.movement = (MovementType)this.monsterMovementDropdown.SelectedIndex;
            }
            if (sender == this.monsterArmorDropdown)
            {
                selectedMonster.armor = (ArmorType)this.monsterArmorDropdown.SelectedIndex;
            }
            if (sender == this.monsterAIDropdown)
            {
                selectedMonster.behavior = (AIType)this.monsterAIDropdown.SelectedIndex;
            }
            if (sender == this.monsterWeaponDropdown)
            {
                selectedMonster.weapon = (GunType)this.monsterWeaponDropdown.SelectedIndex;
            }
        }

        void doodad_change(object sender, System.EventArgs e)
        {
            if (sender == this.doodadFixed)
            {
                selectedDoodad.fixedPosition = this.doodadFixed.Checked;
                if (this.doodadFixed.Checked)
                    this.doodadFixed.Text = "Fixed";
                else
                    this.doodadFixed.Text = "Free";
            }
            if (sender == this.doodadTypeDropdown)
            {
                selectedDoodad.type = (DoodadType)this.doodadTypeDropdown.SelectedIndex;
            }
            if (sender == this.doodadAbilityDropdown)
            {
                selectedDoodad.ability = (AbilityType)this.doodadAbilityDropdown.SelectedIndex;
            }
            if (sender == this.doodadActivationCost)
            {
                selectedDoodad.activationCost = System.Convert.ToInt32(this.doodadActivationCost.Text);
            }
            if (sender == this.doodadExpectedBehavior)
            {
                selectedDoodad.expectBehavior = this.doodadExpectedBehavior.Text;
            }
            if (sender == this.doodadTarget)
            {
                selectedDoodad.targetObject = this.doodadTarget.Text;
            }
            if (sender == this.doodadTargetBehavior)
            {
                selectedDoodad.targetBehavior = this.doodadTargetBehavior.Text;
            }
        }

        void world_rename(object sender, System.EventArgs e)
        {
            if (sender == this.sectorNameField)
            {
                Sector s = world.FindSectorByIDString((string)this.sectorDropdown.Items[this.sectorDropdown.SelectedIndex]);
                s.name = this.sectorNameField.Text;
                this.sectorDropdown.Items[this.sectorDropdown.SelectedIndex] = s.IDString;
            }
            if (sender == this.roomNameField)
            {
                Sector s = world.FindSectorByIDString((string)this.sectorDropdown.Items[this.sectorDropdown.SelectedIndex]);
                Room r = s.FindRoomByIDString((string)this.roomDropdown.Items[this.roomDropdown.SelectedIndex]);
                r.name = this.roomNameField.Text;
                this.roomDropdown.Items[this.roomDropdown.SelectedIndex] = r.IDString;
            }
            if (sender == this.elementNameField)
            {
                if (MainForm.editMode == EditMode.BlockSelect)
                {
                    Block b = MainForm.selectedBlock;
                    b.name = this.elementNameField.Text;
                    this.elementIDField.Text = b.IDString;
                }
                if (MainForm.editMode == EditMode.LineSelect)
                {
                    Edge edge = MainForm.selectedEdge;
                    edge.name = this.elementNameField.Text;
                    this.elementIDField.Text = edge.IDString;
                }
                if (MainForm.editMode == EditMode.Doodad)
                {
                    Doodad d = MainForm.selectedDoodad;
                    d.name = this.elementNameField.Text;
                    this.elementIDField.Text = d.IDString;
                }
                if (MainForm.editMode == EditMode.Monster)
                {
                    Monster m = MainForm.selectedMonster;
                    m.name = this.elementNameField.Text;
                    this.elementIDField.Text = m.IDString;
                }
            }
            if (sender == this.behaviorNameField)
            {
                if (MainForm.editMode == EditMode.BlockSelect)
                {
                    Behavior b = MainForm.selectedBlock.FindBehaviorByIDString((string)this.elementBehaviorDropdown.Items[this.elementBehaviorDropdown.SelectedIndex]);
                    b.name = this.behaviorNameField.Text;
                    this.elementBehaviorDropdown.Items[this.elementBehaviorDropdown.SelectedIndex] = b.IDString;
                }
                if (MainForm.editMode == EditMode.LineSelect)
                {
                    Behavior b = MainForm.selectedEdge.FindBehaviorByIDString((string)this.elementBehaviorDropdown.Items[this.elementBehaviorDropdown.SelectedIndex]);
                    b.name = this.behaviorNameField.Text;
                    this.elementBehaviorDropdown.Items[this.elementBehaviorDropdown.SelectedIndex] = b.IDString;
                }
                if (MainForm.editMode == EditMode.Doodad)
                {
                    Behavior b = MainForm.selectedDoodad.FindBehaviorByIDString((string)this.elementBehaviorDropdown.Items[this.elementBehaviorDropdown.SelectedIndex]);
                    b.name = this.behaviorNameField.Text;
                    this.elementBehaviorDropdown.Items[this.elementBehaviorDropdown.SelectedIndex] = b.IDString;
                }
            }
        }

        void world_data_change(object sender, System.EventArgs e)
        {
            Sector s = world.FindSectorByIDString((string)this.sectorDropdown.Items[this.sectorDropdown.SelectedIndex]);
            Room r = s.FindRoomByIDString((string)this.roomDropdown.Items[this.roomDropdown.SelectedIndex]);

            try
            {
                if (sender == this.roomCenterX)
                {
                    r.centerX = System.Convert.ToInt32(this.roomCenterX.Text);
                }
                if (sender == this.roomCenterY)
                {
                    r.centerY = System.Convert.ToInt32(this.roomCenterY.Text);
                }
                if (sender == this.roomCenterZ)
                {
                    r.centerZ = System.Convert.ToInt32(this.roomCenterZ.Text);
                }
                if (sender == this.roomSizeX)
                {
                    r.sizeX = System.Convert.ToInt32(this.roomSizeX.Text);
                }
                if (sender == this.roomSizeY)
                {
                    r.sizeY = System.Convert.ToInt32(this.roomSizeY.Text);
                }
                if (sender == this.roomSizeZ)
                {
                    r.sizeZ = System.Convert.ToInt32(this.roomSizeZ.Text);
                }
                if (sender == this.roomColorR)
                {
                    r.color.R = System.Convert.ToByte(this.roomColorR.Text);
                }
                if (sender == this.roomColorG)
                {
                    r.color.G = System.Convert.ToByte(this.roomColorG.Text);
                }
                if (sender == this.roomColorB)
                {
                    r.color.B = System.Convert.ToByte(this.roomColorB.Text);
                }
            }
            catch
            {
            }
        }

        void properties_data_change(object sender, System.EventArgs e)
        {
            Behavior b = null;
            if (MainForm.editMode == EditMode.BlockSelect)
            {
                b = MainForm.selectedBlock.FindBehaviorByIDString((string)this.elementBehaviorDropdown.Items[this.elementBehaviorDropdown.SelectedIndex]);
            }
            if (MainForm.editMode == EditMode.LineSelect)
            {
                b = MainForm.selectedEdge.FindBehaviorByIDString((string)this.elementBehaviorDropdown.Items[this.elementBehaviorDropdown.SelectedIndex]);
            }

            try
            {
                if (sender == this.behaviorToggle)
                {
                    b.toggle = this.behaviorToggle.Checked;
                    if (this.behaviorToggle.Checked)
                        this.behaviorToggle.Text = "On";
                    else
                        this.behaviorToggle.Text = "Off";
                }
                if (sender == this.behaviorOffset)
                {
                    b.offset = System.Convert.ToInt32(this.behaviorOffset.Text);
                }
                if (sender == this.behaviorPeriod)
                {
                    b.period = System.Convert.ToInt32(this.behaviorPeriod.Text);
                }
                if (sender == this.behaviorDuration)
                {
                    b.duration = System.Convert.ToInt32(this.behaviorDuration.Text);
                }
                if (sender == this.behaviorNextBehavior)
                {
                    b.nextBehavior = this.behaviorNextBehavior.Text;
                }
                if (sender == this.behaviorPrimaryValue)
                {
                    b.primaryValue = System.Convert.ToInt32(this.behaviorPrimaryValue.Text);
                }
                if (sender == this.behaviorSecondaryValue)
                {
                    b.secondaryValue = System.Convert.ToInt32(this.behaviorSecondaryValue.Text);
                }
                if (sender == this.behaviorDestinationX)
                {
                    b.destination.X = System.Convert.ToInt32(this.behaviorDestinationX.Text);
                }
                if (sender == this.behaviorDestinationY)
                {
                    b.destination.Y = System.Convert.ToInt32(this.behaviorDestinationY.Text);
                }
                if (sender == this.behaviorDestinationZ)
                {
                    b.destination.Z = System.Convert.ToInt32(this.behaviorDestinationZ.Text);
                }
            }
            catch
            {
            }

        }
    }
}
