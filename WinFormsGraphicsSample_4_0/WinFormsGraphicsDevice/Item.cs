using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WinFormsGraphicsDevice
{
    public enum ItemType
    {
        Ability = 0,
        Brick = 1,
        Crate = 2,
        SpikeBall = 3,
        Orb = 4,
        JumpPad = 5,
        Vortex = 6,
        Switch = 7
    }

    public class Item
    {
        public int id;
        public String name;
        public Vector3 position;
        bool fixedPosition;
        public String targetObject;
        public String expectBehavior;
        public String targetBehavior;
        public ItemType type;
        public AbilityType ability;

        public Item()
        {
            name = "Item";
            id = IDControl.GetID();
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
