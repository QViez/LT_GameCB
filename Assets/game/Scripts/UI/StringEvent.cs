using System;
using UnityEngine;
public class StringEvent : ScriptableObject
{
    public event Action<string> OnEventRaised;

    public void Raise(string value)
    {
        OnEventRaised?.Invoke(value);
    }
}