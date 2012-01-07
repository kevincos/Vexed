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
        public List<Texture2D> texture;
        public int frameCount = 1;
        public bool forceSpin = false;

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

        public DecorationTexture(string fileName, ContentManager Content, int frameCount, bool forceSpin)
        {
            this.forceSpin = forceSpin;
            this.frameCount = frameCount;
            this.fileName = fileName;
            this.texture = new List<Texture2D>();
            for (int i = 0; i < frameCount; i++)
            {
                texture.Add(Content.Load<Texture2D>(fileName + "_" + i));
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
            textureLibrary.Add(new DecorationTexture("dec_watermeter", Content));
            
            textureLibrary.Add(new DecorationTexture("dec_waterpipes", Content));
            textureLibrary.Add(new DecorationTexture("dec_shuttle", Content));
            textureLibrary.Add(new DecorationTexture("dec_shuttle_open", Content));
            textureLibrary.Add(new DecorationTexture("dec_thornwall", Content));
            textureLibrary.Add(new DecorationTexture("dec_onionrack", Content));
            textureLibrary.Add(new DecorationTexture("dec_veggietube", Content));
            textureLibrary.Add(new DecorationTexture("dec_tomatotank", Content));
            textureLibrary.Add(new DecorationTexture("dec_bracer", Content));
            textureLibrary.Add(new DecorationTexture("dec_girder", Content));
            textureLibrary.Add(new DecorationTexture("dec_cables", Content));
            textureLibrary.Add(new DecorationTexture("dec_icicle", Content));
            textureLibrary.Add(new DecorationTexture("dec_icecolumn", Content));
            textureLibrary.Add(new DecorationTexture("dec_hotpipe", Content));
            textureLibrary.Add(new DecorationTexture("dec_cargo", Content));
            textureLibrary.Add(new DecorationTexture("dec_crate", Content));
            textureLibrary.Add(new DecorationTexture("dec_shortcargo", Content));
            textureLibrary.Add(new DecorationTexture("dec_snowman", Content));
            textureLibrary.Add(new DecorationTexture("dec_gear", Content, 4));
            textureLibrary.Add(new DecorationTexture("dec_lightpost", Content, 8));

            textureLibrary.Add(new DecorationTexture("dec_piston", Content,11));
            textureLibrary.Add(new DecorationTexture("dec_colorwheel", Content, 8));
            textureLibrary.Add(new DecorationTexture("dec_hotpipesteam", Content,8));
            textureLibrary.Add(new DecorationTexture("dec_hotmeter", Content, 8));
            textureLibrary.Add(new DecorationTexture("dec_vinebranch", Content, 8, true));
            textureLibrary.Add(new DecorationTexture("dec_watertank", Content, 8, true));
            textureLibrary.Add(new DecorationTexture("dec_leftvinebranch", Content,8,true));
            textureLibrary.Add(new DecorationTexture("dec_rightvinebranch", Content,8,true));
            
            
        }

        public static DecorationTexture FetchTexture(string fileName)
        {
            foreach (DecorationTexture tex in textureLibrary)
            {
                if (tex.fileName == fileName)
                    return tex;
            }
            return null;
        }
    }
}
