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
    public enum ScaleType
    {
        None,
        Scale
    }

    public enum BlockSize
    {
        Standard,
    }

    public class Block
    {
        public bool staticObject = true;
        public bool nextBehavior;
        public float boundingBoxTop = 0;
        public float boundingBoxLeft = 0;
        public float boundingBoxRight = 0;
        public float boundingBoxBottom = 0;

        public List<Edge> edges;
        public List<Behavior> behaviors;

        public List<Block> unfoldedBlocks;

        public Behavior currentBehavior = null;
        public String currentBehaviorId;
        public int currentTime = 0;
        public Color color;
        public String id;

        public Vector3 averagePosition;

        public float length;
        public float height;
        public float area;
        public float depth;
        public VL.WallType wallType;
        public bool scales = true;

        public static Texture2D wallTexture;
        public static Texture2D circuitTexture;
        public static Texture2D cobblestoneTexture;
        public static Texture2D fancyPlateTexture;
        public static Texture2D vineTexture;
        public static Texture2D crackedTexture;
        public static Texture2D cargoTexture;
        public static Texture2D crateTexture;
        public static Texture2D iceTexture;
        public static Texture2D crystalTexture;
        public static Texture2D gearslotTexture;

        public Block()
        {
            unfoldedBlocks = new List<Block>();
            edges = new List<Edge>();
        }

        public Block(Block b)
        {
            unfoldedBlocks = new List<Block>();
            if (b.unfoldedBlocks != null)
            {
                foreach (Block oldUnfoldedBlock in b.unfoldedBlocks)
                    unfoldedBlocks.Add(new Block(oldUnfoldedBlock));
            }
            id = b.id;
            staticObject = b.staticObject;
            nextBehavior = b.nextBehavior;
            depth = b.depth;
            length = b.length;
            height = b.height;
            area = b.area;
            wallType = b.wallType;
            scales = b.scales;
            color = b.color;
            currentTime = b.currentTime;
            currentBehaviorId = b.currentBehaviorId;
            if(b.currentBehavior != null)
                currentBehaviorId = b.currentBehavior.id;
            edges = new List<Edge>();
            behaviors = new List<Behavior>();
            foreach (Edge e in b.edges)
            {
                edges.Add(new Edge(e));
            }
            if (b.behaviors != null)
            {
                foreach (Behavior behavior in b.behaviors)
                {
                    behaviors.Add(new Behavior(behavior));
                }
            }
        }

        public Block(VL.Block xmlBlock)
        {
            unfoldedBlocks = new List<Block>();
            edges = new List<Edge>();
            behaviors = new List<Behavior>();
            color = xmlBlock.color;
            id = xmlBlock.IDString;
        }

        public void Load(Bl b)
        {
            currentBehaviorId = b.cbi;
            currentTime = b.ct;
            edges[0].start = new Vertex(b.e1.vs);
            edges[0].end = new Vertex(b.e1.ve);
            edges[0].currentBehaviorId = b.e1.cbi;
            edges[0].properties.primaryValue = b.e1.pv;
            edges[0].properties.secondaryValue = b.e1.sv;
            edges[0].toggleOn = b.e1.to;
            edges[0].currentTime = b.e1.ct;
            edges[0].behaviorStarted = b.e1.bs;

            edges[1].start = new Vertex(b.e2.vs);
            edges[1].end = new Vertex(b.e2.ve);
            edges[1].currentBehaviorId = b.e2.cbi;
            edges[1].properties.primaryValue = b.e2.pv;
            edges[1].properties.secondaryValue = b.e2.sv;
            edges[1].toggleOn = b.e2.to;
            edges[1].currentTime = b.e2.ct;
            edges[1].behaviorStarted = b.e2.bs;

            edges[2].start = new Vertex(b.e3.vs);
            edges[2].end = new Vertex(b.e3.ve);
            edges[2].currentBehaviorId = b.e3.cbi;
            edges[2].properties.primaryValue = b.e3.pv;
            edges[2].properties.secondaryValue = b.e3.sv;
            edges[2].toggleOn = b.e3.to;
            edges[2].currentTime = b.e3.ct;
            edges[2].behaviorStarted = b.e3.bs;

            edges[3].start = new Vertex(b.e4.vs);
            edges[3].end = new Vertex(b.e4.ve);
            edges[3].currentBehaviorId = b.e4.cbi;
            edges[3].properties.primaryValue = b.e4.pv;
            edges[3].properties.secondaryValue = b.e4.sv;
            edges[3].toggleOn = b.e4.to;
            edges[3].currentTime = b.e4.ct;
            edges[3].behaviorStarted = b.e4.bs;
        }

        public void UpdateUnfoldedBlocks(Room r, Vector3 normal, Vector3 up)
        {
            List<Vertex> points = new List<Vertex>();
            List<EdgeProperties> edgeTypes = new List<EdgeProperties>();
            unfoldedBlocks.Clear();
            for (int i = 0; i < this.edges.Count; i++)
            {
                Edge e = this.edges[i];
                points.Add(e.start);
                edgeTypes.Add(e.properties);
                if (e.start.normal != e.end.normal && (e.start.normal != normal && e.end.normal != normal))
                {
                    Vector3 fullEdge = e.end.position - e.start.position;
                    Vector3 currentComponent = Vector3.Dot(e.end.normal, fullEdge) * e.end.normal;
                    Vector3 nextComponent = Vector3.Dot(e.start.normal, fullEdge) * e.start.normal;
                    Vector3 constantComponent = Vector3.Dot(Vector3.Cross(e.end.normal, e.start.normal), fullEdge) * Vector3.Cross(e.end.normal, e.start.normal);
                    float currentPercent = currentComponent.Length() / (currentComponent.Length() + nextComponent.Length());

                    Vector3 midPoint = e.start.position + currentComponent + currentPercent * constantComponent;
                    points.Add(new Vertex(midPoint, e.start.normal, e.start.velocity, e.start.direction));
                    edgeTypes.Add(e.properties);
                    points.Add(new Vertex(midPoint, e.end.normal, e.end.velocity, e.end.direction));
                    edgeTypes.Add(this.edges[(i + 1) % this.edges.Count].properties);

                }
            }
            if (points.Count == 4)
            {
                unfoldedBlocks.Add(new Block(points, edgeTypes, r, normal, up));
            }
            else
            {
                Vector3 n1 = points[0].normal;
                List<Vertex> vList1 = new List<Vertex>();
                List<Vertex> vList2 = new List<Vertex>();
                for (int i = 0; i < 8; i++)
                {
                    if (points[i].normal == n1)
                        vList1.Add(points[i]);
                    else
                        vList2.Add(points[i]);
                }
                unfoldedBlocks.Add(new Block(vList1, edgeTypes, r, normal, up));
                unfoldedBlocks.Add(new Block(vList2, edgeTypes, r, normal, up));
            }

            foreach (Block b in unfoldedBlocks)
                b.UpdateBoundingBox(Engine.player.up, Engine.player.right);
        }

        public void UpdateBoundingBox(Vector3 up, Vector3 right)
        {
            boundingBoxBottom = Vector3.Dot(up, edges[0].start.position);
            boundingBoxTop = Vector3.Dot(up, edges[0].start.position);
            boundingBoxLeft = Vector3.Dot(right, edges[0].start.position);
            boundingBoxRight = Vector3.Dot(right, edges[0].start.position);
            for (int i = 1; i < edges.Count; i++)
            {
                float y = Vector3.Dot(up, edges[i].start.position);
                float x = Vector3.Dot(right, edges[i].start.position);
                if (x < boundingBoxLeft)
                    boundingBoxLeft = x;                
                if (x > boundingBoxRight)
                    boundingBoxRight = x;
                if (y < boundingBoxBottom)
                    boundingBoxBottom = y;
                if (y > boundingBoxTop)
                    boundingBoxTop = y;
            }
        }

        public Block(List<Vertex> vList, List<EdgeProperties> edgePropertiesList, Room r, Vector3 n, Vector3 u)
        {
            Vector3 anchor = r.center + Math.Abs(Vector3.Dot(r.size/2,n))*n;
            edges = new List<Edge>();
            for (int i = 0; i < 4; i++)
            {                
                Edge newEdge = new Edge();
                newEdge.start = vList[i].Unfold(r, n, u);
                newEdge.end = vList[(i+1)%4].Unfold(r, n, u);
                newEdge.properties = edgePropertiesList[i];
                edges.Add(newEdge);
            }
        }

        public List<Vector3> GetCollisionRect()
        {
            List<Vector3> blockVertexList = new List<Vector3>();
            foreach (Edge e in edges)
            {
                if (e.start.position != e.end.position)
                    blockVertexList.Add(e.start.position);
            }
            return blockVertexList;
        }

        public Vector3 GetVelocity()
        {
            return edges[0].start.velocity;
        }

        public EdgeProperties GetProperties(Vector3 projection)
        {
            EdgeProperties properties = new EdgeProperties();
            properties.type = VL.EdgeType.Normal;
            foreach (Edge e in edges)
            {
                Vector3 edgeNormal = Vector3.Cross(e.start.normal, e.start.position - e.end.position);
                edgeNormal.Normalize();
                Vector3 projectionNormal = Vector3.Normalize(projection);
                float result = Vector3.Dot(edgeNormal, projectionNormal);
                if (result > .99f)
                {
                    properties = e.properties;
                }
            }
            return properties;
        }

        public int UpdateBehavior(int gameTime)
        {
            averagePosition = Vector3.Zero;
            foreach (Edge e in edges)
            {
                averagePosition += e.start.position;
            }
            averagePosition = averagePosition / 4;
            if (currentBehavior == null)
                return 0;
            if (nextBehavior == true)
            {

                foreach (Behavior b in behaviors)
                {
                    if (b.id == currentBehavior.nextBehavior)
                    {
                        if (b.duration != 0)
                        {
                            SoundFX.PlatformMove(averagePosition);
                        }
                        else if (currentBehavior.duration != 0)
                        {
                            SoundFX.PlatformStop(averagePosition);
                        }
                        currentBehavior = b;
                        break;
                    }
                }

                foreach (Edge e in edges)
                {
                    if (currentBehavior.duration != 0)
                    {
                        e.start.velocity = currentBehavior.destination / currentBehavior.duration;
                        e.end.velocity = currentBehavior.destination / currentBehavior.duration;
                    }
                    else
                    {
                        e.start.velocity = Vector3.Zero;
                        e.end.velocity = Vector3.Zero;
                    }
                }
                currentTime = gameTime;
                nextBehavior = false;
                return gameTime;
            }
            currentTime += gameTime;
            if (currentTime > currentBehavior.duration)
            {                
                nextBehavior = true;
                return currentBehavior.duration - (currentTime - gameTime);
            }
            return gameTime;
        }

        public void SetBehavior(Behavior b)
        {

            if (b.duration != 0)
            {
                SoundFX.PlatformMove(averagePosition);
            }
            else if (currentBehavior.duration != 0)
            {
                SoundFX.PlatformStop(averagePosition);
            }

            currentBehavior = b;
            
            currentTime = 0;
            nextBehavior = false;



            foreach (Edge e in edges)
            {
                if (currentBehavior.duration != 0)
                {
                    e.start.velocity = currentBehavior.destination / currentBehavior.duration;
                    e.end.velocity = currentBehavior.destination / currentBehavior.duration;
                }
                else
                {
                    e.start.velocity = Vector3.Zero;
                    e.end.velocity = Vector3.Zero;
                }
            }
        }

        public void UpdateBehavior()
        {
            if (currentBehavior == null)
            {
                currentBehavior = behaviors[0];

                if (currentBehavior.destination != Vector3.Zero)
                {
                    foreach (Edge e in edges)
                    {
                        if (currentBehavior.duration != 0)
                        {
                            e.start.velocity = currentBehavior.destination / currentBehavior.duration;
                            e.end.velocity = currentBehavior.destination / currentBehavior.duration;
                        }
                    }
                    currentTime = 0;
                }
            }
        }

        public List<VertexPositionColorNormalTexture> baseTriangleList;
        public VertexPositionColorNormalTexture[] baseTriangleArray;

        public List<VertexPositionColorNormalTexture> sideTriangleList;
        public VertexPositionColorNormalTexture[] sideTriangleArray;

        public Color GetCurrentColor(Room currentRoom)
        {
            Color powerUpColor = color;
            if (currentRoom.maxOrbs != 0)
            {
                powerUpColor.R = (Byte)(40 + currentRoom.currentOrbs * (color.R - 40) / currentRoom.maxOrbs);
                powerUpColor.G = (Byte)(40 + currentRoom.currentOrbs * (color.G - 40) / currentRoom.maxOrbs);
                powerUpColor.B = (Byte)(40 + currentRoom.currentOrbs * (color.B - 40) / currentRoom.maxOrbs);
            }
            return powerUpColor;            
        }

        public void UpdateVertexData(Room currentRoom)
        {
            
            if (baseTriangleList == null || staticObject == false || currentRoom.refreshVertices == true)
            {
                Engine.debug_blocksGenerated++;
                baseTriangleList = new List<VertexPositionColorNormalTexture>();
                sideTriangleList = new List<VertexPositionColorNormalTexture>();
            
                List<Vertex> vList = new List<Vertex>();
                vList.Add(edges[0].start);
                vList.Add(edges[1].start);
                vList.Add(edges[2].start);
                vList.Add(edges[3].start);
                Color powerUpColor = GetCurrentColor(currentRoom);
                if (scales)
                {
                    if (staticObject == false)
                    {
                        currentRoom.AddBlockToTriangleList2(vList, powerUpColor, depth, baseTriangleList);
                        currentRoom.AddBlockSidesToTriangleList(vList, powerUpColor, depth, Room.plateTexCoords, sideTriangleList);
                    }
                    else
                    {
                        if (wallType == VL.WallType.FancyPlate)
                        {
                            currentRoom.AddBlockToTriangleList2(vList, powerUpColor, depth, currentRoom.staticFancyPlate);
                            currentRoom.AddBlockSidesToTriangleList(vList, powerUpColor, depth, Room.plateTexCoords, currentRoom.staticFancyPlate);
                        }
                        else if (wallType == VL.WallType.Plate)
                        {
                            currentRoom.AddBlockToTriangleList2(vList, powerUpColor, depth, currentRoom.staticPlate);
                            currentRoom.AddBlockSidesToTriangleList(vList, powerUpColor, depth, Room.plateTexCoords, currentRoom.staticFancyPlate);
                        }
                        else if (wallType == VL.WallType.Gearslot)
                        {
                            currentRoom.AddBlockToTriangleList2(vList, powerUpColor, depth, currentRoom.staticFancyPlate);
                            currentRoom.AddBlockSidesToTriangleList(vList, powerUpColor, depth, Room.plateTexCoords, currentRoom.staticGearslot);
                        }
                        else if (wallType == VL.WallType.Vines)
                        {
                            currentRoom.AddBlockToTriangleList2(vList, powerUpColor, depth, currentRoom.staticVines);
                            currentRoom.AddBlockSidesToTriangleList(vList, powerUpColor, depth, Room.plateTexCoords, currentRoom.staticVines);
                        }
                        else if (wallType == VL.WallType.Circuit)
                        {
                            currentRoom.AddBlockToTriangleList2(vList, powerUpColor, depth, currentRoom.staticCircuit);
                            currentRoom.AddBlockSidesToTriangleList(vList, powerUpColor, depth, Room.plateTexCoords, currentRoom.staticCircuit);
                        }
                        else if (wallType == VL.WallType.Cargo)
                        {
                            currentRoom.AddBlockToTriangleList2(vList, powerUpColor, depth, currentRoom.staticCargo);
                            currentRoom.AddBlockSidesToTriangleList(vList, powerUpColor, depth, Room.plateTexCoords, currentRoom.staticFancyPlate);
                        }
                        else if (wallType == VL.WallType.Cobblestone)
                        {
                            currentRoom.AddBlockToTriangleList2(vList, powerUpColor, depth, currentRoom.staticCobblestone);
                            currentRoom.AddBlockSidesToTriangleList(vList, powerUpColor, depth, Room.plateTexCoords, currentRoom.staticCobblestone);
                        }
                        else if (wallType == VL.WallType.Crate)
                        {
                            currentRoom.AddBlockToTriangleList2(vList, powerUpColor, depth, currentRoom.staticCrate);
                            currentRoom.AddBlockSidesToTriangleList(vList, powerUpColor, depth, Room.plateTexCoords, currentRoom.staticFancyPlate);
                        }
                        else if (wallType == VL.WallType.Ice)
                        {
                            currentRoom.AddBlockToTriangleList2(vList, powerUpColor, depth, currentRoom.staticIce);
                            currentRoom.AddBlockSidesToTriangleList(vList, powerUpColor, depth, Room.plateTexCoords, currentRoom.staticIce);
                        }
                        else if (wallType == VL.WallType.Crystal)
                        {
                            currentRoom.AddBlockToTriangleList2(vList, powerUpColor, depth, currentRoom.staticCrystal);
                            currentRoom.AddBlockSidesToTriangleList(vList, powerUpColor, depth, Room.plateTexCoords, currentRoom.staticCrystal);
                        }
                        else
                        {
                            currentRoom.AddBlockToTriangleList2(vList, powerUpColor, depth, baseTriangleList);
                            currentRoom.AddBlockSidesToTriangleList(vList, powerUpColor, depth, Room.plateTexCoords, sideTriangleList);
                        }
                    }
                }
                else
                {
                    if (staticObject == false)
                    {
                        currentRoom.AddBlockToTriangleList(vList, powerUpColor, depth, Room.plateTexCoords, baseTriangleList);
                        currentRoom.BasicAddBlockSidesToTriangleList(vList, powerUpColor, depth, Room.plateTexCoords, sideTriangleList);
                    }
                    else
                    {
                        if (wallType == VL.WallType.FancyPlate)
                        {
                            currentRoom.AddBlockToTriangleList(vList, powerUpColor, depth, Room.plateTexCoords, currentRoom.staticFancyPlate);
                            currentRoom.BasicAddBlockSidesToTriangleList(vList, powerUpColor, depth, Room.plateTexCoords, currentRoom.staticFancyPlate);
                        }
                        else if (wallType == VL.WallType.Plate)
                        {
                            currentRoom.AddBlockToTriangleList(vList, powerUpColor, depth, Room.plateTexCoords, currentRoom.staticPlate);
                            currentRoom.BasicAddBlockSidesToTriangleList(vList, powerUpColor, depth, Room.plateTexCoords, currentRoom.staticFancyPlate);
                        }
                        else if (wallType == VL.WallType.Gearslot)
                        {
                            currentRoom.AddBlockToTriangleList(vList, powerUpColor, depth, Room.plateTexCoords, currentRoom.staticFancyPlate);
                            currentRoom.BasicAddBlockSidesToTriangleList(vList, powerUpColor, depth, Room.plateTexCoords, currentRoom.staticGearslot);
                        }
                        else if (wallType == VL.WallType.Vines)
                        {
                            currentRoom.AddBlockToTriangleList(vList, powerUpColor, depth, Room.plateTexCoords, currentRoom.staticVines);
                            currentRoom.BasicAddBlockSidesToTriangleList(vList, powerUpColor, depth, Room.plateTexCoords, currentRoom.staticVines);
                        }
                        else if (wallType == VL.WallType.Cobblestone)
                        {
                            currentRoom.AddBlockToTriangleList(vList, powerUpColor, depth, Room.plateTexCoords, currentRoom.staticCobblestone);
                            currentRoom.BasicAddBlockSidesToTriangleList(vList, powerUpColor, depth, Room.plateTexCoords, currentRoom.staticCobblestone);
                        }
                        else if (wallType == VL.WallType.Circuit)
                        {
                            currentRoom.AddBlockToTriangleList(vList, powerUpColor, depth, Room.plateTexCoords, currentRoom.staticCircuit);
                            currentRoom.BasicAddBlockSidesToTriangleList(vList, powerUpColor, depth, Room.plateTexCoords, currentRoom.staticCircuit);
                        }
                        else if (wallType == VL.WallType.Cargo)
                        {
                            currentRoom.AddBlockToTriangleList(vList, powerUpColor, depth, Room.plateTexCoords, currentRoom.staticCargo);
                            currentRoom.BasicAddBlockSidesToTriangleList(vList, powerUpColor, depth, Room.plateTexCoords, currentRoom.staticFancyPlate);
                        }
                        else if (wallType == VL.WallType.Crate)
                        {
                            currentRoom.AddBlockToTriangleList(vList, powerUpColor, depth, Room.plateTexCoords, currentRoom.staticCrate);
                            currentRoom.BasicAddBlockSidesToTriangleList(vList, powerUpColor, depth, Room.plateTexCoords, currentRoom.staticFancyPlate);
                        }
                        else if (wallType == VL.WallType.Ice)
                        {
                            currentRoom.AddBlockToTriangleList(vList, powerUpColor, depth, Room.plateTexCoords, currentRoom.staticIce);
                            currentRoom.BasicAddBlockSidesToTriangleList(vList, powerUpColor, depth, Room.plateTexCoords, currentRoom.staticIce);
                        }
                        else if (wallType == VL.WallType.Crystal)
                        {
                            currentRoom.AddBlockToTriangleList(vList, powerUpColor, depth, Room.plateTexCoords, currentRoom.staticCrystal);
                            currentRoom.BasicAddBlockSidesToTriangleList(vList, powerUpColor, depth, Room.plateTexCoords, currentRoom.staticCrystal);
                        }
                        else
                        {
                            currentRoom.AddBlockToTriangleList(vList, powerUpColor, depth, Room.plateTexCoords, baseTriangleList);
                            currentRoom.BasicAddBlockSidesToTriangleList(vList, powerUpColor, depth, Room.plateTexCoords, sideTriangleList);
                        }                        
                    }
                }
                if (staticObject == false)
                {
                    baseTriangleArray = baseTriangleList.ToArray();
                    sideTriangleArray = sideTriangleList.ToArray();
                }                                
            }
            if (currentRoom.refreshVertices == true && staticObject == true)
            {
                List<VertexPositionColorNormalTexture> newColorsBase = new List<VertexPositionColorNormalTexture>();
                List<VertexPositionColorNormalTexture> newColorsSide = new List<VertexPositionColorNormalTexture>();
                Color powerUpColor = GetCurrentColor(currentRoom);

                for (int i = 0; i < baseTriangleList.Count; i++)
                {
                    newColorsBase.Add(new VertexPositionColorNormalTexture(baseTriangleList[i].Position, FakeShader.Shade(powerUpColor,baseTriangleList[i].Normal) , baseTriangleList[i].Normal, baseTriangleList[i].TextureCoordinates));                    
                }
                for (int i = 0; i < sideTriangleList.Count; i++)
                {
                    newColorsSide.Add(new VertexPositionColorNormalTexture(sideTriangleList[i].Position, FakeShader.Shade(powerUpColor, sideTriangleList[i].Normal), sideTriangleList[i].Normal, sideTriangleList[i].TextureCoordinates));
                }
                baseTriangleList.Clear();
                baseTriangleList = newColorsBase;
                baseTriangleArray = baseTriangleList.ToArray();
                sideTriangleList.Clear();
                sideTriangleList = newColorsSide;
                sideTriangleArray = sideTriangleList.ToArray();
                Engine.reDraw = true;
            }
        }

        public void Draw(Room currentRoom)
        {
            UpdateVertexData(currentRoom);
            if (staticObject == true)
                return;
            if (wallType == VL.WallType.Plate)
                Engine.playerTextureEffect.Texture = wallTexture;
            else if (wallType == VL.WallType.Circuit)
                Engine.playerTextureEffect.Texture = circuitTexture;
            else if (wallType == VL.WallType.Cobblestone)
                Engine.playerTextureEffect.Texture = cobblestoneTexture;
            else if (wallType == VL.WallType.FancyPlate)
                Engine.playerTextureEffect.Texture = fancyPlateTexture;
            else if (wallType == VL.WallType.Vines)
                Engine.playerTextureEffect.Texture = vineTexture;
            else if (wallType == VL.WallType.Cargo)
                Engine.playerTextureEffect.Texture = cargoTexture;
            else if (wallType == VL.WallType.Ice)
                Engine.playerTextureEffect.Texture = iceTexture;
            else if (wallType == VL.WallType.Crystal)
                Engine.playerTextureEffect.Texture = crystalTexture;
            else if (wallType == VL.WallType.Crate)
                Engine.playerTextureEffect.Texture = crateTexture;
            else
                Engine.playerTextureEffect.Texture = fancyPlateTexture;
            Engine.playerTextureEffect.CurrentTechnique.Passes[0].Apply();
            Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                baseTriangleArray, 0, baseTriangleList.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);

            if (scales)
            {
                Engine.playerTextureEffect.Texture = fancyPlateTexture;
                Engine.playerTextureEffect.CurrentTechnique.Passes[0].Apply();
            }
            if (sideTriangleList.Count > 0)
            {
                Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                    sideTriangleArray, 0, sideTriangleList.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
            }            
        }

    
    }
}
