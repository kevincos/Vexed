using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VexedLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace VexedCore
{
    class Player
    {
        public Vector3 position;
        public Vector3 up;
        public Face currentRoom;
        public Face currentFace;
    }
}
