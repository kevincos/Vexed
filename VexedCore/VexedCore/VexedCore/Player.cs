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
using System.Xml.Serialization;

namespace VexedCore
{
    public enum State
    {
        Normal,
        Jump,
        BridgeJump,
        Tunnel,
        Phase,
        PhaseFail,
        Spin,
        Death,
        Dialog,
        HookSpin
    }

    public enum HookState
    {
        Waiting,
        Out,
        Hook,
        In
    }

    public class Player
    {

        public static Texture2D player_textures_detail;
        public static Texture2D player_textures_clean;
        public static Texture2D player_textures;
        public static Texture2D player_gun_textures;
        public static Texture2D player_boots_textures;
        public static Texture2D player_jetpack_textures;
        public static Texture2D player_booster_textures;
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

        public State state = State.Normal;
        public Vertex center;
        public Vertex tunnelDummy;
        public Vertex hookShot;
        public Vector3 hookLand;
        [XmlIgnore]public Room currentRoom;
        public string currentRoomId;
        public int jumpRecovery = 0;
        public Vector3 jumpDestination;
        public Vector3 jumpSource;
        public Vector3 jumpCameraDestination;
        public Vector3 jumpCameraSource;
        public Vector3 jumpPosition;
        public Vector3 jumpNormal;
        public Vector3 platformVelocity;
        public float referenceFrameSpeed;
        public Vector3 spinUp;
        public Vector3 lastLivingPosition;
        [XmlIgnore]
        public Doodad tunnelExit;
        [XmlIgnore]public Room jumpRoom;
        public int wallTime = 0;
        public int wallJumpCooldown = 0;
        public static int wallJumpCooldownMax = 300;
        public int spinTime = 0;
        public int launchTime = 0;
        public int jumpTime = 0;
        public int deathTime = 0;
        public int lastFireTime = 0;
        public int hookTime = 0;
        public int hookHangTime = 0;
        public bool superJump = false;
        public bool jetPacking = false;
        public bool jetPackThrust = false;
        public bool sliding = false;
        public int walkTime = 0;
        public int idleTime = 0;
        public int boostTime = 0;
        public int fireCooldown = 100;
        public bool leftWall = false;
        public bool rightWall = false;
        public int weaponSwitchCooldown = 0;
        public Ability primaryAbility;
        public Ability secondaryAbility;
        public Ability naturalShield;
        public bool boosting = false;
        public bool[] upgrades;
        public Vector3 oldNormal = Vector3.Zero;
        public Vector3 oldUp = Vector3.Zero;

        public HookState hookState = HookState.Waiting;

        public float playerHalfWidth = .35f;
        public float playerHalfHeight = .5f;

        public static int jumpRecoveryMax = 300;
        public static int spinMaxTime = 200;
        public static int walkMaxTime = 800;
        public static int launchMaxTime = 1000;
        public static int maxBoostTime = 300;
        public static int maxDeathTime = 1000;
        public static int maxHookTime = 150;
        public static int maxHookHangTime = 100;
        public static int weaponSwitchCooldownMax = 200;

        public VexedLib.GunType gunType = VexedLib.GunType.Blaster;
        
        
        public float walkSpeed = .001f;
        public float airSpeed = .0005f;
        public float jumpSpeed = .020f;
        
        public float wallJumpSpeed = .01f;
        public float maxHorizSpeed = .01f;
        
        public float gravityAcceleration = .0009f;
        public float boostAcceleration = .002f;
        private bool _grounded = false;
        public int groundTolerance = 100;
        public int groundCounter = 0;
        public int faceDirection = 0;
        public float baseCameraDistance = 12;
        public int orbsCollected = 0;

        public Vector2 cameraAngle;
        public Vector2 targetCameraAngle;

        [XmlIgnore]public Doodad respawnPoint;
        [XmlIgnore]public Player respawnPlayer;

        public bool dead = false;

        public float boundingBoxTop;
        public float boundingBoxBottom;
        public float boundingBoxLeft;
        public float boundingBoxRight;

        public static float cameraUpTilt = .2f;
        public static float cameraRoundingThreshold = 5f;

        public Player()
        {
            upgrades = new bool[64];
            upgrades[(int)AbilityType.RedKey] = true;
            upgrades[(int)AbilityType.BlueKey] = true;
            upgrades[(int)AbilityType.YellowKey] = true;
            primaryAbility = new Ability(AbilityType.Empty);
            secondaryAbility = new Ability(AbilityType.Empty);
            naturalShield = new Ability(AbilityType.Shield);
            //upgrades[(int)AbilityType.PermanentWallJump] = true;
            //upgrades[(int)AbilityType.WallJump] = true;
            //upgrades[(int)AbilityType.DoubleJump] = true;
            //for (int i = 8; i < 19; i++)
                //upgrades[i] = true;            
        }

