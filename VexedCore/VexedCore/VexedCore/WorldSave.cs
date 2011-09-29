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
    class WorldSave
    {
        PlayerSave player;
        List<RoomSave> rooms;
        

        public WorldSave()
        {
        }

        public WorldSave(Player p, List<Room> roomList)
        {
            player = new PlayerSave(p);
            rooms = new List<RoomSave>();
            foreach (Room r in roomList)
            {
                rooms.Add(new RoomSave(r));
            }
        }
    }

    class PlayerSave
    {
        Vector3 position;
        Vector3 velocity;
        Vector3 normal;
        Vector3 direction;

        public PlayerSave()
        {
        }

        public PlayerSave(Player p)
        {
            position = p.center.position;
            velocity = p.center.velocity;
            normal = p.center.normal;
            direction = p.center.direction;
        }
    }

    class RoomSave
    {
        public RoomSave()
        {
        }

        public RoomSave(Room r)
        {
        }
    }

    class MonsterSave
    {
        public MonsterSave()
        {
        }

        public MonsterSave(Monster m)
        {
        }
    }

    class DoodadSave
    {
        public DoodadSave()
        {
        }

        public DoodadSave(Doodad d)
        {
        }
    }

    class ProjectileSave
    {
        public ProjectileSave()
        {
        }

        public ProjectileSave(Projectile p)
        {
        }
    }
}
