using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace VexedCore
{
    public class DecorationTexture
    {
        public string fileName;
        public Texture2D texture;

        public DecorationTexture(string fileName, ContentManager Content)
        {
            this.fileName = fileName;
            this.texture = Content.Load<Texture2D>(fileName);
        }
    }

    public class DecorationImage
    {
        public static List<DecorationTexture> textureLibrary;

        public static void LoadTextures(ContentManager Content)
        {
            if(textureLibrary == null)
                textureLibrary = new List<DecorationTexture>();
            textureLibrary.Add(new DecorationTexture("dec_arrow", Content));
            textureLibrary.Add(new DecorationTexture("dec_tree", Content));
            textureLibrary.Add(new DecorationTexture("dec_vinebranch", Content));
            textureLibrary.Add(new DecorationTexture("dec_leftvinebranch", Content));
            textureLibrary.Add(new DecorationTexture("dec_rightvinebranch", Content));
            textureLibrary.Add(new DecorationTexture("dec_watermeter", Content));
            textureLibrary.Add(new DecorationTexture("dec_watertank", Content));
            textureLibrary.Add(new DecorationTexture("dec_waterpipes", Content));
            textureLibrary.Add(new DecorationTexture("dec_shuttle", Content));
            textureLibrary.Add(new DecorationTexture("dec_shuttle_open", Content));
            textureLibrary.Add(new DecorationTexture("dec_thornwall", Content));
        }

        public static Texture2D FetchTexture(string fileName)
        {
            foreach (DecorationTexture tex in textureLibrary)
            {
                if (tex.fileName == fileName)
                    return tex.texture;
            }
            return null;
        }
    }
}
