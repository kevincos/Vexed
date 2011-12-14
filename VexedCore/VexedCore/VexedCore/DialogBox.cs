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
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace VexedCore
{


    public class DialogChunk
    {
        public String id;
        public String speaker;
        public List<String> textList;
        public bool pause;

        
        public DialogChunk()
        {
        }

        public DialogChunk(String id, String speaker, String text)
        {
            this.id = id;
            this.speaker = speaker;
            this.textList = new List<string>();
            this.textList.Add(text);
            this.textList.Add("EXTRATEXT");
        }
    }

    public class DialogBox
    {


        public static Texture2D box;
        public static Texture2D portraits;

        public static int lifeTime = 0;
        int cooldown = 0;
        public static int stage = 0;
        public string dialogId = "Default";
        public static int maxLifeTime = 2000;

        public static DialogChunk currentDialog;

        public static List<DialogChunk> dialogLibrary;

        public static List<Vector2> oldmanPortait;
        public static List<Vector2> chaseBossPortait;
        public static List<Vector2> armorBossPortait;

        public static RenderTarget2D dialogRenderTarget;

        public static void RenderTextures()
        {
            Game1.graphicsDevice.SetRenderTarget(dialogRenderTarget);
            Game1.graphicsDevice.Clear(Color.Transparent);
            Engine.spriteBatch.Begin();
            if (currentDialog != null && stage < currentDialog.textList.Count)
            {
                Engine.spriteBatch.Draw(box, Vector2.Zero, Color.White);

                int portraitIndex = 0;
                if (currentDialog.speaker == "OldMan")
                    portraitIndex = 0;
                else if (currentDialog.speaker == "ChaseBoss")
                    portraitIndex = 1;
                else if (currentDialog.speaker == "ArmorBoss")
                    portraitIndex = 2;
                else if (currentDialog.speaker == "System")
                    portraitIndex = 3;
                else if (currentDialog.speaker == "IceSnake")
                    portraitIndex = 4;
                else if (currentDialog.speaker == "SnowMan")
                    portraitIndex = 5;
                else if (currentDialog.speaker == "JetBoss")
                    portraitIndex = 6;
                else if (currentDialog.speaker == "FireMan")
                    portraitIndex = 7;
                else if (currentDialog.speaker == "CommandBoss")
                    portraitIndex = 8;
                else if (currentDialog.speaker == "FinalBoss")
                    portraitIndex = 9;
                else
                    portraitIndex = 0;
                Engine.spriteBatch.Draw(portraits, new Rectangle(0,0,64,64), new Rectangle((portraitIndex % 8) * 64, (portraitIndex / 8) * 64, 64, 64), Color.White);


                Engine.spriteBatch.DrawString(Engine.spriteFont, currentDialog.textList[stage], new Vector2(67,5), Color.YellowGreen, 0, Vector2.Zero, .9f, SpriteEffects.None,0);
            }
            Engine.spriteBatch.End();
            Game1.graphicsDevice.SetRenderTarget(null);
        }

        static DialogBox()
        {
            dialogLibrary = new List<DialogChunk>();
            Stream stream = new FileStream("dialog.xml", FileMode.Open, FileAccess.ReadWrite);
            //Stream stream = new FileStream("test.xml", FileMode.Create, FileAccess.ReadWrite);
            XmlSerializer serializer = new XmlSerializer(typeof(List<DialogChunk>));
            dialogLibrary = (List<DialogChunk>)serializer.Deserialize(stream);

            PresentationParameters pp = Game1.graphicsDevice.PresentationParameters;
            dialogRenderTarget = new RenderTarget2D(Game1.graphicsDevice,
                                                   512, 64, false,
                                                   pp.BackBufferFormat, pp.DepthStencilFormat);    
        }

        public static int texGridCount = 8;

        public static List<Vector2> LoadTexCoords(int x, int y)
        {
            float texWidth = 1f / texGridCount;
            List<Vector2> texCoords = new List<Vector2>();
            texCoords.Add(new Vector2((x + 1) * texWidth, y * texWidth));
            texCoords.Add(new Vector2(x * texWidth, y * texWidth));
            texCoords.Add(new Vector2(x * texWidth, (y + 1) * texWidth));
            texCoords.Add(new Vector2((x + 1) * texWidth, (y + 1) * texWidth));

            return texCoords;
        }

        public static void InitTexCoords()
        {
            oldmanPortait = LoadTexCoords(0, 0);
            chaseBossPortait = LoadTexCoords(1, 0);
            armorBossPortait = LoadTexCoords(2, 0);
        }

        public static void SetDialog(String id)
        {                                    
            foreach (DialogChunk chunk in dialogLibrary)
            {
                if (chunk.id == id)
                {
                    currentDialog = chunk;
                }
            }
            if(currentDialog.pause == true)
                Engine.player.state = State.Dialog;

            stage = 0;
            lifeTime = 0;
            
        }

        public void Draw()
        {
            if (currentDialog != null && stage < currentDialog.textList.Count && ((currentDialog.pause == false && Engine.player.state != State.Dialog && (lifeTime < maxLifeTime)) || Engine.player.state == State.Dialog))
            {
                Engine.spriteBatch.Begin();
                float dialogBoxHeight = Game1.titleSafeRect.Height / 6;
                float dialogBoxLength = dialogBoxHeight * 8;
                Engine.spriteBatch.Draw(dialogRenderTarget, new Rectangle((int)(Game1.titleSafeRect.Center.X - dialogBoxLength/2), (int)(Game1.titleSafeRect.Bottom - dialogBoxHeight - 50), (int)dialogBoxLength, (int)dialogBoxHeight), Color.White);

                Engine.spriteBatch.End();
            }
        }

        public bool Next()
        {
            if (currentDialog == null)
                return false;
            if (cooldown > 0)
                return true;
            stage++;
            cooldown = 300;            
            if (stage >= currentDialog.textList.Count)
                return false;
            return true;
        }

        public void Update(GameTime gameTime)
        {
            lifeTime += gameTime.ElapsedGameTime.Milliseconds;
            cooldown -= gameTime.ElapsedGameTime.Milliseconds;
            if (cooldown < 0)
                cooldown = 0;
        }
    }
}
