#region File Description
//-----------------------------------------------------------------------------
// WorldPreviewControl.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
#endregion

namespace WinFormsGraphicsDevice
{
    /// <summary>
    /// Example control inherits from GraphicsDeviceControl, which allows it to
    /// render using a GraphicsDevice. This control shows how to draw animating
    /// 3D graphics inside a WinForms application. It hooks the Application.Idle
    /// event, using this to invalidate the control, which will cause the animation
    /// to constantly redraw.
    /// </summary>
    class WorldPreviewControl : GraphicsDeviceControl
    {
        BasicEffect effect;
        Stopwatch timer;

        Vector3 currentTarget;
        Vector3 currentCamera;
        Vector3 currentUp;

        Vector3 futureTarget;
        Vector3 futureCamera;
        Vector3 futureUp;

        float currentRotate = 0;
        float futureRotate = 0;
        float savedRotate = 0;
        float currentPitch = 0;
        float futurePitch = 0;
        float savedPitch = 0;

        bool ready = false;
        bool mouseReady = true;
                
        public WorldPreviewControl()
        {
            futureTarget = currentTarget = Vector3.Zero;
            futureCamera = currentCamera = new Vector3(100, 100, 100);
            futureUp = currentUp = new Vector3(0, 0, 1);
        }

        public static int TriangleCount;
        public static int LineCount;
        public static int RoomEdgeCount;
        // Vertex positions and colors used to display a spinning triangle.
        public VertexPositionColor[] TriangleVertexList()
        {
            TriangleCount = 0;
            foreach (Sector s in MainForm.world.sectors)
            {
                foreach (Room r in s.rooms)
                {
                    TriangleCount+=12;
                }
            }
            VertexPositionColor[] vList = new VertexPositionColor[TriangleCount*3];
            int currentTriangleIndex = 0;
            
            foreach (Sector s in MainForm.world.sectors)
            {
                foreach (Room r in s.rooms)
                {
                    //Color col = r.color;
                    Color col = r.color;
                    // Top
                    vList[currentTriangleIndex + 0] = new VertexPositionColor(new Vector3(r.centerX - r.sizeX / 2, r.centerY - r.sizeY / 2, r.centerZ + r.sizeZ / 2), col);
                    vList[currentTriangleIndex + 1] = new VertexPositionColor(new Vector3(r.centerX - r.sizeX / 2, r.centerY + r.sizeY / 2, r.centerZ + r.sizeZ / 2), col);
                    vList[currentTriangleIndex + 2] = new VertexPositionColor(new Vector3(r.centerX + r.sizeX / 2, r.centerY - r.sizeY / 2, r.centerZ + r.sizeZ / 2), col);

                    vList[currentTriangleIndex + 3] = new VertexPositionColor(new Vector3(r.centerX - r.sizeX / 2, r.centerY + r.sizeY / 2, r.centerZ + r.sizeZ / 2), col);
                    vList[currentTriangleIndex + 4] = new VertexPositionColor(new Vector3(r.centerX + r.sizeX / 2, r.centerY - r.sizeY / 2, r.centerZ + r.sizeZ / 2), col);
                    vList[currentTriangleIndex + 5] = new VertexPositionColor(new Vector3(r.centerX + r.sizeX / 2, r.centerY + r.sizeY / 2, r.centerZ + r.sizeZ / 2), col);

                    // Bottom
                    vList[currentTriangleIndex + 6] = new VertexPositionColor(new Vector3(r.centerX - r.sizeX / 2, r.centerY - r.sizeY / 2, r.centerZ - r.sizeZ / 2), col);
                    vList[currentTriangleIndex + 7] = new VertexPositionColor(new Vector3(r.centerX - r.sizeX / 2, r.centerY + r.sizeY / 2, r.centerZ - r.sizeZ / 2), col);
                    vList[currentTriangleIndex + 8] = new VertexPositionColor(new Vector3(r.centerX + r.sizeX / 2, r.centerY - r.sizeY / 2, r.centerZ - r.sizeZ / 2), col);

                    vList[currentTriangleIndex + 9] = new VertexPositionColor(new Vector3(r.centerX - r.sizeX / 2, r.centerY + r.sizeY / 2, r.centerZ - r.sizeZ / 2), col);
                    vList[currentTriangleIndex + 10] = new VertexPositionColor(new Vector3(r.centerX + r.sizeX / 2, r.centerY - r.sizeY / 2, r.centerZ - r.sizeZ / 2), col);
                    vList[currentTriangleIndex + 11] = new VertexPositionColor(new Vector3(r.centerX + r.sizeX / 2, r.centerY + r.sizeY / 2, r.centerZ - r.sizeZ / 2), col);

                    // Left
                    vList[currentTriangleIndex + 12] = new VertexPositionColor(new Vector3(r.centerX - r.sizeX / 2, r.centerY - r.sizeY / 2, r.centerZ - r.sizeZ / 2), col);
                    vList[currentTriangleIndex + 13] = new VertexPositionColor(new Vector3(r.centerX - r.sizeX / 2, r.centerY + r.sizeY / 2, r.centerZ - r.sizeZ / 2), col);
                    vList[currentTriangleIndex + 14] = new VertexPositionColor(new Vector3(r.centerX - r.sizeX / 2, r.centerY - r.sizeY / 2, r.centerZ + r.sizeZ / 2), col);

                    vList[currentTriangleIndex + 15] = new VertexPositionColor(new Vector3(r.centerX - r.sizeX / 2, r.centerY + r.sizeY / 2, r.centerZ - r.sizeZ / 2), col);
                    vList[currentTriangleIndex + 16] = new VertexPositionColor(new Vector3(r.centerX - r.sizeX / 2, r.centerY - r.sizeY / 2, r.centerZ + r.sizeZ / 2), col);
                    vList[currentTriangleIndex + 17] = new VertexPositionColor(new Vector3(r.centerX - r.sizeX / 2, r.centerY + r.sizeY / 2, r.centerZ + r.sizeZ / 2), col);

                    // Right
                    vList[currentTriangleIndex + 18] = new VertexPositionColor(new Vector3(r.centerX + r.sizeX / 2, r.centerY - r.sizeY / 2, r.centerZ - r.sizeZ / 2), col);
                    vList[currentTriangleIndex + 19] = new VertexPositionColor(new Vector3(r.centerX + r.sizeX / 2, r.centerY + r.sizeY / 2, r.centerZ - r.sizeZ / 2), col);
                    vList[currentTriangleIndex + 20] = new VertexPositionColor(new Vector3(r.centerX + r.sizeX / 2, r.centerY - r.sizeY / 2, r.centerZ + r.sizeZ / 2), col);

                    vList[currentTriangleIndex + 21] = new VertexPositionColor(new Vector3(r.centerX + r.sizeX / 2, r.centerY + r.sizeY / 2, r.centerZ - r.sizeZ / 2), col);
                    vList[currentTriangleIndex + 22] = new VertexPositionColor(new Vector3(r.centerX + r.sizeX / 2, r.centerY - r.sizeY / 2, r.centerZ + r.sizeZ / 2), col);
                    vList[currentTriangleIndex + 23] = new VertexPositionColor(new Vector3(r.centerX + r.sizeX / 2, r.centerY + r.sizeY / 2, r.centerZ + r.sizeZ / 2), col);


                    // Back
                    vList[currentTriangleIndex + 24] = new VertexPositionColor(new Vector3(r.centerX - r.sizeX / 2, r.centerY - r.sizeY / 2, r.centerZ - r.sizeZ / 2), col);
                    vList[currentTriangleIndex + 25] = new VertexPositionColor(new Vector3(r.centerX - r.sizeX / 2, r.centerY - r.sizeY / 2, r.centerZ + r.sizeZ / 2), col);
                    vList[currentTriangleIndex + 26] = new VertexPositionColor(new Vector3(r.centerX + r.sizeX / 2, r.centerY - r.sizeY / 2, r.centerZ - r.sizeZ / 2), col);

                    vList[currentTriangleIndex + 27] = new VertexPositionColor(new Vector3(r.centerX - r.sizeX / 2, r.centerY - r.sizeY / 2, r.centerZ + r.sizeZ / 2), col);
                    vList[currentTriangleIndex + 28] = new VertexPositionColor(new Vector3(r.centerX + r.sizeX / 2, r.centerY - r.sizeY / 2, r.centerZ - r.sizeZ / 2), col);
                    vList[currentTriangleIndex + 29] = new VertexPositionColor(new Vector3(r.centerX + r.sizeX / 2, r.centerY - r.sizeY / 2, r.centerZ + r.sizeZ / 2), col);

                    // Front
                    vList[currentTriangleIndex + 30] = new VertexPositionColor(new Vector3(r.centerX - r.sizeX / 2, r.centerY + r.sizeY / 2, r.centerZ - r.sizeZ / 2), col);
                    vList[currentTriangleIndex + 31] = new VertexPositionColor(new Vector3(r.centerX - r.sizeX / 2, r.centerY + r.sizeY / 2, r.centerZ + r.sizeZ / 2), col);
                    vList[currentTriangleIndex + 32] = new VertexPositionColor(new Vector3(r.centerX + r.sizeX / 2, r.centerY + r.sizeY / 2, r.centerZ - r.sizeZ / 2), col);

                    vList[currentTriangleIndex + 33] = new VertexPositionColor(new Vector3(r.centerX - r.sizeX / 2, r.centerY + r.sizeY / 2, r.centerZ + r.sizeZ / 2), col);
                    vList[currentTriangleIndex + 34] = new VertexPositionColor(new Vector3(r.centerX + r.sizeX / 2, r.centerY + r.sizeY / 2, r.centerZ - r.sizeZ / 2), col);
                    vList[currentTriangleIndex + 35] = new VertexPositionColor(new Vector3(r.centerX + r.sizeX / 2, r.centerY + r.sizeY / 2, r.centerZ + r.sizeZ / 2), col);
                    currentTriangleIndex += 36;
                }
            }

            return vList;

        }
        
