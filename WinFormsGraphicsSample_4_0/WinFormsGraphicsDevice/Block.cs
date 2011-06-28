using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WinFormsGraphicsDevice
{
    public class Block
    {
        public int id;
        public String name;
        public List<Edge> edges;

        public List<Behavior> behaviors;
        public Color color;

        public Block()
        {
            color = Color.Black;
            name = "Block";
            id = 0;
            edges = new List<Edge>();
            behaviors = new List<Behavior>();
        }

        public Block(Block b)
        {
            id = b.id;
            name = b.name;
            color = b.color;
            edges = new List<Edge>();
            behaviors = new List<Behavior>();
            foreach (Edge e in b.edges)
            {
                edges.Add(new Edge(e));
            }
            foreach (Behavior be in b.behaviors)
            {
                behaviors.Add(new Behavior(be));
            }
        }

        public void Init()
        {
            id = IDControl.GetID();
            foreach (Edge e in edges)
                e.Init();
            behaviors.Add(new Behavior());
        }

        public void Move(Vector3 delta)
        {
            foreach (Edge e in edges)
            {
                e.Move(delta);
            }
        }

        public String IDString
        {
            get
            {
                return name + "_" + id;
            }
        }

        public void Resize(Vector3 mousePos, Edge dragEdge, Vector3 normal)
        {
            if (MainForm.editMode == EditMode.BlockDrag)
            {
                Move(mousePos - edges[0].start);
                return;
            }

            Vector3 edgeDir = dragEdge.end - dragEdge.start;
            edgeDir.Normalize();
            Vector3 edgeNormal = Vector3.Cross(edgeDir, normal);
            int delta = (int)Vector3.Dot(edgeNormal, mousePos - dragEdge.start);
            Vector3 oldStart = dragEdge.start;
            Vector3 oldEnd = dragEdge.end;
            if (MainForm.editMode == EditMode.LineDrag)
            {
                foreach (Edge e in edges)
                {
                    if (e.start == oldStart || e.start == oldEnd)
                        e.start += delta * edgeNormal;
                    if (e.end == oldStart || e.end == oldEnd)
                        e.end += delta * edgeNormal;
                }
            }
            int newX = (int)mousePos.X;
            int newY = (int)mousePos.Y;
            int newZ = (int)mousePos.Z;
            if (MainForm.editMode == EditMode.PointDrag)
            {
                foreach (Edge e in edges)
                {
                    if (e.start == oldStart)
                    {
                        e.start = new Vector3(newX, newY, newZ);
                    }
                    if (e.end == oldStart)
                    {
                        e.end = new Vector3(newX,newY, newZ);
                    }
                    
                }
            }
        }

        public Behavior FindBehaviorByIDString(String idString)
        {
            foreach (Behavior b in behaviors)
            {
                if (b.IDString == idString)
                    return b;
            }
            return null;
        }
    }
}
