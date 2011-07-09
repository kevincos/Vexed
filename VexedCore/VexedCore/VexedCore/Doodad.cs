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

        public JumpPad(Vector3 position, Vector3 normal)
        {
            this.position = new Vertex(position, normal,Vector3.Zero, Vector3.Zero);
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
    }
}
