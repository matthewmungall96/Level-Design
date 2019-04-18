using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ANDGateEvent : MonoBehaviour
{
    public UnityEvent onTrue;

    bool leftSide;
    bool rightSide;

    public void SetLeftSide(bool leftSide)
    {
        this.leftSide = leftSide;

        if (rightSide && leftSide)
            onTrue.Invoke();
    }

    public void SetRightSide(bool rightSide)
    {
        this.rightSide = rightSide;

        if (rightSide && leftSide)
            onTrue.Invoke();
    }
}
