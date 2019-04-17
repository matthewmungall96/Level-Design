using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UniverseChange : MonoBehaviour
{
    [SerializeField]
    UnityEvent onActivate;
    [SerializeField]
    UnityEvent onDeactivate;
    [SerializeField]
    Universes universe;

    private void Start()
    {
        UniverseController.onUniverseChanged = UniverseController.onUniverseChanged ?? new OnUniverseChanged();
        UniverseController.onUniverseChanged.AddListener(OnSelectedUniverseActive);
    }

    void OnSelectedUniverseActive(int universe)
    {
        Universes universeEnum = (Universes)universe;

        if(this.universe == universeEnum)
        {
            onActivate.Invoke();
        }
        else
        {
            onDeactivate.Invoke();
        }
    }
}
