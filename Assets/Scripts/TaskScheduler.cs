using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class TaskScheduler : MonoBehaviour
{
    public event Action<int> TaskCompleted = id => { };
    public readonly List<int> Tasks = new List<int>();

    public void CreateTask(Action task, int id, float delay)
    {
        Tasks.Add(id);
        StartCoroutine(Coroutine(task, id, delay));
    }

    public void CreateTask(Action task, float delay)
    {
        var id = new CryptoRandom().Next(999999);

        Tasks.Add(id);
        StartCoroutine(Coroutine(task, id, delay));
    }

    public void Reset()
    {
        Tasks.Clear();
    }

    private IEnumerator Coroutine(Action task, int id, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!Tasks.Contains(id)) yield break;

        task();
        Tasks.Remove(id);
        TaskCompleted(id);
    }
}
