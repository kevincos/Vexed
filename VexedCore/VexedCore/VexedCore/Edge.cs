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
    public class EdgeProperties
    {
        public VL.EdgeType type = VL.EdgeType.Normal;
        public int primaryValue =0;
        public int secondaryValue =0;
    }

    public class Edge
    {
        public Vertex start;
        public Vertex end;

        public EdgeProperties properties;
        public List<Behavior> behaviors;
        [XmlIgnore]public Behavior currentBehavior = null;
        public String currentBehaviorId = null;
        public int currentTime = 0;
        public bool nextBehavior = false;
        public bool behaviorStarted = false;
        public bool toggleOn = true;
        public string id;
        public float edgeMod = -1f;

        public bool refreshVertices = false;

        public static Texture2D iceEdge;
        public static Texture2D magnetEdge;
        public static Texture2D lavaCoolEdge;
        public static Texture2D lavaHotEdge;
        public static Texture2D spikeEdge;

        public Edge()
        {
            start = new Vertex();
            end = new Vertex();
            properties = new EdgeProperties();
            properties.type = VL.EdgeType.Normal;
            behaviors = new List<Behavior>();
        }

        public Edge(Edge e)
        {
            id = e.id;
            start = new Vertex(e.start);
            end = new Vertex(e.end);
            properties = e.properties;
            currentBehavior = null;
            currentBehaviorId = e.currentBehaviorId;
            if(e.currentBehavior != null)
                currentBehaviorId = e.currentBehavior.id;
            currentTime = e.currentTime;
            nextBehavior = e.nextBehavior;
            behaviorStarted = e.behaviorStarted;
            toggleOn = e.toggleOn;
            behaviors = new List<Behavior>();
            foreach (Behavior b in e.behaviors)
            {
                behaviors.Add(new Behavior(b));
            }            
        }

        public Edge(VL.Edge xmlEdge, Vector3 normal)
        {
            start = new Vertex(xmlEdge.start, normal, Vector3.Zero, xmlEdge.end - xmlEdge.start);
            end = new Vertex(xmlEdge.end, normal, Vector3.Zero, xmlEdge.start - xmlEdge.end);
            
            start.direction.Normalize();
            end.direction.Normalize();
            start.normal.Normalize();
            end.normal.Normalize();

            properties = new EdgeProperties();
            properties.type = xmlEdge.type;

            behaviors = new List<Behavior>();

            id = xmlEdge.IDString;
        }

        public Edge(Vector3 s, Vector3 e, Vector3 normal)
        {
            start = new Vertex(s, normal, Vector3.Zero, e-s);
            end = new Vertex(e, normal, Vector3.Zero, s-e);

            start.direction.Normalize();
            end.direction.Normalize();
            start.normal.Normalize();
            end.normal.Normalize();

            properties = new EdgeProperties();
            properties.type = VL.EdgeType.Normal;

            behaviors = new List<Behavior>();
        }

        public int UpdateBehavior(int gameTime)
        {
            if (currentBehavior == null)
                return 0;
            if (behaviorStarted == false && currentTime > currentBehavior.offSet)
            {
                properties.primaryValue = currentBehavior.primaryValue;
                properties.secondaryValue = currentBehavior.secondaryValue;
                refreshVertices = true;
                currentTime -= currentBehavior.offSet;
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
                properties.primaryValue = currentBehavior.primaryValue;
                properties.secondaryValue = currentBehavior.secondaryValue;
                refreshVertices = true;                
                nextBehavior = false;
                return gameTime;
            }
            currentTime += gameTime;
            if (behaviorStarted)
            {
                if (currentBehavior.duration != 0 && currentTime > currentBehavior.duration)
                {
                    currentTime -= currentBehavior.duration;
                    nextBehavior = true;
                    return currentBehavior.duration - (currentTime - gameTime);
                }
                if (currentBehavior.period != 0 && currentTime > currentBehavior.period)
                {
                    currentTime -= currentBehavior.period;
                    toggleOn = !toggleOn;
                    if (!toggleOn)
                    {
                        refreshVertices = true;
                        properties.primaryValue = 0;
                        properties.secondaryValue = 0;
                    }
                    else
                    {
                        refreshVertices = true;
                        properties.primaryValue = currentBehavior.primaryValue;
                        properties.secondaryValue = currentBehavior.secondaryValue;
                    }                    
                }
            }
            return gameTime;
        }

        public void SetBehavior(Behavior b)
        {
            currentBehavior = b;
            if (!toggleOn)
            {
                properties.primaryValue = 0;
                properties.secondaryValue = 0;
                refreshVertices = true;
            }
            else
            {
                properties.primaryValue = currentBehavior.primaryValue;
                properties.secondaryValue = currentBehavior.secondaryValue;
                refreshVertices = true;
            }    
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
                    properties.primaryValue = currentBehavior.primaryValue;
                    properties.secondaryValue = currentBehavior.secondaryValue;
                    refreshVertices = true;
                    behaviorStarted = true;
                }
            }
        }


        public List<VertexPositionColorNormalTexture> baseTriangleList;
        public VertexPositionColorNormalTexture[] baseTriangleArray;

        public void UpdateVertexData(Room currentRoom, bool dynamic)
        {
            if (Engine.staticObjectsInitialized == false || baseTriangleList == null || refreshVertices == true || dynamic == true || properties.type == VL.EdgeType.ConveyorBelt)
            {
                if (edgeMod < 0f)
                {
                    Vector3 edgeDir = start.position - end.position;
                    edgeDir.Normalize();
                    Vector3 edgeOutDir = Vector3.Cross(edgeDir, end.normal);
                    edgeOutDir.Normalize();                    
                    if (Vector3.Dot(edgeOutDir, Vector3.UnitX) > .75f || Vector3.Dot(edgeOutDir, Vector3.UnitX) < -.75f)
                    {
                        edgeMod = .001f;
                    }
                    else if (Vector3.Dot(edgeOutDir, Vector3.UnitZ) > .75f || Vector3.Dot(edgeOutDir, Vector3.UnitZ) < -.75f)
                    {
                        edgeMod = .002f;
                    }
                    else
                        edgeMod = 0f;
                }

                baseTriangleList = new List<VertexPositionColorNormalTexture>();
            
                if (properties.type == VL.EdgeType.Spikes)
                    currentRoom.AddSpikesToTriangleList(this, .5f, baseTriangleList);
                else if (properties.type != VL.EdgeType.Normal)
                    currentRoom.AddStripToTriangleList2(this, .5f + edgeMod, baseTriangleList);

                baseTriangleArray = baseTriangleList.ToArray();
            }
            refreshVertices = false;
        }

        public void Draw(Room currentRoom)
        {
            if (baseTriangleArray.Length > 0)
            {

                if(properties.type == VL.EdgeType.Spikes)
                    Engine.playerTextureEffect.Texture = Edge.spikeEdge;
                if(properties.type == VL.EdgeType.Ice)
                    Engine.playerTextureEffect.Texture = Edge.iceEdge;
                if(properties.type == VL.EdgeType.Magnet)
                    Engine.playerTextureEffect.Texture = Edge.magnetEdge;
                if (properties.type == VL.EdgeType.Fire)
                {
                    if (properties.primaryValue > 0)
                        Engine.playerTextureEffect.Texture = Edge.lavaHotEdge;
                    else
                        Engine.playerTextureEffect.Texture = Edge.lavaCoolEdge;
                }
                Engine.playerTextureEffect.CurrentTechnique.Passes[0].Apply();
                Game1.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                    baseTriangleArray, 0, baseTriangleList.Count() / 3, VertexPositionColorNormalTexture.VertexDeclaration);
            }
        }
    }
}