        public VertexPositionColor[] LineVertexList()
        {
            LineCount = 0;
            foreach (Sector s in MainForm.world.sectors)
            {
                foreach (Room r in s.rooms)
                {
                    LineCount += 12;
                }
            }
            VertexPositionColor[] vList = new VertexPositionColor[LineCount * 2];
            int currentLineIndex = 0;
            foreach (Sector s in MainForm.world.sectors)
            {
                foreach (Room r in s.rooms)
                {
                    Color lineColor = Color.Black;
                    if (s== MainForm.selectedSector || r == MainForm.selectedRoom)
                        lineColor = Color.White;
                    // Top
                    vList[currentLineIndex + 0] = new VertexPositionColor(new Vector3(r.centerX - r.sizeX / 2, r.centerY - r.sizeY / 2, r.centerZ + r.sizeZ / 2), lineColor);
                    vList[currentLineIndex + 1] = new VertexPositionColor(new Vector3(r.centerX - r.sizeX / 2, r.centerY + r.sizeY / 2, r.centerZ + r.sizeZ / 2), lineColor);
                    vList[currentLineIndex + 2] = new VertexPositionColor(new Vector3(r.centerX - r.sizeX / 2, r.centerY + r.sizeY / 2, r.centerZ + r.sizeZ / 2), lineColor);
                    vList[currentLineIndex + 3] = new VertexPositionColor(new Vector3(r.centerX + r.sizeX / 2, r.centerY + r.sizeY / 2, r.centerZ + r.sizeZ / 2), lineColor);
                    vList[currentLineIndex + 4] = new VertexPositionColor(new Vector3(r.centerX + r.sizeX / 2, r.centerY + r.sizeY / 2, r.centerZ + r.sizeZ / 2), lineColor);
                    vList[currentLineIndex + 5] = new VertexPositionColor(new Vector3(r.centerX + r.sizeX / 2, r.centerY - r.sizeY / 2, r.centerZ + r.sizeZ / 2), lineColor);
                    vList[currentLineIndex + 6] = new VertexPositionColor(new Vector3(r.centerX + r.sizeX / 2, r.centerY - r.sizeY / 2, r.centerZ + r.sizeZ / 2), lineColor);
                    vList[currentLineIndex + 7] = new VertexPositionColor(new Vector3(r.centerX - r.sizeX / 2, r.centerY - r.sizeY / 2, r.centerZ + r.sizeZ / 2), lineColor);
                    // Bottom
                    vList[currentLineIndex + 8] = new VertexPositionColor(new Vector3(r.centerX - r.sizeX / 2, r.centerY - r.sizeY / 2, r.centerZ - r.sizeZ / 2), lineColor);
                    vList[currentLineIndex + 9] = new VertexPositionColor(new Vector3(r.centerX - r.sizeX / 2, r.centerY + r.sizeY / 2, r.centerZ - r.sizeZ / 2), lineColor);
                    vList[currentLineIndex + 10] = new VertexPositionColor(new Vector3(r.centerX - r.sizeX / 2, r.centerY + r.sizeY / 2, r.centerZ - r.sizeZ / 2), lineColor);
                    vList[currentLineIndex + 11] = new VertexPositionColor(new Vector3(r.centerX + r.sizeX / 2, r.centerY + r.sizeY / 2, r.centerZ - r.sizeZ / 2), lineColor);
                    vList[currentLineIndex + 12] = new VertexPositionColor(new Vector3(r.centerX + r.sizeX / 2, r.centerY + r.sizeY / 2, r.centerZ - r.sizeZ / 2), lineColor);
                    vList[currentLineIndex + 13] = new VertexPositionColor(new Vector3(r.centerX + r.sizeX / 2, r.centerY - r.sizeY / 2, r.centerZ - r.sizeZ / 2), lineColor);
                    vList[currentLineIndex + 14] = new VertexPositionColor(new Vector3(r.centerX + r.sizeX / 2, r.centerY - r.sizeY / 2, r.centerZ - r.sizeZ / 2), lineColor);
                    vList[currentLineIndex + 15] = new VertexPositionColor(new Vector3(r.centerX - r.sizeX / 2, r.centerY - r.sizeY / 2, r.centerZ - r.sizeZ / 2), lineColor);
                    // Sides
                    vList[currentLineIndex + 16] = new VertexPositionColor(new Vector3(r.centerX - r.sizeX / 2, r.centerY - r.sizeY / 2, r.centerZ - r.sizeZ / 2), lineColor);
                    vList[currentLineIndex + 17] = new VertexPositionColor(new Vector3(r.centerX - r.sizeX / 2, r.centerY - r.sizeY / 2, r.centerZ + r.sizeZ / 2), lineColor);
                    vList[currentLineIndex + 18] = new VertexPositionColor(new Vector3(r.centerX - r.sizeX / 2, r.centerY + r.sizeY / 2, r.centerZ - r.sizeZ / 2), lineColor);
                    vList[currentLineIndex + 19] = new VertexPositionColor(new Vector3(r.centerX - r.sizeX / 2, r.centerY + r.sizeY / 2, r.centerZ + r.sizeZ / 2), lineColor);
                    vList[currentLineIndex + 20] = new VertexPositionColor(new Vector3(r.centerX + r.sizeX / 2, r.centerY + r.sizeY / 2, r.centerZ - r.sizeZ / 2), lineColor);
                    vList[currentLineIndex + 21] = new VertexPositionColor(new Vector3(r.centerX + r.sizeX / 2, r.centerY + r.sizeY / 2, r.centerZ + r.sizeZ / 2), lineColor);
                    vList[currentLineIndex + 22] = new VertexPositionColor(new Vector3(r.centerX + r.sizeX / 2, r.centerY - r.sizeY / 2, r.centerZ - r.sizeZ / 2), lineColor);
                    vList[currentLineIndex + 23] = new VertexPositionColor(new Vector3(r.centerX + r.sizeX / 2, r.centerY - r.sizeY / 2, r.centerZ + r.sizeZ / 2), lineColor);
                    currentLineIndex += 24;
                }
            }

            return vList;

        }

