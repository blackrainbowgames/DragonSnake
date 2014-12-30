using System;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class Controller : LogicBase
{
    public ControllerType ControllerType;

    private readonly Dictionary<int, GameButton> _pressed = new Dictionary<int, GameButton>();
    private readonly Dictionary<int, Touch> _touches = new Dictionary<int, Touch>();

    public void Update()
    {
        if (Input.touchCount == 0) return;

        switch (Profile.ControllerType)
        {
            case ControllerType.D:
                HandleSlideController();
                break;
            default:
                HandleArrowsController();
                break;
        }
    }

    private void HandleArrowsController()
    {
        foreach (var touch in Input.touches)
        {
            var pos = Engine.Camera.ScreenToWorldPoint(touch.position);

            pos.z = -10;

            if (Physics.Raycast(pos, Vector3.forward)) continue;

            if (touch.phase == TouchPhase.Began)
            {
                var arrows = FindObjectsOfType<ControllerArrow>();
                var nearest = arrows[0];

                for (var i = 1; i < arrows.Length; i++)
                {
                    if (Vector2.Distance(pos, arrows[i].transform.position) < Vector2.Distance(pos, nearest.transform.position))
                    {
                        nearest = arrows[i];
                    }
                }

                var button = nearest.GetComponent<GameButton>() ?? nearest.GetComponentInChildren<GameButton>();

                button.OnPress(true);
                _pressed.Add(touch.fingerId, button);
            }

            if (touch.phase == TouchPhase.Ended)
            {
                var button = _pressed[touch.fingerId];

                button.OnPress(false);
                _pressed.Remove(touch.fingerId);
            }
        }
    }

    private void HandleSlideController()
    {
        foreach (var touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                _touches.Add(touch.fingerId, touch);
            }

            if (touch.phase == TouchPhase.Moved && _touches.ContainsKey(touch.fingerId))
            {
                var delta = touch.position - _touches[touch.fingerId].position;

                if (Mathf.Abs(delta.x) > 20 || Mathf.Abs(delta.y) > 20)
                {
                    _touches.Remove(touch.fingerId);
                    PressArrow(delta);
                }
            }

            if (touch.phase == TouchPhase.Ended && _touches.ContainsKey(touch.fingerId))
            {
                var delta = touch.position - _touches[touch.fingerId].position;

                _touches.Remove(touch.fingerId);
                PressArrow(delta);
            }
        }
    }

    private void PressArrow(Vector2 delta)
    {
        Vector2 direction;

        if (Mathf.Abs(delta.x) > Math.Abs(delta.y))
        {
            direction = delta.x < 0 ? -Vector2.right : Vector2.right;
        }
        else
        {
            direction = delta.y < 0 ? -Vector2.up : Vector2.up;
        }

        Engine.Interface.ArrowPressed(direction);
    }
}