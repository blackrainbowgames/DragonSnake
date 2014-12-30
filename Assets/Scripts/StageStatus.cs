using Assets.Scripts;

public class StageStatus : LogicBase
{
    public Drake Drake;
    public UISprite Star1;
    public UISprite Star2;
    public UISprite Star3;
    public UISprite Lock;

    public void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        foreach (var game in FindObjectsOfType<Game>())
        {
            if (game.Drake == Drake)
            {
                var score = Profile.Scores.ContainsKey(Drake) ? Profile.Scores[Drake] : 0;
                var stars = new[] { Star1, Star2, Star3 };

                if (Engine.IsDrakeLocked(Drake))
                {
                    for (var i = 0; i < 3; i++)
                    {
                        stars[i].color = ColorHelper.GetColor(0, 0, 0, 0);
                    }

                    Lock.color = ColorHelper.GetColor(0, 0, 0, 40);
                }
                else
                {
                    for (var i = 0; i < 3; i++)
                    {
                        stars[i].color = score >= game.Targets[game.Targets.Length - 3 + i]
                                             ? ColorHelper.GetColor(255, 180, 0, 255)
                                             : ColorHelper.GetColor(0, 0, 0, 40);
                    }

                    Lock.color = ColorHelper.GetColor(0, 0, 0, 0);
                }

                return;
            }
        }
    }
}