        public VertexPositionColor[] RoomVertexList()
        {
            /*Room r;
            if (MainForm.zoomRoom != null)
                r = MainForm.zoomRoom;
            else
                r = MainForm.selectedRoom;*/

            RoomEdgeCount = 0;
            foreach (Sector s in MainForm.world.sectors)
            {
                foreach (Room r in s.rooms)
                {
                    foreach (Face f in r.faceList)
                    {
                        foreach (Block b in f.blocks)
                        {
                            foreach (Edge e in b.edges)
                            {
                                RoomEdgeCount++;
                            }
                        }
                    }
                }
            }
            VertexPositionColor[] vList = new VertexPositionColor[RoomEdgeCount * 2];
            int currentEdge = 0;
            Color c = Color.Black;
            foreach (Sector s in MainForm.world.sectors)
            {
                foreach (Room r in s.rooms)
                {
                    foreach (Face f in r.faceList)
                    {
                        foreach (Block b in f.blocks)
                        {
                            foreach (Edge e in b.edges)
                            {
                                vList[currentEdge] = new VertexPositionColor(e.start + (.1f * f.normal), c);
                                vList[currentEdge + 1] = new VertexPositionColor(e.end + (.1f * f.normal), c);
                                currentEdge += 2;
                            }
                        }
                    }
                }
            }
            return vList;
            
        }

