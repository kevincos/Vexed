using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WinFormsGraphicsDevice
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
        Waypoint = 22
    }

    public enum AbilityType
    {        
        DoubleJump = 0,
        Boost = 1,
        Blaster = 2,
        RedKey = 3,
        BlueKey = 4,
        YellowKey = 5,
        GreenKey =6,
        Jets =7,
        Bomb = 8,
        Boots = 9,
        WallJump = 10,
        Jetpack = 11,
        HighJump = 12,
        Speed = 13,
        NinjaRope = 14
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
            behaviors.Add(new Behavior());
            type = DoodadType.PowerOrb;
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
