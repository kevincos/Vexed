using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace VexedCore
{
    public enum State
    {
        Normal,
        Jump,
        BridgeJump
    }
    public class Player
    {
        public State state = State.Normal;
        public Vertex center;
        public Room currentRoom;
        int jumpRecovery = 0;
        public Vector3 jumpDestination;
        public Vector3 jumpSource;
        public Vector3 jumpCameraDestination;
        public Vector3 jumpCameraSource;
        public Vector3 jumpPosition;
        public Vector3 jumpNormal;
        public Room jumpRoom;
        public int jumpMaxTime = 1000;
        public int jumpTime = 0;

        public Vector3 cameraPos
        {
            get
            {
                if (state == State.Normal)
                {
                    return currentRoom.RaisedPosition(center.position, 16, 6f);
                }
                if (state == State.BridgeJump)
                {
                    return ((jumpMaxTime - jumpTime) * jumpCameraSource + jumpTime * jumpCameraDestination) / jumpMaxTime;
                    //return jumpPosition + center.normal * 16;
                }
                else
                {
                    Vector3 side = Vector3.Cross(center.direction, center.normal);
                    float sideValue = 1f*jumpTime / jumpMaxTime;
                    Vector3 cameraBase = ((jumpMaxTime-jumpTime)*jumpCameraSource + jumpTime* jumpCameraDestination) / jumpMaxTime;
                    Vector3 camera = cameraBase + sideValue * side;
                    return camera;
                }
            }
        }

        public Vector3 cameraUp
        {
            get
            {
                return currentRoom.AdjustedUp(center.position, center.direction, center.normal, 1f);
            }
        }

        public Vector3 cameraTarget
        {
            get
            {
                if (state == State.Normal)
                    return center.position;
                else
                    return jumpPosition;
            }
        }


        public void Update(GameTime gameTime)
        {
            

            Vector2 stick = GamePad.GetState(Game1.activePlayer).ThumbSticks.Left;
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            
            if (state == State.Normal)
            {
                #region normal update
                center.Update(currentRoom, gameTime.ElapsedGameTime.Milliseconds);
                jumpRecovery -= gameTime.ElapsedGameTime.Milliseconds;
                if (jumpRecovery < 0) jumpRecovery = 0;
                Vector3 up = center.direction;
                Vector3 right = Vector3.Cross(up, center.normal);
            
                if (Keyboard.GetState().IsKeyDown(Keys.Left) || stick.X < -Game1.controlStickTrigger)
                {
                    center.velocity -= .001f * right;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Right) || stick.X > Game1.controlStickTrigger)
                {
                    center.velocity += .001f * right;
                }
                if ((gamePadState.IsButtonDown(Buttons.A) || Keyboard.GetState().IsKeyDown(Keys.Space)) && jumpRecovery == 0)
                {
                    center.velocity += .01f * up;
                    jumpRecovery = 500;
                }

                center.velocity += .001f * stick.X * right;
                center.velocity -= .0002f * up;

                if (center.velocity.Length() > .02f)
                {
                    center.velocity.Normalize();
                    center.velocity = center.velocity * .02f;
                }

                if (gamePadState.IsButtonDown(Buttons.X) || Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    foreach (JumpPad j in currentRoom.jumpPads)
                    {
                        if (j.active)
                        {
                            jumpRoom = j.targetRoom;
                            float roomSize = Math.Abs(Vector3.Dot(jumpRoom.size / 2, center.normal));
                            jumpSource = center.position;
                            jumpDestination = center.position + Vector3.Dot(jumpRoom.center - center.position - roomSize * center.normal, center.normal) * center.normal;
                            jumpCameraSource = currentRoom.RaisedPosition(jumpSource, 16, 6f);
                            jumpCameraDestination = jumpRoom.RaisedPosition(jumpDestination, 16, 6f);
                            state = State.Jump;
                            center.velocity = Vector3.Zero;
                            jumpTime = 0;
                            j.active = false;
                            jumpNormal = -center.normal;
                        }
                    }
                    foreach (Bridge b in currentRoom.bridges)
                    {
                        if (b.active)
                        {
                            jumpRoom = b.targetRoom;
                            jumpSource = center.position;
                            jumpDestination = b.targetBridge.position.position;
                            jumpCameraSource = currentRoom.RaisedPosition(jumpSource, 16, 6f);
                            jumpCameraDestination = jumpRoom.RaisedPosition(jumpDestination, 16, 6f);
                            
                            jumpNormal = center.normal;
                            jumpTime = 0;
                            state = State.BridgeJump;
                            center.velocity = Vector3.Zero;
                            b.active = false;
                        }
                    }
                }
                #endregion
            }
            if (state == State.Jump || state == State.BridgeJump)
            {
                jumpTime += gameTime.ElapsedGameTime.Milliseconds;
                if (jumpTime > jumpMaxTime)
                    jumpTime = jumpMaxTime;
                jumpPosition = ((jumpMaxTime - jumpTime) * jumpSource + jumpTime * jumpDestination) / jumpMaxTime;
                if (jumpTime == jumpMaxTime)
                {
                    center.normal = jumpNormal;
                    center.position = jumpDestination;
                    state = State.Normal;
                    currentRoom = jumpRoom;                    
                }

            }
        }

        public void Draw(GameTime gameTime)
        {
            List<VertexPositionColorNormal> triangleList = new List<VertexPositionColorNormal>();
            List<Vertex> rectVertexList = new List<Vertex>();
            Vector3 up = center.direction;
            Vector3 right = Vector3.Cross(up, center.normal);            
            rectVertexList.Add(new Vertex(center.position, center.normal, +.5f * up + .5f * right, center.direction));
            rectVertexList.Add(new Vertex(center.position, center.normal, +.5f * up - .5f * right, center.direction));
            rectVertexList.Add(new Vertex(center.position, center.normal, -.5f * up - .5f * right, center.direction));
            rectVertexList.Add(new Vertex(center.position, center.normal, -.5f * up + .5f * right, center.direction));
            foreach (Vertex v in rectVertexList)
            {
                v.Update(currentRoom, 1);
            }

            currentRoom.AddBlockToTriangleList(rectVertexList, Color.White, .2f, triangleList);

            VertexPositionColorNormal[] triangleArray = triangleList.ToArray();
            if (state == State.Jump || state == State.BridgeJump)
            {
                for (int i = 0; i < triangleArray.Count(); i++)
                {
                    triangleArray[i].Position += jumpPosition - center.position;
                }
            }

            Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                triangleArray, 0, triangleList.Count / 3, VertexPositionColorNormal.VertexDeclaration);     
            
        }

    }
}
