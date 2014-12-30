using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public static class AchivementsManager
    {
        private static readonly List<float> Timers = new List<float>();
        private static bool _jerk;

        public static void AppleEaten(int score)
        {
            if (score == 040) AddAchivement(Achivements.DummySnake);
            if (score == 050) AddAchivement(Achivements.LongSnake);
            if (score == 060) AddAchivement(Achivements.BossSnake);
            if (score == 070) AddAchivement(Achivements.MegaSnake);
            if (score == 080) AddAchivement(Achivements.KingSnake);
            if (score == 100) AddAchivement(Achivements.ChuckNorrisSnake);

            Timers.Add(Time.time);

            if (Timers.Count >= 2 && Timers[Timers.Count - 1] - Timers[Timers.Count - 2] <= 0.5f)
            {
                AddAchivement(Achivements.LuckySnake);
            }

            if (Timers.Count >= 5 && Timers[Timers.Count - 1] - Timers[Timers.Count - 5] <= 6)
            {
                AddAchivement(Achivements.SpeedySnake);
            }

            Profile.ApplesEaten++;

            if (Profile.ApplesEaten >= 0100) AddAchivement(Achivements.Eaten100Apples);
            if (Profile.ApplesEaten >= 0500) AddAchivement(Achivements.Eaten500Apples);
            if (Profile.ApplesEaten >= 1000) AddAchivement(Achivements.Eaten1000Apples);
            if (Profile.ApplesEaten >= 2500) AddAchivement(Achivements.Eaten2500Apples);
        }

        public static void GameCompleted(int score)
        {
            if (score + 1 == Engine.Game.LastTarget) AddAchivement(Achivements.EpicFailSnake);

            if (score >= Engine.Game.Targets[Engine.Game.Targets.Length - 3])
            {
                if (Engine.Game.Drake == Drake.Ryuu) AddAchivement(Achivements.UnlockViper);
                if (Engine.Game.Drake == Drake.Viper) AddAchivement(Achivements.UnlockVolos);
            }

            if (score >= Engine.Game.LastTarget)
            {
                if (Engine.Game.Drake == Drake.Astarot) AddAchivement(Achivements.AstarotScale);
                if (Engine.Game.Drake == Drake.Leviathan) AddAchivement(Achivements.LeviathanScale);
                if (Engine.Game.Drake == Drake.Ryuu) AddAchivement(Achivements.RyuuScale);
                if (Engine.Game.Drake == Drake.Viper) AddAchivement(Achivements.ViperScale);
                if (Engine.Game.Drake == Drake.Volos) AddAchivement(Achivements.VolosScale);

                if (Profile.Achivements.Contains(Achivements.AstarotScale)
                    && Profile.Achivements.Contains(Achivements.LeviathanScale)
                    && Profile.Achivements.Contains(Achivements.RyuuScale))
                {
                    AddAchivement(Achivements.GameMaster);
                }

                if (Profile.Achivements.Contains(Achivements.GameMaster)
                    && Profile.Achivements.Contains(Achivements.ViperScale) && Profile.Achivements.Contains(Achivements.VolosScale))
                {
                    AddAchivement(Achivements.GameKing);
                }
            }

            if (score >= Engine.Game.LastTarget)
            {
                if (_jerk)
                {
                    AddAchivement(Achivements.JerkSnake);
                }
                else
                {
                    _jerk = true;
                }
            }

            if (Profile.TimePlayed + Time.time >= 03600) AddAchivement(Achivements.Played1Hours);
            if (Profile.TimePlayed + Time.time >= 07200) AddAchivement(Achivements.Played2Hours);
            if (Profile.TimePlayed + Time.time >= 10800) AddAchivement(Achivements.Played3Hours);
            if (Profile.TimePlayed + Time.time >= 14400) AddAchivement(Achivements.Played4Hours);
        }

        private static void AddAchivement(Achivements achivement)
        {
            if (Profile.Achivements.Contains(achivement)) return;

            Profile.Achivements.Add(achivement);
            Profile.Save();

            var name = Localization.Localize("%" + achivement + "%").Split(Convert.ToChar("\n"))[0];

            Engine.Instance.TaskScheduler.CreateTask(() =>
                {
                    Engine.Instance.PanelManager.ShowAchievementUnlockedPanel(name);
                    Engine.Instance.AudioPlayer.PlayAchievement();
                }, 0.5f);
        }
    }
}