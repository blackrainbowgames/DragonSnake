using System;
using Assets.Scripts;
using UnityEngine;

public class Engine : MonoBehaviour
{
    public Interface Interface;
    public PanelManager PanelManager;
    public AudioPlayer AudioPlayer;
    public Field Field;
    public TaskScheduler TaskScheduler;
    public Camera Camera;

    public void Awake()
    {
        DetectLanguage();
        Profile.Initialize();
        InitializeEngine();
        PanelManager.Initialize();

        AudioPlayer.Mute = Profile.Mute;
        Screen.orientation = ScreenOrientation.Portrait;
        Screen.fullScreen = true;
        Instance = this;

        if (Profile.TimePlayed > 0)
        {
            PanelManager.ShowMenuPanels();
        }
        else
        {
            PanelManager.ShowIntroPanel();
        }
    }

    public static Engine Instance;

    public static Game Game
    {
        get
        {
            foreach (var game in FindObjectsOfType<Game>())
            {
                if (game.State != GameState.Ready)
                {
                    return game;
                }
            }

            throw new Exception();
        }
    }

    public bool GameRunning
    {
        get { return PanelManager.GamePanels[0].Displayed; }
    }

    public bool IsDrakeLocked(Drake drake)
    {
        switch (drake)
        {
            case Drake.Astarot:
            case Drake.Leviathan:
            case Drake.Ryuu:
                return false;
            case Drake.Viper:
                return CheckDrakeLocked(Drake.Ryuu);
            case Drake.Volos:
                return CheckDrakeLocked(Drake.Viper);
            default:
                throw new Exception();
        }
    }

    private static bool CheckDrakeLocked(Drake drake)
    {
        if (!Profile.Scores.ContainsKey(drake))
        {
            return true;
        }

        foreach (var game in FindObjectsOfType<Game>())
        {
            if (game.Drake == drake)
            {
                return Profile.Scores[drake] < game.PassTarget;
            }
        }

       throw new Exception();
    }

    private void InitializeEngine()
    {
        Interface = FindObjectOfType<Interface>();
        PanelManager = FindObjectOfType<PanelManager>();
        AudioPlayer = FindObjectOfType<AudioPlayer>();
        Field = FindObjectOfType<Field>();
        TaskScheduler = FindObjectOfType<TaskScheduler>();
        Camera = FindObjectOfType<Camera>();
    }

    private static void DetectLanguage()
    {
        switch (Application.systemLanguage)
        {
            case SystemLanguage.Russian:
                //Localization.instance.currentLanguage = "Russian";
                break;
            case SystemLanguage.German:
                //Localization.instance.currentLanguage = "German";
                break;
            case SystemLanguage.Unknown:
               
                #if UNITY_ANDROID

                var language = new AndroidJavaClass("java/util/Locale").CallStatic<AndroidJavaObject>("getDefault").Call<string>("getLanguage");

                switch (language.ToLowerInvariant())
                {
                    case "ru":
                        Localization.instance.currentLanguage = "Russian";
                        break;
                    case "de":
                        Localization.instance.currentLanguage = "German";
                        break;
                    default:
                        Localization.instance.currentLanguage = "English";
                        break;
                }

                #else

                //Localization.instance.currentLanguage = "English";

                #endif

                break;
            default:
                //Localization.instance.currentLanguage = "English";
                break;
        }
    }
}