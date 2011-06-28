using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WinFormsGraphicsDevice
{
    public class Room
    {
        public int id;
        public String name;
        public int centerX, centerY, centerZ;
        public int sizeX, sizeY, sizeZ;
        public Color color;

        public Face[] faceList;

        public Room()
        {
            name = "Room";
            id = IDControl.GetID();
            centerX = 0;
            centerY = 0;
            centerZ = 0;
            sizeX = 10;
            sizeY = 10;
            sizeZ = 10;
            color = Color.GreenYellow;

            faceList = new Face[6];
            faceList[0] = new Face(new Vector3(1, 0, 0), new Vector3[]{
                new Vector3(centerX + sizeX / 2, centerY - sizeY / 2, centerZ - sizeZ/2),
                new Vector3(centerX + sizeX / 2, centerY - sizeY/2, centerZ+sizeZ/2),
                new Vector3(centerX + sizeX / 2, centerY + sizeZ/2, centerZ+sizeZ/2),
                new Vector3(centerX + sizeX / 2, centerY + sizeZ/2, centerZ-sizeZ/2),});
            faceList[1] = new Face(new Vector3(-1, 0, 0), new Vector3[]{
                new Vector3(centerX - sizeX / 2, centerY - sizeY / 2, centerZ - sizeZ/2),
                new Vector3(centerX - sizeX / 2, centerY - sizeY/2, centerZ+sizeZ/2),
                new Vector3(centerX - sizeX / 2, centerY + sizeZ/2, centerZ+sizeZ/2),
                new Vector3(centerX - sizeX / 2, centerY + sizeZ/2, centerZ-sizeZ/2),});
            faceList[2] = new Face(new Vector3(0, 1, 0), new Vector3[]{
                new Vector3(centerX - sizeX / 2, centerY + sizeY / 2, centerZ - sizeZ/2),
                new Vector3(centerX - sizeX / 2, centerY + sizeY/2, centerZ+sizeZ/2),
                new Vector3(centerX + sizeX / 2, centerY + sizeZ/2, centerZ+sizeZ/2),
                new Vector3(centerX + sizeX / 2, centerY + sizeZ/2, centerZ-sizeZ/2),});
            faceList[3] = new Face(new Vector3(0, -1, 0), new Vector3[]{
                new Vector3(centerX - sizeX / 2, centerY - sizeY / 2, centerZ - sizeZ/2),
                new Vector3(centerX - sizeX / 2, centerY - sizeY/2, centerZ+sizeZ/2),
                new Vector3(centerX + sizeX / 2, centerY - sizeZ/2, centerZ+sizeZ/2),
                new Vector3(centerX + sizeX / 2, centerY - sizeZ/2, centerZ-sizeZ/2),});
            faceList[4] = new Face(new Vector3(0, 0, 1), new Vector3[]{
                new Vector3(centerX - sizeX / 2, centerY - sizeY / 2, centerZ + sizeZ/2),
                new Vector3(centerX - sizeX / 2, centerY + sizeY/2, centerZ+sizeZ/2),
                new Vector3(centerX + sizeX / 2, centerY + sizeZ/2, centerZ+sizeZ/2),
                new Vector3(centerX + sizeX / 2, centerY - sizeZ/2, centerZ+sizeZ/2),});
            faceList[5] = new Face(new Vector3(0, 0, -1), new Vector3[]{
                new Vector3(centerX - sizeX / 2, centerY - sizeY / 2, centerZ - sizeZ/2),
                new Vector3(centerX - sizeX / 2, centerY + sizeY/2, centerZ-sizeZ/2),
                new Vector3(centerX + sizeX / 2, centerY + sizeZ/2, centerZ-sizeZ/2),
                new Vector3(centerX + sizeX / 2, centerY - sizeZ/2, centerZ-sizeZ/2),});

        }

        public Room(Room r)
        {
            id = r.id;
            name = r.name;
            centerX = r.centerX;
            centerY = r.centerY;
            centerZ = r.centerZ;
            sizeX = r.sizeX;
            sizeY = r.sizeY;
            sizeZ = r.sizeZ;
            color = r.color;
            faceList = new Face[6];
            for (int i = 0; i < 6; i++)
            {
                faceList[i] = new Face(r.faceList[i]);
            }
        }

        public void Move(Vector3 delta)
        {
            centerX += (int)delta.X;
            centerY += (int)delta.Y;
            centerZ += (int)delta.Z;
            foreach (Face f in faceList)
            {
                f.Move( delta );                
            }
        }

        public void Resize(Vector3 delta)
        {
            sizeX += (int)delta.X;
            sizeY += (int)delta.Y;
            sizeZ += (int)delta.Z;
            Vector3 u = delta;
            u.Normalize();
            foreach (Face f in faceList)
            {
                if (Vector3.Dot(f.normal, u) == 1)
                {
                    if (delta.X < 0 || delta.Y < 0 || delta.Z < 0)
                        f.Move(-.5f * delta);
                    else
                        f.Move(.5f*delta);
                }
                if (Vector3.Dot(f.normal, u) == -1)
                {
                    if (delta.X < 0 || delta.Y < 0 || delta.Z < 0)
                        f.Move(.5f * delta);
                    else
                        f.Move(-.5f * delta);
                }
                f.Resize(new Vector3(sizeX, sizeY, sizeZ));
            }
        }
        
        public String IDString
        {
            get
            {
                return name + "_" + id;
            }
        }
    }
}
