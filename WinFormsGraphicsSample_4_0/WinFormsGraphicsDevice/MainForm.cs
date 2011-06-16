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
            if (MainForm.zoomRoom != null)
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
        }

        void room_mode_change(object sender, System.EventArgs e)
        {
            if (sender == this.modeDraw)
            {
                editMode = EditMode.New;   
            }
            if (sender == this.modeLine)
            {
                editMode = EditMode.Line;
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
    }
}
