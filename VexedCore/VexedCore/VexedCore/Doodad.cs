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
    public enum RoomStyle
    {
        Electric,
        Flame,
        Blue,
        Blade
    }

    public enum Decal
    {
        Save = 0,
        Health = 1,
        BlueKey,
        BlueLock,
        Booster,
        Boots,
        Cherry,
        DoubleJump,
        HookTarget,
        ImprovedJump,
        JetPack,
        JumpPad,
        Laser,
        LaserSwitch,
        Missile,
        Onion,
        BlueCodes,
        PermanantBoots,
        RedCodes,
        PermanantWallJump,
        YellowCodes,
        Phase,
        Blaster,
        PlugSlot,
        PowerOrb,
        PowerPlug,
        RainDrop,
        RedKey,
        RedLock,
        Station,
        WallJump,
        Warp,
        YellowKey,
        YellowLock,
        Leaf,
        Lander,
        Skull,
        Switch,
        Empty
    }
   
    public class Doodad
    {
        public Vertex unfoldedPosition;
        public Vertex position;
        public Vertex spawnPosition;
        public bool active = false;
        public bool ready = false;
        public bool available = true;
        public bool tracking = false;

        public bool refreshVertexData = false;
        public string id = "";
        public string targetBehavior ="";
        public string targetObject = "";
        public string expectedBehavior ="";
        public VL.DoodadType type;
        public List<Behavior> behaviors;
        [XmlIgnore]public Behavior currentBehavior = null;
        public String currentBehaviorId;
        [XmlIgnore]public Room currentRoom;

        public RoomStyle style = RoomStyle.Electric;
        public int orbsRemaining = 10;

        public int currentTime = 0;
        public bool nextBehavior = false;
        public bool behaviorStarted = false;
        public bool toggleOn = true;
        public bool alreadyUsed = false;
        [XmlIgnore]public Doodad targetDoodad = null;
        [XmlIgnore]public Block targetBlock = null;
        [XmlIgnore]public Edge targetEdge = null;
        [XmlIgnore]public Room targetRoom = null;
        public string targetDoodadId;
        public string targetBlockId;
        public string targetEdgeId;
        public string targetRoomId;

        public Decal doorDecal = 0;
        public bool idle = false;

        public int animationFrame = 0;
        public int animationTime = 0;

        public int helpIconTime = 0;
        public int helpIconMaxTime = 100;

        public int cooldown = 0;
        public int activationCost = 0;

        public int flashTime = 0;
        public static int maxFlashTime = 1000;

        public AbilityType abilityType = AbilityType.Empty;
        public AbilityType originalAbilityType = AbilityType.Empty;

        [XmlIgnore]public Doodad srcDoodad = null;

        public float stateTransition = 1;
        public float stateTransitionVelocity = .005f;
        public int stateTransitionDir = 0;

        //public static Texture2D beam_textures;
        public static List<Texture2D> flame_beam_textures;
        public static List<Texture2D> electric_beam_textures;
        public static List<List<Vector2>> texCoordList;
        public static List<Vector2> beamTexCoords;

        public static List<Texture2D> decalTextures;

        public static Texture2D useButton;
        public static Texture2D leftButton;
        public static Texture2D rightButton;

        public static int texGridCount = 8;

        public static List<Vector2> LoadTexCoords(int x, int y)
        {
            float texWidth = 1f / texGridCount;
            List<Vector2> texCoords = new List<Vector2>();
            texCoords.Add(new Vector2((x + 1) * texWidth, y*2 * texWidth));
            texCoords.Add(new Vector2(x * texWidth, y*2 * texWidth));
            texCoords.Add(new Vector2(x * texWidth, (y*2 + 2) * texWidth));
            texCoords.Add(new Vector2((x + 1) * texWidth, (y*2 + 2) * texWidth));

            return texCoords;
        }
        public static void InitTexCoords()
        {
            texCoordList = new List<List<Vector2>>();
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    texCoordList.Add(LoadTexCoords(x, y));
                }
            }
        }

        public static void InitDecalTextures(ContentManager Content)
        {
            decalTextures = new List<Texture2D>();
            for (int i = 0; i < 60; i++)
            {
                decalTextures.Add(null);
            }
            decalTextures[(int)Decal.Save] = Content.Load<Texture2D>("decal_save");
            decalTextures[(int)Decal.Health] = Content.Load<Texture2D>("decal_health");
            decalTextures[(int)Decal.BlueKey] = Content.Load<Texture2D>("decal_bluekey");
            decalTextures[(int)Decal.BlueLock] = Content.Load<Texture2D>("decal_bluelock");
            decalTextures[(int)Decal.Booster] = Content.Load<Texture2D>("decal_booster");
            decalTextures[(int)Decal.Boots] = Content.Load<Texture2D>("decal_boots");
            decalTextures[(int)Decal.Cherry] = Content.Load<Texture2D>("decal_cherry");
            decalTextures[(int)Decal.DoubleJump] = Content.Load<Texture2D>("decal_doublejump");
            decalTextures[(int)Decal.HookTarget] = Content.Load<Texture2D>("decal_hooktarget");
            decalTextures[(int)Decal.ImprovedJump] = Content.Load<Texture2D>("decal_improvedjump");
            decalTextures[(int)Decal.JetPack] = Content.Load<Texture2D>("decal_jetpack");
            decalTextures[(int)Decal.JumpPad] = Content.Load<Texture2D>("decal_jump");
            decalTextures[(int)Decal.Laser] = Content.Load<Texture2D>("decal_laser");
            decalTextures[(int)Decal.LaserSwitch] = Content.Load<Texture2D>("decal_laserswitch");
            decalTextures[(int)Decal.Missile] = Content.Load<Texture2D>("decal_missile");
            decalTextures[(int)Decal.Onion] = Content.Load<Texture2D>("decal_onion");
            decalTextures[(int)Decal.BlueCodes] = Content.Load<Texture2D>("decal_permanantbluekey");
            decalTextures[(int)Decal.PermanantBoots] = Content.Load<Texture2D>("decal_permanantboots");
            decalTextures[(int)Decal.RedCodes] = Content.Load<Texture2D>("decal_permanantredkey");
            decalTextures[(int)Decal.PermanantWallJump] = Content.Load<Texture2D>("decal_permanantwalljump");
            decalTextures[(int)Decal.YellowCodes] = Content.Load<Texture2D>("decal_permanantyellowkey");
            decalTextures[(int)Decal.Phase] = Content.Load<Texture2D>("decal_phase");
            decalTextures[(int)Decal.Blaster] = Content.Load<Texture2D>("decal_plasma");
            decalTextures[(int)Decal.PlugSlot] = Content.Load<Texture2D>("decal_plugslot");
            decalTextures[(int)Decal.PowerOrb] = Content.Load<Texture2D>("decal_powerorb");
            decalTextures[(int)Decal.PowerPlug] = Content.Load<Texture2D>("decal_powerplug");
            decalTextures[(int)Decal.RainDrop] = Content.Load<Texture2D>("decal_raindrop");
            decalTextures[(int)Decal.RedKey] = Content.Load<Texture2D>("decal_redkey");
            decalTextures[(int)Decal.RedLock] = Content.Load<Texture2D>("decal_redlock");
            decalTextures[(int)Decal.Station] = Content.Load<Texture2D>("decal_station");
            decalTextures[(int)Decal.WallJump] = Content.Load<Texture2D>("decal_walljump");
            decalTextures[(int)Decal.Warp] = Content.Load<Texture2D>("decal_warp");
            decalTextures[(int)Decal.YellowKey] = Content.Load<Texture2D>("decal_yellowkey");
            decalTextures[(int)Decal.YellowLock] = Content.Load<Texture2D>("decal_yellowlock");
            decalTextures[(int)Decal.Lander] = Content.Load<Texture2D>("decal_lander");
            decalTextures[(int)Decal.Skull] = Content.Load<Texture2D>("decal_skull");
            decalTextures[(int)Decal.Switch] = Content.Load<Texture2D>("decal_switch");
            decalTextures[(int)Decal.Empty] = Content.Load<Texture2D>("decal_empty");    
        }

        public static void InitBeamTextures(ContentManager Content)
        {
            flame_beam_textures = new List<Texture2D>();
            electric_beam_textures = new List<Texture2D>();
            flame_beam_textures.Add(Content.Load<Texture2D>("beam_flame_0"));
            flame_beam_textures.Add(Content.Load<Texture2D>("beam_flame_1"));
            electric_beam_textures.Add(Content.Load<Texture2D>("beam_electric_0"));
            electric_beam_textures.Add(Content.Load<Texture2D>("beam_electric_1"));
            beamTexCoords = new List<Vector2>();
            beamTexCoords.Add(new Vector2(.75f, 0));
            beamTexCoords.Add(new Vector2(.25f, 0));
            beamTexCoords.Add(new Vector2(.25f, 1));
            beamTexCoords.Add(new Vector2(.75f, 1));

            

        }

        public Doodad(Doodad d)
        {
            srcDoodad = this;
            style = d.style;
            //unfoldedPosition = new Vertex(d.unfoldedPosition);
            position = new Vertex(d.position);
            spawnPosition = new Vertex(d.spawnPosition);
            active = d.active;

            available = d.available;
            id = d.id;
            idle = d.idle;
            targetBehavior = d.targetBehavior;
            targetObject = d.targetObject;
            expectedBehavior = d.expectedBehavior;
            alreadyUsed = d.alreadyUsed;
            type = d.type;
            behaviors = d.behaviors;
            currentBehaviorId = d.currentBehaviorId;
            if(d.currentBehavior != null)
                currentBehaviorId = d.currentBehavior.id;
            orbsRemaining = d.orbsRemaining;
            doorDecal = d.doorDecal;
            currentTime = d.currentTime;
            nextBehavior = d.nextBehavior;
            behaviorStarted = d.behaviorStarted;
            toggleOn = d.toggleOn;
            abilityType = d.abilityType;
            originalAbilityType = d.originalAbilityType;
            cooldown = d.cooldown;
            activationCost = d.activationCost;

            targetDoodadId = d.targetDoodadId;
            if(d.targetDoodad != null)
                targetDoodadId = d.targetDoodad.id;
            targetBlockId = d.targetBlockId;
            if (d.targetBlock != null)
                targetBlockId = d.targetBlock.id;
            targetEdgeId = d.targetEdgeId;
            if (d.targetEdge != null)
                targetEdgeId = d.targetEdge.id;

            targetRoomId = d.targetRoomId;
            if(d.targetRoom != null)
                targetRoomId = d.targetRoom.id;
            stateTransition = d.stateTransition;
            stateTransitionDir = d.stateTransitionDir;
            stateTransitionVelocity = d.stateTransitionVelocity;
            behaviors = new List<Behavior>();
            foreach (Behavior b in d.behaviors)
            {
                behaviors.Add(new Behavior(b));
            }
        }

 

        public Doodad(VL.Doodad xmlDoodad, Vector3 normal)
        {
            srcDoodad = this;
            this.type = xmlDoodad.type;
            this.id = xmlDoodad.IDString;
            this.targetBehavior = xmlDoodad.targetBehavior;
            this.targetObject = xmlDoodad.targetObject;
            this.expectedBehavior = xmlDoodad.expectBehavior;
            this.activationCost = xmlDoodad.activationCost;

            this.abilityType = (AbilityType)xmlDoodad.ability;
            this.originalAbilityType = (AbilityType)xmlDoodad.ability;
            this.position = new Vertex(xmlDoodad.position, normal, Vector3.Zero, xmlDoodad.up);
            this.spawnPosition = new Vertex(xmlDoodad.position, normal, Vector3.Zero, xmlDoodad.up);
            behaviors = new List<Behavior>();
            currentBehavior = null;

            if (isOrb)
                active = true;
            if (type == VL.DoodadType.BluePowerStation)
                orbsRemaining = 1;
            if (type == VL.DoodadType.RedPowerStation)
                orbsRemaining = 1;

            if (type == VL.DoodadType.WallSwitch)
                stateTransition = 0;
            if (isStation)
                stateTransition = 0;
        }

        public Doodad(VL.DoodadType type, Vector3 position, Vector3 normal, Vector3 direction)
        {
            id = "d_"+type.ToString();
            srcDoodad = this;
            this.type = type;
            this.position = new Vertex(position, normal, Vector3.Zero, direction);
            this.spawnPosition = new Vertex(position, normal, Vector3.Zero, direction);
            behaviors = new List<Behavior>();
            currentBehavior = null;
            if (type == VL.DoodadType.LeftDoor || type == VL.DoodadType.RightDoor)
                stateTransition = 0;

            if (isOrb)
                active = true;
        }

        public void Load(Dd d)
        {
            stateTransition = d.st;
            orbsRemaining = d.or;
            alreadyUsed = d.au;
            currentBehaviorId = d.cbi;
            toggleOn = d.to;
            active = d.ac;
            currentTime = d.ct;
            behaviorStarted = d.bs;
            idle = d.i;

        }

        public Doodad()
        {
        }

        public Doodad(Doodad d, Room r, Vector3 n, Vector3 u)
        {
            position = d.position.Unfold(r, n, u);
            type = d.type;
            toggleOn = d.toggleOn;
            targetDoodad = d.targetDoodad;
            active = d.active;
            srcDoodad = d;
            stateTransition = d.stateTransition;
        }

        public void UpdateUnfoldedDoodad(Room r, Vector3 n, Vector3 u)
        {
            unfoldedPosition = position.Unfold(r, n, u);
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

        public bool ActivationRange(Player p)
        {
            if ((p.center.position - position.position).Length() < triggerDistance)
            {
                if (isStation)
                {
                    if (p.center.direction != position.direction)
                        return false;
                }
                return true;
            }
            else
                return false;
        }

        public void ActivateDoodad(Room currentRoom, bool state)
        {
            bool futureState = state;

            if (powered == false)
                futureState = false;

            if (type == VL.DoodadType.UpgradeStation && abilityType == AbilityType.Empty)
                futureState = false;

            if (futureState != active)
            {
                refreshVertexData = true;
            }

            if (isStation == true || type == VL.DoodadType.Vortex)
            {
                if (active == true && futureState == false)
                {                
                    if(Engine.player.state != State.Tunnel)
                        SoundFX.CloseDoor();
                }
                if (active == false && futureState == true)
                {
                    if(Engine.player.state != State.Jump && Engine.player.state != State.Upgrade && Engine.player.state != State.Tunnel && Engine.player.state != State.Save && Engine.player.state != State.Dialog)
                        SoundFX.OpenDoor();
                }
            }

            active = futureState;
        }

        public bool Animated
        {
            get
            {
                if (type == VL.DoodadType.Beam)
                {
                    return true;
                }
                return false;
            }
        }

        public bool _powered = false;

        public bool powered
        {
            get
            {
                if (type == VL.DoodadType.WarpStation)
                {
                    if (currentRoom.parentSector.currentBlueOrbs >= activationCost)
                    {
                        if (_powered == false)
                        {
                            _powered = true;
                            refreshVertexData = true;
                        }
                        return true;
                    }
                }
                else
                {
                    if (currentRoom.currentOrbs >= activationCost)
                    {
                        if (_powered == false && activationCost > 0)
                        {
                            _powered = true;
                            refreshVertexData = true;
                        }
                        return true;
                    }
                }
                return false;
            }
        }

        public bool isPowerStation
        {
            get
            {
                if (type == VL.DoodadType.BluePowerStation || type == VL.DoodadType.RedPowerStation || type == VL.DoodadType.PowerStation)
                    return true;
                return false;
            }
        }

        public bool isOrb
        {
            get
            {
                if (idle == false && (type == VL.DoodadType.PowerOrb || type == VL.DoodadType.RedCube || type == VL.DoodadType.BlueCube))
                    return true;
                return false;
            }
        }

        public bool dynamic
        {
            get
            {
                if (stateTransition != 0 && stateTransition != 1)
                    return true;
                if (type == VL.DoodadType.Crate || type== VL.DoodadType.PowerPlug)                
                    return true;                
                return false;
            }
        }

        public Color iconColor
        {
            get
            {
                if (type == VL.DoodadType.StationIcon)
                {
                    if (targetDoodad.type == VL.DoodadType.PowerStation)
                        return Color.Yellow;
                    if (targetDoodad.type == VL.DoodadType.RedPowerStation)
                        return Color.Red;
                    if (targetDoodad.type == VL.DoodadType.BluePowerStation)
                        return Color.Blue;
                    
                    return Color.White;
                }
                return Color.White;
            }
        }

        public Texture2D currentDecalTexture
        {
            get
            {
                if (isStation)
                {
                    return decalTextures[(int)Decal.Station];
                }
                if (type == VL.DoodadType.HookTarget)
                {
                    return decalTextures[(int)Decal.HookTarget];
                }
                if (type == VL.DoodadType.PlugSlot)
                {
                    return decalTextures[(int)Decal.PlugSlot];
                }
                if (type == VL.DoodadType.PowerPlug)
                {
                    return decalTextures[(int)Decal.PowerPlug];
                }
                if (type == VL.DoodadType.LaserSwitch)
                {
                    return decalTextures[(int)Decal.LaserSwitch];
                }
                if (type == VL.DoodadType.JumpPad)
                {
                    return decalTextures[(int)Decal.JumpPad];
                }
                if (type == VL.DoodadType.StationIcon)
                {
                    if (targetDoodad.type == VL.DoodadType.JumpStation)
                    {
                        return decalTextures[(int)targetDoodad.doorDecal];
                    }
                    if (targetDoodad.type == VL.DoodadType.LoadStation || targetDoodad.type == VL.DoodadType.SaveStation)
                        return decalTextures[(int)Decal.Save];
                    if (targetDoodad.type == VL.DoodadType.SwitchStation)
                        return decalTextures[(int)Decal.Switch];
                    if (targetDoodad.type == VL.DoodadType.HealthStation)
                        return decalTextures[(int)Decal.Health];
                    if (targetDoodad.type == VL.DoodadType.PowerStation || targetDoodad.type == VL.DoodadType.RedPowerStation || targetDoodad.type == VL.DoodadType.BluePowerStation)
                        return decalTextures[(int)Decal.PowerOrb];
                    if (targetDoodad.type == VL.DoodadType.WarpStation)
                        return decalTextures[(int)Decal.Warp];
                    if (targetDoodad.type == VL.DoodadType.SwitchStation)
                    {
                        if (targetDoodad.abilityType == AbilityType.RedKey)
                            return decalTextures[(int)Decal.RedLock];
                        if (targetDoodad.abilityType == AbilityType.BlueKey)
                            return decalTextures[(int)Decal.BlueLock];
                        if (targetDoodad.abilityType == AbilityType.YellowKey)
                            return decalTextures[(int)Decal.YellowLock];
                    }
                    if (targetDoodad.type == VL.DoodadType.UpgradeStation || targetDoodad.type == VL.DoodadType.ItemStation)
                    {
                        if (targetDoodad.abilityType == AbilityType.RedKey)
                            return decalTextures[(int)Decal.RedKey];
                        if (targetDoodad.abilityType == AbilityType.BlueKey)
                            return decalTextures[(int)Decal.BlueKey];
                        if (targetDoodad.abilityType == AbilityType.YellowKey)
                            return decalTextures[(int)Decal.YellowKey];
                        if (targetDoodad.abilityType == AbilityType.Laser)
                            return decalTextures[(int)Decal.Laser];
                        if (targetDoodad.abilityType == AbilityType.Blaster)
                            return decalTextures[(int)Decal.Blaster];
                        if (targetDoodad.abilityType == AbilityType.Boots)
                            return decalTextures[(int)Decal.Boots];
                        if (targetDoodad.abilityType == AbilityType.JetPack)
                            return decalTextures[(int)Decal.JetPack];
                        if (targetDoodad.abilityType == AbilityType.Phase)
                            return decalTextures[(int)Decal.Phase];
                        if (targetDoodad.abilityType == AbilityType.Booster)
                            return decalTextures[(int)Decal.Booster];
                        if (targetDoodad.abilityType == AbilityType.DoubleJump)
                            return decalTextures[(int)Decal.DoubleJump];
                        if (targetDoodad.abilityType == AbilityType.WallJump)
                            return decalTextures[(int)Decal.WallJump];
                        if (targetDoodad.abilityType == AbilityType.Missile)
                            return decalTextures[(int)Decal.Missile];
                        if (targetDoodad.abilityType == AbilityType.PermanentBoots)
                            return decalTextures[(int)Decal.PermanantBoots];
                        if (targetDoodad.abilityType == AbilityType.PermanentWallJump)
                            return decalTextures[(int)Decal.PermanantWallJump];
                        if (targetDoodad.abilityType == AbilityType.ImprovedJump)
                            return decalTextures[(int)Decal.ImprovedJump];
                        if (targetDoodad.abilityType == AbilityType.PermanentRedKey)
                            return decalTextures[(int)Decal.RedCodes];
                        if (targetDoodad.abilityType == AbilityType.PermanentBlueKey)
                            return decalTextures[(int)Decal.BlueCodes];
                        if (targetDoodad.abilityType == AbilityType.PermanentYellowKey)
                            return decalTextures[(int)Decal.YellowCodes];

                    }                    
                }
                return decalTextures[(int)Decal.Empty];
            }
        }
        
        public Color baseColor
        {
            get
            {

                if (type == VL.DoodadType.StationIcon)
                {

                    if(targetDoodad.powered == false)
                        return new Color(50,50, 50);
                    if (targetDoodad.type == VL.DoodadType.UpgradeStation)
                    {
                        return Color.DarkGoldenrod;
                    }
                    if (targetDoodad.type == VL.DoodadType.HealthStation)
                    {
                        return Color.White;
                    }
                    if (targetDoodad.type == VL.DoodadType.JumpStation && targetDoodad.alreadyUsed == false)
                    {
                        float x = (1f*(maxFlashTime - flashTime))/(1f*maxFlashTime);
                        float y = 4*(.25f-(x - .5f) * (x - .5f));

                        Color flashColor = Color.White;
                        flashColor.R = (Byte)((y * Color.Blue.R + (1 - y) * Color.LightGreen.R));
                        flashColor.G = (Byte)((y * Color.Blue.G + (1 - y) * Color.LightGreen.G));
                        flashColor.B = (Byte)((y * Color.Blue.B + (1 - y) * Color.LightGreen.B));
                        refreshVertexData = true;
                        return flashColor;                        
                    }
                    if (available == false)
                    {
                        if (targetDoodad.type == VL.DoodadType.ItemStation)
                            return new Color(170, 40, 40);
                        else
                            return new Color(80, 80, 80);
                    }
                    return new Color(80, 130, 80);
                }

                if (type == VL.DoodadType.RightDoor || type == VL.DoodadType.LeftDoor)
                {
                    if (targetDoodad.type == VL.DoodadType.UpgradeStation)
                        return Color.DarkGoldenrod;
                    return Color.Gray;
                }
                if (type == VL.DoodadType.RightTunnelDoor || type == VL.DoodadType.LeftTunnelDoor)
                {
                    if (powered == true)
                        return Color.Gray;
                    else
                        return new Color(100,100,100);
                }
                if (type == VL.DoodadType.JumpStation)
                    return new Color(40, 40, 40);
                if (type == VL.DoodadType.UpgradeStation)
                    return Color.Gray;
                if (type == VL.DoodadType.PowerStation)
                    return Color.Yellow;
                if (type == VL.DoodadType.HookTarget || type == VL.DoodadType.LaserSwitch || type == VL.DoodadType.PowerPlug || type == VL.DoodadType.PlugSlot)
                    return new Color(100, 100, 100);
                if (type == VL.DoodadType.JumpPad)
                {
                    if (powered == false)
                        return new Color(80,80,80);
                    else
                        return new Color(160, 100, 100);
                }
                if (type == VL.DoodadType.BridgeGate)
                    return Color.LightBlue;
                if (type == VL.DoodadType.BridgeSide || type == VL.DoodadType.BridgeCover || type == VL.DoodadType.BridgeBack)
                    return Color.Gray;
                if (type == VL.DoodadType.PowerOrb)
                    return Color.Gold;
                if (type == VL.DoodadType.BlueCube)
                    return Color.Blue;
                if (type == VL.DoodadType.RedCube)
                    return Color.Red;
                if (type == VL.DoodadType.Brick)
                    return Color.Brown;
                if (type == VL.DoodadType.Door)
                    return Color.Gray;
                if (type == VL.DoodadType.Beam)
                    return Color.LightBlue;
                if (type == VL.DoodadType.Checkpoint)
                    return Color.DarkBlue;
                else
                    return Color.LightGray;
            }
        }

        public Color activeColor
        {
            get
            {
                if (type == VL.DoodadType.PowerPlug)
                    return Color.Yellow;
                if (type == VL.DoodadType.LaserSwitch)
                    return Color.Blue;
                if (type == VL.DoodadType.StationIcon)
                {
                    if (targetDoodad.powered == false)
                        return new Color(50, 50, 50);
                    if (targetDoodad.type == VL.DoodadType.UpgradeStation)
                        return Color.DarkGoldenrod;                              
                    if (available == false)
                    {
                        if (targetDoodad.type == VL.DoodadType.ItemStation)
                            return new Color(190, 60, 60);
                        else
                            return new Color(80, 80, 80);
                    }
                    return new Color(50, 200, 50);
                }
                if (type == VL.DoodadType.RightDoor || type == VL.DoodadType.LeftDoor)
                    return Color.Gray;


                if (isPowerStation)
                    return Color.DarkGray;
                if (type == VL.DoodadType.HookTarget)
                    return new Color(100, 100, 100);
                if (type == VL.DoodadType.JumpStation)
                    return new Color(40, 40, 40);
                if (type == VL.DoodadType.JumpPad)
                {
                    if (powered == false)
                        return new Color(80, 80, 80);
                    else
                        return new Color(210,150,150);
                }
                if (type == VL.DoodadType.BridgeSide || type == VL.DoodadType.BridgeCover || type == VL.DoodadType.BridgeBack)
                    return Color.Gray;
                if (type == VL.DoodadType.BridgeGate)
                    return Color.LightBlue;
                if (type == VL.DoodadType.Checkpoint || type == VL.DoodadType.PowerOrb)
                    return Color.Yellow;
                if (type == VL.DoodadType.BlueCube)
                    return Color.Blue;
                if (type == VL.DoodadType.RedCube)
                    return Color.Red;
                else
                    return Color.DarkGray;
            }
        }

        public float triggerDistance
        {
            get
            {
                if (type == VL.DoodadType.TriggerPoint)
                    return .75f;
                if (type == VL.DoodadType.Vortex)
                    return 1.3f;
                if (type == VL.DoodadType.NPC_OldMan)
                    return 1f;
                if (type == VL.DoodadType.Checkpoint)
                    return 2f;
                if (type == VL.DoodadType.WarpStation)
                    return .5f;
                return .5f;
            }
        }


        public int styleSpriteIndex
        {
            get
            {
                if (type == VL.DoodadType.Beam && style == RoomStyle.Electric)
                    return 0;
                if (type == VL.DoodadType.Beam && style == RoomStyle.Flame)
                    return 8;
                
                return 0;
            }
        }

        public bool hasCollisionRect
        {
            get
            {
                if (type == VL.DoodadType.TriggerPoint)
                    return false;
                if (type == VL.DoodadType.PlugSlot)
                    return false;
                if (type == VL.DoodadType.Beam || type == VL.DoodadType.LaserSwitch)
                    return false;
                if (type == VL.DoodadType.NPC_OldMan || type == VL.DoodadType.HookTarget)
                    return false;
                if (type == VL.DoodadType.RightTunnelDoor || type == VL.DoodadType.LeftTunnelDoor || type == VL.DoodadType.RightDoor || type == VL.DoodadType.LeftDoor || type == VL.DoodadType.StationIcon || type == VL.DoodadType.TunnelTop || type == VL.DoodadType.TunnelSide || type == VL.DoodadType.RingSide || type == VL.DoodadType.RingTop)
                    return false;
                if (type == VL.DoodadType.BridgeCover || type == VL.DoodadType.BridgeGate || type == VL.DoodadType.JumpPad || isStation)
                    return false;
                if (type == VL.DoodadType.Brick && active == true)
                    return false;
                if (type == VL.DoodadType.Vortex || type == VL.DoodadType.Waypoint || type == VL.DoodadType.WallSwitch || type == VL.DoodadType.Checkpoint || isOrb || type == VL.DoodadType.ItemBlock)
                    return false;
                return true;
            }
        }

        public bool shouldRender
        {
            get
            {
                if (idle == true)
                    return false;
                if (type == VL.DoodadType.TriggerPoint)
                    return false;
                if (type == VL.DoodadType.Vortex)
                    return false;
                if (type == VL.DoodadType.BridgeSide || type == VL.DoodadType.BridgeBack)
                    return false;
                if ((type == VL.DoodadType.Door || type == VL.DoodadType.Beam) && stateTransition == 0)
                    return false;
                if (type == VL.DoodadType.Brick && active == true)
                    return false;
                if (type == VL.DoodadType.Waypoint)
                    return false;
                if(isOrb && active == false)
                    return false;
                return true;
            }
        }

        public bool freeMotion
        {
            get
            {
                if (type == VL.DoodadType.PowerPlug)
                    return true;
                if (type == VL.DoodadType.Crate || type == VL.DoodadType.SpikeBall)
                    return true;
                return false;
            }
        }

        public Vector3 upUnit
        {
            get
            {
                return position.direction;
            }
        }
        public Vector3 rightUnit
        {
            get
            {
                return Vector3.Cross(position.direction, position.normal);
            }
        }

        public Vector3 unfoldedUpUnit
        {
            get
            {
                return unfoldedPosition.direction;
            }
        }
        public Vector3 unfoldedRightUnit
        {
            get
            {
                return Vector3.Cross(unfoldedPosition.direction, unfoldedPosition.normal);
            }
        }

        public float right_mag
        {
            get
            {
                if (type == VL.DoodadType.LeftDoor)
                    return halfWidth - .5f * stateTransition;
                if (type == VL.DoodadType.RightDoor)
                    return halfWidth + .5f * stateTransition;

                if (type == VL.DoodadType.RightTunnelDoor)
                    return halfWidth - 2 * halfWidth * stateTransition;
                

                return halfWidth;
            }
        }
        public float left_mag
        {
            get
            {
                if (type == VL.DoodadType.LeftDoor)
                    return -halfWidth - .5f * stateTransition;
                if (type == VL.DoodadType.RightDoor)
                    return -halfWidth + .5f * stateTransition;

                if (type == VL.DoodadType.LeftTunnelDoor)
                    return -halfWidth + 2*halfWidth * stateTransition;
                
                return -halfWidth;
            }
        }
        public float up_mag
        {
            get
            {
                if (type == VL.DoodadType.Door)
                {
                    return -.5f + stateTransition * 3f;
                }
                if(type == VL.DoodadType.Beam)
                {
                    return -.3f + stateTransition * 2.7f;
                }
                if (type == VL.DoodadType.WallSwitch)
                {
                    return -.3f - stateTransition * .2f;
                }
                return halfHeight;
            }
        }
        public float down_mag
        {
            get
            {
                if (type == VL.DoodadType.Beam)
                {
                    return -.3f;
                }
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
                if (type == VL.DoodadType.LeftTunnelDoor || type == VL.DoodadType.RightTunnelDoor)
                    return .35f;
                if (type == VL.DoodadType.TunnelSide || type == VL.DoodadType.RingSide)
                    return .1f;
                if (type == VL.DoodadType.TunnelTop || type == VL.DoodadType.RingTop) 
                    return .7f;
                if (type == VL.DoodadType.LeftDoor || type == VL.DoodadType.RightDoor)
                    return .3f;
                if (type == VL.DoodadType.StationIcon)
                    return .4f;
                if (type == VL.DoodadType.BridgeGate)
                    return 1f;
                if (type == VL.DoodadType.BridgeSide)
                    return .25f;
                if (type == VL.DoodadType.BridgeBack)
                    return 1.5f;
                if (type == VL.DoodadType.BridgeCover)
                    return 1.5f;
                if (isOrb)
                    return .1f;
                if (type == VL.DoodadType.Door)
                    return .2f;
                if (type == VL.DoodadType.Beam)
                    return .4f;
                if (type == VL.DoodadType.WallSwitch)
                    return .25f;
                return .5f;
            }
        }
        public float halfHeight
        {
            get
            {
                if (type == VL.DoodadType.LeftTunnelDoor || type == VL.DoodadType.RightTunnelDoor)
                    return .7f;
                if (type == VL.DoodadType.TunnelTop ||type == VL.DoodadType.RingTop)
                    return .1f;
                if (type == VL.DoodadType.TunnelSide || type == VL.DoodadType.RingSide)
                    return .8f;
                if (type == VL.DoodadType.LeftDoor || type == VL.DoodadType.RightDoor)
                    return .6f;
                if (type == VL.DoodadType.StationIcon)
                    return .4f;
                if (type == VL.DoodadType.BridgeGate)
                    return .25f;
                if (type == VL.DoodadType.BridgeCover)
                    return 1f;
                if (type == VL.DoodadType.BridgeBack)                
                    return .25f;
                if (type == VL.DoodadType.BridgeSide)
                    return .75f;                
                if (isOrb)
                    return .1f;
                return .5f;
            }
        }
        public float depth
        {
            get
            {
                if (type == VL.DoodadType.RightTunnelDoor || type == VL.DoodadType.LeftTunnelDoor)
                    return .02f;
                if (type == VL.DoodadType.TunnelTop || type == VL.DoodadType.TunnelSide || type == VL.DoodadType.RingSide || type == VL.DoodadType.RingTop)
                    return .1f;
                if (type == VL.DoodadType.NPC_OldMan)
                    return 0f;
                if (type == VL.DoodadType.LeftDoor || type == VL.DoodadType.RightDoor)
                    return .15f;
                if (type == VL.DoodadType.StationIcon)
                    return .1f;
                if (type == VL.DoodadType.WallSwitch)
                    return .25f;
                if (type == VL.DoodadType.Checkpoint || type == VL.DoodadType.PlugSlot || type == VL.DoodadType.LaserSwitch || type == VL.DoodadType.ItemBlock || isOrb || type == VL.DoodadType.HookTarget || type == VL.DoodadType.JumpPad || isStation)
                    return .1f;
                if (type == VL.DoodadType.PowerPlug)
                    return .11f;
                if (type == VL.DoodadType.Beam)
                    return .25f;
                return .5f;
            }
        }

        public bool isStation
        {
            get
            {
                return type == VL.DoodadType.HealthStation || type == VL.DoodadType.JumpStation || type == VL.DoodadType.ItemStation || type == VL.DoodadType.WarpStation || type == VL.DoodadType.SwitchStation || type == VL.DoodadType.UpgradeStation || type == VL.DoodadType.PowerStation || type == VL.DoodadType.RedPowerStation || type == VL.DoodadType.BluePowerStation || type == VL.DoodadType.SaveStation || type == VL.DoodadType.LoadStation || type == VL.DoodadType.MenuStation;
            }
        }

        public int maxCooldown
        {
            get
            {
                if (type == VL.DoodadType.ItemBlock || type == VL.DoodadType.ItemStation)
                    return 500;
                return 0;
            }
        }

        public void AdjustVertex(Vector3 pos, Vector3 vel, Vector3 normal, Vector3 playerUp)
        {
            Vector3 playerRight = Vector3.Cross(playerUp, normal);
            if (position.normal == normal)
            {
                position.position += pos;
                position.velocity += vel;
            }
            else if (position.normal == -normal)
            {
                float badVelComponent = Vector3.Dot(playerUp, vel);
                float badPosComponent = Vector3.Dot(playerUp, pos);
                position.position += pos - 2* badPosComponent * playerUp;
                position.velocity += vel - 2* badVelComponent * playerUp;
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

        public List<Vector3> GetCollisionRect()
        {
            List<Vector3> doodadVertexList = new List<Vector3>();

            doodadVertexList.Add(unfoldedPosition.position + unfolded_up + unfolded_right);
            doodadVertexList.Add(unfoldedPosition.position + unfolded_up + unfolded_left);
            doodadVertexList.Add(unfoldedPosition.position + unfolded_down + unfolded_left);
            doodadVertexList.Add(unfoldedPosition.position + unfolded_down + unfolded_right);
            
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

        public List<VertexPositionColorNormalTexture> baseTriangleList;
        public List<VertexPositionColorNormalTexture> decalList;
        public List<VertexPositionColorNormalTexture> loadButtonsList;
        public List<VertexPositionColorNormalTexture> useButtonList;
        public List<VertexPositionColorNormalTexture> xEquipButtonList;
        public List<VertexPositionColorNormalTexture> yEquipButtonList;
        public List<VertexPositionColorNormalTexture> confirmButtonsList;
        public List<VertexPositionColorNormalTexture> spriteList;
        public List<VertexPositionColorNormalTexture> beamList;

        public VertexPositionColorNormalTexture[] baseTriangleArray;
        public VertexPositionColorNormalTexture[] decalArray;
        public VertexPositionColorNormalTexture[] spriteArray;
        public VertexPositionColorNormalTexture[] beamArray;

        public void UpdateVertexData(Room currentRoom)
        {
            if (baseTriangleList == null || dynamic == true || refreshVertexData == true)
            {

                Engine.debug_updateDoodadVertexData++;
                refreshVertexData = false;
                baseTriangleList = new List<VertexPositionColorNormalTexture>();
                decalList = new List<VertexPositionColorNormalTexture>();
                confirmButtonsList = new List<VertexPositionColorNormalTexture>();
                loadButtonsList = new List<VertexPositionColorNormalTexture>();
                useButtonList = new List<VertexPositionColorNormalTexture>();
                xEquipButtonList = new List<VertexPositionColorNormalTexture>();
                yEquipButtonList = new List<VertexPositionColorNormalTexture>();
                spriteList = new List<VertexPositionColorNormalTexture>();
                beamList = new List<VertexPositionColorNormalTexture>();

                List<Vertex> vList = new List<Vertex>();
                vList.Add(new Vertex(position, up + right));
                vList.Add(new Vertex(position, up + left));
                vList.Add(new Vertex(position, down + left));
                vList.Add(new Vertex(position, down + right));

                if (helpIconTime != 0)
                {
                    if (type == VL.DoodadType.LoadStation)
                    {
                        float size = 2*((float)(helpIconTime)) / helpIconMaxTime;

                        List<Vertex> XButtonList = new List<Vertex>();
                        XButtonList.Add(new Vertex(position, size * up + size * right + up + 2f * left));
                        XButtonList.Add(new Vertex(position, size * up + size * left + up + 2f * left));
                        XButtonList.Add(new Vertex(position, size * down + size * left + up + 2f * left));
                        XButtonList.Add(new Vertex(position, size * down + size * right + up + 2f * left));
                        List<Vertex> YButtonList = new List<Vertex>();
                        YButtonList.Add(new Vertex(position, size * up + size * right + 3 * up));
                        YButtonList.Add(new Vertex(position, size * up + size * left + 3 * up));
                        YButtonList.Add(new Vertex(position, size * down + size * left + 3 * up));
                        YButtonList.Add(new Vertex(position, size * down + size * right + 3 * up));
                        List<Vertex> BButtonList = new List<Vertex>();
                        BButtonList.Add(new Vertex(position, size * up + size * right + 5f * up + 2f * right));
                        BButtonList.Add(new Vertex(position, size * up + size * left + 5f * up + 2f * right));
                        BButtonList.Add(new Vertex(position, size * down + size * left + 5f * up + 2f * right));
                        BButtonList.Add(new Vertex(position, size * down + size * right + 5f * up + 2f * right));
                        if (Engine.controlType == ControlType.GamePad)
                        {
                            currentRoom.AddBlockFrontToTriangleList(XButtonList, Color.White, .55f, Ability.texCoordList[25], decalList, true);
                            currentRoom.AddBlockFrontToTriangleList(YButtonList, Color.White, .55f, Ability.texCoordList[27], decalList, true);
                        }
                        if (Engine.controlType == ControlType.KeyboardOnly)
                        {
                            currentRoom.AddBlockFrontToTriangleList(XButtonList, Color.Blue, .55f, Ability.texCoordList[49], decalList, true);
                            currentRoom.AddBlockFrontToTriangleList(YButtonList, Color.Yellow, .55f, Ability.texCoordList[51], decalList, true);
                        }
                        if (Engine.controlType == ControlType.MouseAndKeyboard)
                        {
                            XButtonList = new List<Vertex>();
                            XButtonList.Add(new Vertex(position, size * up + size * right + 5f * up + 2f * left));
                            XButtonList.Add(new Vertex(position, size * up + size * left + 5f * up + 2f * left));
                            XButtonList.Add(new Vertex(position, size * down + size * left + 5f * up + 2f * left));
                            XButtonList.Add(new Vertex(position, size * down + size * right + 5f * up + 2f * left));
                            YButtonList = new List<Vertex>();
                            YButtonList.Add(new Vertex(position, size * up + size * right + 5f * up + 2f * right));
                            YButtonList.Add(new Vertex(position, size * up + size * left + 5f * up + 2f * right));
                            YButtonList.Add(new Vertex(position, size * down + size * left + 5f * up + 2f * right));
                            YButtonList.Add(new Vertex(position, size * down + size * right + 5f * up + 2f * right));
                            BButtonList = new List<Vertex>();
                            BButtonList.Add(new Vertex(position, size * up + size * right - 2.5f * up + 2f * right));
                            BButtonList.Add(new Vertex(position, size * up + size * left - 2.5f * up + 2f * left));
                            BButtonList.Add(new Vertex(position, size * down + size * left - 2.5f * up + 2f * left));
                            BButtonList.Add(new Vertex(position, size * down + size * right - 2.5f * up + 2f * right));

                            currentRoom.AddBlockFrontToTriangleList(XButtonList, Color.Blue, .55f, SaveGameText.okTexCoords, loadButtonsList, true);
                            currentRoom.AddBlockFrontToTriangleList(YButtonList, Color.Yellow, .55f, SaveGameText.cancelTexCoords, loadButtonsList, true);
                            if(SaveGameText.expertAvailable && SaveGameText.activeSaveSlot > -1 && SaveGameText.saveSummaryData[SaveGameText.activeSaveSlot].empty)
                                currentRoom.AddBlockFrontToTriangleList(BButtonList, Color.Red, .55f, SaveGameText.extraTexCoords, loadButtonsList, true);
                        }

                    }
                    else if ((isStation == true && type != VL.DoodadType.ItemStation) || type == VL.DoodadType.NPC_OldMan || type == VL.DoodadType.Vortex)
                    {
                        float size = ((float)(helpIconTime)) / helpIconMaxTime;
                        List<Vertex> BButtonList = new List<Vertex>();
                        BButtonList.Add(new Vertex(position, size * up + size * right + up + 2.3f * right));
                        BButtonList.Add(new Vertex(position, size * up + size * left + up + 2.3f * right));
                        BButtonList.Add(new Vertex(position, size * down + size * left + up + 2.3f * right));
                        BButtonList.Add(new Vertex(position, size * down + size * right + up + 2.3f * right));
                        if (Engine.controlType == ControlType.GamePad)
                            currentRoom.AddBlockFrontToTriangleList(BButtonList, Color.White, .55f, Room.plateTexCoords, useButtonList, true);
                        if (Engine.controlType == ControlType.KeyboardOnly)
                            currentRoom.AddBlockFrontToTriangleList(BButtonList, Color.Red, .55f, Room.plateTexCoords, useButtonList, true);
                        if (Engine.controlType == ControlType.MouseAndKeyboard)
                            currentRoom.AddBlockFrontToTriangleList(BButtonList, Color.Red, .55f, Room.plateTexCoords, useButtonList, true);

                    }
                    else if (type == VL.DoodadType.ItemStation)
                    {
                        float size = ((float)(helpIconTime)) / helpIconMaxTime;

                        List<Vertex> XButtonList = new List<Vertex>();
                        XButtonList.Add(new Vertex(position, size * up + size * right + up + 2f * left));
                        XButtonList.Add(new Vertex(position, size * up + size * left + up + 2f * left));
                        XButtonList.Add(new Vertex(position, size * down + size * left + up + 2f * left));
                        XButtonList.Add(new Vertex(position, size * down + size * right + up + 2f * left));
                        List<Vertex> YButtonList = new List<Vertex>();
                        YButtonList.Add(new Vertex(position, size * up + size * right + 3 * up));
                        YButtonList.Add(new Vertex(position, size * up + size * left + 3 * up));
                        YButtonList.Add(new Vertex(position, size * down + size * left + 3 * up));
                        YButtonList.Add(new Vertex(position, size * down + size * right + 3 * up));
                        if (Engine.controlType == ControlType.GamePad)
                        {
                            currentRoom.AddBlockFrontToTriangleList(XButtonList, Color.White, .55f, Room.plateTexCoords, xEquipButtonList, true);
                            currentRoom.AddBlockFrontToTriangleList(YButtonList, Color.White, .55f, Room.plateTexCoords, yEquipButtonList, true);
                        }
                        if (Engine.controlType == ControlType.KeyboardOnly)
                        {
                            currentRoom.AddBlockFrontToTriangleList(XButtonList, Color.Blue, .55f, Room.plateTexCoords, xEquipButtonList, true);
                            currentRoom.AddBlockFrontToTriangleList(YButtonList, Color.Yellow, .55f, Room.plateTexCoords, yEquipButtonList, true);
                        }
                        if (Engine.controlType == ControlType.MouseAndKeyboard)
                        {
                            XButtonList = new List<Vertex>();
                            XButtonList.Add(new Vertex(position, size * up + size * right + 2.5f * up + 1f * left));
                            XButtonList.Add(new Vertex(position, size * up + size * left + 2.5f * up + 1f * left));
                            XButtonList.Add(new Vertex(position, size * down + size * left + 2.5f * up + 1f * left));
                            XButtonList.Add(new Vertex(position, size * down + size * right + 2.5f * up + 1f * left));
                            YButtonList = new List<Vertex>();
                            YButtonList.Add(new Vertex(position, size * up + size * right + 2.5f * up + 1f * right));
                            YButtonList.Add(new Vertex(position, size * up + size * left + 2.5f * up + 1f * right));
                            YButtonList.Add(new Vertex(position, size * down + size * left + 2.5f * up + 1f * right));
                            YButtonList.Add(new Vertex(position, size * down + size * right + 2.5f * up + 1f * right));

                            currentRoom.AddBlockFrontToTriangleList(XButtonList, Color.Blue, .55f, Room.plateTexCoords, xEquipButtonList, true);
                            currentRoom.AddBlockFrontToTriangleList(YButtonList, Color.Yellow, .55f, Room.plateTexCoords, yEquipButtonList, true);
                        }

                    }
                }


                if (type == VL.DoodadType.BridgeCover)
                {
                    currentRoom.AddBlockToTriangleList(vList, baseColor, .5f, -.6f, Room.plateTexCoords, baseTriangleList);
                    currentRoom.AddBlockToTriangleList(vList, baseColor, -.5f, .6f, Room.plateTexCoords, baseTriangleList);
                    currentRoom.BasicAddBlockSidesToTriangleList(vList, baseColor, .5f, -.6f, Room.plateTexCoords, baseTriangleList);
                    currentRoom.BasicAddBlockSidesToTriangleList(vList, baseColor, -.5f, .6f, Room.plateTexCoords, baseTriangleList);
                }
                else if (isPowerStation)
                {
                    if (orbsRemaining > 0)
                    {
                        currentRoom.AddBlockToTriangleList(vList, baseColor, depth, depth, Room.plateTexCoords, baseTriangleList);
                        currentRoom.BasicAddBlockSidesToTriangleList(vList, baseColor, depth, depth, Room.plateTexCoords, baseTriangleList, false);
                    }
                    else
                    {
                        currentRoom.AddBlockToTriangleList(vList, activeColor, depth, depth, Room.plateTexCoords, baseTriangleList);
                        currentRoom.BasicAddBlockSidesToTriangleList(vList, activeColor, depth, depth, Room.plateTexCoords, baseTriangleList, false);
                    }
                }
                else if (type == VL.DoodadType.SwitchStation)
                {
                    if (targetDoodad != null)
                    {
                        if (targetDoodad.currentBehavior.id == targetBehavior)
                        {
                            currentRoom.AddBlockToTriangleList(vList, activeColor, depth, depth, Room.plateTexCoords, baseTriangleList);
                            currentRoom.BasicAddBlockSidesToTriangleList(vList, activeColor, depth, depth, Room.plateTexCoords, baseTriangleList, false);
                        }
                        else
                        {
                            currentRoom.AddBlockToTriangleList(vList, baseColor, depth, depth, Room.plateTexCoords, baseTriangleList);
                            currentRoom.BasicAddBlockSidesToTriangleList(vList, baseColor, depth, depth, Room.plateTexCoords, baseTriangleList, false);
                        }
                    }
                    if (targetBlock != null)
                    {
                        if (targetBlock.currentBehavior.id == targetBehavior)
                        {
                            currentRoom.AddBlockToTriangleList(vList, activeColor, depth, depth, Room.plateTexCoords, baseTriangleList);
                            currentRoom.BasicAddBlockSidesToTriangleList(vList, activeColor, depth, depth, Room.plateTexCoords, baseTriangleList, false);
                        }
                        else
                        {
                            currentRoom.AddBlockToTriangleList(vList, baseColor, depth, depth, Room.plateTexCoords, baseTriangleList);
                            currentRoom.BasicAddBlockSidesToTriangleList(vList, baseColor, depth, depth, Room.plateTexCoords, baseTriangleList, false);
                        }
                    }
                    if (targetEdge != null)
                    {
                        if (targetEdge.currentBehavior.id == targetBehavior)
                        {
                            currentRoom.AddBlockToTriangleList(vList, activeColor, depth, depth, Room.plateTexCoords, baseTriangleList);
                            currentRoom.BasicAddBlockSidesToTriangleList(vList, activeColor, depth, depth, Room.plateTexCoords, baseTriangleList, false);
                        }
                        else
                        {
                            currentRoom.AddBlockToTriangleList(vList, baseColor, depth, depth, Room.plateTexCoords, baseTriangleList);
                            currentRoom.BasicAddBlockSidesToTriangleList(vList, activeColor, depth, depth, Room.plateTexCoords, baseTriangleList, false);
                        }
                    }
                }
                else if (type == VL.DoodadType.TunnelSide || type == VL.DoodadType.TunnelTop)
                {
                    float roomSize = .5f*Math.Abs(Vector3.Dot(currentRoom.size, position.normal));
                    currentRoom.AddBlockToTriangleList(vList, activeColor, depth, roomSize, Room.plateTexCoords, baseTriangleList,false);
                    currentRoom.BasicAddBlockSidesToTriangleList(vList, activeColor, depth, roomSize, Room.plateTexCoords, baseTriangleList, false);

                }
                else if ((type == VL.DoodadType.RingSide || type == VL.DoodadType.RingTop) && targetDoodad.targetRoom != null)
                {                    
                    float nextRoomSize = .5f * Math.Abs(Vector3.Dot(targetDoodad.targetRoom.size, position.normal));
                    float currentRoomSize = .5f * Math.Abs(Vector3.Dot(currentRoom.size, position.normal));
                    float distanceToNextRoom = Math.Abs(Vector3.Dot(position.normal, (targetDoodad.targetRoom.center - currentRoom.center))) - nextRoomSize - currentRoomSize;

                    float ringMod = 3f;
                    for (; ringMod < Math.Min(20f, distanceToNextRoom / 2); ringMod += 4)
                    {
                        currentRoom.AddBlockToTriangleList(vList, currentRoom.currentColor, ringMod,  -ringMod +.2f, Room.plateTexCoords, baseTriangleList,false);
                        currentRoom.BasicAddBlockSidesToTriangleList(vList, currentRoom.currentColor, ringMod, -ringMod + .2f, Room.plateTexCoords, baseTriangleList,false);                        
                    }
                    if (ringMod >= 10f && distanceToNextRoom / 2 - 20 > .2f)
                    {
                        currentRoom.AddBlockToTriangleList(vList, currentRoom.currentColor, 20f, -(distanceToNextRoom / 2), Room.plateTexCoords, baseTriangleList, false);
                        currentRoom.BasicAddBlockSidesToTriangleList(vList, currentRoom.currentColor, 20f, -(distanceToNextRoom / 2), Room.plateTexCoords, baseTriangleList, false);
                    }
                    
                    
                }
                else if (type != VL.DoodadType.NPC_OldMan && type != VL.DoodadType.Beam && type != VL.DoodadType.PowerPlug && type != VL.DoodadType.LaserSwitch)
                {
                    if (active)
                    {
                        currentRoom.BasicAddBlockSidesToTriangleList(vList, activeColor, depth, depth, Room.plateTexCoords, baseTriangleList);
                        currentRoom.AddBlockToTriangleList(vList, activeColor, depth, depth, Room.plateTexCoords, baseTriangleList);
                    }
                    else
                    {
                        currentRoom.BasicAddBlockSidesToTriangleList(vList, baseColor, depth, depth, Room.plateTexCoords, baseTriangleList);
                        currentRoom.AddBlockToTriangleList(vList, baseColor, depth, depth, Room.plateTexCoords, baseTriangleList);
                    }
                }

                if (type == VL.DoodadType.ItemBlock || isStation)
                {
                    currentRoom.AddBlockFrontToTriangleList(vList, Color.White, depth + .01f, Room.plateTexCoords, decalList, true);
                }
                if (type == VL.DoodadType.JumpStation)
                {
                    float jumpVel = (Engine.player.jumpDestination - Engine.player.jumpSource).Length() / (1f * Player.launchMaxTime);
                    float transitionTime = 1f / Math.Abs(stateTransitionVelocity);
                    float maxExtend = jumpVel * transitionTime;

                    if (active == true)
                    {
                        currentRoom.AddBlockToTriangleList(vList, activeColor, depth + maxExtend * stateTransition, depth, Room.plateTexCoords, baseTriangleList);
                        currentRoom.BasicAddBlockSidesToTriangleList(vList, activeColor, depth + maxExtend * stateTransition, depth, Room.plateTexCoords, baseTriangleList);
                        currentRoom.AddBlockFrontToTriangleList(vList, baseColor, depth + maxExtend * stateTransition + .01f, Room.plateTexCoords, decalList, true);
                    }
                    else
                    {
                        currentRoom.AddBlockToTriangleList(vList, baseColor, depth + maxExtend * stateTransition, depth, Room.plateTexCoords, baseTriangleList);
                        currentRoom.BasicAddBlockSidesToTriangleList(vList, baseColor, depth + maxExtend * stateTransition, depth, Room.plateTexCoords, baseTriangleList);
                        currentRoom.AddBlockFrontToTriangleList(vList, baseColor, depth + maxExtend * stateTransition + .01f, Room.plateTexCoords, decalList, true);
                    }
                }
                if (type == VL.DoodadType.JumpPad)
                {
                    float jumpVel = (Engine.player.jumpDestination - Engine.player.jumpSource).Length() / (1f * Player.launchMaxTime);
                    float transitionTime = 1f / Math.Abs(stateTransitionVelocity);
                    float maxExtend = jumpVel * transitionTime;

                    if (active == true)
                    {
                        currentRoom.AddBlockToTriangleList(vList, activeColor, depth + maxExtend * stateTransition, depth, Room.plateTexCoords, baseTriangleList);
                        currentRoom.BasicAddBlockSidesToTriangleList(vList, baseColor, depth + maxExtend * stateTransition, depth, Room.plateTexCoords, baseTriangleList);
                        currentRoom.AddBlockFrontToTriangleList(vList, activeColor, depth + maxExtend * stateTransition + .01f, Room.plateTexCoords, decalList, true);
                    }
                    else
                    {                        
                        currentRoom.AddBlockToTriangleList(vList, baseColor, depth + maxExtend * stateTransition, depth, Room.plateTexCoords, baseTriangleList);
                        currentRoom.BasicAddBlockSidesToTriangleList(vList, baseColor, depth + maxExtend * stateTransition, depth, Room.plateTexCoords, baseTriangleList);
                        currentRoom.AddBlockFrontToTriangleList(vList, baseColor, depth + maxExtend * stateTransition + .01f, Room.plateTexCoords, decalList, true);
                    }
                }
                if (type == VL.DoodadType.HookTarget)
                {
                    currentRoom.AddBlockToTriangleList(vList, activeColor, depth, depth, Room.plateTexCoords, baseTriangleList);
                    currentRoom.BasicAddBlockSidesToTriangleList(vList, baseColor, depth, depth, Room.plateTexCoords, baseTriangleList);
                    currentRoom.AddBlockFrontToTriangleList(vList, activeColor, depth, Room.plateTexCoords, decalList, true);                
                }
                if (type == VL.DoodadType.PlugSlot)
                {
                    currentRoom.AddBlockToTriangleList(vList, baseColor, depth, depth, Room.plateTexCoords, baseTriangleList);
                    currentRoom.BasicAddBlockSidesToTriangleList(vList, baseColor, depth, depth, Room.plateTexCoords, baseTriangleList);
                    currentRoom.AddBlockFrontToTriangleList(vList, baseColor, depth, Room.plateTexCoords, decalList, true);
                }
                if (type == VL.DoodadType.LaserSwitch)
                {
                    currentRoom.AddBlockToTriangleList(vList, baseColor, depth, depth, Room.plateTexCoords, baseTriangleList);
                    if (active)
                        currentRoom.AddBlockFrontToTriangleList(vList, activeColor, depth, Room.plateTexCoords, decalList, true);
                    else
                        currentRoom.AddBlockFrontToTriangleList(vList, baseColor, depth, Room.plateTexCoords, decalList, true);
                }

                if (type == VL.DoodadType.StationIcon)
                {                    
                    currentRoom.AddBlockFrontToTriangleList(vList, iconColor, depth + .01f, Room.plateTexCoords, decalList, true);
                }

                if (type == VL.DoodadType.NPC_OldMan)
                {
                    currentRoom.AddBlockFrontToTriangleList(vList, Color.White, depth + .01f, Ability.texCoordList[36], spriteList, true);
                }
                if (type == VL.DoodadType.Beam)
                {
                    currentRoom.AddBlockFrontToTriangleList(vList, Color.White, depth, beamTexCoords, beamList, true);
                }
                if (type == VL.DoodadType.PowerPlug)
                {
                    if(active == true)
                        currentRoom.AddBlockFrontToTriangleList(vList, activeColor, depth, Room.plateTexCoords, decalList, true);
                    else
                        currentRoom.AddBlockFrontToTriangleList(vList, baseColor, depth, Room.plateTexCoords, decalList, true);
                }

                baseTriangleArray = baseTriangleList.ToArray();
                decalArray = decalList.ToArray();
                spriteArray = spriteList.ToArray();
                beamArray = beamList.ToArray();
                
            }
        }

        public void DrawSolids(Room currentRoom)
        {
            if (shouldRender == true)
            {
                UpdateVertexData(currentRoom);

                if (baseTriangleList.Count > 0)
                {
                    if(type == VL.DoodadType.Brick)
                        Engine.playerTextureEffect.Texture = Block.crackedTexture;
                    else
                        Engine.playerTextureEffect.Texture = Block.wallTexture;
                    Engine.playerTextureEffect.CurrentTechnique.Passes[0].Apply();
                    Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                        baseTriangleArray, 0, baseTriangleList.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                }
                
                if (decalList.Count > 0)
                {
                    Engine.playerTextureEffect.Texture = currentDecalTexture;
                    Engine.playerTextureEffect.CurrentTechnique.Passes[0].Apply();
                    Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                        decalArray, 0, decalList.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                }

            }
        }

        public void DrawSprites(Room currentRoom)
        {
            if (shouldRender == true)
            {
                UpdateVertexData(currentRoom);

                if (spriteList.Count > 0)
                {
                    Engine.playerTextureEffect.Texture = Ability.ability_textures;
                    Engine.playerTextureEffect.CurrentTechnique.Passes[0].Apply();
                    Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                        spriteArray, 0, spriteList.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                }

                if (beamList.Count > 0)
                {
                    
                    if (styleSpriteIndex == 0)
                        Engine.playerTextureEffect.Texture = Doodad.electric_beam_textures[animationFrame];
                    else
                        Engine.playerTextureEffect.Texture = Doodad.flame_beam_textures[animationFrame];
                    Engine.playerTextureEffect.CurrentTechnique.Passes[0].Apply();
                    Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                        beamArray, 0, beamList.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);                    
                }

                if (loadButtonsList.Count > 0)
                {
                    if (SaveGameText.state == LoadState.Normal)
                    {
                        Engine.playerTextureEffect.Texture = SaveGameText.loadOptions;
                    }
                    else
                    {
                        Engine.playerTextureEffect.Texture = SaveGameText.confirmOptions;
                    }
                    Engine.playerTextureEffect.CurrentTechnique.Passes[0].Apply();
                    Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                        loadButtonsList.ToArray(), 0, loadButtonsList.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                }

                if (useButtonList.Count > 0)
                {
                    Engine.playerTextureEffect.Texture = Doodad.useButton;
                    
                    Engine.playerTextureEffect.CurrentTechnique.Passes[0].Apply();
                    Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                        useButtonList.ToArray(), 0, useButtonList.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                }

                if (xEquipButtonList.Count > 0)
                {
                    Engine.playerTextureEffect.Texture = Doodad.leftButton;

                    Engine.playerTextureEffect.CurrentTechnique.Passes[0].Apply();
                    Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                        xEquipButtonList.ToArray(), 0, xEquipButtonList.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                }


                if (yEquipButtonList.Count > 0)
                {
                    Engine.playerTextureEffect.Texture = Doodad.rightButton;

                    Engine.playerTextureEffect.CurrentTechnique.Passes[0].Apply();
                    Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                        yEquipButtonList.ToArray(), 0, yEquipButtonList.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            if (alreadyUsed == false)
            {
                flashTime += gameTime.ElapsedGameTime.Milliseconds;
                if (flashTime > maxFlashTime)
                    flashTime = 0;
            }
            if (type == VL.DoodadType.NPC_OldMan || isStation == true)
            {
                if (helpIconTime != 0 && helpIconTime != helpIconMaxTime)
                    refreshVertexData = true;
                if (!ActivationRange(Engine.player) || Engine.player.state != State.Normal)
                {
                    helpIconTime -= gameTime.ElapsedGameTime.Milliseconds;
                    if (helpIconTime < 0) helpIconTime = 0;
                }
                else
                {
                    helpIconTime += gameTime.ElapsedGameTime.Milliseconds;
                    if (helpIconTime > helpIconMaxTime) helpIconTime = helpIconMaxTime;
                    
                }
            }

            //if (Animated)
            {
                animationTime += gameTime.ElapsedGameTime.Milliseconds;
                if (animationTime > 100)
                {
                    refreshVertexData = true;
                    animationFrame++;
                    animationTime = 0;
                }
            }
            animationFrame %= 2;
            if (isOrb && tracking == true && active == true)
            {
                Vector3 dir = Engine.player.center.position - position.position;
                dir.Normalize();
                position.velocity += .018f * dir;
                position.Update(currentRoom, gameTime.ElapsedGameTime.Milliseconds);
                refreshVertexData = true;
            }
            
            if (type == VL.DoodadType.StationIcon)
            {
                if (targetDoodad.type == VL.DoodadType.ItemStation)
                {
                    if (available == false && Engine.player.upgrades[(int)targetDoodad.abilityType] == true)
                    {
                        available = true;
                        refreshVertexData = true;
                    }
                    if (available == true && Engine.player.upgrades[(int)targetDoodad.abilityType] == false)
                    {
                        available = false;                        
                        refreshVertexData = true;
                    }   
                }
                if (targetDoodad.type == VL.DoodadType.UpgradeStation)
                {
                    if (available == true && targetDoodad.abilityType == AbilityType.Empty)
                    {
                        targetDoodad.available = false;
                        available = false;
                        refreshVertexData = true;
                    }                    
                }
                if (targetDoodad.isPowerStation)
                {
                    if (available == true && targetDoodad.orbsRemaining <= 0)
                    {
                        targetDoodad.available = false;
                        available = false;
                        refreshVertexData = true;
                    }
                }
                if (targetDoodad.type == VL.DoodadType.SwitchStation)
                {
                    bool switchReady = false;
                    if(targetDoodad.targetDoodad != null)
                        switchReady = targetDoodad.expectedBehavior == targetDoodad.targetDoodad.currentBehavior.id;
                    if(targetDoodad.targetBlock != null)
                        switchReady = targetDoodad.expectedBehavior == targetDoodad.targetBlock.currentBehavior.id;
                    if (targetDoodad.targetEdge != null)
                        switchReady = targetDoodad.expectedBehavior == targetDoodad.targetEdge.currentBehavior.id;


                    if (available == true && switchReady == false)
                    {
                        targetDoodad.available = false;
                        available = false;
                        refreshVertexData = true;
                    }
                    if (available == false && switchReady == true)
                    {
                        targetDoodad.available = true;
                        available = true;
                        refreshVertexData = true;
                    }
                }
            }
            if (type == VL.DoodadType.LaserSwitch)
            {
                bool switchReady = false;
                if (targetDoodad != null)
                    switchReady = expectedBehavior == targetDoodad.currentBehavior.id;
                if (targetBlock != null)
                    switchReady = expectedBehavior == targetBlock.currentBehavior.id;
                if (targetEdge != null)
                    switchReady = expectedBehavior == targetEdge.currentBehavior.id;
                if (switchReady == true)
                {
                    ActivateDoodad(currentRoom, false);
                }
            }


            if (type == VL.DoodadType.LeftDoor || type == VL.DoodadType.RightDoor)
            {
                if (targetDoodad.active == true || targetDoodad.available == false)
                    Activate();
                else
                    Deactivate();
            }
            if (type == VL.DoodadType.LeftTunnelDoor || type == VL.DoodadType.RightTunnelDoor)
            {
                if (targetDoodad.active == true || targetDoodad.available == false)
                    Activate();
                else
                    Deactivate();
            }
            if (type == VL.DoodadType.StationIcon)
            {
                if (targetDoodad.available == false && active == false)
                {
                    active = true;
                    refreshVertexData = true;
                }
                else if (targetDoodad.active != active)
                {
                    active = targetDoodad.active;
                    refreshVertexData = true;
                }
                else if (targetDoodad.abilityType != abilityType)
                {
                    abilityType = targetDoodad.abilityType;
                    refreshVertexData = true;
                }
            }

            cooldown -= gameTime.ElapsedGameTime.Milliseconds;
            if (cooldown < 0) cooldown = 0;

            stateTransition += gameTime.ElapsedGameTime.Milliseconds * stateTransitionDir * stateTransitionVelocity;
            if (stateTransitionDir == 1 && stateTransition > 1)
            {
                toggleOn = true;
                stateTransition = 1;
                refreshVertexData = true;
                stateTransitionDir = 0;
            }
            if (stateTransitionDir == -1 && stateTransition < 0)
            {
                toggleOn = false;
                stateTransition = 0;
                refreshVertexData = true;
                stateTransitionDir = 0;
            }

            if (type == VL.DoodadType.JumpPad && stateTransition == 1)
                Deactivate();
            if (type == VL.DoodadType.JumpStation && stateTransition == 1)
                Deactivate();

            if (type == VL.DoodadType.WallSwitch)
            {
                if (stateTransition == 1f && stateTransitionDir == 1)
                {
                    if (targetDoodad != null)
                    {
                        if (targetDoodad.currentBehavior.id == expectedBehavior)
                        {
                            foreach (Behavior b in targetDoodad.behaviors)
                            {
                                if (b.id == targetBehavior)
                                {
                                    targetDoodad.SetBehavior(b);
                                    stateTransitionDir = 0;

                                }
                            }
                        }
                    }
                    if (targetBlock != null)
                    {
                        if (targetBlock.currentBehavior.id == expectedBehavior)
                        {
                            foreach (Behavior b in targetBlock.behaviors)
                            {
                                if (b.id == targetBehavior)
                                {
                                    targetBlock.SetBehavior(b);
                                    stateTransitionDir = 0;

                                }
                            }
                        }
                    }
                    if (targetEdge != null)
                    {
                        if (targetEdge.currentBehavior.id == expectedBehavior)
                        {
                            foreach (Behavior b in targetEdge.behaviors)
                            {
                                if (b.id == targetBehavior)
                                {
                                    targetEdge.SetBehavior(b);
                                    stateTransitionDir = 0;

                                }
                            }
                        }
                    }
                }
                if (targetDoodad != null)
                {
                    if (targetDoodad.currentBehavior.id == expectedBehavior && stateTransition == 1f && stateTransitionDir == 0)
                    {
                        stateTransitionDir = -1;
                    }
                }
                if (targetBlock != null)
                {
                    if (targetBlock.currentBehavior.id == expectedBehavior && stateTransition == 1f && stateTransitionDir == 0)
                    {
                        stateTransitionDir = -1;
                    }
                }
                if (targetEdge != null)
                {
                    if (targetEdge.currentBehavior.id == expectedBehavior && stateTransition == 1f && stateTransitionDir == 0)
                    {
                        stateTransitionDir = -1;
                    }
                }
            }
            
            
        }

        public void Activate()
        {
            if(stateTransition < 1)
            {
                stateTransitionDir = 1;
                refreshVertexData = true;
            }
        }
        public void Deactivate()
        {
            if (stateTransition > 0)
            {
                stateTransitionDir = -1;
                refreshVertexData = true;
            }
        }


        public int UpdateBehavior(GameTime gameTime)
        {
            if (currentBehavior == null)
                return 0;
            if (behaviorStarted == false && currentTime > currentBehavior.offSet)
            {
                //properties.primaryValue = currentBehavior.primaryValue;
                //properties.secondaryValue = currentBehavior.secondaryValue;
                if (currentBehavior.toggle)
                    Activate();
                else
                    Deactivate();
                currentTime = gameTime.ElapsedGameTime.Milliseconds;
                behaviorStarted = true;
                nextBehavior = false;
            }
            if (nextBehavior == true && behaviorStarted == true)
            {
                behaviorStarted = true;
                foreach (Behavior b in behaviors)
                {
                    if (b.id == currentBehavior.nextBehavior)
                    {
                        currentBehavior = b;
                        currentBehaviorId = currentBehavior.id;
                        break;
                    }
                }
                //properties.primaryValue = currentBehavior.primaryValue;
                //properties.secondaryValue = currentBehavior.secondaryValue;
                if (currentBehavior.toggle)
                    Activate();
                else
                    Deactivate();

                currentTime = 0;
                nextBehavior = false;
                return gameTime.ElapsedGameTime.Milliseconds;
            }
            currentTime += gameTime.ElapsedGameTime.Milliseconds;
            if (behaviorStarted)
            {
                if (currentBehavior.duration != 0 && currentTime > currentBehavior.duration)
                {
                    nextBehavior = true;
                    return currentBehavior.duration - (currentTime - gameTime.ElapsedGameTime.Milliseconds);
                }
                if (currentBehavior.period != 0 && currentTime > currentBehavior.period)
                {
                    currentTime = 0;
                    if (toggleOn)
                        Deactivate();
                    else
                        Activate();                   
                }
            }
            return gameTime.ElapsedGameTime.Milliseconds;
        }

        public void SetTargetBehavior()
        {
            if (targetDoodad != null)
            {
                if (targetDoodad.currentBehavior.id == expectedBehavior)
                {
                    foreach (Behavior behavior in targetDoodad.behaviors)
                    {
                        if (behavior.id == targetBehavior)
                        {
                            targetDoodad.SetBehavior(behavior);
                        }
                    }
                }
            }
            if (targetBlock != null)
            {
                if (targetBlock.currentBehavior.id == expectedBehavior)
                {
                    foreach (Behavior behavior in targetBlock.behaviors)
                    {
                        if (behavior.id == targetBehavior)
                        {
                            targetBlock.SetBehavior(behavior);
                        }
                    }
                }
            }
            if (targetEdge != null)
            {
                if (targetEdge.currentBehavior.id == expectedBehavior)
                {
                    foreach (Behavior behavior in targetEdge.behaviors)
                    {
                        if (behavior.id == targetBehavior)
                        {
                            targetEdge.SetBehavior(behavior);
                        }
                    }
                }
            }
        }

        public void SetBehavior(Behavior b)
        {           
            currentBehavior = b;
            currentBehaviorId = currentBehavior.id;
            if (currentBehavior.toggle)
                Activate();
            else
                Deactivate();
            //toggleOn = currentBehavior.toggle;
            currentTime = 0;
            nextBehavior = false;            
        }

        public void UpdateBehavior()
        {            
            if (currentBehavior == null)
            {
                currentBehavior = behaviors[0];
                currentBehaviorId = currentBehavior.id;
                if (currentBehavior.offSet == 0)
                {
                    //properties.primaryValue = currentBehavior.primaryValue;
                    //properties.secondaryValue = currentBehavior.secondaryValue;
                    toggleOn = currentBehavior.toggle;
                    if (toggleOn == true)
                        stateTransition = 1;
                    else
                        stateTransition = 0;

                    behaviorStarted = true;
                }
            }
        }
    }
}