        public Vector3 GetIntCoordsFromMouse()
        {
            
            System.Drawing.Point p = this.PointToClient(new System.Drawing.Point(Mouse.GetState().X, Mouse.GetState().Y));
            Vector2 screenCoords = new Vector2((p.X - this.ClientRectangle.Width / 2) / 10, (p.Y - this.ClientRectangle.Height / 2) / 10);
            MainForm m = (MainForm)this.Parent.Parent.Parent;
            screenCoords.X += (int)MainForm.translation.X;
            screenCoords.Y += (int)MainForm.translation.Y;
            Vector3 right = Vector3.Cross(MainForm.currentUp, MainForm.selectedFace.normal);
            return MainForm.selectedFace.center + screenCoords.X * right - screenCoords.Y * MainForm.currentUp;
        }

        public Vector3 GetFloatCoordsFromMouse()
        {
            System.Drawing.Point p = this.PointToClient(new System.Drawing.Point(Mouse.GetState().X, Mouse.GetState().Y));
            Vector2 screenCoords = new Vector2((p.X - this.ClientRectangle.Width / 2f) / 10f, (p.Y - this.ClientRectangle.Height / 2f) / 10f);
            MainForm m = (MainForm)this.Parent.Parent.Parent;
            screenCoords.X += MainForm.translation.X;
            screenCoords.Y += MainForm.translation.Y;
            Vector3 right = Vector3.Cross(MainForm.currentUp, MainForm.selectedFace.normal);
            return MainForm.selectedFace.center + screenCoords.X * right - screenCoords.Y * MainForm.currentUp;
        }

