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

        static float zoomSpeed = .03f;

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
            Room r;
            if (MainForm.zoomRoom != null)
                r = MainForm.zoomRoom;
            else
                r = MainForm.selectedRoom;
            RoomEdgeCount = 0;
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
            VertexPositionColor[] vList = new VertexPositionColor[RoomEdgeCount * 2];
            int currentEdge = 0;
            Color c = Color.Black;
            foreach (Face f in r.faceList)
            {
                foreach (Block b in f.blocks)
                {
                    foreach (Edge e in b.edges)
                    {
                        vList[currentEdge] = new VertexPositionColor(e.start +(.1f*f.normal), c);
                        vList[currentEdge+1] = new VertexPositionColor(e.end+(.1f*f.normal), c);
                        currentEdge += 2;
                    }
                }
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
        }

        public void ViewWorld()
        {
            futureTarget = Vector3.Zero;
            futureCamera = new Vector3(100, 100, 100);
            futureUp = new Vector3(0, 0, 1);
            futureRotate = savedRotate;
            futurePitch = savedPitch;
        }

        public void FindFace(Vector3 newNormal)
        {
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

        private void UpdateCamera()
        {
            ready = true;
            if (currentRotate < futureRotate)
            {
                currentRotate += .001f;
                ready = false;
                if (currentRotate > futureRotate) currentRotate = futureRotate;
            }
            if (currentRotate > futureRotate)
            {
                currentRotate -= .001f;
                ready = false;
                if (currentRotate < futureRotate) currentRotate = futureRotate;
            }
            if (currentPitch < futurePitch)
            {
                currentPitch += .001f;
                ready = false;
                if (currentPitch > futurePitch) currentPitch = futurePitch;
            }
            if (currentPitch > futurePitch)
            {
                currentPitch -= .001f;
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
                currentTarget.X += zoomSpeed;
                if (currentTarget.X > futureTarget.X) currentTarget.X = futureTarget.X;
            }
            if (currentTarget.Y < futureTarget.Y)
            {
                ready = false;
                currentTarget.Y += zoomSpeed;
                if (currentTarget.Y > futureTarget.Y) currentTarget.Y = futureTarget.Y;
            }
            if (currentTarget.Z < futureTarget.Z)
            {
                ready = false;
                currentTarget.Z += zoomSpeed;
                if (currentTarget.Z > futureTarget.Z) currentTarget.Z = futureTarget.Z;
            }
            if (currentTarget.X > futureTarget.X)
            {
                ready = false;
                currentTarget.X -= zoomSpeed;
                if (currentTarget.X < futureTarget.X) currentTarget.X = futureTarget.X;
            }
            if (currentTarget.Y > futureTarget.Y)
            {
                ready = false;
                currentTarget.Y -= zoomSpeed;
                if (currentTarget.Y < futureTarget.Y) currentTarget.Y = futureTarget.Y;
            }
            if (currentTarget.Z > futureTarget.Z)
            {
                ready = false;
                currentTarget.Z -= zoomSpeed;
                if (currentTarget.Z < futureTarget.Z) currentTarget.Z = futureTarget.Z;
            }

            if (currentCamera.X < futureCamera.X)
            {
                ready = false;
                currentCamera.X += zoomSpeed;
                if (currentCamera.X > futureCamera.X) currentCamera.X = futureCamera.X;
            }
            if (currentCamera.Y < futureCamera.Y)
            {
                ready = false;
                currentCamera.Y += zoomSpeed;
                if (currentCamera.Y > futureCamera.Y) currentCamera.Y = futureCamera.Y;
            }
            if (currentCamera.Z < futureCamera.Z)
            {
                ready = false;
                currentCamera.Z += zoomSpeed;
                if (currentCamera.Z > futureCamera.Z) currentCamera.Z = futureCamera.Z;
            }
            if (currentCamera.X > futureCamera.X)
            {
                ready = false;
                currentCamera.X -= zoomSpeed;
                if (currentCamera.X < futureCamera.X) currentCamera.X = futureCamera.X;
            }
            if (currentCamera.Y > futureCamera.Y)
            {
                ready = false;
                currentCamera.Y -= zoomSpeed;
                if (currentCamera.Y < futureCamera.Y) currentCamera.Y = futureCamera.Y;
            }
            if (currentCamera.Z > futureCamera.Z)
            {
                ready = false;
                currentCamera.Z -= zoomSpeed;
                if (currentCamera.Z < futureCamera.Z) currentCamera.Z = futureCamera.Z;
            }

            if (currentUp.X < futureUp.X)
            {
                ready = false;
                currentUp.X += zoomSpeed;
                if (currentUp.X > futureUp.X) currentUp.X = futureUp.X;
            }
            if (currentUp.Y < futureUp.Y)
            {
                ready = false;
                currentUp.Y += zoomSpeed;
                if (currentUp.Y > futureUp.Y) currentUp.Y = futureUp.Y;
            }
            if (currentUp.Z < futureUp.Z)
            {
                ready = false;
                currentUp.Z += zoomSpeed;
                if (currentUp.Z > futureUp.Z) currentUp.Z = futureUp.Z;
            }
            if (currentUp.X > futureUp.X)
            {
                ready = false;
                currentUp.X -= zoomSpeed;
                if (currentUp.X < futureUp.X) currentUp.X = futureUp.X;
            }
            if (currentUp.Y > futureUp.Y)
            {
                ready = false;
                currentUp.Y -= zoomSpeed;
                if (currentUp.Y < futureUp.Y) currentUp.Y = futureUp.Y;
            }
            if (currentUp.Z > futureUp.Z)
            {
                ready = false;
                currentUp.Z -= zoomSpeed;
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
                        futureRotate += .001f;
                    }
                    if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
                    {
                        futureRotate -= .001f;
                    }
                    if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
                    {
                        futurePitch += .001f;
                    }
                    if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
                    {
                        futurePitch -= .001f;
                    }
                }
                else if(ready==true)
                {
                    if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
                    {
                        Vector3 nextNormal = Vector3.Cross(MainForm.selectedFace.normal, MainForm.currentUp);
                        FindFace(nextNormal);
                    }
                    if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
                    {
                        Vector3 nextNormal = -Vector3.Cross(MainForm.selectedFace.normal, MainForm.currentUp);
                        FindFace(nextNormal);
                    }
                    if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
                    {
                        Vector3 nextNormal = MainForm.currentUp;
                        MainForm.currentUp = futureUp = -MainForm.selectedFace.normal;                        
                        FindFace(nextNormal);                        
                    }
                    if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
                    {
                        Vector3 nextNormal = -MainForm.currentUp;
                        MainForm.currentUp = futureUp = MainForm.selectedFace.normal;
                        FindFace(nextNormal);
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

                if (MainForm.zoomRoom != null || MainForm.selectedRoom !=null)
                {
                    GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList,
                                  RoomVertexList(), 0, RoomEdgeCount);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }
    }
}
