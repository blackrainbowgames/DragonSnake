using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Assets.Scripts;
using UnityEngine;

public class Interface : LogicBase
{
    public GameObject FillerSprite;
    public GameObject ControllerA;
    public GameObject[] ControllerB;
    public GameObject[] ControllerC;
    public GameObject ControllerD;
    public UILabel GameMessage;
    public UILabel Score;
    public UILabel ScoreSlash;
    public UILabel TargetScore;
    public UISprite[] Stars;
    public UILabel Achievment;
    public UILabel AchievmentDescription;
    public GameObject Controller;
    public GameObject ScoreGroup;

    public void Start()
    {
        UpdateControllerType();
        UpdateMuteButton();

        Input.multiTouchEnabled = true;
    }

    private readonly Dictionary<int, Touch> _touches = new Dictionary<int, Touch>();

    public void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        HandleControllerD();
    }

    private void HandleControllerD()
    {
        var handle = (Engine.GameRunning && Profile.ControllerType == ControllerType.D);

        if (!handle || Input.touchCount == 0) return;

        foreach (var touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                _touches.Add(touch.fingerId, touch);
            }

            if (touch.phase != TouchPhase.Ended) continue;

            var delta = touch.position - _touches[touch.fingerId].position;

            _touches.Remove(touch.fingerId);

            if (Mathf.Abs(delta.x) > Math.Abs(delta.y))
            {
                if (delta.x < 0)
                {
                    TurnLeft();
                }
                else
                {
                    TurnRight();
                }
            }
            else
            {
                if (delta.y > 0)
                {
                    TurnUp();
                }
                else
                {
                    TurnDown();
                }
            }
        }
    }

    private void StartGame(Drake drake)
    {
        if (Engine.IsDrakeLocked(drake)) return;

        Engine.PanelManager.ShowGamePanels();
        Engine.TaskScheduler.CreateTask(() => { if (Engine.GameRunning) { TweenAlpha.Begin(FillerSprite, 1, 20 / 255f); } }, 1);

        foreach (var game in FindObjectsOfType<Game>())
        {
            if (game.Drake == drake)
            {
                game.Sprite.enabled = game.SpriteGray.enabled = true;
                game.StartGame();
            }
            else
            {
                game.Sprite.enabled = game.SpriteGray.enabled = false;
                game.StopGame();
            }
        }

        Countdown();
    }

    private void UpdateControllerType()
    {
        ControllerA.SetActive(false);
        foreach (var c in ControllerB) c.SetActive(false);
        foreach (var c in ControllerC) c.SetActive(false);
        ControllerD.SetActive(false);

        #if UNITY_WEBPLAYER

        return;

        #endif

        switch (Profile.ControllerType)
        {
            case ControllerType.A:
                ControllerA.SetActive(true);
                break;
            case ControllerType.B:
                foreach (var c in ControllerB) c.SetActive(true);
                break;
            case ControllerType.C:
                foreach (var c in ControllerC) c.SetActive(true);
                break;
            case ControllerType.D:
                ControllerD.SetActive(true);
                break;
            default:
                throw new Exception();
        }
    }

    private void Countdown()
    {
        UpdateScoreStars();
        Score.text = TargetScore.text = "";
        ScoreSlash.text = "3";
        Engine.TaskScheduler.CreateTask(() => ScoreSlash.text = "3", 0);
        Engine.TaskScheduler.CreateTask(() => ScoreSlash.text = "2", 1);
        Engine.TaskScheduler.CreateTask(() => ScoreSlash.text = "1", 2);
        Engine.TaskScheduler.CreateTask(() => ScoreSlash.text = "GO!", 3);
        Engine.TaskScheduler.CreateTask(UpdateScore, 4);
    }

    public void UpdateScore()
    {
        var score = Engine.Game.Snake.Count;

        Score.SetText(score);
        ScoreSlash.text = "/";

        UpdateScoreStars();

        foreach (var target in Engine.Game.Targets)
        {
            if (target <= score) continue;

            TargetScore.SetText(target);

            return;
        }

        TargetScore.SetText("+");
    }

    public void UpdateScoreStars()
    {
        for (var i = 0; i < 3; i++)
        {
            Stars[i].color = Engine.Game.Snake.Count >= Engine.Game.Targets[Engine.Game.Targets.Length - 3 + i]
                ? ColorHelper.GetColor(255, 180, 0, 255) : ColorHelper.GetColor(0, 0, 0, 0);
        }
    }

    // ReSharper disable UnusedMember.Local

    private void ExitGame()
    {
        Application.Quit();
    }

    private void MuteSound()
    {
        Profile.Mute = !Profile.Mute;
        Profile.Save();
        Engine.AudioPlayer.Mute = Profile.Mute;
        UpdateMuteButton();
    }

    private static void UpdateMuteButton()
    {
        GameObject.Find("MuteSound").GetComponent<UISprite>().spriteName = Profile.Mute ? "UnMute" : "Mute";
    }

    private void PlayAstarot()
    {
        StartGame(Drake.Astarot);
    }

    private void PlayLeviathan()
    {
        StartGame(Drake.Leviathan);
    }

    private void PlayRyuu()
    {
        StartGame(Drake.Ryuu);
    }

    private void PlayViper()
    {
        StartGame(Drake.Viper);
    }

    private void PlayVolos()
    {
        StartGame(Drake.Volos);
    }

    public void StopGame()
    {
        Engine.Game.StopGame();
        Engine.Field.Reset();
        Engine.TaskScheduler.Reset();
        Engine.PanelManager.ShowMenuPanels();
        TweenAlpha.Begin(FillerSprite, 0.25f, 0);
        
        foreach (var stageStatus in FindObjectsOfType<StageStatus>())
        {
            stageStatus.Refresh();
        }
    }

    private void PauseGame()
    {
        if (Engine.Game.State == GameState.Initializing) return;

        if (Engine.Game.State == GameState.Running)
        {
            Engine.Game.PauseGame();
        }
        else
        {
            Engine.Game.ResumeGame();
        }

        Controller.SetActive(Engine.Game.State == GameState.Paused);
        ScoreGroup.SetActive(Engine.Game.State != GameState.Paused);
    }

    private void ReplayGame()
    {
        Engine.Field.Reset();
        Engine.TaskScheduler.Reset();
        StartGame(Engine.Game.Drake);
    }

    private void ChangeControls()
    {
        if (++Profile.ControllerType > ControllerType.D)
        {
            Profile.ControllerType = ControllerType.A;
        }

        UpdateControllerType();
        Profile.Save();
    }

    private void TurnLeft()
    {
        ArrowPressed(-Vector2.right);
    }

    private void TurnRight()
    {
        ArrowPressed(Vector2.right);
    }

    private void TurnUp()
    {
        ArrowPressed(Vector2.up);
    }

    private void TurnDown()
    {
        ArrowPressed(-Vector2.up);
    }

    public void ArrowPressed(Vector2 direction)
    {
        if (Engine.GameRunning)
        {
            Engine.Game.ArrowPressed(direction);
        }
    }

    private void SelectAchievement(Achivements achivement)
    {
        string text;

        switch (achivement)
        {
            case Achivements.DummySnake:
                text = "%DummySnake%"; break;
            case Achivements.LongSnake:
                text = "%LongSnake%"; break;
            case Achivements.BossSnake:
                text = "%BossSnake%"; break;
            case Achivements.MegaSnake:
                text = "%MegaSnake%"; break;
            case Achivements.KingSnake:
                text = "%KingSnake%"; break;
            case Achivements.ChuckNorrisSnake:
                text = "%ChuckNorrisSnake%"; break;
            case Achivements.LuckySnake:
                text = "%LuckySnake%"; break;
            case Achivements.SpeedySnake:
                text = "%SpeedySnake%"; break;
            case Achivements.EpicFailSnake:
                text = "%EpicFailSnake%"; break;
            case Achivements.JerkSnake:
                text = "%JerkSnake%"; break;
            case Achivements.AstarotScale:
                text = "%AstarotScale%"; break;
            case Achivements.LeviathanScale:
                text = "%LeviathanScale%"; break;
            case Achivements.RyuuScale:
                text = "%RyuuScale%"; break;
            case Achivements.ViperScale:
                text = "%ViperScale%"; break;
            case Achivements.VolosScale:
                text = "%VolosScale%"; break;
            case Achivements.UnlockViper:
                text = "%UnlockViper%"; break;
            case Achivements.UnlockVolos:
                text = "%UnlockVolos%"; break;
            case Achivements.GameMaster:
                text = "%GameMaster%"; break;
            case Achivements.GameKing:
                text = "%GameKing%"; break;
            case Achivements.Played1Hours:
                text = "%Played1Hours%"; break;
            case Achivements.Played2Hours:
                text = "%Played2Hours%"; break;
            case Achivements.Played3Hours:
                text = "%Played3Hours%"; break;
            case Achivements.Played4Hours:
                text = "%Played4Hours%"; break;
            case Achivements.Eaten100Apples:
                text = "%Eaten100Apples%"; break;
            case Achivements.Eaten500Apples:
                text = "%Eaten500Apples%"; break;
            case Achivements.Eaten1000Apples:
                text = "%Eaten1000Apples%"; break;
            case Achivements.Eaten2500Apples:
                text = "%Eaten2500Apples%"; break;
            default:
                throw new Exception();
        }

        foreach (Match match in new Regex("%.+%").Matches(text))
        {
            text = text.Replace(match.Value, Localization.Localize(match.Value));
        }

        var info = text.Split(Convert.ToChar("\n"));

        Achievment.SetLocalizedText(info[0]);
        AchievmentDescription.SetLocalizedText(info[1]);
        Engine.PanelManager.ShowAchievement();
    }

    // ReSharper restore UnusedMember.Local
}