        public VertexPositionColor[] TargetVertexList()
        {
            Color targetColor = Color.White;
            VertexPositionColor[] vList = null;
            Vector3 right = Vector3.Cross(MainForm.currentUp, MainForm.selectedFace.normal);
            Vector3 pointer = Vector3.Zero;

            if (MainForm.selectedFace != null)
            {
                if (MainForm.editMode == EditMode.New)
                {
                    pointer = GetIntCoordsFromMouse();
                    vList = MainForm.selectedFace.GetTemplate(pointer);
                }
                if (MainForm.editMode == EditMode.Line)
                {
                    pointer = GetFloatCoordsFromMouse();
                    vList = MainForm.selectedFace.GetSelectedLineHighlight(pointer);
                }
            }
            
            if (vList == null)
            {
                vList = new VertexPositionColor[4];
                vList[0] = new VertexPositionColor(pointer + right + .2f * MainForm.selectedFace.normal, targetColor);
                vList[1] = new VertexPositionColor(pointer - right + +.2f * MainForm.selectedFace.normal, targetColor);
                vList[2] = new VertexPositionColor(pointer + MainForm.currentUp + .2f * MainForm.selectedFace.normal, targetColor);
                vList[3] = new VertexPositionColor(pointer - MainForm.currentUp + .2f * MainForm.selectedFace.normal, targetColor);
            }
            return vList;
        }

