using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VexedLib
{
    public class Face
    {
        public Vector3 normal;        
        public Vector3 center;
        public List<Block> blocks;
        public List<Doodad> doodads;
        public List<Monster> monsters;
        public Vector3[] vertices;

        public Face()
        {
        }

        public Face(Face f)
        {
            normal = f.normal;
            center = f.center;
            blocks = new List<Block>();
            doodads = new List<Doodad>();
            monsters = new List<Monster>();
            vertices = new Vector3[f.vertices.Length];
            for (int i = 0; i < f.vertices.Length; i++)
            {
                vertices[i] = f.vertices[i];
            }
            foreach (Monster m in f.monsters)
            {
                monsters.Add(new Monster(m));
            }
            foreach (Doodad d in f.doodads)
            {
                doodads.Add(new Doodad(d));
            }
            foreach (Block b in f.blocks)
            {
                blocks.Add(new Block(b));
            }

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
            b.Init();
            blocks.Add(b);

            doodads = new List<Doodad>();
            monsters = new List<Monster>();
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
            doodads = new List<Doodad>();
            monsters = new List<Monster>();
        }

        public VertexPositionColor[] GetTemplate(Vector3 position, Vector3 templatePosition)
        {
            Color templateColor = Color.White;
            Block b = null;
            VertexPositionColor[] vList = new VertexPositionColor[8];

            Vector3 xDir = Vector3.UnitX;
            if (Vector3.Cross(xDir, normal).Equals(Vector3.Zero))
                xDir = Vector3.UnitZ;
            Vector3 yDir = Vector3.Cross(normal, xDir);
            yDir.Normalize();
            xDir.Normalize();
            xDir = 2f * xDir;
            yDir = 2f * yDir;

            b = GetHoverBlock(position);
            if (b == null)
            {                
                templateColor = Color.Black;
                b = new Block();
                b.edges.Add(new Edge(templatePosition, templatePosition + xDir));
                b.edges.Add(new Edge(templatePosition + xDir, templatePosition + xDir + yDir));
                b.edges.Add(new Edge(templatePosition + xDir + yDir, templatePosition + yDir));
                b.edges.Add(new Edge(templatePosition + yDir, templatePosition));
            }

            if(IsBlockValid(b))
            {
                vList[0] = new VertexPositionColor(b.edges[0].start + .2f*normal, templateColor);
                vList[1] = new VertexPositionColor(b.edges[0].end + .2f * normal, templateColor);
                vList[2] = new VertexPositionColor(b.edges[1].start + .2f * normal, templateColor);
                vList[3] = new VertexPositionColor(b.edges[1].end + .2f * normal, templateColor);
                vList[4] = new VertexPositionColor(b.edges[2].start + .2f * normal, templateColor);
                vList[5] = new VertexPositionColor(b.edges[2].end + .2f * normal, templateColor);
                vList[6] = new VertexPositionColor(b.edges[3].start + .2f * normal, templateColor);
                vList[7] = new VertexPositionColor(b.edges[3].end + .2f * normal, templateColor);
                return vList;
            }
            return null;

        }

        public Edge GetHoverEdge(Vector3 position)
        {
            foreach (Block b in blocks)
            {
                foreach (Edge e in b.edges)
                {
                    Vector3 v = e.end - e.start;
                    Vector3 p = position - e.end;
                    Vector3 n = Vector3.Cross(v, normal);
                    n.Normalize();
                    Vector3 u = v;
                    u.Normalize();
                    float nCoord = Math.Abs(Vector3.Dot(n, p));
                    float uCoord = -Vector3.Dot(u, p);
                    if (nCoord < .2f && uCoord < v.Length() && uCoord > 0)
                    {
                        return e;
                    }
                }
            }
            return null;
        }

        public Edge GetHoverVertex(Vector3 position)
        {
            foreach (Block b in blocks)
            {
                foreach (Edge e in b.edges)
                {                  
                    if ((e.start-position).Length() < .2f)
                    {
                        return e;
                    }
                }
            }
            return null;
        }

        public Doodad GetHoverDoodad(Vector3 position)
        {
            foreach (Doodad d in doodads)
            {
                if ((position - d.position).Length() < .3f)
                    return d;
            }
            return null;
        }

        public Monster GetHoverMonster(Vector3 position)
        {
            foreach (Monster m in monsters)
            {
                if ((position - m.position).Length() < .3f)
                    return m;
            }
            return null;
        }

        public Block GetHoverBlock(Vector3 position)
        {
            foreach (Block b in blocks)
            {
                Vector3 result;
                Vector3 prevResult = Vector3.Zero;
                bool isHoverBlock = true;
                    
                foreach (Edge e in b.edges)
                {
                    Vector3 v1 = e.start - e.end;
                    Vector3 v2 = position - e.end;
                    result = Vector3.Cross(v1, v2);
                    if (Vector3.Dot(result, prevResult) < 0)
                        isHoverBlock = false;
                    prevResult = result;                               

                    Vector3 v = e.end - e.start;
                    Vector3 p = position - e.end;
                    Vector3 n = Vector3.Cross(v, normal);
                    n.Normalize();
                    Vector3 u = v;
                    u.Normalize();
                    float nCoord = Math.Abs(Vector3.Dot(n, p));
                    float uCoord = -Vector3.Dot(u, p);
                    if (nCoord < .2f && uCoord < v.Length() && uCoord > 0)
                    {
                        return b;
                    }
                    if ((e.start - position).Length() < .2f || (e.end - position).Length() < .2f)
                    {
                        return b;
                    }
                }
                if (isHoverBlock == true)
                    return b;
            }
            return null;
        }

        public VertexPositionColor[] GetSelectedDoodadHighlight(Vector3 position, Vector3 currentUp)
        {
            
            VertexPositionColor[] vList = null;
            Doodad d = GetHoverDoodad(position);
            if (d == null)
            {
                Color templateColor = Color.Blue;
                Vector3 lockPosition = new Vector3((int)position.X, (int)position.Y, (int)position.Z);                                               
                Vector3 up = .5f * currentUp;
                Vector3 left = .5f * Vector3.Cross(currentUp, normal);
                lockPosition += up + left + .4f*normal;

                Block testBlock = new Block();
                testBlock.edges.Add(new Edge(lockPosition - up, lockPosition + left));
                testBlock.edges.Add(new Edge(lockPosition + left, lockPosition + up));
                testBlock.edges.Add(new Edge(lockPosition + up, lockPosition - left));
                testBlock.edges.Add(new Edge(lockPosition - left, lockPosition - up));
                if(IsBlockValid(testBlock))
                {
                    vList = new VertexPositionColor[10];
                    vList[0] = new VertexPositionColor(lockPosition - up, templateColor);
                    vList[1] = new VertexPositionColor(lockPosition + left, templateColor);
                    vList[2] = new VertexPositionColor(lockPosition + left, templateColor);
                    vList[3] = new VertexPositionColor(lockPosition + up, templateColor);
                    vList[4] = new VertexPositionColor(lockPosition + up, templateColor);
                    vList[5] = new VertexPositionColor(lockPosition - left, templateColor);
                    vList[6] = new VertexPositionColor(lockPosition - left, templateColor);
                    vList[7] = new VertexPositionColor(lockPosition - up, templateColor);
                    vList[8] = new VertexPositionColor(lockPosition + up, templateColor);
                    vList[9] = new VertexPositionColor(lockPosition, templateColor);
                }
            
            }

            return vList;
        }

        public VertexPositionColor[] GetSelectedMonsterHighlight(Vector3 position, Vector3 currentUp)
        {

            VertexPositionColor[] vList = null;
            Monster m = GetHoverMonster(position);
            if (m == null)
            {
                Color templateColor = Color.Yellow;
                Vector3 lockPosition = new Vector3((int)position.X, (int)position.Y, (int)position.Z);
                Vector3 up = .5f * currentUp;
                Vector3 left = .5f * Vector3.Cross(currentUp, normal);
                lockPosition += up + left + .4f * normal;

                Block testBlock = new Block();
                testBlock.edges.Add(new Edge(lockPosition - up, lockPosition + left));
                testBlock.edges.Add(new Edge(lockPosition + left, lockPosition + up));
                testBlock.edges.Add(new Edge(lockPosition + up, lockPosition - left));
                testBlock.edges.Add(new Edge(lockPosition - left, lockPosition - up));
                if (IsBlockValid(testBlock))
                {
                    vList = new VertexPositionColor[6];
                    vList[0] = new VertexPositionColor(lockPosition - up - left, templateColor);
                    vList[1] = new VertexPositionColor(lockPosition + up + left, templateColor);
                    vList[2] = new VertexPositionColor(lockPosition - up + left, templateColor);
                    vList[3] = new VertexPositionColor(lockPosition + up - left, templateColor);
                    vList[4] = new VertexPositionColor(lockPosition + up, templateColor);
                    vList[5] = new VertexPositionColor(lockPosition, templateColor);
 
                }

            }

            return vList;
        }

        public VertexPositionColor[] GetSelectedLineHighlight(Vector3 position)
        {
            Color templateColor = Color.White;
            
            VertexPositionColor[] vList = null;
            
           Edge e = GetHoverEdge(position);
            if(e!=null)
            {
                if(vList == null)
                    vList = new VertexPositionColor[2];
                vList[0] = new VertexPositionColor(e.start + .3f*normal,templateColor);
                vList[1] = new VertexPositionColor(e.end + .3f*normal, templateColor);               
            }
            return vList;
        }

        public VertexPositionColor[] GetSelectedVertexHighlight(Vector3 position)
        {
            Color templateColor = Color.White;

            VertexPositionColor[] vList = null;

            Edge e = GetHoverVertex(position);
            if (e != null)
            {
                if (vList == null)
                    vList = new VertexPositionColor[2];
                vList[0] = new VertexPositionColor(e.start, templateColor);
                vList[1] = new VertexPositionColor(e.start + .3f * normal, templateColor);
            }
            return vList;
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
            xDir = 2f * xDir;
            yDir = 2f * yDir;

            b.edges.Add(new Edge(position, position + xDir));
            b.edges.Add(new Edge(position + xDir, position + xDir + yDir));
            b.edges.Add(new Edge(position + xDir + yDir, position + yDir));
            b.edges.Add(new Edge(position + yDir, position));

            bool blockOK = IsBlockValid(b);
            if (blockOK)
            {
                b.Init();
                blocks.Add(b);
            }
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
            foreach (Monster m in monsters)
            {
                m.Move(delta);
            }
            foreach (Doodad d in doodads)
            {
                d.Move(delta);
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
