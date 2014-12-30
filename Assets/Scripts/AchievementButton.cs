using Assets.Scripts;

public class AchievementButton : GameButton
{
    public Achivements Achivement;

    public new void OnPress(bool isDown)
    {
        if (!enabled || !Panel.Displayed) return;

        Tween(isDown);

        if (Listener != null)
        {
            if (!string.IsNullOrEmpty(ListenerMethodDown) && isDown)
            {
                Listener.SendMessage(ListenerMethodDown, Achivement);
            }

            if (!string.IsNullOrEmpty(ListenerMethodUp) && !isDown)
            {
                Listener.SendMessage(ListenerMethodUp, Achivement);
            }
        }
    }
}