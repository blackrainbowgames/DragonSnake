using System;
using System.Collections.Generic;
using UnityEngine;

#if !UNITY_WEBPLAYER

using System.IO;

#endif

namespace Assets.Scripts
{
    public static class Profile
    {
        public static bool Mute = false;
        public static ControllerType ControllerType = ControllerType.B;
        public static int ApplesEaten;
        public static float TimePlayed;
        public static List<Achivements> Achivements = new List<Achivements>();
        public static Dictionary<Drake, int> Scores = new Dictionary<Drake, int>();

        public static void Initialize()
        {
            TimePlayed = 0;

            try
            {
                Load();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        #region Private

        #if !UNITY_WEBPLAYER

        private static string ProfilePath
        {
            get { return Path.Combine(Application.persistentDataPath, Settings.ProfilePath); }
        }

        #endif

        public static void Save()
        {
            #if !UNITY_WEBPLAYER

            File.WriteAllText(ProfilePath, ToString());

            #endif
        }

        public new static string ToString()
        {
            var contents = string.Format("{0};{1};{2};{3}",
                Mute, ControllerType, ApplesEaten, TimePlayed + Time.time);

            var achivements = new string[Achivements.Count];

            for (var i = 0; i < Achivements.Count; i++)
            {
                achivements[i] = Achivements[i].ToString();
            }

            contents += ";" + string.Join(",", achivements);

            foreach (var highscore in Scores)
            {
                contents += string.Format(";{0}:{1}", highscore.Key, highscore.Value);
            }

            return contents;
        }

        private static void Load()
        {
            #if !UNITY_WEBPLAYER

            Debug.Log("Reading: " + ProfilePath);

            var contents = File.ReadAllText(ProfilePath);
            var data = contents.Split(Convert.ToChar(";"));

            Mute = bool.Parse(data[0]);
            ControllerType = (ControllerType) Enum.Parse(typeof (ControllerType), data[1]);
            ApplesEaten = int.Parse(data[2]);
            TimePlayed = float.Parse(data[3]);

            if (!string.IsNullOrEmpty(data[4]))
            {
                var achivements = data[4].Split(Convert.ToChar(","));

                foreach (var achivement in achivements)
                {
                    Achivements.Add((Achivements) Enum.Parse(typeof (Achivements), achivement));
                }
            }

            if (data.Length <= 5) return;

            for (var i = 5; i < data.Length; i++)
            {
                var d = data[i].Split(Convert.ToChar(":"));
                var girl = (Drake) Enum.Parse(typeof (Drake), d[0]);
                var score = int.Parse(d[1]);

                Scores.Add(girl, score);
            }

            #endif
        }

        #endregion
    }
}