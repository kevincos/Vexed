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
        public Vector3 platformVelocity;
        public Room jumpRoom;
        public int jumpMaxTime = 1000;
        public int jumpTime = 0;
        public bool leftWall = false;
        public bool rightWall = false;

        public static Texture2D characterTexture;
        
        public float walkSpeed = .001f;
        public float airSpeed = .0005f;
        public float jumpSpeed = .026f;
        public float wallJumpSpeed = .01f;
        public float maxHorizSpeed = .01f;
        public float maxVertSpeed = .025f;
        public float gravityAcceleration = .001f;
        private bool _grounded = false;
        public int groundTolerance = 100;
        public int groundCounter = 0;

        public bool grounded
        {
            get
            {
                return _grounded;
            }
            set
            {
                _grounded = value;
                if(_grounded == true)
                    groundCounter = 0;
            }
        }

        public bool walking
        {
            get
            {
                Vector2 stick = GamePad.GetState(Game1.activePlayer).ThumbSticks.Left;
                return (Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.Right) || stick.X != 0) && grounded == true;
            }
        }

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
            groundCounter += gameTime.ElapsedGameTime.Milliseconds;
            if (groundCounter > groundTolerance)
            {
                groundCounter = groundTolerance;
                grounded = false;
            }
            Vector2 stick = GamePad.GetState(Game1.activePlayer).ThumbSticks.Left;
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            
            if (state == State.Normal)
            {
                #region normal update
                float upMagnitude = 0;
                float rightMagnitude = 0;
                    
                center.Update(currentRoom, gameTime.ElapsedGameTime.Milliseconds);
                jumpRecovery -= gameTime.ElapsedGameTime.Milliseconds;
                if (jumpRecovery < 0) jumpRecovery = 0;
                Vector3 up = center.direction;
                Vector3 right = Vector3.Cross(up, center.normal);

                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                {
                    if (grounded == true)
                        center.velocity -= walkSpeed * right;
                    else
                        center.velocity -= airSpeed * right;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                {
                    if (grounded == true)
                        center.velocity += walkSpeed * right;                    
                    else
                        center.velocity += airSpeed * right;
                }
                if (Math.Abs(stick.X) > 0)
                {
                    if (grounded == true)
                        center.velocity += walkSpeed * stick.X * right;
                    else
                        center.velocity += airSpeed * stick.X * right;
                }
                if ((gamePadState.IsButtonDown(Buttons.A) || Keyboard.GetState().IsKeyDown(Keys.Space)) && jumpRecovery == 0)
                {
                    upMagnitude = Vector3.Dot(up, center.velocity);
                    rightMagnitude = Vector3.Dot(right, center.velocity);
                        
                    if (grounded)
                    {
                        if (upMagnitude < jumpSpeed)
                        {
                            center.velocity += (jumpSpeed - upMagnitude) * up;
                            jumpRecovery = 500;
                        }
                    }
                    else if (leftWall)
                    {
                        if (upMagnitude < jumpSpeed)
                            center.velocity += (jumpSpeed - upMagnitude) * up;                                                    
                        if(rightMagnitude < wallJumpSpeed)
                            center.velocity += (wallJumpSpeed - rightMagnitude) * right;
                        jumpRecovery = 500;
                        
                    }
                    else if (rightWall)
                    {
                        if (upMagnitude < jumpSpeed)
                            center.velocity += (jumpSpeed - upMagnitude) * up;
                        if (rightMagnitude > -wallJumpSpeed)
                            center.velocity -= (wallJumpSpeed + rightMagnitude) * right;                         
                        jumpRecovery = 500;
                    }
                }
                
                center.velocity -= gravityAcceleration * up;

                upMagnitude = Vector3.Dot(up, center.velocity);
                rightMagnitude = Vector3.Dot(right, center.velocity - platformVelocity);
                    
                if (upMagnitude > maxVertSpeed)
                {
                    center.velocity -= (upMagnitude - maxVertSpeed) * up;
                }
                if (upMagnitude < -maxVertSpeed)
                {
                    center.velocity -= (maxVertSpeed + upMagnitude) * up;
                }
                if (rightMagnitude > maxHorizSpeed)
                {
                    center.velocity -= (rightMagnitude - maxHorizSpeed) * right;
                }
                if (rightMagnitude < -maxHorizSpeed)
                {
                    center.velocity -= (maxHorizSpeed + rightMagnitude) * right;
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

        public void DrawTexture(GameTime gameTime)
        {
            List<VertexPositionColorNormal> triangleList = new List<VertexPositionColorNormal>();
            List<VertexPositionColorNormalTexture> textureTriangleList = new List<VertexPositionColorNormalTexture>();
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

            currentRoom.AddTextureToTriangleList(rectVertexList, Color.White, .3f, textureTriangleList);


            VertexPositionColorNormalTexture[] triangleArray = textureTriangleList.ToArray();
            if (state == State.Jump || state == State.BridgeJump)
            {
                for (int i = 0; i < textureTriangleList.Count(); i++)
                {
                    triangleArray[i].Position += jumpPosition - center.position;
                }
            }

            Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                triangleArray, 0, triangleArray.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration); 
            
        }        

        public void Draw(GameTime gameTime)
        {
            List<VertexPositionColorNormal> triangleList = new List<VertexPositionColorNormal>();
            List<VertexPositionColorNormalTexture> textureTriangleList = new List<VertexPositionColorNormalTexture>();
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


            if(grounded == true)
                currentRoom.AddBlockToTriangleList(rectVertexList, Color.White, .2f, triangleList);
            else if (leftWall == true)
                currentRoom.AddBlockToTriangleList(rectVertexList, Color.Blue, .2f, triangleList);
            else if (rightWall == true)
                currentRoom.AddBlockToTriangleList(rectVertexList, Color.Green, .2f, triangleList);
            else
                currentRoom.AddBlockToTriangleList(rectVertexList, Color.Brown, .2f, triangleList);

            
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
