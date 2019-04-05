using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

// TODO Set Physics settings

namespace UniProject
{
    public class CameraLerperTrigger : MonoBehaviour
    {
        [SerializeField]
        Transform lerpTarget;

        Vector3 defaultPosition;
        Quaternion defaultRotation;

        public float positionLerpSpeed = 1;
        public float rotationLerpSpeed = 1;

        bool isLerped = false;

        RigidbodyFirstPersonController fpsObj;

        private void OnValidate()
        {
            // Attempt to get the lerp target if one has not been set
            if (lerpTarget == null)
                lerpTarget = transform.GetChild(0);

            // Create a new lerp target if null
            if (lerpTarget == null)
            {
                GameObject lerpTargetObj = new GameObject("Lerp Target");
                lerpTargetObj.transform.SetParent(transform);
                lerpTarget = lerpTargetObj.transform;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            // Only update if interact button is pressed
            if (!Input.GetButtonDown("Fire1"))
                return;

            if (other.tag == "Player" && !isLerped)
            {
                // Cache current position/rotation
                defaultPosition = transform.position;
                defaultRotation = transform.rotation;

                // Cache & Disable first person controller
                fpsObj = other.GetComponent<RigidbodyFirstPersonController>();
                fpsObj.enabled = false;

                // Lerp camera to target
                StartCoroutine(LerpToTransform(lerpTarget.position, lerpTarget.rotation, defaultPosition, defaultRotation, () =>
                {
                    Player.Instance.GetInteractionSystem.UseMousePosition = true;
                }));

                isLerped = true;
            }
        }

        IEnumerator LerpToTransform(Vector3 targetPos, Quaternion targetRotation, Vector3 startPosition, Quaternion startRotation, Action onCompleteCallback = null)
        {
            // Lerp time and position
            float rT = 0;
            float pT = 0;

            Transform camTrans = Camera.main.transform;

            while (rT < 1f && pT < 1f)
            {
                if (rT < 1f)
                {
                    camTrans.rotation = Quaternion.Slerp(startRotation, targetRotation, rT);
                    rT += Time.deltaTime;
                }

                if (pT < 1f)
                {
                    camTrans.transform.position = Vector3.Slerp(startPosition, targetPos, pT);
                    pT += Time.deltaTime;
                }

                yield return null;
            }

            // Snap rotation and position
            transform.rotation = targetRotation;
            transform.position = targetPos;

            onCompleteCallback.Invoke();
        }

        private void Update()
        {
            if (isLerped && Input.GetKeyDown(KeyCode.B))
            {
                LerpBack();
            }
        }
        public void LerpBack()
        {
            StartCoroutine(LerpToTransform(defaultPosition, defaultRotation, transform.position, transform.rotation, () =>
            {
            // Reactivate the FPS controller
            isLerped = false;
                fpsObj.enabled = true;
                Player.Instance.GetInteractionSystem.UseMousePosition = false;
            }));
        }
    }
}