        /// <summary>
        /// Initializes the control.
        /// </summary>
        protected override void Initialize()
        {
            // Create our effect.
            effect = new BasicEffect(GraphicsDevice);

            effect.VertexColorEnabled = true;

            // Start the animation timer.
            timer = Stopwatch.StartNew();

            // Hook the idle event to constantly redraw our animation.
            Application.Idle += delegate { Invalidate(); };
        }

        public void ViewFace(Face f)
        {
            //futureTarget = f.center;
            futureTarget = new Vector3(MainForm.zoomRoom.centerX, MainForm.zoomRoom.centerY, MainForm.zoomRoom.centerZ);
            futureCamera = f.center + 50f * f.normal;            
            if (Vector3.Cross(futureUp, f.normal).Equals(Vector3.Zero))
                futureUp = Vector3.UnitY;
            savedRotate = currentRotate;
            savedPitch = currentPitch;
            futureRotate = 0;
            futurePitch = 0;
            MainForm.translation = Vector2.Zero;
        }

        public void ViewWorld()
        {
            futureTarget = Vector3.Zero;
            futureCamera = new Vector3(100, 100, 100);
            futureUp = new Vector3(0, 0, 1);
            futureRotate = savedRotate;
            futurePitch = savedPitch;
        }

        public void FindFace(Vector3 newNormal, Vector3 newUp)
        {
            if (ready == true && MainForm.zoomRoom!=null)
            {
                MainForm.currentUp = futureUp = newUp;
                foreach (Face f in MainForm.zoomRoom.faceList)
                {
                    if (f.normal.Equals(newNormal))
                    {
                        MainForm.selectedFace = f;
                        ViewFace(f);
                        return;
                    }
                }
            }
        }

