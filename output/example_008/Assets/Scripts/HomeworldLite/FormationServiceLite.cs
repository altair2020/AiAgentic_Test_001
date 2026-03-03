using System.Collections.Generic;
using UnityEngine;

namespace Example008.HomeworldLite
{
    public enum FormationType
    {
        Line,
        Sphere
    }

    public static class FormationServiceLite
    {
        public static List<Vector3> BuildSlots(int count, Vector3 anchor, Vector3 forward, float spacing, FormationType type)
        {
            var slots = new List<Vector3>(count);
            if (count <= 0)
            {
                return slots;
            }

            Vector3 fwd = forward.sqrMagnitude < 0.01f ? Vector3.forward : forward.normalized;
            Vector3 right = Vector3.Cross(Vector3.up, fwd).normalized;

            if (type == FormationType.Sphere)
            {
                float radius = Mathf.Max(2f, Mathf.Sqrt(count) * spacing * 0.45f);
                for (int i = 0; i < count; i++)
                {
                    float angle = (Mathf.PI * 2f * i) / count;
                    Vector3 offset = (Mathf.Cos(angle) * right + Mathf.Sin(angle) * fwd) * radius;
                    slots.Add(anchor + offset);
                }
                return slots;
            }

            int cols = Mathf.CeilToInt(Mathf.Sqrt(count));
            int rows = Mathf.CeilToInt((float)count / cols);
            Vector3 origin = anchor - right * ((cols - 1) * 0.5f * spacing) - fwd * ((rows - 1) * 0.5f * spacing);

            for (int i = 0; i < count; i++)
            {
                int r = i / cols;
                int c = i % cols;
                slots.Add(origin + right * (c * spacing) + fwd * (r * spacing));
            }

            return slots;
        }
    }
}
