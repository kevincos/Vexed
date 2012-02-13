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
using VL;
using System.Runtime.Serialization.Formatters.Binary;
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

        void editor_undo(object sender, System.EventArgs e)
        {
            if (undoWorld != null)
            {
                world = undoWorld;
                foreach (Sector s in world.sectors)
                {
                    foreach (Room r in s.rooms)
                    {
                        foreach (Face f in r.faceList)
                        {
                            if (selectedFace != null && f.center == selectedFace.center && f.normal == selectedFace.normal)
                                selectedFace = f;
                            foreach (Monster m in f.monsters)
                            {
                                if (selectedMonster != null && m.id == selectedMonster.id)
                                    selectedMonster = m;
                            }
                            foreach (Doodad d in f.doodads)
                            {
                                if (selectedDoodad != null && d.id == selectedDoodad.id)
                                    selectedDoodad = d;
                            }
                            foreach (Block b in f.blocks)
                            {
                                if (selectedBlock != null && b.id == selectedBlock.id)
                                    selectedBlock = b;
                                foreach (Edge edge in b.edges)
                                {
                                    if (selectedEdge != null && edge.id == selectedEdge.id)
                                        selectedEdge = edge;
                                }
                            }
                        }
                    }
                }
                undoWorld = null;                
            }
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

        void editor_update_file(object sender, System.EventArgs e)
        {
            foreach (Sector s in world.sectors)
            {
                s.Update();
                foreach (Room r in s.rooms)
                {
                    r.Update();
                    foreach (Face f in r.faceList)
                    {
                        foreach (Doodad d in f.doodads)
                        {
                            d.Update();
                            foreach (Behavior behavior in d.behaviors)
                            {
                                behavior.Update();
                            }
                        }
                        foreach (Decoration d in f.decorations)
                        {
                            d.Update();
                        }
                        foreach (Monster m in f.monsters)
                        {
                            m.Update();
                        }
                        foreach (Block b in f.blocks)
                        {
                            b.Update();
                            foreach (Behavior behavior in b.behaviors)
                            {
                                behavior.Update();
                            }
                            foreach (Edge edge in b.edges)
                            {
                                edge.Update();
                                foreach (Behavior behavior in edge.behaviors)
                                {
                                    behavior.Update();
                                }
                            }
                        }
                    }
                }
            }
        }

        void editor_save(object sender, System.EventArgs e)
        {
            editor_update_file(sender, e);
            if (currentFileName == null)
                editor_saveAs(sender, e);
            else
            {                
                Stream currentStream = new FileStream(currentFileName, FileMode.Create);
                XmlSerializer serializer = new XmlSerializer(typeof(World));
                serializer.Serialize(currentStream, world);
                // Code to write the stream goes here.
                currentStream.Close();
                Stream altStream = new FileStream(currentFileName+"_binaryBlob.txt", FileMode.Create);
            }

        }

        void editor_saveAs(object sender, System.EventArgs e)
        {
            editor_update_file(sender, e);
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
                animateSpeed = .01f + (.05f* this.speedSlider.Value / this.speedSlider.Maximum);
            }
        }

        void world_zoom(object sender, System.EventArgs e)
        {
            if (sender == this.sectorView)
            {
                this.WorldPreviewControl.ViewWorld();
                selectedBlock = null;
                selectedFace = null;
                selectedEdge = null;
                this.behaviorPropertiesGroup.Visible = false;
                this.edgePropertiesGroup.Visible = false;
                this.blockPropertiesGroup.Visible = false;
                this.monsterPropertiesGroup.Visible = false;
                this.doodadPropertiesGroup.Visible = false;
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
                if (sender == this.faceClockwise)
                {
                    MainForm.currentUp = -Vector3.Cross(MainForm.selectedFace.normal, MainForm.currentUp);
                    this.WorldPreviewControl.FindFace(MainForm.selectedFace.normal, currentUp);
                }
                if (sender == this.faceCounterClockwise)
                {
                    MainForm.currentUp = Vector3.Cross(MainForm.selectedFace.normal, MainForm.currentUp);
                    this.WorldPreviewControl.FindFace(MainForm.selectedFace.normal, currentUp);
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
            /*Sector s = world.FindSectorByIDString((string)this.sectorDropdown.Items[this.sectorDropdown.SelectedIndex]);
            Room r = s.FindRoomByIDString((string)this.roomDropdown.Items[this.roomDropdown.SelectedIndex]);

            if (sender == this.sectorGroup)
            {
                selectedSector = s;
                selectedRoom = null;
            }
            if (sender == this.roomGroup)
            {
                selectedSector = s;
                selectedRoom = r;
            }*/
        }

        void world_mouse_leave(object sender, System.EventArgs e)
        {
            //selectedSector = null;
            //selectedRoom = null;            
        }

        void world_selected_change(object sender, System.EventArgs e)
        {
            if (sender == this.sectorDropdown)
            {
                Sector s = world.FindSectorByIDString((string)this.sectorDropdown.Items[this.sectorDropdown.SelectedIndex]);
                this.sectorNameField.Text = s.IDString;

                this.roomDropdown.Items.Clear();
                foreach (Room r in s.rooms)
                {
                    this.roomDropdown.Items.Add(r.IDString);
                }
                this.roomDropdown.SelectedIndex = 0;
                
                selectedSector = s;
                selectedSector.UpdateSectorCenter();
            }
            if (sender == this.roomDropdown)
            {
                Sector s = world.FindSectorByIDString((string)this.sectorDropdown.Items[this.sectorDropdown.SelectedIndex]);
                Room r = s.FindRoomByIDString((string)this.roomDropdown.Items[this.roomDropdown.SelectedIndex]);
                this.roomNameField.Text = r.IDString;
                this.roomFriendlyNameField.Text = r.friendlyName;
                this.roomDecalDropdown.SelectedIndex = (int)r.decal;
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
                Room r = new Room(selectedSector.center);
                Sector s = world.FindSectorByIDString((string)this.sectorDropdown.Items[this.sectorDropdown.SelectedIndex]);
                s.rooms.Add(r);
                this.roomDropdown.Items.Add(r.IDString);
                this.roomDropdown.SelectedIndex = this.roomDropdown.Items.Count - 1;
            }
            if (sender == this.elementBehaviorAdd)
            {
                if (MainForm.editMode == EditMode.Block)
                {
                    Block block = MainForm.selectedBlock;
                    Behavior b = new Behavior();
                    block.behaviors.Add(b);
                    this.elementBehaviorDropdown.Items.Add(b.IDString);
                    this.elementBehaviorDropdown.SelectedIndex = this.elementBehaviorDropdown.Items.Count - 1;

                }
                if (MainForm.editMode == EditMode.Line)
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

        void world_copy(object sender, System.EventArgs e)
        {
            if (sender == this.elementCopy)
            {
                if (editMode == EditMode.Monster)
                {
                    undoWorld = new World(world);
                    Monster newMonster = new Monster(selectedMonster);
                    newMonster.Init();                   
                    selectedFace.monsters.Add(newMonster);
                }
                if (editMode == EditMode.Block)
                {
                    undoWorld = new World(world);
                    Block newBlock = new Block(selectedBlock);
                    newBlock.Init();
                    selectedFace.blocks.Add(newBlock);
                }
                if (editMode == EditMode.Doodad)
                {
                    undoWorld = new World(world);
                    Doodad newDoodad = new Doodad(selectedDoodad);
                    newDoodad.Init();
                    selectedFace.doodads.Add(newDoodad);
                }
                if (editMode == EditMode.Decoration)
                {
                    undoWorld = new World(world);
                    Decoration newDecoration = new Decoration(selectedDecoration);
                    newDecoration.Init();
                    selectedFace.decorations.Add(newDecoration);
                }
            }
        }


        void world_delete(object sender, System.EventArgs e)
        {
            if (sender == this.sectorDelete)
            {
                if (world.sectors.Count > 1)
                {
                    undoWorld = new World(world);
                    Sector s = world.FindSectorByIDString((string)this.sectorDropdown.Items[this.sectorDropdown.SelectedIndex]);
                    world.sectors.Remove(s);
                    this.sectorDropdown.Items.RemoveAt(this.sectorDropdown.SelectedIndex);
                    this.sectorDropdown.SelectedIndex = 0;
                }
            }
            if (sender == this.roomDelete)
            {
                Sector s = world.FindSectorByIDString((string)this.sectorDropdown.Items[this.sectorDropdown.SelectedIndex]);
                if (s.rooms.Count > 1)
                {
                    undoWorld = new World(world);
                    Room r = s.FindRoomByIDString((string)this.roomDropdown.Items[this.roomDropdown.SelectedIndex]);
                    s.rooms.Remove(r);
                    this.roomDropdown.Items.RemoveAt(this.roomDropdown.SelectedIndex);
                    this.roomDropdown.SelectedIndex = 0;
                }
            }
            if (sender == this.elementBehaviorDelete)
            {
                Behavior b = null;
                if (this.elementBehaviorDropdown.Items.Count > 1)
                {
                    if (editMode == EditMode.Block)
                    {
                        undoWorld = new World(world);
                        b = selectedBlock.FindBehaviorByIDString((string)this.elementBehaviorDropdown.Items[this.elementBehaviorDropdown.SelectedIndex]);
                        selectedBlock.behaviors.Remove(b);
                        this.elementBehaviorDropdown.Items.RemoveAt(this.elementBehaviorDropdown.SelectedIndex);
                        this.elementBehaviorDropdown.SelectedIndex = 0;
                    }
                    if (editMode == EditMode.Line)
                    {
                        undoWorld = new World(world);
                        b = selectedEdge.FindBehaviorByIDString((string)this.elementBehaviorDropdown.Items[this.elementBehaviorDropdown.SelectedIndex]);
                        selectedEdge.behaviors.Remove(b);
                        this.elementBehaviorDropdown.Items.RemoveAt(this.elementBehaviorDropdown.SelectedIndex);
                        this.elementBehaviorDropdown.SelectedIndex = 0;
                    }
                    if (editMode == EditMode.Doodad)
                    {
                        undoWorld = new World(world);
                        b = selectedDoodad.FindBehaviorByIDString((string)this.elementBehaviorDropdown.Items[this.elementBehaviorDropdown.SelectedIndex]);
                        selectedDoodad.behaviors.Remove(b);
                        this.elementBehaviorDropdown.Items.RemoveAt(this.elementBehaviorDropdown.SelectedIndex);
                        this.elementBehaviorDropdown.SelectedIndex = 0;
                    }
                }
            }
            if (sender == this.elementDelete)
            {
                if (editMode == EditMode.Monster)
                {
                    undoWorld = new World(world);
                    selectedFace.monsters.Remove(selectedMonster);
                }
                if (editMode == EditMode.Block)
                {
                    undoWorld = new World(world);
                    selectedFace.blocks.Remove(selectedBlock);
                }
                if (editMode == EditMode.Doodad)
                {
                    undoWorld = new World(world);
                    selectedFace.doodads.Remove(selectedDoodad);
                }
                if (editMode == EditMode.Decoration)
                {
                    undoWorld = new World(world);
                    selectedFace.decorations.Remove(selectedDecoration);
                }
            }
        }

        public void set_mode()
        {
            if (editMode == EditMode.Monster)
            {
                this.modeMonster.Checked = true;
            }
            if (editMode == EditMode.Block)
            {
                this.modeDraw.Checked = true;
            }
            if (editMode == EditMode.Line)
            {
                this.modeLine.Checked = true;
            }
            if (editMode == EditMode.Doodad)
            {
                this.modeDoodad.Checked = true;
            }
            if (editMode == EditMode.Point)
            {
                this.modePoint.Checked = true;
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
                this.blockPropertiesGroup.Visible = false;
                this.decorationPropertiesGroup.Visible = false;
                
                this.elementNameField.Text = selectedMonster.IDString;
                this.elementIDField.Text = selectedMonster.IDString;
                this.elementBehaviorDropdown.Items.Clear();
                this.behaviorNameField.Text = "";

                this.monsterMovementDropdown.SelectedIndex = (int)selectedMonster.movement;
                this.monsterArmorDropdown.SelectedIndex = (int)selectedMonster.armor;
                this.monsterAIDropdown.SelectedIndex = (int)selectedMonster.behavior;
                this.monsterWeaponDropdown.SelectedIndex = (int)selectedMonster.weapon;
                this.monsterWaypointID.Text = selectedMonster.waypointId;
                this.monsterFixedPath.Checked = selectedMonster.fixedPath;
                this.monsterSizeDropdown.SelectedIndex = (int)selectedMonster.size;
                this.monsterSpeedDropdown.SelectedIndex = (int)selectedMonster.speed;
                this.monsterTrackingDropdown.SelectedIndex = (int)selectedMonster.trackType;
                this.monsterHealthDropdown.SelectedIndex = (int)selectedMonster.health;
            }
            else if (selectedDecoration != null && MainForm.editMode == EditMode.Decoration)
            {
                this.elementGroup.Visible = true;
                this.edgePropertiesGroup.Visible = false;
                this.behaviorPropertiesGroup.Visible = true;
                this.doodadPropertiesGroup.Visible = false;
                this.blockPropertiesGroup.Visible = false;
                this.monsterPropertiesGroup.Visible = false;
                this.decorationPropertiesGroup.Visible = true;
                this.elementNameField.Text = selectedDecoration.IDString;
                this.elementIDField.Text = selectedDecoration.IDString;
                this.decorationTexture.Text = selectedDecoration.texture;
                this.decorationDepth.Text = selectedDecoration.depth.ToString();
                this.decorationStartFrame.Text = selectedDecoration.startFrame.ToString();
                this.decorationWrap.Checked = selectedDecoration.wrap;
                this.decorationSpin.Checked = selectedDecoration.freespin;
                this.decorationReverse.Checked = selectedDecoration.reverseAnimation;
                this.decorationR.Text = selectedDecoration.color.R.ToString();
                this.decorationG.Text = selectedDecoration.color.G.ToString();
                this.decorationB.Text = selectedDecoration.color.B.ToString();
            }
            else if (selectedDoodad != null && MainForm.editMode == EditMode.Doodad)
            {
                this.elementGroup.Visible = true;
                this.edgePropertiesGroup.Visible = false;
                this.behaviorPropertiesGroup.Visible = true;
                this.doodadPropertiesGroup.Visible = true;
                this.blockPropertiesGroup.Visible = false;
                this.monsterPropertiesGroup.Visible = false;
                this.decorationPropertiesGroup.Visible = false;
                this.elementNameField.Text = selectedDoodad.IDString;
                this.elementIDField.Text = selectedDoodad.IDString;
                this.elementBehaviorDropdown.Items.Clear();
                foreach (Behavior b in selectedDoodad.behaviors)
                {
                    this.elementBehaviorDropdown.Items.Add(b.IDString);
                }
                this.elementBehaviorDropdown.SelectedIndex = 0;
                this.behaviorNameField.Text = selectedDoodad.FindBehaviorByIDString((string)this.elementBehaviorDropdown.Items[this.elementBehaviorDropdown.SelectedIndex]).IDString;

                this.doodadAbilityDropdown.SelectedIndex = (int)selectedDoodad.ability;
                this.doodadTypeDropdown.SelectedIndex = (int)selectedDoodad.type;
                this.doodadFixed.Checked = selectedDoodad.fixedPosition;
                this.doodadTarget.Text = selectedDoodad.targetObject;
                this.doodadTargetBehavior.Text = selectedDoodad.targetBehavior;
                this.doodadExpectedBehavior.Text = selectedDoodad.expectBehavior;
                this.doodadActivationCost.Text = selectedDoodad.activationCost.ToString();
            }
            else if (selectedEdge != null && MainForm.editMode == EditMode.Line)
            {
                this.elementGroup.Visible = true;
                this.edgePropertiesGroup.Visible = true;
                this.behaviorPropertiesGroup.Visible = true;
                this.doodadPropertiesGroup.Visible = false;
                this.blockPropertiesGroup.Visible = false;
                this.monsterPropertiesGroup.Visible = false;
                this.decorationPropertiesGroup.Visible = false;
                this.elementNameField.Text = selectedEdge.IDString;
                this.elementIDField.Text = selectedEdge.IDString;
                this.elementBehaviorDropdown.Items.Clear();
                foreach (Behavior b in selectedEdge.behaviors)
                {
                    this.elementBehaviorDropdown.Items.Add(b.IDString);
                }
                this.edgeTypeDropdown.SelectedIndex = (int)selectedEdge.type;
                this.elementBehaviorDropdown.SelectedIndex = 0;
                this.behaviorNameField.Text = selectedEdge.FindBehaviorByIDString((string)this.elementBehaviorDropdown.Items[this.elementBehaviorDropdown.SelectedIndex]).IDString;
            }
            else if (selectedBlock != null && MainForm.editMode == EditMode.Block)
            {
                this.elementGroup.Visible = true;
                this.edgePropertiesGroup.Visible = false;
                this.behaviorPropertiesGroup.Visible = true;
                this.doodadPropertiesGroup.Visible = false;
                this.blockPropertiesGroup.Visible = true;
                this.monsterPropertiesGroup.Visible = false;
                this.decorationPropertiesGroup.Visible = false;
                this.blockColorR.Text = selectedBlock.color.R.ToString();
                this.blockColorG.Text = selectedBlock.color.G.ToString();
                this.blockColorB.Text = selectedBlock.color.B.ToString();
                this.blockScale.Checked = selectedBlock.scales;
                this.blockDepth.Text = selectedBlock.depth.ToString();
                this.blockType.SelectedIndex = (int)selectedBlock.type;
                
                this.elementNameField.Text = selectedBlock.IDString;
                this.elementIDField.Text = selectedBlock.IDString;
                this.elementBehaviorDropdown.Items.Clear();
                foreach (Behavior b in selectedBlock.behaviors)
                {
                    this.elementBehaviorDropdown.Items.Add(b.IDString);
                }
                this.elementBehaviorDropdown.SelectedIndex = 0;
                this.behaviorNameField.Text = selectedBlock.FindBehaviorByIDString((string)this.elementBehaviorDropdown.Items[this.elementBehaviorDropdown.SelectedIndex]).IDString;                
            }
            else
            {
                this.edgePropertiesGroup.Visible = false;
                this.elementGroup.Visible = false;
                this.behaviorPropertiesGroup.Visible = false;
                this.doodadPropertiesGroup.Visible = false;
                this.monsterPropertiesGroup.Visible = false;
                this.blockPropertiesGroup.Visible = false;
                this.monsterPropertiesGroup.Visible = false;
                this.decorationPropertiesGroup.Visible = false;
            }
        }

        void behavior_select(object sender, System.EventArgs e)
        {
            if (sender == elementBehaviorDropdown)
            {
                if (editMode == EditMode.Block)
                {
                    update_behavior_data(MainForm.selectedBlock.FindBehaviorByIDString((string)this.elementBehaviorDropdown.Items[this.elementBehaviorDropdown.SelectedIndex]));                    
                }
                if (editMode == EditMode.Line)
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
            if (sender == this.modeDoodad)
            {
                editMode = EditMode.Doodad;
            }
            if (sender == this.modeMonster)
            {
                editMode = EditMode.Monster;
            }
            if (sender == this.modeDecoration)
            {
                editMode = EditMode.Decoration;
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

        void block_change(object sender, System.EventArgs e)
        {
            try
            {
                if (sender == this.blockColorR)
                {
                    selectedBlock.color.R = System.Convert.ToByte(this.blockColorR.Text);
                }
                if (sender == this.blockColorG)
                {
                    selectedBlock.color.G = System.Convert.ToByte(this.blockColorG.Text);
                }
                if (sender == this.blockColorB)
                {
                    selectedBlock.color.B = System.Convert.ToByte(this.blockColorB.Text);
                }
                if (sender == this.blockType)
                {
                    selectedBlock.type = (WallType)(this.blockType.SelectedIndex);
                }
                if (sender == this.blockScale)
                {
                    selectedBlock.scales = this.blockScale.Checked;
                }
                if (sender == this.blockDepth)
                {
                    selectedBlock.depth = (float)System.Convert.ToDouble(this.blockDepth.Text);
                }
            }
            catch { }
        }

        void decoration_change(object sender, System.EventArgs e)
        {
            try
            {
                
                if (sender == this.decorationTexture)
                {
                    selectedDecoration.texture = this.decorationTexture.Text;
                }
                if (sender == this.decorationStartFrame)
                {
                    selectedDecoration.startFrame = System.Convert.ToInt32(this.decorationStartFrame.Text);
                }
                if (sender == this.decorationWrap)
                {
                    selectedDecoration.wrap = this.decorationWrap.Checked;
                }
                if (sender == this.decorationSpin)
                {
                    selectedDecoration.freespin = this.decorationSpin.Checked;
                }
                if (sender == this.decorationReverse)
                {
                    selectedDecoration.reverseAnimation = this.decorationReverse.Checked;
                }
                if (sender == this.decorationDepth)
                {
                    selectedDecoration.depth = System.Convert.ToInt32(this.decorationDepth.Text);
                }
                if (sender == this.decorationR)
                {
                    selectedDecoration.color.R = System.Convert.ToByte(this.decorationR.Text);
                }
                if (sender == this.decorationG)
                {
                    selectedDecoration.color.G = System.Convert.ToByte(this.decorationG.Text);
                }
                if (sender == this.decorationB)
                {
                    selectedDecoration.color.B = System.Convert.ToByte(this.decorationB.Text);
                }
            }
            catch { }
        }

        void monster_change(object sender, System.EventArgs e)
        {
            try
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
                if (sender == this.monsterHealthDropdown)
                {
                    selectedMonster.health = (MonsterHealth)this.monsterHealthDropdown.SelectedIndex;
                }
                if (sender == this.monsterSpeedDropdown)
                {
                    selectedMonster.speed = (MonsterSpeed)this.monsterSpeedDropdown.SelectedIndex;
                }
                if (sender == this.monsterSizeDropdown)
                {
                    selectedMonster.size = (MonsterSize)this.monsterSizeDropdown.SelectedIndex;
                }
                if (sender == this.monsterTrackingDropdown)
                {
                    selectedMonster.trackType = (TrackType)this.monsterTrackingDropdown.SelectedIndex;
                }
            }
            catch { }
        }

        void doodad_change(object sender, System.EventArgs e)
        {
            try
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
            catch { }
        }

        void world_rename(object sender, System.EventArgs e)
        {
            if (sender == this.sectorNameField)
            {
                Sector s = world.FindSectorByIDString((string)this.sectorDropdown.Items[this.sectorDropdown.SelectedIndex]);
                s._name = this.sectorNameField.Text;
                this.sectorDropdown.Items[this.sectorDropdown.SelectedIndex] = s.IDString;
            }
            if (sender == this.roomNameField)
            {
                Sector s = world.FindSectorByIDString((string)this.sectorDropdown.Items[this.sectorDropdown.SelectedIndex]);
                Room r = s.FindRoomByIDString((string)this.roomDropdown.Items[this.roomDropdown.SelectedIndex]);
                r._name = this.roomNameField.Text;
                this.roomDropdown.Items[this.roomDropdown.SelectedIndex] = r.IDString;
            }
            if (sender == this.roomFriendlyNameField)
            {
                Sector s = world.FindSectorByIDString((string)this.sectorDropdown.Items[this.sectorDropdown.SelectedIndex]);
                Room r = s.FindRoomByIDString((string)this.roomDropdown.Items[this.roomDropdown.SelectedIndex]);
                r.friendlyName = this.roomFriendlyNameField.Text;                
            }
            if (sender == this.roomDecalDropdown)
            {
                Sector s = world.FindSectorByIDString((string)this.sectorDropdown.Items[this.sectorDropdown.SelectedIndex]);
                Room r = s.FindRoomByIDString((string)this.roomDropdown.Items[this.roomDropdown.SelectedIndex]);
                r.decal = (Decal)this.roomDecalDropdown.SelectedIndex;
            }
            if (sender == this.elementNameField)
            {
                if (MainForm.editMode == EditMode.Block)
                {
                    Block b = MainForm.selectedBlock;
                    b._name = this.elementNameField.Text;
                    this.elementIDField.Text = b.IDString;
                }
                if (MainForm.editMode == EditMode.Line)
                {
                    Edge edge = MainForm.selectedEdge;
                    edge._name = this.elementNameField.Text;
                    this.elementIDField.Text = edge.IDString;
                }
                if (MainForm.editMode == EditMode.Doodad)
                {
                    Doodad d = MainForm.selectedDoodad;
                    d._name = this.elementNameField.Text;
                    this.elementIDField.Text = d.IDString;
                }
                if (MainForm.editMode == EditMode.Monster)
                {
                    Monster m = MainForm.selectedMonster;
                    m._name = this.elementNameField.Text;
                    this.elementIDField.Text = m.IDString;
                }
                if (MainForm.editMode == EditMode.Decoration)
                {
                    Decoration d = MainForm.selectedDecoration;
                    d._name = this.elementNameField.Text;
                    this.elementIDField.Text = d.IDString;
                }
            }
            if (sender == this.behaviorNameField)
            {
                if (MainForm.editMode == EditMode.Block)
                {
                    Behavior b = MainForm.selectedBlock.FindBehaviorByIDString((string)this.elementBehaviorDropdown.Items[this.elementBehaviorDropdown.SelectedIndex]);
                    b._name = this.behaviorNameField.Text;
                    this.elementBehaviorDropdown.Items[this.elementBehaviorDropdown.SelectedIndex] = b.IDString;
                }
                if (MainForm.editMode == EditMode.Line)
                {
                    Behavior b = MainForm.selectedEdge.FindBehaviorByIDString((string)this.elementBehaviorDropdown.Items[this.elementBehaviorDropdown.SelectedIndex]);
                    b._name = this.behaviorNameField.Text;
                    this.elementBehaviorDropdown.Items[this.elementBehaviorDropdown.SelectedIndex] = b.IDString;
                }
                if (MainForm.editMode == EditMode.Doodad)
                {
                    Behavior b = MainForm.selectedDoodad.FindBehaviorByIDString((string)this.elementBehaviorDropdown.Items[this.elementBehaviorDropdown.SelectedIndex]);
                    b._name = this.behaviorNameField.Text;
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
                    r.Move(new Vector3(System.Convert.ToInt32(this.roomCenterX.Text)- r.centerX,0,0));
                }
                if (sender == this.roomCenterY)
                {
                    r.Move(new Vector3(0,System.Convert.ToInt32(this.roomCenterY.Text) - r.centerY, 0));                    
                }
                if (sender == this.roomCenterZ)
                {
                    r.Move(new Vector3(0,0,System.Convert.ToInt32(this.roomCenterZ.Text) - r.centerZ));                    
                }
                if (sender == this.roomSizeX)
                {
                    r.Resize(new Vector3(System.Convert.ToInt32(this.roomSizeX.Text) - r.sizeX, 0, 0));                    
                }
                if (sender == this.roomSizeY)
                {
                    r.Resize(new Vector3(0, System.Convert.ToInt32(this.roomSizeY.Text) - r.sizeY, 0));
                }
                if (sender == this.roomSizeZ)
                {
                    r.Resize(new Vector3(0, 0, System.Convert.ToInt32(this.roomSizeZ.Text) - r.sizeZ));                    
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
            if (MainForm.editMode == EditMode.Block)
            {
                b = MainForm.selectedBlock.FindBehaviorByIDString((string)this.elementBehaviorDropdown.Items[this.elementBehaviorDropdown.SelectedIndex]);
            }
            if (MainForm.editMode == EditMode.Line)
            {
                b = MainForm.selectedEdge.FindBehaviorByIDString((string)this.elementBehaviorDropdown.Items[this.elementBehaviorDropdown.SelectedIndex]);
            }
            if (MainForm.editMode == EditMode.Doodad)
            {
                b = MainForm.selectedDoodad.FindBehaviorByIDString((string)this.elementBehaviorDropdown.Items[this.elementBehaviorDropdown.SelectedIndex]);
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
