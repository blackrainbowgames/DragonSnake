using UnityEngine;

public class GameButton : MonoBehaviour
{
    public Color ColorDown = Color.white;
    public float ScaleDown = 1.2f;
    public float TweenTimeDown = 0.1f;
    public float TweenTimeUp = 0.4f;

    public MonoBehaviour Listener;
    public string ListenerMethodDown;
    public string ListenerMethodUp;

    protected TweenPanel Panel;
    protected Color Color;

    public void Start()
    {
        var parent = transform.parent;
        var i = 0;

        while (Panel == null && i < 100)
        {
            Panel = parent.GetComponent<TweenPanel>();
            parent = parent.parent;
            i++;
        }

        Color = GetComponent<UISprite>().color;
    }
	
	public void OnPress(bool isDown)
	{
        if (!enabled || !Panel.Displayed) return;

        Tween(isDown);
        
	    if (Listener != null)
	    {
            if (!string.IsNullOrEmpty(ListenerMethodDown) && isDown)
	        {
	            Listener.SendMessage(ListenerMethodDown);
	        }

            if (!string.IsNullOrEmpty(ListenerMethodUp) && !isDown)
	        {
	            Listener.SendMessage(ListenerMethodUp);
	        }
	    }
	}

    protected void Tween(bool isDown)
    {
        if (isDown)
        {
            TweenColor.Begin(gameObject, TweenTimeDown, ColorDown);
            TweenScale.Begin(gameObject, TweenTimeDown, ScaleDown * Vector3.one);
        }
        else
        {
            TweenColor.Begin(gameObject, TweenTimeUp, Color);
            TweenScale.Begin(gameObject, TweenTimeUp, Vector3.one);
        }
    }
}