// RotationTools.cs
// Created by Alexander Ameye
// Version 1.2.0

using System.Collections;
using UnityEngine;

namespace AlexDoorSystem
{
    public class RotationTools : MonoBehaviour 
    {
        public static IEnumerator Rotate(GameObject door, float StartAngle, float EndAngle, float Speed, bool UnityConvention, float Offset, bool ShortestWay)
        {
            Quaternion startRotation, endRotation, RotationOffset;

            RotationOffset = Quaternion.Euler(0, Offset, 0);

            AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

            float timeProgression = 0f;

            if (UnityConvention) // Unity Convention
            {
                startRotation = Quaternion.Euler(0, StartAngle, 0);
                endRotation = Quaternion.Euler(0, EndAngle, 0);
            }

            else // Alex's Door System Convention
            {
                startRotation = Quaternion.Euler(0, -StartAngle, 0);
                endRotation = Quaternion.Euler(0, -EndAngle, 0);
            }

            while (timeProgression <= (1 / Speed))
            {
                timeProgression += Time.deltaTime;
                float rotationProgression = Mathf.Clamp01(timeProgression / (1 / Speed));
                float rotationCurveValue = curve.Evaluate(rotationProgression);

                door.transform.rotation = Lerp(startRotation * RotationOffset, endRotation * RotationOffset, rotationCurveValue, ShortestWay);

                yield return null;
            }
        }

        public static Quaternion Lerp(Quaternion p, Quaternion q, float t, bool shortWay)
        {
            if (shortWay)
            {
                float dot = Quaternion.Dot(p, q);
                if (dot < 0.0f)
                    return Lerp(ScalarMultiply(p, -1.0f), q, t, true);
            }

            Quaternion r = Quaternion.identity;
            r.x = p.x * (1f - t) + q.x * (t);
            r.y = p.y * (1f - t) + q.y * (t);
            r.z = p.z * (1f - t) + q.z * (t);
            r.w = p.w * (1f - t) + q.w * (t);
            return r;
        }

        public static Quaternion Slerp(Quaternion p, Quaternion q, float t, bool shortWay)
        {
            float dot = Quaternion.Dot(p, q);
            if (shortWay)
            {
                if (dot < 0.0f)
                    return Slerp(ScalarMultiply(p, -1.0f), q, t, true);
            }

            float angle = Mathf.Acos(dot);
            Quaternion first = ScalarMultiply(p, Mathf.Sin((1f - t) * angle));
            Quaternion second = ScalarMultiply(q, Mathf.Sin((t) * angle));
            float division = 1f / Mathf.Sin(angle);
            return ScalarMultiply(Add(first, second), division);
        }

        public static Quaternion ScalarMultiply(Quaternion input, float scalar)
        {
            return new Quaternion(input.x * scalar, input.y * scalar, input.z * scalar, input.w * scalar);
        }

        public static Quaternion Add(Quaternion p, Quaternion q)
        {
            return new Quaternion(p.x + q.x, p.y + q.y, p.z + q.z, p.w + q.w);
        }
    }
}