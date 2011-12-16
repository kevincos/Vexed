﻿using System;
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
        public List<Texture2D> texture;
        public int frameCount = 1;

        public DecorationTexture(string fileName, ContentManager Content)
        {
            this.fileName = fileName;
            this.texture = new List<Texture2D>();
            texture.Add(Content.Load<Texture2D>(fileName));
        }

        public DecorationTexture(string fileName, ContentManager Content, int frameCount)
        {
            this.frameCount = frameCount;
            this.fileName = fileName;
            this.texture = new List<Texture2D>();
            for (int i = 0; i < frameCount; i++)
            {
                texture.Add(Content.Load<Texture2D>(fileName+"_"+i));
            }
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

            textureLibrary.Add(new DecorationTexture("dec_piston", Content,11));
            textureLibrary.Add(new DecorationTexture("dec_colorwheel", Content, 8));
            
        }

        public static List<Texture2D> FetchTexture(string fileName)
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
