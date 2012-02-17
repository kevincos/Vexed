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
    public class Room
    {

        public static Texture2D blockTexture;


        public static List<Vector2> edgeSideTexCoords;
        public static List<Vector2> edgeSideEndTexCoords;
        public static List<Vector2> edgeTopTexCoords;
        public static List<Vector2> edgeTopEndTexCoords;
      
        public static List<Vector2> rubberSideSmallTexCoords;
        public static List<Vector2> rubberSideSmallEndTexCoords;

        public static List<Vector2> beltSideSmallTexCoords;
        public static List<Vector2> beltSideSmallEndTexCoords;

        public static List<Vector2> plateTexCoords;
        public static List<Vector2> blankTexCoords;
        public static List<Vector2> fullTexCoords;
        public static List<Vector2> midTexCoords = new List<Vector2>();
        public static List<Vector2> startTexCoords = new List<Vector2>();
        public static List<Vector2> endTexCoords = new List<Vector2>();

        public static float plateTexWidth = 1f;

        public static int texGridCount = 8;

        public static int beltAnimation = 0;

        public static int innerBlockMode = 2;


        public bool roomHighlight = false;
        public bool sectorHighlight = false;
        public bool adjacent = false;

        public String friendlyName;
        public VL.Decal stationDecal;

        public Vector3 center;
        public Vector3 size;
        public bool hasWarp = false;
        public int warpCost = 0;
        public Color color;
        public int maxOrbs = 0;
        public int currentOrbs = 0;
        public bool explored = false;
        public bool bossCleared = false;

        public int currentBlueOrbs = 0;
        public int maxBlueOrbs = 0;

        public int currentRedOrbs = 0;
        public int maxRedOrbs = 0;

        public List<Block> blocks;
        public List<Doodad> doodads;
        public List<JumpRing> jumpRings;
        public List<Tunnel> tunnels;
        public List<Monster> monsters;
        public List<Projectile> projectiles;
        public List<Decoration> decorations;
        public string id;
        public string sectorID;
        [XmlIgnore]public Sector parentSector;
        [XmlIgnore]public Vector2 mapPosition2D;

        public List<VertexPositionColorNormalTexture> staticPlate;
        public List<VertexPositionColorNormalTexture> staticFancyPlate;
        public List<VertexPositionColorNormalTexture> staticVines;
        public List<VertexPositionColorNormalTexture> staticCircuit;
        public List<VertexPositionColorNormalTexture> staticCargo;
        public List<VertexPositionColorNormalTexture> staticCrate;
        public List<VertexPositionColorNormalTexture> staticCobblestone;
        public List<VertexPositionColorNormalTexture> staticCrystal;
        public List<VertexPositionColorNormalTexture> staticIce;
        public List<VertexPositionColorNormalTexture> staticGearslot;
        public List<VertexPositionColorNormalTexture> staticRings;

        public List<VertexPositionColorNormalTexture> dynamicFancyPlate;
        public List<VertexPositionColorNormalTexture> dynamicPlate;
        public List<VertexPositionColorNormalTexture> dynamicBrick;        
        public VertexPositionColorNormalTexture[] dynamicFancyPlateTriangleArray;
        public VertexPositionColorNormalTexture[] dynamicPlateTriangleArray;
        public VertexPositionColorNormalTexture[] dynamicBrickTriangleArray;
        public List<VertexPositionColorNormalTexture> []masterBlockTextures = new List<VertexPositionColorNormalTexture>[11];
        public VertexPositionColorNormalTexture[][] masterBlockArray;

        public VertexPositionColorNormalTexture[] fancyPlateTriangleArray;
        public VertexPositionColorNormalTexture[] plateTriangleArray;
        public VertexPositionColorNormalTexture[] circuitTriangleArray;
        public VertexPositionColorNormalTexture[] vinesTriangleArray;
        public VertexPositionColorNormalTexture[] cargoTriangleArray;
        public VertexPositionColorNormalTexture[] crateTriangleArray;
        public VertexPositionColorNormalTexture[] crystalTriangleArray;
        public VertexPositionColorNormalTexture[] iceTriangleArray;
        public VertexPositionColorNormalTexture[] cobblestoneTriangleArray;
        public VertexPositionColorNormalTexture[] gearslotTriangleArray;
        public VertexPositionColorNormalTexture[] ringTriangleArray;

        public List<VertexPositionColorNormalTexture>[] projectilesTriangles;
        public List<VertexPositionColorNormalTexture> blastTriangles;
        public List<VertexPositionColorNormalTexture>[] monsterTriangles;
        
        public bool refreshVertices = false;

        public Room(Room r)
        {
            center = r.center;
            size = r.size;
            hasWarp = r.hasWarp;
            color = r.color;
            id = r.id;
            currentOrbs = r.currentOrbs;
            maxOrbs = r.maxOrbs;
            currentBlueOrbs = r.currentBlueOrbs;
            currentRedOrbs = r.currentRedOrbs;
            maxRedOrbs = r.maxRedOrbs;
            maxBlueOrbs = r.maxBlueOrbs;
            sectorID = r.sectorID;
            explored = r.explored;
            bossCleared = r.bossCleared;
            stationDecal = r.stationDecal;
            friendlyName = r.friendlyName;

            blocks = new List<Block>();
            foreach (Block b in r.blocks)
            {
                blocks.Add(new Block(b));
            }
            doodads = new List<Doodad>();
            foreach (Doodad d in r.doodads)
            {
                doodads.Add(new Doodad(d));
            }
            jumpRings = new List<JumpRing>();
            tunnels = new List<Tunnel>();

            monsters = new List<Monster>();
            foreach (Monster m in r.monsters)
            {
                monsters.Add(new Monster(m));
            }
            projectiles = new List<Projectile>();
            foreach (Projectile p in r.projectiles)
            {
                projectiles.Add(new Projectile(p));
            }
            decorations = new List<Decoration>();
            foreach (Decoration d in r.decorations)
            {
                decorations.Add(new Decoration(d));
            }
        }

        public Room()
        {
            blocks = new List<Block>();
            doodads = new List<Doodad>();
            monsters = new List<Monster>();
            projectiles = new List<Projectile>();
            decorations = new List<Decoration>();
            jumpRings = new List<JumpRing>();
            tunnels = new List<Tunnel>();
        }

        public Room(VL.Room xmlRoom)
        {
            id = xmlRoom.IDString;
            center = new Vector3(xmlRoom.centerX, xmlRoom.centerY, xmlRoom.centerZ);
            size = new Vector3(xmlRoom.sizeX, xmlRoom.sizeY, xmlRoom.sizeZ);
            blocks = new List<Block>();
            doodads = new List<Doodad>();
            monsters = new List<Monster>();
            projectiles = new List<Projectile>();
            decorations = new List<Decoration>();
            jumpRings = new List<JumpRing>();
            tunnels = new List<Tunnel>();
        }

        public void Load(Rm r)
        {
            bossCleared = r.bc;
            explored = r.e;
            currentOrbs = r.co;
            currentBlueOrbs = r.cbo;
            currentRedOrbs = r.cro;
        }


        public static List<Vector2> LoadTexCoords(int x, int y, float epsilonX, float epsilonY)
        {
            float texWidth = 1f / texGridCount;
            List<Vector2> texCoords = new List<Vector2>();
            texCoords.Add(new Vector2((x + 1) * texWidth - epsilonX, y * texWidth + epsilonY));
            texCoords.Add(new Vector2(x * texWidth + epsilonX, y * texWidth + epsilonY));
            texCoords.Add(new Vector2(x * texWidth + epsilonX, (y + 1) * texWidth - epsilonY));
            texCoords.Add(new Vector2((x + 1) * texWidth - epsilonX, (y + 1) * texWidth - epsilonY));

            return texCoords;
        }

        public static void InitTexCoords()
        {
            plateTexCoords = new List<Vector2>();
            plateTexCoords.Add(new Vector2(plateTexWidth, 0));
            plateTexCoords.Add(new Vector2(0, 0));
            plateTexCoords.Add(new Vector2(0, plateTexWidth));
            plateTexCoords.Add(new Vector2(plateTexWidth, plateTexWidth));
            blankTexCoords = new List<Vector2>();
            blankTexCoords.Add(new Vector2(1, 0));
            blankTexCoords.Add(new Vector2(0, 0));
            blankTexCoords.Add(new Vector2(0, 1));
            blankTexCoords.Add(new Vector2(1, 1));


            edgeSideTexCoords = new List<Vector2>();
            edgeSideTexCoords.Add(new Vector2(1f, 0.5f));
            edgeSideTexCoords.Add(new Vector2(.5f, 0.5f));
            edgeSideTexCoords.Add(new Vector2(.5f, 1));
            edgeSideTexCoords.Add(new Vector2(1f, 1));

            edgeTopTexCoords = new List<Vector2>();
            edgeTopTexCoords.Add(new Vector2(1, 0));
            edgeTopTexCoords.Add(new Vector2(.5f, 0));
            edgeTopTexCoords.Add(new Vector2(.5f, .5f));
            edgeTopTexCoords.Add(new Vector2(1f, .5f));

            edgeSideEndTexCoords = new List<Vector2>();
            edgeSideEndTexCoords.Add(new Vector2(.5f, 0.5f));
            edgeSideEndTexCoords.Add(new Vector2(0, 0.5f));
            edgeSideEndTexCoords.Add(new Vector2(0, 1));
            edgeSideEndTexCoords.Add(new Vector2(.5f, 1));

            edgeTopEndTexCoords = new List<Vector2>();
            edgeTopEndTexCoords.Add(new Vector2(.5f, 0));
            edgeTopEndTexCoords.Add(new Vector2(.0f, 0));
            edgeTopEndTexCoords.Add(new Vector2(.0f, .5f));
            edgeTopEndTexCoords.Add(new Vector2(.5f, .5f));
          
            beltSideSmallTexCoords = LoadTexCoords(5, 6, .0f, .003f);
            beltSideSmallEndTexCoords = LoadTexCoords(4, 6, .003f, .003f);
            rubberSideSmallTexCoords = LoadTexCoords(7, 3, .0f, .003f);
            rubberSideSmallEndTexCoords = LoadTexCoords(7, 3, .003f, .003f);

            midTexCoords = new List<Vector2>();
            midTexCoords.Add(new Vector2(.7f, 0));
            midTexCoords.Add(new Vector2(.3f, 0));
            midTexCoords.Add(new Vector2(.3f, 1));
            midTexCoords.Add(new Vector2(.7f, 1));

            endTexCoords = new List<Vector2>();
            endTexCoords.Add(new Vector2(1, 0));
            endTexCoords.Add(new Vector2(.15f, 0));
            endTexCoords.Add(new Vector2(.15f, 1));
            endTexCoords.Add(new Vector2(1, 1));

            startTexCoords = new List<Vector2>();
            startTexCoords.Add(new Vector2(.85f, 0));
            startTexCoords.Add(new Vector2(0, 0));
            startTexCoords.Add(new Vector2(0, 1));
            startTexCoords.Add(new Vector2(.85f, 1));
        }

        public Doodad ActivateMonsterOrb()
        {
            foreach (Doodad d in doodads)
            {
                if (d.idle == true)
                {
                    d.idle = false;
                    return d;
                }
            }
            return null;
        }

        public void Reset()
        {
            projectiles.Clear();
            SnakeBoss.Init();
            foreach (Monster m in monsters)
            {
                if(bossCleared == true && (m.moveType == VL.MovementType.ArmorBoss || m.moveType == VL.MovementType.BattleBoss || m.moveType == VL.MovementType.ChaseBoss || m.moveType == VL.MovementType.FaceBoss ||
                    m.moveType == VL.MovementType.JetBoss || m.moveType == VL.MovementType.RockBoss || m.moveType == VL.MovementType.SnakeBoss))
                {
                    m.dead = true;
                    m.deathTime = 0;
                    m.state = MonsterState.Death;
                    m.baseHP = 0;
                    continue;
                }
                m.ResetBossState();
                m.position = new Vertex(m.spawnPosition);
                m.armorType = m.startingArmorType;
                m.baseHP = m.startingBaseHP;
                m.armorHP = m.startingArmorHP;
                m.dead = false;
                m.state = MonsterState.Spawn;
                m.spawnTime = 0;
                m.armorBreakTime = 0;
                m.deathTime = Monster.maxDeathTime;
                m.armorState = ArmorState.Normal;
                foreach (GunEmplacement g in m.guns)
                    g.Reset(m);
            }
            foreach (Monster m in monsters)
            {
                if(m.moveType == VL.MovementType.SnakeBoss)
                    m.snakeBoss.InitializeLinks(m, this);                
            }
            foreach (Doodad d in doodads)
            {
                d.position = new Vertex(d.spawnPosition);
                d.abilityType = d.originalAbilityType;
                if (d.type == VL.DoodadType.Brick)
                {
                    d.active = false;
                    d.breakTime = 0;
                    d.refreshVertexData = true;
                }
                if (d.type == VL.DoodadType.PowerPlug)
                    d.active = false;
            }
        }

        public Color currentColor
        {
            get
            {
                Color powerUpColor = color;
                if (maxOrbs != 0)
                {
                    powerUpColor.R = (Byte)(40 + currentOrbs * (color.R - 40) / maxOrbs);
                    powerUpColor.G = (Byte)(40 + currentOrbs * (color.G - 40) / maxOrbs);
                    powerUpColor.B = (Byte)(40 + currentOrbs * (color.B - 40) / maxOrbs);
                }
                return powerUpColor;
            }
        }

        public void Update(int gameTime)
        {
            
            if (WorldMap.state == ZoomState.None || WorldMap.state == ZoomState.ZoomFromSector || WorldMap.state == ZoomState.ZoomToSector || Engine.player.currentRoom == this || roomHighlight == true)
            {
                if(this == Engine.player.currentRoom || adjacent == true)
                {
                    
                    beltAnimation += gameTime;
                    foreach (Block b in blocks)
                    {
                        int blockUpdateTime = b.UpdateBehavior(gameTime);
                        foreach (Edge e in b.edges)
                        {
                            e.start.Update(this, blockUpdateTime);
                            e.end.Update(this, blockUpdateTime);
                            e.UpdateBehavior(gameTime);
                        }
                    }
                    foreach (Doodad d in doodads)
                    {
                        d.UpdateAmbientSounds(gameTime);
                    }
                    
                    if (this == Engine.player.currentRoom)
                    {
                        foreach (Doodad d in doodads)
                        {
                            if (d.freeMotion)
                            {
                                d.position.Update(this, gameTime);
                                d.refreshBoundingBox = true;
                                Vector3 gravityDirection = Engine.player.center.direction;
                                if (d.position.normal == Engine.player.center.direction)
                                {
                                    gravityDirection = -Engine.player.center.normal;
                                }
                                else if (d.position.normal == -Engine.player.center.direction)
                                {
                                    gravityDirection = Engine.player.center.normal;
                                }

                                d.position.velocity -= Engine.player.gravityAcceleration * gravityDirection;

                                Vector3 up = d.position.direction;
                                Vector3 right = Vector3.Cross(d.position.direction, d.position.normal);

                                float upMagnitude = Vector3.Dot(up, d.position.velocity);
                                float rightMagnitude = Vector3.Dot(right, d.position.velocity);
                                float maxSpeed = .009f;
                                if (upMagnitude > maxSpeed)
                                {
                                    d.position.velocity -= (upMagnitude - maxSpeed) * up;
                                }
                                if (upMagnitude < -maxSpeed)
                                {
                                    d.position.velocity -= (maxSpeed + upMagnitude) * up;
                                }
                                if (rightMagnitude > maxSpeed)
                                {
                                    d.position.velocity -= (rightMagnitude - maxSpeed) * right;
                                }
                                if (rightMagnitude < -maxSpeed)
                                {
                                    d.position.velocity -= (maxSpeed + rightMagnitude) * right;
                                }

                            }
                            else
                            {
                                d.position.velocity = Vector3.Zero;
                            }
                            d.UpdateBehavior(gameTime);
                            d.Update(gameTime);
                        }
                    }
                }
            }
        }

        public Vector3 AdjustedUp(Vector3 position, Vector3 up, Vector3 normal, float roundingThreshold)
        {
            Vector3 right = Vector3.Cross(up, normal);
            Vector3 relPosition = position - center;
            Vector3 returnUp = up;
            float upValue = Vector3.Dot(relPosition, up);
            float maxUpValue = Math.Abs(Vector3.Dot(size / 2, up));
            float rightValue = Vector3.Dot(relPosition, right);
            float maxRightValue = Math.Abs(Vector3.Dot(size / 2, right));
            if (upValue > maxUpValue - roundingThreshold)
            {
                float percentage = (roundingThreshold + maxUpValue - upValue) / (2*roundingThreshold);
                returnUp = percentage * up - (1-percentage) * normal;
            }
            if (upValue < roundingThreshold - maxUpValue)
            {
                float percentage = (roundingThreshold + upValue + maxUpValue) / (2*roundingThreshold);
                returnUp = percentage * up + (1-percentage) * normal;
            }
            returnUp.Normalize();
            return returnUp;
        }

        public Vector3 RaisedPosition(Vector3 position, float distanceModifier, float roundingThreshold)
        {
            Vector3 modifier = Vector3.Zero;
            Vector3 relPosition = position - center;
            
            if (relPosition.X > size.X / 2 - roundingThreshold)
            {
                modifier.X += distanceModifier * (relPosition.X - size.X / 2 + roundingThreshold)/roundingThreshold;
            }
            if (relPosition.X < -size.X / 2 + roundingThreshold)
            {
                modifier.X += distanceModifier * (relPosition.X + size.X / 2 - roundingThreshold) / roundingThreshold;
            }
            if (relPosition.Y > size.Y / 2 - roundingThreshold)
            {
                modifier.Y += distanceModifier * (relPosition.Y - size.Y / 2 + roundingThreshold) / roundingThreshold;
            }
            if (relPosition.Y < -size.Y / 2 + roundingThreshold)
            {
                modifier.Y += distanceModifier * (relPosition.Y + size.Y / 2 - roundingThreshold) / roundingThreshold;
            }
            if (relPosition.Z > size.Z / 2 - roundingThreshold)
            {
                modifier.Z += distanceModifier * (relPosition.Z - size.Z / 2 + roundingThreshold) / roundingThreshold;
            }
            if (relPosition.Z < -size.Z / 2 + roundingThreshold)
            {
                modifier.Z += distanceModifier * (relPosition.Z + size.Z / 2 - roundingThreshold) / roundingThreshold;
            }
            
            return position + modifier;
        }

        public VertexPositionColorNormal GenerateVertex(Vector3 position, Color color, Vector3 normal, float distanceModifier)
        {
            return new VertexPositionColorNormal(RaisedPosition(position, distanceModifier, 1f), color, normal);
        }
        
        public VertexPositionColorNormalTexture GenerateTexturedVertex(Vector3 position, Vector2 texCoord, Color color, Vector3 normal, float distanceModifier)
        {
            return new VertexPositionColorNormalTexture(RaisedPosition(position, distanceModifier, 1f), color, normal, texCoord);
        }

        public VertexPositionColorNormalTexture GenerateTexturedVertex(Vector3 position, Vector2 texCoord, Color color, Vector3 normal, float distanceModifier, bool cornerStretch)
        {
            if(cornerStretch == true)
                return new VertexPositionColorNormalTexture(RaisedPosition(position, distanceModifier, 1f), color, normal, texCoord);
            return new VertexPositionColorNormalTexture(RaisedPosition(position, distanceModifier, .1f), color, normal, texCoord);
        }

        public void AddStripToTriangleListHelper(Vertex start, Vertex end, float depth, EdgeProperties properties, List<VertexPositionColorNormalTexture> triangleList)
        {
            Vector3 edgeDir = end.position - start.position;
            Vector3 edgeNormal = Vector3.Cross(end.position - start.position, start.normal);
            edgeNormal.Normalize();
            edgeDir.Normalize();
            Color baseColor = Color.White;
            if(properties.type == VL.EdgeType.Ice)
                baseColor = Color.White;
            if (properties.type == VL.EdgeType.Magnet)
                baseColor = Color.Gray;
            if (properties.type == VL.EdgeType.Bounce)
                baseColor = Color.Magenta;
            if (properties.type == VL.EdgeType.Electric)
            {
                if (properties.primaryValue == 0)
                    baseColor = new Color(40, 40, 40);
                else
                    baseColor = Color.Yellow;
            }
            if (properties.type == VL.EdgeType.ConveyorBelt)
                baseColor = Color.DarkGray;
            
            if (properties.type != VL.EdgeType.Spikes)
            {
                float epsilon = .01f;

                //side
                triangleList.Add(GenerateTexturedVertex(start.position + epsilon * edgeNormal, blankTexCoords[1], baseColor, start.normal, depth + epsilon));
                triangleList.Add(GenerateTexturedVertex(start.position - .5f * edgeNormal, blankTexCoords[1], baseColor, start.normal, depth + epsilon));
                triangleList.Add(GenerateTexturedVertex(end.position + epsilon * edgeNormal, blankTexCoords[1], baseColor, start.normal, depth + epsilon));

                triangleList.Add(GenerateTexturedVertex(end.position + epsilon * edgeNormal, blankTexCoords[1], baseColor, start.normal, depth + epsilon));
                triangleList.Add(GenerateTexturedVertex(start.position - .5f * edgeNormal, blankTexCoords[1], baseColor, start.normal, depth + epsilon));
                triangleList.Add(GenerateTexturedVertex(end.position - .5f * edgeNormal, blankTexCoords[1], baseColor, start.normal, depth + epsilon));

                //top
                triangleList.Add(GenerateTexturedVertex(start.position + epsilon * edgeNormal, blankTexCoords[1], baseColor, edgeNormal, depth + epsilon));
                triangleList.Add(GenerateTexturedVertex(start.position + epsilon * edgeNormal, blankTexCoords[1], baseColor, edgeNormal, -depth));
                triangleList.Add(GenerateTexturedVertex(end.position + epsilon * edgeNormal, blankTexCoords[1], baseColor, edgeNormal, depth + epsilon));

                triangleList.Add(GenerateTexturedVertex(end.position + epsilon * edgeNormal, blankTexCoords[1], baseColor, edgeNormal, -depth));
                triangleList.Add(GenerateTexturedVertex(start.position + epsilon * edgeNormal, blankTexCoords[1], baseColor, edgeNormal, -depth));
                triangleList.Add(GenerateTexturedVertex(end.position + epsilon * edgeNormal, blankTexCoords[1], baseColor, edgeNormal, depth + epsilon));
            }
            if (properties.type == VL.EdgeType.Spikes)
            {
                int numSpikes = 2 * (int)(end.position - start.position).Length();
                float spikeHeight = .75f;
                float spikeWidth = (end.position - start.position).Length() / numSpikes;
                Color spikeColor = new Color(180, 180, 180);
                for (int i = 0; i < numSpikes; i++)
                {
                    Vector3 spikeStart = start.position + i * spikeWidth * edgeDir;
                    Vector3 spikeEnd = start.position + (i + 1) * spikeWidth * edgeDir;
                    Vector3 spikePoint = .5f * (spikeStart + spikeEnd) + spikeHeight * edgeNormal;

                    Color shadedColor = Color.Blue;

                    shadedColor = FakeShader.Shade(spikeColor, start.normal);
                    triangleList.Add(GenerateTexturedVertex(spikeStart, blankTexCoords[1], shadedColor, start.normal, depth));
                    triangleList.Add(GenerateTexturedVertex(spikeEnd, blankTexCoords[1], shadedColor, start.normal, depth));
                    triangleList.Add(GenerateTexturedVertex(spikePoint, blankTexCoords[1], shadedColor, edgeNormal, depth / 2));
                    
                    shadedColor = FakeShader.Shade(spikeColor, -start.normal);
                    triangleList.Add(GenerateTexturedVertex(spikeStart, blankTexCoords[1], shadedColor, -start.normal, 0));
                    triangleList.Add(GenerateTexturedVertex(spikeEnd, blankTexCoords[1], shadedColor, -start.normal, 0));
                    triangleList.Add(GenerateTexturedVertex(spikePoint, blankTexCoords[1], shadedColor, edgeNormal, depth / 2));

                    shadedColor = FakeShader.Shade(spikeColor, edgeDir);
                    triangleList.Add(GenerateTexturedVertex(spikeStart, blankTexCoords[1], shadedColor, edgeDir, depth));
                    triangleList.Add(GenerateTexturedVertex(spikeStart, blankTexCoords[1], shadedColor, edgeDir, 0));
                    triangleList.Add(GenerateTexturedVertex(spikePoint, blankTexCoords[1], shadedColor, edgeNormal, depth / 2));

                    shadedColor = FakeShader.Shade(spikeColor, -edgeDir);
                    triangleList.Add(GenerateTexturedVertex(spikeEnd, blankTexCoords[1], shadedColor, -edgeDir, depth));
                    triangleList.Add(GenerateTexturedVertex(spikeEnd, blankTexCoords[1], shadedColor, -edgeDir, 0));
                    triangleList.Add(GenerateTexturedVertex(spikePoint, blankTexCoords[1], shadedColor, edgeNormal, depth / 2));
                }
            
            }
            if (properties.type == VL.EdgeType.ConveyorBelt)
            {
                int numSegments = (int)(end.position - start.position).Length();
                float segmentWidth = (end.position - start.position).Length() / numSegments;
                float arrowWidth = 0;
                if (properties.primaryValue > 0)
                    arrowWidth = -.5f;
                if (properties.primaryValue < 0)
                    arrowWidth = .5f;


                for (int i = 1; i < numSegments; i++)
                {
                    triangleList.Add(GenerateTexturedVertex(start.position + i * segmentWidth * edgeDir,blankTexCoords[1], Color.Yellow, start.normal, depth + .002f));
                    triangleList.Add(GenerateTexturedVertex(start.position + i * segmentWidth * edgeDir - .5f * edgeNormal,blankTexCoords[1], Color.Yellow, start.normal, depth + .002f));
                    triangleList.Add(GenerateTexturedVertex(start.position + arrowWidth * edgeDir + i * segmentWidth * edgeDir - .25f * edgeNormal,blankTexCoords[1], Color.Yellow, start.normal, depth + .002f));
                }
            }            
        }

        public void AddTopStrip(Vertex start, Vertex end, float depth, float epsilon, bool flip, List<Vector2> texCoords, List<VertexPositionColorNormalTexture> triangleList)
        {           
            Color baseColor = Color.White;

            if (start.normal == end.normal)
            {                
                Vector3 edgeDir = end.position - start.position;
                Vector3 edgeNormal = Vector3.Cross(edgeDir, start.normal);
                edgeNormal.Normalize();
                edgeDir.Normalize();
                if(flip == true)
                    edgeNormal *= -1;            

                triangleList.Add(GenerateTexturedVertex(start.position + epsilon * edgeNormal, texCoords[2], baseColor, edgeNormal, depth + epsilon));
                triangleList.Add(GenerateTexturedVertex(start.position + epsilon * edgeNormal, texCoords[1], baseColor, edgeNormal, -depth));
                triangleList.Add(GenerateTexturedVertex(end.position + epsilon * edgeNormal, texCoords[3], baseColor, edgeNormal, depth + epsilon));

                triangleList.Add(GenerateTexturedVertex(end.position + epsilon * edgeNormal, texCoords[0], baseColor, edgeNormal, -depth));
                triangleList.Add(GenerateTexturedVertex(start.position + epsilon * edgeNormal, texCoords[1], baseColor, edgeNormal, -depth));
                triangleList.Add(GenerateTexturedVertex(end.position + epsilon * edgeNormal, texCoords[3], baseColor, edgeNormal, depth + epsilon));
            }
            else
            {
                Vector3 fullEdge = end.position - start.position;
                Vector3 currentComponent = Vector3.Dot(end.normal, fullEdge) * end.normal;
                Vector3 nextComponent = Vector3.Dot(start.normal, fullEdge) * start.normal;
                Vector3 constantComponent = Vector3.Dot(Vector3.Cross(end.normal, start.normal), fullEdge) * Vector3.Cross(end.normal, start.normal);
                float currentPercent = currentComponent.Length() / (currentComponent.Length() + nextComponent.Length());

                Vector3 midPoint = start.position + currentComponent + currentPercent * constantComponent;

                Vector3 edgeDir = midPoint - start.position;
                Vector3 edgeNormal = Vector3.Cross(midPoint - start.position, start.normal);
                edgeNormal.Normalize();
                edgeDir.Normalize();
                if (flip == true)
                    edgeNormal *= -1;    
                
                Vector2 midA = currentPercent * texCoords[3] + (1f-currentPercent) * texCoords[2];
                Vector2 midB = currentPercent * texCoords[0] + (1f-currentPercent) * texCoords[1];

                triangleList.Add(GenerateTexturedVertex(start.position + epsilon * edgeNormal, texCoords[2], baseColor, edgeNormal, depth + epsilon));
                triangleList.Add(GenerateTexturedVertex(start.position + epsilon * edgeNormal, texCoords[1], baseColor, edgeNormal, -depth));
                triangleList.Add(GenerateTexturedVertex(midPoint + epsilon * edgeNormal, midA, baseColor, edgeNormal, depth + epsilon));

                triangleList.Add(GenerateTexturedVertex(midPoint + epsilon * edgeNormal, midB, baseColor, edgeNormal, -depth));
                triangleList.Add(GenerateTexturedVertex(start.position + epsilon * edgeNormal, texCoords[1], baseColor, edgeNormal, -depth));
                triangleList.Add(GenerateTexturedVertex(midPoint + epsilon * edgeNormal, midA, baseColor, edgeNormal, depth + epsilon));

                edgeDir = end.position - midPoint;
                edgeNormal = Vector3.Cross(end.position - midPoint, end.normal);
                edgeNormal.Normalize();
                edgeDir.Normalize();
                if (flip == true)
                    edgeNormal *= -1;    
                
                triangleList.Add(GenerateTexturedVertex(midPoint + epsilon * edgeNormal, midA, baseColor, edgeNormal, depth + epsilon));
                triangleList.Add(GenerateTexturedVertex(midPoint + epsilon * edgeNormal, midB, baseColor, edgeNormal, -depth));
                triangleList.Add(GenerateTexturedVertex(end.position + epsilon * edgeNormal, texCoords[3], baseColor, edgeNormal, depth + epsilon));

                triangleList.Add(GenerateTexturedVertex(end.position + epsilon * edgeNormal, texCoords[0], baseColor, edgeNormal, -depth));
                triangleList.Add(GenerateTexturedVertex(midPoint + epsilon * edgeNormal, midB, baseColor, edgeNormal, -depth));
                triangleList.Add(GenerateTexturedVertex(end.position + epsilon * edgeNormal, texCoords[3], baseColor, edgeNormal, depth + epsilon));
            
            }


        }

        public void AddStripToTriangleList2(Edge e, float depth, List<VertexPositionColorNormalTexture> triangleList)
        {
            float edgeLength = (e.end.position - e.start.position).Length();           

            if (e.start.normal != e.end.normal)
            {
                Vector3 fullEdge = e.end.position - e.start.position;
                Vector3 currentComponent = Vector3.Dot(e.end.normal, fullEdge) * e.end.normal;
                Vector3 nextComponent = Vector3.Dot(e.start.normal, fullEdge) * e.start.normal;
                Vector3 constantComponent = Vector3.Dot(Vector3.Cross(e.end.normal, e.start.normal), fullEdge) * Vector3.Cross(e.end.normal, e.start.normal);
                float currentPercent = currentComponent.Length() / (currentComponent.Length() + nextComponent.Length());
                Vector3 midPoint = e.start.position + currentComponent + currentPercent * constantComponent;
                edgeLength = (e.end.position - midPoint).Length() + (e.start.position - midPoint).Length();
            }

            int width = (int)(edgeLength+.5f);
            float blockWidth = edgeLength / width;


            List<Vertex> fullPointList = new List<Vertex>();
            for (int i = 0; i < width+1; i++)
            {
                Vector3 up = Vector3.Cross(e.start.normal, e.start.direction);
                fullPointList.Add(new Vertex(e.start, e.start.direction * i + up));
                fullPointList.Add(new Vertex(e.start, e.start.direction * i));                
            }
            for (int i = 0; i < (width+1) * 2; i++)
            {
                fullPointList[i].Update(this, 0);
            }

            for (int i = 0; i < width; i++)
            {
                List<Vertex> subList = new List<Vertex>();
                List<Vertex> mirrorSubList = new List<Vertex>();
                Vector3 up = Vector3.Cross(fullPointList[(i * 2)].direction, fullPointList[i * 2].normal);

                mirrorSubList.Add(new Vertex(fullPointList[(i) * 2 + 1], .01f * up));
                mirrorSubList.Add(new Vertex(fullPointList[(i + 1) * 2 + 1], .01f * up));
                mirrorSubList.Add(fullPointList[(i + 1) * 2]);
                mirrorSubList.Add(fullPointList[i * 2]);
                
                subList.Add(new Vertex(fullPointList[(i+1) * 2 + 1], .01f*up));
                subList.Add(new Vertex(fullPointList[(i) * 2 + 1], .01f*up));
                subList.Add(fullPointList[(i) * 2]);
                subList.Add(fullPointList[(i+1) * 2]);
                /*foreach (Vertex v in mirrorSubList)
                    v.Update(this, 0);
                foreach (Vertex v in subList)
                    v.Update(this, 0);*/

                if (e.properties.type == VL.EdgeType.Ice)
                {
                    #region ice
                    if (i == width - 1)
                    {
                        AddBlockFrontToTriangleList(mirrorSubList, Color.White, depth + .01f, edgeSideEndTexCoords, triangleList, true);
                        AddTopStrip(fullPointList[(i + 1) * 2 + 1], fullPointList[i * 2 + 1], depth + .01f, .01f, true, edgeTopEndTexCoords, triangleList);                        
                    }
                    else if (i == 0)
                    {
                        AddBlockFrontToTriangleList(subList, Color.White, depth + .01f, edgeSideEndTexCoords, triangleList, true);
                        AddTopStrip(fullPointList[i * 2 + 1], fullPointList[(i + 1) * 2 + 1], depth + .01f, .01f, false, edgeTopEndTexCoords, triangleList);
                    }
                    else
                    {                        
                        AddBlockFrontToTriangleList(subList, Color.White, depth + .01f, edgeSideTexCoords, triangleList, true);
                        AddTopStrip(fullPointList[i * 2 + 1], fullPointList[(i + 1) * 2 + 1], depth + .01f, .01f, false, edgeTopTexCoords, triangleList);
                    }
                    #endregion
                }
                else if (e.properties.type == VL.EdgeType.Bounce)
                {
                    #region bounce
                    float treadRise = .1f;
                    float treadFraction = .75f;
                    if (i == width - 1)
                    {
                        Vertex v1t = new Vertex(fullPointList[i * 2 + 1], treadRise * up);
                        Vertex v3t = fullPointList[(i + 1) * 2 + 1];
                        Vertex v2t = new Vertex(fullPointList[i * 2 + 1], treadRise * up + treadFraction * (fullPointList[(i + 1) * 2 + 1].position - fullPointList[i * 2 + 1].position));
                        v1t.Update(this, 0);
                        v2t.Update(this, 0);
                        v3t.Update(this, 0);

                        List<Vertex> subListA = new List<Vertex>();
                        subListA.Add(new Vertex(subList[0], Vector3.Zero)); // Left
                        subListA.Add(new Vertex(subList[1], treadRise * up + treadFraction * (fullPointList[(i + 1) * 2 + 1].position - fullPointList[i * 2 + 1].position)));
                        subListA.Add(new Vertex(subList[2], treadFraction * (fullPointList[(i + 1) * 2 + 1].position - fullPointList[i * 2 + 1].position)));
                        subListA.Add(new Vertex(subList[3], Vector3.Zero)); // Left
                        foreach (Vertex v in subListA)
                            v.Update(this, 0);
                        List<Vertex> subListB = new List<Vertex>();
                        subListB.Add(new Vertex(subList[1], treadRise * up + treadFraction * (fullPointList[(i + 1) * 2 + 1].position - fullPointList[i * 2 + 1].position)));
                        subListB.Add(new Vertex(subList[1], treadRise * up));
                        subListB.Add(new Vertex(subList[2], Vector3.Zero));
                        subListB.Add(new Vertex(subList[2], treadFraction * (fullPointList[(i + 1) * 2 + 1].position - fullPointList[i * 2 + 1].position)));
                        foreach (Vertex v in subListB)
                            v.Update(this, 0);

                        AddTopStrip(v1t, v2t, depth + .011f, .00f, false, edgeTopTexCoords, triangleList);
                        AddTopStrip(v2t, v3t, depth + .011f, .00f, false, edgeTopTexCoords, triangleList);

                        AddBlockFrontToTriangleList(mirrorSubList, Color.White, depth + .01f, edgeSideEndTexCoords, triangleList, true);
                        AddBlockFrontToTriangleList(subListA, Color.White, depth + .011f, rubberSideSmallTexCoords, triangleList, true);
                        AddBlockFrontToTriangleList(subListB, Color.White, depth + .011f, rubberSideSmallTexCoords, triangleList, true);                        
                    }
                    else if (i == 0)
                    {
                        Vertex v1 = fullPointList[i * 2 + 1];
                        Vertex v3 = new Vertex(fullPointList[(i + 1) * 2 + 1], treadRise * up);
                        Vertex v2 = new Vertex(fullPointList[i * 2 + 1], treadRise * up + (1 - treadFraction) * (fullPointList[(i + 1) * 2 + 1].position - fullPointList[i * 2 + 1].position));
                        v1.Update(this, 0);
                        v2.Update(this, 0);
                        v3.Update(this, 0);

                        List<Vertex> subListA = new List<Vertex>();
                        subListA.Add(new Vertex(subList[0], treadRise * up)); // Left
                        subListA.Add(new Vertex(subList[1], treadRise * up + (1 - treadFraction) * (fullPointList[(i + 1) * 2 + 1].position - fullPointList[i * 2 + 1].position)));
                        subListA.Add(new Vertex(subList[2], (1 - treadFraction) * (fullPointList[(i + 1) * 2 + 1].position - fullPointList[i * 2 + 1].position)));
                        subListA.Add(new Vertex(subList[3], Vector3.Zero)); // Left
                        List<Vertex> subListB = new List<Vertex>();
                        subListB.Add(new Vertex(subList[1], treadRise * up + (1 - treadFraction) * (fullPointList[(i + 1) * 2 + 1].position - fullPointList[i * 2 + 1].position)));
                        subListB.Add(new Vertex(subList[1], Vector3.Zero));
                        subListB.Add(new Vertex(subList[2], Vector3.Zero));
                        subListB.Add(new Vertex(subList[2], (1 - treadFraction) * (fullPointList[(i + 1) * 2 + 1].position - fullPointList[i * 2 + 1].position)));

                        AddBlockFrontToTriangleList(subList, Color.White, depth + .01f, edgeSideEndTexCoords, triangleList, true);
                        AddBlockFrontToTriangleList(subListA, Color.White, depth + .011f, rubberSideSmallTexCoords, triangleList, true);
                        AddBlockFrontToTriangleList(subListB, Color.White, depth + .011f, rubberSideSmallTexCoords, triangleList, true);
                        AddTopStrip(v1, v2, depth + .011f, .00f, false, edgeTopTexCoords, triangleList);
                        AddTopStrip(v2, v3, depth + .011f, .00f, false, edgeTopTexCoords, triangleList);
                        
                    }
                    else
                    {
                        Vertex v1 = new Vertex(fullPointList[i * 2 + 1], treadRise * up);
                        Vertex v2 = new Vertex(fullPointList[(i + 1) * 2 + 1], treadRise * up);
                        v1.Update(this, 0);
                        v2.Update(this, 0);
                        List<Vertex> subListA = new List<Vertex>();
                        subListA.Add(new Vertex(subList[0], treadRise * up));
                        subListA.Add(new Vertex(subList[1], treadRise * up));
                        subListA.Add(new Vertex(subList[2], Vector3.Zero));
                        subListA.Add(new Vertex(subList[3], Vector3.Zero));


                        AddBlockFrontToTriangleList(subList, Color.White, depth + .01f, edgeSideTexCoords, triangleList, true);
                        AddBlockFrontToTriangleList(subListA, Color.White, depth + .01f, rubberSideSmallTexCoords, triangleList, true);
                        AddTopStrip(v1, v2, depth + .01f, .00f, false, edgeTopTexCoords, triangleList);
                    }
#endregion
                }
                else if (e.properties.type == VL.EdgeType.ConveyorBelt)
                {
                    #region conveyorbelt
                    float beltOffset = .0000125f * beltAnimation * e.properties.primaryValue;

                    beltOffset %= .125f;
                    if (beltOffset < 0)
                        beltOffset += .125f;

                    List<Vector2> activeBeltTopTexCoords = new List<Vector2>();
                    activeBeltTopTexCoords.Add(edgeTopTexCoords[0] + beltOffset * Vector2.UnitX);
                    activeBeltTopTexCoords.Add(edgeTopTexCoords[1] + beltOffset * Vector2.UnitX);
                    activeBeltTopTexCoords.Add(edgeTopTexCoords[2] + beltOffset * Vector2.UnitX);
                    activeBeltTopTexCoords.Add(edgeTopTexCoords[3] + beltOffset * Vector2.UnitX);
                    List<Vector2> activeBeltSideTexCoords = new List<Vector2>();
                    activeBeltSideTexCoords.Add(beltSideSmallTexCoords[0] + beltOffset * Vector2.UnitX);
                    activeBeltSideTexCoords.Add(beltSideSmallTexCoords[1] + beltOffset * Vector2.UnitX);
                    activeBeltSideTexCoords.Add(beltSideSmallTexCoords[2] + beltOffset * Vector2.UnitX);
                    activeBeltSideTexCoords.Add(beltSideSmallTexCoords[3] + beltOffset * Vector2.UnitX);

                    float treadRise = .08f;
                    float treadFraction = .8f;
                    
                    if (i == width - 1)
                    {
                        Vertex v1t = new Vertex(fullPointList[i * 2 + 1], treadRise * up);
                        Vertex v3t = fullPointList[(i + 1) * 2 + 1];
                        Vertex v2t = new Vertex(fullPointList[i * 2 + 1], treadRise * up + treadFraction * (fullPointList[(i + 1) * 2 + 1].position - fullPointList[i * 2 + 1].position));

                        List<Vertex> subListA = new List<Vertex>();
                        subListA.Add(new Vertex(subList[0], Vector3.Zero)); // Left
                        subListA.Add(new Vertex(subList[1], treadRise * up + treadFraction * (fullPointList[(i + 1) * 2 + 1].position - fullPointList[i * 2 + 1].position)));
                        subListA.Add(new Vertex(subList[2], treadFraction * (fullPointList[(i + 1) * 2 + 1].position - fullPointList[i * 2 + 1].position)));
                        subListA.Add(new Vertex(subList[3], Vector3.Zero)); // Left
                        List<Vertex> subListB = new List<Vertex>();
                        subListB.Add(new Vertex(subList[1], treadRise * up + treadFraction * (fullPointList[(i + 1) * 2 + 1].position - fullPointList[i * 2 + 1].position)));
                        subListB.Add(new Vertex(subList[1], treadRise * up));
                        subListB.Add(new Vertex(subList[2], Vector3.Zero));
                        subListB.Add(new Vertex(subList[2], treadFraction * (fullPointList[(i + 1) * 2 + 1].position - fullPointList[i * 2 + 1].position)));


                        AddTopStrip(v1t, v2t, depth + .011f, .00f, false, activeBeltTopTexCoords, triangleList);
                        AddTopStrip(v2t, v3t, depth + .011f, .00f, false, activeBeltTopTexCoords, triangleList);
                        AddBlockFrontToTriangleList(mirrorSubList, Color.White, depth + .01f, edgeSideEndTexCoords, triangleList, true);
                        AddBlockFrontToTriangleList(subListA, Color.White, depth + .011f, activeBeltSideTexCoords, triangleList, true);
                        AddBlockFrontToTriangleList(subListB, Color.White, depth + .011f, activeBeltSideTexCoords, triangleList, true);                        
                    }
                    else if (i == 0)
                    {
                        Vertex v1 = fullPointList[i * 2 + 1];
                        Vertex v3 = new Vertex(fullPointList[(i + 1) * 2 + 1], treadRise * up);
                        Vertex v2 = new Vertex(fullPointList[i * 2 + 1], treadRise * up + (1-treadFraction) * (fullPointList[(i + 1) * 2 + 1].position - fullPointList[i * 2 + 1].position));
                        v1.Update(this, 0);
                        v2.Update(this, 0);
                        v3.Update(this, 0);

                        List<Vertex> subListA = new List<Vertex>();
                        subListA.Add(new Vertex(subList[0], treadRise * up)); // Left
                        subListA.Add(new Vertex(subList[1], treadRise * up+(1 - treadFraction) * (fullPointList[(i + 1) * 2 + 1].position - fullPointList[i * 2 + 1].position)));
                        subListA.Add(new Vertex(subList[2], (1-treadFraction) * (fullPointList[(i + 1) * 2 + 1].position - fullPointList[i * 2 + 1].position)));
                        subListA.Add(new Vertex(subList[3], Vector3.Zero)); // Left
                        List<Vertex> subListB = new List<Vertex>();
                        subListB.Add(new Vertex(subList[1], treadRise * up + (1-treadFraction) * (fullPointList[(i + 1) * 2 + 1].position - fullPointList[i * 2 + 1].position)));
                        subListB.Add(new Vertex(subList[1], Vector3.Zero));
                        subListB.Add(new Vertex(subList[2], Vector3.Zero));
                        subListB.Add(new Vertex(subList[2], (1 - treadFraction) * (fullPointList[(i + 1) * 2 + 1].position - fullPointList[i * 2 + 1].position)));


                        AddTopStrip(v1, v2, depth + .011f, .00f, false, activeBeltTopTexCoords, triangleList);
                        AddTopStrip(v2, v3, depth + .011f, .00f, false, activeBeltTopTexCoords, triangleList);
                        AddBlockFrontToTriangleList(subList, Color.White, depth + .01f, edgeSideEndTexCoords, triangleList, true);
                        AddBlockFrontToTriangleList(subListA, Color.White, depth + .011f, activeBeltSideTexCoords, triangleList, true);
                        AddBlockFrontToTriangleList(subListB, Color.White, depth + .011f, activeBeltSideTexCoords, triangleList, true);                        
                    }
                    else
                    {
                        Vertex v1 = new Vertex(fullPointList[i * 2 + 1], treadRise * up);
                        Vertex v2 = new Vertex(fullPointList[(i + 1) * 2 + 1], treadRise * up);
                        v1.Update(this, 0);
                        v2.Update(this, 0);

                        List<Vertex> subListA = new List<Vertex>();
                        subListA.Add(new Vertex(subList[0], treadRise * up));
                        subListA.Add(new Vertex(subList[1], treadRise * up));
                        subListA.Add(new Vertex(subList[2], Vector3.Zero));
                        subListA.Add(new Vertex(subList[3], Vector3.Zero));

                        AddTopStrip(v1, v2, depth + .011f, .00f, false, activeBeltTopTexCoords, triangleList);
                        AddBlockFrontToTriangleList(subList, Color.White, depth + .01f, edgeSideTexCoords, triangleList, true);
                        AddBlockFrontToTriangleList(subListA, Color.White, depth + .011f, activeBeltSideTexCoords, triangleList, true);
                    }
                    #endregion
                }
                else if (e.properties.type == VL.EdgeType.Electric && e.properties.primaryValue == 0)
                {
                    #region electric off
                    if (i == width - 1)
                    {
                        AddBlockFrontToTriangleList(mirrorSubList, Color.White, depth + .01f, edgeSideEndTexCoords, triangleList, true);
                        AddTopStrip(fullPointList[(i + 1) * 2 + 1], fullPointList[i * 2 + 1], depth + .01f, .01f, true, edgeTopEndTexCoords, triangleList);
                    }
                    else if (i == 0)
                    {
                        AddBlockFrontToTriangleList(subList, Color.White, depth + .01f, edgeSideEndTexCoords, triangleList, true);
                        AddTopStrip(fullPointList[i * 2 + 1], fullPointList[(i + 1) * 2 + 1], depth + .01f, .01f, false, edgeTopEndTexCoords, triangleList);
                    }
                    else
                    {
                        AddBlockFrontToTriangleList(subList, Color.White, depth + .01f, edgeSideTexCoords, triangleList, true);
                        AddTopStrip(fullPointList[i * 2 + 1], fullPointList[(i + 1) * 2 + 1], depth + .01f, .01f, false, edgeTopTexCoords, triangleList);
                    }
                    #endregion
                }
                else if (e.properties.type == VL.EdgeType.Electric && e.properties.primaryValue != 0)
                {
                    #region electric on
                    if (i == width - 1)
                    {
                        AddBlockFrontToTriangleList(mirrorSubList, Color.White, depth + .01f, edgeSideEndTexCoords, triangleList, true);
                        AddTopStrip(fullPointList[(i + 1) * 2 + 1], fullPointList[i * 2 + 1], depth + .01f, .01f, true, edgeTopEndTexCoords, triangleList);
                    }
                    else if (i == 0)
                    {
                        AddBlockFrontToTriangleList(subList, Color.White, depth + .01f, edgeSideEndTexCoords, triangleList, true);
                        AddTopStrip(fullPointList[i * 2 + 1], fullPointList[(i + 1) * 2 + 1], depth + .01f, .01f, false, edgeTopEndTexCoords, triangleList);
                    }
                    else
                    {
                        AddBlockFrontToTriangleList(subList, Color.White, depth + .01f, edgeSideTexCoords, triangleList, true);
                        AddTopStrip(fullPointList[i * 2 + 1], fullPointList[(i + 1) * 2 + 1], depth + .01f, .01f, false, edgeTopTexCoords, triangleList);
                    }
                    #endregion
                }
                else if (e.properties.type == VL.EdgeType.Fire && e.properties.primaryValue == 0)
                {
                    #region lava off
                    if (i == width - 1)
                    {
                        AddBlockFrontToTriangleList(mirrorSubList, Color.White, depth + .01f, edgeSideEndTexCoords, triangleList, true);
                        AddTopStrip(fullPointList[(i + 1) * 2 + 1], fullPointList[i * 2 + 1], depth + .01f, .01f, true, edgeTopEndTexCoords, triangleList);
                    }
                    else if (i == 0)
                    {
                        AddBlockFrontToTriangleList(subList, Color.White, depth + .01f, edgeSideEndTexCoords, triangleList, true);
                        AddTopStrip(fullPointList[i * 2 + 1], fullPointList[(i + 1) * 2 + 1], depth + .01f, .01f, false, edgeTopEndTexCoords, triangleList);
                    }
                    else
                    {
                        AddBlockFrontToTriangleList(subList, Color.White, depth + .01f, edgeSideTexCoords, triangleList, true);
                        AddTopStrip(fullPointList[i * 2 + 1], fullPointList[(i + 1) * 2 + 1], depth + .01f, .01f, false, edgeTopTexCoords, triangleList);
                    }
                    #endregion
                }
                else if (e.properties.type == VL.EdgeType.Fire && e.properties.primaryValue != 0)
                {
                    #region lava on
                    if (i == width - 1)
                    {
                        AddBlockFrontToTriangleList(mirrorSubList, Color.White, depth + .01f, edgeSideEndTexCoords, triangleList, true);
                        AddTopStrip(fullPointList[(i + 1) * 2 + 1], fullPointList[i * 2 + 1], depth + .01f, .01f, true, edgeTopEndTexCoords, triangleList);
                    }
                    else if (i == 0)
                    {
                        AddBlockFrontToTriangleList(subList, Color.White, depth + .01f, edgeSideEndTexCoords, triangleList, true);
                        AddTopStrip(fullPointList[i * 2 + 1], fullPointList[(i + 1) * 2 + 1], depth + .01f, .01f, false, edgeTopEndTexCoords, triangleList);
                    }
                    else
                    {
                        AddBlockFrontToTriangleList(subList, Color.White, depth + .01f, edgeSideTexCoords, triangleList, true);
                        AddTopStrip(fullPointList[i * 2 + 1], fullPointList[(i + 1) * 2 + 1], depth + .01f, .01f, false, edgeTopTexCoords, triangleList);
                    }
                    #endregion
                }
                else
                {
                    #region magnet
                    if (i == width - 1)
                        AddBlockFrontToTriangleList(mirrorSubList, Color.White, depth + .01f, edgeSideEndTexCoords, triangleList, true);
                    else if (i == 0)
                        AddBlockFrontToTriangleList(subList, Color.White, depth + .01f, edgeSideEndTexCoords, triangleList, true);
                    else
                        AddBlockFrontToTriangleList(subList, Color.White, depth + .01f, edgeSideTexCoords, triangleList, true);

                    AddTopStrip(fullPointList[i * 2 + 1], fullPointList[(i + 1) * 2 + 1], depth + .01f, .01f, false, edgeTopTexCoords, triangleList);
                    #endregion
                }
            }

        }

        public void AddStripToTriangleList(Edge e, float depth, List<VertexPositionColorNormalTexture> triangleList)
        {
            if (e.start.normal != e.end.normal)
            {
                Vector3 fullEdge = e.end.position - e.start.position;
                Vector3 currentComponent = Vector3.Dot(e.end.normal, fullEdge) * e.end.normal;
                Vector3 nextComponent = Vector3.Dot(e.start.normal, fullEdge) * e.start.normal;
                Vector3 constantComponent = Vector3.Dot(Vector3.Cross(e.end.normal, e.start.normal), fullEdge) * Vector3.Cross(e.end.normal, e.start.normal);
                float currentPercent = currentComponent.Length() / (currentComponent.Length() + nextComponent.Length());

                Vector3 midPoint = e.start.position + currentComponent + currentPercent * constantComponent;

                AddStripToTriangleListHelper(e.start, new Vertex(midPoint, e.start.normal, e.start.velocity, e.start.direction), depth, e.properties, triangleList);
                AddStripToTriangleListHelper(new Vertex(midPoint, e.end.normal, e.end.velocity, e.end.direction), e.end, depth, e.properties, triangleList);
            }
            else
                AddStripToTriangleListHelper(e.start, e.end, depth, e.properties, triangleList);

        }

        public void AddSpikesToTriangleList(Edge e, float depth, List<VertexPositionColorNormalTexture> triangeList)
        {
            Vector3 edgeDir = e.end.position - e.start.position;
            int numSpikes = 2 * (int)edgeDir.Length();
            float spikeHeight = .75f;
            float spikeWidth = edgeDir.Length() / numSpikes;
            Vector3 edgeNormal = Vector3.Cross(e.end.position - e.start.position, e.start.normal);
            edgeNormal.Normalize();
            edgeDir.Normalize();
            
            Color spikeColor = new Color(180,180,180);

            //if (e.start.velocity != Vector3.Zero)
            //{
              //  AddStripToTriangleList(e, depth, triangeList);
            //}
            //else
            //{
                Color shadedColor = Color.Blue;
                for (int i = 0; i < numSpikes; i++)
                {
                    Vector3 spikeStart = e.start.position + i * spikeWidth * edgeDir;
                    Vector3 spikeEnd = e.start.position + (i + 1) * spikeWidth * edgeDir;
                    Vector3 spikePoint = .5f * (spikeStart + spikeEnd) + spikeHeight * edgeNormal;

                    shadedColor = FakeShader.Shade(spikeColor, e.start.normal);
                    triangeList.Add(GenerateTexturedVertex(spikeStart, blankTexCoords[0], shadedColor, e.start.normal, depth));
                    triangeList.Add(GenerateTexturedVertex(spikeEnd, blankTexCoords[1], shadedColor, e.start.normal, depth));
                    triangeList.Add(GenerateTexturedVertex(spikePoint, blankTexCoords[2], shadedColor, e.start.normal, depth / 2));

                    shadedColor = FakeShader.Shade(spikeColor, -e.start.normal);
                    triangeList.Add(GenerateTexturedVertex(spikeStart, blankTexCoords[0], shadedColor, -e.start.normal, 0));
                    triangeList.Add(GenerateTexturedVertex(spikeEnd, blankTexCoords[1], shadedColor, -e.start.normal, 0));
                    triangeList.Add(GenerateTexturedVertex(spikePoint, blankTexCoords[2], shadedColor, -e.start.normal, depth / 2));

                    shadedColor = FakeShader.Shade(spikeColor, edgeDir);
                    triangeList.Add(GenerateTexturedVertex(spikeStart, blankTexCoords[0], shadedColor, edgeDir, depth));
                    triangeList.Add(GenerateTexturedVertex(spikeStart, blankTexCoords[1], shadedColor, edgeDir, 0));
                    triangeList.Add(GenerateTexturedVertex(spikePoint, blankTexCoords[2], shadedColor, edgeDir, depth / 2));

                    shadedColor = FakeShader.Shade(spikeColor, -edgeDir);
                    triangeList.Add(GenerateTexturedVertex(spikeEnd, blankTexCoords[0], shadedColor, -edgeDir, depth));
                    triangeList.Add(GenerateTexturedVertex(spikeEnd, blankTexCoords[1], shadedColor, -edgeDir, 0));
                    triangeList.Add(GenerateTexturedVertex(spikePoint, blankTexCoords[2], shadedColor, -edgeDir, depth / 2));

                    if (i < numSpikes - 1)
                    {
                        spikeStart = e.start.position + (.5f + i) * spikeWidth * edgeDir;
                        spikeEnd = e.start.position + (i + 1.5f) * spikeWidth * edgeDir;
                        spikePoint = .5f * (spikeStart + spikeEnd) + spikeHeight * edgeNormal;

                        shadedColor = FakeShader.Shade(spikeColor, e.start.normal);
                        triangeList.Add(GenerateTexturedVertex(spikeStart, blankTexCoords[0], shadedColor, e.start.normal, 0));
                        triangeList.Add(GenerateTexturedVertex(spikeEnd, blankTexCoords[1], shadedColor, e.start.normal, 0));
                        triangeList.Add(GenerateTexturedVertex(spikePoint, blankTexCoords[2], shadedColor, e.start.normal, -depth / 2));

                        shadedColor = FakeShader.Shade(spikeColor, -e.start.normal);
                        triangeList.Add(GenerateTexturedVertex(spikeStart, blankTexCoords[0], shadedColor, -e.start.normal, -depth));
                        triangeList.Add(GenerateTexturedVertex(spikeEnd, blankTexCoords[1], shadedColor, -e.start.normal, -depth));
                        triangeList.Add(GenerateTexturedVertex(spikePoint, blankTexCoords[2], shadedColor, -e.start.normal, -depth / 2));

                        shadedColor = FakeShader.Shade(spikeColor, edgeDir);
                        triangeList.Add(GenerateTexturedVertex(spikeStart, blankTexCoords[0], shadedColor, edgeDir, 0));
                        triangeList.Add(GenerateTexturedVertex(spikeStart, blankTexCoords[1], shadedColor, edgeDir, -depth));
                        triangeList.Add(GenerateTexturedVertex(spikePoint, blankTexCoords[2], shadedColor, edgeDir, -depth / 2));

                        shadedColor = FakeShader.Shade(spikeColor, -edgeDir);
                        triangeList.Add(GenerateTexturedVertex(spikeEnd, blankTexCoords[0], shadedColor, -edgeDir, 0));
                        triangeList.Add(GenerateTexturedVertex(spikeEnd, blankTexCoords[1], shadedColor, -edgeDir, -depth));
                        triangeList.Add(GenerateTexturedVertex(spikePoint, blankTexCoords[2], shadedColor, -edgeDir, -depth / 2));
                    }
                }
            //}
        }

        public void AddSingleBlockSideToTriangleList(Vertex v1, Vertex v2, Color c, float depth, List<Vector2> texCoords, List<VertexPositionColorNormalTexture> triangleList)
        {
            Color shadedColor = Color.Blue;
            Color shadedColorBack = Color.Blue;
            if (v1.normal != v2.normal)
            {
                // corner edge case
                Vector3 fullEdge = v2.position - v1.position;
                Vector3 currentComponent = Vector3.Dot(v2.normal, fullEdge) * v2.normal;
                Vector3 nextComponent = Vector3.Dot(v1.normal, fullEdge) * v1.normal;
                Vector3 constantComponent = Vector3.Dot(Vector3.Cross(v2.normal, v1.normal), fullEdge) * Vector3.Cross(v2.normal, v1.normal);
                float currentPercent = currentComponent.Length() / (currentComponent.Length() + nextComponent.Length());

                Vector3 midPoint = v1.position + currentComponent + currentPercent * constantComponent;

                Vector3 edgeNormal = Vector3.Cross(midPoint - v1.position, v1.normal);
                edgeNormal.Normalize();
                shadedColor = FakeShader.Shade(c, edgeNormal);

                triangleList.Add(GenerateTexturedVertex(v1.position, texCoords[1], shadedColor, edgeNormal, depth));
                triangleList.Add(GenerateTexturedVertex(v1.position, texCoords[2], shadedColor, edgeNormal, -depth));
                triangleList.Add(GenerateTexturedVertex(midPoint, texCoords[0], shadedColor, edgeNormal, depth));

                triangleList.Add(GenerateTexturedVertex(midPoint, texCoords[0], shadedColor, edgeNormal, depth));
                triangleList.Add(GenerateTexturedVertex(v1.position, texCoords[2], shadedColor, edgeNormal, -depth));
                triangleList.Add(GenerateTexturedVertex(midPoint, texCoords[3], shadedColor, edgeNormal, -depth));

                triangleList.Add(GenerateTexturedVertex(midPoint, texCoords[1], shadedColor, edgeNormal, depth));
                triangleList.Add(GenerateTexturedVertex(midPoint, texCoords[2], shadedColor, edgeNormal, -depth));
                triangleList.Add(GenerateTexturedVertex(v2.position, texCoords[0], shadedColor, edgeNormal, depth));

                triangleList.Add(GenerateTexturedVertex(v2.position, texCoords[0], shadedColor, edgeNormal, depth));
                triangleList.Add(GenerateTexturedVertex(midPoint, texCoords[2], shadedColor, edgeNormal, -depth));
                triangleList.Add(GenerateTexturedVertex(v2.position, texCoords[3], shadedColor, edgeNormal, -depth));
            }
            else
            {

                Vector3 edgeNormal = Vector3.Cross(v2.position - v1.position, v1.normal);
                edgeNormal.Normalize();
                shadedColor = FakeShader.Shade(c, edgeNormal);
                // straight edge case
                triangleList.Add(GenerateTexturedVertex(v1.position, texCoords[1], shadedColor, edgeNormal, depth));
                triangleList.Add(GenerateTexturedVertex(v1.position, texCoords[2], shadedColor, edgeNormal, -depth));
                triangleList.Add(GenerateTexturedVertex(v2.position, texCoords[0], shadedColor, edgeNormal, depth));

                triangleList.Add(GenerateTexturedVertex(v2.position, texCoords[0], shadedColor, edgeNormal, depth));
                triangleList.Add(GenerateTexturedVertex(v1.position, texCoords[2], shadedColor, edgeNormal, -depth));
                triangleList.Add(GenerateTexturedVertex(v2.position, texCoords[3], shadedColor, edgeNormal, -depth));
            }
        }

        public void AddBlockSidesToTriangleList(List<Vertex> vList, Color c, float depth, List<Vector2> texCoords, List<VertexPositionColorNormalTexture> triangleList)
        {            
            for (int i = 0; i < 4; i++)
            {
                Vertex vs = vList[i];
                Vertex ve = vList[(i + 1) % 4];
                
                if (vs.normal != ve.normal)
                {
                    Vertex vCorner1 = new Vertex(vs.position + Vector3.Dot(vs.direction, ve.position - vs.position) * vs.direction, vs.normal, vs.velocity, vs.direction);
                    Vertex vCorner2 = new Vertex(vCorner1.position, ve.normal, ve.velocity, ve.direction);

                    float halfLength1 = (vCorner1.position - vs.position).Length();
                    if (halfLength1 < 1.5f)
                    {
                        AddSingleBlockSideToTriangleList(vs, vCorner1, c, depth, startTexCoords, triangleList);
                    }
                    else
                    {
                        Vertex v2 = new Vertex((1.5f * vs.position + (halfLength1 - 1.5f) * vCorner1.position) / halfLength1, vs.normal, Vector3.Zero, vs.direction);
                        AddSingleBlockSideToTriangleList(vs, v2, c, depth, startTexCoords, triangleList);
                        AddSingleBlockSideToTriangleList(v2, vCorner1, c, depth, midTexCoords, triangleList);
                    }
                    

                    float halfLength2 = (ve.position - vCorner2.position).Length();
                    if (halfLength2 < 1.5f)
                    {
                        AddSingleBlockSideToTriangleList(vCorner2, ve, c, depth, endTexCoords, triangleList);
                    }
                    else
                    {
                        Vertex v2 = new Vertex((1.5f * vCorner2.position + (halfLength2 - 1.5f) * ve.position) / halfLength2, vs.normal, Vector3.Zero, vs.direction);
                        AddSingleBlockSideToTriangleList(vCorner2, v2, c, depth, midTexCoords, triangleList);
                        AddSingleBlockSideToTriangleList(v2, ve, c, depth, endTexCoords, triangleList);

                    }
                    
                }                
                else if (vs.normal == ve.normal)
                {
                    float fullLength = (vs.position - ve.position).Length();
                    if (fullLength < 3)
                        AddSingleBlockSideToTriangleList(vs, ve, c, depth, texCoords, triangleList);
                    else
                    {
                        Vertex v3 = new Vertex((1.5f * vs.position + (fullLength - 1.5f) * ve.position) / fullLength, vs.normal, Vector3.Zero, vs.direction);
                        Vertex v2 = new Vertex(((fullLength - 1.5f) * vs.position + 1.5f * ve.position) / fullLength, vs.normal, Vector3.Zero, vs.direction);
                        AddSingleBlockSideToTriangleList(vs, v2, c, depth, startTexCoords, triangleList);
                        AddSingleBlockSideToTriangleList(v2, v3, c, depth, midTexCoords, triangleList);
                        AddSingleBlockSideToTriangleList(v3, ve, c, depth, endTexCoords, triangleList);
                    }
                }
            }
        }

        public void BasicAddBlockSidesToTriangleList(List<Vertex> vList, Color c, float frontDepth, List<Vector2> texCoords, List<VertexPositionColorNormalTexture> triangleList)
        {
            BasicAddBlockSidesToTriangleList(vList, c, frontDepth, frontDepth, texCoords, triangleList);
        }

        public void BasicAddBlockSidesToTriangleList(List<Vertex> vList, Color c, float frontDepth, float backDepth, List<Vector2> texCoords, List<VertexPositionColorNormalTexture> triangleList)
        {
            BasicAddBlockSidesToTriangleList(vList, c, frontDepth, backDepth, texCoords, triangleList, true);
        }

        public void BasicAddBlockSidesToTriangleList(List<Vertex> vList, Color c, float frontDepth, float backDepth, List<Vector2> texCoords, List<VertexPositionColorNormalTexture> triangleList, bool cornerStretch)
        {
            Color shadedColor = Color.Blue;
            for (int i = 0; i < 4; i++)
            {
                if (vList[i].normal != vList[(i+1)%4].normal)
                {
                    // corner edge case
                    Vector3 fullEdge = vList[(i+1)%4].position - vList[i].position;
                    Vector3 currentComponent = Vector3.Dot(vList[(i+1)%4].normal, fullEdge) * vList[(i+1)%4].normal;
                    Vector3 nextComponent = Vector3.Dot(vList[i].normal, fullEdge) * vList[i].normal;
                    Vector3 constantComponent = Vector3.Dot(Vector3.Cross(vList[(i+1)%4].normal, vList[i].normal), fullEdge) * Vector3.Cross(vList[(i+1)%4].normal, vList[i].normal);
                    float currentPercent = currentComponent.Length() / (currentComponent.Length() + nextComponent.Length());

                    Vector3 midPoint = vList[i].position + currentComponent + currentPercent * constantComponent;

                    Vector3 edgeNormal = Vector3.Cross(midPoint - vList[i].position, vList[i].normal);
                    shadedColor = FakeShader.Shade(c, edgeNormal);

                    Vector2 texCoordMidFront = (1-currentPercent) * texCoords[1] + (currentPercent) * texCoords[0];
                    Vector2 texCoordMidBack = (1-currentPercent) * texCoords[2] + (currentPercent) * texCoords[3];

                    triangleList.Add(GenerateTexturedVertex(vList[i].position, texCoords[1], shadedColor, edgeNormal, frontDepth, cornerStretch));
                    triangleList.Add(GenerateTexturedVertex(vList[i].position, texCoords[2], shadedColor, edgeNormal, -backDepth, cornerStretch));
                    triangleList.Add(GenerateTexturedVertex(midPoint, texCoordMidFront, shadedColor, edgeNormal, frontDepth, cornerStretch));

                    triangleList.Add(GenerateTexturedVertex(midPoint, texCoordMidFront, shadedColor, edgeNormal, frontDepth, cornerStretch));
                    triangleList.Add(GenerateTexturedVertex(vList[i].position, texCoords[2], shadedColor, edgeNormal, -backDepth, cornerStretch));
                    triangleList.Add(GenerateTexturedVertex(midPoint, texCoordMidBack, shadedColor, edgeNormal, -backDepth, cornerStretch));


                    triangleList.Add(GenerateTexturedVertex(midPoint, texCoordMidFront, shadedColor, edgeNormal, frontDepth, cornerStretch));
                    triangleList.Add(GenerateTexturedVertex(midPoint, texCoordMidBack, shadedColor, edgeNormal, -backDepth, cornerStretch));
                    triangleList.Add(GenerateTexturedVertex(vList[(i + 1) % 4].position, texCoords[0], shadedColor, edgeNormal, frontDepth, cornerStretch));

                    triangleList.Add(GenerateTexturedVertex(vList[(i + 1) % 4].position, texCoords[0], shadedColor, edgeNormal, frontDepth, cornerStretch));
                    triangleList.Add(GenerateTexturedVertex(midPoint, texCoordMidBack, shadedColor, edgeNormal, -backDepth, cornerStretch));
                    triangleList.Add(GenerateTexturedVertex(vList[(i + 1) % 4].position, texCoords[3], shadedColor, edgeNormal, -backDepth, cornerStretch));
                }
                else
                {
                    Vector3 edgeNormal = Vector3.Cross(vList[(i+1)%4].position - vList[i].position, vList[i].normal);
                    
                    // straight edge case
                    shadedColor = FakeShader.Shade(c, edgeNormal);
                    triangleList.Add(GenerateTexturedVertex(vList[i].position, texCoords[1], shadedColor, edgeNormal, frontDepth, cornerStretch));
                    triangleList.Add(GenerateTexturedVertex(vList[i].position, texCoords[2], shadedColor, edgeNormal, -backDepth, cornerStretch));
                    triangleList.Add(GenerateTexturedVertex(vList[(i + 1) % 4].position, texCoords[0], shadedColor, edgeNormal, frontDepth, cornerStretch));

                    triangleList.Add(GenerateTexturedVertex(vList[(i + 1) % 4].position, texCoords[0], shadedColor, edgeNormal, frontDepth, cornerStretch));
                    triangleList.Add(GenerateTexturedVertex(vList[i].position, texCoords[2], shadedColor, edgeNormal, -backDepth, cornerStretch));
                    triangleList.Add(GenerateTexturedVertex(vList[(i + 1) % 4].position, texCoords[3], shadedColor, edgeNormal, -backDepth, cornerStretch));
                }
            }
        }




        public void AddBlockToTriangleList(List<Vertex> vList, Color c, float frontDepth, List<Vector2> texCoords, List<VertexPositionColorNormalTexture> triangleList)
        {
            AddBlockToTriangleList(vList, c, frontDepth, frontDepth, texCoords, triangleList);
        }

        public void AddBlockToTriangleList(List<Vertex> vList, Color c, float frontDepth, float backDepth, List<Vector2> texCoords, List<VertexPositionColorNormalTexture> triangleList)
        {
            AddBlockToTriangleList(vList, c, frontDepth, backDepth, texCoords, triangleList, true);
        }       

        public void AddBlockToTriangleList(List<Vertex> vList, Color c, float frontDepth, float backDepth, List<Vector2> texCoords, List<VertexPositionColorNormalTexture> triangleList, bool cornerStretch)
        {
            List<Vertex> points = new List<Vertex>();
            List<Vector2> pointsTexCoords = new List<Vector2>();
            
            Color shadedColor = Color.Blue;
            Color shadedColorBack = Color.Blue;
            int jointVertexIndex = 0;
            int count = 0;

            for (int i = 0; i < 4; i++)
            {
                points.Add(vList[i]);
                pointsTexCoords.Add(texCoords[i]);
                count++;

                if (vList[i].normal != vList[(i + 1) % 4].normal)
                {
                    // corner edge case
                    Vector3 fullEdge = vList[(i + 1) % 4].position - vList[i].position;
                    Vector3 currentComponent = Vector3.Dot(vList[(i + 1) % 4].normal, fullEdge) * vList[(i + 1) % 4].normal;
                    Vector3 nextComponent = Vector3.Dot(vList[i].normal, fullEdge) * vList[i].normal;
                    Vector3 constantComponent = Vector3.Dot(Vector3.Cross(vList[(i + 1) % 4].normal, vList[i].normal), fullEdge) * Vector3.Cross(vList[(i + 1) % 4].normal, vList[i].normal);
                    float currentPercent = currentComponent.Length() / (currentComponent.Length() + nextComponent.Length());

                    Vector3 midPoint = vList[i].position + currentComponent + currentPercent * constantComponent;
                    points.Add(new Vertex(midPoint, Vector3.Zero, Vector3.Zero, Vector3.Zero));
                    pointsTexCoords.Add((1 - currentPercent) * texCoords[i] + currentPercent * texCoords[(i + 1) % 4]);
                    jointVertexIndex = count;
                    count++;
                }     
            }

            if (points.Count == 4)
            {
                shadedColor = FakeShader.Shade(c, vList[0].normal);
                triangleList.Add(GenerateTexturedVertex(vList[0].position, texCoords[0], shadedColor, vList[0].normal, frontDepth, cornerStretch));
                triangleList.Add(GenerateTexturedVertex(vList[1].position, texCoords[1], shadedColor, vList[1].normal, frontDepth, cornerStretch));
                triangleList.Add(GenerateTexturedVertex(vList[2].position, texCoords[2], shadedColor, vList[2].normal, frontDepth, cornerStretch));
                triangleList.Add(GenerateTexturedVertex(vList[0].position, texCoords[0], shadedColor, vList[0].normal, frontDepth, cornerStretch));
                triangleList.Add(GenerateTexturedVertex(vList[2].position, texCoords[2], shadedColor, vList[2].normal, frontDepth, cornerStretch));
                triangleList.Add(GenerateTexturedVertex(vList[3].position, texCoords[3], shadedColor, vList[3].normal, frontDepth, cornerStretch));

                shadedColor = FakeShader.Shade(c, -vList[0].normal);
                shadedColorBack = FakeShader.RearShade(shadedColor);


                triangleList.Add(GenerateTexturedVertex(vList[0].position, texCoords[0], shadedColorBack, vList[0].normal, -backDepth, cornerStretch));
                triangleList.Add(GenerateTexturedVertex(vList[1].position, texCoords[1], shadedColorBack, vList[1].normal, -backDepth, cornerStretch));
                triangleList.Add(GenerateTexturedVertex(vList[2].position, texCoords[2], shadedColorBack, vList[2].normal, -backDepth, cornerStretch));
                triangleList.Add(GenerateTexturedVertex(vList[0].position, texCoords[0], shadedColorBack, vList[0].normal, -backDepth, cornerStretch));
                triangleList.Add(GenerateTexturedVertex(vList[2].position, texCoords[2], shadedColorBack, vList[2].normal, -backDepth, cornerStretch));
                triangleList.Add(GenerateTexturedVertex(vList[3].position, texCoords[3], shadedColorBack, vList[3].normal, -backDepth, cornerStretch));
            }
            else
            {
                for (int i = 1; i < 5; i++)
                {
                    Vector3 normal = points[(jointVertexIndex + i) % 6].normal;
                    if (normal == Vector3.Zero)
                        normal = points[(jointVertexIndex + i + 1) % 6].normal;

                    shadedColor = FakeShader.Shade(c, normal);
                    triangleList.Add(GenerateTexturedVertex(points[jointVertexIndex].position, pointsTexCoords[jointVertexIndex], shadedColor, normal, frontDepth, cornerStretch));
                    triangleList.Add(GenerateTexturedVertex(points[(jointVertexIndex + i) % 6].position, pointsTexCoords[(jointVertexIndex + i) % 6], shadedColor, normal, frontDepth, cornerStretch));
                    triangleList.Add(GenerateTexturedVertex(points[(jointVertexIndex + i + 1) % 6].position, pointsTexCoords[(jointVertexIndex + i + 1) % 6], shadedColor, normal, frontDepth, cornerStretch));

                    shadedColor = FakeShader.Shade(c, -normal);
                    shadedColorBack = FakeShader.RearShade(shadedColor);
                    triangleList.Add(GenerateTexturedVertex(points[jointVertexIndex].position, pointsTexCoords[jointVertexIndex], shadedColorBack, normal, -backDepth, cornerStretch));
                    triangleList.Add(GenerateTexturedVertex(points[(jointVertexIndex + i) % 6].position, pointsTexCoords[(jointVertexIndex + i) % 6], shadedColorBack, normal, -backDepth, cornerStretch));
                    triangleList.Add(GenerateTexturedVertex(points[(jointVertexIndex + i + 1) % 6].position, pointsTexCoords[(jointVertexIndex + i + 1) % 6], shadedColorBack, normal, -backDepth, cornerStretch));
                }
            }

        }

        public void AddBlockFrontToTriangleList(List<Vertex> vList, Color c, float depth, List<Vector2> texCoords, List<VertexPositionColorNormalTexture> triangleList, bool outsideOnly)
        {
            List<Vertex> points = new List<Vertex>();
            List<Vector2> pointsTexCoords = new List<Vector2>();

            int jointVertexIndex = 0;
            int count = 0;
            Color shadedColor = Color.Blue;
            Color shadedColorBack = Color.Red;
            for (int i = 0; i < 4; i++)
            {
                points.Add(vList[i]);
                pointsTexCoords.Add(texCoords[i]);
                count++;
                if (vList[i].normal != vList[(i + 1) % 4].normal)
                {
                    // corner edge case
                    Vector3 fullEdge = vList[(i + 1) % 4].position - vList[i].position;
                    Vector3 currentComponent = Vector3.Dot(vList[(i + 1) % 4].normal, fullEdge) * vList[(i + 1) % 4].normal;
                    Vector3 nextComponent = Vector3.Dot(vList[i].normal, fullEdge) * vList[i].normal;
                    Vector3 constantComponent = Vector3.Dot(Vector3.Cross(vList[(i + 1) % 4].normal, vList[i].normal), fullEdge) * Vector3.Cross(vList[(i + 1) % 4].normal, vList[i].normal);
                    float currentPercent = currentComponent.Length() / (currentComponent.Length() + nextComponent.Length());

                    Vector3 midPoint = vList[i].position + currentComponent + currentPercent * constantComponent;
                    points.Add(new Vertex(midPoint, Vector3.Zero, Vector3.Zero, Vector3.Zero));
                    pointsTexCoords.Add((1 - currentPercent) * texCoords[i] + currentPercent * texCoords[(i + 1) % 4]);
                    jointVertexIndex = count;
                    count++;
                }                
            }

            if (points.Count == 4)
            {
                shadedColor = FakeShader.Shade(c, vList[0].normal);
                triangleList.Add(GenerateTexturedVertex(vList[0].position, texCoords[0], shadedColor, vList[0].normal, depth));
                triangleList.Add(GenerateTexturedVertex(vList[1].position, texCoords[1], shadedColor, vList[1].normal, depth));
                triangleList.Add(GenerateTexturedVertex(vList[2].position, texCoords[2], shadedColor, vList[2].normal, depth));
                triangleList.Add(GenerateTexturedVertex(vList[0].position, texCoords[0], shadedColor, vList[0].normal, depth));
                triangleList.Add(GenerateTexturedVertex(vList[2].position, texCoords[2], shadedColor, vList[2].normal, depth));
                triangleList.Add(GenerateTexturedVertex(vList[3].position, texCoords[3], shadedColor, vList[3].normal, depth));

                if (outsideOnly == false)
                {
                    shadedColor = FakeShader.Shade(c, -vList[0].normal);
                    shadedColorBack = FakeShader.RearShade(shadedColor);

                    triangleList.Add(GenerateTexturedVertex(vList[0].position, texCoords[0], shadedColorBack, vList[0].normal, -depth));
                    triangleList.Add(GenerateTexturedVertex(vList[1].position, texCoords[1], shadedColorBack, vList[1].normal, -depth));
                    triangleList.Add(GenerateTexturedVertex(vList[2].position, texCoords[2], shadedColorBack, vList[2].normal, -depth));
                    triangleList.Add(GenerateTexturedVertex(vList[0].position, texCoords[0], shadedColorBack, vList[0].normal, -depth));
                    triangleList.Add(GenerateTexturedVertex(vList[2].position, texCoords[2], shadedColorBack, vList[2].normal, -depth));
                    triangleList.Add(GenerateTexturedVertex(vList[3].position, texCoords[3], shadedColorBack, vList[3].normal, -depth));
                }
            }
            else
            {
                for (int i = 1; i < 5; i++)
                {
                    Vector3 normal = points[(jointVertexIndex + i) % 6].normal;
                    if (normal == Vector3.Zero)
                        normal = points[(jointVertexIndex + i + 1) % 6].normal;
                    shadedColor = FakeShader.Shade(c, normal);
                    shadedColorBack = FakeShader.RearShade(shadedColor);
                    
                    triangleList.Add(GenerateTexturedVertex(points[jointVertexIndex].position, pointsTexCoords[jointVertexIndex], shadedColor, normal, depth));
                    triangleList.Add(GenerateTexturedVertex(points[(jointVertexIndex + i) % 6].position, pointsTexCoords[(jointVertexIndex + i) % 6], shadedColor, normal, depth));
                    triangleList.Add(GenerateTexturedVertex(points[(jointVertexIndex + i + 1) % 6].position, pointsTexCoords[(jointVertexIndex + i + 1) % 6], shadedColor, normal, depth));

                    if (outsideOnly == false)
                    {
                        shadedColor = FakeShader.Shade(c, -normal);
                        shadedColorBack = FakeShader.RearShade(shadedColor);
                        triangleList.Add(GenerateTexturedVertex(points[jointVertexIndex].position, pointsTexCoords[jointVertexIndex], shadedColorBack, normal, -depth));
                        triangleList.Add(GenerateTexturedVertex(points[(jointVertexIndex + i) % 6].position, pointsTexCoords[(jointVertexIndex + i) % 6], shadedColorBack, normal, -depth));
                        triangleList.Add(GenerateTexturedVertex(points[(jointVertexIndex + i + 1) % 6].position, pointsTexCoords[(jointVertexIndex + i + 1) % 6], shadedColorBack, normal, -depth));
                    }
                }
            }

        }


        public void AddBlockToTriangleList2(List<Vertex> vList, Color c, float depth, List<VertexPositionColorNormalTexture> triangleList)
        {
            //AddBlockSidesToTriangleList(vList, c, depth, Room.blankTexCoords, triangleList);
            
            List<Vertex> points = new List<Vertex>();
            List<Vector2> pointsTexCoords = new List<Vector2>();
            
            List<Vertex> fullPointList = new List<Vertex>();
            List<Vector2> fullTexCoordsList = new List<Vector2>();

            //teal
            fullTexCoordsList.Add(new Vector2(0 * plateTexWidth, 1 * plateTexWidth));
            fullTexCoordsList.Add(new Vector2(0 * plateTexWidth, .6f * plateTexWidth));
            fullTexCoordsList.Add(new Vector2(.4f * plateTexWidth, .6f * plateTexWidth));
            fullTexCoordsList.Add(new Vector2(.4f * plateTexWidth, 1f * plateTexWidth));
            
            //red
            fullTexCoordsList.Add(new Vector2(1 * plateTexWidth, 1 * plateTexWidth));
            fullTexCoordsList.Add(new Vector2(.6f * plateTexWidth, 1f * plateTexWidth));
            fullTexCoordsList.Add(new Vector2(.6f * plateTexWidth, .6f * plateTexWidth));
            fullTexCoordsList.Add(new Vector2(1f * plateTexWidth, .6f * plateTexWidth));

            //green
            fullTexCoordsList.Add(new Vector2(1 * plateTexWidth, 0 * plateTexWidth));
            fullTexCoordsList.Add(new Vector2(1f * plateTexWidth, .4f * plateTexWidth));
            fullTexCoordsList.Add(new Vector2(.6f * plateTexWidth, .4f * plateTexWidth));
            fullTexCoordsList.Add(new Vector2(.6f * plateTexWidth, 0 * plateTexWidth));
            
            //purple
            fullTexCoordsList.Add(new Vector2(0 * plateTexWidth, 0 * plateTexWidth));
            fullTexCoordsList.Add(new Vector2(.4f * plateTexWidth, 0f * plateTexWidth));
            fullTexCoordsList.Add(new Vector2(.4f * plateTexWidth, .4f * plateTexWidth));
            fullTexCoordsList.Add(new Vector2(0 * plateTexWidth, .4f * plateTexWidth));

            for (int i = 0; i < 4; i++)
            {
                Vector3 incomingDirection = vList[(i+3)%4].direction;
                if (vList[(i + 3) % 4].normal != vList[i].normal)
                {
                    Vector3 fullEdge = vList[(i + 3) % 4].position - vList[i].position;
                    Vector3 currentComponent = Vector3.Dot(vList[(i + 3) % 4].normal, fullEdge) * vList[(i + 3) % 4].normal;
                    Vector3 nextComponent = Vector3.Dot(vList[i].normal, fullEdge) * vList[i].normal;
                    Vector3 constantComponent = Vector3.Dot(Vector3.Cross(vList[(i + 3) % 4].normal, vList[i].normal), fullEdge) * Vector3.Cross(vList[(i + 3) % 4].normal, vList[i].normal);
                    float currentPercent = currentComponent.Length() / (currentComponent.Length() + nextComponent.Length());
                    Vector3 midPoint = vList[i].position + currentComponent + currentPercent * constantComponent;
                    incomingDirection = vList[i].position - midPoint;
                    incomingDirection.Normalize();
                }                                
                fullPointList.Add(new Vertex(vList[i], Vector3.Zero));
                fullPointList.Add(new Vertex(vList[i], -.5f * incomingDirection));
                fullPointList.Add(new Vertex(vList[i], .5f * vList[i].direction - .5f * incomingDirection));
                fullPointList.Add(new Vertex(vList[i], .5f * vList[i].direction));
            }
            for (int i = 0; i < 16; i++)
            {
                fullPointList[i].Update(this, 0);
            }


            for (int i = 0; i < 4; i++)
            {
                List<Vertex> subList = new List<Vertex>();
                List<Vector2> texCoordSubList = new List<Vector2>();
                subList.Add(fullPointList[i * 4 + 0]);
                subList.Add(fullPointList[i * 4 + 1]);
                subList.Add(fullPointList[i * 4 + 2]);
                subList.Add(fullPointList[i * 4 + 3]);
                texCoordSubList.Add(fullTexCoordsList[i * 4 + 0]);
                texCoordSubList.Add(fullTexCoordsList[i * 4 + 1]);
                texCoordSubList.Add(fullTexCoordsList[i * 4 + 2]);
                texCoordSubList.Add(fullTexCoordsList[i * 4 + 3]);
                AddBlockFrontToTriangleList(subList, c, depth, texCoordSubList, triangleList, false);
            }

            for (int i = 0; i < 4; i++)
            {
                List<Vertex> subList = new List<Vertex>();
                subList.Add(fullPointList[i * 4 + 3]);
                subList.Add(fullPointList[i * 4 + 2]);
                subList.Add(fullPointList[((i+1) * 4 + 2)%16]);
                subList.Add(fullPointList[((i+1) * 4 + 1)%16]);

                List<Vector2> texCoordSubList = new List<Vector2>();
                texCoordSubList.Add(fullTexCoordsList[i * 4 + 3]);
                texCoordSubList.Add(fullTexCoordsList[i * 4 + 2]);
                texCoordSubList.Add(fullTexCoordsList[((i + 1) * 4 + 2) % 16]);
                texCoordSubList.Add(fullTexCoordsList[((i+1) * 4 + 1)%16]);
                AddBlockFrontToTriangleList(subList, c, depth, texCoordSubList, triangleList, false);
            }

            List<Vertex> centerList = new List<Vertex>();
            centerList.Add(fullPointList[2]);
            centerList.Add(fullPointList[6]);
            centerList.Add(fullPointList[10]);
            centerList.Add(fullPointList[14]);
            List<Vector2> texCoordCenterList = new List<Vector2>();
            texCoordCenterList.Add(fullTexCoordsList[2]);
            texCoordCenterList.Add(fullTexCoordsList[6]);
            texCoordCenterList.Add(fullTexCoordsList[10]);
            texCoordCenterList.Add(fullTexCoordsList[14]);
            AddBlockFrontToTriangleList(centerList, c, depth, texCoordCenterList, triangleList, false);            
        }


        public void AddTextureToTriangleList(List<Vertex> vList, Color c, float depth, List<VertexPositionColorNormalTexture> triangleList, List<Vector2> inputTexCoords, bool flipHorizontal)
        {
            List<Vector2> texCoords = new List<Vector2>();
            if (flipHorizontal)
            {
                texCoords.Add(new Vector2(inputTexCoords[1].X, inputTexCoords[0].Y));
                texCoords.Add(new Vector2(inputTexCoords[0].X, inputTexCoords[1].Y));
                texCoords.Add(new Vector2(inputTexCoords[3].X, inputTexCoords[2].Y));
                texCoords.Add(new Vector2(inputTexCoords[2].X, inputTexCoords[3].Y));
            }
            else
            {
                texCoords.Add(new Vector2(inputTexCoords[0].X, inputTexCoords[0].Y));
                texCoords.Add(new Vector2(inputTexCoords[1].X, inputTexCoords[1].Y));
                texCoords.Add(new Vector2(inputTexCoords[2].X, inputTexCoords[2].Y));
                texCoords.Add(new Vector2(inputTexCoords[3].X, inputTexCoords[3].Y));
            }
            List<Vertex> points = new List<Vertex>();
            List<Vector2> pointsTexCoords = new List<Vector2>();
            int jointVertexIndex = 0;
            int count = 0;

            for (int i = 0; i < 4; i++)
            {
                points.Add(vList[i]);
                pointsTexCoords.Add(texCoords[i]);
                count++;
                if (vList[i].normal != vList[(i + 1) % 4].normal)
                {
                    // corner edge case
                    Vector3 fullEdge = vList[(i + 1) % 4].position - vList[i].position;
                    Vector3 currentComponent = Vector3.Dot(vList[(i + 1) % 4].normal, fullEdge) * vList[(i + 1) % 4].normal;
                    Vector3 nextComponent = Vector3.Dot(vList[i].normal, fullEdge) * vList[i].normal;
                    Vector3 constantComponent = Vector3.Dot(Vector3.Cross(vList[(i + 1) % 4].normal, vList[i].normal), fullEdge) * Vector3.Cross(vList[(i + 1) % 4].normal, vList[i].normal);
                    float currentPercent = currentComponent.Length() / (currentComponent.Length() + nextComponent.Length());

                    Vector3 midPoint = vList[i].position + currentComponent + currentPercent * constantComponent;
                    points.Add(new Vertex(midPoint, Vector3.Zero, Vector3.Zero, Vector3.Zero));
                    pointsTexCoords.Add((1-currentPercent) * texCoords[i] + currentPercent * texCoords[(i + 1) % 4]);
                    jointVertexIndex = count;
                    count++;
                }
            }

            if (points.Count == 4)
            {
                triangleList.Add(GenerateTexturedVertex(vList[0].position,texCoords[0],c, vList[0].normal, depth));
                triangleList.Add(GenerateTexturedVertex(vList[1].position, texCoords[1], c, vList[1].normal, depth));
                triangleList.Add(GenerateTexturedVertex(vList[2].position, texCoords[2], c, vList[2].normal, depth));

                triangleList.Add(GenerateTexturedVertex(vList[0].position, texCoords[0], c, vList[0].normal, depth));
                triangleList.Add(GenerateTexturedVertex(vList[2].position, texCoords[2], c, vList[2].normal, depth));
                triangleList.Add(GenerateTexturedVertex(vList[3].position, texCoords[3], c, vList[3].normal, depth));

            }
            else
            {
                for (int i = 1; i < 5; i++)
                {
                    Vector3 normal = points[(jointVertexIndex + i) % 6].normal;
                    if (normal == Vector3.Zero)
                        normal = points[(jointVertexIndex + i + 1) % 6].normal;
                    triangleList.Add(GenerateTexturedVertex(points[jointVertexIndex].position, pointsTexCoords[jointVertexIndex], c, normal, depth));
                    triangleList.Add(GenerateTexturedVertex(points[(jointVertexIndex + i) % 6].position, pointsTexCoords[(jointVertexIndex + i) % 6], c, normal, depth));
                    triangleList.Add(GenerateTexturedVertex(points[(jointVertexIndex + i + 1) % 6].position, pointsTexCoords[(jointVertexIndex + i + 1) % 6], c, normal, depth));
                }
            }

        }

        public void DrawSortedSprites()
        {
            if (fullRender)
            {
                if (this == Engine.player.currentRoom || this == Engine.player.jumpRoom)
                {
                    foreach (Monster m in monsters)
                    {
                        if (m.moveType == VL.MovementType.FaceBoss)
                        {
                            m.Draw(this);
                            m.faceBoss.Render(this);
                        }
                    }
                }
                List<DepthIndex> decorationIndexer = new List<DepthIndex>();
                for (int i = 0; i < decorations.Count; i++)
                {
                    decorationIndexer.Add(new DepthIndex(decorations[i].position.position + decorations[i].depth * decorations[i].position.normal, i, DepthIndexType.Decoration));
                }
                for (int i = 0; i < doodads.Count; i++)
                {
                    decorationIndexer.Add(new DepthIndex(doodads[i].position.position + doodads[i].depth * doodads[i].position.normal, i, DepthIndexType.DoodadSprite));
                }
                for (int i = 0; i < monsters.Count; i++)
                {
                    decorationIndexer.Add(new DepthIndex(monsters[i].position.position + monsters[i].depth * monsters[i].position.normal, i, DepthIndexType.Monster));
                }
                for (int i = 0; i < projectiles.Count; i++)
                {
                    decorationIndexer.Add(new DepthIndex(projectiles[i].position.position + projectiles[i].depth * projectiles[i].position.normal, i, DepthIndexType.Projectile));
                }
                decorationIndexer.Add(new DepthIndex(Engine.player.center.position + Engine.player.depth * Engine.player.center.normal, 0, DepthIndexType.Player));
                //decorationIndexer.Sort(new DepthIndexSorter(Engine.cameraTarget - Engine.cameraPos));
                decorationIndexer.Sort(new DepthIndexSorter(-Engine.player.center.normal));
                foreach (DepthIndex d in decorationIndexer)
                {
                    if (d.type == DepthIndexType.Decoration)
                        decorations[d.index].Draw(this);
                    else if (d.type == DepthIndexType.DoodadSprite)
                        doodads[d.index].DrawSprites(this);
                    else if (d.type == DepthIndexType.Projectile && this == Engine.player.currentRoom)
                        projectiles[d.index].Draw(this);
                    else if (d.type == DepthIndexType.Player && this == Engine.player.currentRoom)
                        Engine.player.DrawTexture(Engine.playerTextureEffect);
                    else if (d.type == DepthIndexType.Monster && this == Engine.player.currentRoom)
                    {
                        monsters[d.index].Draw(this);
                        if (monsters[d.index].textureSlices != null)
                        {
                            for (int i = 0; i < monsters[d.index].textureSlices.Count; i++)
                            {
                                int index = i;
                                if (Vector3.Dot(monsters[d.index].position.normal, Engine.cameraTarget - Engine.cameraPos) >= 0)
                                    index = monsters[d.index].textureSlices.Count - i - 1;
                                Engine.playerTextureEffect.Texture = Monster.monsterTextures[(int)monsters[d.index].textureSlices[index].textureId];
                                Engine.playerTextureEffect.CurrentTechnique.Passes[0].Apply();
                                Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                                    monsters[d.index].textureSlices[index].vertexList.ToArray(), 0, monsters[d.index].textureSlices[index].vertexList.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                            }
                        }                        
                    }
                }
            }
        }
        public void DrawDecorations()
        {                    
            if(fullRender)
            {
                List<DepthIndex> decorationIndexer = new List<DepthIndex>();
                for(int i = 0; i < decorations.Count; i++)
                {
                    decorationIndexer.Add(new DepthIndex(decorations[i].position.position + decorations[i].depth * decorations[i].position.normal, i, DepthIndexType.Decoration));
                }
                for(int i = 0; i < doodads.Count; i++)
                {
                    decorationIndexer.Add(new DepthIndex(doodads[i].position.position + doodads[i].depth * doodads[i].position.normal, i, DepthIndexType.DoodadSprite));                    
                }
                //decorationIndexer.Sort(new DepthIndexSorter(Engine.cameraTarget - Engine.cameraPos));
                decorationIndexer.Sort(new DepthIndexSorter(-Engine.player.center.normal));
                foreach (DepthIndex d in decorationIndexer)
                {
                    if (d.type == DepthIndexType.Decoration)
                        decorations[d.index].Draw(this);
                    else if (d.type == DepthIndexType.DoodadSprite)
                        doodads[d.index].DrawSprites(this);
                }
            }
        }

        public void UpdateDecorations(int gameTime)
        {
            foreach (Decoration d in decorations)
            {
                d.Update(gameTime);
            }
        }

        public void DrawMonsters()
        {
            if (monsterTriangles == null)
            {
                monsterTriangles = new List<VertexPositionColorNormalTexture>[Monster.textureCount];
            }
            for (int i = 0; i < Monster.textureCount; i++)
            {
                monsterTriangles[i] = new List<VertexPositionColorNormalTexture>();
            }
            foreach (Monster m in monsters)
            {
                m.Draw(this);
                for(int i = 0; i < m.textureSlices.Count; i++)
                {
                    int index = i;
                    if(Vector3.Dot(m.position.normal, Engine.cameraTarget - Engine.cameraPos)>= 0)
                        index = m.textureSlices.Count - i - 1;
                    Engine.playerTextureEffect.Texture = Monster.monsterTextures[(int)m.textureSlices[index].textureId];
                    Engine.playerTextureEffect.CurrentTechnique.Passes[0].Apply();
                    Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                        m.textureSlices[index].vertexList.ToArray(), 0, m.textureSlices[index].vertexList.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                }
            }
            /*for (int i = 0; i < Monster.textureCount; i++)
            {
                if (monsterTriangles[i].Count > 0)
                {
                    Engine.playerTextureEffect.Texture = Monster.monsterTextures[i];
                    Engine.playerTextureEffect.CurrentTechnique.Passes[0].Apply();
                    Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                        monsterTriangles[i].ToArray(), 0, monsterTriangles[i].Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                }
            }*/
        }

        public void DrawProjectiles()
        {
            if (projectilesTriangles == null)
            {
                projectilesTriangles = new List<VertexPositionColorNormalTexture>[3];                
            }
            projectilesTriangles[(int)ProjectileType.Laser] = new List<VertexPositionColorNormalTexture>();
            projectilesTriangles[(int)ProjectileType.Plasma] = new List<VertexPositionColorNormalTexture>();
            projectilesTriangles[(int)ProjectileType.Missile] = new List<VertexPositionColorNormalTexture>();
            blastTriangles = new List<VertexPositionColorNormalTexture>();

            foreach (Projectile p in projectiles)
            {
                p.Draw(this);
            }

            if (projectilesTriangles[(int)ProjectileType.Laser].Count() > 0)
            {
                Engine.playerTextureEffect.Texture = Projectile.laserTexture;
                Engine.playerTextureEffect.CurrentTechnique.Passes[0].Apply();
                Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                    projectilesTriangles[(int)ProjectileType.Laser].ToArray(), 0, projectilesTriangles[(int)ProjectileType.Laser].Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
            }
            if (projectilesTriangles[(int)ProjectileType.Missile].Count() > 0)
            {
                Engine.playerTextureEffect.Texture = Projectile.missileTexture;
                Engine.playerTextureEffect.CurrentTechnique.Passes[0].Apply();
                Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                    projectilesTriangles[(int)ProjectileType.Missile].ToArray(), 0, projectilesTriangles[(int)ProjectileType.Missile].Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
            }
            if (projectilesTriangles[(int)ProjectileType.Plasma].Count() > 0)
            {
                Engine.playerTextureEffect.Texture = Projectile.plasmaTexture;
                Engine.playerTextureEffect.CurrentTechnique.Passes[0].Apply();
                Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                    projectilesTriangles[(int)ProjectileType.Plasma].ToArray(), 0, projectilesTriangles[(int)ProjectileType.Plasma].Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
            }
            if (blastTriangles.Count() > 0)
            {
                Engine.playerTextureEffect.Texture = Projectile.blastTexture;
                Engine.playerTextureEffect.CurrentTechnique.Passes[0].Apply();
                Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                    blastTriangles.ToArray(), 0, blastTriangles.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
            }

        }

        public void UpdateMonsters(int gameTime)
        {
            if (Engine.player.state != State.Dialog)
            {
                foreach (Monster m in monsters)
                {
                    m.Update(gameTime);
                }
                foreach (Projectile p in projectiles)
                {
                    p.Update(gameTime);
                }
            }
            for(int i = projectiles.Count()-1; i >= 0; i--)
            {
                if (projectiles[i].exploded == true)
                {
                    projectiles.Remove(projectiles[i]);
                }
            }
        }

        public List<TransparentSquare> GetMapBlock(Vector3 adjustedSize, Color blockColor)
        {
            return GetMapBlock(adjustedSize, blockColor, false);
        }

        VertexPositionColorNormalTexture[] mapDecalVertices = null;

        public void DrawMapDecal(Vector3 cameraUp, Vector3 cameraRight, Texture2D decalTexture, Vertex v, bool objective, float decalSize, Color color)
        {
            
            float iconDistance = 3f;
            float iconSize = decalSize;
            if (objective == true)
            {
                iconDistance = 5f;
                iconSize = 3f + ObjectiveControl.oscillate * 3f / ObjectiveControl.maxOscillate;
            }            
            if (WorldMap.state == ZoomState.Objectives || WorldMap.state == ZoomState.World || WorldMap.state == ZoomState.ZoomFromWorld || WorldMap.state == ZoomState.ZoomToWorld)
            {
                iconDistance *= 1f * (3 * WorldMap.worldZoomLevel);
                iconSize *= 1f + (2 * WorldMap.worldZoomLevel);
            }
            Vector3 offset = Vector3.Zero;
            

            Vector3 lowerLeft = v.position + iconSize * -cameraRight + iconSize * -cameraUp + v.normal * iconDistance;
            Vector3 lowerRight = v.position + iconSize * cameraRight + iconSize * -cameraUp + v.normal * iconDistance;
            Vector3 upperLeft = v.position + iconSize * -cameraRight + iconSize * cameraUp + v.normal * iconDistance;
            Vector3 upperRight = v.position + iconSize * cameraRight + iconSize * cameraUp + v.normal * iconDistance;

            VertexPositionColorNormalTexture[] mapDecalList = new VertexPositionColorNormalTexture[6];

            mapDecalList[0] = (new VertexPositionColorNormalTexture(lowerLeft, color, v.normal, Room.plateTexCoords[2]));
            mapDecalList[1] = (new VertexPositionColorNormalTexture(lowerRight, color, v.normal, Room.plateTexCoords[3]));
            mapDecalList[2] = (new VertexPositionColorNormalTexture(upperLeft, color, v.normal, Room.plateTexCoords[1]));

            mapDecalList[3] = (new VertexPositionColorNormalTexture(lowerRight, color, v.normal, Room.plateTexCoords[3]));
            mapDecalList[4] = (new VertexPositionColorNormalTexture(upperLeft, color, v.normal, Room.plateTexCoords[1]));
            mapDecalList[5] = (new VertexPositionColorNormalTexture(upperRight, color, v.normal, Room.plateTexCoords[0]));

            Engine.playerTextureEffect.Texture = decalTexture;
            Engine.playerTextureEffect.CurrentTechnique.Passes[0].Apply();
            Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                mapDecalList, 0, mapDecalList.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
        }

        public void DrawMapArrow()
        {
            List<VertexPositionColorNormalTexture> mapDecalList = new List<VertexPositionColorNormalTexture>();
            Vector3 cameraUp = WorldMap.cameraUp;
            Vector3 cameraRight = Vector3.Cross(cameraUp, WorldMap.cameraPosition - WorldMap.cameraTarget);
            cameraRight.Normalize();
            DrawMapDecal(cameraUp, cameraRight, Doodad.decalTextures[(int)VL.Decal.Arrow], Engine.player.center, true, 4f, Color.White);
        }

        public void DrawMapIcons()
        {
            //if (mapDecalVertices == null)
            {
                List<VertexPositionColorNormalTexture> mapDecalList = new List<VertexPositionColorNormalTexture>();
                Vector3 cameraUp = WorldMap.cameraUp;
                Vector3 cameraRight = Vector3.Cross(cameraUp, WorldMap.cameraPosition - WorldMap.cameraTarget);
                cameraRight.Normalize();

                // Objectives
                if (parentSector == Engine.sectorList[WorldMap.selectedSectorIndex] || WorldMap.state == ZoomState.World || WorldMap.state == ZoomState.Objectives)
                {
                    if (Engine.player.currentRoom == this)
                    {
                        if (Engine.player.objectiveFilter == false)
                        {
                            foreach (Vertex v in ObjectiveControl.GetObjectiveLocations())
                            {
                                DrawMapDecal(cameraUp, cameraRight, Doodad.decalTextures[(int)VL.Decal.Objective], v, true, 2f, Color.White);
                            }
                        }
                    }
                }

                if (Engine.player.stationFilter == false && parentSector == Engine.sectorList[WorldMap.selectedSectorIndex] && !(WorldMap.state == ZoomState.World || WorldMap.state == ZoomState.ZoomToWorld || WorldMap.state == ZoomState.ZoomFromWorld || WorldMap.state == ZoomState.Objectives))
                {
                    if (stationDecal != VL.Decal.Empty)
                    {
                        DrawMapDecal(cameraUp, cameraRight, Doodad.decalTextures[(int)VL.Decal.MapLabel], new Vertex(center + size/2, Engine.player.center.normal, Vector3.Zero, cameraUp), false, 5f, Color.LightGray);
                        DrawMapDecal(cameraUp, cameraRight, Doodad.decalTextures[(int)stationDecal], new Vertex(center + size/2, Engine.player.center.normal, Vector3.Zero, cameraUp), false, 5f, Color.White);
                    }
                }
                if (explored == true && parentSector == Engine.sectorList[WorldMap.selectedSectorIndex] && !(WorldMap.state == ZoomState.World || WorldMap.state == ZoomState.ZoomToWorld || WorldMap.state == ZoomState.ZoomFromWorld || WorldMap.state == ZoomState.Objectives))
                {
                    foreach (Doodad d in doodads)
                    {
                        if (d.type == VL.DoodadType.ItemStation && Engine.player.itemFilter == false)
                        {
                            if(Engine.player.upgrades[(int)d.abilityType] == true)
                                DrawMapDecal(cameraUp, cameraRight, Doodad.decalTextures[(int)VL.Decal.MapLabel], d.position, false, 4f, Color.LightGray);
                            else
                                DrawMapDecal(cameraUp, cameraRight, Doodad.decalTextures[(int)VL.Decal.MapLabel], d.position, false, 4f, Color.PaleVioletRed);
                            DrawMapDecal(cameraUp, cameraRight, Ability.GetDecal(d.abilityType), d.position, false, 4f, Color.White);
                        }
                        if (d.type == VL.DoodadType.HealthStation && Engine.player.healthFilter == false)
                        {
                            DrawMapDecal(cameraUp, cameraRight, Doodad.decalTextures[(int)VL.Decal.MapLabel], d.position, false, 4f, Color.LightGreen);
                            DrawMapDecal(cameraUp, cameraRight, Doodad.decalTextures[(int)VL.Decal.Health], d.position, false, 4f, Color.White);
                        }
                        if (d.type == VL.DoodadType.WarpStation && Engine.player.warpFilter == false)
                        {
                            DrawMapDecal(cameraUp, cameraRight, Doodad.decalTextures[(int)VL.Decal.MapLabel], d.position, false, 4f, Color.LightBlue);
                            DrawMapDecal(cameraUp, cameraRight, Doodad.decalTextures[(int)VL.Decal.Warp], d.position, false, 4f, Color.White);
                        }
                        if (d.type == VL.DoodadType.SaveStation && Engine.player.saveFilter == false)
                        {
                            DrawMapDecal(cameraUp, cameraRight, Doodad.decalTextures[(int)VL.Decal.MapLabel], d.position, false, 4f, Color.LightYellow);
                            DrawMapDecal(cameraUp, cameraRight, Doodad.decalTextures[(int)VL.Decal.Save], d.position, false, 4f, Color.White);
                        }
                    }
                }
                mapDecalVertices = mapDecalList.ToArray();                
            }
            
        }

        public List<TransparentSquare> GetMapBlockHelper(Vector3 adjustedSize, Color shellColor)
        {
            List<VertexPositionColorNormalTexture> translucentTriangleList = new List<VertexPositionColorNormalTexture>();

            translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f, .5f), shellColor, Vector3.UnitX, -.5f));
            translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, -adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f, .5f), shellColor, Vector3.UnitX, -.5f));
            translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f, .5f), shellColor, Vector3.UnitX, -.5f));

            translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f, .5f), shellColor, Vector3.UnitX, -.5f));
            translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, -adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f, .5f), shellColor, Vector3.UnitX, -.5f));
            translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, -adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f, .5f), shellColor, Vector3.UnitX, -.5f));

            translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f, .5f), shellColor, -Vector3.UnitX, -.5f));
            translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, -adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f, .5f), shellColor, -Vector3.UnitX, -.5f));
            translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f, .5f), shellColor, -Vector3.UnitX, -.5f));

            translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f, .5f), shellColor, -Vector3.UnitX, -.5f));
            translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, -adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f, .5f), shellColor, -Vector3.UnitX, -.5f));
            translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, -adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f, .5f), shellColor, -Vector3.UnitX, -.5f));

            translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f, .5f), shellColor, Vector3.UnitY, -.5f));
            translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f, .5f), shellColor, Vector3.UnitY, -.5f));
            translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f, .5f), shellColor, Vector3.UnitY, -.5f));

            translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f, .5f), shellColor, Vector3.UnitY, -.5f));
            translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f, .5f), shellColor, Vector3.UnitY, -.5f));
            translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f, .5f), shellColor, Vector3.UnitY, -.5f));

            translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, -adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f, .5f), shellColor, -Vector3.UnitY, -.5f));
            translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, -adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f, .5f), shellColor, -Vector3.UnitY, -.5f));
            translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, -adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f, .5f), shellColor, -Vector3.UnitY, -.5f));

            translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, -adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f, .5f), shellColor, -Vector3.UnitY, -.5f));
            translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, -adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f, .5f), shellColor, -Vector3.UnitY, -.5f));
            translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, -adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f, .5f), shellColor, -Vector3.UnitY, -.5f));

            translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f, .5f), shellColor, Vector3.UnitZ, -.5f));
            translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f, .5f), shellColor, Vector3.UnitZ, -.5f));
            translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, -adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f, .5f), shellColor, Vector3.UnitZ, -.5f));

            translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, -adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f, .5f), shellColor, Vector3.UnitZ, -.5f));
            translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f, .5f), shellColor, Vector3.UnitZ, -.5f));
            translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, -adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f, .5f), shellColor, Vector3.UnitZ, -.5f));

            translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f, .5f), shellColor, -Vector3.UnitZ, -.5f));
            translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f, .5f), shellColor, -Vector3.UnitZ, -.5f));
            translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, -adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f, .5f), shellColor, -Vector3.UnitZ, -.5f));

            translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, -adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f, .5f), shellColor, -Vector3.UnitZ, -.5f));
            translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f, .5f), shellColor, -Vector3.UnitZ, -.5f));
            translucentTriangleList.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, -adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f, .5f), shellColor, -Vector3.UnitZ, -.5f));
            
            foreach (Doodad d in doodads)
            {
                // Jump Gates
                if ((d.type == VL.DoodadType.JumpStation || d.type == VL.DoodadType.JumpPad || d.type == VL.DoodadType.BridgeGate) && d.targetRoom != null)
                {
                    Vector3 startLowerLeft = d.position.position + d.left + d.down;
                    Vector3 startLowerRight = d.position.position + d.right + d.down;
                    Vector3 startUpperLeft = d.position.position + d.left + d.up;
                    Vector3 startUpperRight = d.position.position + d.right + d.up;
                    
                    float roomSize = Math.Abs(Vector3.Dot(d.targetRoom.size / 2, d.position.normal));

                    float distanceToEnd = ((d.position.position - d.targetRoom.center).Length() - roomSize) / 2;
                    Vector3 endLowerLeft = d.position.position + d.left + d.down + d.position.normal * distanceToEnd;
                    Vector3 endLowerRight = d.position.position + d.right + d.down + d.position.normal * distanceToEnd;
                    Vector3 endUpperLeft = d.position.position + d.left + d.up + d.position.normal * distanceToEnd;
                    Vector3 endUpperRight = d.position.position + d.right + d.up + d.position.normal * distanceToEnd;

                    if (d.type == VL.DoodadType.BridgeGate)
                    {
                        Vector3 bridgeSrcDirection = d.targetDoodad.position.position - d.position.position;
                        bridgeSrcDirection.Normalize();

                        Vector3 bridgeSrcLeft = .5f * Vector3.Cross(d.position.normal,bridgeSrcDirection);
                        Vector3 bridgeSrcUp = .5f * Vector3.Cross(bridgeSrcDirection, bridgeSrcLeft);

                        startLowerLeft = d.position.position + bridgeSrcLeft - bridgeSrcUp;
                        startLowerRight = d.position.position - bridgeSrcLeft - bridgeSrcUp;
                        startUpperLeft = d.position.position + bridgeSrcLeft + bridgeSrcUp;
                        startUpperRight = d.position.position - bridgeSrcLeft + bridgeSrcUp;

                        distanceToEnd = (d.position.position - d.targetDoodad.position.position).Length() / 2;

                        endLowerLeft = d.position.position + bridgeSrcLeft - bridgeSrcUp + bridgeSrcDirection * distanceToEnd;
                        endLowerRight = d.position.position - bridgeSrcLeft - bridgeSrcUp + bridgeSrcDirection * distanceToEnd;
                        endUpperLeft = d.position.position + bridgeSrcLeft + bridgeSrcUp + bridgeSrcDirection * distanceToEnd;
                        endUpperRight = d.position.position - bridgeSrcLeft + bridgeSrcUp + bridgeSrcDirection * distanceToEnd;
                    }

                    translucentTriangleList.Add(new VertexPositionColorNormalTexture(startLowerLeft, shellColor, d.left, new Vector2(.5f, .5f)));
                    translucentTriangleList.Add(new VertexPositionColorNormalTexture(startUpperLeft, shellColor, d.left, new Vector2(.5f, .5f)));
                    translucentTriangleList.Add(new VertexPositionColorNormalTexture(endLowerLeft, shellColor, d.left, new Vector2(.5f, .5f)));

                    translucentTriangleList.Add(new VertexPositionColorNormalTexture(startUpperLeft, shellColor, d.left, new Vector2(.5f, .5f)));
                    translucentTriangleList.Add(new VertexPositionColorNormalTexture(endUpperLeft, shellColor, d.left, new Vector2(.5f, .5f)));
                    translucentTriangleList.Add(new VertexPositionColorNormalTexture(endLowerLeft, shellColor, d.left, new Vector2(.5f, .5f)));

                    translucentTriangleList.Add(new VertexPositionColorNormalTexture(startLowerRight, shellColor, d.right, new Vector2(.5f, .5f)));
                    translucentTriangleList.Add(new VertexPositionColorNormalTexture(startUpperRight, shellColor, d.right, new Vector2(.5f, .5f)));
                    translucentTriangleList.Add(new VertexPositionColorNormalTexture(endLowerRight, shellColor, d.right, new Vector2(.5f, .5f)));

                    translucentTriangleList.Add(new VertexPositionColorNormalTexture(startUpperRight, shellColor, d.right, new Vector2(.5f, .5f)));
                    translucentTriangleList.Add(new VertexPositionColorNormalTexture(endUpperRight, shellColor, d.right, new Vector2(.5f, .5f)));
                    translucentTriangleList.Add(new VertexPositionColorNormalTexture(endLowerRight, shellColor, d.right, new Vector2(.5f, .5f)));

                    translucentTriangleList.Add(new VertexPositionColorNormalTexture(startUpperLeft, shellColor, d.up, new Vector2(.5f, .5f)));
                    translucentTriangleList.Add(new VertexPositionColorNormalTexture(startUpperRight, shellColor, d.up, new Vector2(.5f, .5f)));
                    translucentTriangleList.Add(new VertexPositionColorNormalTexture(endUpperRight, shellColor, d.up, new Vector2(.5f, .5f)));

                    translucentTriangleList.Add(new VertexPositionColorNormalTexture(startUpperLeft, shellColor, d.up, new Vector2(.5f, .5f)));
                    translucentTriangleList.Add(new VertexPositionColorNormalTexture(endUpperLeft, shellColor, d.up, new Vector2(.5f, .5f)));
                    translucentTriangleList.Add(new VertexPositionColorNormalTexture(endUpperRight, shellColor, d.up, new Vector2(.5f, .5f)));

                    translucentTriangleList.Add(new VertexPositionColorNormalTexture(startLowerLeft, shellColor, d.down, new Vector2(.5f, .5f)));
                    translucentTriangleList.Add(new VertexPositionColorNormalTexture(startLowerRight, shellColor, d.down, new Vector2(.5f, .5f)));
                    translucentTriangleList.Add(new VertexPositionColorNormalTexture(endLowerRight, shellColor, d.down, new Vector2(.5f, .5f)));

                    translucentTriangleList.Add(new VertexPositionColorNormalTexture(startLowerLeft, shellColor, d.down, new Vector2(.5f, .5f)));
                    translucentTriangleList.Add(new VertexPositionColorNormalTexture(endLowerLeft, shellColor, d.down, new Vector2(.5f, .5f)));
                    translucentTriangleList.Add(new VertexPositionColorNormalTexture(endLowerRight, shellColor, d.down, new Vector2(.5f, .5f)));

                }
            }

            List<TransparentSquare> squareList = new List<TransparentSquare>();
            for (int i = 0; i < translucentTriangleList.Count(); i += 6)
            {
                TransparentSquare t = new TransparentSquare(translucentTriangleList[i], translucentTriangleList[i + 1], translucentTriangleList[i + 2], translucentTriangleList[i + 3], translucentTriangleList[i + 4], translucentTriangleList[i + 5]);
                squareList.Add(t);
            }
            return squareList;
        }

        public List<TransparentSquare> GetMapBlock(Vector3 adjustedSize, Color blockColor, bool highlight)
        {            
            if (WorldMap.state == ZoomState.None)
                return new List<TransparentSquare>();
            Color shellColor = blockColor;
            if (parentSector != Engine.sectorList[WorldMap.selectedSectorIndex] && WorldMap.state != ZoomState.None && !(WorldMap.state == ZoomState.World || WorldMap.state == ZoomState.ZoomToWorld || WorldMap.state == ZoomState.ZoomFromWorld || WorldMap.state == ZoomState.Objectives))
            {
                return new List<TransparentSquare>();
            }
            else if (this.sectorHighlight == true)
            {
                shellColor = Color.White;
                shellColor.A = (Byte)(200 * WorldMap.zoomLevel);
                shellColor.R = (Byte)(shellColor.R * WorldMap.zoomLevel);
                shellColor.G = (Byte)(shellColor.G * WorldMap.zoomLevel);
                shellColor.B = (Byte)(shellColor.B * WorldMap.zoomLevel);
            }
            else if (highlight == true && !(WorldMap.state == ZoomState.World || WorldMap.state == ZoomState.ZoomToWorld || WorldMap.state == ZoomState.ZoomFromWorld || WorldMap.state == ZoomState.Objectives))
            {
                shellColor.A = (Byte)(200 * WorldMap.zoomLevel / 3);
                shellColor.R = (Byte)(shellColor.R * WorldMap.zoomLevel / 3);
                shellColor.G = (Byte)(shellColor.G * WorldMap.zoomLevel / 3);
                shellColor.B = (Byte)(shellColor.B * WorldMap.zoomLevel / 3);
            }
            else if (explored == false && parentSector == Engine.sectorList[WorldMap.selectedSectorIndex])
            {

                shellColor.A = (Byte)(110);
                shellColor.R = (Byte)(30);
                shellColor.G = (Byte)(30);
                shellColor.B = (Byte)(30);
            }
            else if (explored == false && parentSector != Engine.sectorList[WorldMap.selectedSectorIndex])
            {

                shellColor.A = (Byte)(WorldMap.worldZoomLevel * 110);
                shellColor.R = (Byte)(WorldMap.worldZoomLevel * 30);
                shellColor.G = (Byte)(WorldMap.worldZoomLevel * 30);
                shellColor.B = (Byte)(WorldMap.worldZoomLevel * 30);
            }
            else if (Engine.state == EngineState.Active)
            {
                shellColor.A = 128;
                shellColor.R = (Byte)(shellColor.R / 2);
                shellColor.G = (Byte)(shellColor.G / 2);
                shellColor.B = (Byte)(shellColor.B / 2);
            }
            else if (this == Engine.player.currentRoom || this.roomHighlight == true)
            {
                shellColor.A = (Byte)(200 * WorldMap.zoomLevel / 3);
                shellColor.R = (Byte)(shellColor.R * WorldMap.zoomLevel / 3);
                shellColor.G = (Byte)(shellColor.G * WorldMap.zoomLevel / 3);
                shellColor.B = (Byte)(shellColor.B * WorldMap.zoomLevel / 3);
            }
            else
            {
                shellColor.A = (Byte)(200 * WorldMap.zoomLevel);
                shellColor.R = (Byte)(shellColor.R * WorldMap.zoomLevel);
                shellColor.G = (Byte)(shellColor.G * WorldMap.zoomLevel);
                shellColor.B = (Byte)(shellColor.B * WorldMap.zoomLevel);
            }

            if (EngineState.Active != Engine.state)
            {

                return GetMapBlockHelper(adjustedSize, shellColor);
            }
            return null;
        }

        public List<Wormhole> wormholeList;


        public bool shouldRender
        {
            get
            {
                if (WorldMap.state != ZoomState.None || adjacent == true)
                {
                    return true;
                }
                return false;
            }
        }

        public bool fullRender
        {
            get
            {
                if (roomHighlight == true || this == Engine.player.currentRoom)
                {
                    return true;
                }
                if(adjacent == true && (WorldMap.state == ZoomState.None || WorldMap.state == ZoomState.ZoomToWorld || WorldMap.state == ZoomState.ZoomFromWorld))
                    return true;
                return false;
            }
        }

        public int[] adjacencyLevel;

        public void ComputeAdjacentRoomCount()
        {
            adjacencyLevel = new int[4];
            adjacencyLevel[0] = 1;

        }

        public int CountAdjacentRooms(int depth)
        {
            
            int count = 0;
            if (adjacent == false)
                count++;
            adjacent = true;
            if(depth > 0)
            {                
                foreach (Doodad d in doodads)
                {
                    if ((d.type == VL.DoodadType.JumpPad || d.type == VL.DoodadType.JumpStation || d.type == VL.DoodadType.BridgeGate) && d.targetRoom != null)
                        count += d.targetRoom.CountAdjacentRooms(depth - 1);
                }
            }
            return count;
        }

        public void MarkAdjacentRooms(int depth)
        {
            adjacent = true;
            if (depth == 0)
                return;
            else
            {
                foreach (Doodad d in doodads)
                {
                    if (d.type == VL.DoodadType.JumpPad || d.type == VL.DoodadType.JumpStation || d.type == VL.DoodadType.BridgeGate)
                    {
                        if (d.targetRoom != null)
                            d.targetRoom.MarkAdjacentRooms(depth - 1);
                    }
                }
            }        
        }

        public void BuildWormholes()
        {
            foreach (Doodad d in doodads)
            {
                if ((d.type == VL.DoodadType.JumpStation || d.type == VL.DoodadType.JumpPad) && d.distanceToTarget != 0)
                {
                    Engine.wormholeList.Add(new Wormhole(d.position.position + d.distanceToTarget * d.position.normal / 2, d.position.direction, d.position.normal, this, d.targetRoom));
                }
            }
        }


        List<VertexPositionColorNormalTexture> translucentBoxVertices;
        List<TransparentSquare> transparentSquareList;

        public void Draw()
        {

            if (fullRender)
            {

                if (dynamicFancyPlateTriangleArray == null)
                {
                    int fancyPlateCount = 0;
                    foreach (Doodad d in doodads)
                    {
                        d.cacheOffset = fancyPlateCount;
                        fancyPlateCount += d.cacheSize;
                    }
                    dynamicFancyPlateTriangleArray = new VertexPositionColorNormalTexture[fancyPlateCount];
                }
                if (dynamicBrickTriangleArray == null)
                {
                    int fancyBrickCount = 0;
                    foreach (Doodad d in doodads)
                    {
                        d.cacheOffsetBrick = fancyBrickCount;
                        fancyBrickCount += d.cacheSizeBrick;
                    }
                    dynamicBrickTriangleArray = new VertexPositionColorNormalTexture[fancyBrickCount];
                }
                if (masterBlockArray == null)
                {
                    int[] arraySizes = new int[Block.maxWallTextureTypes];
                    foreach (Block b in blocks)
                    {
                        if (b.scales)
                        {
                            // 54 per face, 18 per side, 180 per block
                            if (b.wallType == VL.WallType.Plate || b.wallType == VL.WallType.Crate || b.wallType == VL.WallType.Cargo)
                            {
                                b.cacheOffset[(int)VL.WallType.Plate] = arraySizes[(int)b.wallType];
                                b.cacheOffset[(int)VL.WallType.FancyPlate] = arraySizes[(int)VL.WallType.FancyPlate];
                                arraySizes[(int)VL.WallType.Plate] += 108;
                                arraySizes[(int)VL.WallType.FancyPlate] += 72;
                                
                            }
                            else if (b.wallType == VL.WallType.Gearslot)
                            {
                                b.cacheOffset[(int)VL.WallType.FancyPlate] = arraySizes[(int)VL.WallType.FancyPlate];
                                b.cacheOffset[(int)VL.WallType.Gearslot] = arraySizes[(int)VL.WallType.Gearslot];
                                arraySizes[(int)VL.WallType.FancyPlate] += 108;
                                arraySizes[(int)VL.WallType.Gearslot] += 72;
                                
                            }
                            else
                            {
                                b.cacheOffset[(int)b.wallType] = arraySizes[(int)b.wallType];
                                arraySizes[(int)b.wallType] += 180;
                                
                            }
                        }
                        else
                        {
                            if (b.wallType == VL.WallType.Plate || b.wallType == VL.WallType.Crate || b.wallType == VL.WallType.Cargo)
                            {

                                b.cacheOffset[(int)b.wallType] = arraySizes[(int)b.wallType];
                                b.cacheOffset[(int)VL.WallType.FancyPlate] = arraySizes[(int)VL.WallType.FancyPlate];
                                arraySizes[(int)b.wallType] += 12;
                                arraySizes[(int)VL.WallType.FancyPlate] += 24;
                            }
                            else if (b.wallType == VL.WallType.Gearslot)
                            {                                
                                b.cacheOffset[(int)VL.WallType.FancyPlate] = arraySizes[(int)VL.WallType.FancyPlate];
                                b.cacheOffset[(int)b.wallType] = arraySizes[(int)b.wallType];
                                arraySizes[(int)VL.WallType.FancyPlate] += 12;
                                arraySizes[(int)b.wallType] += 24;
                            }
                            else
                            {                                
                                b.cacheOffset[(int)b.wallType] = arraySizes[(int)b.wallType];
                                arraySizes[(int)b.wallType] += 36;
                            }
                        }
                    }
                    masterBlockArray = new VertexPositionColorNormalTexture[Block.maxWallTextureTypes][];
                    for (int i = 0; i < Block.maxWallTextureTypes; i++)
                    {
                        masterBlockArray[i] = new VertexPositionColorNormalTexture[arraySizes[i]];
                    }
                }
                

                float cameraLineDistance = Vector3.Dot(center - Engine.player.center.position, Vector3.Normalize(Engine.player.cameraTarget - Engine.player.cameraPos));







                #region Blocks
                if (staticFancyPlate == null || refreshVertices == true)
                {
                    staticFancyPlate = new List<VertexPositionColorNormalTexture>();
                    staticCircuit = new List<VertexPositionColorNormalTexture>();
                    staticVines = new List<VertexPositionColorNormalTexture>();
                    staticPlate = new List<VertexPositionColorNormalTexture>();
                    staticCrate = new List<VertexPositionColorNormalTexture>();
                    staticCargo = new List<VertexPositionColorNormalTexture>();
                    staticCrystal = new List<VertexPositionColorNormalTexture>();
                    staticIce = new List<VertexPositionColorNormalTexture>();
                    staticCobblestone = new List<VertexPositionColorNormalTexture>();
                    staticGearslot = new List<VertexPositionColorNormalTexture>();
                    staticRings = new List<VertexPositionColorNormalTexture>();
                }
                foreach (Block b in blocks)
                {
                    List<Vertex> vList = new List<Vertex>();
                    vList.Add(b.edges[0].start);
                    vList.Add(b.edges[1].start);
                    vList.Add(b.edges[2].start);
                    vList.Add(b.edges[3].start);

                    b.UpdateVertexData(this);
                    b.Draw(this);

                }
                foreach (JumpRing j in jumpRings)
                {
                    j.Draw(this);
                }
                foreach (Tunnel t in tunnels)
                {
                    t.Draw(this);
                }

                if (fancyPlateTriangleArray == null || refreshVertices == true)
                {
                    fancyPlateTriangleArray = staticFancyPlate.ToArray();
                    circuitTriangleArray = staticCircuit.ToArray();
                    vinesTriangleArray = staticVines.ToArray();
                    plateTriangleArray = staticPlate.ToArray();
                    cargoTriangleArray = staticCargo.ToArray();
                    crateTriangleArray = staticCrate.ToArray();
                    gearslotTriangleArray = staticGearslot.ToArray();
                    crystalTriangleArray = staticCrystal.ToArray();
                    cobblestoneTriangleArray = staticCobblestone.ToArray();
                    iceTriangleArray = staticIce.ToArray();
                    ringTriangleArray = staticRings.ToArray();
                }
                if (ringTriangleArray.Count() > 0)
                {
                    Engine.playerTextureEffect.Texture = Block.fancyPlateTexture;
                    Engine.playerTextureEffect.CurrentTechnique.Passes[0].Apply();
                    Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                        ringTriangleArray, 0, ringTriangleArray.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                }

                /*if (fancyPlateTriangleArray.Count() > 0)
                {
                    Engine.playerTextureEffect.Texture = Block.fancyPlateTexture;
                    Engine.playerTextureEffect.CurrentTechnique.Passes[0].Apply();
                    Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                        fancyPlateTriangleArray, 0, fancyPlateTriangleArray.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                }
                if (vinesTriangleArray.Count() > 0)
                {
                    Engine.playerTextureEffect.Texture = Block.vineTexture;
                    Engine.playerTextureEffect.CurrentTechnique.Passes[0].Apply();
                    Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                        vinesTriangleArray, 0, vinesTriangleArray.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                }
                if (plateTriangleArray.Count() > 0)
                {
                    Engine.playerTextureEffect.Texture = Block.wallTexture;
                    Engine.playerTextureEffect.CurrentTechnique.Passes[0].Apply();
                    Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                        plateTriangleArray, 0, plateTriangleArray.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                }
                if (circuitTriangleArray.Count() > 0)
                {
                    Engine.playerTextureEffect.Texture = Block.circuitTexture;
                    Engine.playerTextureEffect.CurrentTechnique.Passes[0].Apply();
                    Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                        circuitTriangleArray, 0, circuitTriangleArray.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                }
                if (cargoTriangleArray.Count() > 0)
                {
                    Engine.playerTextureEffect.Texture = Block.cargoTexture;
                    Engine.playerTextureEffect.CurrentTechnique.Passes[0].Apply();
                    Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                        cargoTriangleArray, 0, cargoTriangleArray.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                }
                if (cobblestoneTriangleArray.Count() > 0)
                {
                    Engine.playerTextureEffect.Texture = Block.cobblestoneTexture;
                    Engine.playerTextureEffect.CurrentTechnique.Passes[0].Apply();
                    Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                        cobblestoneTriangleArray, 0, cobblestoneTriangleArray.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                }
                if (gearslotTriangleArray.Count() > 0)
                {
                    Engine.playerTextureEffect.Texture = Block.gearslotTexture;
                    Engine.playerTextureEffect.CurrentTechnique.Passes[0].Apply();
                    Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                        gearslotTriangleArray, 0, gearslotTriangleArray.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                }
                if (crateTriangleArray.Count() > 0)
                {
                    Engine.playerTextureEffect.Texture = Block.crateTexture;
                    Engine.playerTextureEffect.CurrentTechnique.Passes[0].Apply();
                    Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                        crateTriangleArray, 0, crateTriangleArray.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                }
                if (iceTriangleArray.Count() > 0)
                {
                    Engine.playerTextureEffect.Texture = Block.iceTexture;
                    Engine.playerTextureEffect.CurrentTechnique.Passes[0].Apply();
                    Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                        iceTriangleArray, 0, iceTriangleArray.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                }
                if (crystalTriangleArray.Count() > 0)
                {
                    Engine.playerTextureEffect.Texture = Block.crystalTexture;
                    Engine.playerTextureEffect.CurrentTechnique.Passes[0].Apply();
                    Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                        crystalTriangleArray, 0, crystalTriangleArray.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                }*/
                for (int i = 0; i < Block.maxWallTextureTypes; i++)
                {
                    if (masterBlockArray[i].Count() > 0)
                    {
                        Engine.playerTextureEffect.Texture = Block.masterTextureList[i];
                        Engine.playerTextureEffect.CurrentTechnique.Passes[0].Apply();
                        Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                            masterBlockArray[i], 0, masterBlockArray[i].Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                    }
                }


                foreach (Block b in blocks)
                {
                    foreach (Edge e in b.edges)
                    {
                        e.UpdateVertexData(this, !b.staticObject);
                        e.Draw(this);
                    }

                }
                #endregion


                #region Doodads

                dynamicFancyPlate = new List<VertexPositionColorNormalTexture>();
                dynamicBrick = new List<VertexPositionColorNormalTexture>();
                dynamicPlate = new List<VertexPositionColorNormalTexture>();

                foreach (Doodad d in doodads)
                {
                    d.DrawSolids(this);
                }



                if (dynamicFancyPlateTriangleArray.Count() > 0)
                {
                    Engine.playerTextureEffect.Texture = Block.fancyPlateTexture;
                    Engine.playerTextureEffect.CurrentTechnique.Passes[0].Apply();
                    Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                        dynamicFancyPlateTriangleArray, 0, dynamicFancyPlateTriangleArray.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                }
                if (dynamicBrickTriangleArray.Count() > 0)
                {
                    Engine.playerTextureEffect.Texture = Block.crackedTexture;
                    Engine.playerTextureEffect.CurrentTechnique.Passes[0].Apply();
                    Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                        dynamicBrickTriangleArray, 0, dynamicBrickTriangleArray.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
                }


                foreach (Doodad d in doodads)
                {
                    d.DrawDecals(this);
                }

                #endregion
            }

            #region innerBlock
            Color interiorColor = new Color(20, 20, 20);

            if (innerBlockMode == 2)
                interiorColor.A = 90;
            Vector3 adjustedSize = new Vector3(size.X - .1f, size.Y - .1f, size.Z - .1f);
            if (translucentBoxVertices == null)
            {
                translucentBoxVertices = new List<VertexPositionColorNormalTexture>();
                translucentBoxVertices.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f, .5f), interiorColor, Vector3.UnitX, -.5f));
                translucentBoxVertices.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, -adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f, .5f), interiorColor, Vector3.UnitX, -.5f));
                translucentBoxVertices.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f, .5f), interiorColor, Vector3.UnitX, -.5f));

                translucentBoxVertices.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f, .5f), interiorColor, Vector3.UnitX, -.5f));
                translucentBoxVertices.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, -adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f, .5f), interiorColor, Vector3.UnitX, -.5f));
                translucentBoxVertices.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, -adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f, .5f), interiorColor, Vector3.UnitX, -.5f));

                translucentBoxVertices.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f, .5f), interiorColor, -Vector3.UnitX, -.5f));
                translucentBoxVertices.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, -adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f, .5f), interiorColor, -Vector3.UnitX, -.5f));
                translucentBoxVertices.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f, .5f), interiorColor, -Vector3.UnitX, -.5f));

                translucentBoxVertices.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f, .5f), interiorColor, -Vector3.UnitX, -.5f));
                translucentBoxVertices.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, -adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f, .5f), interiorColor, -Vector3.UnitX, -.5f));
                translucentBoxVertices.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, -adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f, .5f), interiorColor, -Vector3.UnitX, -.5f));

                translucentBoxVertices.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f, .5f), interiorColor, Vector3.UnitY, -.5f));
                translucentBoxVertices.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f, .5f), interiorColor, Vector3.UnitY, -.5f));
                translucentBoxVertices.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f, .5f), interiorColor, Vector3.UnitY, -.5f));

                translucentBoxVertices.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f, .5f), interiorColor, Vector3.UnitY, -.5f));
                translucentBoxVertices.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f, .5f), interiorColor, Vector3.UnitY, -.5f));
                translucentBoxVertices.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f, .5f), interiorColor, Vector3.UnitY, -.5f));

                translucentBoxVertices.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, -adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f, .5f), interiorColor, -Vector3.UnitY, -.5f));
                translucentBoxVertices.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, -adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f, .5f), interiorColor, -Vector3.UnitY, -.5f));
                translucentBoxVertices.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, -adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f, .5f), interiorColor, -Vector3.UnitY, -.5f));

                translucentBoxVertices.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, -adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f, .5f), interiorColor, -Vector3.UnitY, -.5f));
                translucentBoxVertices.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, -adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f, .5f), interiorColor, -Vector3.UnitY, -.5f));
                translucentBoxVertices.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, -adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f, .5f), interiorColor, -Vector3.UnitY, -.5f));

                translucentBoxVertices.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f, .5f), interiorColor, Vector3.UnitZ, -.5f));
                translucentBoxVertices.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f, .5f), interiorColor, Vector3.UnitZ, -.5f));
                translucentBoxVertices.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, -adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f, .5f), interiorColor, Vector3.UnitZ, -.5f));

                translucentBoxVertices.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, -adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f, .5f), interiorColor, Vector3.UnitZ, -.5f));
                translucentBoxVertices.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f, .5f), interiorColor, Vector3.UnitZ, -.5f));
                translucentBoxVertices.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, -adjustedSize.Y / 2, adjustedSize.Z / 2), new Vector2(.5f, .5f), interiorColor, Vector3.UnitZ, -.5f));

                translucentBoxVertices.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f, .5f), interiorColor, -Vector3.UnitZ, -.5f));
                translucentBoxVertices.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f, .5f), interiorColor, -Vector3.UnitZ, -.5f));
                translucentBoxVertices.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, -adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f, .5f), interiorColor, -Vector3.UnitZ, -.5f));

                translucentBoxVertices.Add(GenerateTexturedVertex(center + new Vector3(adjustedSize.X / 2, -adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f, .5f), interiorColor, -Vector3.UnitZ, -.5f));
                translucentBoxVertices.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f, .5f), interiorColor, -Vector3.UnitZ, -.5f));
                translucentBoxVertices.Add(GenerateTexturedVertex(center + new Vector3(-adjustedSize.X / 2, -adjustedSize.Y / 2, -adjustedSize.Z / 2), new Vector2(.5f, .5f), interiorColor, -Vector3.UnitZ, -.5f));

                transparentSquareList = new List<TransparentSquare>();
                for (int i = 0; i < translucentBoxVertices.Count(); i += 6)
                {
                    TransparentSquare t = new TransparentSquare(translucentBoxVertices[i], translucentBoxVertices[i + 1], translucentBoxVertices[i + 2], translucentBoxVertices[i + 3], translucentBoxVertices[i + 4], translucentBoxVertices[i + 5]);
                    transparentSquareList.Add(t);
                }
            }
            if (Engine.staticObjectsInitialized == false)
            {
                for (int i = 0; i < transparentSquareList.Count(); i++)
                {
                    Engine.staticTranslucentObjects.Add(transparentSquareList[i]);
                }
            }

            #endregion

            #region outerBlock



            Vector3 outerAdjustedSize = new Vector3(size.X + 5f, size.Y + 5f, size.Z + 5f);
            if (innerBlockMode > 0)
            {
                Engine.mapShellObjects.AddRange(GetMapBlock(outerAdjustedSize, currentColor));
            }
            #endregion

            if (refreshVertices == true)
                refreshVertices = false;
                                          
        }
        

    }
}
