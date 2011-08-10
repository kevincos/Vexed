using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace VexedLib
{
    public enum DoodadType
    {
        SaveStation = 0,
        WarpStation = 1,
        JumpStation = 2,
        SwitchStation = 3,
        UpgradeStation = 4,        
        PowerStation = 5,
        ItemStation = 6,
        WallSwitch = 7,
        Spring = 8,
        Door = 9,
        Beam = 10,
        Brick = 11,
        Crate = 12,
        SpikeBall = 13,
        PowerOrb = 14,
        JumpPad = 15,
        Vortex = 16,
        Button = 17,
        BridgeGate = 18,
        ItemBlock = 19,
        PlayerSpawn = 20,
        Checkpoint = 21,
        Waypoint = 22,
        BridgeBack = 23,
        BridgeSide = 24,
        BridgeCover = 25,
        StationIcon = 26,
        LeftDoor = 27,
        RightDoor = 28,
        NPC_OldMan = 29
    }

    public enum AbilityType
    {
        Empty = 29,
        Blaster = 19,
        Missile = 16,
        Laser = 17,
        Bomb = 18,
        WallJump = 0,
        DoubleJump = 1,
        JetPack = 9,
        Boots = 10,
        Booster = 8,
        Shield = 4,
        RedKey = 11,
        BlueKey = 13,
        YellowKey = 12,
        NormalJump = 2,
        Use = 3,
        BButton = 24,
        XButton = 25,
        AButton = 26,
        YButton = 27,
        Passive = 28,
        PermanentBoots = 5,
        PermanentWallJump = 6,
        PermanentShield = 7,
        PermanentYellowKey = 14,
        PermanentBlueKey = 15,
        PermanentRedKey = 20,
        DualWield = 21,
        ImprovedJump = 22,
        Ultima = 23
    }

    public class Doodad
    {
        public int id;
        public String name;
        public Vector3 position;
        public Vector3 up;
        public int activationCost;
        public String targetObject;
        public String expectBehavior;
        public String targetBehavior;
        public bool fixedPosition;
        public DoodadType type;
        public AbilityType ability;
        public List<Behavior> behaviors;

        public Doodad()
        {
            name = "Doodad";
            id = IDControl.GetID();
            behaviors = new List<Behavior>();            
            type = DoodadType.PowerOrb;
        }

        public void Init()
        {
            behaviors.Add(new Behavior());
        }

        public Doodad(Doodad d)
        {
            name = d.name;
            id = d.id;
            position = d.position;
            up = d.up;
            activationCost = d.activationCost;
            targetBehavior = d.targetBehavior;
            targetObject = d.targetObject;
            expectBehavior = d.expectBehavior;
            fixedPosition = d.fixedPosition;
            type = d.type;
            ability = d.ability;
            behaviors = new List<Behavior>();
            foreach (Behavior b in d.behaviors)
            {
                behaviors.Add(new Behavior(b));
            }
        }

        public String IDString
        {
            get
            {
                return name + "_" + id;
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