        private void UpdateCamera()
        {
            ready = true;
            if (currentRotate < futureRotate)
            {
                currentRotate += MainForm.animateSpeed;
                ready = false;
                if (currentRotate > futureRotate) currentRotate = futureRotate;
            }
            if (currentRotate > futureRotate)
            {
                currentRotate -= MainForm.animateSpeed;
                ready = false;
                if (currentRotate < futureRotate) currentRotate = futureRotate;
            }
            if (currentPitch < futurePitch)
            {
                currentPitch += MainForm.animateSpeed;
                ready = false;
                if (currentPitch > futurePitch) currentPitch = futurePitch;
            }
            if (currentPitch > futurePitch)
            {
                currentPitch -= MainForm.animateSpeed;
                ready = false;
                if (currentPitch < futurePitch) currentPitch = futurePitch;
            }

            if (currentRotate < 0)
            {
                ready = false;
                currentRotate += (float)Math.PI * 2;
                futureRotate += (float)Math.PI * 2;
            }
            if (currentRotate > (float)Math.PI * 2)
            {
                ready = false;
                currentRotate -= (float)Math.PI * 2;
                futureRotate -= (float)Math.PI * 2;
            }
            if (currentPitch < 0)
            {
                ready = false;
                currentPitch += (float)Math.PI * 2;
                futurePitch += (float)Math.PI * 2;
            }
            if (currentPitch > (float)Math.PI*2)
            {
                ready = false;
                currentPitch -= (float)Math.PI * 2;
                futurePitch -= (float)Math.PI * 2;
            }

            if (currentTarget.X < futureTarget.X)
            {
                ready = false;
                currentTarget.X += MainForm.animateSpeed*30;
                if (currentTarget.X > futureTarget.X) currentTarget.X = futureTarget.X;
            }
            if (currentTarget.Y < futureTarget.Y)
            {
                ready = false;
                currentTarget.Y += MainForm.animateSpeed*30;
                if (currentTarget.Y > futureTarget.Y) currentTarget.Y = futureTarget.Y;
            }
            if (currentTarget.Z < futureTarget.Z)
            {
                ready = false;
                currentTarget.Z += MainForm.animateSpeed*30;
                if (currentTarget.Z > futureTarget.Z) currentTarget.Z = futureTarget.Z;
            }
            if (currentTarget.X > futureTarget.X)
            {
                ready = false;
                currentTarget.X -= MainForm.animateSpeed*30;
                if (currentTarget.X < futureTarget.X) currentTarget.X = futureTarget.X;
            }
            if (currentTarget.Y > futureTarget.Y)
            {
                ready = false;
                currentTarget.Y -= MainForm.animateSpeed*30;
                if (currentTarget.Y < futureTarget.Y) currentTarget.Y = futureTarget.Y;
            }
            if (currentTarget.Z > futureTarget.Z)
            {
                ready = false;
                currentTarget.Z -= MainForm.animateSpeed*30;
                if (currentTarget.Z < futureTarget.Z) currentTarget.Z = futureTarget.Z;
            }

            if (currentCamera.X < futureCamera.X)
            {
                ready = false;
                currentCamera.X += MainForm.animateSpeed*30;
                if (currentCamera.X > futureCamera.X) currentCamera.X = futureCamera.X;
            }
            if (currentCamera.Y < futureCamera.Y)
            {
                ready = false;
                currentCamera.Y += MainForm.animateSpeed*30;
                if (currentCamera.Y > futureCamera.Y) currentCamera.Y = futureCamera.Y;
            }
            if (currentCamera.Z < futureCamera.Z)
            {
                ready = false;
                currentCamera.Z += MainForm.animateSpeed*30;
                if (currentCamera.Z > futureCamera.Z) currentCamera.Z = futureCamera.Z;
            }
            if (currentCamera.X > futureCamera.X)
            {
                ready = false;
                currentCamera.X -= MainForm.animateSpeed*30;
                if (currentCamera.X < futureCamera.X) currentCamera.X = futureCamera.X;
            }
            if (currentCamera.Y > futureCamera.Y)
            {
                ready = false;
                currentCamera.Y -= MainForm.animateSpeed*30;
                if (currentCamera.Y < futureCamera.Y) currentCamera.Y = futureCamera.Y;
            }
            if (currentCamera.Z > futureCamera.Z)
            {
                ready = false;
                currentCamera.Z -= MainForm.animateSpeed*30;
                if (currentCamera.Z < futureCamera.Z) currentCamera.Z = futureCamera.Z;
            }

            if (currentUp.X < futureUp.X)
            {
                ready = false;
                currentUp.X += MainForm.animateSpeed*30;
                if (currentUp.X > futureUp.X) currentUp.X = futureUp.X;
            }
            if (currentUp.Y < futureUp.Y)
            {
                ready = false;
                currentUp.Y += MainForm.animateSpeed*30;
                if (currentUp.Y > futureUp.Y) currentUp.Y = futureUp.Y;
            }
            if (currentUp.Z < futureUp.Z)
            {
                ready = false;
                currentUp.Z += MainForm.animateSpeed*30;
                if (currentUp.Z > futureUp.Z) currentUp.Z = futureUp.Z;
            }
            if (currentUp.X > futureUp.X)
            {
                ready = false;
                currentUp.X -= MainForm.animateSpeed*30;
                if (currentUp.X < futureUp.X) currentUp.X = futureUp.X;
            }
            if (currentUp.Y > futureUp.Y)
            {
                ready = false;
                currentUp.Y -= MainForm.animateSpeed*30;
                if (currentUp.Y < futureUp.Y) currentUp.Y = futureUp.Y;
            }
            if (currentUp.Z > futureUp.Z)
            {
                ready = false;
                currentUp.Z -= MainForm.animateSpeed*30;
                if (currentUp.Z < futureUp.Z) currentUp.Z = futureUp.Z;
            }  
        }