        public Player(Player p)
        {
            
            state = p.state;
            center = new Vertex(p.center);
            jumpRecovery = p.jumpRecovery;
            jumpDestination = p.jumpDestination;
            jumpSource = p.jumpSource;
            jumpCameraDestination = p.jumpCameraDestination;
            jumpPosition = p.jumpPosition;
            jumpNormal = p.jumpNormal;
            platformVelocity = p.platformVelocity;
            spinUp = p.spinUp;
            spinTime = p.spinTime;
            launchTime = p.launchTime;
            walkTime = p.walkTime;
            boostTime = p.boostTime;
            fireCooldown = p.fireCooldown;
            leftWall = p.leftWall;
            rightWall = p.rightWall;
            jumpTime = p.jumpTime;
            weaponSwitchCooldown = p.weaponSwitchCooldown;
            gunType = p.gunType;
            currentRoomId = p.currentRoomId;
            primaryAbility = new Ability(p.primaryAbility);
            secondaryAbility = new Ability(p.secondaryAbility);
            naturalShield = new Ability(p.naturalShield);
            boosting = p.boosting;

            upgrades = new bool[64];
            for (int i = 0; i < 64; i++)
                upgrades[i] = p.upgrades[i];

            

            if (p.currentRoom != null)
                currentRoomId = p.currentRoom.id;

        }

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
                return AnimationControl.GetCurrentFrame();
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
                if (_grounded == false && value == true)
                    SoundFX.Land();

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
                return (Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.Right) ||
                    Keyboard.GetState().IsKeyDown(Keys.A) || (Keyboard.GetState().IsKeyDown(Keys.D)) || stick.X != 0) && grounded == true;
            }
        }

        public Vector3 cameraPos
        {
            get
            {
                if (state == State.Tunnel || state == State.Phase || state == State.PhaseFail)
                {
                    return currentRoom.RaisedPosition(tunnelDummy.position + cameraOffset, baseCameraDistance, cameraRoundingThreshold);
                }
                if (state == State.Normal || state == State.Spin || state == State.Dialog)
                {
                    return currentRoom.RaisedPosition(center.position + cameraOffset, baseCameraDistance, cameraRoundingThreshold);
                }
                if (state == State.Death)
                {
                    return currentRoom.RaisedPosition(lastLivingPosition + cameraOffset, baseCameraDistance, cameraRoundingThreshold);
                }
                if (state == State.BridgeJump)
                {
                    return ((launchMaxTime - launchTime) * jumpCameraSource + launchTime * jumpCameraDestination) / launchMaxTime;
                    //return jumpPosition + center.normal * 16;
                }
                else
                {
                    Vector3 side = Vector3.Cross(center.direction, center.normal);
                    float sideValue = 1f * launchTime / launchMaxTime;
                    Vector3 cameraBase = ((launchMaxTime-launchTime)*jumpCameraSource + launchTime* jumpCameraDestination) / launchMaxTime;
                    Vector3 camera = cameraBase + 5f * sideValue * (1 - sideValue) * side;
                    return camera;
                }
            }
        }

        public Vector3 baseCameraPos
        {
            get
            {
                if (state == State.Tunnel || state == State.Phase || state == State.PhaseFail)
                {
                    return currentRoom.RaisedPosition(tunnelDummy.position, baseCameraDistance, cameraRoundingThreshold);
                }
                if (state == State.Normal || state == State.Spin || state == State.Dialog)
                {
                    return currentRoom.RaisedPosition(center.position, baseCameraDistance, cameraRoundingThreshold);
                }
                else if (state == State.Death)
                {
                    return currentRoom.RaisedPosition(lastLivingPosition, baseCameraDistance, cameraRoundingThreshold);
                }
                if (state == State.BridgeJump)
                {
                    return ((launchMaxTime - launchTime) * jumpCameraSource + launchTime * jumpCameraDestination) / launchMaxTime;
                    //return jumpPosition + center.normal * 16;
                }
                else
                {
                    Vector3 side = Vector3.Cross(center.direction, center.normal);
                    float sideValue = 1f * launchTime / launchMaxTime;
                    Vector3 cameraBase = ((launchMaxTime - launchTime) * jumpCameraSource + launchTime * jumpCameraDestination) / launchMaxTime;
                    Vector3 camera = cameraBase + 5f*sideValue * (1 - sideValue) * side;
                    return camera;
                }
            }
        }

        public Vector3 cameraOffset
        {
            get
            {
                Vector3 cameraOut = baseCameraPos - cameraTarget;
                cameraOut.Normalize();
                Vector3 cameraRight = Vector3.Cross(cameraUp, cameraOut);
                return 5 * (cameraAngle.Y * cameraUp + cameraAngle.X * cameraRight);
            }
        }

        public Vector3 simpleCameraOffset
        {
            get
            {
                return 5 * (cameraUpTilt* cameraUp);
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
                else if(state == State.Death)
                    return currentRoom.AdjustedUp(lastLivingPosition, center.direction, center.normal, 1f);
                else if (state == State.Tunnel || state == State.Phase || state == State.PhaseFail)
                    return currentRoom.AdjustedUp(tunnelDummy.position, tunnelDummy.direction, tunnelDummy.normal, 1f);
                else
                    return currentRoom.AdjustedUp(center.position, center.direction, center.normal, 1f);
            }
        }

        public Vector3 cameraTarget
        {
            get
            {
                if (state == State.Normal || state == State.Spin || state == State.Dialog)
                    return center.position;
                else if (state == State.Tunnel)
                    return tunnelDummy.position;
                else if (state == State.Death)
                    return lastLivingPosition;
                else
                    return jumpPosition;
            }
        }

        public float maxVertSpeed
        {
            get
            {
                if ((leftWall && faceDirection == -1 && (GamePad.GetState(Game1.activePlayer).ThumbSticks.Left.X < 0 || wallTime < 500)) || (rightWall && faceDirection == 1 && (GamePad.GetState(Game1.activePlayer).ThumbSticks.Left.X > 0 || wallTime < 500)))
                {
                    if(Vector3.Dot(center.velocity, center.direction) < 0)
                        return .009f;
                }
                return .018f;
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

        public bool insideBox
        {
            get
            {
                if (state != State.Tunnel && state != State.PhaseFail && state != State.Phase) return false;
                float distanceFromMid = Math.Abs(Vector3.Dot(jumpPosition - currentRoom.center, center.normal));
                float boxHalfSize = .5f * Math.Abs(Vector3.Dot(currentRoom.size, center.normal));
                return distanceFromMid < boxHalfSize;
            }
        }

        public void Warp(Room targetRoom)
        {
            foreach (Doodad d in targetRoom.doodads)
            {
                if (d.type == VexedLib.DoodadType.WarpStation)
                {
                    currentRoom = targetRoom;
                    Physics.refresh = true;
                    Engine.reDraw = true;
                    center = new Vertex(d.position.position, d.position.normal, Vector3.Zero, d.position.direction);
                    state = State.Normal;
                }
            }
        }

        public void QuickSave()
        {
            LevelLoader.QuickSave();
            
            respawnPlayer = new Player();
            respawnPlayer.currentRoom = currentRoom;
            respawnPlayer.center = new Vertex(center.position, center.normal, Vector3.Zero, center.direction);
        }

        public void SaveGame()
        {
            LevelLoader.SaveToDisk(Engine.saveFileIndex);

            respawnPlayer = new Player();
            respawnPlayer.currentRoom = currentRoom;
            respawnPlayer.center = new Vertex(center.position, center.normal, Vector3.Zero, center.direction);
        }

        public void Respawn()
        {
            Engine.reDraw = true;
            LevelLoader.QuickLoad();
        }

        public bool HasTraction()
        {
            return primaryAbility.type == AbilityType.Boots || secondaryAbility.type == AbilityType.Boots || upgrades[(int)AbilityType.PermanentBoots];
        }

        public void SpinHook()
        {
            if (hookState == HookState.Waiting)
            {
                hookState = HookState.Out;
                hookShot = new Vertex(center.position + .5f * right * faceDirection, center.normal, .01f * up + .01f * right * faceDirection, center.direction);
                hookTime = 0;
            }
            else
            {
                foreach (Doodad d in currentRoom.doodads)
                {
                    if (d.type == VexedLib.DoodadType.HookTarget && (d.position.position - hookShot.position).Length() < .8f)
                    {
                        center.velocity = Vector3.Zero;
                        jumpSource = center.position;
                        jumpDestination = d.position.position + right * faceDirection - up;
                        state = State.Spin;
                        spinUp = -faceDirection * right;
                        hookHangTime = 0;
                        hookState = HookState.Hook;
                    }
                }
            }
        }

        public void Boost()
        {
            //center.velocity -= Vector3.Dot(center.velocity, up) * up;
            int boostDirection = faceDirection;
            if (leftWall == true)
                boostDirection = 1;
            if (rightWall == true)
                boostDirection = -1;

            center.velocity = right * maxHorizSpeed * boostDirection;
            faceDirection = boostDirection;
            
            boosting = true;
            boostTime = maxBoostTime;
        }

        public void Damage(Vector3 projection)
        {
            idleTime = 0;
            if (state == State.Death)
                return;
            center.velocity += 2*maxHorizSpeed * Vector3.Normalize(projection);
            if (primaryAbility.type == AbilityType.Shield && primaryAbility.ammo != 0)
            {
                primaryAbility.DepleteAmmo(800);
            }
            else if (secondaryAbility.type == AbilityType.Shield && secondaryAbility.ammo != 0)
            {
                secondaryAbility.DepleteAmmo(800);
            }
            else if (naturalShield.ammo != 0)
            {
                naturalShield.DepleteAmmo(800);
            }
            else
            {
                deathTime = 0;
                state = State.Death;
                lastLivingPosition = center.position;
            }
        }

        public void Spin(Vector3 newUp)
        {
            if (upgrades[(int)AbilityType.PermanentBoots]==true || primaryAbility.type == AbilityType.Boots || secondaryAbility.type == AbilityType.Boots)
            {
                jumpSource = center.position;
                jumpDestination = center.position;
                state = State.Spin;
                spinUp = newUp;
            }
        }

        public void AttemptPhase()
        {
            float throughDistance = Math.Abs(Vector3.Dot(center.normal, currentRoom.size));
            float sideSize = .5f * Math.Abs(Vector3.Dot(right, currentRoom.size));
            float sideDistance = Vector3.Dot(currentRoom.center - center.position, right) + sideSize;


            
            center.velocity = Vector3.Zero;
            jumpRoom = currentRoom;
            jumpSource = center.position;
            jumpNormal = -center.normal;

            Monster finalBoss = null;
            foreach (Monster m in currentRoom.monsters)
            {
                if (m.moveType == VexedLib.MovementType.FaceBoss)
                    finalBoss = m;
            }
            
            bool worldPhaseOK = Physics.PhaseTest();
            bool bossPhaseOK = true;
            if(finalBoss != null && (finalBoss.faceBoss.state == FaceState.Armored || finalBoss.faceBoss.state == FaceState.Rebuilding))
                bossPhaseOK = finalBoss.faceBoss.PhaseTest();
            if (true == worldPhaseOK && bossPhaseOK == true)
            {
                jumpDestination = center.position - throughDistance * center.normal;
                state = State.Phase;
                tunnelDummy = new Vertex(center.position, center.normal, (throughDistance + 2 * sideDistance) / (.5f * launchMaxTime) * right, center.direction);
            }
            else if (bossPhaseOK == true)
            {
                jumpDestination = center.position - (2f * throughDistance - 1f) * center.normal;
                state = State.PhaseFail;
                tunnelDummy = new Vertex(center.position, center.normal, Vector3.Zero, center.direction);
            }
            else
            {
                jumpDestination = center.position - 10.2f * center.normal;
                state = State.PhaseFail;
                tunnelDummy = new Vertex(center.position, center.normal, Vector3.Zero, center.direction);
            }

            launchTime = 0;
        }

        public void EnforceVelocityLimits()
        {
            float upMagnitude = Vector3.Dot(up, center.velocity);
            float rightMagnitude = Vector3.Dot(right, center.velocity - platformVelocity);


            if (upMagnitude > maxVertSpeed)
            {
                center.velocity -= (upMagnitude - maxVertSpeed) * up;
            }
            if (upMagnitude < -maxVertSpeed)
            {
                center.velocity -= (maxVertSpeed + upMagnitude) * up;
            }
            if (rightMagnitude > maxHorizSpeed && boosting == false)
            {
                center.velocity -= (rightMagnitude - maxHorizSpeed) * right;
            }
            if (rightMagnitude < -maxHorizSpeed && boosting == false)
            {
                center.velocity -= (maxHorizSpeed + rightMagnitude) * right;
            }
        }

        public void SetAnimationState()
        {
            if (state == State.Dialog)
            {
                if (faceDirection < 0)
                    AnimationControl.SetState(AnimationState.IdleLeft);
                if (faceDirection > 0)
                    AnimationControl.SetState(AnimationState.IdleRight);
                if (faceDirection == 0)
                    AnimationControl.SetState(AnimationState.Idle);
            }
            else if (state == State.Death)
            {
                AnimationControl.SetState(AnimationState.JumpPad);
            }
            else if (state == State.Jump || state == State.Tunnel || state == State.Phase || state == State.PhaseFail)
            {
                AnimationControl.SetState(AnimationState.JumpPad);
            }
            else if (lastFireTime > 1000)
            {
                if (boosting)
                {
                    if (faceDirection < 0)
                        AnimationControl.SetState(AnimationState.BoostLeft);
                    if (faceDirection > 0)
                        AnimationControl.SetState(AnimationState.BoostRight);
                }
                else if (jetPacking)
                {
                    if (faceDirection < 0)
                        AnimationControl.SetState(AnimationState.FlyLeft);
                    if (faceDirection > 0)
                        AnimationControl.SetState(AnimationState.FlyRight);
                }
                else if (walking)
                {
                    if (faceDirection < 0)
                        AnimationControl.SetState(AnimationState.RunLeft);
                    if (faceDirection > 0)
                        AnimationControl.SetState(AnimationState.RunRight);
                }
                else if (grounded == false && leftWall == true && faceDirection < 0)
                {
                    AnimationControl.SetState(AnimationState.WallRight);
                }
                else if (grounded == false && rightWall == true && faceDirection > 0)
                {
                    AnimationControl.SetState(AnimationState.WallLeft);
                }
                else if (grounded == false)
                {
                    if (faceDirection < 0)
                        AnimationControl.SetState(AnimationState.JumpLeft);
                    if (faceDirection > 0)
                        AnimationControl.SetState(AnimationState.JumpRight);
                }
                else
                {
                    if (faceDirection < 0)
                        AnimationControl.SetState(AnimationState.IdleLeft);
                    if (faceDirection > 0)
                        AnimationControl.SetState(AnimationState.IdleRight);
                    if (faceDirection == 0)
                        AnimationControl.SetState(AnimationState.Idle);
                }
            }
            else
            {
                if (boosting)
                {
                    if (faceDirection < 0)
                        AnimationControl.SetState(AnimationState.BoostLeft);
                    if (faceDirection > 0)
                        AnimationControl.SetState(AnimationState.BoostRight);
                }
                else if (jetPacking)
                {
                    if (faceDirection < 0)
                        AnimationControl.SetState(AnimationState.FlyLeftFiring);
                    if (faceDirection > 0)
                        AnimationControl.SetState(AnimationState.FlyRightFiring);
                }
                else if (walking)
                {
                    if (faceDirection < 0)
                        AnimationControl.SetState(AnimationState.RunLeftFiring);
                    if (faceDirection > 0)
                        AnimationControl.SetState(AnimationState.RunRightFiring);
                }
                else if (grounded == false && leftWall == true && faceDirection < 0)
                {
                    AnimationControl.SetState(AnimationState.WallRightFiring);
                }
                else if (grounded == false && rightWall == true && faceDirection > 0)
                {
                    AnimationControl.SetState(AnimationState.WallLeftFiring);
                }
                else if (grounded == false)
                {
                    if (faceDirection < 0)
                        AnimationControl.SetState(AnimationState.JumpLeftFiring);
                    if (faceDirection > 0)
                        AnimationControl.SetState(AnimationState.JumpRightFiring);
                }
                else
                {
                    if (faceDirection < 0)
                        AnimationControl.SetState(AnimationState.IdleLeftFiring);
                    if (faceDirection > 0)
                        AnimationControl.SetState(AnimationState.IdleRightFiring);
                    if (faceDirection == 0)
                        AnimationControl.SetState(AnimationState.Idle);
                }
            }            
        }

        public void ResetAdjacentRooms()
        {
            foreach (Room r in Engine.roomList)
            {
                if (r == currentRoom || r == jumpRoom)
                    r.adjacent = true;
                else
                    r.adjacent = false;
            }
            foreach (Doodad d in currentRoom.doodads)
            {
                if (d.type == VexedLib.DoodadType.JumpPad || d.type == VexedLib.DoodadType.JumpStation)
                {
                    if(d.targetRoom != null)
                        d.targetRoom.adjacent = true;
                }
            }
            if (jumpRoom != null)
            {
                foreach (Doodad d in jumpRoom.doodads)
                {
                    if (d.type == VexedLib.DoodadType.JumpPad || d.type == VexedLib.DoodadType.JumpStation)
                    {
                        if (d.targetRoom != null)
                            d.targetRoom.adjacent = true;
                    }
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            wallJumpCooldown -= gameTime.ElapsedGameTime.Milliseconds;
            if (wallJumpCooldown < 0) wallJumpCooldown = 0;
            if (leftWall == true || rightWall == true)
            {
                wallTime += gameTime.ElapsedGameTime.Milliseconds;
            }
            else
                wallTime = 0;

            if (grounded == true)
            {
                wallTime = 0;
                referenceFrameSpeed = Vector3.Dot(platformVelocity, right);
            }
            else
            {
                float currentSpeed = Vector3.Dot(center.velocity, right);
                Vector2 controlStick = GamePad.GetState(Game1.activePlayer).ThumbSticks.Left;
                if (!(Game1.controller.AButton.Pressed == true ||  Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.Right) ||
                    Keyboard.GetState().IsKeyDown(Keys.A) || (Keyboard.GetState().IsKeyDown(Keys.D)) || Math.Abs(controlStick.X) > .1f))
                {
                    if (currentSpeed > referenceFrameSpeed)
                    {
                        center.velocity -= .0005f * right;
                    }
                    if (currentSpeed < referenceFrameSpeed)
                    {
                        center.velocity += .0005f * right;
                    }
                }
            }
            

            center.direction.Normalize();
            if (state == State.Tunnel || state == State.Phase || state == State.PhaseFail)
            {
                if (launchTime > launchMaxTime / 4 && launchTime < 3 * launchMaxTime / 4)
                    tunnelDummy.Update(currentRoom, gameTime.ElapsedGameTime.Milliseconds);
                if (launchTime > 3 * launchMaxTime / 4 && tunnelExit != null)
                    tunnelExit.ActivateDoodad(currentRoom, true);
                oldUp = tunnelDummy.direction;
                oldNormal = tunnelDummy.normal;
            }
            else
            {
                oldUp = center.direction;
                oldNormal = center.normal;
            }

            if (hookState != HookState.Waiting)
            {
                if(hookState == HookState.Hook && state != State.Spin)
                    hookHangTime += gameTime.ElapsedGameTime.Milliseconds;
                else if (hookState == HookState.Out)
                    hookTime += gameTime.ElapsedGameTime.Milliseconds;
                else if (hookState == HookState.In)
                    hookTime -= gameTime.ElapsedGameTime.Milliseconds;
                if (hookState == HookState.Out)
                {
                    hookShot = new Vertex(center.position + .5f * right * faceDirection, center.normal, .01f * up + .01f * right * faceDirection, center.direction);
                    hookShot.Update(currentRoom, hookTime);
                    foreach (Doodad d in currentRoom.doodads)
                    {
                        if (d.type == VexedLib.DoodadType.HookTarget && (d.position.position - hookShot.position).Length() < .8f)
                        {
                            center.velocity = Vector3.Zero;
                            jumpSource = center.position;
                            jumpDestination = d.position.position + right * faceDirection - up;
                            state = State.Spin;
                            spinUp = -faceDirection * right;
                            hookHangTime = 0;
                            hookState = HookState.Hook;
                        }
                    }
                }                    
                if (hookState == HookState.In)
                {
                    hookShot = new Vertex(center.position + .5f * right * faceDirection, center.normal, .01f * up + .01f * right * faceDirection, center.direction);
                    hookShot.Update(currentRoom, hookTime);
                }

                if (hookState == HookState.Hook)
                {
                    foreach (Doodad d in currentRoom.doodads)
                    {
                        if (d.type == VexedLib.DoodadType.HookTarget && (d.position.position - hookShot.position).Length() < .8f)
                        {
                            Vector3 vel = .01f * up + .01f * right * faceDirection;

                            float distance = (d.position.position - (center.position + .5f * right * faceDirection)).Length();
                            hookTime = (int)(distance / vel.Length());

                            hookShot = new Vertex(d.position.position, center.normal, Vector3.Zero, center.direction);
                        }
                    }
                    if (state != State.Spin)
                    {
                        hookShot = new Vertex(center.position + .5f * right * faceDirection, center.normal, .01f * up + .01f * right * faceDirection, center.direction);
                        hookShot.Update(currentRoom, hookTime);
                    }

                }

                    
                if (hookTime > maxHookTime && hookState == HookState.Out)
                {
                    hookLand = hookShot.position;
                    hookShot.velocity = -hookShot.velocity;
                    hookHangTime = 0;                            
                    hookState = HookState.Hook;
                }
                if (hookTime < 0 && hookState == HookState.In)
                {
                    hookState = HookState.Waiting;
                }
                if (hookHangTime > maxHookHangTime && hookState == HookState.Hook)
                {                    
                    hookState = HookState.In;
                } 
            }

            SetAnimationState();
            primaryAbility.Update(gameTime);
            secondaryAbility.Update(gameTime);

            if (grounded && !walking)
                idleTime += gameTime.ElapsedGameTime.Milliseconds;
            else
                idleTime = 0;

            lastFireTime += gameTime.ElapsedGameTime.Milliseconds;

            if (idleTime > 2000)
            {
                if (primaryAbility.type == AbilityType.Shield)
                {
                    primaryAbility.AddAmmo(gameTime.ElapsedGameTime.Milliseconds);
                }
                if (secondaryAbility.type == AbilityType.Shield)
                {
                    secondaryAbility.AddAmmo(gameTime.ElapsedGameTime.Milliseconds);
                }
            }
            if (grounded)
                jetPacking = false;

            if (dead == true)
                Respawn();
            
            cameraAngle = (cameraAngle + targetCameraAngle) / 2;

            groundCounter += gameTime.ElapsedGameTime.Milliseconds;
            if (groundCounter > groundTolerance)
            {
                groundCounter = groundTolerance;
                grounded = false;
            }
            Vector2 stick = GamePad.GetState(Game1.activePlayer).ThumbSticks.Left;
            Vector2 rightStick = GamePad.GetState(Game1.activePlayer).ThumbSticks.Right;
            GamePadState gamePadState = GamePad.GetState(Game1.activePlayer);

            if (state == State.Death)
            {
                center.position += center.velocity * gameTime.ElapsedGameTime.Milliseconds;
                center.velocity -= gravityAcceleration * up;
                EnforceVelocityLimits();
                deathTime += gameTime.ElapsedGameTime.Milliseconds;
                if (deathTime > maxDeathTime)
                {
                    Respawn();
                }
            }
            else if (state == State.Normal)
            {
                #region normal update
                float upMagnitude = 0;
                float rightMagnitude = 0;

                walkTime += gameTime.ElapsedGameTime.Milliseconds;
                if (walkTime > walkMaxTime) walkTime -= walkMaxTime;
                center.Update(currentRoom, gameTime.ElapsedGameTime.Milliseconds);
                jumpRecovery -= gameTime.ElapsedGameTime.Milliseconds;
                if (jumpRecovery < 0) jumpRecovery = 0;
                boostTime -= gameTime.ElapsedGameTime.Milliseconds;
                if (boostTime < 0) boostTime = 0;
                if (boostTime == 0)
                    boosting = false;

                Vector3 up = center.direction;
                Vector3 right = Vector3.Cross(up, center.normal);

                //if (grounded == true)
                //faceDirection = 0;

                if (wallJumpCooldown == 0)
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.A))
                    {
                        faceDirection = -1;
                        if (grounded == true)
                            center.velocity -= walkSpeed * right;
                        else
                            center.velocity -= airSpeed * right;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.Right) || Keyboard.GetState().IsKeyDown(Keys.D))
                    {
                        faceDirection = 1;
                        if (grounded == true)
                            center.velocity += walkSpeed * right;
                        else
                            center.velocity += airSpeed * right;
                    }
                    if (Math.Abs(stick.X) > .2)
                    {
                        if (grounded == true)
                            center.velocity += walkSpeed * stick.X * right;
                        else
                            center.velocity += airSpeed * stick.X * right;
                        if (stick.X > .2) faceDirection = 1;
                        if (stick.X < -.2) faceDirection = -1;
                    }
                }

                if (superJump == true)
                {
                    jumpTime += gameTime.ElapsedGameTime.Milliseconds;

                    if (jumpTime > 45 && grounded == true)
                        center.velocity += (jumpSpeed - upMagnitude) * up;
                    int jumpLimit = 60;
                    if (upgrades[(int)AbilityType.ImprovedJump])
                        jumpLimit = 150;
                    if (jumpTime > jumpLimit)
                        superJump = false;
                    if (jumpTime < 45 && grounded == false)
                        superJump = false;
                    if (Game1.controller.AButton.Pressed == false && jumpTime > 45)
                        superJump = false;
                }

                if (Game1.controller.BackButton.NewPressed)
                {
                    dead = true;
                }
                if (Game1.controller.AButton.NewPressed && jumpRecovery == 0)
                {

                    Game1.controller.AButton.Invalidate();
                    upMagnitude = Vector3.Dot(up, center.velocity);
                    rightMagnitude = Vector3.Dot(right, center.velocity);
                    if (grounded)
                    {
                        if (upMagnitude < jumpSpeed)
                        {
                            superJump = true;
                            jumpTime = 0;
                            jetPacking = false;
                            //center.velocity += (jumpSpeed - upMagnitude) * up;
                            jumpRecovery = jumpRecoveryMax;
                        }
                    }
                    else if (leftWall && faceDirection < 0 && (upgrades[(int)AbilityType.PermanentWallJump] == true || primaryAbility.type == AbilityType.WallJump || secondaryAbility.type == AbilityType.WallJump))
                    {
                        faceDirection = 1;
                        if (upMagnitude < jumpSpeed)
                            center.velocity += (jumpSpeed - upMagnitude) * up;
                        if (rightMagnitude < wallJumpSpeed)
                            center.velocity += (wallJumpSpeed - rightMagnitude) * right;
                        jumpRecovery = jumpRecoveryMax;
                        wallJumpCooldown = wallJumpCooldownMax;

                    }
                    else if (rightWall && faceDirection > 0 && (upgrades[(int)AbilityType.PermanentWallJump] == true || primaryAbility.type == AbilityType.WallJump || secondaryAbility.type == AbilityType.WallJump))
                    {
                        faceDirection = -1;
                        if (upMagnitude < jumpSpeed)
                            center.velocity += (jumpSpeed - upMagnitude) * up;
                        if (rightMagnitude > -wallJumpSpeed)
                            center.velocity -= (wallJumpSpeed + rightMagnitude) * right;
                        jumpRecovery = jumpRecoveryMax;
                        wallJumpCooldown = wallJumpCooldownMax;
                    }
                    else
                    {
                        if (primaryAbility.type == AbilityType.DoubleJump && primaryAbility.ammo != 0)
                        {
                            if (upMagnitude < jumpSpeed)
                            {
                                jetPacking = false;
                                center.velocity += (jumpSpeed - upMagnitude) * up;
                                jumpRecovery = jumpRecoveryMax;
                                primaryAbility.ammo--;
                            }
                        }
                        else if (secondaryAbility.type == AbilityType.DoubleJump && secondaryAbility.ammo != 0)
                        {
                            if (upMagnitude < jumpSpeed)
                            {
                                jetPacking = false;
                                center.velocity += (jumpSpeed - upMagnitude) * up;
                                jumpRecovery = jumpRecoveryMax;
                                secondaryAbility.ammo--;
                            }
                        }
                    }
                }

                if ((gamePadState.IsButtonDown(Buttons.Back)))
                {
                    Respawn();
                }


                if (boosting == false)
                    center.velocity -= gravityAcceleration * up;
                else
                    center.velocity += boostAcceleration * right * faceDirection;

                targetCameraAngle = rightStick + new Vector2(0,cameraUpTilt);

                EnforceVelocityLimits();
                jetPackThrust = false;
                if (Game1.controller.XButton.Pressed)
                {
                    Doodad itemStation = null;
                    bool stationPresent = false;
                    foreach (Doodad d in currentRoom.doodads)
                    {
                        if (d.active && (d.type == VexedLib.DoodadType.ItemStation || d.type == VexedLib.DoodadType.ItemBlock))
                        {
                            if (Game1.controller.XButton.NewPressed && d.cooldown == 0 && upgrades[(int)d.abilityType] == true)
                            {
                                itemStation = d;
                            }
                            stationPresent = true;
                        }
                    }

                    if (stationPresent == false)
                    {
                        primaryAbility.Do(gameTime);
                        Game1.controller.XButton.Invalidate();
                    }
                    else if (itemStation != null)
                    {
                        AbilityType swapAbilityType = primaryAbility.type;
                        primaryAbility = new Ability(itemStation.abilityType);
                        itemStation.abilityType = swapAbilityType;
                        itemStation.cooldown = itemStation.maxCooldown;
                    }
                }
                if (Game1.controller.YButton.Pressed)
                {
                    Doodad itemStation = null;
                    bool stationPresent = false;

                    foreach (Doodad d in currentRoom.doodads)
                    {
                        if (d.active && (d.type == VexedLib.DoodadType.ItemStation || d.type == VexedLib.DoodadType.ItemBlock))
                        {
                            if (Game1.controller.YButton.NewPressed && d.cooldown == 0 && upgrades[(int)d.abilityType] == true)
                            {
                                itemStation = d;
                            }
                            stationPresent = true;
                        }
                    }
                    if (stationPresent == false)
                    {
                        secondaryAbility.Do(gameTime);
                        Game1.controller.YButton.Invalidate();
                    }
                    else if (itemStation != null)
                    {
                        AbilityType swapAbilityType = secondaryAbility.type;
                        secondaryAbility = new Ability(itemStation.abilityType);
                        itemStation.abilityType = swapAbilityType;
                        itemStation.cooldown = itemStation.maxCooldown;
                    }
                }
                if (Game1.controller.BButton.Pressed)
                {

                    foreach (Doodad d in currentRoom.doodads)
                    {
                        if (d.powered == false && d.ActivationRange(this) && d.activationCost != 0)
                        {
                            DialogBox.SetDialog("PowerUp");
                        }
                        if (d.active)
                        {
                            if (d.type == VexedLib.DoodadType.Vortex && (d.position.position - center.position).Length() < .3f)
                            {
                                d.targetDoodad.ActivateDoodad(currentRoom, true);
                                tunnelExit = d.targetDoodad;
                                state = State.Tunnel;
                                center.velocity = Vector3.Zero;
                                jumpRoom = currentRoom;
                                jumpSource = center.position;
                                jumpDestination = d.targetDoodad.position.position;                                
                                jumpNormal = -center.normal;

                                float throughDistance = Math.Abs(Vector3.Dot(center.normal, currentRoom.size));
                                float sideSize = .5f*Math.Abs(Vector3.Dot(right, currentRoom.size));
                                float sideDistance = Vector3.Dot(currentRoom.center - center.position, right) + sideSize;
                                
                                tunnelDummy = new Vertex(center.position, center.normal, (throughDistance + 2*sideDistance)/(.5f*launchMaxTime)*right, center.direction);
                                launchTime = 0;
                                d.targetDoodad.active = false;
                            }
                            if (d.type == VexedLib.DoodadType.BridgeGate)
                            {
                                if (Vector3.Dot(center.velocity, d.position.direction) > 0)
                                {
                                    jumpRoom = d.targetRoom;
                                    jumpSource = center.position;
                                    jumpDestination = d.targetDoodad.position.position - 2f * d.targetDoodad.upUnit;
                                    jumpCameraSource = currentRoom.RaisedPosition(jumpSource + cameraOffset, baseCameraDistance, cameraRoundingThreshold);
                                    jumpCameraDestination = jumpRoom.RaisedPosition(jumpDestination + simpleCameraOffset, baseCameraDistance, cameraRoundingThreshold);
                                    
                                    jumpNormal = center.normal;
                                    launchTime = 0;
                                    state = State.BridgeJump;
                                    faceDirection = 0;
                                    d.active = false;
                                    d.targetDoodad.active = false;
                                }
                            }
                            if (d.type == VexedLib.DoodadType.LoadStation)
                            {
                                if (d.id.Contains("Slot1"))
                                    Engine.saveFileIndex = 1;
                                if (d.id.Contains("Slot2"))
                                    Engine.saveFileIndex = 2;
                                if (d.id.Contains("Slot3"))
                                    Engine.saveFileIndex = 3;
                                if (d.id.Contains("Slot4"))
                                    Engine.saveFileIndex = 4;
                                LevelLoader.LoadFromDisk(Engine.saveFileIndex);
                                Physics.refresh = true;
                                Engine.reDraw = true;
                            }
                            if (d.type == VexedLib.DoodadType.PowerStation)
                            {
                                if (d.orbsRemaining > 0)
                                {
                                    orbsCollected += 1;
                                    d.orbsRemaining--;
                                    currentRoom.currentOrbs++;
                                    currentRoom.parentSector.currentOrbs++;
                                    //Engine.reDraw = true;
                                    currentRoom.refreshVertices = true;
                                }
                            }
                            if (d.type == VexedLib.DoodadType.SaveStation && d.cooldown == 0)
                            {
                                d.cooldown = d.maxCooldown;
                                SaveGame();
                            }
                            if (d.type == VexedLib.DoodadType.UpgradeStation)
                            {
                                if (d.abilityType == AbilityType.Ultima)
                                {
                                    for (int i = 0; i < 32; i++)
                                        upgrades[i] = true;
                                }
                                upgrades[(int)d.abilityType] = true;
                                d.abilityType = AbilityType.Empty;
                            }
                            if (d.type == VexedLib.DoodadType.JumpPad || d.type == VexedLib.DoodadType.JumpStation)
                            {
                                if (d.type == VexedLib.DoodadType.JumpPad)
                                    d.Activate();
                                Engine.reDraw = true;
                                SoundFX.RoomJump();
                                jumpRoom = d.targetRoom;
                                jumpRoom.Reset();
                                ResetAdjacentRooms();
                                
                                float roomSize = Math.Abs(Vector3.Dot(jumpRoom.size / 2, center.normal));
                                jumpSource = center.position;
                                jumpDestination = center.position + Vector3.Dot(jumpRoom.center - center.position - roomSize * center.normal, center.normal) * center.normal;
                                // Base Camera
                                jumpCameraSource = currentRoom.RaisedPosition(jumpSource + cameraOffset, baseCameraDistance, cameraRoundingThreshold);
                                jumpCameraDestination = jumpRoom.RaisedPosition(jumpDestination + simpleCameraOffset, baseCameraDistance, cameraRoundingThreshold);
                                    
                                state = State.Jump;
                                center.velocity = Vector3.Zero;
                                launchTime = 0;
                                d.active = false;
                                jumpNormal = -center.normal;
                            }
                            if (d.type == VexedLib.DoodadType.NPC_OldMan)
                            {
                                DialogBox.SetDialog("Default");
                                state = State.Dialog;
                                
                            }
                            if (d.type == VexedLib.DoodadType.SwitchStation)
                            {
                                bool locked = false;
                                if (d.abilityType == AbilityType.RedKey && !(primaryAbility.type == AbilityType.RedKey || secondaryAbility.type == AbilityType.RedKey || upgrades[(int)AbilityType.PermanentRedKey] == true))
                                    locked = true;
                                if (d.abilityType == AbilityType.BlueKey && !(primaryAbility.type == AbilityType.BlueKey || secondaryAbility.type == AbilityType.BlueKey || upgrades[(int)AbilityType.PermanentBlueKey] == true))
                                    locked = true;
                                if (d.abilityType == AbilityType.YellowKey && !(primaryAbility.type == AbilityType.YellowKey || secondaryAbility.type == AbilityType.YellowKey || upgrades[(int)AbilityType.PermanentYellowKey] == true))
                                    locked = true;
                                if (locked == false)
                                {

                                    if (d.targetDoodad != null)
                                    {
                                        if (d.targetDoodad.currentBehavior.id == d.expectedBehavior)
                                        {
                                            foreach (Behavior b in d.targetDoodad.behaviors)
                                            {
                                                if (b.id == d.targetBehavior)
                                                {
                                                    d.targetDoodad.SetBehavior(b);
                                                }
                                            }
                                        }
                                    }
                                    if (d.targetBlock != null)
                                    {
                                        if (d.targetBlock.currentBehavior.id == d.expectedBehavior)
                                        {
                                            foreach (Behavior b in d.targetBlock.behaviors)
                                            {
                                                if (b.id == d.targetBehavior)
                                                {
                                                    d.targetBlock.SetBehavior(b);
                                                }
                                            }
                                        }
                                    }
                                    if (d.targetEdge != null)
                                    {
                                        if (d.targetEdge.currentBehavior.id == d.expectedBehavior)
                                        {
                                            foreach (Behavior b in d.targetEdge.behaviors)
                                            {
                                                if (b.id == d.targetBehavior)
                                                {
                                                    d.targetEdge.SetBehavior(b);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (d.type == VexedLib.DoodadType.WarpStation)
                            {
                                Engine.state = EngineState.Map;
                                WorldMap.state = ZoomState.ZoomToSector;
                                WorldMap.warp = true;
                                WorldMap.ZoomToSector();
                                
                                for (int i = 0; i < Engine.roomList.Count(); i++)
                                {
                                    if (Engine.roomList[i] == currentRoom)
                                        WorldMap.selectedRoomIndex = i;
                                }
                            }
                        }
                    }
                }
                foreach (Doodad d in currentRoom.doodads)
                {
                    if (d.active)
                    {
                        if (d.type == VexedLib.DoodadType.BridgeGate)
                        {
                            jumpRoom = d.targetRoom;
                            jumpRoom.Reset();
                            jumpSource = center.position;
                            jumpDestination = d.targetDoodad.position.position - 1f * d.targetDoodad.upUnit;

                            // Base Camera
                            jumpCameraSource = currentRoom.RaisedPosition(jumpSource + cameraOffset, baseCameraDistance, cameraRoundingThreshold);
                            jumpCameraDestination = jumpRoom.RaisedPosition(jumpDestination + simpleCameraOffset, baseCameraDistance, cameraRoundingThreshold);
                                    
                            
                            jumpNormal = center.normal;
                            launchTime = 0;
                            state = State.BridgeJump;
                            faceDirection = 0;
                            d.active = false;
                            d.targetDoodad.active = false;
                        }
                    }
                }
                #endregion
            }
            if (state == State.Jump || state == State.BridgeJump || state == State.Tunnel || state == State.Phase || state == State.PhaseFail)
            {
                launchTime += gameTime.ElapsedGameTime.Milliseconds;
                if (launchTime > launchMaxTime)
                    launchTime = launchMaxTime;
                jumpPosition = ((launchMaxTime - launchTime) * jumpSource + launchTime * (jumpDestination+.3f*jumpNormal)) / launchMaxTime;
                if (state == State.PhaseFail && jumpDestination != center.position && launchTime > launchMaxTime / 2)
                {
                    jumpSource = jumpDestination;                   
                    jumpDestination = center.position;
                    jumpNormal = center.normal;
                }
                if (launchTime == launchMaxTime)
                {
                    center.normal = jumpNormal;
                    center.position = jumpDestination;
                    //center.velocity = Vector3.Zero;
                    faceDirection = 0;
                    State oldState = state;
                    state = State.Normal;
                    currentRoom = jumpRoom;


                    if (oldState == State.Jump)
                    {
                        naturalShield.ammo = naturalShield.maxAmmo;
                        QuickSave();
                    }
                    
                    Physics.refresh = true;
                    Engine.reDraw = true;
                    foreach (Doodad d in jumpRoom.doodads)
                    {
                        if(d.type == VexedLib.DoodadType.BridgeGate)
                            d.active = false;
                    }

                    jumpRoom = null;
                    ResetAdjacentRooms();

                }

            }
            if (state == State.Spin)
            {
                spinTime += gameTime.ElapsedGameTime.Milliseconds;
                center.position = (spinTime * jumpDestination + (spinMaxTime - spinTime) * jumpSource) / spinMaxTime;
                    
                if (spinTime > spinMaxTime)
                {
                    spinTime = 0;
                    center.direction = spinUp;
                    
                    state = State.Normal;
                    if (hookState == HookState.Hook)
                    {
                        hookHangTime = maxHookHangTime;
                    }
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

        public void DrawTexture(AlphaTestEffect playerEffect)
        {
            if (primaryAbility.isGun || secondaryAbility.isGun)
            {
                playerEffect.Texture = Player.player_gun_textures;
                playerEffect.CurrentTechnique.Passes[0].Apply();
            }
            else
            {
                playerEffect.Texture = Player.player_textures;
                playerEffect.CurrentTechnique.Passes[0].Apply();
            }

            if (state == State.BridgeJump)
            {
                if ((jumpPosition - jumpDestination).Length() > .75f && (jumpPosition - center.position).Length() > .75f)
                    return;
            }

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
            if (state == State.Death)
            {
                foreach (Vertex v in rectVertexList)
                {
                    v.SimpleUpdate(currentRoom, 1);
                }
            }
            else
            {
                foreach (Vertex v in rectVertexList)
                {
                    v.Update(currentRoom, 1);
                }
            }
            


            currentRoom.AddTextureToTriangleList(rectVertexList, Color.White, .3f, textureTriangleList, Player.texCoordList[currentTextureIndex], true);


            VertexPositionColorNormalTexture[] triangleArray = textureTriangleList.ToArray();
            if (state == State.Jump || state == State.BridgeJump || state == State.Tunnel || state == State.Phase || state == State.PhaseFail)
            {
                for (int i = 0; i < textureTriangleList.Count(); i++)
                {
                     triangleArray[i].Position += jumpPosition - center.position;
                }
            }
            if (state == State.Death)
            {
                for (int i = 0; i < textureTriangleList.Count(); i++)
                {
                    triangleArray[i].Position += .01f * deathTime * center.normal;
                }
            }


            Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                triangleArray, 0, triangleArray.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);

            if (hookState != HookState.Waiting)
            {
                List<VertexPositionColorNormalTexture> hookTriangleList = new List<VertexPositionColorNormalTexture>();
                List<Vertex> hookVertexList = new List<Vertex>();
                Vector3 hookUp = hookShot.direction;
                Vector3 hookRight = Vector3.Cross(hookShot.direction, hookShot.normal);
                hookVertexList.Add(new Vertex(hookShot.position, hookShot.normal, +playerHalfHeight * hookUp + .5f * hookRight, hookShot.direction));
                hookVertexList.Add(new Vertex(hookShot.position, hookShot.normal, +playerHalfHeight * hookUp - .5f * hookRight, hookShot.direction));
                hookVertexList.Add(new Vertex(hookShot.position, hookShot.normal, -playerHalfHeight * hookUp - .5f * hookRight, hookShot.direction));
                hookVertexList.Add(new Vertex(hookShot.position, hookShot.normal, -playerHalfHeight * hookUp + .5f * hookRight, hookShot.direction));
                foreach (Vertex v in hookVertexList)
                    v.Update(currentRoom, 1);


                currentRoom.AddTextureToTriangleList(hookVertexList, Color.White, .5f, hookTriangleList, Ability.texCoordList[7], true);

                VertexPositionColorNormalTexture[] hookArray = hookTriangleList.ToArray();

                playerEffect.Texture = Ability.ability_textures;
                playerEffect.CurrentTechnique.Passes[0].Apply();

                Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                    hookArray, 0, hookArray.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
            }

            if (primaryAbility.isBoots || secondaryAbility.isBoots)
            {
                playerEffect.Texture = Player.player_boots_textures;
                playerEffect.CurrentTechnique.Passes[0].Apply();
                Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                    triangleArray, 0, triangleArray.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
            }
            if (primaryAbility.type == AbilityType.Booster || secondaryAbility.type == AbilityType.Booster)
            {
                playerEffect.Texture = Player.player_booster_textures;
                playerEffect.CurrentTechnique.Passes[0].Apply();
                Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                    triangleArray, 0, triangleArray.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                if (boosting == true)
                {
                    float flameDepth = .35f;
                    if (faceDirection < 0)
                    {
                        List<VertexPositionColorNormalTexture> jetFlameTriangleList = new List<VertexPositionColorNormalTexture>();
                        List<Vertex> flameVertexList = new List<Vertex>();
                        flameVertexList.Add(new Vertex(center.position, center.normal, +.8f*playerHalfHeight * up + 1.3f * right, center.direction));
                        flameVertexList.Add(new Vertex(center.position, center.normal, +.8f* playerHalfHeight * up - .1f * right, center.direction));
                        flameVertexList.Add(new Vertex(center.position, center.normal, -.6f * playerHalfHeight * up - .1f * right, center.direction));
                        flameVertexList.Add(new Vertex(center.position, center.normal, -.6f * playerHalfHeight * up + 1.3f * right, center.direction));
                        foreach (Vertex v in flameVertexList)
                        {
                            v.Update(currentRoom, 1);
                        }
                        currentRoom.AddTextureToTriangleList(flameVertexList, Color.White, flameDepth, jetFlameTriangleList, Ability.texCoordList[33], true);
                        playerEffect.Texture = Ability.ability_textures;
                        playerEffect.CurrentTechnique.Passes[0].Apply();
                        Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                            jetFlameTriangleList.ToArray(), 0, jetFlameTriangleList.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                    }
                    if (faceDirection > 0)
                    {
                        List<VertexPositionColorNormalTexture> jetFlameTriangleList = new List<VertexPositionColorNormalTexture>();
                        List<Vertex> flameVertexList = new List<Vertex>();
                        flameVertexList.Add(new Vertex(center.position, center.normal, +.8f * playerHalfHeight * up + .1f * right, center.direction));
                        flameVertexList.Add(new Vertex(center.position, center.normal, +.8f * playerHalfHeight * up - 1.3f * right, center.direction));
                        flameVertexList.Add(new Vertex(center.position, center.normal, -.6f * playerHalfHeight * up - 1.3f * right, center.direction));
                        flameVertexList.Add(new Vertex(center.position, center.normal, -.6f * playerHalfHeight * up + .1f * right, center.direction));
                        foreach (Vertex v in flameVertexList)
                        {
                            v.Update(currentRoom, 1);
                        }
                        currentRoom.AddTextureToTriangleList(flameVertexList, Color.White, flameDepth, jetFlameTriangleList, Ability.texCoordList[33], false);
                        playerEffect.Texture = Ability.ability_textures;
                        playerEffect.CurrentTechnique.Passes[0].Apply();
                        Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                            jetFlameTriangleList.ToArray(), 0, jetFlameTriangleList.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                    }
                }
            }
            if (primaryAbility.type == AbilityType.JetPack || secondaryAbility.type == AbilityType.JetPack)
            {
                playerEffect.Texture = Player.player_jetpack_textures;
                playerEffect.CurrentTechnique.Passes[0].Apply();
                Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                    triangleArray, 0, triangleArray.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                if (jetPackThrust == true)
                {
                    float flameDepth = .25f;
                    if (faceDirection != 0)
                        flameDepth = .35f;
                    if (faceDirection <= 0)
                    {
                        List<VertexPositionColorNormalTexture> jetFlameTriangleList = new List<VertexPositionColorNormalTexture>();
                        List<Vertex> flameVertexList = new List<Vertex>();
                        flameVertexList.Add(new Vertex(center.position, center.normal, -.3f * playerHalfHeight * up + .43f * right, center.direction));
                        flameVertexList.Add(new Vertex(center.position, center.normal, -.3f * playerHalfHeight * up - .17f * right, center.direction));
                        flameVertexList.Add(new Vertex(center.position, center.normal, -1.3f * playerHalfHeight * up - .17f * right, center.direction));
                        flameVertexList.Add(new Vertex(center.position, center.normal, -1.3f*playerHalfHeight * up + .43f * right, center.direction));
                        foreach (Vertex v in flameVertexList)
                        {
                            v.Update(currentRoom, 1);
                        }
                        currentRoom.AddTextureToTriangleList(flameVertexList, Color.White, flameDepth, jetFlameTriangleList, Ability.texCoordList[32], true);
                        playerEffect.Texture = Ability.ability_textures;
                        playerEffect.CurrentTechnique.Passes[0].Apply();
                        Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                            jetFlameTriangleList.ToArray(), 0, jetFlameTriangleList.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                    }
                    if (faceDirection >= 0)
                    {
                        List<VertexPositionColorNormalTexture> jetFlameTriangleList = new List<VertexPositionColorNormalTexture>();
                        List<Vertex> flameVertexList = new List<Vertex>();
                        flameVertexList.Add(new Vertex(center.position, center.normal,-.3f * playerHalfHeight * up + .17f * right, center.direction));
                        flameVertexList.Add(new Vertex(center.position, center.normal, -.3f * playerHalfHeight * up - .43f * right, center.direction));
                        flameVertexList.Add(new Vertex(center.position, center.normal, -1.3f*playerHalfHeight * up -.43f * right, center.direction));
                        flameVertexList.Add(new Vertex(center.position, center.normal, -1.3f*playerHalfHeight * up + .17f * right, center.direction));
                        foreach (Vertex v in flameVertexList)
                        {
                            v.Update(currentRoom, 1);
                        }
                        currentRoom.AddTextureToTriangleList(flameVertexList, Color.White, flameDepth, jetFlameTriangleList, Ability.texCoordList[32], true);
                        playerEffect.Texture = Ability.ability_textures;
                        playerEffect.CurrentTechnique.Passes[0].Apply();
                        Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                            jetFlameTriangleList.ToArray(), 0, jetFlameTriangleList.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                    }
                }
            }

        }        
        

    }
}
