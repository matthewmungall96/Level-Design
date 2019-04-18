using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationCallbackHandler : MonoBehaviour
{
    [SerializeField] UnityEvent onAnimationCallback;

    private void InvokeCallbackEvent()
    {
        if(onAnimationCallback != null)
        {
            onAnimationCallback.Invoke();
        }
    }
}
