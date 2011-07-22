﻿using System;
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
        BridgeJump,
        Spin
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
        public Vector3 spinUp;
        public Room jumpRoom;
        public int spinMaxTime = 200;
        public int spinTime = 0;
        public int jumpMaxTime = 1000;
        public int jumpTime = 0;
        public int walkTime = 0;
        public int walkMaxTime = 800;
        public int fireCooldown = 100;
        public bool leftWall = false;
        public bool rightWall = false;
        public float playerHalfWidth = .35f;
        public float playerHalfHeight = .5f;
        public int weaponSwitchCooldown = 0;
        public int weaponSwitchCooldownMax = 200;

        public VexedLib.GunType gunType = VexedLib.GunType.Blaster;

        public static Texture2D player_textures;
        public static List<List<Vector2>> texCoordList;

        public static int texGridCount = 8;

        public static List<Vector2> LoadTexCoords(int x, int y)
        {
            float texWidth = 1f / texGridCount;
            List<Vector2> texCoords = new List<Vector2>();
            texCoords.Add(new Vector2((x + 1) * texWidth, y * texWidth));
            texCoords.Add(new Vector2(x * texWidth, y * texWidth));
            texCoords.Add(new Vector2(x * texWidth, (y + 1) * texWidth));
            texCoords.Add(new Vector2((x + 1) * texWidth, (y + 1) * texWidth));

            return texCoords;
        }
        public static void InitTexCoords()
        {
            texCoordList = new List<List<Vector2>>();
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {                    
                    texCoordList.Add(LoadTexCoords(x, y));
                }
            }            
        }
        
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
        public int faceDirection = 0;
        public float baseCameraDistance = 9;
        public int orbsCollected = 0;

        public Doodad respawnPoint;
        public Vertex respawnCenter;
        public bool dead = false;

        public float boundingBoxTop;
        public float boundingBoxBottom;
        public float boundingBoxLeft;
        public float boundingBoxRight;


        public int fireTime
        {
            get
            {
                if (gunType == VexedLib.GunType.Blaster || gunType == VexedLib.GunType.Spread)
                    return 1000;
                if (gunType == VexedLib.GunType.Missile)
                    return 4000;
                if (gunType == VexedLib.GunType.Beam)
                    return 2000;
                if (gunType == VexedLib.GunType.Repeater)
                    return 200;
                return 0;
            }
        }


        public void UpdateBoundingBox()
        {
            Vector3 up = center.direction;
            Vector3 right = Vector3.Cross(up, center.normal);
            float centerX = Vector3.Dot(right, center.position);
            float centerY = Vector3.Dot(up, center.position);
            boundingBoxBottom = centerY - playerHalfHeight;
            boundingBoxTop = centerY + playerHalfHeight;
            boundingBoxLeft = centerX - playerHalfWidth;
            boundingBoxRight = centerX + playerHalfWidth;            
        }
        
        public int currentTextureIndex
        {
            get
            {
                if (state != State.Normal)
                    return 0;
                if (walking == true)
                {
                    if (faceDirection < 0)
                    {
                        if (walkTime > 3 * walkMaxTime / 4)
                            return 16;
                        else if (walkTime > walkMaxTime / 2)
                            return 18;
                        else if (walkTime > walkMaxTime / 4)
                            return 19;
                        else
                            return 17;
                    }
                    else
                    {
                        if (walkTime > 3 * walkMaxTime / 4)
                            return 24;
                        else if (walkTime > walkMaxTime / 2)
                            return 26;
                        else if (walkTime > walkMaxTime / 4)
                            return 27;
                        else
                            return 25;
                    }
                }
                else if (grounded == true)
                    if (faceDirection < 0)
                        return 2;
                    else if (faceDirection > 0)
                        return 1;
                    else
                        return 0;
                else if (leftWall == true && faceDirection < 0)
                    return 32;
                else if (rightWall == true && faceDirection > 0)
                    return 40;
                else if (faceDirection < 0)
                    return 19;
                else if (faceDirection > 0)
                    return 27;
                else
                    return 8;
            }
        }

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
                if (state == State.Normal || state == State.Spin)
                {
                    return currentRoom.RaisedPosition(center.position, baseCameraDistance, 6f);
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
                if (state == State.Spin)
                {
                    Vector3 oldUp = currentRoom.AdjustedUp(center.position, center.direction, center.normal, 1f);
                    Vector3 newUp = currentRoom.AdjustedUp(center.position, spinUp, center.normal, 1f);
                    return ((spinMaxTime - spinTime) * oldUp + spinTime * newUp) / spinMaxTime;
                }
                else
                    return currentRoom.AdjustedUp(center.position, center.direction, center.normal, 1f);
            }
        }

        public Vector3 cameraTarget
        {
            get
            {
                if (state == State.Normal || state == State.Spin)
                    return center.position;
                else
                    return jumpPosition;
            }
        }

        public Vector3 up
        {
            get
            {
                return center.direction;
            }
        }

        public Vector3 right
        {
            get
            {
                return Vector3.Cross(center.direction, center.normal);
            }
        }

        public void Respawn()
        {
            currentRoom = respawnPoint.targetRoom;
            state = State.Normal;
            center = new Vertex(respawnCenter.position, respawnCenter.normal, respawnCenter.velocity, respawnCenter.direction);
            dead = false;
        }

        public void Update(GameTime gameTime)
        {
            fireCooldown -= gameTime.ElapsedGameTime.Milliseconds;
            if (fireCooldown < 0) fireCooldown = 0;

            weaponSwitchCooldown -= gameTime.ElapsedGameTime.Milliseconds;
                if (weaponSwitchCooldown < 0) weaponSwitchCooldown = 0;
            if (dead == true)
                Respawn();
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

                walkTime += gameTime.ElapsedGameTime.Milliseconds;
                if (walkTime > walkMaxTime) walkTime -= walkMaxTime;
                center.Update(currentRoom, gameTime.ElapsedGameTime.Milliseconds);
                jumpRecovery -= gameTime.ElapsedGameTime.Milliseconds;
                if (jumpRecovery < 0) jumpRecovery = 0;
                Vector3 up = center.direction;
                Vector3 right = Vector3.Cross(up, center.normal);

                //if (grounded == true)
                    //faceDirection = 0;

                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                {
                    faceDirection = -1;
                    if (grounded == true)
                        center.velocity -= walkSpeed * right;
                    else
                        center.velocity -= airSpeed * right;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                {
                    faceDirection = 1;
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
                    if (stick.X > 0) faceDirection = 1;
                    if (stick.X < 0) faceDirection = -1;
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
                    else if (leftWall && faceDirection < 0)
                    {
                        faceDirection = 1;
                        if (upMagnitude < jumpSpeed)
                            center.velocity += (jumpSpeed - upMagnitude) * up;                                                    
                        if(rightMagnitude < wallJumpSpeed)
                            center.velocity += (wallJumpSpeed - rightMagnitude) * right;
                        jumpRecovery = 500;
                        
                    }
                    else if (rightWall && faceDirection > 0)
                    {
                        faceDirection = -1;
                        if (upMagnitude < jumpSpeed)
                            center.velocity += (jumpSpeed - upMagnitude) * up;
                        if (rightMagnitude > -wallJumpSpeed)
                            center.velocity -= (wallJumpSpeed + rightMagnitude) * right;                         
                        jumpRecovery = 500;
                    }
                }
                if((gamePadState.IsButtonDown(Buttons.B)))
                {
                    Respawn();
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
                if(gamePadState.IsButtonDown(Buttons.RightShoulder))
                {
                    if (weaponSwitchCooldown == 0)
                    {
                        weaponSwitchCooldown = weaponSwitchCooldownMax;
                        if (gunType == VexedLib.GunType.Blaster)
                            gunType = VexedLib.GunType.Beam;
                        else if (gunType == VexedLib.GunType.Beam)
                            gunType = VexedLib.GunType.Missile;
                        else if (gunType == VexedLib.GunType.Missile)
                            gunType = VexedLib.GunType.Bomb;
                        else
                            gunType = VexedLib.GunType.Blaster;
                    }
                }
                if(gamePadState.IsButtonDown(Buttons.X))
                {
                    if (fireCooldown == 0)
                    {
                        fireCooldown = 400;
                        Vector3 shootDirection;
                        if (grounded == false && leftWall == true)
                            shootDirection = right / right.Length();
                        else if (grounded == false && rightWall == true)
                            shootDirection = -right / right.Length();
                        else if (faceDirection >= 0)
                            shootDirection = right / right.Length();
                        else
                            shootDirection = -right / right.Length();

                        if (gunType == VexedLib.GunType.Blaster)
                            currentRoom.projectiles.Add(new Projectile(null, ProjectileType.Player, center.position, center.velocity, center.normal, shootDirection));
                        if (gunType == VexedLib.GunType.Missile)
                            currentRoom.projectiles.Add(new Projectile(null, ProjectileType.Missile, center.position + .5f * shootDirection, center.velocity, center.normal, shootDirection));
                        if (gunType == VexedLib.GunType.Bomb)
                            currentRoom.projectiles.Add(new Projectile(null, ProjectileType.Bomb, center.position + .5f * shootDirection, center.velocity, center.normal, shootDirection));
                        if (gunType == VexedLib.GunType.Beam)
                            currentRoom.projectiles.Add(new Projectile(null, ProjectileType.Laser, center.position, center.velocity, center.normal, shootDirection));

                    }
                }
                if (gamePadState.IsButtonDown(Buttons.Y) || Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    foreach (JumpPad j in currentRoom.jumpPads)
                    {
                        if (j.active)
                        {
                            jumpRoom = j.targetRoom;
                            float roomSize = Math.Abs(Vector3.Dot(jumpRoom.size / 2, center.normal));
                            jumpSource = center.position;
                            jumpDestination = center.position + Vector3.Dot(jumpRoom.center - center.position - roomSize * center.normal, center.normal) * center.normal;
                            jumpCameraSource = currentRoom.RaisedPosition(jumpSource, baseCameraDistance, 6f);
                            jumpCameraDestination = jumpRoom.RaisedPosition(jumpDestination, baseCameraDistance, 6f);
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
                            jumpCameraSource = currentRoom.RaisedPosition(jumpSource, baseCameraDistance, 6f);
                            jumpCameraDestination = jumpRoom.RaisedPosition(jumpDestination, baseCameraDistance, 6f);
                            
                            jumpNormal = center.normal;
                            jumpTime = 0;
                            state = State.BridgeJump;
                            center.velocity = Vector3.Zero;
                            faceDirection = 0;
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
                    center.velocity = Vector3.Zero;
                    faceDirection = 0;
                    state = State.Normal;
                    currentRoom = jumpRoom;                    
                }

            }
            if (state == State.Spin)
            {
                spinTime += gameTime.ElapsedGameTime.Milliseconds;
                if (spinTime > spinMaxTime)
                {
                    spinTime = 0;
                    center.direction = spinUp;
                    state = State.Normal;
                }
            }
        }

        public List<Vector3> GetCollisionRect()
        {
            List<Vector3> playerVertexList = new List<Vector3>();
            playerVertexList.Add(center.position + playerHalfHeight * up + playerHalfWidth * right);
            playerVertexList.Add(center.position + playerHalfHeight * up - playerHalfWidth * right);
            playerVertexList.Add(center.position - playerHalfHeight * up - playerHalfWidth * right);
            playerVertexList.Add(center.position - playerHalfHeight * up + playerHalfWidth * right);
            UpdateBoundingBox();
            return playerVertexList;
        }

        public bool CollisionFirstPass(Block b)
        {
            return (boundingBoxBottom > b.boundingBoxTop ||
                        boundingBoxTop < b.boundingBoxBottom ||
                        boundingBoxLeft > b.boundingBoxRight ||
                        boundingBoxRight < b.boundingBoxLeft);
        }

        public bool CollisionFirstPass(Doodad d)
        {
            return (boundingBoxBottom > d.boundingBoxTop ||
                        boundingBoxTop < d.boundingBoxBottom ||
                        boundingBoxLeft > d.boundingBoxRight ||
                        boundingBoxRight < d.boundingBoxLeft);
        }

        public bool CollisionFirstPass(Projectile p)
        {
            return (boundingBoxBottom > p.boundingBoxTop ||
                        boundingBoxTop < p.boundingBoxBottom ||
                        boundingBoxLeft > p.boundingBoxRight ||
                        boundingBoxRight < p.boundingBoxLeft);
        }

        public bool CollisionFirstPass(Monster m)
        {
            return (boundingBoxBottom > m.boundingBoxTop ||
                        boundingBoxTop < m.boundingBoxBottom ||
                        boundingBoxLeft > m.boundingBoxRight ||
                        boundingBoxRight < m.boundingBoxLeft);
        }


        public void DrawTexture()
        {
            List<VertexPositionColorNormal> triangleList = new List<VertexPositionColorNormal>();
            List<VertexPositionColorNormalTexture> textureTriangleList = new List<VertexPositionColorNormalTexture>();
            List<Vertex> rectVertexList = new List<Vertex>();
            Vector3 up = center.direction;
            if (state == State.Spin)
            {
                up = ((spinMaxTime - spinTime) * center.direction + spinTime * spinUp) / spinMaxTime;
                up.Normalize();
            }
            Vector3 right = Vector3.Cross(up, center.normal);
            rectVertexList.Add(new Vertex(center.position, center.normal, +playerHalfHeight * up + .5f * right, center.direction));
            rectVertexList.Add(new Vertex(center.position, center.normal, +playerHalfHeight * up - .5f * right, center.direction));
            rectVertexList.Add(new Vertex(center.position, center.normal, -playerHalfHeight * up - .5f * right, center.direction));
            rectVertexList.Add(new Vertex(center.position, center.normal, -playerHalfHeight * up + .5f * right, center.direction));
            foreach (Vertex v in rectVertexList)
            {
                v.Update(currentRoom, 1);
            }


            currentRoom.AddTextureToTriangleList(rectVertexList, Color.White, .3f, textureTriangleList, Player.texCoordList[currentTextureIndex], true);


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
        

    }
}
