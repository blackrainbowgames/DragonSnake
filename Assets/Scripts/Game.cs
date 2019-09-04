using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class Game : LogicBase
{
    public Drake Drake;
    public List<Vector2> Snake;
    public Vector2 Apple;
    public List<Vector2> DirectionNext;
    public Vector2 DirectionPrev;
    public Color SnakeColor;
    public Color AppleColor;
    public Color GreenAppleColor;
    public UISprite Sprite;
    public UISprite SpriteGray;
    public int[] Targets;
    [HideInInspector] public float DeltaTime;
    public float[] DeltaTimes;
    public GameState State;
    
    private float _fillAmount;

    public void StartGame()
    {
        DeltaTime = DeltaTimes[0];
        SpriteGray.fillAmount = _fillAmount = 1;

        Snake = new List<Vector2> { new Vector2((int) (Engine.Field.Size.x / 2f), (int) (Engine.Field.Size.y / 4f)) };

        for (var i = 0; i < 0; i++)
        {
            Snake.Add(Snake[0] - new Vector2(1, 0));
        }

        DirectionNext = new List<Vector2> { Vector2.up };
        DirectionPrev = Vector2.up;
        Engine.TaskScheduler.CreateTask(CreateSnake, 1);
        Engine.TaskScheduler.CreateTask(() => { CreateApple(); State = GameState.Running; }, 2);
        Engine.TaskScheduler.CreateTask(Move, 3);
        State = GameState.Initializing;
    }

    public void StopGame()
    {
        State = GameState.Ready;
    }

    public void PauseGame()
    {
        State = GameState.Paused;
    }

    public void ResumeGame()
    {
        State = GameState.Running;
    }

    public void Update()
	{
        if (State != GameState.Running) return;

	    var direction = new Vector2();

	    if (Input.GetKeyDown(KeyCode.LeftArrow))
	    {
            direction = -Vector2.right;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            direction = Vector2.right;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            direction = Vector2.up;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            direction = -Vector2.up;
        }
        
        if (direction != Vector2.zero)
        {
            ArrowPressed(direction);
        }

        if (_fillAmount < SpriteGray.fillAmount)
        {
            SpriteGray.fillAmount -= Time.deltaTime / 10;

            if (SpriteGray.fillAmount < 0)
            {
                SpriteGray.fillAmount = 0;
            }
        }
    }

    public void ArrowPressed(Vector2 direction)
    {
        DirectionNext = DirectionNext.Count == 0
            ? new List<Vector2> { direction }
            : new List<Vector2> { DirectionNext[DirectionNext.Count - 1], direction };
    }

    public int PassTarget
    {
        get { return Targets[Targets.Length - 3]; }
    }

    public int LastTarget
    {
        get { return Targets[Targets.Length - 1]; }
    }

    private void CreateSnake()
    {
        TweenColor.Begin(Engine.Field.Sprites[(int) Snake[0].x, (int) Snake[0].y].gameObject,
            DeltaTime, SnakeColor);
    }

    private void CreateApple()
    {
        var random = new CryptoRandom();
        var color = AppleColor;

        foreach (var target in Targets)
        {
            if (Snake.Count + 1 == target)
            {
                color = GreenAppleColor;
            }
        }
        
        while (true)
        {
            Apple = new Vector2(random.Next(0, (int) Engine.Field.Size.x),
                random.Next(0, (int) Engine.Field.Size.y));

            if (!Snake.Contains(Apple))
            {
                break;
            }
        }

        TweenColor.Begin(Engine.Field.Sprites[(int) Apple.x, (int) Apple.y].gameObject, DeltaTime, color);
    }

    private void Move()
    {
        if (State == GameState.Running || State == GameState.Paused)
        {
            FindObjectOfType<TaskScheduler>().CreateTask(Move, DeltaTime);
        }

        if (State != GameState.Running) return;

        var direction = DirectionNext.Count > 0 ? DirectionNext[0] : DirectionPrev;

        if (direction + DirectionPrev == Vector2.zero)
        {
            direction = DirectionPrev;
        }

        var head = Snake[0] + direction;
        var tail = Snake[Snake.Count - 1];

        if (head.x < 0 || head.x >= (int) Engine.Field.Size.x || head.y < 0 ||
            head.y >= (int) Engine.Field.Size.y || Snake.Contains(head))
        {
            Snake.Add(tail);
            CompleteGame();

            for (var i = 0; i < Snake.Count; i++)
            {
                var index = i;
                var sprite = Engine.Field.Sprites[(int) Snake[index].x, (int) Snake[index].y];

                Engine.TaskScheduler.CreateTask(() =>
                {
                    TweenAlpha.Begin(sprite.gameObject, 0.25f, 0.4f);
                    CreateSparks(Snake[index], SnakeColor);
                }, (i + 1)*0.05f);
            }

            return;
        }

        Snake.Reverse();
        Snake.Add(head);
        Snake.Reverse();

        if (head != Apple)
        {
            Snake.Remove(tail);
        }

        TweenColor.Begin(Engine.Field.Sprites[(int) head.x, (int) head.y].gameObject, DeltaTime, SnakeColor);

        if (head != tail)
        {
            TweenColor.Begin(Engine.Field.Sprites[(int) tail.x, (int) tail.y].gameObject, DeltaTime,
                Engine.Field.Color);
        }

        DirectionPrev = direction;

        if (DirectionNext.Count > 0)
        {
            DirectionNext.RemoveAt(0);
        }

        if (head == Apple)
        {
            CreateApple();
            Engine.Interface.UpdateScore();

            var up = false;

            for (var i = 0; i < Targets.Length; i++)
            {
                if (Snake.Count != Targets[i]) continue;

                DeltaTime = DeltaTimes[i];

                _fillAmount = Mathf.Max(0, 1 - Snake.Count / (float) PassTarget);

                up = true;
            }

            if (up)
            {
                Engine.AudioPlayer.PlayUp();
            }
            else
            {
                Engine.AudioPlayer.PlayAppleEaten();
            }

            CreateSparks(head, up ? GreenAppleColor : AppleColor);
            AchivementsManager.AppleEaten(Snake.Count);

            if (Snake.Count == Targets[Targets.Length - 3])
            {
                Engine.TaskScheduler.CreateTask(Engine.AudioPlayer.PlayStageCompleted, 0.5f);
            }
        }
    }

    private void CompleteGame()
    {
        var score = Snake.Count - 1; // TODO: Affected by Move() logic

        AchivementsManager.GameCompleted(score);

        if (Profile.Scores.ContainsKey(Drake))
        {
            if (score > Profile.Scores[Drake])
            {
                Profile.Scores[Drake] = score;
            }
        }
        else
        {
            Profile.Scores.Add(Drake, score);
        }

        Profile.Save();

        State = GameState.Completed;

        Engine.AudioPlayer.PlayPop();
        Engine.PanelManager.HideAchievementUnlockedPanel();
        Engine.PanelManager.ShowGameOver();
        Engine.TaskScheduler.CreateTask(Engine.AudioPlayer.PlayStageFailed, 0.5f);
        Engine.TaskScheduler.CreateTask(() => TweenAlpha.Begin(Engine.Interface.FillerSprite, 1, 0), 0.5f);
        Engine.Field.RePaint(new Color(1, 1, 1, 0), Snake, Apple);
    }

    private void CreateSparks(Vector2 position, Color color)
    {
        var sparks = PrefabsHelper.InstantiateSparks();
        var sprite = Engine.Field.Sprites[(int) position.x, (int) position.y];
        var ps = sparks.GetComponent<ParticleSystem>();

        ps.GetComponent<Renderer>().material.color = color;
        ps.maxParticles = new CryptoRandom().Next(4, 10);
        sparks.transform.position = sprite.transform.position;
        Destroy(sparks, ps.startLifetime);
    }
}