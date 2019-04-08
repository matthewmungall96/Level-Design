using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniProject
{
    public class AreaData : MonoBehaviour
    {
        public string alterativeTimeSceneName;

        public static Action<string> onSpawn;

        private void Awake()
        {
            onSpawn.Invoke(alterativeTimeSceneName);
        }
    }
}
