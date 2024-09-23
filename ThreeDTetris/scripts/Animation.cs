using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace SimpleAnimation
{
    public class Animation<T>
    {
        internal T startValue;
        public T value;
        internal T endValue;

        internal float t = 0f;

        public bool done = false;

        public Animation(T startValue, T endValue, T value)
        {
            this.startValue = startValue;
            this.endValue = endValue;
            this.value = value;
        }
    }

    public class AnimationVector3 : Animation<Vector3>
    {
        public AnimationVector3(Vector3 startValue, Vector3 endValue, Vector3 value) : base(startValue, endValue, value)
        {
            this.startValue = startValue;
            this.endValue = endValue;
            this.value = value;
        }

        public Vector3 Lerp(float time)
        {
            t += time;

            if (t >= 1)
            {
                done = true;
                t = 1;
            }

            return value = startValue * (1 - t) + endValue * t;
        }
    }

    public class AnimationFloat : Animation<float>
    {
        public AnimationFloat(float startValue, float endValue, float value) : base(startValue, endValue, value)
        {
            this.startValue = startValue;
            this.endValue = endValue;
            this.value = value;
        }

        public float Lerp(float time)
        {
            t += time;

            if (t >= 1)
            {
                done = true;
                t = 1;
            }

            return value = startValue * (1 - t) + endValue * t;
        }

        public float deltaLerp(float time)
        {
            float lastValue = startValue * (1 - t) + endValue * t;

            t += time;

            if (t >= 1)
            {
                done = true;
                t = 1;
            }

            return value = startValue * (1 - t) + endValue * t - lastValue;
        }

        public float deltaEaseInEaseOut(float time)
        {
            static float ParametricBlend(float t)
            {
                float sqr = t * t;
                return sqr / (2.0f * (sqr - t) + 1.0f);
            }

            float lastValue = startValue * ParametricBlend(1 - t) + endValue * ParametricBlend(t);

            t += time;

            if (t >= 1)
            {
                done = true;
                t = 1;
            }

            return value = startValue * ParametricBlend(1 - t) + endValue * ParametricBlend(t) - lastValue;
        }
        public float EaseInEaseOut(float time)
        {
            static float ParametricBlend(float t)
            {
                float sqr = t * t;
                return sqr / (2.0f * (sqr - t) + 1.0f);
            }

            t += time;

            if (t >= 1)
            {   
                done = true;
                t = 1;
            }

            return value = startValue * ParametricBlend(1 - t) + endValue * ParametricBlend(t);
        }

        public float EaseOutCubic(float time)
        {
            t += time;

            if (t >= 1)
            {
                done = true;
                t = 1;
            }

            return value = startValue * MathF.Pow(1 - t, 3) + endValue * MathF.Pow(t, 3);
        }

        public float easeInEaseOutBump(float time)
        {
            t += time;

            if (t >= 1)
            {
                done = true;
                t = 1;
            }

            float c1 = 0.24f;
            float c2 = c1 * 1.525f;

            if(t < 0.5f)
            {
                return ((endValue - startValue) * (MathF.Pow(2 * t, 2) * ((c2 + 1f) * t - c2)) ) + startValue;
            }
            return ( (endValue - startValue) * ((MathF.Pow(2 * t - 2f, 2) * ((c2 + 1f) * (2 * t - 2f) + c2) + 2) / 2) ) + startValue;
        }
    }
}
