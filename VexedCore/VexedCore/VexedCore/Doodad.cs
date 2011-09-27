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
   
    public class Doodad
    {
        public Vertex unfoldedPosition;
        public Vertex position;
        public Vertex spawnPosition;
        public bool active = false;
        public bool powered = false;
        public bool ready = false;
        public bool available = true;
        public bool tracking = false;

        public bool refreshVertexData = false;
        public string id = "";
        public string targetBehavior ="";
        public string targetObject = "";
        public string expectedBehavior ="";
        public VexedLib.DoodadType type;
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
        [XmlIgnore]public Doodad targetDoodad = null;
        [XmlIgnore]public Block targetBlock = null;
        [XmlIgnore]public Edge targetEdge = null;
        [XmlIgnore]public Room targetRoom = null;
        public string targetDoodadId;
        public string targetBlockId;
        public string targetEdgeId;
        public string targetRoomId;

        public int animationFrame = 0;
        public int animationTime = 0;

        public int cooldown = 0;
        public int activationCost = 0;

        public AbilityType abilityType = AbilityType.Empty;

        [XmlIgnore]public Doodad srcDoodad = null;

        public float stateTransition = 1;
        public float stateTransitionVelocity = .005f;
        public int stateTransitionDir = 0;

        public static Texture2D beam_textures;
        public static List<List<Vector2>> texCoordList;

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


        public Doodad(Doodad d)
        {
            srcDoodad = this;
            style = d.style;
            //unfoldedPosition = new Vertex(d.unfoldedPosition);
            position = new Vertex(d.position);
            spawnPosition = new Vertex(d.spawnPosition);
            active = d.active;

            powered = d.powered;
            available = d.available;
            id = d.id;
            targetBehavior = d.targetBehavior;
            targetObject = d.targetObject;
            expectedBehavior = d.expectedBehavior;
            type = d.type;
            behaviors = d.behaviors;
            currentBehaviorId = d.currentBehaviorId;
            if(d.currentBehavior != null)
                currentBehaviorId = d.currentBehavior.id;
            orbsRemaining = d.orbsRemaining;
            currentTime = d.currentTime;
            nextBehavior = d.nextBehavior;
            behaviorStarted = d.behaviorStarted;
            toggleOn = d.toggleOn;
            abilityType = d.abilityType;
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

        public Doodad()
        {
        }

        public Doodad(VexedLib.Doodad xmlDoodad, Vector3 normal)
        {
            srcDoodad = this;
            this.type = xmlDoodad.type;
            this.id = xmlDoodad.IDString;
            this.targetBehavior = xmlDoodad.targetBehavior;
            this.targetObject = xmlDoodad.targetObject;
            this.expectedBehavior = xmlDoodad.expectBehavior;
            this.activationCost = xmlDoodad.activationCost;

            this.abilityType = (AbilityType)xmlDoodad.ability;
            this.position = new Vertex(xmlDoodad.position, normal, Vector3.Zero, xmlDoodad.up);
            this.spawnPosition = new Vertex(xmlDoodad.position, normal, Vector3.Zero, xmlDoodad.up);
            behaviors = new List<Behavior>();
            currentBehavior = null;

            if (type == VexedLib.DoodadType.PowerOrb)
                active = true;

            if (type == VexedLib.DoodadType.WallSwitch)
                stateTransition = 0;
            if (isStation)
                stateTransition = 0;
        }

        public Doodad(VexedLib.DoodadType type, Vector3 position, Vector3 normal, Vector3 direction)
        {
            srcDoodad = this;
            this.type = type;
            this.position = new Vertex(position, normal, Vector3.Zero, direction);
            this.spawnPosition = new Vertex(position, normal, Vector3.Zero, direction);
            behaviors = new List<Behavior>();
            currentBehavior = null;
            if (type == VexedLib.DoodadType.LeftDoor || type == VexedLib.DoodadType.RightDoor)
                stateTransition = 0;

            if (type == VexedLib.DoodadType.PowerOrb)
                active = true;
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
                if (type == VexedLib.DoodadType.JumpStation || type == VexedLib.DoodadType.ItemStation || type == VexedLib.DoodadType.SwitchStation || type == VexedLib.DoodadType.WarpStation || type == VexedLib.DoodadType.UpgradeStation || type == VexedLib.DoodadType.PowerStation)
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

            if (currentRoom.currentOrbs < activationCost)
                futureState = false;

            if (futureState != active)
            {
                refreshVertexData = true;
            }

            active = futureState;
        }

        public bool dynamic
        {
            get
            {
                if (stateTransition != 0 && stateTransition != 1)
                    return true;
                if (type == VexedLib.DoodadType.Crate || type== VexedLib.DoodadType.PowerPlug)                
                    return true;                
                return false;
            }
        }
        
        public Color baseColor
        {
            get
            {
                if (type == VexedLib.DoodadType.StationIcon)
                {
                    if(targetDoodad.powered == false)
                        return new Color(50,50, 50);
                    if (available == false)
                    {
                        if (targetDoodad.type == VexedLib.DoodadType.ItemStation)
                            return new Color(170, 40, 40);
                        else
                            return new Color(80, 80, 80);
                    }
                    return new Color(80, 130, 80);
                }
                if (type == VexedLib.DoodadType.RightDoor || type == VexedLib.DoodadType.LeftDoor)
                    return Color.Gray;
                if (type == VexedLib.DoodadType.RightTunnelDoor || type == VexedLib.DoodadType.LeftTunnelDoor)
                {
                    if (powered == true)
                        return Color.Gray;
                    else
                        return new Color(100,100,100);
                }
                if (type == VexedLib.DoodadType.UpgradeStation)
                    return Color.Gray;
                if (type == VexedLib.DoodadType.PowerStation)
                    return Color.Yellow;
                if (type == VexedLib.DoodadType.HookTarget || type == VexedLib.DoodadType.LaserSwitch || type == VexedLib.DoodadType.PowerPlug || type == VexedLib.DoodadType.PlugSlot)
                    return new Color(100, 100, 100);
                if (type == VexedLib.DoodadType.JumpPad)
                {
                    if (powered == false)
                        return new Color(80,80,80);
                    else
                        return new Color(160, 100, 100);
                }
                if (type == VexedLib.DoodadType.BridgeGate)
                    return Color.LightBlue;
                if (type == VexedLib.DoodadType.BridgeSide || type == VexedLib.DoodadType.BridgeCover || type == VexedLib.DoodadType.BridgeBack)
                    return Color.Gray;
                if (type == VexedLib.DoodadType.PowerOrb)
                    return Color.Gold;
                if (type == VexedLib.DoodadType.Brick)
                    return Color.Brown;
                if (type == VexedLib.DoodadType.Door)
                    return Color.Gray;
                if (type == VexedLib.DoodadType.Beam)
                    return Color.LightBlue;
                if (type == VexedLib.DoodadType.Checkpoint)
                    return Color.DarkBlue;
                else
                    return Color.LightGray;
            }
        }

        public Color activeColor
        {
            get
            {
                if (type == VexedLib.DoodadType.PowerPlug)
                    return Color.Yellow;
                if (type == VexedLib.DoodadType.LaserSwitch)
                    return Color.Blue;
                if (type == VexedLib.DoodadType.StationIcon)
                {
                    if (targetDoodad.powered == false)
                        return new Color(50, 50, 50);
                    if (available == false)
                    {
                        if (targetDoodad.type == VexedLib.DoodadType.ItemStation)
                            return new Color(190, 60, 60);
                        else
                            return new Color(80, 80, 80);
                    }
                    return new Color(50, 200, 50);
                }
                if (type == VexedLib.DoodadType.RightDoor || type == VexedLib.DoodadType.LeftDoor)
                    return Color.Gray;

                if (type == VexedLib.DoodadType.UpgradeStation)
                    return Color.DarkGray;
                if (type == VexedLib.DoodadType.PowerStation)
                    return Color.DarkGray;
                if (type == VexedLib.DoodadType.HookTarget)
                    return new Color(100, 100, 100);
                if (type == VexedLib.DoodadType.JumpPad)
                {
                    if (powered == false)
                        return new Color(80, 80, 80);
                    else
                        return new Color(210,150,150);
                }
                if (type == VexedLib.DoodadType.BridgeSide || type == VexedLib.DoodadType.BridgeCover || type == VexedLib.DoodadType.BridgeBack)
                    return Color.Gray;
                if (type == VexedLib.DoodadType.BridgeGate)
                    return Color.LightBlue;
                if (type == VexedLib.DoodadType.Checkpoint || type == VexedLib.DoodadType.PowerOrb)
                    return Color.Yellow;
                else
                    return Color.DarkGray;
            }
        }

        public float triggerDistance
        {
            get
            {
                if (type == VexedLib.DoodadType.TriggerPoint)
                    return .75f;
                if (type == VexedLib.DoodadType.Vortex)
                    return 1.3f;
                if (type == VexedLib.DoodadType.NPC_OldMan)
                    return 1f;
                if (type == VexedLib.DoodadType.Checkpoint)
                    return 2f;
                if (type == VexedLib.DoodadType.WarpStation)
                    return .5f;
                return .5f;
            }
        }


        public int styleSpriteIndex
        {
            get
            {
                if (type == VexedLib.DoodadType.Beam && style == RoomStyle.Electric)
                    return 0;
                if (type == VexedLib.DoodadType.Beam && style == RoomStyle.Flame)
                    return 8;
                
                return 0;
            }
        }

        public bool hasCollisionRect
        {
            get
            {
                if (type == VexedLib.DoodadType.TriggerPoint)
                    return false;
                if (type == VexedLib.DoodadType.PlugSlot)
                    return false;
                if (type == VexedLib.DoodadType.Beam || type == VexedLib.DoodadType.LaserSwitch)
                    return false;
                if (type == VexedLib.DoodadType.NPC_OldMan || type == VexedLib.DoodadType.HookTarget)
                    return false;
                if (type == VexedLib.DoodadType.RightTunnelDoor || type == VexedLib.DoodadType.LeftTunnelDoor || type == VexedLib.DoodadType.RightDoor || type == VexedLib.DoodadType.LeftDoor || type == VexedLib.DoodadType.StationIcon || type == VexedLib.DoodadType.TunnelTop || type == VexedLib.DoodadType.TunnelSide)
                    return false;
                if (type == VexedLib.DoodadType.BridgeCover || type == VexedLib.DoodadType.BridgeGate || type == VexedLib.DoodadType.JumpPad || type == VexedLib.DoodadType.UpgradeStation || type == VexedLib.DoodadType.JumpStation || type == VexedLib.DoodadType.ItemStation || type == VexedLib.DoodadType.PowerStation || type == VexedLib.DoodadType.SaveStation || type == VexedLib.DoodadType.LoadStation || type == VexedLib.DoodadType.MenuStation || type == VexedLib.DoodadType.SwitchStation)
                    return false;
                if (type == VexedLib.DoodadType.Brick && active == true)
                    return false;
                if (type == VexedLib.DoodadType.Vortex || type == VexedLib.DoodadType.Waypoint || type == VexedLib.DoodadType.WallSwitch || type == VexedLib.DoodadType.Checkpoint || type == VexedLib.DoodadType.PowerOrb || type == VexedLib.DoodadType.WarpStation || type == VexedLib.DoodadType.ItemBlock)
                    return false;
                return true;
            }
        }

        public bool shouldRender
        {
            get
            {
                if (type == VexedLib.DoodadType.TriggerPoint)
                    return false;
                if (type == VexedLib.DoodadType.Vortex)
                    return false;
                if ((type == VexedLib.DoodadType.Door || type == VexedLib.DoodadType.Beam) && stateTransition == 0)
                    return false;
                if (type == VexedLib.DoodadType.Brick && active == true)
                    return false;
                if (type == VexedLib.DoodadType.Waypoint)
                    return false;
                if(type == VexedLib.DoodadType.PowerOrb && active == false)
                    return false;
                return true;
            }
        }

        public bool freeMotion
        {
            get
            {
                if (type == VexedLib.DoodadType.PowerPlug)
                    return true;
                if (type == VexedLib.DoodadType.Crate || type == VexedLib.DoodadType.SpikeBall)
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
                if (type == VexedLib.DoodadType.LeftDoor)
                    return halfWidth - .5f * stateTransition;
                if (type == VexedLib.DoodadType.RightDoor)
                    return halfWidth + .5f * stateTransition;

                if (type == VexedLib.DoodadType.RightTunnelDoor)
                    return halfWidth - 2 * halfWidth * stateTransition;
                

                return halfWidth;
            }
        }
        public float left_mag
        {
            get
            {
                if (type == VexedLib.DoodadType.LeftDoor)
                    return -halfWidth - .5f * stateTransition;
                if (type == VexedLib.DoodadType.RightDoor)
                    return -halfWidth + .5f * stateTransition;

                if (type == VexedLib.DoodadType.LeftTunnelDoor)
                    return -halfWidth + 2*halfWidth * stateTransition;
                
                return -halfWidth;
            }
        }
        public float up_mag
        {
            get
            {
                if (type == VexedLib.DoodadType.Door || type == VexedLib.DoodadType.Beam)
                {
                    return -.5f + stateTransition * 3f;
                }
                if (type == VexedLib.DoodadType.WallSwitch)
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
                if (type == VexedLib.DoodadType.LeftTunnelDoor || type == VexedLib.DoodadType.RightTunnelDoor)
                    return .35f;
                if (type == VexedLib.DoodadType.TunnelSide)
                    return .1f;
                if (type == VexedLib.DoodadType.TunnelTop)
                    return .7f;
                if (type == VexedLib.DoodadType.LeftDoor || type == VexedLib.DoodadType.RightDoor)
                    return .3f;
                if (type == VexedLib.DoodadType.StationIcon)
                    return .4f;
                if (type == VexedLib.DoodadType.BridgeGate)
                    return 1f;
                if (type == VexedLib.DoodadType.BridgeSide)
                    return .25f;
                if (type == VexedLib.DoodadType.BridgeBack)
                    return 1.5f;
                if (type == VexedLib.DoodadType.BridgeCover)
                    return 1.5f;
                if (type == VexedLib.DoodadType.Door || type == VexedLib.DoodadType.PowerOrb)
                    return .1f;
                if (type == VexedLib.DoodadType.Beam)
                    return .3f;
                if (type == VexedLib.DoodadType.WallSwitch)
                    return .25f;
                return .5f;
            }
        }
        public float halfHeight
        {
            get
            {
                if (type == VexedLib.DoodadType.LeftTunnelDoor || type == VexedLib.DoodadType.RightTunnelDoor)
                    return .7f;
                if (type == VexedLib.DoodadType.TunnelTop)
                    return .1f;
                if (type == VexedLib.DoodadType.TunnelSide)
                    return .8f;
                if (type == VexedLib.DoodadType.LeftDoor || type == VexedLib.DoodadType.RightDoor)
                    return .6f;
                if (type == VexedLib.DoodadType.StationIcon)
                    return .4f;
                if (type == VexedLib.DoodadType.BridgeGate)
                    return .25f;
                if (type == VexedLib.DoodadType.BridgeCover)
                    return 1f;
                if (type == VexedLib.DoodadType.BridgeBack)                
                    return .25f;
                if (type == VexedLib.DoodadType.BridgeSide)
                    return .75f;                
                if (type == VexedLib.DoodadType.PowerOrb)
                    return .1f;
                return .5f;
            }
        }
        public float depth
        {
            get
            {
                if (type == VexedLib.DoodadType.RightTunnelDoor || type == VexedLib.DoodadType.LeftTunnelDoor)
                    return .02f;
                if (type == VexedLib.DoodadType.TunnelTop || type == VexedLib.DoodadType.TunnelSide)
                    return .1f;
                if (type == VexedLib.DoodadType.NPC_OldMan)
                    return 0f;
                if (type == VexedLib.DoodadType.LeftDoor || type == VexedLib.DoodadType.RightDoor)
                    return .15f;
                if (type == VexedLib.DoodadType.StationIcon)
                    return .1f;
                if (type == VexedLib.DoodadType.WallSwitch)
                    return .25f;
                if (type == VexedLib.DoodadType.Checkpoint || type == VexedLib.DoodadType.PlugSlot || type == VexedLib.DoodadType.LaserSwitch || type == VexedLib.DoodadType.ItemBlock || type == VexedLib.DoodadType.PowerOrb || type == VexedLib.DoodadType.HookTarget || type == VexedLib.DoodadType.WarpStation || type == VexedLib.DoodadType.JumpPad || type == VexedLib.DoodadType.JumpStation || type == VexedLib.DoodadType.ItemStation || type == VexedLib.DoodadType.PowerStation || type == VexedLib.DoodadType.SaveStation || type == VexedLib.DoodadType.LoadStation || type == VexedLib.DoodadType.MenuStation || type == VexedLib.DoodadType.SwitchStation || type == VexedLib.DoodadType.UpgradeStation)
                    return .1f;
                if (type == VexedLib.DoodadType.PowerPlug)
                    return .11f;
                if (type == VexedLib.DoodadType.Beam)
                    return .25f;
                return .5f;
            }
        }

        public bool isStation
        {
            get
            {
                return type == VexedLib.DoodadType.JumpStation || type == VexedLib.DoodadType.ItemStation || type == VexedLib.DoodadType.WarpStation || type == VexedLib.DoodadType.SwitchStation || type == VexedLib.DoodadType.UpgradeStation || type == VexedLib.DoodadType.PowerStation || type == VexedLib.DoodadType.SaveStation || type == VexedLib.DoodadType.LoadStation || type == VexedLib.DoodadType.MenuStation;
            }
        }

        public int maxCooldown
        {
            get
            {
                if (type == VexedLib.DoodadType.ItemBlock || type == VexedLib.DoodadType.ItemStation)
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
        public List<VertexPositionColorNormalTexture> spriteList;
        public List<VertexPositionColorNormalTexture> beamList;

        public void UpdateVertexData(Room currentRoom)
        {
            if (baseTriangleList == null || Engine.staticDoodadsInitialized == false || dynamic == true || refreshVertexData == true)
            {
                refreshVertexData = false;
                baseTriangleList = new List<VertexPositionColorNormalTexture>();
                decalList = new List<VertexPositionColorNormalTexture>();
                spriteList = new List<VertexPositionColorNormalTexture>();
                beamList = new List<VertexPositionColorNormalTexture>();

                List<Vertex> vList = new List<Vertex>();
                vList.Add(new Vertex(position, up + right));
                vList.Add(new Vertex(position, up + left));
                vList.Add(new Vertex(position, down + left));
                vList.Add(new Vertex(position, down + right));

                if (type == VexedLib.DoodadType.BridgeCover)
                {
                    currentRoom.AddBlockToTriangleList(vList, baseColor, .5f, -.6f, Room.plateTexCoords, baseTriangleList);
                    currentRoom.AddBlockToTriangleList(vList, baseColor, -.5f, .6f, Room.plateTexCoords, baseTriangleList);
                }
                else if (type == VexedLib.DoodadType.PowerStation)
                {
                    if (orbsRemaining > 0)
                        currentRoom.AddBlockToTriangleList(vList, baseColor, depth, depth, Room.plateTexCoords, baseTriangleList);
                    else
                        currentRoom.AddBlockToTriangleList(vList, activeColor, depth, depth, Room.plateTexCoords, baseTriangleList);
                }
                else if (type == VexedLib.DoodadType.SwitchStation)
                {
                    if (targetDoodad != null)
                    {
                        if (targetDoodad.currentBehavior.id == targetBehavior)
                            currentRoom.AddBlockToTriangleList(vList, activeColor, depth, depth, Room.plateTexCoords, baseTriangleList);
                        else
                            currentRoom.AddBlockToTriangleList(vList, baseColor, depth, depth, Room.plateTexCoords, baseTriangleList);
                    }
                    if (targetBlock != null)
                    {
                        if (targetBlock.currentBehavior.id == targetBehavior)
                            currentRoom.AddBlockToTriangleList(vList, activeColor, depth, depth, Room.plateTexCoords, baseTriangleList);
                        else
                            currentRoom.AddBlockToTriangleList(vList, baseColor, depth, depth, Room.plateTexCoords, baseTriangleList);
                    }
                    if (targetEdge != null)
                    {
                        if (targetEdge.currentBehavior.id == targetBehavior)
                            currentRoom.AddBlockToTriangleList(vList, activeColor, depth, depth, Room.plateTexCoords, baseTriangleList);
                        else
                            currentRoom.AddBlockToTriangleList(vList, baseColor, depth, depth, Room.plateTexCoords, baseTriangleList);
                    }
                }
                else if (type == VexedLib.DoodadType.TunnelSide || type == VexedLib.DoodadType.TunnelTop)
                {
                    float roomSize = .5f*Math.Abs(Vector3.Dot(currentRoom.size, position.normal));
                    currentRoom.AddBlockToTriangleList(vList, activeColor, depth, roomSize, Room.plateTexCoords, baseTriangleList);
                }
                else if (type != VexedLib.DoodadType.NPC_OldMan && type != VexedLib.DoodadType.Beam && type != VexedLib.DoodadType.PowerPlug)
                {
                    if (active)
                        currentRoom.AddBlockToTriangleList(vList, activeColor, depth, depth, Room.plateTexCoords, baseTriangleList);
                    else
                        currentRoom.AddBlockToTriangleList(vList, baseColor, depth, depth, Room.plateTexCoords, baseTriangleList);
                }

                if (type == VexedLib.DoodadType.ItemStation || type == VexedLib.DoodadType.ItemBlock || type == VexedLib.DoodadType.UpgradeStation ||
                    type == VexedLib.DoodadType.SwitchStation || type == VexedLib.DoodadType.SaveStation || type == VexedLib.DoodadType.LoadStation || type == VexedLib.DoodadType.MenuStation || type == VexedLib.DoodadType.PowerStation || type == VexedLib.DoodadType.WarpStation
                    || type == VexedLib.DoodadType.JumpStation)
                {
                    currentRoom.AddBlockFrontToTriangleList(vList, Color.White, depth + .01f, Ability.texCoordList[34], decalList, true);
                }
                if (type == VexedLib.DoodadType.JumpPad)
                {
                    float jumpVel = (Engine.player.jumpDestination - Engine.player.jumpSource).Length() / (1f * Player.launchMaxTime);
                    float transitionTime = 1f / Math.Abs(stateTransitionVelocity);
                    float maxExtend = jumpVel * transitionTime;

                    if (active == true)
                    {
                        currentRoom.AddBlockToTriangleList(vList, activeColor, depth + maxExtend * stateTransition, depth, Room.plateTexCoords, baseTriangleList);
                        currentRoom.AddBlockFrontToTriangleList(vList, activeColor, depth + maxExtend * stateTransition + .01f, Ability.texCoordList[35], decalList, true);
                    }
                    else
                    {                        
                        currentRoom.AddBlockToTriangleList(vList, baseColor, depth + maxExtend * stateTransition, depth, Room.plateTexCoords, baseTriangleList);
                        currentRoom.AddBlockFrontToTriangleList(vList, baseColor, depth + maxExtend * stateTransition + .01f, Ability.texCoordList[35], decalList, true);
                    }
                }
                if (type == VexedLib.DoodadType.HookTarget)
                {
                    currentRoom.AddBlockToTriangleList(vList, activeColor, depth, depth, Room.plateTexCoords, baseTriangleList);
                    currentRoom.AddBlockFrontToTriangleList(vList, activeColor, depth, Ability.texCoordList[7], decalList, true);                
                }
                if (type == VexedLib.DoodadType.PlugSlot)
                {
                    currentRoom.AddBlockToTriangleList(vList, baseColor, depth, depth, Room.plateTexCoords, baseTriangleList);
                    currentRoom.AddBlockFrontToTriangleList(vList, baseColor, depth, Ability.texCoordList[42], decalList, true);
                }
                if (type == VexedLib.DoodadType.LaserSwitch)
                {
                    currentRoom.AddBlockToTriangleList(vList, baseColor, depth, depth, Room.plateTexCoords, baseTriangleList);
                    if (active)
                        currentRoom.AddBlockFrontToTriangleList(vList, activeColor, depth, Ability.texCoordList[40], decalList, true);
                    else
                        currentRoom.AddBlockFrontToTriangleList(vList, baseColor, depth, Ability.texCoordList[40], decalList, true);
                }

                if (type == VexedLib.DoodadType.StationIcon)
                {
                    int decalIndex = (int)targetDoodad.abilityType;
                    if (targetDoodad.type == VexedLib.DoodadType.PowerStation)
                        decalIndex = 37;
                    if (targetDoodad.type == VexedLib.DoodadType.JumpStation)
                        decalIndex = 38;
                    if (targetDoodad.type == VexedLib.DoodadType.WarpStation)
                        decalIndex = 39;
                    if (targetDoodad.type == VexedLib.DoodadType.SwitchStation && targetDoodad.abilityType != AbilityType.RedKey && targetDoodad.abilityType != AbilityType.BlueKey && targetDoodad.abilityType != AbilityType.YellowKey)
                        decalIndex = 31;
                    currentRoom.AddBlockFrontToTriangleList(vList, Color.White, depth + .01f, Ability.texCoordList[decalIndex], decalList, true);
                }

                if (type == VexedLib.DoodadType.NPC_OldMan)
                {
                    currentRoom.AddBlockFrontToTriangleList(vList, Color.White, depth + .01f, Ability.texCoordList[36], spriteList, true);
                }
                if (type == VexedLib.DoodadType.Beam)
                {
                    currentRoom.AddBlockFrontToTriangleList(vList, Color.White, depth, Doodad.texCoordList[styleSpriteIndex+animationFrame], beamList, true);
                }
                if (type == VexedLib.DoodadType.PowerPlug)
                {
                    if(active == true)
                        currentRoom.AddBlockFrontToTriangleList(vList, activeColor, depth, Ability.texCoordList[41], decalList, true);
                    else
                        currentRoom.AddBlockFrontToTriangleList(vList, baseColor, depth, Ability.texCoordList[41], decalList, true);
                }
            }
        }

        public void Draw(Room currentRoom)
        {
            if (shouldRender == true)
            {
                UpdateVertexData(currentRoom);
                for (int i = 0; i < baseTriangleList.Count; i++)
                {
                    Engine.doodadVertexArray[Engine.doodadVertexArrayCount + i] = baseTriangleList[i];
                }
                Engine.doodadVertexArrayCount += baseTriangleList.Count;                
                for (int i = 0; i < decalList.Count; i++)
                {
                    Engine.decalVertexArray[Engine.decalVertexArrayCount + i] = decalList[i];
                }
                Engine.decalVertexArrayCount += decalList.Count;
                for (int i = 0; i < spriteList.Count; i++)
                {
                    Engine.spriteVertexArray[Engine.spriteVertexArrayCount + i] = spriteList[i];
                }
                Engine.spriteVertexArrayCount += spriteList.Count;
                for (int i = 0; i < beamList.Count; i++)
                {
                    Engine.beamVertexArray[Engine.beamVertexArrayCount + i] = beamList[i];
                }
                Engine.beamVertexArrayCount += beamList.Count;
            }
        }

        public void Update(GameTime gameTime)
        {
            animationTime += gameTime.ElapsedGameTime.Milliseconds;
            if (animationTime > 100)
            {
                refreshVertexData = true;
                animationFrame++;
                animationTime = 0;
            }
            animationFrame %= 2;
            if (type == VexedLib.DoodadType.PowerOrb && tracking == true && active == true)
            {
                Vector3 dir = Engine.player.center.position - position.position;
                dir.Normalize();
                position.velocity += .018f * dir;
                position.Update(currentRoom, gameTime.ElapsedGameTime.Milliseconds);
                refreshVertexData = true;
            }
            if (type == VexedLib.DoodadType.JumpPad && powered == false)
            {
                if (currentRoom.currentOrbs >= activationCost)
                {
                    powered = true;
                    refreshVertexData = true;
                }
            }
            if (targetDoodad != null && powered == false)
            {
                if (currentRoom.currentOrbs >= targetDoodad.activationCost)
                {
                    powered = true;
                    refreshVertexData = true;
                }
            }
            if (type == VexedLib.DoodadType.StationIcon)
            {
                if (targetDoodad.type == VexedLib.DoodadType.ItemStation)
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
                if (targetDoodad.type == VexedLib.DoodadType.UpgradeStation)
                {
                    if (available == true && targetDoodad.abilityType == AbilityType.Empty)
                    {
                        targetDoodad.available = false;
                        available = false;
                        refreshVertexData = true;
                    }                    
                }
                if (targetDoodad.type == VexedLib.DoodadType.PowerStation)
                {
                    if (available == true && targetDoodad.orbsRemaining <= 0)
                    {
                        targetDoodad.available = false;
                        available = false;
                        refreshVertexData = true;
                    }
                }
                if (targetDoodad.type == VexedLib.DoodadType.SwitchStation)
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
            if (isStation == true && powered == false)
            {
                if (currentRoom.currentOrbs >= activationCost)
                {
                    powered = true;
                    refreshVertexData = true;
                }
            }

            if (type == VexedLib.DoodadType.LeftDoor || type == VexedLib.DoodadType.RightDoor)
            {
                if (targetDoodad.active == true || targetDoodad.available == false)
                    Activate();
                else
                    Deactivate();
            }
            if (type == VexedLib.DoodadType.LeftTunnelDoor || type == VexedLib.DoodadType.RightTunnelDoor)
            {
                if (targetDoodad.active == true || targetDoodad.available == false)
                    Activate();
                else
                    Deactivate();
            }
            if (type == VexedLib.DoodadType.StationIcon)
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

            if (type == VexedLib.DoodadType.JumpPad && stateTransition == 1)
                Deactivate();

            if (type == VexedLib.DoodadType.WallSwitch)
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
                    
                    if (!toggleOn)
                    {
                        //properties.primaryValue = 0;
                        //properties.secondaryValue = 0;
                    }
                    else
                    {
                        //properties.primaryValue = currentBehavior.primaryValue;
                        //properties.secondaryValue = currentBehavior.secondaryValue;
                    }
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

