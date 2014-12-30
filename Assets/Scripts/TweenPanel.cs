using System;
using Assets.Scripts;
using UnityEngine;

public class TweenPanel : MonoBehaviour
{
	public PanelTweenPosition PanelTweenPosition;
    public bool UseCustomTweenPosition;
    public Vector3 CustomTweenPosition;
    public float DefaultTimeout = 0.4f;

    public void Show()
    {
        Tween(true, DefaultTimeout, PanelTweenPosition);
    }

    public void Show(float timeout)
    {
        Tween(true, timeout, PanelTweenPosition);
    }

    public void Show(PanelTweenPosition panelTweenPosition)
    {
        Tween(true, DefaultTimeout, panelTweenPosition);
    }

    public void Hide()
    {
        Tween(false, DefaultTimeout, PanelTweenPosition);
    }

    public void Hide(float timeout)
    {
        Tween(false, timeout, PanelTweenPosition);
    }

    public void Hide(PanelTweenPosition panelTweenPosition)
    {
        Tween(false, DefaultTimeout, panelTweenPosition);
    }

    public void Switch(bool show)
    {
        Tween(show, DefaultTimeout, PanelTweenPosition);
    }

    public void Switch(bool show, float timeout)
    {
        Tween(show, timeout, PanelTweenPosition);
    }

    public bool Displayed
    {
        get
        {
            return transform.localPosition == Vector3.zero;
        }
    }

    private void Tween(bool show, float timeout, PanelTweenPosition panelTweenPosition)
    {
        Vector3 vector;

        if (show)
        {
            if (!Displayed)
            {
                Tween(false, 0, panelTweenPosition);
            }

            vector = Vector3.zero;
        }
        else
        {
            if (UseCustomTweenPosition)
            {
                vector = new Vector2(CustomTweenPosition.x * transform.localScale.x, CustomTweenPosition.y * transform.localScale.y);
            }
            else
            {
                var cam = GameObject.Find("Camera").GetComponent<Camera>();

                switch (panelTweenPosition)
                {
                    case PanelTweenPosition.Left:
                        vector = -Vector2.right * Settings.ManualNguiHeight * cam.aspect;
                        break;
                    case PanelTweenPosition.Right:
                        vector = Vector2.right * Settings.ManualNguiHeight * cam.aspect;
                        break;
                    case PanelTweenPosition.Top:
						vector = Vector2.up * Settings.ManualNguiHeight;
                        break;
                    case PanelTweenPosition.Bottom:
						vector = -Vector2.up * Settings.ManualNguiHeight;
                        break;
                    default:
                        throw new Exception();
                }
            }
        }

        if (Math.Abs(timeout - 0) < Settings.Epsilon)
        {
            transform.localPosition = vector;
        }
        else
        {
            TweenPosition.Begin(gameObject, timeout, vector);
        }

        foreach (var c in GetComponentsInChildren<Collider>())
        {
            c.enabled = show;
        }
    }
}