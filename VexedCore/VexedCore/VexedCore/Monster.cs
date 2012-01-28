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
    public enum MonsterState
    {
        Spawn,
        Normal,
        Death
    }

    public enum ArmorState
    {
        Normal,
        Break
    }

    public enum MonsterTextureId
    {
        Gun,
        BasicBody,
        SnakeBody,
        Eyes,
        FullArmor,
        TopArmor,
        FrontArmor,
        FullArmorBack,
        TopArmorBack,
        FrontArmorBack,
        LegsBase,
        Legs2,
        Legs3,
        Legs4,
        TreadsBase,
        Treads2,
        Treads3,
        Treads4,
        BossArmor,
        BossGun,
        StandardTurret,
        FullSuperArmor,
        TopSuperArmor,
        FrontSuperArmor,
        FullSuperArmorBack,
        TopSuperArmorBack,
        FrontSuperArmorBack,
        RockNormal,
        RockCracked,
        RockNormalBreak,
        RockCrackedBreak,
        SnowNormal,
        SnowCracked,
        SnowNormalBreak,
        SnowCrackedBreak,
        BossEyes,
        RockTurret,
        SnowTurret,
        SpikeFace,
        Jet,
        IceSnake,
        IceSnakeCrack1,
        IceSnakeCrack2,
        IceTurret,
        FacePlate,
        FaceWhite,
        FaceAngryEye,
        FaceNormalEye,
        Flash
    }

    public class Monster
    {
        public static int textureCount = 49;
        

        public static Texture2D monsterTexture;
        public static Texture2D monsterTextureDetail;

        public static Texture2D[] monsterTextures;
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
        public VL.AIType aiType;
        public VL.MovementType moveType;
        public VL.MonsterHealth healthType;
        public VL.MonsterSpeed speedType;
        public VL.MonsterSize sizeType;
        public VL.TrackType trackType;
        public VL.ArmorType startingArmorType;
        public VL.ArmorType armorType;
        public VL.GunType gunType;
        public Vector3 groundProjection;
        public Vector3 forwardGroundProjection;
        public Vector3 forwardProjection;
        public bool jumping = false;
        public int jumpCooldown = 0;
        public int jumpTime = 0;
        public Vector3 spinUp;

        public int flashCooldown;
        public static int maxFlashCooldown = 200;
        public int spinTime = 0;
        public bool rightFacing = true;
        public bool rightMoving = false;
        public int currentDirection = 1;
        public float huntMinDistance = 3.5f;
        
        public static int maxArmorBreakTime = 150;
        public int armorBreakTime = 0;
        public int spawnTime = 0;
        public static int maxSpawnTime = 500;
        
        public static int maxDeathTime = 300;
        public int deathTime = maxDeathTime;
        public MonsterState state = MonsterState.Spawn;
        public ArmorState armorState = ArmorState.Normal;

            
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

        public static void Init(ContentManager content)
        {
            monsterTextures = new Texture2D[textureCount];
            monsterTextures[(int)MonsterTextureId.BasicBody] = content.Load<Texture2D>("MonsterParts\\basicBody");
            monsterTextures[(int)MonsterTextureId.SnakeBody] = content.Load<Texture2D>("MonsterParts\\snakeBody");
            monsterTextures[(int)MonsterTextureId.BossArmor] = content.Load<Texture2D>("MonsterParts\\bossArmor");
            monsterTextures[(int)MonsterTextureId.Eyes] = content.Load<Texture2D>("MonsterParts\\eyes");
            monsterTextures[(int)MonsterTextureId.BossEyes] = content.Load<Texture2D>("MonsterParts\\bossEyes");
            monsterTextures[(int)MonsterTextureId.FaceAngryEye] = content.Load<Texture2D>("MonsterParts\\faceAngryEye");
            monsterTextures[(int)MonsterTextureId.FaceNormalEye] = content.Load<Texture2D>("MonsterParts\\faceNormalEye");
            monsterTextures[(int)MonsterTextureId.FacePlate] = content.Load<Texture2D>("MonsterParts\\facePlate");
            monsterTextures[(int)MonsterTextureId.FaceWhite] = content.Load<Texture2D>("MonsterParts\\faceWhite");
            monsterTextures[(int)MonsterTextureId.FrontArmor] = content.Load<Texture2D>("MonsterParts\\frontArmor");
            monsterTextures[(int)MonsterTextureId.FrontSuperArmor] = content.Load<Texture2D>("MonsterParts\\frontSuperArmor");
            monsterTextures[(int)MonsterTextureId.FullArmor] = content.Load<Texture2D>("MonsterParts\\fullArmor");
            monsterTextures[(int)MonsterTextureId.FullSuperArmor] = content.Load<Texture2D>("MonsterParts\\fullSuperArmor");
            monsterTextures[(int)MonsterTextureId.FrontArmorBack] = content.Load<Texture2D>("MonsterParts\\frontArmorBack");
            monsterTextures[(int)MonsterTextureId.FrontSuperArmorBack] = content.Load<Texture2D>("MonsterParts\\frontSuperArmorBack");
            monsterTextures[(int)MonsterTextureId.FullArmorBack] = content.Load<Texture2D>("MonsterParts\\fullArmorBack");
            monsterTextures[(int)MonsterTextureId.FullSuperArmorBack] = content.Load<Texture2D>("MonsterParts\\fullSuperArmorBack");
            monsterTextures[(int)MonsterTextureId.Gun] = content.Load<Texture2D>("MonsterParts\\gun");
            monsterTextures[(int)MonsterTextureId.IceSnake] = content.Load<Texture2D>("MonsterParts\\iceSnake");
            monsterTextures[(int)MonsterTextureId.IceSnakeCrack1] = content.Load<Texture2D>("MonsterParts\\iceSnakeCrack1");
            monsterTextures[(int)MonsterTextureId.IceSnakeCrack2] = content.Load<Texture2D>("MonsterParts\\iceSnakeCrack2");
            monsterTextures[(int)MonsterTextureId.IceTurret] = content.Load<Texture2D>("MonsterParts\\iceTurret");
            monsterTextures[(int)MonsterTextureId.Jet] = content.Load<Texture2D>("MonsterParts\\jet");
            monsterTextures[(int)MonsterTextureId.Legs2] = content.Load<Texture2D>("MonsterParts\\legs2");
            monsterTextures[(int)MonsterTextureId.Legs3] = content.Load<Texture2D>("MonsterParts\\legs3");
            monsterTextures[(int)MonsterTextureId.Legs4] = content.Load<Texture2D>("MonsterParts\\legs4");
            monsterTextures[(int)MonsterTextureId.LegsBase] = content.Load<Texture2D>("MonsterParts\\legs1");
            monsterTextures[(int)MonsterTextureId.RockCracked] = content.Load<Texture2D>("MonsterParts\\rockCracked");
            monsterTextures[(int)MonsterTextureId.RockCrackedBreak] = content.Load<Texture2D>("MonsterParts\\rockCrackedBreak");
            monsterTextures[(int)MonsterTextureId.RockNormal] = content.Load<Texture2D>("MonsterParts\\rockNormal");
            monsterTextures[(int)MonsterTextureId.RockNormalBreak] = content.Load<Texture2D>("MonsterParts\\rockNormalBreak");
            monsterTextures[(int)MonsterTextureId.SnowCracked] = content.Load<Texture2D>("MonsterParts\\snowCracked");
            monsterTextures[(int)MonsterTextureId.SnowCrackedBreak] = content.Load<Texture2D>("MonsterParts\\snowCrackedBreak");
            monsterTextures[(int)MonsterTextureId.SnowNormal] = content.Load<Texture2D>("MonsterParts\\snowNormal");
            monsterTextures[(int)MonsterTextureId.SnowNormalBreak] = content.Load<Texture2D>("MonsterParts\\snowNormalBreak");
            monsterTextures[(int)MonsterTextureId.SpikeFace] = content.Load<Texture2D>("MonsterParts\\spikeFace");
            monsterTextures[(int)MonsterTextureId.TopArmor] = content.Load<Texture2D>("MonsterParts\\topArmor");
            monsterTextures[(int)MonsterTextureId.TopSuperArmor] = content.Load<Texture2D>("MonsterParts\\topSuperArmor");
            monsterTextures[(int)MonsterTextureId.TopArmorBack] = content.Load<Texture2D>("MonsterParts\\topArmorBack");
            monsterTextures[(int)MonsterTextureId.TopSuperArmorBack] = content.Load<Texture2D>("MonsterParts\\topSuperArmorBack");
            monsterTextures[(int)MonsterTextureId.Treads2] = content.Load<Texture2D>("MonsterParts\\treads2");
            monsterTextures[(int)MonsterTextureId.Treads3] = content.Load<Texture2D>("MonsterParts\\treads3");
            monsterTextures[(int)MonsterTextureId.Treads4] = content.Load<Texture2D>("MonsterParts\\treads4");
            monsterTextures[(int)MonsterTextureId.TreadsBase] = content.Load<Texture2D>("MonsterParts\\treads1");
            monsterTextures[(int)MonsterTextureId.Flash] = content.Load<Texture2D>("MonsterParts\\flash");
            monsterTextures[(int)MonsterTextureId.StandardTurret] = content.Load<Texture2D>("MonsterParts\\standardTurret");
            monsterTextures[(int)MonsterTextureId.RockTurret] = content.Load<Texture2D>("MonsterParts\\rockTurret");
            monsterTextures[(int)MonsterTextureId.SnowTurret] = content.Load<Texture2D>("MonsterParts\\snowTurret");
            monsterTextures[(int)MonsterTextureId.BossGun] = content.Load<Texture2D>("MonsterParts\\bossGun");      
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

        public Monster(VL.Monster xmlMonster, Vector3 normal)
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
            if (moveType == VL.MovementType.RockBoss)
            {
                if (id.Contains("Snow"))
                {
                    guns.Add(new GunEmplacement(VL.TrackType.Normal, VL.GunType.Blaster, new Vector2(1.3f, .3f), .7f * halfWidth, .05f, BaseType.Snow, this));
                    guns.Add(new GunEmplacement(VL.TrackType.Normal, VL.GunType.Blaster, new Vector2(-1.3f, .3f), .7f * halfWidth, .05f, BaseType.Snow, this));
                }
                else
                {
                    guns.Add(new GunEmplacement(VL.TrackType.Fast, VL.GunType.Blaster, new Vector2(1.3f, .3f), .7f * halfWidth, .05f, BaseType.Rock, this));
                    guns.Add(new GunEmplacement(VL.TrackType.Fast, VL.GunType.Blaster, new Vector2(-1.3f, .3f), .7f * halfWidth, .05f, BaseType.Rock, this));
                }
            }
            else if (moveType == VL.MovementType.ChaseBoss)
            {
                rightFacing = true;
                armorHP = 1;
                startingArmorType = VL.ArmorType.ShieldSuper;
            }
            else if (moveType == VL.MovementType.SnakeBoss)
            {
                
            }
            else if (moveType == VL.MovementType.BattleBoss)
            {
                guns.Add(new GunEmplacement(VL.TrackType.Normal, VL.GunType.None, new Vector2(.3f, 1.3f), .7f * halfWidth, .05f, BaseType.Standard, this));
                guns.Add(new GunEmplacement(VL.TrackType.Normal, VL.GunType.None, new Vector2(.3f, -1.3f), .7f * halfWidth, .05f, BaseType.Standard, this));
            }
            else if (moveType == VL.MovementType.JetBoss)
            {
                guns.Add(new GunEmplacement(VL.TrackType.Normal, VL.GunType.Blaster, new Vector2(.3f, -1.3f), .7f * halfWidth, .05f, BaseType.Standard, this));
            }
            else if (moveType == VL.MovementType.ArmorBoss)
            {
                if (id.Contains("Basic"))
                {

                }
                rightFacing = true;
                startingArmorType = VL.ArmorType.ShieldSuper;
                guns.Add(new GunEmplacement(VL.TrackType.Normal, VL.GunType.Blaster, new Vector2(.3f, -1.3f), .7f * halfWidth, .05f, BaseType.Standard, this));
                guns.Add(new GunEmplacement(VL.TrackType.Normal, VL.GunType.Blaster, new Vector2(.3f, 1.3f), .7f * halfWidth, .05f, BaseType.Standard, this));
            }
            else if(gunType != VL.GunType.None)
                guns.Add(new GunEmplacement(trackType, gunType, Vector2.Zero, halfWidth, -.05f, BaseType.None, this));

            baseHP = startingBaseHP;
        }

        public void Load(Mo m)
        {
            hasOrbs = m.ho;
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
                if (moveType == VL.MovementType.ArmorBoss)
                {
                    if (id.Contains("Basic"))
                        return 16;
                    return 6;
                }
                if (moveType == VL.MovementType.JetBoss)
                    return 48;
                if (moveType == VL.MovementType.FaceBoss)
                    return 4;
                if (moveType == VL.MovementType.BattleBoss)
                    return 5;
                if (moveType == VL.MovementType.SnakeBoss)
                    return 4;
                if (healthType == VL.MonsterHealth.Weak)
                    return 1;
                if (healthType == VL.MonsterHealth.Normal)
                    return 4;
                if (healthType == VL.MonsterHealth.Tough)
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
                if (moveType == VL.MovementType.SnakeBoss)
                {
                    if (baseHP > 0)
                        return new Color(60, 60, 100);
                    else
                        return new Color(60, 60, 60);
                }
                if (moveType == VL.MovementType.RockBoss)
                {
                    if (id.Contains("Snow"))
                        return new Color(60, 60, 150);
                    return Color.OrangeRed;
                }
                if (gunType == VL.GunType.None)
                {
                    return new Color(60, 60, 60);
                }
                if (gunType == VL.GunType.Blaster)
                {
                    return Color.Red;
                }
                if (gunType == VL.GunType.Beam)
                {
                    return Color.Blue;
                }
                if (gunType == VL.GunType.Missile)
                {
                    return Color.Gray;
                }
                if (gunType == VL.GunType.Spread)
                {
                    return Color.Green;
                }
                if (gunType == VL.GunType.Repeater)
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
                if(speedType == VL.MonsterSpeed.Medium)
                    return .009f;
                if (speedType == VL.MonsterSpeed.Slow)
                    return .002f;
                if (speedType == VL.MonsterSpeed.Fast)
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
                if (moveType == VL.MovementType.ChaseBoss)
                    return 2.4f;
                if (sizeType == VL.MonsterSize.Normal)
                    return .5f;
                if (sizeType == VL.MonsterSize.Large)
                    return 1f;
                if (sizeType == VL.MonsterSize.Huge)
                    return 1.5f;
                return .5f;                
            }
        }
        public float halfHeight
        {
            get
            {
                if (moveType == VL.MovementType.ChaseBoss)
                    return 2.4f;
                if (sizeType == VL.MonsterSize.Normal)
                    return .5f;
                if (sizeType == VL.MonsterSize.Large)
                    return 1f;
                if (sizeType == VL.MonsterSize.Huge)
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
                if (gunType == VL.GunType.Missile)
                    return 20f;
                if (gunType == VL.GunType.Beam)
                    return 20f;
                return 15f;
            }
        }

        public void ApplyDamage(bool armor, ProjectileType gunType)
        {
            if (dead == true)
                return;

            if (flashCooldown != 0)
                return;
            if (moveType == VL.MovementType.RockBoss && rockBoss.rockHits != 0)
            {
                armor = rockBoss.rockHits == 0;
                if (gunType == ProjectileType.Impact)
                {
                    rockBoss.Impact();
                }
                return;
            }
            if(moveType == VL.MovementType.RockBoss && (rockBoss.state != RockBossState.Command_Battle1 && rockBoss.state != RockBossState.Command_Battle2 && rockBoss.state != RockBossState.Snow_Battle1 && rockBoss.state != RockBossState.Snow_Battle2))
                return;

            if (armor == false)
            {
                if (baseHP > 0)
                {
                    SoundFX.MonsterHit();
                    baseHP--;
                    flashCooldown = maxFlashCooldown;
                }
                if (moveType == VL.MovementType.ArmorBoss)
                {
                    armorBoss.Rotate(this);
                }
            }
            else if (gunType == ProjectileType.Missile || gunType == ProjectileType.Bomb)
            {
                if (armorType != VL.ArmorType.FullSuper && armorType != VL.ArmorType.ShieldSuper && armorType != VL.ArmorType.TopSuper)
                {
                    armorHP--;
                    if (armorHP == 0)
                    {
                        SoundFX.ArmorBreak();
                        armorState = ArmorState.Break;
                    }
                }
            }
            else
            {
                SoundFX.ArmorHit();
            }

            if (gunType == ProjectileType.Spikes)
            {
                baseHP = 0;
                armorHP = 0;
                SoundFX.ArmorBreak();
            }
            if (baseHP == 0)
            {
                if (moveType == VL.MovementType.BattleBoss && battleBoss.phase < 2)                
                    dead = false;                
                else if (moveType == VL.MovementType.SnakeBoss && SnakeBoss.totalLife > 0)
                    dead = false;
                else if (moveType == VL.MovementType.RockBoss && id.Contains("Standard") && rockBoss.state != RockBossState.Fight3)
                    dead = false;
                else if (moveType == VL.MovementType.RockBoss && id.Contains("Snow") && rockBoss.state != RockBossState.Snow_Battle2)
                    dead = false;
                else if (moveType == VL.MovementType.RockBoss && id.Contains("CommandBoss") && rockBoss.state != RockBossState.Command_Battle2)
                    dead = false;
                else
                {
                    dead = true;
                    SoundFX.MonsterDeath();

                    state = MonsterState.Death;
                    if (armorState == ArmorState.Normal)
                        armorState = ArmorState.Break;
                }

                if (hasOrbs == true)
                {
                    /*Doodad bonusOrb1 = new Doodad(VL.DoodadType.PowerOrb, position.position + .3f * upUnit, position.normal, position.direction);
                    Doodad bonusOrb2 = new Doodad(VL.DoodadType.PowerOrb, position.position - .3f * upUnit, position.normal, position.direction);
                    Doodad bonusOrb3 = new Doodad(VL.DoodadType.PowerOrb, position.position + .3f * rightUnit, position.normal, position.direction);
                    Doodad bonusOrb4 = new Doodad(VL.DoodadType.PowerOrb, position.position - .3f * rightUnit, position.normal, position.direction);
                    Doodad bonusOrb5 = new Doodad(VL.DoodadType.PowerOrb, position.position, position.normal, position.direction);*/
                    Doodad bonusOrb1 = Engine.player.currentRoom.ActivateMonsterOrb();
                    Doodad bonusOrb2 = Engine.player.currentRoom.ActivateMonsterOrb();
                    Doodad bonusOrb3 = Engine.player.currentRoom.ActivateMonsterOrb();
                    Doodad bonusOrb4 = Engine.player.currentRoom.ActivateMonsterOrb();
                    Doodad bonusOrb5 = Engine.player.currentRoom.ActivateMonsterOrb();
                    if (bonusOrb1 != null)
                    {
                        bonusOrb1.tracking = true;
                        bonusOrb1.currentRoom = Engine.player.currentRoom;
                        bonusOrb1.position.velocity += .3f * upUnit;
                        bonusOrb1.position.position = position.position + .3f * upUnit;
                        bonusOrb1.position.normal = position.normal;
                        bonusOrb1.position.direction = position.direction;
                    }
                    if (bonusOrb2 != null)
                    {
                        bonusOrb2.tracking = true;
                        bonusOrb2.currentRoom = Engine.player.currentRoom;
                        bonusOrb2.position.velocity -= .3f * upUnit;
                        bonusOrb2.position.position = position.position - .3f * upUnit;
                        bonusOrb2.position.normal = position.normal;
                        bonusOrb2.position.direction = position.direction; bonusOrb3.tracking = true;
                    }
                    if (bonusOrb3 != null)
                    {
                        bonusOrb3.tracking = true;
                        bonusOrb3.currentRoom = Engine.player.currentRoom;
                        bonusOrb3.position.position = position.position + .3f * rightUnit;
                        bonusOrb3.position.velocity += .3f * rightUnit;
                        bonusOrb3.position.normal = position.normal;
                        bonusOrb3.position.direction = position.direction; bonusOrb3.position.position = position.position + .3f * rightUnit;
                    }
                    if (bonusOrb4 != null)
                    {
                        bonusOrb4.tracking = true;
                        bonusOrb4.currentRoom = Engine.player.currentRoom;
                        bonusOrb4.position.velocity -= .3f * rightUnit;
                        bonusOrb4.position.position = position.position - .3f * rightUnit;
                        bonusOrb4.position.normal = position.normal;
                        bonusOrb4.position.direction = position.direction; bonusOrb5.tracking = true;
                    }
                    if (bonusOrb5 != null)
                    {
                        bonusOrb5.tracking = true;
                        bonusOrb5.currentRoom = Engine.player.currentRoom;
                        bonusOrb5.position.position = position.position;
                        bonusOrb5.position.normal = position.normal;
                        bonusOrb5.position.direction = position.direction;
                    }

                    if (moveType == VL.MovementType.RockBoss && !id.Contains("Snow") && rockBoss.state != RockBossState.Fight3)
                        hasOrbs = true;
                    else
                        hasOrbs = false;
                }                
            }
        }

        public void Update(int gameTime)
        {
            if (armorState == ArmorState.Break)
            {
                armorBreakTime += gameTime;
                if (armorBreakTime > maxArmorBreakTime)
                {
                    armorBreakTime = maxArmorBreakTime;
                    armorType = VL.ArmorType.None;                        
                }
            }
            if (state == MonsterState.Spawn)
            {
                spawnTime += gameTime;
                if (spawnTime > maxSpawnTime)
                    state = MonsterState.Normal;
                return;
            }

            if (moveType == VL.MovementType.SnakeBoss)
            {
                snakeBoss.Update(gameTime, this);
            }
            if (moveType == VL.MovementType.RockBoss)
            {
                rockBoss.Update(gameTime, this);
            }
            if (moveType == VL.MovementType.ChaseBoss)
            {
                chaseBoss.Update(gameTime, this);
            }
            if (moveType == VL.MovementType.BattleBoss)
            {
                battleBoss.Update(gameTime, this);
            }
            if (moveType == VL.MovementType.ArmorBoss)
            {
                armorBoss.Update(gameTime, this);
            }
            if (moveType == VL.MovementType.FaceBoss)
            {
                faceBoss.Update(gameTime, this);
            }
            if (moveType == VL.MovementType.JetBoss)
            {
                jetBoss.Update(gameTime, this);
            }

            if (state == MonsterState.Death)
            {
                deathTime -= gameTime;
                if (deathTime < 0)
                    deathTime = 0;
                flashCooldown -= gameTime;
                if (flashCooldown < 0)
                {
                    flashCooldown = 0;
                }
                return;
            }

            flashCooldown -= gameTime;
            if (flashCooldown < 0)
            {
                flashCooldown = 0;
            }

            Vector3 effectiveUp = Monster.AdjustVector(Engine.player.center.direction, position.normal, Engine.player.center.normal, Engine.player.center.direction, false);
            if ((moveType == VL.MovementType.Tank || moveType == VL.MovementType.Spider) && prevUp != Engine.player.center.direction)
            {
                prevUp = Engine.player.center.direction;
                position.velocity = Vector3.Zero;                
            }
            spinRecovery -= gameTime;
            if (spinRecovery < 0) spinRecovery = 0;

            if (dead == true)
                return;
            directionChangeCooldown -= gameTime;
            if (directionChangeCooldown < 0)
                directionChangeCooldown = 0;



            if (impactVector != Vector3.Zero)
            {
                impactVector.Normalize();
                bool armorBlock = true;
                if (armorType == VL.ArmorType.None)
                    armorBlock = false;
                else if ((armorType == VL.ArmorType.Top || armorType == VL.ArmorType.TopSuper) && Vector3.Dot(impactVector, position.direction) > .5f)
                    armorBlock = false;
                else if (rightFacing == true && (armorType == VL.ArmorType.Shield || armorType == VL.ArmorType.ShieldSuper) && Vector3.Dot(impactVector, rightUnit) > 0)
                    armorBlock = false;
                else if (rightFacing == false && (armorType == VL.ArmorType.Shield || armorType == VL.ArmorType.ShieldSuper) && Vector3.Dot(impactVector, -rightUnit) > 0)
                    armorBlock = false;

                

                ApplyDamage(armorBlock, lastHitType);

                impactVector = Vector3.Zero;
            }

            if(rockBoss.phasing == true)
                position.Update(Engine.player.currentRoom, gameTime,false);
            else
                position.Update(Engine.player.currentRoom, gameTime, true);

            Vector3 direction = Vector3.Zero;
            if (aiType == VL.AIType.Waypoint)
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
            if (aiType == VL.AIType.Hunter)
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
                    if (guns.Count != 0 && moveType != VL.MovementType.Jump && direction.Length() < huntMinDistance)
                    {
                        directionChangeCooldown = 300;
                        direction *= -1;
                    }

                    direction = direction - Vector3.Dot(direction, position.normal) * position.normal;
                    if (direction.Length() > 1)
                        direction.Normalize();
                }
            }
            if (aiType == VL.AIType.Wander)
            {
                direction = currentDirection * rightUnit;
                if ( 
                    ((moveType == VL.MovementType.Spider && groundProjection!=Vector3.Zero)&& (forwardProjection != Vector3.Zero || forwardGroundProjection == Vector3.Zero)) ||
                    ((moveType == VL.MovementType.Tank && Vector3.Dot(groundProjection, effectiveUp) > 0) && (forwardProjection != Vector3.Zero || forwardGroundProjection == Vector3.Zero)) ||
                    (moveType == VL.MovementType.Hover && forwardProjection != Vector3.Zero))
                {

                    position.velocity -= 2*Vector3.Dot(position.velocity, rightUnit)*rightUnit;
                    currentDirection = -currentDirection;
                }
            }
            
            
            if (moveType == VL.MovementType.RockBoss)
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
            if (moveType == VL.MovementType.ChaseBoss)
            {
                if (chaseBoss.nextWaypointTarget != Vector3.Zero)
                {
                    direction = chaseBoss.nextWaypointTarget - position.position;
                    direction = direction - Vector3.Dot(direction, position.normal) * position.normal;                    
                }
            }
            if (moveType == VL.MovementType.SnakeBoss)
            {
                if (snakeBoss.nextWaypointTarget != Vector3.Zero && snakeBoss.waiting == false)
                {
                    direction = snakeBoss.nextWaypointTarget - position.position;
                    direction = direction - Vector3.Dot(direction, position.normal) * position.normal;
                }
            }
            if (moveType == VL.MovementType.BattleBoss)
            {
                if (battleBoss.nextWaypointTarget != Vector3.Zero)
                {
                    direction = battleBoss.nextWaypointTarget - position.position;
                    direction = direction - Vector3.Dot(direction, position.normal) * position.normal;
                }
            }
            if (moveType == VL.MovementType.JetBoss)
            {
                if (jetBoss.nextWaypointTarget != Vector3.Zero)
                {
                    direction = jetBoss.nextWaypointTarget - position.position;
                    direction = direction - Vector3.Dot(direction, position.normal) * position.normal;
                }
            }

            foreach (GunEmplacement g in guns)
                g.Upgate(gameTime, this);
            
            if (moveType == VL.MovementType.Tank)
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
            else if (moveType == VL.MovementType.Spider)
            {
                if (groundProjection != Vector3.Zero)
                {
                    position.velocity += acceleration * direction;
                    Vector3 groundDirection = groundProjection / groundProjection.Length();
                    position.velocity -= Vector3.Dot(position.direction, position.velocity) * position.direction;
                    if (aiType == VL.AIType.Stationary)
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
            else if (moveType == VL.MovementType.Jump)
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
                    spinTime += gameTime;
                    if (spinTime > spinMaxTime)
                    {
                        spinTime = 0;
                        position.direction = spinUp;
                        spinUp = Vector3.Zero;
                    }
                }

                if (groundProjection != Vector3.Zero)
                {
                    jumpCooldown -= gameTime;
                    if (jumpCooldown < 0) jumpCooldown = 0;

                    if (jumpTime > 10)
                    {
                        jumpTime = 0;
                        jumping = false;
                    }
                    if (jumping == true)
                        jumpTime += gameTime;
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
            else if (moveType == VL.MovementType.Hover || moveType == VL.MovementType.RockBoss || moveType == VL.MovementType.ChaseBoss || moveType == VL.MovementType.SnakeBoss|| moveType == VL.MovementType.BattleBoss || moveType == VL.MovementType.JetBoss)
            {
                if (moveType != VL.MovementType.BattleBoss && moveType != VL.MovementType.JetBoss && moveType != VL.MovementType.ArmorBoss)
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

                    if ((moveType == VL.MovementType.ChaseBoss) && spinUp == Vector3.Zero)
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

                if ((moveType == VL.MovementType.RockBoss || aiType != VL.AIType.Wander) && moveType != VL.MovementType.BattleBoss && moveType != VL.MovementType.JetBoss && moveType != VL.MovementType.SnakeBoss && Engine.player.spinRecovery == 0 && position.direction != Monster.AdjustVector(Engine.player.center.direction, position.normal, Engine.player.center.normal, Engine.player.center.direction, false))
                {
                    spinUp = Monster.AdjustVector(Engine.player.center.direction, position.normal, Engine.player.center.normal, Engine.player.center.direction, false);
                    if (Vector3.Dot(spinUp, position.direction) == -1)
                    {
                        spinUp = Vector3.Cross(position.normal, position.direction);
                    }
                }
                if (spinUp != Vector3.Zero)
                {
                    spinTime += gameTime;
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

        public static float layer_gunNatural = 0f;
        public static float layer_body = .001f;
        public static float layer_eyes = .0015f;
        public static float layer_armor = .0022f;
        public static float layer_iceSnakeArmor = .02f;
        public static float layer_bossEyes = .0025f;
        public static float layer_legs = .1f;
        public static float layer_innerGun = .0018f;
        public static float layer_innerTurretOut = .002f;
        public static float layer_innerTurretIn = .0016f;
        public static float layer_outerGun = .0028f;
        public static float layer_standardTurretOut = .003f;
        public static float layer_standardTurretIn = .0026f;

        public void Draw(Room r)
        {
             if (deathTime == 0)
                return;

            float scale = (1f * spawnTime) / (1f * maxSpawnTime);
            float armorScale = scale;
            if (state == MonsterState.Death)
            {
                scale = (1f * deathTime) / (1f * maxDeathTime);
            }
            if (armorState == ArmorState.Break)
            {
                armorScale = 1f + (.5f * armorBreakTime) / (.5f * maxArmorBreakTime);
            }

            if (moveType == VL.MovementType.FaceBoss)
            {
                //faceBoss.Render();
                return;
            }



            List<VertexPositionColorNormal> triangleList = new List<VertexPositionColorNormal>();
            List<VertexPositionColorNormalTexture> textureTriangleList = new List<VertexPositionColorNormalTexture>();
            List<Vertex> rectVertexList = new List<Vertex>();

            rectVertexList.Add(new Vertex(position.position, position.normal, scale * (up + right), position.direction));
            rectVertexList.Add(new Vertex(position.position, position.normal, scale * (up + left), position.direction));
            rectVertexList.Add(new Vertex(position.position, position.normal, scale * (down + left), position.direction));
            rectVertexList.Add(new Vertex(position.position, position.normal, scale * (down + right), position.direction));

            List<Vertex> armorRectVertexList = new List<Vertex>();

            armorRectVertexList.Add(new Vertex(position.position, position.normal, armorScale * (up + right), position.direction));
            armorRectVertexList.Add(new Vertex(position.position, position.normal, armorScale * (up + left), position.direction));
            armorRectVertexList.Add(new Vertex(position.position, position.normal, armorScale * (down + left), position.direction));
            armorRectVertexList.Add(new Vertex(position.position, position.normal, armorScale * (down + right), position.direction));

            
            foreach (Vertex v in rectVertexList)
            {
                v.Update(Engine.player.currentRoom, 1);
            }
            foreach (Vertex v in armorRectVertexList)
            {
                v.Update(Engine.player.currentRoom, 1);
            }

            float bossAdjustment = .0f;
            if (snakeBoss.chainIndex % 3 == 1)
                bossAdjustment = .01f;
            if (snakeBoss.chainIndex % 3 == 2)
                bossAdjustment = .02f;

            

            if (moveType == VL.MovementType.Tank)
            {
                r.AddTextureToTriangleList(rectVertexList, Color.White, depth - layer_legs, r.monsterTriangles[(int)MonsterTextureId.TreadsBase], Room.plateTexCoords, rightFacing);
            }
            else if (moveType == VL.MovementType.Spider)
            {
                r.AddTextureToTriangleList(rectVertexList, Color.White, depth - layer_legs, r.monsterTriangles[(int)MonsterTextureId.LegsBase], Room.plateTexCoords, rightFacing);
            }

            if (moveType == VL.MovementType.SnakeBoss)
            {
                r.AddTextureToTriangleList(rectVertexList, monsterColor, depth + layer_body + bossAdjustment, r.monsterTriangles[(int)MonsterTextureId.SnakeBody], Room.plateTexCoords, rightFacing);
                r.AddTextureToTriangleList(rectVertexList, monsterColor, depth - layer_body + bossAdjustment, r.monsterTriangles[(int)MonsterTextureId.SnakeBody], Room.plateTexCoords, rightFacing);
            }
            else
            {
                r.AddTextureToTriangleList(rectVertexList, monsterColor, depth + layer_body + bossAdjustment, r.monsterTriangles[(int)MonsterTextureId.BasicBody], Room.plateTexCoords, rightFacing);
                r.AddTextureToTriangleList(rectVertexList, monsterColor, depth - layer_body + bossAdjustment, r.monsterTriangles[(int)MonsterTextureId.BasicBody], Room.plateTexCoords, rightFacing);
            }

            if (flashCooldown != 0)
            {
                Color flashColor = new Color(255, 255, 0, (Byte)(flashCooldown / maxFlashCooldown));
                r.AddTextureToTriangleList(rectVertexList, flashColor, depth + layer_body + bossAdjustment, r.monsterTriangles[(int)MonsterTextureId.Flash], Room.plateTexCoords, rightFacing);
                r.AddTextureToTriangleList(rectVertexList, flashColor, depth - layer_body + bossAdjustment, r.monsterTriangles[(int)MonsterTextureId.Flash], Room.plateTexCoords, rightFacing);
            }
            if (moveType == VL.MovementType.RockBoss && !id.Contains("Snow"))
            {
                if(rockBoss.rockHits == 2 && rockBoss.rockHitCooldown == 0)
                    r.AddTextureToTriangleList(armorRectVertexList, Color.White, depth + .001f, r.monsterTriangles[(int)MonsterTextureId.RockNormal], Room.plateTexCoords, rightFacing);
                if (rockBoss.rockHits == 1 && rockBoss.rockHitCooldown == 0)
                    r.AddTextureToTriangleList(armorRectVertexList, Color.White, depth + .001f, r.monsterTriangles[(int)MonsterTextureId.RockCracked], Room.plateTexCoords, rightFacing);
                if (rockBoss.rockHits == 1 && rockBoss.rockHitCooldown != 0)
                    r.AddTextureToTriangleList(armorRectVertexList, Color.White, depth + .001f, r.monsterTriangles[(int)MonsterTextureId.RockNormalBreak], Room.plateTexCoords, rightFacing);
                if (rockBoss.rockHits == 0 && rockBoss.rockHitCooldown != 0)
                    r.AddTextureToTriangleList(armorRectVertexList, Color.White, depth + .001f, r.monsterTriangles[(int)MonsterTextureId.RockCrackedBreak], Room.plateTexCoords, rightFacing);
            }
            if (moveType == VL.MovementType.RockBoss && id.Contains("Snow"))
            {
                if (rockBoss.rockHits == 2 && rockBoss.rockHitCooldown == 0)
                    r.AddTextureToTriangleList(armorRectVertexList, Color.White, depth + .001f, r.monsterTriangles[(int)MonsterTextureId.SnowNormal], Room.plateTexCoords, rightFacing);
                if (rockBoss.rockHits == 1 && rockBoss.rockHitCooldown == 0)
                    r.AddTextureToTriangleList(armorRectVertexList, Color.White, depth + .001f, r.monsterTriangles[(int)MonsterTextureId.SnowCracked], Room.plateTexCoords, rightFacing);
                if (rockBoss.rockHits == 1 && rockBoss.rockHitCooldown != 0)
                    r.AddTextureToTriangleList(armorRectVertexList, Color.White, depth + .001f, r.monsterTriangles[(int)MonsterTextureId.SnowNormalBreak], Room.plateTexCoords, rightFacing);
                if (rockBoss.rockHits == 0 && rockBoss.rockHitCooldown != 0)
                    r.AddTextureToTriangleList(armorRectVertexList, Color.White, depth + .001f, r.monsterTriangles[(int)MonsterTextureId.SnowCrackedBreak], Room.plateTexCoords, rightFacing);

            }
            if (moveType == VL.MovementType.ChaseBoss || moveType == VL.MovementType.JetBoss || moveType == VL.MovementType.BattleBoss)
            {
                List<Vertex> spikeShieldVertexList = new List<Vertex>();
                Vector3 forward = Vector3.Cross(position.direction, position.normal);
                float shieldOffset = halfWidth / 4f;
                if (rightFacing == false)
                    shieldOffset = -shieldOffset;
                spikeShieldVertexList.Add(new Vertex(position.position, position.normal, armorScale*(up + right + shieldOffset * forward), position.direction));
                spikeShieldVertexList.Add(new Vertex(position.position, position.normal, armorScale*(up + left + shieldOffset * forward), position.direction));
                spikeShieldVertexList.Add(new Vertex(position.position, position.normal, armorScale*(down + left + shieldOffset * forward), position.direction));
                spikeShieldVertexList.Add(new Vertex(position.position, position.normal, armorScale*(down + right + shieldOffset * forward), position.direction));
                foreach (Vertex v in spikeShieldVertexList)
                {
                    v.Update(Engine.player.currentRoom, 1);
                }
                // CHASE BOSS
                if (moveType == VL.MovementType.ChaseBoss)
                {
                    r.AddTextureToTriangleList(spikeShieldVertexList, Color.White, depth + layer_armor, r.monsterTriangles[(int)MonsterTextureId.SpikeFace], Room.plateTexCoords, rightFacing);
                    r.AddTextureToTriangleList(spikeShieldVertexList, Color.White, depth - layer_armor, r.monsterTriangles[(int)MonsterTextureId.SpikeFace], Room.plateTexCoords, rightFacing);
                }
                if (moveType == VL.MovementType.JetBoss || moveType == VL.MovementType.BattleBoss)
                {
                    r.AddTextureToTriangleList(spikeShieldVertexList, Color.White, depth + layer_armor, r.monsterTriangles[(int)MonsterTextureId.Jet], Room.plateTexCoords, rightFacing);
                    r.AddTextureToTriangleList(spikeShieldVertexList, Color.White, depth - layer_armor, r.monsterTriangles[(int)MonsterTextureId.Jet], Room.plateTexCoords, rightFacing);
                }

            }

            if (moveType == VL.MovementType.SnakeBoss)
            {
                if (baseHP > 3)
                {
                    r.AddTextureToTriangleList(armorRectVertexList, Color.White, depth + bossAdjustment + layer_iceSnakeArmor, r.monsterTriangles[(int)MonsterTextureId.IceSnake], Room.plateTexCoords, rightFacing);
                    r.AddTextureToTriangleList(armorRectVertexList, Color.White, depth + bossAdjustment - layer_iceSnakeArmor, r.monsterTriangles[(int)MonsterTextureId.IceSnake], Room.plateTexCoords, rightFacing);
                }
                else if (baseHP > 2)
                {
                    r.AddTextureToTriangleList(armorRectVertexList, Color.White, depth + bossAdjustment + layer_iceSnakeArmor, r.monsterTriangles[(int)MonsterTextureId.IceSnakeCrack1], Room.plateTexCoords, rightFacing);
                    r.AddTextureToTriangleList(armorRectVertexList, Color.White, depth + bossAdjustment - layer_iceSnakeArmor, r.monsterTriangles[(int)MonsterTextureId.IceSnakeCrack1], Room.plateTexCoords, rightFacing);
                }
                else if (baseHP > 1)
                {
                    r.AddTextureToTriangleList(armorRectVertexList, Color.White, depth + bossAdjustment + layer_iceSnakeArmor, r.monsterTriangles[(int)MonsterTextureId.IceSnakeCrack2], Room.plateTexCoords, rightFacing);
                    r.AddTextureToTriangleList(armorRectVertexList, Color.White, depth + bossAdjustment - layer_iceSnakeArmor, r.monsterTriangles[(int)MonsterTextureId.IceSnakeCrack2], Room.plateTexCoords, rightFacing);
                }
            }


            // ARMORTRON ARMOR
            if (moveType == VL.MovementType.ArmorBoss)
            {
                r.AddTextureToTriangleList(armorRectVertexList, Color.White, depth + bossAdjustment + layer_armor, r.monsterTriangles[(int)MonsterTextureId.BossArmor], Room.plateTexCoords, rightFacing);
                r.AddTextureToTriangleList(armorRectVertexList, Color.White, depth + bossAdjustment - layer_armor, r.monsterTriangles[(int)MonsterTextureId.BossArmor], Room.plateTexCoords, rightFacing);
            }

            // STANDARD ARMOR
            if (moveType != VL.MovementType.ChaseBoss && moveType != VL.MovementType.RockBoss && moveType != VL.MovementType.ArmorBoss && moveType != VL.MovementType.BattleBoss && moveType != VL.MovementType.JetBoss && moveType != VL.MovementType.ArmorBoss)
            {
                if (armorType == VL.ArmorType.Full)
                {
                    r.AddTextureToTriangleList(armorRectVertexList, Color.White, depth + layer_armor, r.monsterTriangles[(int)MonsterTextureId.FullArmor], Room.plateTexCoords, rightFacing);
                    r.AddTextureToTriangleList(armorRectVertexList, Color.White, depth - layer_armor, r.monsterTriangles[(int)MonsterTextureId.FullArmor], Room.plateTexCoords, rightFacing);
                }
                if (armorType == VL.ArmorType.Top)
                {
                    r.AddTextureToTriangleList(armorRectVertexList, Color.White, depth + layer_armor, r.monsterTriangles[(int)MonsterTextureId.TopArmor], Room.plateTexCoords, rightFacing);
                    r.AddTextureToTriangleList(armorRectVertexList, Color.White, depth - layer_armor, r.monsterTriangles[(int)MonsterTextureId.TopArmor], Room.plateTexCoords, rightFacing);
                }
                if (armorType == VL.ArmorType.Shield)
                {
                    r.AddTextureToTriangleList(armorRectVertexList, Color.White, depth + layer_armor, r.monsterTriangles[(int)MonsterTextureId.FrontArmor], Room.plateTexCoords, rightFacing);
                    r.AddTextureToTriangleList(armorRectVertexList, Color.White, depth - layer_armor, r.monsterTriangles[(int)MonsterTextureId.FrontArmor], Room.plateTexCoords, rightFacing);
                }
                if (armorType == VL.ArmorType.FullSuper)
                {
                    r.AddTextureToTriangleList(armorRectVertexList, Color.White, depth + layer_armor, r.monsterTriangles[(int)MonsterTextureId.FullSuperArmor], Room.plateTexCoords, rightFacing);
                    r.AddTextureToTriangleList(armorRectVertexList, Color.White, depth - layer_armor, r.monsterTriangles[(int)MonsterTextureId.FullSuperArmor], Room.plateTexCoords, rightFacing);
                }
                if (armorType == VL.ArmorType.TopSuper)
                {
                    r.AddTextureToTriangleList(armorRectVertexList, Color.White, depth + layer_armor, r.monsterTriangles[(int)MonsterTextureId.TopSuperArmor], Room.plateTexCoords, rightFacing);
                    r.AddTextureToTriangleList(armorRectVertexList, Color.White, depth - layer_armor, r.monsterTriangles[(int)MonsterTextureId.TopSuperArmor], Room.plateTexCoords, rightFacing);
                }
                if (armorType == VL.ArmorType.ShieldSuper)
                {
                    r.AddTextureToTriangleList(armorRectVertexList, Color.White, depth + layer_armor, r.monsterTriangles[(int)MonsterTextureId.FrontSuperArmor], Room.plateTexCoords, rightFacing);
                    r.AddTextureToTriangleList(armorRectVertexList, Color.White, depth - layer_armor, r.monsterTriangles[(int)MonsterTextureId.FrontSuperArmor], Room.plateTexCoords, rightFacing);
                }
            }

            // EYES
            if (moveType == VL.MovementType.RockBoss)
            {
                r.AddTextureToTriangleList(rectVertexList, Color.White, depth + layer_bossEyes, r.monsterTriangles[(int)MonsterTextureId.BossEyes], Room.plateTexCoords, rightFacing);
                r.AddTextureToTriangleList(rectVertexList, Color.White, depth - layer_bossEyes, r.monsterTriangles[(int)MonsterTextureId.BossEyes], Room.plateTexCoords, rightFacing);
            }
            else if (moveType != VL.MovementType.ChaseBoss && moveType != VL.MovementType.SnakeBoss && moveType != VL.MovementType.BattleBoss && moveType != VL.MovementType.JetBoss && moveType != VL.MovementType.ArmorBoss)
            {
                r.AddTextureToTriangleList(rectVertexList, Color.White, depth + layer_eyes, r.monsterTriangles[(int)MonsterTextureId.Eyes], Room.plateTexCoords, rightFacing);
                r.AddTextureToTriangleList(rectVertexList, Color.White, depth - layer_eyes, r.monsterTriangles[(int)MonsterTextureId.Eyes], Room.plateTexCoords, rightFacing);
            }
            else if (moveType == VL.MovementType.SnakeBoss && snakeBoss.chainIndex == 0)
            {
                r.AddTextureToTriangleList(rectVertexList, Color.White, depth + bossAdjustment + .021f, r.monsterTriangles[(int)MonsterTextureId.BossEyes], Room.plateTexCoords, rightFacing);
            }

            // FRONT LEGS
            if (moveType == VL.MovementType.Tank)
            {
                r.AddTextureToTriangleList(rectVertexList, Color.White, depth + .1f, r.monsterTriangles[(int)MonsterTextureId.TreadsBase], Room.plateTexCoords, rightFacing);
            }
            else if (moveType == VL.MovementType.Spider)
            {
                r.AddTextureToTriangleList(rectVertexList, Color.White, depth + .1f, r.monsterTriangles[(int)MonsterTextureId.LegsBase], Room.plateTexCoords, rightFacing);
            }


            // GUN EMPLACEMENTS
            foreach (GunEmplacement g in guns)
            {
                List<Vertex> gunVertexList = new List<Vertex>();
                gunVertexList.Add(new Vertex(g.position.position, g.position.normal, scale * (g.radius * g.gunNormal), g.position.direction));
                gunVertexList.Add(new Vertex(g.position.position, g.position.normal, scale * (1.5f * g.radius * g.gunLine + g.radius * g.gunNormal), g.position.direction));
                gunVertexList.Add(new Vertex(g.position.position, g.position.normal, scale * (1.5f * g.radius * g.gunLine - g.radius * g.gunNormal), g.position.direction));
                gunVertexList.Add(new Vertex(g.position.position, g.position.normal, scale * (-g.radius * g.gunNormal), g.position.direction));



                foreach (Vertex v in gunVertexList)
                {
                    v.Update(Engine.player.currentRoom, 1);
                }


                // GUNS
                if(g.baseType == BaseType.None)
                    r.AddTextureToTriangleList(gunVertexList, Color.White, depth + bossAdjustment, r.monsterTriangles[(int)MonsterTextureId.Gun], Room.plateTexCoords, true);
                else if (moveType == VL.MovementType.BattleBoss)
                {
                    r.AddTextureToTriangleList(gunVertexList, Color.White, depth + layer_innerGun + bossAdjustment, r.monsterTriangles[(int)MonsterTextureId.BossGun], Room.plateTexCoords, true);
                }
                else if (g.baseType == BaseType.Ice)
                    r.AddTextureToTriangleList(gunVertexList, Color.White, depth + layer_innerGun + bossAdjustment, r.monsterTriangles[(int)MonsterTextureId.BossGun], Room.plateTexCoords, true);
                else if (g.baseType == BaseType.Standard || g.baseType == BaseType.Snow || g.baseType == BaseType.Rock)
                    r.AddTextureToTriangleList(gunVertexList, Color.White, depth + layer_outerGun + bossAdjustment, r.monsterTriangles[(int)MonsterTextureId.BossGun], Room.plateTexCoords, true);

                
                if (g.baseType != BaseType.None)
                {
                    List<Vertex> gunBaseVertexList = new List<Vertex>();
                    gunBaseVertexList.Add(new Vertex(position.position, position.normal, scale * (upUnit * (g.baseRadius - g.positionOffset.Y) + rightUnit * (g.baseRadius + g.positionOffset.X)), g.position.direction));
                    gunBaseVertexList.Add(new Vertex(position.position, position.normal, scale * (upUnit * (g.baseRadius - g.positionOffset.Y) - rightUnit * (g.baseRadius - g.positionOffset.X)), g.position.direction));
                    gunBaseVertexList.Add(new Vertex(position.position, position.normal, scale * (-upUnit * (g.baseRadius + g.positionOffset.Y) - rightUnit * (g.baseRadius - g.positionOffset.X)), g.position.direction));
                    gunBaseVertexList.Add(new Vertex(position.position, position.normal, scale * (-upUnit * (g.baseRadius + g.positionOffset.Y) + rightUnit * (g.baseRadius + g.positionOffset.X)), g.position.direction));

                    foreach (Vertex v in gunBaseVertexList)
                    {
                        v.Update(Engine.player.currentRoom, 1);
                    }

                    // TURRETS
                    if (g.baseType == BaseType.Rock)
                    {
                        r.AddTextureToTriangleList(gunBaseVertexList, Color.White, depth + layer_standardTurretOut + bossAdjustment, r.monsterTriangles[(int)MonsterTextureId.RockTurret], Room.plateTexCoords, true);
                    }
                    else if (g.baseType == BaseType.Snow)
                    {
                        r.AddTextureToTriangleList(gunBaseVertexList, Color.White, depth + layer_standardTurretOut + bossAdjustment, r.monsterTriangles[(int)MonsterTextureId.SnowTurret], Room.plateTexCoords, true);
                    }
                    else if (g.baseType == BaseType.Ice)
                    {
                        r.AddTextureToTriangleList(gunBaseVertexList, Color.White, depth + layer_innerTurretOut + bossAdjustment, r.monsterTriangles[(int)MonsterTextureId.IceTurret], Room.plateTexCoords, true);
                    }
                    else if (moveType == VL.MovementType.BattleBoss)
                    {
                        r.AddTextureToTriangleList(gunBaseVertexList, Color.Gray, depth + layer_innerTurretIn + bossAdjustment, r.monsterTriangles[(int)MonsterTextureId.StandardTurret], Room.plateTexCoords, true);
                        r.AddTextureToTriangleList(gunBaseVertexList, Color.Gray, depth + layer_innerTurretOut + bossAdjustment, r.monsterTriangles[(int)MonsterTextureId.StandardTurret], Room.plateTexCoords, true);
                    }
                    else if (g.baseType == BaseType.Standard)
                    {
                        r.AddTextureToTriangleList(gunBaseVertexList, Color.Gray, depth + layer_standardTurretIn + bossAdjustment, r.monsterTriangles[(int)MonsterTextureId.StandardTurret], Room.plateTexCoords, true);
                        r.AddTextureToTriangleList(gunBaseVertexList, Color.Gray, depth + layer_standardTurretOut + bossAdjustment, r.monsterTriangles[(int)MonsterTextureId.StandardTurret], Room.plateTexCoords, true);
                    }
                }
            }
            
            /*VertexPositionColorNormalTexture[] triangleArray = textureTriangleList.ToArray();
            Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                triangleArray, 0, triangleArray.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);*/
        }
    }
}
