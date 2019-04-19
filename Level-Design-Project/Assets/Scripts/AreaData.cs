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
            if(onSpawn != null)
                onSpawn.Invoke(alterativeTimeSceneName);
        }

        public void SetWaterworksPowered()
        {
            GameManager.Instance.GetPersistedLevelData.WaterworksPowered = true;
            GameManager.Instance.GetPersistedLevelData.RespawnFromWaterworks = true;
        }

        public void SetObservatoryPowered()
        {
            GameManager.Instance.GetPersistedLevelData.ObservatoryPowered = true;
            GameManager.Instance.GetPersistedLevelData.RespawnFromObservatory = true;
        }

        public void CloseGame()
        {
            Application.Quit();
        }
    }
}
