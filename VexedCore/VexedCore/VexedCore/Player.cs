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
        HookSpin,
        Upgrade,
        Save,
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
        public static Texture2D player_doublejump;
        public static Texture2D player_jetpackthrust;
        public static Texture2D player_boosterthrust;
        public static Texture2D player_spinhook;
        public static Texture2D player_spinlink;
        public static List<List<Vector2>> texCoordList;

        public int totalGameTime = 0;

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
        public Doodad hookShotTarget;
        public Vector3 hookLand;
        [XmlIgnore]public Room currentRoom;
        public string currentRoomId;
        public int jumpRecovery = 0;
        public bool saveComplete = false;
        public Vector3 jumpDestination;
        public Vector3 jumpSource;
        public Vector3 jumpCameraDestination;
        public Vector3 jumpCameraSource;
        public Vector3 jumpPosition;
        public Vector3 jumpNormal;
        public Vector3 platformVelocity;
        public Vector3 lastKnownPlatformVelocity;
        public float referenceFrameSpeed;
        public int currentObjective = 0;
        public Vector3 spinUp;
        public Vertex lastLivingPosition;
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
        public int upgradeTime = 0;
        public int spinRecovery = 0;
        public bool superJump = false;
        public bool jetPacking = false;
        public bool jetPackThrust = false;
        public bool sliding = false;
        public int walkTime = 0;
        public int crushCount = 0;
        public static int maxCrushCount = 3;
        public int idleTime = 0;
        public int boostTime = 0;
        public int flashTime = 0;
        public int fireCooldown = 100;
        public bool leftWall = false;
        public bool rightWall = false;
        public int weaponSwitchCooldown = 0;
        public Ability primaryAbility;
        public Ability secondaryAbility;
        public Ability naturalShield;
        public bool safeLanding = true;
        public bool boosting = false;
        public bool[] upgrades;
        public Vector3 oldNormal = Vector3.Zero;
        public Vector3 oldUp = Vector3.Zero;
        public Doodad upgradeStationDoodad = null;
        public String upgradeStationDoodadId = "";
        public int upgradeStage = 0;
        public int doubleJumpTime = 0;
        public static int doubleJumpFadeTime = 300;

        public HookState hookState = HookState.Waiting;

        public float playerHalfWidth = .35f;
        public float playerHalfHeight = .5f;
        public bool expertLevel = false;

        // Boss victory flags
        public bool chaseBoss1 = false;
        public bool chaseBoss2 = false;
        public bool armorBoss1 = false;
        public bool armorBoss2 = false;
        public bool armorBoss3 = false;
        public bool rockBoss1 = false;
        public bool rockBoss2 = false;
        public bool jetBoss = false;
        public bool snakeBoss = false;
        public bool faceBoss = false; 

        public static int flashMaxTime = 400;
        public static int jumpRecoveryMax = 300;
        public static int spinMaxTime = 270;
        public static int walkMaxTime = 800;
        public static int launchMaxTime = 1000;
        public static int maxBoostTime = 350;
        public static int maxDeathTime = 1000;
        public static int maxHookTime = 150;
        public static int maxHookHangTime = 100;
        public static int weaponSwitchCooldownMax = 200;
        public static int upgradeWalkTime = 500;
        public static int upgradeInTime = upgradeWalkTime + 300;
        public static int upgradeWaitTime = upgradeInTime + 1000;
        public static int upgradeOutTime = upgradeWaitTime + 500;

        public VL.GunType gunType = VL.GunType.Blaster;
        
        
        public float walkSpeed = .001f;
        public float airSpeed = .0005f;
        public float jumpSpeed = .020f;
        
        public float wallJumpSpeed = .01f;
        public float maxHorizSpeed = .01f;
        public float maxBoostHorizSpeed = .03f;
        
        public float gravityAcceleration = .0009f;
        public float boostAcceleration = .002f;
        private bool _grounded = false;
        public int groundTolerance = 100;
        public int groundCounter = 0;
        public int faceDirection = -1;
        public float _baseCameraDistance = 12;
        public int orbsCollected = 0;
        public int redOrbsCollected = 0;
        public int redOrbsGoal = 2;

        public bool objectiveFilter = false;
        public bool stationFilter = false;
        public bool itemFilter = true;
        public bool saveFilter = false;
        public bool healthFilter = true;
        public bool warpFilter = true;

        public Vector2 cameraAngle;
        public Vector2 targetCameraAngle;

        [XmlIgnore]public Doodad respawnPoint;
        [XmlIgnore]public Player respawnPlayer;

        public bool dead = false;

        public float boundingBoxTop;
        public float boundingBoxBottom;
        public float boundingBoxLeft;
        public float boundingBoxRight;

        public static float cameraUpTilt = .1f;
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

            /*upgrades[(int)AbilityType.Laser] = true;
            upgrades[(int)AbilityType.Boots] = true;
            upgrades[(int)AbilityType.Empty] = true;
            upgrades[(int)AbilityType.WallJump] = true;
            upgrades[(int)AbilityType.DoubleJump] = true;
            upgrades[(int)AbilityType.Blaster] = true;
            upgrades[(int)AbilityType.PermanentBoots] = true;
            upgrades[(int)AbilityType.PermanentWallJump] = true;
            upgrades[(int)AbilityType.ImprovedJump] = true;
            upgrades[(int)AbilityType.SpinHook] = true;
            
            upgrades[(int)AbilityType.Missile] = true;
            upgrades[(int)AbilityType.Booster] = true;
            upgrades[(int)AbilityType.JetPack] = true;
            upgrades[(int)AbilityType.Phase] = true;
            upgrades[(int)AbilityType.PermanentBlueKey] = true;
            upgrades[(int)AbilityType.PermanentRedKey] = true;
            upgrades[(int)AbilityType.PermanentYellowKey] = true;*/
            //for (int i = 8; i < 19; i++)
                //upgrades[i] = true;            
        }

        public Player(Player p)
        {
            currentObjective = p.currentObjective;
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
            totalGameTime = p.totalGameTime;
            boostTime = p.boostTime;
            fireCooldown = p.fireCooldown;
            expertLevel = p.expertLevel;
            leftWall = p.leftWall;
            rightWall = p.rightWall;
            jumpTime = p.jumpTime;
            redOrbsCollected = p.redOrbsCollected;
            redOrbsGoal = p.redOrbsGoal;
            orbsCollected = p.orbsCollected;
            weaponSwitchCooldown = p.weaponSwitchCooldown;
            currentObjective = p.currentObjective;

            healthFilter = p.healthFilter;
            itemFilter = p.itemFilter;
            saveFilter = p.saveFilter;
            warpFilter = p.warpFilter;
            objectiveFilter = p.objectiveFilter;

            gunType = p.gunType;
            currentRoomId = p.currentRoomId;
            upgradeStationDoodadId = p.upgradeStationDoodadId;
            upgradeTime = p.upgradeTime;
            saveComplete = p.saveComplete;
            primaryAbility = new Ability(p.primaryAbility);
            secondaryAbility = new Ability(p.secondaryAbility);
            naturalShield = new Ability(p.naturalShield);
            boosting = p.boosting;

            upgrades = new bool[64];
            for (int i = 0; i < 64; i++)
                upgrades[i] = p.upgrades[i];

            

            if (p.currentRoom != null)
                currentRoomId = p.currentRoom.id;
            if (p.upgradeStationDoodad != null)
                upgradeStationDoodadId = p.upgradeStationDoodad.id;

        }

        public int fireTime
        {
            get
            {
                if (gunType == VL.GunType.Blaster || gunType == VL.GunType.Spread)
                    return 1000;
                if (gunType == VL.GunType.Missile)
                    return 4000;
                if (gunType == VL.GunType.Beam)
                    return 2000;
                if (gunType == VL.GunType.Repeater)
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
                if ((state == State.Upgrade || state == State.Save) && upgradeTime < upgradeWalkTime)
                    return true;
                Vector2 stick = Controls.LeftStick();
                return (Controls.IsLeftKeyDown() ||
                    Controls.IsRightKeyDown() || stick.X != 0) && grounded == true;
            }
        }

        public float depth
        {
            get
            {
                if ((state == State.Upgrade || state == State.Save) && upgradeTime > upgradeWalkTime && upgradeTime < upgradeInTime)
                {
                    return (.3f * (upgradeInTime - upgradeTime) + .13f * (upgradeTime - upgradeWalkTime)) / (upgradeInTime - upgradeWalkTime);
                }
                if ((state == State.Upgrade || state == State.Save) && upgradeTime >= upgradeInTime && upgradeTime < upgradeWaitTime)
                {
                    return .14f;
                }
                if ((state == State.Upgrade || state == State.Save) && upgradeTime >= upgradeWaitTime && upgradeTime < upgradeOutTime)
                {
                    return (.3f * (upgradeTime - upgradeWaitTime) + .14f * (upgradeOutTime - upgradeTime)) / (upgradeOutTime - upgradeWaitTime);
                }
                
                return .3f;
            }
        }

        public float adjustedCameraDistance
        {
            get
            {
                if (cameraAngle.Length() > 1f)
                    cameraAngle.Normalize();

                float result = (1f - cameraAngle.Length()) * _baseCameraDistance;
                if (result < 2f) 
                    result = 2f;
                return result;
            }
        }

        public float baseCameraDistance
        {
            get
            {
                return _baseCameraDistance;
            }
            set
            {
                _baseCameraDistance = value;
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
                if (state == State.Normal || state == State.Spin || state == State.Dialog || state == State.Upgrade || state == State.Save)
                {
                    return currentRoom.RaisedPosition(center.position + cameraOffset, adjustedCameraDistance, cameraRoundingThreshold);
                }
                if (state == State.Death)
                {
                    return currentRoom.RaisedPosition(lastLivingPosition.position + cameraOffset, baseCameraDistance, cameraRoundingThreshold);
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
                if (state == State.Normal || state == State.Spin || state == State.Dialog || state == State.Upgrade || state == State.Save)
                {
                    return currentRoom.RaisedPosition(center.position, baseCameraDistance, cameraRoundingThreshold);
                }
                else if (state == State.Death)
                {
                    return currentRoom.RaisedPosition(lastLivingPosition.position, baseCameraDistance, cameraRoundingThreshold);
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
                return baseCameraDistance * (cameraAngle.Y * cameraUp + cameraAngle.X * cameraRight);
            }
        }

        public Vector3 simpleCameraOffset
        {
            get
            {
                return baseCameraDistance * (cameraUpTilt * cameraUp);
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
                    return currentRoom.AdjustedUp(lastLivingPosition.position, lastLivingPosition.direction, lastLivingPosition.normal, 1f);
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
                if (state == State.Normal || state == State.Spin || state == State.Dialog || state == State.Upgrade || state == State.Save)
                    return center.position;
                else if (state == State.Tunnel)
                    return tunnelDummy.position;
                else if (state == State.Death)
                    return lastLivingPosition.position;
                else
                    return jumpPosition;
            }
        }

        public float maxVertSpeed
        {
            get
            {
                if ((leftWall && faceDirection == -1 && (Controls.LeftStick().X < 0 || wallTime < 500)) || (rightWall && faceDirection == 1 && (Controls.LeftStick().X > 0 || wallTime < 500)))
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
            bool warpSuccess = false;
            foreach (Doodad d in targetRoom.doodads)
            {
                if (d.type == VL.DoodadType.WarpStation)
                {
                    currentRoom = targetRoom;
                    Physics.refresh = true;
                    Engine.reDraw = true;
                    center = new Vertex(d.position.position, d.position.normal, Vector3.Zero, d.position.direction);
                    state = State.Normal;
                    warpSuccess = true;
                }
            }
            if (warpSuccess == false)
            {
                currentRoom = targetRoom;
                Physics.refresh = true;
                Engine.reDraw = true;
                center = new Vertex(new Vector3(-5.5f, 10f, 1.3f), Vector3.UnitY, Vector3.Zero, Vector3.UnitZ);
                Engine.playerCameraPos = cameraPos;
                Engine.playerCameraUp = cameraUp;
                Engine.playerCameraTarget = cameraTarget;
                state = State.Normal;
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
            LevelLoader.QuickLoad();
            //Engine.reDraw = true;
            Physics.refresh = true;
            
            Engine.player.currentRoom.Reset();
        }

        public bool HasTraction()
        {
            return primaryAbility.type == AbilityType.Boots || secondaryAbility.type == AbilityType.Boots || upgrades[(int)AbilityType.PermanentBoots];
        }

        public void SpinHookUpdate(int gameTime)
        {
            if (hookState != HookState.Waiting)
            {
                if (hookState == HookState.Hook && state != State.Spin)
                    hookHangTime += gameTime;
                else if (hookState == HookState.Out)
                    hookTime += gameTime;
                else if (hookState == HookState.In)
                    hookTime -= gameTime;
                if (hookState == HookState.Out)
                {
                    if (hookShotTarget != null)
                    {
                        Vector3 dir = hookShotTarget.position.position - (center.position + .5f * right * faceDirection);
                        dir.Normalize();
                        dir *= .014f;

                        hookShot = new Vertex(center.position + .5f * right * faceDirection, center.normal, dir, center.direction);
                    }
                    else
                    {
                        hookShot = new Vertex(center.position + .5f * right * faceDirection, center.normal, .01f * up + .01f * right * faceDirection, center.direction);
                    }
                    hookShot.Update(currentRoom, hookTime);
                    foreach (Doodad d in currentRoom.doodads)
                    {
                        if (d.type == VL.DoodadType.HookTarget && (d.position.position - hookShot.position).Length() < .8f)
                        {
                            center.velocity = Vector3.Zero;
                            jumpSource = center.position;
                            jumpDestination = d.position.position + right * faceDirection - up;
                            state = State.Spin;
                            spinUp = -faceDirection * right;
                            hookHangTime = 0;
                            hookState = HookState.Hook;
                            SoundFX.HookLock();
                        }
                    }
                }
                if (hookState == HookState.In)
                {
                    if (hookShotTarget != null)
                    {
                        Vector3 dir = hookShotTarget.position.position - (center.position + .5f * right * faceDirection);
                        dir.Normalize();
                        dir *= .014f;

                        hookShot = new Vertex(center.position + .5f * right * faceDirection, center.normal, dir, center.direction);
                    }
                    else
                    {
                        hookShot = new Vertex(center.position + .5f * right * faceDirection, center.normal, .01f * up + .01f * right * faceDirection, center.direction);
                    } 
                    hookShot.Update(currentRoom, hookTime);
                }

                if (hookState == HookState.Hook)
                {
                    foreach (Doodad d in currentRoom.doodads)
                    {
                        if (d.type == VL.DoodadType.HookTarget && (d.position.position - hookShot.position).Length() < .8f)
                        {
                            Vector3 vel = .01f * up + .01f * right * faceDirection;

                            float distance = (d.position.position - (center.position + .5f * right * faceDirection)).Length();
                            hookTime = (int)(distance / vel.Length());                            

                            hookShot = new Vertex(d.position.position, center.normal, Vector3.Zero, center.direction);
                        }
                    }
                    if (state != State.Spin)
                    {
                        if (hookShotTarget != null)
                        {
                            Vector3 dir = hookShotTarget.position.position - (center.position + .5f * right * faceDirection);
                            dir.Normalize();
                            dir *= .014f;

                            
                            if ((hookShotTarget.position.position - hookShot.position).Length() < .8f)
                                hookShot = new Vertex(hookShotTarget.position.position, center.normal, Vector3.Zero, center.direction);
                            else
                                hookShot = new Vertex(center.position + .5f * right * faceDirection, center.normal, dir, center.direction);
                        }
                        else
                        {
                            hookShot = new Vertex(center.position + .5f * right * faceDirection, center.normal, .01f * up + .01f * right * faceDirection, center.direction);
                        }
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
        }

        public void SpinHook()
        {
            if (hookState == HookState.Waiting)
            {
                SoundFX.LaunchHook();
                hookState = HookState.Out;
                if (hookShotTarget != null)
                {
                    Vector3 dir = hookShotTarget.position.position - (center.position + .5f * right * faceDirection);
                    dir.Normalize();
                    dir *= .014f;

                    hookShot = new Vertex(center.position + .5f * right * faceDirection, center.normal, dir, center.direction);
                }
                else
                {
                    hookShot = new Vertex(center.position + .5f * right * faceDirection, center.normal, .01f * up + .01f * right * faceDirection, center.direction);
                }
                hookShot.Update(currentRoom, maxHookTime/2);
                hookTime = 0;
                hookShotTarget = null;
                foreach (Doodad d in currentRoom.doodads)
                {
                    if (d.type == VL.DoodadType.HookTarget && (d.position.position - hookShot.position).Length() < 2f)
                    {
                        hookShotTarget = d;                        
                    }
                }
                if (hookShotTarget != null)
                {
                    Vector3 dir = hookShotTarget.position.position - (center.position + .5f * right * faceDirection);
                    dir.Normalize();
                    dir *= .014f;

                    hookShot = new Vertex(center.position + .5f * right * faceDirection, center.normal, dir, center.direction);
                }
                else
                {
                    hookShot = new Vertex(center.position + .5f * right * faceDirection, center.normal, .01f * up + .01f * right * faceDirection, center.direction);
                }
            }
            else
            {
                foreach (Doodad d in currentRoom.doodads)
                {
                    if (d.type == VL.DoodadType.HookTarget && (d.position.position - hookShot.position).Length() < .8f)
                    {
                        center.velocity = Vector3.Zero;
                        jumpSource = center.position;
                        jumpDestination = d.position.position + right * faceDirection - up;
                        if (state != State.Spin)
                        {
                            SoundFX.PlayMove();
                        }
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

        public void Damage(Vector3 projection, bool crushing)
        {
            if (crushing == true)
            {
                crushCount += 2;
                if (crushCount < maxCrushCount)
                    return;
            }
            
            idleTime = 0;
            if (state == State.Death)
                return;
            center.velocity += 1.5f*maxHorizSpeed * Vector3.Normalize(projection);
            safeLanding = false;

            if (crushing == true)
            {
                deathTime = 0;
                flashTime = flashMaxTime;
                state = State.Death;
                SoundFX.PlayerHit();
                lastLivingPosition = new Vertex(center);
            }

            if (primaryAbility.type == AbilityType.DoubleJump)
            {
                primaryAbility.ammo = 0;
            }
            else if (secondaryAbility.type == AbilityType.DoubleJump)
            {
                secondaryAbility.ammo = 0;
            }
            if (flashTime == 0)
            {
                if (primaryAbility.type == AbilityType.Shield && primaryAbility.ammo != 0)
                {
                    primaryAbility.DepleteAmmo(800);
                    flashTime = flashMaxTime;
                }
                else if (secondaryAbility.type == AbilityType.Shield && secondaryAbility.ammo != 0)
                {
                    secondaryAbility.DepleteAmmo(800);
                    flashTime = flashMaxTime;
                }
                else if (naturalShield.ammo != 0)
                {
                    SoundFX.PlayerHit();
                    naturalShield.DepleteAmmo(1);
                    flashTime = flashMaxTime;
                }
                else
                {
                    deathTime = 0;
                    flashTime = flashMaxTime;
                    state = State.Death;
                    lastLivingPosition = new Vertex(center);
                }
            }
        }

        public void Spin(Vector3 newUp)
        {
            if (upgrades[(int)AbilityType.PermanentBoots]==true || primaryAbility.type == AbilityType.Boots || secondaryAbility.type == AbilityType.Boots)
            {
                jumpSource = center.position;
                jumpDestination = center.position;
                if (state != State.Spin)
                    SoundFX.BootSpin();
                state = State.Spin;
                spinUp = newUp;
                boosting = false;
            }
        }

        public void AttemptPhase()
        {
            if (state == State.Spin || state == State.Upgrade || state == State.Save)
                return;
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
                if (m.moveType == VL.MovementType.FaceBoss)
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
            SoundFX.Phase();
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
            if (rightMagnitude > maxBoostHorizSpeed && boosting == true)
            {
                center.velocity -= (rightMagnitude - maxBoostHorizSpeed) * right;
            }
            if (rightMagnitude < -maxBoostHorizSpeed && boosting == true)
            {
                center.velocity -= (maxBoostHorizSpeed + rightMagnitude) * right;
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
            else if (state == State.Jump || state == State.Tunnel)
            {
                AnimationControl.SetState(AnimationState.JumpPad);
            }
            else if (state == State.Phase || state == State.PhaseFail)
            {
                AnimationControl.SetState(AnimationState.Phase);
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
                    else if (faceDirection > 0)
                        AnimationControl.SetState(AnimationState.JumpRight);
                    else
                        AnimationControl.SetState(AnimationState.JumpPad);
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
                else if (grounded == false && rightWall == true && faceDirection > 0)
                {
                    AnimationControl.SetState(AnimationState.WallLeftFiring);
                }
                else if (grounded == false && leftWall == true && faceDirection < 0)
                {
                    AnimationControl.SetState(AnimationState.WallRightFiring);
                }
                else if (grounded == false)
                {
                    if (faceDirection < 0)
                        AnimationControl.SetState(AnimationState.JumpLeftFiring);
                    else if (faceDirection > 0)
                        AnimationControl.SetState(AnimationState.JumpRightFiring);
                    else
                        AnimationControl.SetState(AnimationState.JumpPad);
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

        /*public void ResetAdjacentRooms()
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
                if (d.type == VL.DoodadType.JumpPad || d.type == VL.DoodadType.JumpStation)
                {
                    if(d.targetRoom != null)
                        d.targetRoom.adjacent = true;
                }
            }
            if (jumpRoom != null)
            {
                foreach (Doodad d in jumpRoom.doodads)
                {
                    if (d.type == VL.DoodadType.JumpPad || d.type == VL.DoodadType.JumpStation)
                    {
                        if (d.targetRoom != null)
                            d.targetRoom.adjacent = true;
                    }
                }
            }
        }*/

        public void Update(int gameTime)
        {
            totalGameTime += gameTime;
            if (state == State.Spin)
                spinRecovery = 50;
            else
            {
                spinRecovery -= gameTime;
                if (spinRecovery < 0)
                    spinRecovery = 0;
            }
            currentRoom.explored = true;
            crushCount--;
            if (crushCount < 0)
                crushCount = 0;
            flashTime -= gameTime;
            if (flashTime < 0) flashTime = 0;
            wallJumpCooldown -= gameTime;
            if (wallJumpCooldown < 0) wallJumpCooldown = 0;
            if (leftWall == true || rightWall == true)
            {
                wallTime += gameTime;
            }
            else
                wallTime = 0;

            if (doubleJumpTime > 0)
            {
                doubleJumpTime -= gameTime;
                if (doubleJumpTime < 0)
                    doubleJumpTime = 0;
            }



            if (jetPackThrust == false)
            {
                SoundFX.EndJetPack();
            }
            if (boosting == false)
            {
                SoundFX.EndBooster();
            }

            if (jetPacking == true)
            {
                if (platformVelocity.Length() > 0)
                {
                    platformVelocity -= .005f * platformVelocity;
                    if (platformVelocity.Length() < .01f)
                        platformVelocity = Vector3.Zero;
                }
                if(jetPackThrust == false && (leftWall == true || rightWall == true))
                    jetPacking = false;
                
            }
            if (grounded == true)
            {
                wallTime = 0;
                referenceFrameSpeed = Vector3.Dot(platformVelocity, right);
            }
            else
            {
                if(referenceFrameSpeed > 0)
                {
                    referenceFrameSpeed -= .0001f;
                    if (referenceFrameSpeed < 0) referenceFrameSpeed = 0;
                }
                if (referenceFrameSpeed < 0)
                {
                    referenceFrameSpeed += .0001f;
                    if (referenceFrameSpeed > 0) referenceFrameSpeed = 0;
                }
                float currentSpeed = Vector3.Dot(center.velocity, right);
                Vector2 controlStick = Controls.LeftStick();
                if (!(Game1.controller.JumpButtonPressed() == true ||  Controls.IsRightKeyDown() ||
                    Controls.IsLeftKeyDown() || Math.Abs(controlStick.X) > .1f || boosting == true))
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
                    tunnelDummy.Update(currentRoom, gameTime);
                if (launchTime > 3 * launchMaxTime / 4 && tunnelExit != null)
                    tunnelExit.ActivateDoodad(currentRoom, true);
                oldUp = tunnelDummy.direction;
                oldNormal = tunnelDummy.normal;
            }
            else
            {
                //oldUp = center.direction;
                //oldNormal = center.normal;
            }


            Vector2 stick = Controls.LeftStick();
            Vector2 rightStick = Controls.GetStaticCameraHelper();
            if (state != State.Jump && state != State.PhaseFail && state != State.PhaseFail && state != State.Tunnel)
            {
                targetCameraAngle = rightStick + new Vector2(0, cameraUpTilt);
                cameraAngle = (cameraAngle + targetCameraAngle) / 2;
            }

            /// Upgrade / Save Logic
            if (state == State.Upgrade)
            {
                upgradeTime += gameTime;
                if (upgradeTime < upgradeWalkTime)
                {
                    upgradeStage = 0;
                    center.position = ((upgradeWalkTime - upgradeTime) * jumpSource + upgradeTime * upgradeStationDoodad.position.position) / upgradeWalkTime;
                }
                if (Vector3.Dot(right, upgradeStationDoodad.position.position - center.position) > 0)
                    faceDirection = 1;
                else
                    faceDirection = -1;
                if (upgradeTime > upgradeWalkTime)
                {                    
                    faceDirection = 0;                    
                }
                if (upgradeTime > upgradeInTime)
                {                    
                    upgradeStationDoodad.active = false;
                }
                if (upgradeTime > upgradeWaitTime-100)
                {
                    if (upgradeStationDoodad.type == VL.DoodadType.UpgradeStation)
                    {
                        if (upgradeStationDoodad.abilityType == AbilityType.Ultima)
                        {
                            for (int i = 0; i < 32; i++)
                                upgrades[i] = true;
                        }
                        upgrades[(int)upgradeStationDoodad.abilityType] = true;
                        upgradeStationDoodad.abilityType = AbilityType.Empty;                        
                    }
                    if (upgradeStationDoodad.type == VL.DoodadType.RedPowerStation)
                    {
                        if (upgradeStationDoodad.orbsRemaining > 0)
                        {
                            redOrbsCollected += 1;
                            if (redOrbsCollected % redOrbsGoal == 0)
                            {
                                redOrbsCollected = 0;
                                redOrbsGoal = redOrbsGoal + 1;
                                naturalShield.maxAmmo = naturalShield.maxAmmo + 1;
                                naturalShield.ammo = naturalShield.maxAmmo;
                            }
                            upgradeStationDoodad.orbsRemaining--;
                            currentRoom.currentRedOrbs++;
                            currentRoom.parentSector.currentRedOrbs++;
                            //Engine.reDraw = true;
                            currentRoom.refreshVertices = true;
                        }
                    }
                    if (upgradeStationDoodad.type == VL.DoodadType.BluePowerStation)
                    {
                        if (upgradeStationDoodad.orbsRemaining > 0)
                        {
                            upgradeStationDoodad.orbsRemaining--;
                            currentRoom.currentBlueOrbs++;
                            currentRoom.parentSector.currentBlueOrbs++;
                            //Engine.reDraw = true;
                            currentRoom.refreshVertices = true;
                        }
                    }

                    //upgradeTime = 0;
                }
                if (upgradeTime > upgradeOutTime)
                {
                    state = State.Normal;
                    upgradeTime = 0;
                    if (upgradeStationDoodad.type == VL.DoodadType.UpgradeStation)
                    {
                        DialogBox.SetDialog((new Ability(upgradeStationDoodad.originalAbilityType)).GetDialogId());
                        upgradeStationDoodad.originalAbilityType = AbilityType.Empty;
                        upgradeStationDoodad.alreadyUsed = true;
                    }
                    if (upgradeStationDoodad.type == VL.DoodadType.RedPowerStation)
                    {
                        if(redOrbsCollected!=0)
                            DialogBox.SetDialog("RedPowerStation");
                        else
                            DialogBox.SetDialog("RedPowerStationFull");
                    }
                    if (upgradeStationDoodad.type == VL.DoodadType.BluePowerStation)
                        DialogBox.SetDialog("BluePowerStation");
                    
                }
            }
            if (state == State.Save)
            {
                upgradeTime += gameTime;
                if (upgradeTime < upgradeWalkTime)
                {
                    center.position = ((upgradeWalkTime - upgradeTime) * jumpSource + upgradeTime * upgradeStationDoodad.position.position) / upgradeWalkTime;
                    upgradeStage = 0;
                }
                if (Vector3.Dot(right, upgradeStationDoodad.position.position - center.position) > 0)
                    faceDirection = 1;
                else
                    faceDirection = -1;
                if (upgradeTime > upgradeWalkTime)
                {
                    faceDirection = 0;
                }
                if (upgradeTime > upgradeInTime)
                {                    
                    //upgradeStationDoodad.active = false;
                    if(saveComplete == false)
                        upgradeStationDoodad.ActivateDoodad(currentRoom, false);
                    //upgradeStationDoodad.doorState = false;
                }
                //if (upgradeTime > upgradeWaitTime - 100)
                if (upgradeTime < upgradeOutTime)
                {
                    if (Engine.justLoaded == false && DialogBox.state == BoxState.None)
                        DialogBox.SetDialog("Saving");
                }
                if (upgradeTime > upgradeWaitTime - 300)
                {
                    if(saveComplete==false)
                    {
                        SaveGame();
                        Game1.resetTimer = true;

                        saveComplete = true;
                        upgradeTime = upgradeWaitTime - 300;
                    }
                    
                    
                }
                if (upgradeTime > upgradeWaitTime-200)
                {
                    upgradeStationDoodad.ActivateDoodad(currentRoom, true);
                }
                    
                if (upgradeTime > upgradeOutTime)
                {                    
                    state = State.Normal;
                    upgradeTime = 0;
                    if(Engine.justLoaded == false)
                        DialogBox.SetDialog("SaveComplete");

                }
            }

            SpinHookUpdate(gameTime);
            

            SetAnimationState();
            primaryAbility.Update(gameTime);
            secondaryAbility.Update(gameTime);

            if (grounded && !walking)
                idleTime += gameTime;
            else
                idleTime = 0;

            lastFireTime += gameTime;

            if (idleTime > 2000)
            {
                if (primaryAbility.type == AbilityType.Shield)
                {
                    primaryAbility.AddAmmo(gameTime);
                }
                if (secondaryAbility.type == AbilityType.Shield)
                {
                    secondaryAbility.AddAmmo(gameTime);
                }
            }
            if (grounded)
                jetPacking = false;

            if (dead == true)
                Respawn();
            
            
                 

            groundCounter += gameTime;
            if (groundCounter > groundTolerance)
            {
                groundCounter = groundTolerance;
                grounded = false;
            }

            
            GamePadState gamePadState = GamePad.GetState(Game1.activePlayer);

            

            if (state == State.Death)
            {
                center.position += center.velocity * gameTime;
                center.velocity -= gravityAcceleration * up;
                EnforceVelocityLimits();
                deathTime += gameTime;
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

                walkTime += gameTime;
                if (walkTime > walkMaxTime) walkTime -= walkMaxTime;
                center.Update(currentRoom, gameTime);
                jumpRecovery -= gameTime;
                if (jumpRecovery < 0) jumpRecovery = 0;
                boostTime -= gameTime;
                if (boostTime < 0) boostTime = 0;
                if (boostTime == 0)
                    boosting = false;

                Vector3 up = center.direction;
                Vector3 right = Vector3.Cross(up, center.normal);

                //if (grounded == true)
                //faceDirection = 0;

                if (wallJumpCooldown == 0)
                {
                    if (Controls.IsLeftKeyDown())
                    {
                        if(hookState == HookState.Waiting)
                            faceDirection = -1;
                        if (grounded == true)
                            center.velocity -= walkSpeed * right;
                        else
                            center.velocity -= airSpeed * right;
                    }
                    if (Controls.IsRightKeyDown())
                    {
                        if (hookState == HookState.Waiting)
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
                        if (hookState == HookState.Waiting)
                        {
                            if (stick.X > .2) faceDirection = 1;
                            if (stick.X < -.2) faceDirection = -1;
                        }
                    }
                }

                if (superJump == true)
                {
                    jumpTime += gameTime;

                    if (jumpTime > 45 && grounded == true)
                        center.velocity += (jumpSpeed - upMagnitude) * up;
                    int jumpLimit = 60;
                    if (upgrades[(int)AbilityType.ImprovedJump])
                        jumpLimit = 150;
                    if (jumpTime > jumpLimit)
                        superJump = false;
                    if (jumpTime < 45 && grounded == false)
                        superJump = false;
                    
                    if (Game1.controller.JumpButtonPressed() == false && jumpTime > 45)
                        superJump = false;
                }

                if (Game1.controller.BackButton.NewPressed)
                {
                    dead = true;
                }
                

                if ((gamePadState.IsButtonDown(Buttons.Back)))
                {
                    Respawn();
                }


                if (boosting == false)
                    center.velocity -= gravityAcceleration * up;
                else
                    center.velocity += boostAcceleration *gameTime
                         / 16f * right * faceDirection;

                
                EnforceVelocityLimits();
                jetPackThrust = false;
                #region XButton
                if (Game1.controller.XButton.NewPressed)
                {
                    foreach (Doodad d in currentRoom.doodads)
                    {
                        if (d.active && (d.type == VL.DoodadType.LoadStation))
                        {
                            SaveGameText.Confirm();
                            Game1.controller.XButton.Invalidate();
                        }
                    }
                }
                if (Game1.controller.XButton.Pressed)
                {
                    Doodad itemStation = null;
                    bool stationPresent = false;
                    foreach (Doodad d in currentRoom.doodads)
                    {
                        if (d.active && (d.type == VL.DoodadType.LoadStation))
                        {
                            stationPresent = true;
                        }
                        else if (d.active && (d.type == VL.DoodadType.ItemStation || d.type == VL.DoodadType.ItemBlock))
                        {
                            if (Game1.controller.XButton.NewPressed && d.cooldown == 0 && upgrades[(int)d.abilityType] == true)
                            {
                                itemStation = d;
                            }
                            else if (Game1.controller.XButton.NewPressed && d.cooldown == 0 && upgrades[(int)d.abilityType] == false)
                            {
                                SoundFX.EquipError();
                                d.cooldown = d.maxCooldown;
                            }
                            stationPresent = true;
                        }
                    }

                    if (stationPresent == false)
                    {
                        primaryAbility.Do(gameTime);
                        if (primaryAbility.isJump == false)
                            Game1.controller.XButton.Invalidate();
                    }
                    else if (itemStation != null)
                    {
                        if (itemStation.abilityType != primaryAbility.type && itemStation.abilityType != secondaryAbility.type)
                        {
                            AbilityType swapAbilityType = primaryAbility.type;
                            primaryAbility = new Ability(itemStation.abilityType);
                            itemStation.abilityType = swapAbilityType;                            
                            SoundFX.EquipItem();
                        }
                        else
                            SoundFX.EquipError();
                        itemStation.cooldown = itemStation.maxCooldown;
                        Game1.controller.JumpInvalidate();
                    }
                }
                #endregion
                #region YButton
                if (Game1.controller.YButton.NewPressed)
                {
                    foreach (Doodad d in currentRoom.doodads)
                    {
                        if (d.active && (d.type == VL.DoodadType.LoadStation))
                        {
                            // Cancel Load
                            SaveGameText.Cancel();
                            Game1.controller.YButton.Invalidate();
                        }
                    }
                }
                if (Game1.controller.YButton.Pressed)
                {
                    Doodad itemStation = null;
                    bool stationPresent = false;

                    foreach (Doodad d in currentRoom.doodads)
                    {
                        if (d.active && (d.type == VL.DoodadType.LoadStation))
                        {
                            stationPresent = true;
                        }
                        else if (d.active && (d.type == VL.DoodadType.ItemStation || d.type == VL.DoodadType.ItemBlock))
                        {
                            if (Game1.controller.YButton.NewPressed && d.cooldown == 0 && upgrades[(int)d.abilityType] == true)
                            {
                                itemStation = d;
                            }
                            else if (Game1.controller.YButton.NewPressed && d.cooldown == 0 && upgrades[(int)d.abilityType] == false)
                            {
                                SoundFX.EquipError();
                                d.cooldown = d.maxCooldown;
                            }
                            stationPresent = true;
                        }
                    }
                    if (stationPresent == false)
                    {
                        secondaryAbility.Do(gameTime);
                        if(secondaryAbility.isJump == false)
                            Game1.controller.YButton.Invalidate();
                    }
                    else if (itemStation != null)
                    {
                        if (itemStation.abilityType != primaryAbility.type && itemStation.abilityType != secondaryAbility.type)
                        {
                            AbilityType swapAbilityType = secondaryAbility.type;
                            secondaryAbility = new Ability(itemStation.abilityType);
                            itemStation.abilityType = swapAbilityType;                           
                            SoundFX.EquipItem();
                        }
                        else
                            SoundFX.EquipError();
                        itemStation.cooldown = itemStation.maxCooldown;
                        Game1.controller.JumpInvalidate();
                    }
                }
                #endregion
                if (Game1.controller.JumpButtonNewPressed() && jumpRecovery == 0)
                {

                    Game1.controller.JumpInvalidate();
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
                            SoundFX.JumpSound();
                        }
                    }
                    else if (hookState == HookState.Waiting && leftWall && faceDirection < 0 && (upgrades[(int)AbilityType.PermanentWallJump] == true || primaryAbility.type == AbilityType.WallJump || secondaryAbility.type == AbilityType.WallJump))
                    {
                        faceDirection = 1;
                        if (upMagnitude < .8f * jumpSpeed)
                            center.velocity += (.8f * jumpSpeed - upMagnitude) * up;
                        if (rightMagnitude < wallJumpSpeed)
                            center.velocity += (wallJumpSpeed - rightMagnitude) * right;
                        jumpRecovery = jumpRecoveryMax;
                        wallJumpCooldown = wallJumpCooldownMax;
                        SoundFX.JumpSound();

                    }
                    else if (hookState == HookState.Waiting && rightWall && faceDirection > 0 && (upgrades[(int)AbilityType.PermanentWallJump] == true || primaryAbility.type == AbilityType.WallJump || secondaryAbility.type == AbilityType.WallJump))
                    {
                        faceDirection = -1;
                        if (upMagnitude < .8f * jumpSpeed)
                            center.velocity += (.8f * jumpSpeed - upMagnitude) * up;
                        if (rightMagnitude > -wallJumpSpeed)
                            center.velocity -= (wallJumpSpeed + rightMagnitude) * right;
                        jumpRecovery = jumpRecoveryMax;
                        wallJumpCooldown = wallJumpCooldownMax;
                        SoundFX.JumpSound();
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
                                SoundFX.RocketJump();
                                doubleJumpTime = doubleJumpFadeTime;
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
                                SoundFX.RocketJump();
                                doubleJumpTime = doubleJumpFadeTime;
                            }
                        }
                    }
                }
                // CHEAT CODES
                if (Keyboard.GetState().IsKeyDown(Keys.K))
                {
                    currentRoom.currentOrbs++;
                    currentRoom.refreshVertices = true;
                }

                #region BButton
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
                            if (d.type == VL.DoodadType.Vortex && (d.position.position - center.position).Length() < .3f)
                            {
                                SoundFX.TunnelWoosh();
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
                            if (d.type == VL.DoodadType.BridgeGate)
                            {
                                if (Vector3.Dot(center.velocity, d.position.direction) > 0)
                                {
                                    SoundFX.TunnelWoosh();
                                    jumpRoom = d.targetRoom;
                                    jumpSource = center.position;
                                    jumpDestination = d.targetDoodad.position.position - 2f * d.targetDoodad.upUnit;
                                    jumpCameraSource = currentRoom.RaisedPosition(jumpSource + cameraOffset, adjustedCameraDistance, cameraRoundingThreshold);
                                    jumpCameraDestination = jumpRoom.RaisedPosition(jumpDestination + simpleCameraOffset, adjustedCameraDistance, cameraRoundingThreshold);
                                    
                                    jumpNormal = center.normal;
                                    launchTime = 0;
                                    state = State.BridgeJump;
                                    faceDirection = 0;
                                    d.active = false;
                                    d.targetDoodad.active = false;
                                }
                            }
                            if (d.type == VL.DoodadType.LoadStation)
                            {
                                // Extra Load
                                SaveGameText.Extra();                         
                            }
                            if (d.type == VL.DoodadType.PowerStation && d.cooldown == 0)
                            {
                                if (d.orbsRemaining > 0)
                                {
                                    d.cooldown = d.maxCooldown;
                                    SoundFX.PlayScore();
                                    orbsCollected += 1;
                                    d.orbsRemaining--;
                                    currentRoom.currentOrbs++;
                                    currentRoom.parentSector.currentOrbs++;
                                    //Engine.reDraw = true;
                                    currentRoom.refreshVertices = true;
                                    foreach (Doodad powerup in d.currentRoom.doodads)
                                        powerup.RefreshPowerLevels();
                                }
                            }
                            if (d.type == VL.DoodadType.RedPowerStation)
                            {
                                
                                state = State.Upgrade;
                                upgradeStationDoodad = d;
                                upgradeTime = 0;
                                jumpSource = center.position;
                            }
                            if (d.type == VL.DoodadType.BluePowerStation)
                            {

                                state = State.Upgrade;
                                upgradeStationDoodad = d;
                                upgradeTime = 0;
                                jumpSource = center.position;
                            }
                            if (d.type == VL.DoodadType.SaveStation && d.cooldown == 0)
                            {
                                bool orbsTracking = false;
                                foreach (Doodad orb in Engine.player.currentRoom.doodads)
                                {
                                    if (orb.active == true && orb.tracking)
                                        orbsTracking = true;

                                }
                                if (orbsTracking == false)
                                {
                                    d.cooldown = d.maxCooldown;
                                    state = State.Save;
                                    saveComplete = false;
                                    upgradeStationDoodad = d;
                                    jumpSource = center.position;
                                    //SaveGame();
                                }
                            }
                            if (d.type == VL.DoodadType.UpgradeStation)
                            {
                                state = State.Upgrade;
                                upgradeStationDoodad = d;
                                jumpSource = center.position;
                            }
                            if (d.type == VL.DoodadType.HealthStation)
                            {
                                if (d.cooldown == 0)
                                {
                                    
                                    d.cooldown = d.maxCooldown;
                                    if (naturalShield.ammo < naturalShield.maxAmmo)
                                    {
                                        SoundFX.Heal();
                                        naturalShield.ammo++;
                                    }
                                }
                            }
                            if (d.type == VL.DoodadType.JumpPad || d.type == VL.DoodadType.JumpStation)
                            {
                                //if (d.type == VL.DoodadType.JumpPad)
                                d.Activate();
                                d.alreadyUsed = true;
                                Engine.reDraw = true;
                                SoundFX.RoomJump();
                                jumpRoom = d.targetRoom;
                                jumpRoom.Reset();
                                //ResetAdjacentRooms();
                                
                                float roomSize = Math.Abs(Vector3.Dot(jumpRoom.size / 2, center.normal));
                                jumpSource = center.position;
                                jumpDestination = center.position + Vector3.Dot(jumpRoom.center - center.position - roomSize * center.normal, center.normal) * center.normal;
                                
                                // Set up Jump Camera
                                jumpCameraSource = currentRoom.RaisedPosition(jumpSource + cameraOffset, adjustedCameraDistance, cameraRoundingThreshold);
                                Mouse.SetPosition(Game1.titleSafeRect.Center.X, Game1.titleSafeRect.Center.Y);                                
                                targetCameraAngle = Controls.GetStaticCameraHelper() + new Vector2(0, cameraUpTilt);
                                cameraAngle = targetCameraAngle;
                                jumpCameraDestination = jumpRoom.RaisedPosition(jumpDestination + cameraOffset, adjustedCameraDistance, cameraRoundingThreshold);
                                    
                                state = State.Jump;
                                center.velocity = Vector3.Zero;
                                launchTime = 0;
                                d.active = false;
                                jumpNormal = -center.normal;
                            }
                            if (d.type == VL.DoodadType.Holoprojector)
                            {
                                SoundFX.HologramUse();
                                DialogBox.SetDialog(d.id);
                                state = State.Dialog;
                                
                            }
                            if (d.type == VL.DoodadType.SwitchStation && d.cooldown == 0)
                            {
                                bool locked = false;
                                if (d.abilityType == AbilityType.RedKey && !(primaryAbility.type == AbilityType.RedKey || secondaryAbility.type == AbilityType.RedKey || upgrades[(int)AbilityType.PermanentRedKey] == true))
                                    locked = true;
                                if (d.abilityType == AbilityType.BlueKey && !(primaryAbility.type == AbilityType.BlueKey || secondaryAbility.type == AbilityType.BlueKey || upgrades[(int)AbilityType.PermanentBlueKey] == true))
                                    locked = true;
                                if (d.abilityType == AbilityType.YellowKey && !(primaryAbility.type == AbilityType.YellowKey || secondaryAbility.type == AbilityType.YellowKey || upgrades[(int)AbilityType.PermanentYellowKey] == true))
                                    locked = true;

                                if (d.targetDoodad != null)
                                {
                                    if (d.targetDoodad.currentBehavior.id == d.expectedBehavior)
                                    {
                                        foreach (Behavior b in d.targetDoodad.behaviors)
                                        {
                                            if (b.id == d.targetBehavior)
                                            {
                                                if (locked == false)
                                                {
                                                    SoundFX.ActivateSwitchStation();
                                                    d.targetDoodad.SetBehavior(b);
                                                }
                                                else
                                                {
                                                    SoundFX.LockedSwitch();
                                                    d.cooldown = d.maxCooldown;
                                                }
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
                                                if (locked == false)
                                                {
                                                    SoundFX.ActivateSwitchStation();
                                                    d.targetBlock.SetBehavior(b);
                                                }
                                                else
                                                {
                                                    SoundFX.ActivateSwitchStation();
                                                    d.cooldown = d.maxCooldown;
                                                }
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
                                                if (locked == false)
                                                {
                                                    SoundFX.ActivateSwitchStation();
                                                    d.targetEdge.SetBehavior(b);
                                                }
                                                else
                                                {
                                                    SoundFX.ActivateSwitchStation();
                                                    d.cooldown = d.maxCooldown;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                                
                            if (d.type == VL.DoodadType.WarpStation && d.cooldown == 0)
                            {
                                if (currentRoom.parentSector.currentBlueOrbs > currentRoom.warpCost)
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
                                else
                                {
                                    SoundFX.EquipError();
                                    d.cooldown = d.maxCooldown;
                                }
                            }
                        }
                    }
                }
                #endregion
                foreach (Doodad d in currentRoom.doodads)
                {
                    if (d.active)
                    {
                        if (d.type == VL.DoodadType.BridgeGate)
                        {
                            SoundFX.BridgeWarp();
                            jumpRoom = d.targetRoom;
                            jumpRoom.Reset();
                            jumpSource = center.position;
                            jumpDestination = d.targetDoodad.position.position - 1f * d.targetDoodad.upUnit;

                            // Base Camera
                            jumpCameraSource = currentRoom.RaisedPosition(jumpSource + cameraOffset, adjustedCameraDistance, cameraRoundingThreshold);
                            jumpCameraDestination = jumpRoom.RaisedPosition(jumpDestination + simpleCameraOffset, adjustedCameraDistance, cameraRoundingThreshold);
                                    
                            
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
                launchTime += gameTime;
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
                    Room oldRoom = currentRoom;
                    currentRoom = jumpRoom;
                    if (oldRoom != jumpRoom)
                    {
                        oldRoom.Reset();
                        currentRoom.Reset();
                    }


                    if ((oldState == State.Jump || oldState == State.BridgeJump) && expertLevel == false)
                    {
                        naturalShield.ammo = naturalShield.maxAmmo;
                        QuickSave();
                    }
                    
                    Physics.refresh = true;
                    Engine.reDraw = true;
                    foreach (Doodad d in jumpRoom.doodads)
                    {
                        if(d.type == VL.DoodadType.BridgeGate)
                            d.active = false;
                        if (d.type == VL.DoodadType.JumpStation || d.type == VL.DoodadType.JumpPad)
                        {
                            if((d.position.position - center.position).Length() < 1.5f && d.powered == true)
                            {
                                d.alreadyUsed = true;
                            }
                        }
                    }
                    Mouse.SetPosition(Game1.titleSafeRect.Center.X, Game1.titleSafeRect.Center.Y);
                    jumpRoom = null;                    

                }

            }
            if (state == State.Spin)
            {
                spinTime += gameTime;
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
            if(safeLanding == false)
                safeLanding = true;
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


            Color playerColor = Color.White;
            playerColor.G = (Byte)(255 - (255 * flashTime *4/ flashMaxTime));
            playerColor.B = (Byte)(255 - (255 * flashTime *4/ flashMaxTime));
            currentRoom.AddTextureToTriangleList(rectVertexList, playerColor, depth, textureTriangleList, Player.texCoordList[currentTextureIndex], true);


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
                VertexPositionColorNormalTexture[] hookArray = null;

                for (int i = 1; i < 4; i++)
                {

                    hookVertexList.Add(new Vertex((i * hookShot.position + (5 - i) * (center.position + .5f * right * faceDirection + .2f * up)) / 5, hookShot.normal, +playerHalfHeight * hookUp + .5f * hookRight, hookShot.direction));
                    hookVertexList.Add(new Vertex((i * hookShot.position + (5 - i) * (center.position + .5f * right * faceDirection + .2f * up)) / 5, hookShot.normal, +playerHalfHeight * hookUp - .5f * hookRight, hookShot.direction));
                    hookVertexList.Add(new Vertex((i * hookShot.position + (5 - i) * (center.position + .5f * right * faceDirection + .2f * up)) / 5, hookShot.normal, -playerHalfHeight * hookUp - .5f * hookRight, hookShot.direction));
                    hookVertexList.Add(new Vertex((i * hookShot.position + (5 - i) * (center.position + .5f * right * faceDirection + .2f * up)) / 5, hookShot.normal, -playerHalfHeight * hookUp + .5f * hookRight, hookShot.direction));
                    foreach (Vertex v in hookVertexList)
                        v.Update(currentRoom, 1);


                    currentRoom.AddTextureToTriangleList(hookVertexList, Color.White, depth + .075f, hookTriangleList, Room.plateTexCoords, true);

                    hookArray = hookTriangleList.ToArray();

                    playerEffect.Texture = Player.player_spinlink;
                    playerEffect.CurrentTechnique.Passes[0].Apply();

                    Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                        hookArray, 0, hookArray.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);

                    hookVertexList = new List<Vertex>();
                    hookTriangleList = new List<VertexPositionColorNormalTexture>();
                }
                
                hookVertexList.Add(new Vertex(hookShot.position, hookShot.normal, +playerHalfHeight * hookUp + .5f * hookRight, hookShot.direction));
                hookVertexList.Add(new Vertex(hookShot.position, hookShot.normal, +playerHalfHeight * hookUp - .5f * hookRight, hookShot.direction));
                hookVertexList.Add(new Vertex(hookShot.position, hookShot.normal, -playerHalfHeight * hookUp - .5f * hookRight, hookShot.direction));
                hookVertexList.Add(new Vertex(hookShot.position, hookShot.normal, -playerHalfHeight * hookUp + .5f * hookRight, hookShot.direction));
                foreach (Vertex v in hookVertexList)
                    v.Update(currentRoom, 1);


                currentRoom.AddTextureToTriangleList(hookVertexList, Color.White, depth + .075f, hookTriangleList, Room.plateTexCoords, true);

                hookArray = hookTriangleList.ToArray();

                playerEffect.Texture = Player.player_spinhook;
                playerEffect.CurrentTechnique.Passes[0].Apply();
                if (state == State.Jump || state == State.BridgeJump || state == State.Tunnel || state == State.Phase || state == State.PhaseFail)
                {
                    for (int i = 0; i < textureTriangleList.Count(); i++)
                    {
                        hookArray[i].Position += jumpPosition - center.position;
                    }
                }
                Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                    hookArray, 0, hookArray.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
            }

            if (primaryAbility.isBoots || secondaryAbility.isBoots || upgrades[(int)AbilityType.PermanentBoots] == true || upgrades[(int)AbilityType.PermanentWallJump] == true)
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
                    float flameDepth = depth + .05f;
                    if (faceDirection < 0)
                    {
                        List<VertexPositionColorNormalTexture> jetFlameTriangleList = new List<VertexPositionColorNormalTexture>();
                        List<Vertex> flameVertexList = new List<Vertex>();
                        VertexPositionColorNormalTexture[] flameArray = null;
                        flameVertexList.Add(new Vertex(center.position, center.normal, +.8f*playerHalfHeight * up + 1.3f * right, center.direction));
                        flameVertexList.Add(new Vertex(center.position, center.normal, +.8f* playerHalfHeight * up - .1f * right, center.direction));
                        flameVertexList.Add(new Vertex(center.position, center.normal, -.6f * playerHalfHeight * up - .1f * right, center.direction));
                        flameVertexList.Add(new Vertex(center.position, center.normal, -.6f * playerHalfHeight * up + 1.3f * right, center.direction));
                        foreach (Vertex v in flameVertexList)
                        {
                            v.Update(currentRoom, 1);
                        }

                        currentRoom.AddTextureToTriangleList(flameVertexList, Color.White, flameDepth, jetFlameTriangleList, Room.plateTexCoords, true);
                        flameArray = jetFlameTriangleList.ToArray();
                        if (state == State.Jump || state == State.BridgeJump || state == State.Tunnel || state == State.Phase || state == State.PhaseFail)
                        {
                            for (int i = 0; i < textureTriangleList.Count(); i++)
                            {
                                flameArray[i].Position += jumpPosition - center.position;
                            }
                        }

                        playerEffect.Texture = Player.player_boosterthrust;
                        playerEffect.CurrentTechnique.Passes[0].Apply();
                        Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                            flameArray, 0, jetFlameTriangleList.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                    }
                    if (faceDirection > 0)
                    {
                        List<VertexPositionColorNormalTexture> jetFlameTriangleList = new List<VertexPositionColorNormalTexture>();
                        List<Vertex> flameVertexList = new List<Vertex>();
                        VertexPositionColorNormalTexture[] flameArray = null;
                        flameVertexList.Add(new Vertex(center.position, center.normal, +.8f * playerHalfHeight * up + .1f * right, center.direction));
                        flameVertexList.Add(new Vertex(center.position, center.normal, +.8f * playerHalfHeight * up - 1.3f * right, center.direction));
                        flameVertexList.Add(new Vertex(center.position, center.normal, -.6f * playerHalfHeight * up - 1.3f * right, center.direction));
                        flameVertexList.Add(new Vertex(center.position, center.normal, -.6f * playerHalfHeight * up + .1f * right, center.direction));
                        foreach (Vertex v in flameVertexList)
                        {
                            v.Update(currentRoom, 1);
                        }
                        currentRoom.AddTextureToTriangleList(flameVertexList, Color.White, flameDepth, jetFlameTriangleList, Room.plateTexCoords, false);

                        flameArray = jetFlameTriangleList.ToArray();
                        if (state == State.Jump || state == State.BridgeJump || state == State.Tunnel || state == State.Phase || state == State.PhaseFail)
                        {
                            for (int i = 0; i < textureTriangleList.Count(); i++)
                            {
                                flameArray[i].Position += jumpPosition - center.position;
                            }
                        }

                        playerEffect.Texture = Player.player_boosterthrust;
                        playerEffect.CurrentTechnique.Passes[0].Apply();
                        Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                            flameArray, 0, jetFlameTriangleList.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                    }
                }
            }
            if (doubleJumpTime > 0)
            {
                playerEffect.Texture = Player.player_doublejump;
                playerEffect.CurrentTechnique.Passes[0].Apply();
                List<VertexPositionColorNormalTexture> jetFlameTriangleList = new List<VertexPositionColorNormalTexture>();
                List<Vertex> flameVertexList = new List<Vertex>();
                float fade = 1f*doubleJumpTime / doubleJumpFadeTime;
                flameVertexList.Add(new Vertex(center.position, center.normal, (fade-1.9f) * playerHalfHeight * up + .5f * right, center.direction));
                flameVertexList.Add(new Vertex(center.position, center.normal, (fade-1.9f) * playerHalfHeight * up - .5f * right, center.direction));
                flameVertexList.Add(new Vertex(center.position, center.normal, (fade-2.9f) * playerHalfHeight * up - .5f * right, center.direction));
                flameVertexList.Add(new Vertex(center.position, center.normal, (fade-2.9f) * playerHalfHeight * up + .5f * right, center.direction));
                foreach (Vertex v in flameVertexList)
                {
                    v.Update(currentRoom, 1);
                }
                List<VertexPositionColorNormalTexture> doubleJumpTriangleList = new List<VertexPositionColorNormalTexture>();
                Color boostColor = Color.Transparent;
                boostColor.R = (Byte)(fade * 255);
                boostColor.G = (Byte)(fade * 255);
                boostColor.B = (Byte)(fade * 255);
                boostColor.A = (Byte)(fade * 255);
                currentRoom.AddTextureToTriangleList(flameVertexList, boostColor, depth + .05f, doubleJumpTriangleList, Room.plateTexCoords, true);
                Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                    doubleJumpTriangleList.ToArray(), 0, doubleJumpTriangleList.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
            }
            if (primaryAbility.type == AbilityType.JetPack || secondaryAbility.type == AbilityType.JetPack)
            {
                playerEffect.Texture = Player.player_jetpack_textures;
                playerEffect.CurrentTechnique.Passes[0].Apply();
                Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                    triangleArray, 0, triangleArray.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                if (jetPackThrust == true)
                {
                    float flameDepth = depth - .05f;
                    if (faceDirection != 0)
                        flameDepth = depth + .05f;
                    if (faceDirection <= 0)
                    {
                        List<VertexPositionColorNormalTexture> jetFlameTriangleList = new List<VertexPositionColorNormalTexture>();
                        List<Vertex> flameVertexList = new List<Vertex>();
                        VertexPositionColorNormalTexture[] flameArray = null;
                        flameVertexList.Add(new Vertex(center.position, center.normal, -.3f * playerHalfHeight * up + .43f * right, center.direction));
                        flameVertexList.Add(new Vertex(center.position, center.normal, -.3f * playerHalfHeight * up - .17f * right, center.direction));
                        flameVertexList.Add(new Vertex(center.position, center.normal, -1.3f * playerHalfHeight * up - .17f * right, center.direction));
                        flameVertexList.Add(new Vertex(center.position, center.normal, -1.3f*playerHalfHeight * up + .43f * right, center.direction));
                        foreach (Vertex v in flameVertexList)
                        {
                            v.Update(currentRoom, 1);
                        }
                        currentRoom.AddTextureToTriangleList(flameVertexList, Color.White, flameDepth, jetFlameTriangleList, Room.plateTexCoords, true);

                        flameArray = jetFlameTriangleList.ToArray();
                        if (state == State.Jump || state == State.BridgeJump || state == State.Tunnel || state == State.Phase || state == State.PhaseFail)
                        {
                            for (int i = 0; i < textureTriangleList.Count(); i++)
                            {
                                flameArray[i].Position += jumpPosition - center.position;
                            }
                        }
                        playerEffect.Texture = Player.player_jetpackthrust;
                        playerEffect.CurrentTechnique.Passes[0].Apply();
                        Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                            flameArray, 0, jetFlameTriangleList.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                    }
                    if (faceDirection >= 0)
                    {
                        List<VertexPositionColorNormalTexture> jetFlameTriangleList = new List<VertexPositionColorNormalTexture>();
                        List<Vertex> flameVertexList = new List<Vertex>();
                        VertexPositionColorNormalTexture[] flameArray = null;
                        flameVertexList.Add(new Vertex(center.position, center.normal,-.3f * playerHalfHeight * up + .17f * right, center.direction));
                        flameVertexList.Add(new Vertex(center.position, center.normal, -.3f * playerHalfHeight * up - .43f * right, center.direction));
                        flameVertexList.Add(new Vertex(center.position, center.normal, -1.3f*playerHalfHeight * up -.43f * right, center.direction));
                        flameVertexList.Add(new Vertex(center.position, center.normal, -1.3f*playerHalfHeight * up + .17f * right, center.direction));
                        foreach (Vertex v in flameVertexList)
                        {
                            v.Update(currentRoom, 1);
                        }
                        currentRoom.AddTextureToTriangleList(flameVertexList, Color.White, flameDepth, jetFlameTriangleList, Room.plateTexCoords, true);

                        flameArray = jetFlameTriangleList.ToArray();
                        if (state == State.Jump || state == State.BridgeJump || state == State.Tunnel || state == State.Phase || state == State.PhaseFail)
                        {
                            for (int i = 0; i < textureTriangleList.Count(); i++)
                            {
                                flameArray[i].Position += jumpPosition - center.position;
                            }
                        }
                        playerEffect.Texture = Player.player_jetpackthrust;
                        playerEffect.CurrentTechnique.Passes[0].Apply();
                        Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                            flameArray, 0, jetFlameTriangleList.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                    }
                }
            }

        }        
        

    }
}
