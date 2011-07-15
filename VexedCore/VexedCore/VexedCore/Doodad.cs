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
    public class JumpPad
    {
        public Vertex position;
        public bool active = false;

        public Room targetRoom;        

        public JumpPad(Vector3 position, Vector3 normal, Vector3 up)
        {
            this.position = new Vertex(position, normal,Vector3.Zero, up);
        }

        public void Draw(Room currentRoom, List<VertexPositionColorNormal> triangleList)
        {
            Vector3 up = Vector3.UnitX;
            if (Vector3.Dot(position.normal, up) != 0)
                up = Vector3.UnitY;

            Vector3 right = Vector3.Cross(up, position.normal);
            List<Vertex> vList = new List<Vertex>();
            vList.Add(new Vertex(position, +.5f * up + .5f * right));
            vList.Add(new Vertex(position, +.5f * up - .5f * right));
            vList.Add(new Vertex(position, -.5f * up - .5f * right));
            vList.Add(new Vertex(position, -.5f * up + .5f * right));
            if (active == true)
                currentRoom.AddBlockToTriangleList(vList, Color.Yellow, .1f, triangleList);
            else
                currentRoom.AddBlockToTriangleList(vList, Color.HotPink, .1f, triangleList);
        }
    }

    public class Bridge
    {
        public Vertex position;
        public bool active = false;
        public String id;
        public String targetId;
        public Room targetRoom;
        public Bridge targetBridge;

        public Bridge(Vector3 position, Vector3 normal, Vector3 direction, String id, String targetId)
        {
            this.id = id;
            this.targetId = targetId;
            this.position = new Vertex(position, normal, Vector3.Zero, direction);
        }

        public void Draw(Room currentRoom, List<VertexPositionColorNormal> triangleList)
        {
            Vector3 up = Vector3.UnitX;
            if (Vector3.Dot(position.normal, up) != 0)
                up = Vector3.UnitY;
            Vector3 right = Vector3.Cross(up, position.normal);
            List<Vertex> vList = new List<Vertex>();
            vList.Add(new Vertex(position, +.5f * up + .5f * right));
            vList.Add(new Vertex(position, +.5f * up - .5f * right));
            vList.Add(new Vertex(position, -.5f * up - .5f * right));
            vList.Add(new Vertex(position, -.5f * up + .5f * right));
            if (active == true)
                currentRoom.AddBlockToTriangleList(vList, Color.Blue, .1f, triangleList);
            else
                currentRoom.AddBlockToTriangleList(vList, Color.Green, .1f, triangleList);
        }
    }

    public class Doodad
    {
        public Vertex position;
        public bool active = false;
        public string id = "";
        public string targetBehavior ="";
        public string targetObject = "";
        public string expectedBehavior ="";
        public VexedLib.DoodadType type;
        public List<Behavior> behaviors;
        public Behavior currentBehavior = null;

        public int currentTime = 0;
        public bool nextBehavior = false;
        public bool behaviorStarted = false;
        public bool toggleOn = true;
        public Doodad targetDoodad = null;
        public Room targetRoom = null;        

        public Color baseColor
        {
            get
            {
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
                return .5f;
            }
        }

        public bool hasCollisionRect
        {
            get
            {
                if (type == VexedLib.DoodadType.WallSwitch || type == VexedLib.DoodadType.Checkpoint || type == VexedLib.DoodadType.PowerOrb)
                    return false;
                return true;
            }
        }

        public bool shouldRender
        {
            get
            {
                if(type == VexedLib.DoodadType.PowerOrb && active == false)
                    return false;
                return true;
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
                    if (toggleOn == true)
                        return 3 * upUnit;
                    else
                        return -.5f * upUnit;
                }
                if (type == VexedLib.DoodadType.WallSwitch)
                {
                    if (targetDoodad.currentBehavior.id == expectedBehavior)
                        return -.3f * upUnit;
                    else
                        return -.5f * upUnit;
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
                if (type == VexedLib.DoodadType.Checkpoint || type == VexedLib.DoodadType.PowerOrb)
                    return .1f;
                return .5f;
            }
        }

        public Doodad(VexedLib.Doodad xmlDoodad, Vector3 normal)
        {
            this.type = xmlDoodad.type;
            this.id = xmlDoodad.IDString;
            this.targetBehavior = xmlDoodad.targetBehavior;
            this.targetObject = xmlDoodad.targetObject;
            this.expectedBehavior = xmlDoodad.expectBehavior;            
            
            this.position = new Vertex(xmlDoodad.position, normal, Vector3.Zero, xmlDoodad.up);
            behaviors = new List<Behavior>();
            currentBehavior = null;

            if (type == VexedLib.DoodadType.PowerOrb)
                active = true;
        }

        public Doodad(VexedLib.DoodadType type, Vector3 position, Vector3 normal, Vector3 direction)
        {
            this.type = type;
            this.position = new Vertex(position, normal, Vector3.Zero, direction);
            behaviors = new List<Behavior>();
            currentBehavior = null;
        }

        public Doodad(Doodad d, Room r, Vector3 n, Vector3 u)
        {
            position = d.position.Unfold(r,n,u);
            type = d.type;
            toggleOn = d.toggleOn;
            targetDoodad = d.targetDoodad;
        }

        public void Draw(Room currentRoom, List<VertexPositionColorNormal> triangleList)
        {
            if (shouldRender == true)
            {
                List<Vertex> vList = new List<Vertex>();
                vList.Add(new Vertex(position, up + right));
                vList.Add(new Vertex(position, up + left));
                vList.Add(new Vertex(position, down + left));
                vList.Add(new Vertex(position, down + right));
                if (active)
                    currentRoom.AddBlockToTriangleList(vList, activeColor, depth, triangleList);
                else
                    currentRoom.AddBlockToTriangleList(vList, baseColor, depth, triangleList);
            }
        }




        public int UpdateBehavior(GameTime gameTime)
        {
            if (behaviorStarted == false && currentTime > currentBehavior.offSet)
            {
                //properties.primaryValue = currentBehavior.primaryValue;
                //properties.secondaryValue = currentBehavior.secondaryValue;
                toggleOn = currentBehavior.toggle;
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
                toggleOn = currentBehavior.toggle;
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
                    toggleOn = !toggleOn;
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
            toggleOn = currentBehavior.toggle;
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
                    behaviorStarted = true;
                }
            }
        }
    }
}
