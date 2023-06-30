using UnityEngine;
using System;

public class AnimationCallback : MonoBehaviour
{
    private Action _beginCallback;
    private Action _midCallback;
    private Action _endCallback;

    public void InitCallback(Action beginCallback = null, Action midCallback = null, Action endCallback = null)
    {
        _beginCallback = beginCallback;
        _midCallback = midCallback;
        _endCallback = endCallback;
    }

    public void OnBeginEvent()
    {
        _beginCallback?.Invoke();
    }

    public void OnMidEvent()
    {
        _midCallback?.Invoke();
    }

    public void OnEndEvent()
    {
        _endCallback?.Invoke();
    }
}
