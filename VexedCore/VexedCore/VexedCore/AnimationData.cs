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
    public enum AnimationState
    {
        Idle =0,
        Jump =1,
        IdleRight =2,
        IdleLeft=3,
        RunRight=4,
        RunLeft=5,
        JumpRight=6,
        JumpLeft=7,
        RocketJumpRight=8,
        RocketJumpLeft=9,
        WallRight=10,
        WallLeft=11,
        BoostRight=12,
        BoostLeft=13,
        FlyRight=14,
        FlyLeft=15,
        IdleRightFiring = 16,
        IdleLeftFiring = 17,
        RunRightFiring = 18,
        RunLeftFiring = 19,
        JumpRightFiring = 20,
        JumpLeftFiring = 21,
        WallRightFiring = 22,
        WallLeftFiring = 23,
        FlyRightFiring = 24,
        FlyLeftFiring = 25
    }

    public class AnimationData
    {
        public int animation_base;
        public int animation_count;
    }

    public class AnimationControl
    {
        public static List<AnimationData> animationData;

        public static List<int> animationFrameQueue;
        public static AnimationState currentState = AnimationState.Idle;
        public static AnimationState previousState = AnimationState.Idle;
        public static int animationTime = 0;

        public static int frameTime = 200;

        public static void Init()
        {
            animationFrameQueue = new List<int>();
            animationData = new List<AnimationData>();
            for (int i = 0; i < 26; i++)
            {
                animationData.Add(new AnimationData());
            }
            for (int i = 0; i < 26; i++)
            {
                animationData[i].animation_base = 0;
                animationData[i].animation_count = 4;
            }
            animationData[(int)AnimationState.RunRight].animation_base = 16;
            animationData[(int)AnimationState.RunLeft].animation_base = 8;
            animationData[(int)AnimationState.JumpRight].animation_base = 32;
            animationData[(int)AnimationState.JumpRight].animation_count =1;
            animationData[(int)AnimationState.JumpLeft].animation_base = 24;
            animationData[(int)AnimationState.JumpLeft].animation_count = 1;
            animationData[(int)AnimationState.IdleRight].animation_base = 36;
            animationData[(int)AnimationState.IdleRight].animation_count = 1;
            animationData[(int)AnimationState.IdleLeft].animation_base = 28;
            animationData[(int)AnimationState.IdleLeft].animation_count = 1;
            animationData[(int)AnimationState.WallRight].animation_base = 48;
            animationData[(int)AnimationState.WallRight].animation_count = 1;
            animationData[(int)AnimationState.WallLeft].animation_base = 40;
            animationData[(int)AnimationState.WallLeft].animation_count = 1;
            animationData[(int)AnimationState.FlyRight].animation_base = 50;
            animationData[(int)AnimationState.FlyRight].animation_count = 1;
            animationData[(int)AnimationState.FlyLeft].animation_base = 42;
            animationData[(int)AnimationState.FlyLeft].animation_count = 1;
            animationData[(int)AnimationState.BoostRight].animation_base = 52;
            animationData[(int)AnimationState.BoostRight].animation_count = 1;
            animationData[(int)AnimationState.BoostLeft].animation_base = 44;
            animationData[(int)AnimationState.BoostLeft].animation_count = 1;

            animationData[(int)AnimationState.RunRightFiring].animation_base = 20;
            animationData[(int)AnimationState.RunLeftFiring].animation_base = 12;
            animationData[(int)AnimationState.JumpRightFiring].animation_base = 33;
            animationData[(int)AnimationState.JumpRightFiring].animation_count = 1;
            animationData[(int)AnimationState.JumpLeftFiring].animation_base = 25;
            animationData[(int)AnimationState.JumpLeftFiring].animation_count = 1;
            animationData[(int)AnimationState.IdleRightFiring].animation_base = 37;
            animationData[(int)AnimationState.IdleRightFiring].animation_count = 1;
            animationData[(int)AnimationState.IdleLeftFiring].animation_base = 29;
            animationData[(int)AnimationState.IdleLeftFiring].animation_count = 1;
            animationData[(int)AnimationState.WallRightFiring].animation_base = 49;
            animationData[(int)AnimationState.WallRightFiring].animation_count = 1;
            animationData[(int)AnimationState.WallLeftFiring].animation_base = 41;
            animationData[(int)AnimationState.WallLeftFiring].animation_count = 1;
            animationData[(int)AnimationState.FlyRightFiring].animation_base = 51;
            animationData[(int)AnimationState.FlyRightFiring].animation_count = 1;
            animationData[(int)AnimationState.FlyLeftFiring].animation_base = 43;
            animationData[(int)AnimationState.FlyLeftFiring].animation_count = 1;
        }

        public static void SetState(AnimationState newState)
        {
            previousState = currentState;
            currentState = newState;
            if (previousState != currentState)
            {
                animationFrameQueue.Clear();
                Fill();
            }
        }

        public static void Fill()
        {
            if (animationFrameQueue.Count == 0)
            {
                for (int i = 0; i < animationData[(int)currentState].animation_count; i++)
                {
                    animationFrameQueue.Add(animationData[(int)currentState].animation_base + i);
                }
            }
        }

        public static void Update(GameTime gameTime)
        {

            animationTime += gameTime.ElapsedGameTime.Milliseconds;

            while (animationTime > frameTime)
            {
                Fill();
                animationFrameQueue.RemoveAt(0);
                animationTime -= frameTime;
            }

            Fill();
        }

        public static int GetCurrentFrame()
        {
            return animationFrameQueue[0];
        }
    }
}
