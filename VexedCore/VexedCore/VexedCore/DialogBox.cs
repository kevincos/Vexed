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

        static DialogBox()
        {
            dialogLibrary = new List<DialogChunk>();
            Stream stream = new FileStream("dialog.xml", FileMode.Open, FileAccess.ReadWrite);
            //Stream stream = new FileStream("test.xml", FileMode.Create, FileAccess.ReadWrite);
            XmlSerializer serializer = new XmlSerializer(typeof(List<DialogChunk>));
            dialogLibrary = (List<DialogChunk>)serializer.Deserialize(stream);                                       
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
                float xPercent = .1f;
                float yPercent = .7f;
                float textXPercent = .25f;
                float textYPercent = .72f;
                float boxWidth = .9f;
                float boxHeight = .1125f;

                Engine.spriteBatch.Draw(box, new Rectangle((int)(Game1.titleSafeRect.Left + xPercent * Game1.titleSafeRect.Width), (int)(Game1.titleSafeRect.Top + yPercent * Game1.titleSafeRect.Height), (int)(boxWidth * Game1.titleSafeRect.Width), (int)(boxHeight * Game1.titleSafeRect.Width)), new Rectangle(0, 0, 512, 64), Color.White);

                int portraitIndex = 0;
                if (currentDialog.speaker == "OldMan")
                    portraitIndex = 0;
                else if (currentDialog.speaker == "ChaseBoss")
                    portraitIndex = 1;
                else if (currentDialog.speaker == "ArmorBoss")
                    portraitIndex = 2;
                else if (currentDialog.speaker == "System")
                    portraitIndex = 3;
                else
                    portraitIndex = 0;
                Engine.spriteBatch.Draw(portraits, new Rectangle((int)(Game1.titleSafeRect.Left + xPercent * Game1.titleSafeRect.Width), (int)(Game1.titleSafeRect.Top + yPercent * Game1.titleSafeRect.Height), (int)((textXPercent - xPercent) * Game1.titleSafeRect.Width), (int)(boxHeight * Game1.titleSafeRect.Width)), new Rectangle((portraitIndex % 8) * 64, (portraitIndex / 8) * 64, 64, 64), Color.White);
                

                Engine.spriteBatch.DrawString(Engine.spriteFont, currentDialog.textList[stage], new Vector2(Game1.titleSafeRect.Left + textXPercent * Game1.titleSafeRect.Width, Game1.titleSafeRect.Top + textYPercent * Game1.titleSafeRect.Height), Color.YellowGreen);
                Engine.spriteBatch.End();
            }
        }

        public bool Next()
        {
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
