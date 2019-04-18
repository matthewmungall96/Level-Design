using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

namespace UniProject
{
    public class Player : MonoBehaviour
    {
        private static Player instance;

        public static Player Instance { get { return instance; } }

        public PlayerInteraction GetInteractionSystem { get; private set; }
        public FirstPersonController GetFirstPersonController { get; private set; }

        public GameObject timePiece;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
            }

            GetInteractionSystem = GetComponent<PlayerInteraction>();
            GetFirstPersonController = GetComponent<FirstPersonController>();
        }

        public void EnableTimePiece()
        {
            timePiece.GetComponentInChildren<UniverseController>().IsLocked = false;
        }

        public void DisableTimePiece()
        {
            timePiece.GetComponentInChildren<UniverseController>().IsLocked = true;
        }

        public void SetHasTimePiece()
        {
            GameManager.Instance.GetPersistedLevelData.HasTimePiece = true;
            EnableTimePiece();
        }
    }
}