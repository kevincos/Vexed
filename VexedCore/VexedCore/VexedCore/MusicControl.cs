using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace VexedCore
{
    public class MusicControl
    {
        public static Song music_menu;
        public static Song music_game;
        public static Song greenSector;
        public static Song hubSector;
        public static Song engineSector;
        public static Song storageSector;
        public static Song commandSector;
        public static Song coreSector;
        public static Song itemRoom;
        public static Song upgrade;
        public static Song boss;
        public static Song finalBoss;
        public static Song intro;
        public static Song firstLoad;
        public static Song death;

        public static Song currentTrack;
        public static bool upgrading = false;
        public static bool loadedMusic = false;        

        public static Song GetCurrentRoomTrack()
        {
            if (loadedMusic == true || (currentTrack == firstLoad && MediaPlayer.State == MediaState.Playing))
            {
                loadedMusic = false;
                return firstLoad;
            }

            if (Engine.player.state == State.Death)
                return death;

            if (Engine.player.state == State.Upgrade || (upgrading == true && Engine.player.state == State.Dialog))
            {
                upgrading = true;
                
                return upgrade;
            }
            upgrading = false;

            if (Engine.player.currentRoom.id.Contains("Menu"))
                return intro;

            if (Engine.player.currentRoom.id.Contains("FinalBoss"))
                return finalBoss;

            if (Engine.player.currentRoom.bossCleared == false && Engine.player.currentRoom.id.Contains("Boss_"))
                return boss;

            if (Engine.player.currentRoom.bossCleared == true && Engine.player.currentRoom.id.Contains("Boss_"))
                return itemRoom;
            foreach (Doodad d in Engine.player.currentRoom.doodads)
            {
                if (d.type == VL.DoodadType.UpgradeStation && d.abilityType != AbilityType.Empty)
                    return itemRoom;
            }

            if (Engine.player.currentRoom.sectorID.Contains("Green"))
                return greenSector;

            if (Engine.player.currentRoom.sectorID.Contains("Hub"))
            {
                if (Engine.player.currentRoom.id.Contains("Core_"))
                    return coreSector;
                return hubSector;
            }

            if (Engine.player.currentRoom.sectorID.Contains("Storage"))
                return storageSector;

            if (Engine.player.currentRoom.sectorID.Contains("Engine"))
                return engineSector;

            return intro;
        }


        public static void PlayMenuMusic()
        {
            if (Engine.musicEnabled)
            {
                if (currentTrack != music_menu)
                {                    
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Play(music_menu);                    
                        
                    currentTrack = music_menu;
                }
            }
        }

        public static void PlayGameMusic()
        {
            if (Engine.musicEnabled)
            {
                Song expectedTrack = GetCurrentRoomTrack();
                if (currentTrack != expectedTrack)
                {
                    if (expectedTrack != upgrade && expectedTrack != death && expectedTrack != firstLoad)
                        MediaPlayer.IsRepeating = true;
                    else
                        MediaPlayer.IsRepeating = false;
                    MediaPlayer.Play(expectedTrack);
                    currentTrack = expectedTrack;
                }
            }
        }

        public static void Stop()
        {
            MediaPlayer.Stop();
            currentTrack = null;
        }

        public static void Pause()
        {
            MediaPlayer.Pause();
        }

        public static void Resume()
        {
            if (Engine.musicEnabled)
            {
                MediaPlayer.Resume();
                /*if (Game.metaState == MetaState.GamePlay)
                {
                    if (currentTrack == music_game)
                        MediaPlayer.Resume();
                    else
                    {
                        currentTrack = music_game;
                        MediaPlayer.Play(currentTrack);
                    }
                }
                else
                {
                    if (currentTrack == music_menu)
                        MediaPlayer.Resume();
                    else
                    {
                        currentTrack = music_menu;
                        MediaPlayer.Play(currentTrack);
                    }
                }*/
            }
        }

    }
}