        /// <summary>
        /// Draws the control.
        /// </summary>
        protected override void Draw()
        {
            try
            {
                UpdateCamera();

                if (MainForm.zoomRoom == null)
                {
                    if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
                    {
                        futureRotate += MainForm.animateSpeed;
                    }
                    if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
                    {
                        futureRotate -= MainForm.animateSpeed;
                    }
                    if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
                    {
                        futurePitch += MainForm.animateSpeed;
                    }
                    if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
                    {
                        futurePitch -= MainForm.animateSpeed;
                    }
                }
                else if (MainForm.selectedFace != null && ready == true)
                {                             
                    Vector3 right = Vector3.Cross(MainForm.currentUp, MainForm.selectedFace.normal);
                    if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
                    {
                        MainForm.translation += new Vector2(1, 0);
                        futureTarget += right;
                        futureCamera += right;
                    }
                    if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
                    {
                        MainForm.translation -= new Vector2(1, 0);
                        futureTarget -= right;
                        futureCamera -= right;
                    }
                    if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
                    {
                        MainForm.translation -= new Vector2(0, 1);
                        futureTarget += MainForm.currentUp;
                        futureCamera += MainForm.currentUp;
                    }
                    if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
                    {
                        MainForm.translation += new Vector2(0, 1);
                        futureTarget -= MainForm.currentUp;
                        futureCamera -= MainForm.currentUp;
                    }

                    if (Mouse.GetState().LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                    {
                        if (mouseReady == true)
                        {
                            if (MainForm.editMode == EditMode.New)
                            {
                                MainForm.selectedFace.AddBlock(GetIntCoordsFromMouse());
                                mouseReady = false;
                            }
                            else if (MainForm.editMode == EditMode.Line)
                            {
                                Vector3 pointer = GetFloatCoordsFromMouse();
                                MainForm.selectedBlock = MainForm.selectedFace.GetHoverBlock(pointer);
                                MainForm.selectedEdge = MainForm.selectedFace.GetHoverEdge(pointer);
                                if(MainForm.selectedEdge!=null)
                                    MainForm.editMode = EditMode.LineDrag;
                                mouseReady = false;
                            }
                            else if (MainForm.editMode == EditMode.LineDrag)
                            {
                                MainForm.editMode = EditMode.Line;
                                mouseReady = false;
                            }
                            
                        }
                    }
                    else
                    {
                        mouseReady = true;
                    }
                    if (MainForm.editMode == EditMode.LineDrag)
                    {
                        MainForm.selectedBlock.Resize(GetIntCoordsFromMouse(), MainForm.selectedEdge, MainForm.selectedFace.normal);
                    }
                }

                GraphicsDevice.Clear(new Color(40, 40, 40));

                // Set transform matrices.
                float aspect = GraphicsDevice.Viewport.AspectRatio;

                //effect.World = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
                effect.World = Matrix.CreateFromAxisAngle(new Vector3(0, 0, 1), currentRotate) * Matrix.CreateFromAxisAngle(new Vector3(0, 1, 0), currentPitch);

                effect.View = Matrix.CreateLookAt(currentCamera,
                                                  currentTarget,
                                                  currentUp);

                effect.Projection = Matrix.CreatePerspectiveFieldOfView(1, aspect, 1, 1000);

                // Set renderstates.
                GraphicsDevice.RasterizerState = RasterizerState.CullNone;

                // Draw the triangle.
                effect.CurrentTechnique.Passes[0].Apply();

                GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                    TriangleVertexList(), 0, TriangleCount);
                
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList,
                     LineVertexList(), 0, LineCount);

                VertexPositionColor[] roomList = RoomVertexList();
                if (roomList.Length > 0)
                {
                    GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList,
                                    roomList, 0, roomList.Length/2);
                }

                if (MainForm.selectedFace != null)
                {
                    VertexPositionColor[] cursorList = TargetVertexList();
                    GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList,
                                  cursorList, 0, cursorList.Length / 2);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }
    }
}
