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
    
    public class Doodad
    {
        public Vertex position;
        public Vertex spawnPosition;
        public bool active = false;
        public string id = "";
        public string targetBehavior ="";
        public string targetObject = "";
        public string expectedBehavior ="";
        public VexedLib.DoodadType type;
        public List<Behavior> behaviors;
        [XmlIgnore]public Behavior currentBehavior = null;
        public String currentBehaviorId;

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
        public int cooldown = 0;

        public AbilityType abilityType = AbilityType.Empty;

        [XmlIgnore]public Doodad srcDoodad = null;

        public float stateTransition = 1;
        public float stateTransitionVelocity = .005f;
        public int stateTransitionDir = 0;

        public Doodad(Doodad d)
        {
            position = new Vertex(d.position);
            spawnPosition = new Vertex(d.spawnPosition);
            active = d.active;

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
            this.type = xmlDoodad.type;
            this.id = xmlDoodad.IDString;
            this.targetBehavior = xmlDoodad.targetBehavior;
            this.targetObject = xmlDoodad.targetObject;
            this.expectedBehavior = xmlDoodad.expectBehavior;

            this.abilityType = (AbilityType)xmlDoodad.ability;
            this.position = new Vertex(xmlDoodad.position, normal, Vector3.Zero, xmlDoodad.up);
            this.spawnPosition = new Vertex(xmlDoodad.position, normal, Vector3.Zero, xmlDoodad.up);
            behaviors = new List<Behavior>();
            currentBehavior = null;

            if (type == VexedLib.DoodadType.PowerOrb)
                active = true;

            if (type == VexedLib.DoodadType.WallSwitch)
                stateTransition = 0;
        }

        public Doodad(VexedLib.DoodadType type, Vector3 position, Vector3 normal, Vector3 direction)
        {
            this.type = type;
            this.position = new Vertex(position, normal, Vector3.Zero, direction);
            this.spawnPosition = new Vertex(position, normal, Vector3.Zero, direction);
            behaviors = new List<Behavior>();
            currentBehavior = null;
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

        public float boundingBoxTop;
        public float boundingBoxBottom;
        public float boundingBoxLeft;
        public float boundingBoxRight;

        public void UpdateBoundingBox(Vector3 playerUp, Vector3 playerRight)
        {
            float x1, x2, x3, x4 = 0;
            float y1, y2, y3, y4 = 0;
            x1 = Vector3.Dot(playerRight, position.position + up);
            x2 = Vector3.Dot(playerRight, position.position + down);
            x3 = Vector3.Dot(playerRight, position.position + left);
            x4 = Vector3.Dot(playerRight, position.position + right);
            y1 = Vector3.Dot(playerUp, position.position + up);
            y2 = Vector3.Dot(playerUp, position.position + down);
            y3 = Vector3.Dot(playerUp, position.position + left);
            y4 = Vector3.Dot(playerUp, position.position + right);
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

        public Color baseColor
        {
            get
            {
                if (type == VexedLib.DoodadType.UpgradeStation)
                    return Color.Gray;
                if (type == VexedLib.DoodadType.PowerStation)
                    return Color.Yellow;
                if (type == VexedLib.DoodadType.JumpPad)
                    return Color.Pink;
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
                    return Color.White;
            }
        }

        public Color activeColor
        {
            get
            {
                if (type == VexedLib.DoodadType.UpgradeStation)
                    return Color.DarkGray;
                if (type == VexedLib.DoodadType.PowerStation)
                    return Color.DarkGray;
                if (type == VexedLib.DoodadType.JumpPad)
                    return Color.Pink;
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
                if (type == VexedLib.DoodadType.Checkpoint)
                    return 2f;
                if (type == VexedLib.DoodadType.WarpStation)
                    return .5f;
                return .5f;
            }
        }

        public bool hasCollisionRect
        {
            get
            {
                if (type == VexedLib.DoodadType.BridgeCover || type == VexedLib.DoodadType.BridgeGate || type == VexedLib.DoodadType.JumpPad || type == VexedLib.DoodadType.UpgradeStation || type == VexedLib.DoodadType.JumpStation || type == VexedLib.DoodadType.ItemStation || type == VexedLib.DoodadType.PowerStation || type == VexedLib.DoodadType.SaveStation || type == VexedLib.DoodadType.SwitchStation)
                    return false;
                if (type == VexedLib.DoodadType.Brick && active == true)
                    return false;
                if (type == VexedLib.DoodadType.Waypoint || type == VexedLib.DoodadType.WallSwitch || type == VexedLib.DoodadType.Checkpoint || type == VexedLib.DoodadType.PowerOrb || type == VexedLib.DoodadType.WarpStation || type == VexedLib.DoodadType.ItemBlock)
                    return false;
                return true;
            }
        }

        public bool shouldRender
        {
            get
            {
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
        public Vector3 right
        {
            get
            {
                return halfWidth * rightUnit;
            }
        }
        public Vector3 left
        {
            get
            {
                return -halfWidth * rightUnit;
            }
        }
        public Vector3 up
        {
            get
            {
                if (type == VexedLib.DoodadType.Door || type == VexedLib.DoodadType.Beam)
                {
                    return (-.5f + stateTransition * 3f) * upUnit;
                    //return 2.5f * upUnit;
                }
                if (type == VexedLib.DoodadType.WallSwitch)
                {
                    /*if (targetDoodad.currentBehavior.id == expectedBehavior)
                        return -.3f * upUnit;
                    else
                        return -.5f * upUnit;*/
                    return (-.3f - stateTransition * .2f) * upUnit;
                }
                return halfHeight * upUnit;
            }
        }
        public Vector3 down
        {
            get
            {
                return -halfHeight * upUnit;
            }
        }
        public float halfWidth
        {
            get
            {
                if (type == VexedLib.DoodadType.BridgeGate)
                    return 1f;
                if (type == VexedLib.DoodadType.BridgeSide)
                    return .25f;
                if (type == VexedLib.DoodadType.BridgeBack)
                    return 1.5f;
                if (type == VexedLib.DoodadType.BridgeCover)
                    return 1.5f;
                if (type == VexedLib.DoodadType.JumpStation || type == VexedLib.DoodadType.PowerStation || type == VexedLib.DoodadType.WarpStation || type == VexedLib.DoodadType.SwitchStation)
                    return .35f;
                if (type == VexedLib.DoodadType.Door || type == VexedLib.DoodadType.Beam || type == VexedLib.DoodadType.PowerOrb)
                    return .1f;
                if (type == VexedLib.DoodadType.WallSwitch)
                    return .25f;
                return .5f;
            }
        }
        public float halfHeight
        {
            get
            {
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
                if (type == VexedLib.DoodadType.WallSwitch)
                    return .25f;
                if (type == VexedLib.DoodadType.Checkpoint || type == VexedLib.DoodadType.ItemBlock || type == VexedLib.DoodadType.PowerOrb || type == VexedLib.DoodadType.WarpStation || type == VexedLib.DoodadType.JumpPad || type == VexedLib.DoodadType.JumpStation || type == VexedLib.DoodadType.ItemStation || type == VexedLib.DoodadType.PowerStation || type == VexedLib.DoodadType.SaveStation || type == VexedLib.DoodadType.SwitchStation || type == VexedLib.DoodadType.UpgradeStation)
                    return .1f;
                return .5f;
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

            doodadVertexList.Add(position.position + up + right);
            doodadVertexList.Add(position.position + up + left);
            doodadVertexList.Add(position.position + down + left);
            doodadVertexList.Add(position.position + down + right);
            
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

        public void Draw(Room currentRoom, List<VertexPositionColorNormalTexture> triangleList)
        {
            if (shouldRender == true)
            {
                List<Vertex> vList = new List<Vertex>();
                vList.Add(new Vertex(position, up + right));
                vList.Add(new Vertex(position, up + left));
                vList.Add(new Vertex(position, down + left));
                vList.Add(new Vertex(position, down + right));
                if (type == VexedLib.DoodadType.BridgeCover)
                {
                    currentRoom.AddBlockToTriangleList(vList, baseColor, .5f, -.6f, Room.plateTexCoords, triangleList);
                    currentRoom.AddBlockToTriangleList(vList, baseColor, -.5f, .6f, Room.plateTexCoords, triangleList);
                }
                else if (type == VexedLib.DoodadType.PowerStation)
                {
                    if (orbsRemaining > 0)
                        currentRoom.AddBlockToTriangleList(vList, baseColor, depth, depth, Room.plateTexCoords, triangleList);
                    else
                        currentRoom.AddBlockToTriangleList(vList, activeColor, depth, depth, Room.plateTexCoords, triangleList);
                }
                else if (type == VexedLib.DoodadType.SwitchStation)
                {
                    if (targetDoodad != null)
                    {
                        if (targetDoodad.currentBehavior.id == targetBehavior)
                            currentRoom.AddBlockToTriangleList(vList, activeColor, depth, depth, Room.plateTexCoords, triangleList);
                        else
                            currentRoom.AddBlockToTriangleList(vList, baseColor, depth, depth, Room.plateTexCoords, triangleList);
                    }
                    if (targetBlock != null)
                    {
                        if (targetBlock.currentBehavior.id == targetBehavior)
                            currentRoom.AddBlockToTriangleList(vList, activeColor, depth, depth, Room.plateTexCoords, triangleList);
                        else
                            currentRoom.AddBlockToTriangleList(vList, baseColor, depth, depth, Room.plateTexCoords, triangleList);
                    }
                    if (targetEdge != null)
                    {
                        if (targetEdge.currentBehavior.id == targetBehavior)
                            currentRoom.AddBlockToTriangleList(vList, activeColor, depth, depth, Room.plateTexCoords, triangleList);
                        else
                            currentRoom.AddBlockToTriangleList(vList, baseColor, depth, depth, Room.plateTexCoords, triangleList);
                    }
                }
                else
                {
                    if (active)
                        currentRoom.AddBlockToTriangleList(vList, activeColor, depth, depth, Room.plateTexCoords, triangleList);
                    else
                        currentRoom.AddBlockToTriangleList(vList, baseColor, depth, depth, Room.plateTexCoords, triangleList);
                }

                if (type == VexedLib.DoodadType.ItemStation || type == VexedLib.DoodadType.ItemBlock || type == VexedLib.DoodadType.UpgradeStation)
                {
                    currentRoom.AddBlockFrontToTriangleList(vList, Color.White, depth+.1f, Ability.texCoordList[(int)abilityType], Engine.abilityDecalObjects, true);
                }
                if(type == VexedLib.DoodadType.SwitchStation && (abilityType == AbilityType.RedKey || abilityType == AbilityType.BlueKey || abilityType == AbilityType.YellowKey))
                {
                    currentRoom.AddBlockFrontToTriangleList(vList, Color.White, depth + .1f, Ability.texCoordList[(int)abilityType], Engine.abilityDecalObjects, true);
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            cooldown -= gameTime.ElapsedGameTime.Milliseconds;
            if (cooldown < 0) cooldown = 0;

            stateTransition += gameTime.ElapsedGameTime.Milliseconds * stateTransitionDir * stateTransitionVelocity;
            if (stateTransitionDir == 1 && stateTransition > 1)
            {
                toggleOn = true;
                stateTransition = 1;
            }
            if (stateTransitionDir == -1 && stateTransition < 0)
            {
                toggleOn = false;
                stateTransition = 0;
            }

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
            stateTransitionDir = 1;
        }
        public void Deactivate()
        {
            stateTransitionDir = -1;
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
