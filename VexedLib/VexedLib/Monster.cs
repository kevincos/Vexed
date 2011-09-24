using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace VexedLib
{
    public enum MovementType
    {        
        None = 0,
        Tank= 1,
        Spider=2,
        Jump=3,
        Hover=4,
        RockBoss=5,
        SnakeBoss=6,
        ArmorBoss=7,
        ChaseBoss=8,
        BattleBoss=9,
        FaceBoss=10,
        JetBoss=11
    }

    public enum ArmorType
    {
        None=0,
        Top=1,
        Shield=2,
        Full=3,
        TopSuper=4,
        ShieldSuper=5,
        FullSuper=6
    }

    public enum TrackType
    {
        Slow=0,
        Normal=1,
        Fast=2,
        Up=3,
        UpLeft=4,
        Left=5,
        UpRight=6,
        Right=7
    }

    public enum MonsterSpeed
    {
        Slow=0,
        Medium=1,
        Fast=2
    }

    public enum MonsterSize
    {
        Normal = 0,
        Large = 1,
        Huge = 2
    }

    public enum MonsterHealth
    {
        Weak = 0,
        Normal = 1,
        Tough = 2
    }

    public enum GunType
    {
        None=0,
        Blaster=1,
        Repeater=2,
        Beam=3,
        Missile=4,
        Spread=5,
        Bomb=6
    }

    public enum AIType
    {
        Stationary=0,
        Hunter=1,
        Wander=2,
        Waypoint=3
    }

    public class Monster
    {
        public int id;
        public String name;
        public Vector3 position;
        public Vector3 up;
        public bool fixedPath;
        public String waypointId;
        public MovementType movement;
        public ArmorType armor;
        public GunType weapon;
        public AIType behavior;
        public MonsterHealth health;
        public MonsterSize size;
        public MonsterSpeed speed;
        public TrackType trackType;

        public Monster()
        {
            name = "Monster";
            id = IDControl.GetID();            
        }

        public Monster(Monster m)
        {
            name = m.name;
            id = m.id;
            position = m.position;
            up = m.up;
            fixedPath = m.fixedPath;
            waypointId = m.waypointId;
            movement = m.movement;
            armor = m.armor;
            weapon = m.weapon;
            behavior = m.behavior;
        }

        public String IDString
        {
            get
            {
                return name + "_" + id;
            }
        }

        public void Move(Vector3 delta)
        {
            position += delta;
        }
    }
}
