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


    public enum SpeakerId
    {
        OldMan,
        ChaseBoss,
        ArmorBoss,
        System,
        IceSnake,
        SnowMan,
        JetBoss,
        FireBoss,
        CommandBoss,
        FinalBoss
    }

    public class DialogChunk
    {
        public String id;
        public SpeakerId speaker;
        public List<String> textList;
        public bool pause;

        
        public DialogChunk()
        {
        }

        public DialogChunk(String id, SpeakerId speaker, String text)
        {
            this.id = id;
            this.speaker = speaker;
            this.textList = new List<string>();
            this.textList.Add(text);
            this.textList.Add("EXTRATEXT");
        }

        

        public void FitToBox()
        {
            if (DialogBox.currentFont == null) DialogBox.currentFont = DialogBox.newFont;
            int w = Game1.titleSafeRect.Height / 7;
            float boxLength = 6 * w;            
            for (int i = 0; i < textList.Count; i++)
            {
                textList[i] = SpriteUtil.TextFit(textList[i], boxLength);                
            }
            
            
        }
    }

    public enum BoxState
    {
        None,
        Appear,
        Extend,
        Text,
        Wait,
        Close,
        Vanish
    }

    public class DialogBox
    {
        public static int animationTime = 0;
        public static int appearMaxTime = 200;
        public static int extendMaxTime = 300;
        public static int charMaxTime = 10;
        public static String currentText = "";
        public static int currentCharacter = 0;        


        public static Texture2D box;
        public static Texture2D portraits;

        public static Texture2D monitor;
        public static Texture2D monitorFrame;
        public static Texture2D rightEdge;
        public static Texture2D textArea;
        public static Texture2D spaceBar;

        public static int lifeTime = 0;
        int cooldown = 0;
        public static int stage = 0;
        public string dialogId = "Default";
        public static int maxLifeTime = 2000;        
        public int portraitIndex = 0;
        public static BoxState state = BoxState.None;

        public static DialogChunk currentDialog;

        public static List<DialogChunk> dialogLibrary;

        public static List<Vector2> oldmanPortait;
        public static List<Vector2> chaseBossPortait;
        public static List<Vector2> armorBossPortait;


        public static SpriteFont smallFont;
        public static SpriteFont largeFont;


        public static SpriteFont currentFont;

        public static SpriteFont newFont
        {
            get
            {
                if (Engine.res == ResolutionSettings.R_1920x1080)
                {
                    return DialogBox.largeFont;
                }
                return DialogBox.smallFont;
            }
        }

        public static Texture2D[] dialogPortraits;

        public static void InitPortraits(ContentManager content)
        {
            dialogPortraits = new Texture2D[10];
            dialogPortraits[(int)SpeakerId.OldMan] = content.Load<Texture2D>("UI\\portrait_oldman");
            dialogPortraits[(int)SpeakerId.SnowMan] = content.Load<Texture2D>("UI\\portrait_snowman");
            dialogPortraits[(int)SpeakerId.ChaseBoss] = content.Load<Texture2D>("UI\\portrait_spikeface");
            dialogPortraits[(int)SpeakerId.FinalBoss] = content.Load<Texture2D>("UI\\portrait_finalboss");
            dialogPortraits[(int)SpeakerId.FireBoss] = content.Load<Texture2D>("UI\\portrait_spikeface");
            dialogPortraits[(int)SpeakerId.JetBoss] = content.Load<Texture2D>("UI\\portrait_jetboss");
            dialogPortraits[(int)SpeakerId.ArmorBoss] = content.Load<Texture2D>("UI\\portrait_armorboss");
            dialogPortraits[(int)SpeakerId.IceSnake] = content.Load<Texture2D>("UI\\portrait_icesnake");
            dialogPortraits[(int)SpeakerId.CommandBoss] = content.Load<Texture2D>("UI\\portrait_rockboss");
            dialogPortraits[(int)SpeakerId.System] = content.Load<Texture2D>("UI\\portrait_system");
            
        }

        static DialogBox()
        {
            dialogLibrary = new List<DialogChunk>();
            Stream stream = TitleContainer.OpenStream("Dialog\\dialog.xml");
            //Stream stream = new FileStream("dialog.xml", FileMode.Open, FileAccess.ReadWrite);
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
                    currentDialog.FitToBox();
                }
            }
            if(currentDialog.pause == true)
                Engine.player.state = State.Dialog;

            stage = 0;
            lifeTime = 0;
            animationTime = 0;
            state = BoxState.Appear;                        
        }

        public static void SetCustomDialog(String message)
        {
            currentDialog = new DialogChunk();
            currentDialog.pause = false;
            currentDialog.textList = new List<string>();
            currentDialog.textList.Add(message);
            currentDialog.FitToBox();
            currentDialog.speaker = SpeakerId.OldMan;
            currentDialog.id = "Custom";
            if (currentDialog.pause == true)
                Engine.player.state = State.Dialog;

            stage = 0;
            lifeTime = 0;
            animationTime = 0;
            state = BoxState.Appear;
        }

        public static SpeakerId GetSpeaker(String id)
        {
            foreach (DialogChunk chunk in dialogLibrary)
            {
                if (chunk.id == id)
                {
                    return chunk.speaker;
                }
            }
            return SpeakerId.OldMan;

        }

        public void Draw()
        {
            //if (currentDialog != null && stage < currentDialog.textList.Count && ((currentDialog.pause == false && Engine.player.state != State.Dialog && (lifeTime < maxLifeTime)) || Engine.player.state == State.Dialog))
            if (currentDialog != null && state != BoxState.None)
            {
                Engine.spriteBatch.Begin();
                float dialogBoxHeight = Game1.titleSafeRect.Height / 6;
                float dialogBoxLength = dialogBoxHeight * 8;
                //Engine.spriteBatch.Draw(dialogRenderTarget, new Rectangle((int)(Game1.titleSafeRect.Center.X - dialogBoxLength/2), (int)(Game1.titleSafeRect.Bottom - dialogBoxHeight - 50), (int)dialogBoxLength, (int)dialogBoxHeight), Color.White);

                int w = Game1.titleSafeRect.Height/7;

                int dialogBottom = Game1.titleSafeRect.Bottom - w/2;
                int dialogTop = dialogBottom - w;
                int dialogLeft = Game1.titleSafeRect.Center.X - 4*w;
                int dialogRight = Game1.titleSafeRect.Center.X + 4*w;

                if (state == BoxState.Appear || state == BoxState.Vanish)
                {
                    float scale = (1f * animationTime) / appearMaxTime;
                    if (state == BoxState.Vanish)
                        scale = 1f - scale;

                    Engine.spriteBatch.Draw(monitor, new Rectangle((int)(dialogLeft + w / 2 - scale * w / 2), (int)(dialogTop + w / 2 - scale * w / 2), (int)(w * scale), (int)(w * scale)), Color.White);
                    Engine.spriteBatch.Draw(monitorFrame, new Rectangle((int)(dialogLeft + w / 2 - scale * w / 2), (int)(dialogTop + w / 2 - scale * w / 2), (int)(w * scale), (int)(w * scale)), Color.White);
                    Engine.spriteBatch.Draw(rightEdge, new Rectangle((int)(dialogLeft +w / 2), (int)(dialogTop + w / 2 - scale * w / 2), (int)(w * scale), (int)(w * scale)), Color.White);
                }
                else if (state == BoxState.Extend || state == BoxState.Close)
                {
                    float slide = (1f * animationTime) / extendMaxTime;
                    if (state == BoxState.Close)
                        slide = 1f - slide;

                    Engine.spriteBatch.Draw(textArea, new Rectangle(dialogLeft + w / 2, dialogTop, (int)((slide * (dialogRight - w) + (1 - slide) * (dialogLeft + w / 2) - dialogLeft)), w), Color.White);

                    Engine.spriteBatch.Draw(monitor, new Rectangle(dialogLeft, dialogTop, w, w), Color.White);
                    Engine.spriteBatch.Draw(rightEdge, new Rectangle((int)(slide * (dialogRight - w) + (1 - slide) * (dialogLeft + w / 2)), dialogTop, w, w), Color.White);

                    Color fadeColor = Color.White;
                    fadeColor.A = (Byte)(slide * 255);
                    fadeColor.R = (Byte)(slide * 255);
                    fadeColor.G = (Byte)(slide * 255);
                    fadeColor.B = (Byte)(slide * 255);
                    
                    Engine.spriteBatch.Draw(dialogPortraits[(int)currentDialog.speaker], new Rectangle(dialogLeft, dialogTop, w, w), fadeColor);
                    Engine.spriteBatch.Draw(monitorFrame, new Rectangle(dialogLeft, dialogTop, w, w), Color.White);
                }
                else
                {
                    Engine.spriteBatch.Draw(textArea, new Rectangle(dialogLeft + w / 2, dialogTop, 7 * w, w), Color.White);
                    Engine.spriteBatch.Draw(monitor, new Rectangle(dialogLeft, dialogTop, w, w), Color.White);
                    Engine.spriteBatch.Draw(rightEdge, new Rectangle(dialogRight - w, dialogTop, w, w), Color.White);

                    int portraitIndex = (int)currentDialog.speaker;
                    Engine.spriteBatch.Draw(dialogPortraits[(int)currentDialog.speaker], new Rectangle(dialogLeft, dialogTop, w, w), Color.White);
                    Engine.spriteBatch.Draw(monitorFrame, new Rectangle(dialogLeft, dialogTop, w, w), Color.White);
                }

                if (state == BoxState.Text || state == BoxState.Wait)
                {
                    //Engine.spriteBatch.DrawString(currentFont, currentText, new Vector2(dialogLeft + 1.15f*w, dialogTop + .15f*w), Color.YellowGreen);
                    SpriteUtil.DrawString(Engine.spriteBatch, currentText, new Vector2(dialogLeft + 1.15f * w, dialogTop + .15f * w), Color.YellowGreen);                    
                }
                if(state == BoxState.Wait && currentDialog.pause == true)
                    Engine.spriteBatch.Draw(spaceBar, new Rectangle((int)(dialogLeft + w / 2 + 6.21 * w), dialogTop, (int)(.8f * w), w), Color.White);

                Engine.spriteBatch.End();
            }
        }

        public bool Next()
        {
            if (currentDialog == null)
            {
                if (state != BoxState.Close)
                {
                    state = BoxState.Close;
                    if(IntroOverlay.state != IntroState.FadeIn)
                        SoundFX.DialogExtend();
                }
                animationTime = 0;
                return false;
            }
            if (cooldown > 0)
                return true;
            stage++;
            cooldown = 300;
            state = BoxState.Text;
            animationTime = 0;
            currentCharacter = 0;
            currentText = "";
            if (stage >= currentDialog.textList.Count)
            {
                state = BoxState.Close;
                animationTime = 0;
                return false;
            }
            return true;
        }

        public void Update(int gameTime)
        {
            if (DialogBox.currentFont == null) DialogBox.currentFont = DialogBox.newFont;
            if (currentFont != newFont)
            {
                currentFont = newFont;
            }
            lifeTime += gameTime;
            animationTime += gameTime;
            if (state == BoxState.Appear)
            {
                if (animationTime > appearMaxTime)
                {
                    SoundFX.DialogExtend();
                    state = BoxState.Extend;
                    animationTime = 0;
                }
            }
            else if (state == BoxState.Extend)
            {
                if (animationTime > extendMaxTime)
                {
                    state = BoxState.Text;
                    currentText = "";
                    currentCharacter = 0;
                    animationTime = 0;
                }
            }
            else if (state == BoxState.Vanish)
            {
                if (animationTime > appearMaxTime)
                {
                    state = BoxState.None;
                    animationTime = 0;
                }
            }
            else if (state == BoxState.Close)
            {
                if (animationTime > extendMaxTime)
                {
                    state = BoxState.Vanish;
                    animationTime = 0;
                }
            }

            if (state == BoxState.Text)
            {
                bool playSound = false;
                while (animationTime > charMaxTime && state == BoxState.Text)
                {
                    currentText += currentDialog.textList[stage][currentCharacter];
                    if (currentDialog.textList[stage][currentCharacter] != ' ' && currentDialog.textList[stage][currentCharacter] != '\n')
                        playSound = true;
                    currentCharacter++;                    
                    animationTime -= charMaxTime;
                    lifeTime = 0;
                    if (currentCharacter >= currentDialog.textList[stage].Length)
                        state = BoxState.Wait;
                }
                if(playSound)
                    SoundFX.DialogCharacter();
            }

            cooldown -= gameTime;
            if (cooldown < 0)
                cooldown = 0;

            if (Engine.player.state == State.Dialog)
            {
                lifeTime = 0;
            }
            else
            {
                if (lifeTime > maxLifeTime && state != BoxState.Close && state != BoxState.Vanish && state != BoxState.None)
                {
                    state = BoxState.Close;
                    SoundFX.DialogExtend();
                    animationTime = 0;
                }
            }
        }
    }
}
