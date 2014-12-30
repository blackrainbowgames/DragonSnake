using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class Field : MonoBehaviour
{
    public Transform FieldParent;
    public Vector2 Size;
    public UISprite[,] Sprites;
    public Color Color;

    public void Awake()
    {
        const int height = 1024;
        var width = height * FindObjectOfType<Camera>().aspect;
        var step = new Vector2(width / Size.x, height / (Size.y + 2));

        Sprites = new UISprite[(int)Size.x, (int)Size.y];

        for (var x = 0; x < Size.x; x++)
        {
            for (var y = 0; y < Size.y; y++)
            {
                var cell = PrefabsHelper.InstantiateCell();

                cell.transform.parent = FieldParent;
                cell.transform.localPosition = new Vector2(x * step.x - width / 2f + step.x / 2f, y * step.y - height / 2f + step.y / 2f);
                cell.transform.localScale = new Vector2(step.x, step.y);
                Sprites[x, y] = cell.GetComponent<UISprite>();
            }
        }

        Reset();
    }

    public void Reset()
    {
        RePaint(Color);
    }

    public void RePaint(Color color)
    {
        for (var x = 0; x < Size.x; x++)
        {
            for (var y = 0; y < Size.y; y++)
            {
                TweenColor.Begin(Sprites[x, y].gameObject, 0.5f, color);
            }
        }
    }

    public void RePaint(Color color, List<Vector2> snake, Vector2 apple)
    {
        for (var x = 0; x < Size.x; x++)
        {
            for (var y = 0; y < Size.y; y++)
            {
                if (snake.Contains(new Vector2(x, y)) || apple == new Vector2(x, y)) continue;

                TweenColor.Begin(Sprites[x, y].gameObject, 0.5f, color);
            }
        }
    }
}