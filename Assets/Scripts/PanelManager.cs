using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine;

public class PanelManager : LogicBase
{
    public TweenPanel[] MenuPanels;
    public TweenPanel[] GamePanels;
    public TweenPanel[] AchievementsPanels;
    public TweenPanel[] IntroPanels;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (MenuPanels[0].Displayed)
            {
                Application.Quit();
            }
            else if (AchievementsPanels[0].Displayed)
            {
                ShowMenuPanels();
            }
            else if (Engine.GameRunning)
            {
                Engine.Interface.StopGame();
            }
            else
            {
                for (var i = 0; i < IntroPanels.Length; i++)
                {
                    if (IntroPanels[i].Displayed)
                    {
                        IntroPanels[i].Hide(PanelTweenPosition.Left);
                        ShowMenuPanels();
                        MenuPanels[0].Show(PanelTweenPosition.Right);
                        break;
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            NextIntroPanel();
        }
    }

    public void Initialize()
    {
        HidePanels(0);

        foreach (var panel in MenuPanels)
        {
            panel.Show(0f);
        }
    }

    public void ShowGamePanels()
    {
        HidePanels();
        GamePanels[0].Show();
        GamePanels[1].Show();
        GamePanels[4].Show();
        GamePanels[5].Show();
    }

    public void ShowMenuPanels()
    {
        HidePanels();

        foreach (var panel in MenuPanels) panel.Show();
    }

    public void ShowAchievementsPanel()
    {
        HidePanels();
        AchievementsPanels[0].Show();

        foreach (var label in AchievementsPanels[0].GetComponentsInChildren<UILabel>())
        {
            label.SetText("");
        }

        foreach (var button in AchievementsPanels[0].GetComponentsInChildren<AchievementButton>())
        {
            button.GetComponent<UISprite>().spriteName = Profile.Achivements.Contains(button.Achivement)
                ? button.Achivement.ToString()
                : "Locked";
        }
    }

    public void ShowAchievementUnlockedPanel(string text)
    {
        GamePanels[1].Hide();
        GamePanels[2].Show();
        GamePanels[2].GetComponentsInChildren<UILabel>().Single(i => i.name == "Achievement").SetLocalizedText(text);
        Engine.TaskScheduler.CreateTask(HideAchievementUnlockedPanel, 4f);
    }

    public void HideAchievementUnlockedPanel()
    {
        GamePanels[1].Show();
        GamePanels[2].Hide();
    }

    public void ShowAchievement()
    {
        if (!AchievementsPanels[1].Displayed)
        {
            AchievementsPanels[1].Show();
        }
    }

    public void ShowGameOver()
    {
        GamePanels[1].Hide();
        GamePanels[3].Show();
    }

    private void HidePanels(float delay)
    {
        foreach (var panel in MenuPanels) panel.Hide(delay);
        foreach (var panel in GamePanels) panel.Hide(delay);
        foreach (var panel in AchievementsPanels) panel.Hide(delay);
        foreach (var panel in IntroPanels) panel.Hide(delay);
    }

    private void HidePanels()
    {
        foreach (var panel in MenuPanels) panel.Hide();
        foreach (var panel in GamePanels) panel.Hide();
        foreach (var panel in AchievementsPanels) panel.Hide();
    }

    public void ShowIntroPanel()
    {
        HidePanels(0);
        IntroPanels[0].Show(0f);
    }

    // ReSharper disable UnusedMember.Local

    public void ShowIntroPanelAgain()
    {
        HidePanels();
        IntroPanels[0].Show(PanelTweenPosition.Right);
    }

    private void NextIntroPanel()
    {
        var displayed = new List<TweenPanel>();

        foreach (var p in IntroPanels)
        {
            if (p.Displayed)
            {
                displayed.Add(p);
                break;
            }
        }

        if (displayed.Count == 0) return;

        var panel = displayed[0];
        var index = Array.IndexOf(IntroPanels, panel);

        panel.Hide(PanelTweenPosition.Left);

        if (index == IntroPanels.Length - 1)
        {
            Engine.PanelManager.ShowMenuPanels();
            Engine.PanelManager.MenuPanels[0].Show(PanelTweenPosition.Right);
        }
        else
        {
            panel = IntroPanels[index + 1];
            panel.Show(PanelTweenPosition.Right);
        }
    }

    // ReSharper restore UnusedMember.Local
}