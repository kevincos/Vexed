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
    public class Vertex
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector3 direction;
        public Vector3 velocity;

        public Vertex(Vector3 pos, Vector3 n, Vector3 vel, Vector3 dir)
        {
            position = pos;
            normal = n;
            velocity = vel;
            direction = dir;
        }
    }
}
