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
    public class Monster
    {
        public static Texture2D monsterTexture;
        public static Texture2D monsterTextureDetail;

        public static List<Vector2> bodyTexCoords;
        public static List<Vector2> eyesTexCoords;
        public static List<Vector2> fullArmorTexCoords;
        public static List<Vector2> topArmorTexCoords;
        public static List<Vector2> frontArmorTexCoords;
        public static List<Vector2> fullSuperArmorTexCoords;
        public static List<Vector2> topSuperArmorTexCoords;
        public static List<Vector2> frontSuperArmorTexCoords;
        public static List<Vector2> treadsTexCoords;
        public static List<Vector2> spiderTexCoords;
        public static List<Vector2> legsTexCoords;
        public static List<Vector2> gunTexCoords;
        public static List<Vector2> stoneSolidTexCoords;
        public static List<Vector2> stoneCrackTexCoords;
        public static List<Vector2> stoneSolidBreakTexCoords;
        public static List<Vector2> stoneCrackBreakTexCoords;
        public static List<Vector2> snowSolidTexCoords;
        public static List<Vector2> snowCrackTexCoords;
        public static List<Vector2> snowSolidBreakTexCoords;
        public static List<Vector2> snowCrackBreakTexCoords;
        public static List<Vector2> spikeShieldTexCoords;
        public static List<Vector2> iceShieldFullTexCoords;
        public static List<Vector2> iceShieldMedTexCoords;
        public static List<Vector2> iceShieldLowTexCoords;
        public static List<Vector2> iceTurretTexCoords;
        public static List<Vector2> bossShieldTexCoords;
        public static List<Vector2> flashTexCoords;
        public static List<Vector2> jetArmorTexCoords;
        public static List<Vector2> bossArmorTexCoords;
        public static List<Vector2> facePlateTexCoords;
        public static List<Vector2> faceBlankTexCoords;
        public static List<Vector2> faceAngryEyesTexCoords;
        public static List<Vector2> faceNormalEyesTexCoords;


        public static int texGridCount = 8;

        public bool hasOrbs = true;
        public Vertex unfoldedPosition;
        public Vertex spawnPosition;
        public Vertex position;
        public string firstWaypoint;
        public List<Vector3> waypoints;
        public int currentWaypointIndex = 0;
        public int wayPointDirection = 1;
        public bool waypointLoop = false;
        public VexedLib.AIType aiType;
        public VexedLib.MovementType moveType;
        public VexedLib.MonsterHealth healthType;
        public VexedLib.MonsterSpeed speedType;
        public VexedLib.MonsterSize sizeType;
        public VexedLib.TrackType trackType;
        public VexedLib.ArmorType startingArmorType;
        public VexedLib.ArmorType armorType;
        public VexedLib.GunType gunType;
        public Vector3 groundProjection;
        public Vector3 forwardGroundProjection;
        public Vector3 forwardProjection;
        public bool jumping = false;
        public int jumpCooldown = 0;
        public int jumpTime = 0;
        public Vector3 spinUp;

        public int flashCooldown;
        public int maxFlashCooldown = 200;
        public int spinTime = 0;
        public bool rightFacing = true;
        public bool rightMoving = false;
        public int currentDirection = 1;
        public float huntMinDistance = 3.5f;
            
        [XmlIgnore]public Monster srcMonster;
        
        public int directionChangeCooldown = 0;        
        public string id;
        public bool dead;
        public Vector3 impactVector = Vector3.Zero;

        public int startingArmorHP = 2;
        public int armorHP = 2;
        public int baseHP = 5;
        public ProjectileType lastHitType;

        public Vector3 prevUp = Vector3.Zero;
        public int spinRecovery = 0;
        public List<GunEmplacement> guns;

        public RockBoss rockBoss;
        public ChaseBoss chaseBoss;
        public SnakeBoss snakeBoss;
        public ArmorBoss armorBoss;
        public BattleBoss battleBoss;
        public FaceBoss faceBoss;
        public JetBoss jetBoss;

        public Monster(Monster m)
        {
            //unfoldedPosition = new Vertex(m.unfoldedPosition);
            rockBoss = new RockBoss();
            chaseBoss = new ChaseBoss();
            snakeBoss = new SnakeBoss();
            faceBoss = new FaceBoss();
            battleBoss = new BattleBoss();
            armorBoss = new ArmorBoss();
            jetBoss = new JetBoss();
            hasOrbs = m.hasOrbs;
            spawnPosition = new Vertex(m.spawnPosition);
            startingArmorHP = m.startingArmorHP;
            startingArmorType = m.startingArmorType;
            position = new Vertex(m.position);
            firstWaypoint = m.firstWaypoint;
            waypoints = new List<Vector3>();
            foreach (Vector3 v in m.waypoints)
                waypoints.Add(v);
            guns = new List<GunEmplacement>();
            foreach (GunEmplacement g in m.guns)
                guns.Add(new GunEmplacement(g));

            currentWaypointIndex = m.currentWaypointIndex;
            wayPointDirection = m.wayPointDirection;
            waypointLoop = m.waypointLoop;
            aiType = m.aiType;
            currentDirection = m.currentDirection;
            moveType = m.moveType;
            armorType = m.armorType;
            trackType = m.trackType;
            speedType = m.speedType;
            sizeType = m.sizeType;
            healthType = m.healthType;
            gunType = m.gunType;
            groundProjection = m.groundProjection;
            forwardGroundProjection = m.forwardGroundProjection;
            forwardProjection = m.forwardProjection;
            jumping = m.jumping;
            jumpCooldown = m.jumpCooldown;
            spinUp = m.spinUp;
            
            spinTime = m.spinTime;
            rightFacing = m.rightFacing;
            rightMoving = m.rightMoving;
            directionChangeCooldown = m.directionChangeCooldown;

            id = m.id;
            dead = m.dead;
            impactVector = m.impactVector;
            armorHP = m.armorHP;
            baseHP = m.baseHP;
            huntMinDistance = m.huntMinDistance;
        }
        
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
            bodyTexCoords = LoadTexCoords(0, 0);
            fullArmorTexCoords = LoadTexCoords(1, 0);
            topArmorTexCoords = LoadTexCoords(2, 0);
            frontArmorTexCoords = LoadTexCoords(3, 0);
            fullSuperArmorTexCoords = LoadTexCoords(1, 4);
            topSuperArmorTexCoords = LoadTexCoords(2, 4);
            frontSuperArmorTexCoords = LoadTexCoords(3, 4);
            eyesTexCoords = LoadTexCoords(4, 0);
            gunTexCoords = LoadTexCoords(5, 0);
            treadsTexCoords = LoadTexCoords(0, 2);
            spiderTexCoords = LoadTexCoords(0, 1);
            legsTexCoords = LoadTexCoords(0, 3);
            stoneCrackBreakTexCoords = LoadTexCoords(3, 5);
            stoneCrackTexCoords = LoadTexCoords(1, 5);
            stoneSolidBreakTexCoords = LoadTexCoords(2, 5);
            stoneSolidTexCoords = LoadTexCoords(0, 5);
            snowCrackBreakTexCoords = LoadTexCoords(7, 5);
            snowCrackTexCoords = LoadTexCoords(5, 5);
            snowSolidBreakTexCoords = LoadTexCoords(6, 5);
            snowSolidTexCoords = LoadTexCoords(4, 5);
            
            spikeShieldTexCoords = LoadTexCoords(0, 6);
            iceShieldFullTexCoords = LoadTexCoords(0, 7);
            iceShieldMedTexCoords = LoadTexCoords(1, 7);
            iceShieldLowTexCoords = LoadTexCoords(2, 7);
            iceTurretTexCoords = LoadTexCoords(3, 7);
            bossShieldTexCoords = LoadTexCoords(2, 6);
            flashTexCoords = LoadTexCoords(7, 0);
            jetArmorTexCoords = LoadTexCoords(4, 6);
            bossArmorTexCoords = LoadTexCoords(6, 6);
            facePlateTexCoords = LoadTexCoords(7, 1);
            faceBlankTexCoords = LoadTexCoords(7, 2);
            faceAngryEyesTexCoords = LoadTexCoords(7, 3);
            faceNormalEyesTexCoords = LoadTexCoords(7, 4);
        }

        public Monster()
        {
        }

        public Monster(VexedLib.Monster xmlMonster, Vector3 normal)
        {
            snakeBoss = new SnakeBoss();
            rockBoss = new RockBoss();
            chaseBoss = new ChaseBoss();
            faceBoss = new FaceBoss();
            battleBoss = new BattleBoss();
            armorBoss = new ArmorBoss();
            jetBoss = new JetBoss();
            this.spawnPosition = new Vertex(xmlMonster.position, normal, Vector3.Zero, xmlMonster.up);
            this.position = new Vertex(xmlMonster.position, normal, Vector3.Zero, xmlMonster.up);
            firstWaypoint = xmlMonster.waypointId;
            waypoints = new List<Vector3>();
            aiType = xmlMonster.behavior;
            moveType = xmlMonster.movement;
            speedType = xmlMonster.speed;
            healthType = xmlMonster.health;
            trackType = xmlMonster.trackType;
            sizeType = xmlMonster.size;
            startingArmorType = xmlMonster.armor;
            armorType = xmlMonster.armor;
            gunType = xmlMonster.weapon;
            id = xmlMonster.IDString;
            guns = new List<GunEmplacement>();
            if (moveType == VexedLib.MovementType.RockBoss)
            {
                if (id.Contains("Snow"))
                {
                    guns.Add(new GunEmplacement(VexedLib.TrackType.Normal, VexedLib.GunType.Blaster, new Vector2(1.3f, .3f), .7f * halfWidth, .05f, BaseType.Snow));
                    guns.Add(new GunEmplacement(VexedLib.TrackType.Normal, VexedLib.GunType.Blaster, new Vector2(-1.3f, .3f), .7f * halfWidth, .05f, BaseType.Snow));
                }
                else
                {
                    guns.Add(new GunEmplacement(VexedLib.TrackType.Fast, VexedLib.GunType.Blaster, new Vector2(1.3f, .3f), .7f * halfWidth, .05f, BaseType.Rock));
                    guns.Add(new GunEmplacement(VexedLib.TrackType.Fast, VexedLib.GunType.Blaster, new Vector2(-1.3f, .3f), .7f * halfWidth, .05f, BaseType.Rock));
                }
            }
            else if (moveType == VexedLib.MovementType.ChaseBoss)
            {
                rightFacing = true;
                armorHP = 1;
                startingArmorType = VexedLib.ArmorType.ShieldSuper;
            }
            else if (moveType == VexedLib.MovementType.SnakeBoss)
            {
                
            }
            else if (moveType == VexedLib.MovementType.BattleBoss)
            {
                guns.Add(new GunEmplacement(VexedLib.TrackType.Normal, VexedLib.GunType.Blaster, new Vector2(.3f, 1.3f), .7f * halfWidth, .05f, BaseType.Standard));
                guns.Add(new GunEmplacement(VexedLib.TrackType.Normal, VexedLib.GunType.Blaster, new Vector2(.3f, -1.3f), .7f * halfWidth, .05f, BaseType.Standard));
            }
            else if (moveType == VexedLib.MovementType.JetBoss)
            {
                guns.Add(new GunEmplacement(VexedLib.TrackType.Normal, VexedLib.GunType.Blaster, new Vector2(.3f, -1.3f), .7f * halfWidth, .05f, BaseType.Standard));
            }
            else if (moveType == VexedLib.MovementType.ArmorBoss)
            {
                if (id.Contains("Basic"))
                {

                }
                rightFacing = true;
                startingArmorType = VexedLib.ArmorType.ShieldSuper;
                guns.Add(new GunEmplacement(VexedLib.TrackType.Normal, VexedLib.GunType.Blaster, new Vector2(.3f, -1.3f), .7f * halfWidth, .05f, BaseType.Standard));
                guns.Add(new GunEmplacement(VexedLib.TrackType.Normal, VexedLib.GunType.Blaster, new Vector2(.3f, 1.3f), .7f * halfWidth, .05f, BaseType.Standard));
            }
            else if(gunType != VexedLib.GunType.None)
                guns.Add(new GunEmplacement(trackType, gunType, Vector2.Zero, halfWidth, -.05f, BaseType.None));            
        }


        public void ResetBossState()
        {
            chaseBoss = new ChaseBoss();
            armorBoss = new ArmorBoss();
            faceBoss = new FaceBoss();
            rockBoss = new RockBoss();
            snakeBoss = new SnakeBoss();
            jetBoss = new JetBoss();
            battleBoss = new BattleBoss();            
        }

        public Monster(Monster m, Room r, Vector3 n, Vector3 u)
        {
            snakeBoss = new SnakeBoss();
            rockBoss = new RockBoss();
            chaseBoss = new ChaseBoss();
            faceBoss = new FaceBoss();
            battleBoss = new BattleBoss();
            armorBoss = new ArmorBoss();
            position = m.position.Unfold(r,n,u);
            srcMonster = m;
            rightFacing = m.rightFacing;
            rightMoving = m.rightMoving;
        }

        public void UpdateUnfoldedDoodad(Room r, Vector3 n, Vector3 u)
        {
            unfoldedPosition = position.Unfold(r, n, u);
        }


        public int startingBaseHP
        {
            get
            {
                if (moveType == VexedLib.MovementType.ArmorBoss)
                {
                    if (id.Contains("Basic"))
                        return 16;
                    return 6;
                }
                if (moveType == VexedLib.MovementType.JetBoss)
                    return 48;
                if (moveType == VexedLib.MovementType.SnakeBoss)
                    return 4;
                if (healthType == VexedLib.MonsterHealth.Weak)
                    return 1;
                if (healthType == VexedLib.MonsterHealth.Normal)
                    return 4;
                if (healthType == VexedLib.MonsterHealth.Tough)
                    return 8;
                
                return 4;
            }
        }


        public int spinMaxTime
        {
            get
            {
                return 400;
            }
        }

        public Color monsterColor
        {
            get
            {
                if (moveType == VexedLib.MovementType.SnakeBoss)
                {
                    if (baseHP > 0)
                        return new Color(60, 60, 100);
                    else
                        return new Color(60, 60, 60);
                }
                if (moveType == VexedLib.MovementType.RockBoss)
                {
                    if (id.Contains("Snow"))
                        return new Color(60, 60, 150);
                    return Color.OrangeRed;
                }
                if (gunType == VexedLib.GunType.None)
                {
                    return new Color(60, 60, 60);
                }
                if (gunType == VexedLib.GunType.Blaster)
                {
                    return Color.Red;
                }
                if (gunType == VexedLib.GunType.Beam)
                {
                    return Color.Blue;
                }
                if (gunType == VexedLib.GunType.Missile)
                {
                    return Color.Gray;
                }
                if (gunType == VexedLib.GunType.Spread)
                {
                    return Color.Green;
                }
                if (gunType == VexedLib.GunType.Repeater)
                {
                    return Color.Orange;
                }
                return Color.Yellow;
            }
        }

        public float acceleration
        {
            get
            {
                return .0005f;
            }
        }

        public float maxFallSpeed
        {
            get
            {                
                return .009f;
            }
        }

        public float maxSpeed
        {
            get
            {                
                if(speedType == VexedLib.MonsterSpeed.Medium)
                    return .009f;
                if (speedType == VexedLib.MonsterSpeed.Slow)
                    return .002f;
                if (speedType == VexedLib.MonsterSpeed.Fast)
                    return .015f;
                return 0f;
            }
        }

        public Vector3 upUnit
        {
            get
            {
                if (spinUp == Vector3.Zero)
                    return position.direction;
                else
                {
                    Vector3 tempUp = (spinUp * spinTime + position.direction * (spinMaxTime - spinTime)) / (1f * spinMaxTime);
                    tempUp.Normalize();
                    return tempUp;
                }
            }
        }
        public Vector3 rightUnit
        {
            get
            {
                if (spinUp == Vector3.Zero)
                    return Vector3.Cross(position.direction, position.normal);
                else
                {
                    Vector3 tempUp = Vector3.Cross((spinUp * spinTime + position.direction * (spinMaxTime - spinTime)) / (1f * spinMaxTime), position.normal);
                    tempUp.Normalize();
                    return tempUp;
                }
            }
        }

        public Vector3 unfoldedUpUnit
        {
            get
            {
                if (spinUp == Vector3.Zero)
                    return unfoldedPosition.direction;
                else
                {
                    Vector3 tempUp = (spinUp * spinTime + unfoldedPosition.direction * (spinMaxTime - spinTime)) / (1f * spinMaxTime);
                    tempUp.Normalize();
                    return tempUp;
                }
            }
        }
        public Vector3 unfoldedRightUnit
        {
            get
            {
                if (spinUp == Vector3.Zero)
                    return Vector3.Cross(unfoldedPosition.direction, unfoldedPosition.normal);
                else
                {
                    Vector3 tempUp = Vector3.Cross((spinUp * spinTime + unfoldedPosition.direction * (spinMaxTime - spinTime)) / (1f * spinMaxTime), unfoldedPosition.normal);
                    tempUp.Normalize();
                    return tempUp;
                }
            }
        }

        public float right_mag
        {
            get
            {
                return halfWidth;
            }
        }
        public float left_mag
        {
            get
            {
                return -halfWidth;
            }
        }
        public float up_mag
        {
            get
            {
                return halfHeight;
            }
        }
        public float down_mag
        {
            get
            {
                return -halfHeight;
            }
        }

        public Vector3 right
        {
            get
            {
                return right_mag * rightUnit;
            }
        }
        public Vector3 left
        {
            get
            {
                return left_mag * rightUnit;
            }
        }
        public Vector3 up
        {
            get
            {
                return up_mag * upUnit;
            }
        }
        public Vector3 down
        {
            get
            {
                return down_mag * upUnit;
            }
        }

        public Vector3 unfolded_right
        {
            get
            {
                return right_mag * unfoldedRightUnit;
            }
        }
        public Vector3 unfolded_left
        {
            get
            {
                return left_mag * unfoldedRightUnit;
            }
        }
        public Vector3 unfolded_up
        {
            get
            {
                return up_mag * unfoldedUpUnit;
            }
        }
        public Vector3 unfolded_down
        {
            get
            {
                return down_mag * unfoldedUpUnit;
            }
        }

        public float halfWidth
        {
            get
            {
                if (moveType == VexedLib.MovementType.ChaseBoss)
                    return 2.4f;
                if (sizeType == VexedLib.MonsterSize.Normal)
                    return .5f;
                if (sizeType == VexedLib.MonsterSize.Large)
                    return 1f;
                if (sizeType == VexedLib.MonsterSize.Huge)
                    return 1.5f;
                return .5f;                
            }
        }
        public float halfHeight
        {
            get
            {
                if (moveType == VexedLib.MovementType.ChaseBoss)
                    return 2.4f;
                if (sizeType == VexedLib.MonsterSize.Normal)
                    return .5f;
                if (sizeType == VexedLib.MonsterSize.Large)
                    return 1f;
                if (sizeType == VexedLib.MonsterSize.Huge)
                    return 1.5f;
                return .5f;
            }
        }
        public float depth
        {
            get
            {
                return 0f;
            }
        }


        public float trackRange
        {
            get
            {
                if (gunType == VexedLib.GunType.Missile)
                    return 20f;
                if (gunType == VexedLib.GunType.Beam)
                    return 20f;
                return 15f;
            }
        }

        public void ApplyDamage(bool armor, ProjectileType gunType)
        {
            if (flashCooldown != 0)
                return;
            if (moveType == VexedLib.MovementType.RockBoss && rockBoss.rockHits != 0)
            {
                armor = rockBoss.rockHits == 0;
                if (gunType == ProjectileType.Impact)
                {
                    rockBoss.Impact();
                }
                return;
            }

            if (armor == false)
            {
                if (baseHP > 0)
                {
                    baseHP--;
                    flashCooldown = maxFlashCooldown;
                }
                if (moveType == VexedLib.MovementType.ArmorBoss)
                {
                    armorBoss.Rotate(this);
                }
            }
            else if (gunType == ProjectileType.Missile || gunType == ProjectileType.Bomb)
            {
                armorHP--;
                if (armorHP == 0)
                {
                    armorType = VexedLib.ArmorType.None;
                }
            }
            if (gunType == ProjectileType.Spikes)
            {
                baseHP = 0;
                armorHP = 0;
            }
            if (baseHP == 0)
            {
                if (moveType == VexedLib.MovementType.SnakeBoss)
                    dead = false;
                else if (moveType == VexedLib.MovementType.RockBoss && id.Contains("Standard") && rockBoss.state != RockBossState.Fight3)
                    dead = false;
                else if (moveType == VexedLib.MovementType.RockBoss && id.Contains("Snow") && rockBoss.state != RockBossState.Snow_Battle2)
                    dead = false;
                else if (moveType == VexedLib.MovementType.RockBoss && id.Contains("CommandBoss") && rockBoss.state != RockBossState.Command_Battle2)
                    dead = false;
                else
                    dead = true;

                if (hasOrbs == true)
                {
                    Doodad bonusOrb1 = new Doodad(VexedLib.DoodadType.PowerOrb, position.position + .3f * upUnit, position.normal, position.direction);
                    Doodad bonusOrb2 = new Doodad(VexedLib.DoodadType.PowerOrb, position.position - .3f * upUnit, position.normal, position.direction);
                    Doodad bonusOrb3 = new Doodad(VexedLib.DoodadType.PowerOrb, position.position + .3f * rightUnit, position.normal, position.direction);
                    Doodad bonusOrb4 = new Doodad(VexedLib.DoodadType.PowerOrb, position.position - .3f * rightUnit, position.normal, position.direction);
                    Doodad bonusOrb5 = new Doodad(VexedLib.DoodadType.PowerOrb, position.position, position.normal, position.direction);
                    bonusOrb1.tracking = true;
                    bonusOrb1.currentRoom = Engine.player.currentRoom;
                    bonusOrb1.position.velocity += .3f * upUnit;
                    bonusOrb2.tracking = true;
                    bonusOrb2.currentRoom = Engine.player.currentRoom;
                    bonusOrb2.position.velocity -= .3f * upUnit;
                    bonusOrb3.tracking = true;
                    bonusOrb3.currentRoom = Engine.player.currentRoom;
                    bonusOrb3.position.velocity += .3f * rightUnit;
                    bonusOrb4.tracking = true;
                    bonusOrb4.currentRoom = Engine.player.currentRoom;
                    bonusOrb4.position.velocity -= .3f * rightUnit;
                    bonusOrb5.tracking = true;
                    bonusOrb5.currentRoom = Engine.player.currentRoom;
                    Engine.player.currentRoom.doodads.Add(bonusOrb1);
                    Engine.player.currentRoom.doodads.Add(bonusOrb2);
                    Engine.player.currentRoom.doodads.Add(bonusOrb3);
                    Engine.player.currentRoom.doodads.Add(bonusOrb4);
                    Engine.player.currentRoom.doodads.Add(bonusOrb5);
                    if (moveType == VexedLib.MovementType.RockBoss && !id.Contains("Snow") && rockBoss.state != RockBossState.Fight3)
                        hasOrbs = true;
                    else if (moveType == VexedLib.MovementType.RockBoss && id.Contains("Snow") && rockBoss.state != RockBossState.Snow_Battle2)
                        hasOrbs = true;
                    else
                        hasOrbs = false;
                }                
            }
        }

        public void Update(GameTime gameTime)
        {
            if (moveType == VexedLib.MovementType.SnakeBoss)
            {
                snakeBoss.Update(gameTime.ElapsedGameTime.Milliseconds, this);
            }
            if (moveType == VexedLib.MovementType.RockBoss)
            {
                rockBoss.Update(gameTime.ElapsedGameTime.Milliseconds, this);
            }
            if (moveType == VexedLib.MovementType.ChaseBoss)
            {
                chaseBoss.Update(gameTime.ElapsedGameTime.Milliseconds, this);
            }
            if (moveType == VexedLib.MovementType.BattleBoss)
            {
                battleBoss.Update(gameTime.ElapsedGameTime.Milliseconds, this);
            }
            if (moveType == VexedLib.MovementType.ArmorBoss)
            {
                armorBoss.Update(gameTime.ElapsedGameTime.Milliseconds, this);
            }
            if (moveType == VexedLib.MovementType.FaceBoss)
            {
                faceBoss.Update(gameTime.ElapsedGameTime.Milliseconds, this);
            }
            if (moveType == VexedLib.MovementType.JetBoss)
            {
                jetBoss.Update(gameTime.ElapsedGameTime.Milliseconds, this);
            }
            flashCooldown -= gameTime.ElapsedGameTime.Milliseconds;
            if (flashCooldown < 0)
            {
                flashCooldown = 0;
            }

            Vector3 effectiveUp = Monster.AdjustVector(Engine.player.center.direction, position.normal, Engine.player.center.normal, Engine.player.center.direction, false);
            if ((moveType == VexedLib.MovementType.Tank || moveType == VexedLib.MovementType.Spider) && prevUp != Engine.player.center.direction)
            {
                prevUp = Engine.player.center.direction;
                position.velocity = Vector3.Zero;                
            }
            spinRecovery -= gameTime.ElapsedGameTime.Milliseconds;
            if (spinRecovery < 0) spinRecovery = 0;

            if (dead == true)
                return;
            directionChangeCooldown -= gameTime.ElapsedGameTime.Milliseconds;
            if (directionChangeCooldown < 0)
                directionChangeCooldown = 0;



            if (impactVector != Vector3.Zero)
            {
                impactVector.Normalize();
                bool armorBlock = true;
                if (armorType == VexedLib.ArmorType.None)
                    armorBlock = false;
                else if ((armorType == VexedLib.ArmorType.Top || armorType == VexedLib.ArmorType.TopSuper) && Vector3.Dot(impactVector, position.direction) > .5f)
                    armorBlock = false;
                else if (rightFacing == true && (armorType == VexedLib.ArmorType.Shield || armorType == VexedLib.ArmorType.ShieldSuper) && Vector3.Dot(impactVector, rightUnit) > 0)
                    armorBlock = false;
                else if (rightFacing == false && (armorType == VexedLib.ArmorType.Shield || armorType == VexedLib.ArmorType.ShieldSuper) && Vector3.Dot(impactVector, -rightUnit) > 0)
                    armorBlock = false;

                

                ApplyDamage(armorBlock, lastHitType);

                impactVector = Vector3.Zero;
            }

            if(rockBoss.phasing == true)
                position.Update(Engine.player.currentRoom, gameTime.ElapsedGameTime.Milliseconds,false);
            else
                position.Update(Engine.player.currentRoom, gameTime.ElapsedGameTime.Milliseconds, true);

            Vector3 direction = Vector3.Zero;
            if (aiType == VexedLib.AIType.Waypoint)
            {
                Vector3 target = waypoints[currentWaypointIndex];
                direction = target - position.position;
                if (direction.Length() < 1f)
                {
                    currentWaypointIndex += wayPointDirection;
                    if (!waypointLoop)
                    {
                        if (currentWaypointIndex == waypoints.Count() && wayPointDirection > 0)
                        {
                            wayPointDirection = -wayPointDirection;
                            currentWaypointIndex += 2 * wayPointDirection;
                        }
                        else if (currentWaypointIndex == -1 && wayPointDirection < 0)
                        {
                            wayPointDirection = -wayPointDirection;
                            currentWaypointIndex += 2 * wayPointDirection;
                        }
                    }
                    else
                    {
                        currentWaypointIndex %= waypoints.Count();
                    }
                }
                else
                {
                    direction = direction - Vector3.Dot(direction, position.normal) * position.normal;
                    if (direction.Length() > 1)
                        direction.Normalize();
                }
            }
            if (aiType == VexedLib.AIType.Hunter)
            {

                Vector3 target = Engine.player.center.position;
                float distance = (target - position.position).Length();
                if (distance < trackRange)
                {
                    direction = target - position.position;

                    if (position.normal == -Engine.player.center.normal)
                    {
                        direction *= -1;
                    }
                    if (guns.Count != 0 && moveType != VexedLib.MovementType.Jump && direction.Length() < huntMinDistance)
                    {
                        directionChangeCooldown = 300;
                        direction *= -1;
                    }

                    direction = direction - Vector3.Dot(direction, position.normal) * position.normal;
                    if (direction.Length() > 1)
                        direction.Normalize();
                }
            }
            if (aiType == VexedLib.AIType.Wander)
            {
                direction = currentDirection * rightUnit;
                if ( 
                    ((moveType == VexedLib.MovementType.Spider && groundProjection!=Vector3.Zero)&& (forwardProjection != Vector3.Zero || forwardGroundProjection == Vector3.Zero)) ||
                    ((moveType == VexedLib.MovementType.Tank && Vector3.Dot(groundProjection, effectiveUp) > 0) && (forwardProjection != Vector3.Zero || forwardGroundProjection == Vector3.Zero)) ||
                    (moveType == VexedLib.MovementType.Hover && forwardProjection != Vector3.Zero))
                {

                    position.velocity -= 2*Vector3.Dot(position.velocity, rightUnit)*rightUnit;
                    currentDirection = -currentDirection;
                }
            }
            
            
            if (moveType == VexedLib.MovementType.RockBoss)
            {
                if (rockBoss.nextWaypointTarget != Vector3.Zero)
                {
                    direction = rockBoss.nextWaypointTarget - position.position;
                    if(rockBoss.phasing == false)
                        direction = direction - Vector3.Dot(direction, position.normal) * position.normal;
                    if (direction.Length() > 1)
                        direction.Normalize();
                }
            }
            if (moveType == VexedLib.MovementType.ChaseBoss)
            {
                if (chaseBoss.nextWaypointTarget != Vector3.Zero)
                {
                    direction = chaseBoss.nextWaypointTarget - position.position;
                    direction = direction - Vector3.Dot(direction, position.normal) * position.normal;                    
                }
            }
            if (moveType == VexedLib.MovementType.SnakeBoss)
            {
                if (snakeBoss.nextWaypointTarget != Vector3.Zero && snakeBoss.waiting == false)
                {
                    direction = snakeBoss.nextWaypointTarget - position.position;
                    direction = direction - Vector3.Dot(direction, position.normal) * position.normal;
                }
            }
            if (moveType == VexedLib.MovementType.BattleBoss)
            {
                if (battleBoss.nextWaypointTarget != Vector3.Zero)
                {
                    direction = battleBoss.nextWaypointTarget - position.position;
                    direction = direction - Vector3.Dot(direction, position.normal) * position.normal;
                }
            }
            if (moveType == VexedLib.MovementType.JetBoss)
            {
                if (jetBoss.nextWaypointTarget != Vector3.Zero)
                {
                    direction = jetBoss.nextWaypointTarget - position.position;
                    direction = direction - Vector3.Dot(direction, position.normal) * position.normal;
                }
            }

            foreach (GunEmplacement g in guns)
                g.Upgate(gameTime, this);
            
            if (moveType == VexedLib.MovementType.Tank)
            {

                if (groundProjection != Vector3.Zero)
                {
                    Vector3 groundDirection = groundProjection / groundProjection.Length();
                    if (Vector3.Dot(effectiveUp, position.direction) == 1f)
                    {
                        direction -= Vector3.Dot(groundDirection, direction) * groundDirection;

                        position.velocity += acceleration * direction;
                    }
                }

                if (directionChangeCooldown == 0)
                {
                    if (Vector3.Dot(rightUnit, position.velocity) > 0)
                        rightFacing = true;
                    if (Vector3.Dot(rightUnit, position.velocity) < 0)
                        rightFacing = false;
                }
                if (Vector3.Dot(rightUnit, position.velocity) > 0)
                    rightMoving = true;
                if (Vector3.Dot(rightUnit, position.velocity) < 0)
                    rightMoving = false;


                AdjustVertex(Vector3.Zero, -1.5f * acceleration * Engine.player.center.direction, Engine.player.center.normal, Engine.player.center.direction, false);

                
                if (forwardGroundProjection == Vector3.Zero && Vector3.Dot(groundProjection, effectiveUp) > 0f)
                {
                    position.velocity = Vector3.Zero;
                }
                if (Vector3.Dot(effectiveUp, position.velocity) < -maxFallSpeed)
                {
                    position.velocity -= (Vector3.Dot(effectiveUp, position.velocity) + maxFallSpeed) * effectiveUp;
                }
                if (Vector3.Dot(effectiveUp, groundProjection) > 0)
                {
                    if (Vector3.Dot(rightUnit, position.velocity) > maxSpeed)
                    {
                        position.velocity -= (Vector3.Dot(rightUnit, position.velocity) - maxSpeed) * rightUnit;
                    }
                    if (Vector3.Dot(rightUnit, position.velocity) < -maxSpeed)
                    {
                        position.velocity -= (Vector3.Dot(rightUnit, position.velocity) + maxSpeed) * rightUnit;
                    }
                }
            }
            else if (moveType == VexedLib.MovementType.Spider)
            {
                if (groundProjection != Vector3.Zero)
                {
                    position.velocity += acceleration * direction;
                    Vector3 groundDirection = groundProjection / groundProjection.Length();
                    position.velocity -= Vector3.Dot(position.direction, position.velocity) * position.direction;
                    if (aiType == VexedLib.AIType.Stationary)
                        position.velocity = Vector3.Zero;
                }
                else
                {
                    AdjustVertex(Vector3.Zero, -1.5f * acceleration * Engine.player.center.direction, Engine.player.center.normal, Engine.player.center.direction, false);
                }

                if (directionChangeCooldown == 0)
                {
                    if (Vector3.Dot(rightUnit, position.velocity) > 0)
                        rightFacing = true;
                    if (Vector3.Dot(rightUnit, position.velocity) < 0)
                        rightFacing = false;
                }
                if (Vector3.Dot(rightUnit, position.velocity) > 0)
                    rightMoving = true;
                if (Vector3.Dot(rightUnit, position.velocity) < 0)
                    rightMoving = false;

                if (forwardGroundProjection == Vector3.Zero && groundProjection != Vector3.Zero)
                {
                    position.velocity = Vector3.Zero;
                }
                if (position.velocity.Length() > maxSpeed)
                {
                    position.velocity.Normalize();
                    position.velocity *= maxSpeed;
                }
            }
            else if (moveType == VexedLib.MovementType.Jump)
            {
                if (Vector3.Dot(rightUnit, position.velocity) > 0)
                    rightFacing = true;
                if (Vector3.Dot(rightUnit, position.velocity) < 0)
                    rightFacing = false;
                if (Vector3.Dot(rightUnit, position.velocity) > 0)
                    rightMoving = true;
                if (Vector3.Dot(rightUnit, position.velocity) < 0)
                    rightMoving = false;

                if (spinUp != Vector3.Zero)
                    groundProjection = Vector3.Zero;
                if (position.direction != Monster.AdjustVector(Engine.player.center.direction, position.normal, Engine.player.center.normal, Engine.player.center.direction, false))
                {
                    spinUp = Monster.AdjustVector(Engine.player.center.direction, position.normal, Engine.player.center.normal, Engine.player.center.direction, false);
                }
                if (spinUp != Vector3.Zero)
                {
                    spinTime += gameTime.ElapsedGameTime.Milliseconds;
                    if (spinTime > spinMaxTime)
                    {
                        spinTime = 0;
                        position.direction = spinUp;
                        spinUp = Vector3.Zero;
                    }
                }

                if (groundProjection != Vector3.Zero)
                {
                    jumpCooldown -= gameTime.ElapsedGameTime.Milliseconds;
                    if (jumpCooldown < 0) jumpCooldown = 0;

                    if (jumpTime > 10)
                    {
                        jumpTime = 0;
                        jumping = false;
                    }
                    if (jumping == true)
                        jumpTime += gameTime.ElapsedGameTime.Milliseconds;
                }
                else
                {
                    AdjustVertex(Vector3.Zero, -1.5f * acceleration * Engine.player.center.direction, Engine.player.center.normal, Engine.player.center.direction, false);
                }
                if (groundProjection != Vector3.Zero && jumping == false)
                {
                    position.velocity = Vector3.Zero;
                }
                if (groundProjection != Vector3.Zero && jumpCooldown == 0)
                {
                    jumpCooldown = 500;
                    jumping = true;
                    position.velocity += 30 * acceleration * position.direction;
                    position.velocity += 30 * acceleration * direction;
                }

                if (jumping == false)
                {
                    if (position.velocity.Length() > maxSpeed)
                    {
                        position.velocity.Normalize();
                        position.velocity *= maxSpeed;
                    }
                }
                else
                {
                    if (position.velocity.Length() > 2 * maxSpeed)
                    {
                        position.velocity.Normalize();
                        position.velocity *= 2 * maxSpeed;
                    }

                }
            }
            else if (moveType == VexedLib.MovementType.Hover || moveType == VexedLib.MovementType.RockBoss || moveType == VexedLib.MovementType.ChaseBoss || moveType == VexedLib.MovementType.SnakeBoss|| moveType == VexedLib.MovementType.BattleBoss || moveType == VexedLib.MovementType.JetBoss)
            {
                if (moveType != VexedLib.MovementType.BattleBoss && moveType != VexedLib.MovementType.JetBoss && moveType != VexedLib.MovementType.ArmorBoss)
                {
                    if (directionChangeCooldown == 0)
                    {
                        if (Vector3.Dot(rightUnit, position.velocity) > 0)
                            rightFacing = true;
                        if (Vector3.Dot(rightUnit, position.velocity) < 0)
                            rightFacing = false;
                    }
                    if (Vector3.Dot(rightUnit, position.velocity) > 0)
                        rightMoving = true;
                    if (Vector3.Dot(rightUnit, position.velocity) < 0)
                        rightMoving = false;

                    if ((moveType == VexedLib.MovementType.ChaseBoss) && spinUp == Vector3.Zero)
                    {
                        Vector3 forward = rightUnit;
                        if (rightFacing == false)
                            forward = -forward;

                        Vector3 targetDir = direction;
                        targetDir.Normalize();
                        if (Math.Abs(Vector3.Dot(forward, targetDir)) < .95f)
                        {
                            if (Vector3.Dot(targetDir, position.direction) < 0f)
                                spinUp = forward;
                            else
                                spinUp = -forward;
                        }
                    }
                }

                if ((moveType == VexedLib.MovementType.RockBoss || aiType != VexedLib.AIType.Wander) && moveType != VexedLib.MovementType.BattleBoss && moveType != VexedLib.MovementType.JetBoss && moveType != VexedLib.MovementType.SnakeBoss && Engine.player.spinRecovery == 0 && position.direction != Monster.AdjustVector(Engine.player.center.direction, position.normal, Engine.player.center.normal, Engine.player.center.direction, false))
                {
                    spinUp = Monster.AdjustVector(Engine.player.center.direction, position.normal, Engine.player.center.normal, Engine.player.center.direction, false);
                }
                if (spinUp != Vector3.Zero)
                {
                    spinTime += gameTime.ElapsedGameTime.Milliseconds;
                    if (spinTime > spinMaxTime)
                    {
                        spinTime = 0;
                        position.direction = spinUp;
                        spinUp = Vector3.Zero;
                    }
                }

                position.velocity += acceleration * direction;
                if (position.velocity.Length() > maxSpeed)
                {
                    position.velocity.Normalize();
                    position.velocity *= maxSpeed;
                }
            }
        }

        public List<Vector3> GetCollisionRect()
        {
            List<Vector3> doodadVertexList = new List<Vector3>();

            doodadVertexList.Add(unfoldedPosition.position + unfolded_up + unfolded_right);
            doodadVertexList.Add(unfoldedPosition.position + unfolded_up + unfolded_left);
            doodadVertexList.Add(unfoldedPosition.position + unfolded_down + unfolded_left);
            doodadVertexList.Add(unfoldedPosition.position + unfolded_down + unfolded_right);

            return doodadVertexList;
        }

        public List<Vector3> GetGroundCollisionRect()
        {
            List<Vector3> doodadVertexList = new List<Vector3>();

            doodadVertexList.Add(unfoldedPosition.position + unfolded_right);
            doodadVertexList.Add(unfoldedPosition.position + unfolded_left);
            doodadVertexList.Add(unfoldedPosition.position + 1.1f * unfolded_down + unfolded_left);
            doodadVertexList.Add(unfoldedPosition.position + 1.1f * unfolded_down + unfolded_right);

            return doodadVertexList;
        }

        public List<Vector3> GetForwardGroundCollisionRect()
        {
            List<Vector3> doodadVertexList = new List<Vector3>();
            Vector3 forward = Vector3.Zero;
            
            
            if (rightMoving == false)
            {
                forward = -.75f * unfolded_right;
            }
            else if (rightMoving == true)
            {
                forward = .75f * unfolded_right;
            }
            forward += Vector3.Dot(unfoldedPosition.velocity, forward) * forward;

            doodadVertexList.Add(unfoldedPosition.position + unfolded_right / 4f + forward);
            doodadVertexList.Add(unfoldedPosition.position + unfolded_left / 4f + forward);
            doodadVertexList.Add(unfoldedPosition.position + 1.1f * unfolded_down + unfolded_left / 4f + forward);
            doodadVertexList.Add(unfoldedPosition.position + 1.1f * unfolded_down + unfolded_right / 4f + forward);

            return doodadVertexList;
        }

        public List<Vector3> GetForwardCollisionRect()
        {
            List<Vector3> doodadVertexList = new List<Vector3>();
            Vector3 forward = Vector3.Zero;


            if (rightFacing == false)
            {
                forward = -.75f * unfolded_right;
            }
            else if (rightFacing == true)
            {
                forward = .75f * unfolded_right;
            }
            forward += Vector3.Dot(unfoldedPosition.velocity, forward) * forward;

            /*doodadVertexList.Add(unfoldedPosition.position + .48f * unfolded_up + unfolded_right / 4f + forward);
            doodadVertexList.Add(unfoldedPosition.position + .48f * unfolded_up + unfolded_left / 4f + forward);
            doodadVertexList.Add(unfoldedPosition.position + .48f * unfolded_down + unfolded_left / 4f + forward);
            doodadVertexList.Add(unfoldedPosition.position + .48f * unfolded_down + unfolded_right / 4f + forward);*/
            doodadVertexList.Add(unfoldedPosition.position + unfolded_up + unfolded_right / 4f + forward);
            doodadVertexList.Add(unfoldedPosition.position + unfolded_up + unfolded_left / 4f + forward);
            doodadVertexList.Add(unfoldedPosition.position + unfolded_down + unfolded_left / 4f + forward);
            doodadVertexList.Add(unfoldedPosition.position + unfolded_down + unfolded_right / 4f + forward);

            return doodadVertexList;
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


        public float boundingBoxTop;
        public float boundingBoxBottom;
        public float boundingBoxLeft;
        public float boundingBoxRight;

        public void UpdateBoundingBox(Vector3 playerUp, Vector3 playerRight)
        {
            float x1, x2, x3, x4 = 0;
            float y1, y2, y3, y4 = 0;
            x1 = Vector3.Dot(playerRight, unfoldedPosition.position + unfolded_up);
            x2 = Vector3.Dot(playerRight, unfoldedPosition.position + unfolded_down);
            x3 = Vector3.Dot(playerRight, unfoldedPosition.position + unfolded_left);
            x4 = Vector3.Dot(playerRight, unfoldedPosition.position + unfolded_right);
            y1 = Vector3.Dot(playerUp, unfoldedPosition.position + unfolded_up);
            y2 = Vector3.Dot(playerUp, unfoldedPosition.position + unfolded_down);
            y3 = Vector3.Dot(playerUp, unfoldedPosition.position + unfolded_left);
            y4 = Vector3.Dot(playerUp, unfoldedPosition.position + unfolded_right);
            boundingBoxLeft = x1;
            if (x2 < boundingBoxLeft)
                boundingBoxLeft = x2;
            if (x3 < boundingBoxLeft)
                boundingBoxLeft = x3;
            if (x4 < boundingBoxLeft)
                boundingBoxLeft = x4;
            boundingBoxRight = x1;
            if (x2 > boundingBoxRight)
                boundingBoxRight = x2;
            if (x3 > boundingBoxRight)
                boundingBoxRight = x3;
            if (x4 > boundingBoxRight)
                boundingBoxRight = x4;
            boundingBoxTop = y1;
            if (y2 > boundingBoxTop)
                boundingBoxTop = y2;
            if (y3 > boundingBoxTop)
                boundingBoxTop = y3;
            if (y4 > boundingBoxTop)
                boundingBoxTop = y4;
            boundingBoxBottom = y1;
            if (y2 < boundingBoxBottom)
                boundingBoxBottom = y2;
            if (y3 < boundingBoxBottom)
                boundingBoxBottom = y3;
            if (y4 < boundingBoxBottom)
                boundingBoxBottom = y4;
        }


        public void AdjustVertex(Vector3 pos, Vector3 vel, Vector3 normal, Vector3 playerUp)
        {
            AdjustVertex(pos, vel, normal, playerUp, true);
        }

        public void AdjustVertex(Vector3 pos, Vector3 vel, Vector3 normal, Vector3 playerUp, bool fold)
        {
            Vector3 playerRight = Vector3.Cross(playerUp, normal);
            if (position.normal == normal || (position.normal == -normal && fold==false))
            {
                position.position += pos;
                position.velocity += vel;
            }
            else if (position.normal == -normal)
            {
                float badVelComponent = Vector3.Dot(playerUp, vel);
                float badPosComponent = Vector3.Dot(playerUp, pos);
                position.position += pos - 2 * badPosComponent * playerUp;
                position.velocity += vel - 2 * badVelComponent * playerUp;                
            }
            else if (position.normal == playerUp)
            {
                float badVelComponent = Vector3.Dot(playerUp, vel);
                float badPosComponent = Vector3.Dot(playerUp, pos);
                position.position += pos - badPosComponent * playerUp + badPosComponent * -normal;
                position.velocity += vel - badVelComponent * playerUp + badVelComponent * -normal;
            }
            else if (position.normal == -playerUp)
            {
                float badVelComponent = Vector3.Dot(playerUp, vel);
                float badPosComponent = Vector3.Dot(playerUp, pos);
                position.position += pos - badPosComponent * playerUp + badPosComponent * normal;
                position.velocity += vel - badVelComponent * playerUp + badVelComponent * normal;
            }
            else if (position.normal == playerRight)
            {
                float badVelComponent = Vector3.Dot(playerRight, vel);
                float badPosComponent = Vector3.Dot(playerRight, pos);
                position.position += pos - badPosComponent * playerRight + badPosComponent * -normal;
                position.velocity += vel - badVelComponent * playerRight + badVelComponent * -normal;
            }
            else if (position.normal == -playerRight)
            {
                float badVelComponent = Vector3.Dot(playerRight, vel);
                float badPosComponent = Vector3.Dot(playerRight, pos);
                position.position += pos - badPosComponent * playerRight + badPosComponent * normal;
                position.velocity += vel - badVelComponent * playerRight + badVelComponent * normal;
            }
            else
            {
                position.velocity = Vector3.Zero;
            }
        }


        public static Vector3 AdjustVector(Vector3 input, Vector3 targetNormal, Vector3 playerNnormal, Vector3 playerUp, bool fold)
        {
            Vector3 playerRight = Vector3.Cross(playerUp, playerNnormal);
            if (targetNormal == playerNnormal || (targetNormal == -playerNnormal && fold == false))
            {
                return input;
            }
            else if (targetNormal == -playerNnormal)
            {
                float badVelComponent = Vector3.Dot(playerUp, input);
                return input - 2 * badVelComponent * playerUp;
            }
            else if (targetNormal == playerUp)
            {
                float badVelComponent = Vector3.Dot(playerUp, input);
                return input - badVelComponent * playerUp + badVelComponent * -playerNnormal;
            }
            else if (targetNormal == -playerUp)
            {
                float badVelComponent = Vector3.Dot(playerUp, input);
                return input - badVelComponent * playerUp + badVelComponent * playerNnormal;
            }
            else if (targetNormal == playerRight)
            {
                float badVelComponent = Vector3.Dot(playerRight, input);
                return input - badVelComponent * playerRight + badVelComponent * -playerNnormal;
            }
            else if (targetNormal == -playerRight)
            {
                float badVelComponent = Vector3.Dot(playerRight, input);
                return input - badVelComponent * playerRight + badVelComponent * playerNnormal;
            }
            return Vector3.Zero;
        }

        public void Draw(Room r)
        {
            if (dead == true)
                return;

            if (moveType == VexedLib.MovementType.FaceBoss)
            {
                faceBoss.Render();
                return;
            }

            List<VertexPositionColorNormal> triangleList = new List<VertexPositionColorNormal>();
            List<VertexPositionColorNormalTexture> textureTriangleList = new List<VertexPositionColorNormalTexture>();
            List<Vertex> rectVertexList = new List<Vertex>();
    
            rectVertexList.Add(new Vertex(position.position, position.normal, up + right, position.direction));
            rectVertexList.Add(new Vertex(position.position, position.normal, up +left, position.direction));
            rectVertexList.Add(new Vertex(position.position, position.normal, down +left, position.direction));
            rectVertexList.Add(new Vertex(position.position, position.normal, down + right, position.direction));

            
            foreach (Vertex v in rectVertexList)
            {
                v.Update(Engine.player.currentRoom, 1);
            }

            foreach (GunEmplacement g in guns)
            {
                List<Vertex> gunVertexList = new List<Vertex>();
                gunVertexList.Add(new Vertex(g.position.position, g.position.normal, g.radius * g.gunNormal, g.position.direction));
                gunVertexList.Add(new Vertex(g.position.position, g.position.normal, 1.5f * g.radius * g.gunLine + g.radius * g.gunNormal, g.position.direction));
                gunVertexList.Add(new Vertex(g.position.position, g.position.normal, 1.5f * g.radius * g.gunLine - g.radius * g.gunNormal, g.position.direction));
                gunVertexList.Add(new Vertex(g.position.position, g.position.normal, -g.radius * g.gunNormal, g.position.direction));

                

                foreach (Vertex v in gunVertexList)
                {
                    v.Update(Engine.player.currentRoom, 1);
                }

                
                r.AddTextureToTriangleList(gunVertexList, Color.White, depth + g.depthOffset, textureTriangleList, gunTexCoords, true);
                if (g.baseType != BaseType.None)
                {
                    List<Vertex> gunBaseVertexList = new List<Vertex>();
                    gunBaseVertexList.Add(new Vertex(position.position, position.normal, upUnit * (g.baseRadius-g.positionOffset.Y) + rightUnit * (g.baseRadius+g.positionOffset.X), g.position.direction));
                    gunBaseVertexList.Add(new Vertex(position.position, position.normal, upUnit * (g.baseRadius - g.positionOffset.Y) - rightUnit * (g.baseRadius - g.positionOffset.X), g.position.direction));
                    gunBaseVertexList.Add(new Vertex(position.position, position.normal, -upUnit * (g.baseRadius + g.positionOffset.Y) - rightUnit * (g.baseRadius - g.positionOffset.X), g.position.direction));
                    gunBaseVertexList.Add(new Vertex(position.position, position.normal, -upUnit * (g.baseRadius + g.positionOffset.Y) + rightUnit * (g.baseRadius + g.positionOffset.X), g.position.direction));

                    foreach (Vertex v in gunBaseVertexList)
                    {
                        v.Update(Engine.player.currentRoom, 1);
                    }
                    if (g.baseType == BaseType.Rock)
                        r.AddTextureToTriangleList(gunBaseVertexList, Color.White, depth + 1.5f * g.depthOffset, textureTriangleList, stoneSolidTexCoords, true);
                    if (g.baseType == BaseType.Snow)
                        r.AddTextureToTriangleList(gunBaseVertexList, Color.White, depth + 1.5f * g.depthOffset, textureTriangleList, snowSolidTexCoords, true);
                    if (g.baseType == BaseType.Ice)
                        r.AddTextureToTriangleList(gunBaseVertexList, Color.White, depth + 1.5f * g.depthOffset, textureTriangleList, iceTurretTexCoords, true);
                    if (g.baseType == BaseType.Standard)
                        r.AddTextureToTriangleList(gunBaseVertexList, Color.Gray, depth + 1.5f * g.depthOffset, textureTriangleList, bodyTexCoords, true);
                }
            }

            if (moveType == VexedLib.MovementType.Tank)
                r.AddTextureToTriangleList(rectVertexList, Color.White, depth - .1f, textureTriangleList, treadsTexCoords, rightFacing);
            else if (moveType == VexedLib.MovementType.Spider)
                r.AddTextureToTriangleList(rectVertexList, Color.White, depth - .1f, textureTriangleList, spiderTexCoords, rightFacing);
            else if (moveType == VexedLib.MovementType.Jump)
                r.AddTextureToTriangleList(rectVertexList, Color.White, depth - .1f, textureTriangleList, legsTexCoords, rightFacing);

            float bossAdjustment = 0f;
            if(snakeBoss.chainIndex%2!=0)
                bossAdjustment = .001f;
            r.AddTextureToTriangleList(rectVertexList, monsterColor, depth + bossAdjustment, textureTriangleList, bodyTexCoords, rightFacing);
            if (flashCooldown != 0)
            {
                Color flashColor = new Color(255, 255, 0, (Byte)(flashCooldown / maxFlashCooldown));
                r.AddTextureToTriangleList(rectVertexList, flashColor, depth + bossAdjustment, textureTriangleList, flashTexCoords, rightFacing);
            }
            if (moveType == VexedLib.MovementType.RockBoss && !id.Contains("Snow"))
            {
                if(rockBoss.rockHits == 2 && rockBoss.rockHitCooldown == 0)
                    r.AddTextureToTriangleList(rectVertexList, Color.White, depth, textureTriangleList, stoneSolidTexCoords, rightFacing);
                if (rockBoss.rockHits == 1 && rockBoss.rockHitCooldown == 0)
                    r.AddTextureToTriangleList(rectVertexList, Color.White, depth, textureTriangleList, stoneCrackTexCoords, rightFacing);
                if (rockBoss.rockHits == 1 && rockBoss.rockHitCooldown != 0)
                    r.AddTextureToTriangleList(rectVertexList, Color.White, depth, textureTriangleList, stoneSolidBreakTexCoords, rightFacing);
                if (rockBoss.rockHits == 0 && rockBoss.rockHitCooldown != 0)
                    r.AddTextureToTriangleList(rectVertexList, Color.White, depth, textureTriangleList, stoneCrackBreakTexCoords, rightFacing);
            }
            if (moveType == VexedLib.MovementType.RockBoss && id.Contains("Snow"))
            {
                if (rockBoss.rockHits == 2 && rockBoss.rockHitCooldown == 0)
                    r.AddTextureToTriangleList(rectVertexList, Color.White, depth, textureTriangleList, snowSolidTexCoords, rightFacing);
                if (rockBoss.rockHits == 1 && rockBoss.rockHitCooldown == 0)
                    r.AddTextureToTriangleList(rectVertexList, Color.White, depth, textureTriangleList, snowCrackTexCoords, rightFacing);
                if (rockBoss.rockHits == 1 && rockBoss.rockHitCooldown != 0)
                    r.AddTextureToTriangleList(rectVertexList, Color.White, depth, textureTriangleList, snowSolidBreakTexCoords, rightFacing);
                if (rockBoss.rockHits == 0 && rockBoss.rockHitCooldown != 0)
                    r.AddTextureToTriangleList(rectVertexList, Color.White, depth, textureTriangleList, snowCrackBreakTexCoords, rightFacing);
            }
            if (moveType == VexedLib.MovementType.ChaseBoss || moveType == VexedLib.MovementType.JetBoss)
            {
                List<Vertex> spikeShieldVertexList = new List<Vertex>();
                Vector3 forward = Vector3.Cross(position.direction, position.normal);
                float shieldOffset = halfWidth / 4f;
                if (rightFacing == false)
                    shieldOffset = -shieldOffset;
                spikeShieldVertexList.Add(new Vertex(position.position, position.normal, up + right + shieldOffset * forward, position.direction));
                spikeShieldVertexList.Add(new Vertex(position.position, position.normal, up + left + shieldOffset * forward, position.direction));
                spikeShieldVertexList.Add(new Vertex(position.position, position.normal, down + left + shieldOffset * forward, position.direction));
                spikeShieldVertexList.Add(new Vertex(position.position, position.normal, down + right + shieldOffset * forward, position.direction));
                foreach (Vertex v in spikeShieldVertexList)
                {
                    v.Update(Engine.player.currentRoom, 1);
                }
                if(moveType == VexedLib.MovementType.ChaseBoss)
                    r.AddTextureToTriangleList(spikeShieldVertexList, Color.White, depth + .09f, textureTriangleList, spikeShieldTexCoords, rightFacing);
                if(moveType == VexedLib.MovementType.JetBoss)
                    r.AddTextureToTriangleList(spikeShieldVertexList, Color.White, depth + .09f, textureTriangleList, jetArmorTexCoords, rightFacing);

            }

            if (moveType == VexedLib.MovementType.SnakeBoss)
            {
                if (baseHP > 3)
                {
                    r.AddTextureToTriangleList(rectVertexList, Color.White, depth + bossAdjustment, textureTriangleList, iceShieldFullTexCoords, rightFacing);
                }
                else if (baseHP > 2)
                {
                    r.AddTextureToTriangleList(rectVertexList, Color.White, depth + bossAdjustment, textureTriangleList, iceShieldMedTexCoords, rightFacing);
                }
                else if (baseHP > 1)
                {
                    r.AddTextureToTriangleList(rectVertexList, Color.White, depth + bossAdjustment, textureTriangleList, iceShieldLowTexCoords, rightFacing);
                }
            }

            if (moveType == VexedLib.MovementType.BattleBoss)
            {
                r.AddTextureToTriangleList(rectVertexList, Color.White, depth + bossAdjustment, textureTriangleList, bossShieldTexCoords, rightFacing);
            }
            if (moveType == VexedLib.MovementType.ArmorBoss)
            {
                r.AddTextureToTriangleList(rectVertexList, Color.White, depth + bossAdjustment, textureTriangleList, bossArmorTexCoords, rightFacing);
            }

            if (moveType != VexedLib.MovementType.ChaseBoss && moveType != VexedLib.MovementType.RockBoss && moveType != VexedLib.MovementType.ArmorBoss && moveType != VexedLib.MovementType.BattleBoss && moveType != VexedLib.MovementType.JetBoss && moveType != VexedLib.MovementType.ArmorBoss)
            {
                if (armorType == VexedLib.ArmorType.Full)
                    r.AddTextureToTriangleList(rectVertexList, Color.White, depth, textureTriangleList, fullArmorTexCoords, rightFacing);
                if (armorType == VexedLib.ArmorType.Top)
                    r.AddTextureToTriangleList(rectVertexList, Color.White, depth, textureTriangleList, topArmorTexCoords, rightFacing);
                if (armorType == VexedLib.ArmorType.Shield)
                    r.AddTextureToTriangleList(rectVertexList, Color.White, depth, textureTriangleList, frontArmorTexCoords, rightFacing);
                if (armorType == VexedLib.ArmorType.FullSuper)
                    r.AddTextureToTriangleList(rectVertexList, Color.White, depth, textureTriangleList, fullSuperArmorTexCoords, rightFacing);
                if (armorType == VexedLib.ArmorType.TopSuper)
                    r.AddTextureToTriangleList(rectVertexList, Color.White, depth, textureTriangleList, topSuperArmorTexCoords, rightFacing);
                if (armorType == VexedLib.ArmorType.ShieldSuper)
                    r.AddTextureToTriangleList(rectVertexList, Color.White, depth, textureTriangleList, frontSuperArmorTexCoords, rightFacing);
            }
            

            if (moveType != VexedLib.MovementType.ChaseBoss && moveType != VexedLib.MovementType.SnakeBoss && moveType != VexedLib.MovementType.BattleBoss && moveType != VexedLib.MovementType.JetBoss && moveType != VexedLib.MovementType.ArmorBoss)
                r.AddTextureToTriangleList(rectVertexList, Color.White, depth, textureTriangleList, eyesTexCoords, rightFacing);
            if (moveType == VexedLib.MovementType.SnakeBoss && snakeBoss.chainIndex == 0)
                r.AddTextureToTriangleList(rectVertexList, Color.White, depth + bossAdjustment, textureTriangleList, eyesTexCoords, rightFacing);

            if(moveType == VexedLib.MovementType.Tank)
                r.AddTextureToTriangleList(rectVertexList, Color.White, depth+.1f, textureTriangleList, treadsTexCoords, rightFacing);
            else if (moveType == VexedLib.MovementType.Spider)
                r.AddTextureToTriangleList(rectVertexList, Color.White, depth+.1f, textureTriangleList, spiderTexCoords, rightFacing);
            else if (moveType == VexedLib.MovementType.Jump)
                r.AddTextureToTriangleList(rectVertexList, Color.White, depth+.1f, textureTriangleList, legsTexCoords, rightFacing);

            
            VertexPositionColorNormalTexture[] triangleArray = textureTriangleList.ToArray();
            Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                triangleArray, 0, triangleArray.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
        }
    }
}
