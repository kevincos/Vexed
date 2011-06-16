using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WinFormsGraphicsDevice
{
    public class Face
    {
        public Vector3 normal;        
        public Vector3 center;
        public List<Block> blocks;
        public Vector3[] vertices;

        public Face()
        {
        }

        public Face(Vector3 normal, Vector3 center)
        {
            this.normal = normal;
            this.center = center;

            blocks = new List<Block>();
            Block b = new Block();

            Vector3 xDir = Vector3.UnitX;
            if(Vector3.Cross(xDir, normal).Equals(Vector3.Zero))
               xDir = Vector3.UnitZ;
            Vector3 yDir = Vector3.Cross(normal, xDir);
            yDir.Normalize();
            xDir.Normalize();
                        
            b.edges.Add(new Edge(center,center+xDir*2));
            b.edges.Add(new Edge(center+xDir*2,center+xDir*2+yDir*2));
            b.edges.Add(new Edge(center+xDir*2+yDir*2,center+yDir*2));
            b.edges.Add(new Edge(center + yDir * 2, center));
            blocks.Add(b);
        }

        public Face(Vector3 normal, Vector3[] pointList)
        {
            this.vertices = pointList;
            this.normal = normal;
            foreach (Vector3 v in vertices)
            {
                this.center += v;
            }
            this.center = this.center / 4;

            blocks = new List<Block>();
        }

        public VertexPositionColor[] GetTemplate(Vector3 position)
        {
            Color templateColor = Color.Black;
            Block b = new Block();
            VertexPositionColor[] vList = new VertexPositionColor[8];

            Vector3 xDir = Vector3.UnitX;
            if (Vector3.Cross(xDir, normal).Equals(Vector3.Zero))
                xDir = Vector3.UnitZ;
            Vector3 yDir = Vector3.Cross(normal, xDir);
            yDir.Normalize();
            xDir.Normalize();

            b.edges.Add(new Edge(position, position + xDir));
            b.edges.Add(new Edge(position + xDir, position + xDir + yDir));
            b.edges.Add(new Edge(position + xDir + yDir, position + yDir));
            b.edges.Add(new Edge(position + yDir, position));

            if(IsBlockValid(b))
            {
                vList[0] = new VertexPositionColor(b.edges[0].start, templateColor);
                vList[1] = new VertexPositionColor(b.edges[0].end, templateColor);
                vList[2] = new VertexPositionColor(b.edges[1].start, templateColor);
                vList[3] = new VertexPositionColor(b.edges[1].end, templateColor);
                vList[4] = new VertexPositionColor(b.edges[2].start, templateColor);
                vList[5] = new VertexPositionColor(b.edges[2].end, templateColor);
                vList[6] = new VertexPositionColor(b.edges[3].start, templateColor);
                vList[7] = new VertexPositionColor(b.edges[3].end, templateColor);
                return vList;
            }
            return null;

        }

        public bool IsBlockValid(Block b)
        {
            Vector3 result;
            Vector3 prevResult = Vector3.Zero;
            foreach (Edge e in b.edges)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector3 v1 = vertices[i] - vertices[(i + 1) % 4];
                    Vector3 v2 = e.start - vertices[(i + 1) % 4];
                    result = Vector3.Cross(v1, v2);
                    if (i > 0 && Vector3.Dot(result, prevResult) < 0)
                        return false;
                    prevResult = result;
                }
            }
            return true;
        }

        public void AddBlock(Vector3 position)
        {
            Block b = new Block();

            Vector3 xDir = Vector3.UnitX;
            if (Vector3.Cross(xDir, normal).Equals(Vector3.Zero))
                xDir = Vector3.UnitZ;
            Vector3 yDir = Vector3.Cross(normal, xDir);
            yDir.Normalize();
            xDir.Normalize();

            b.edges.Add(new Edge(position, position + xDir));
            b.edges.Add(new Edge(position + xDir, position + xDir + yDir));
            b.edges.Add(new Edge(position + xDir + yDir, position + yDir));
            b.edges.Add(new Edge(position + yDir, position));

            bool blockOK = IsBlockValid(b);
            if(blockOK)
                blocks.Add(b);
        }

        public void Move(Vector3 delta)
        {
            center+=delta;
            vertices[0] += delta;
            vertices[1] += delta;
            vertices[2] += delta;
            vertices[3] += delta;
            foreach (Block b in blocks)
            {
                b.Move(delta);
            }
        }

        public void Resize(Vector3 newSize)
        {
            Vector3 xDir = Vector3.UnitX;
            if (Vector3.Cross(xDir, normal).Equals(Vector3.Zero))
                xDir = Vector3.UnitZ;
            Vector3 yDir = Vector3.Cross(normal, xDir);

            vertices[0] = center + Vector3.Dot(newSize, xDir) / 2 * xDir + Vector3.Dot(newSize, yDir) / 2 * yDir;
            vertices[1] = center + Vector3.Dot(newSize, xDir) / 2 * xDir - Vector3.Dot(newSize, yDir) / 2 * yDir;
            vertices[2] = center - Vector3.Dot(newSize, xDir) / 2 * xDir - Vector3.Dot(newSize, yDir) / 2 * yDir;
            vertices[3] = center - Vector3.Dot(newSize, xDir) / 2 * xDir + Vector3.Dot(newSize, yDir) / 2 * yDir;
            
        }
    }
}
