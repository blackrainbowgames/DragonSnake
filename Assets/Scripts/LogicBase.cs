using UnityEngine;

public abstract class LogicBase : MonoBehaviour
{
    public Engine Engine
    {
        get
        {
            return Engine.Instance;
        }
    }
}