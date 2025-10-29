#if UNITY_5_6_OR_NEWER
using System;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace BX.Tweening
{
    public static partial class BXSTween
    {
        // TODO : This somewhat lacks many objects. Hand writing all of this doesn't make much sense, but here is a starter.. :
        // ----
        // Also, the shortcuts **always allocate garbage in some way, not much I can do about it without making the system unergonomic or complicated**
        //                     ^ it's generally because of lambda captures. I could make a lambda proxy where it supplies the object to setter/getter..
        // ----
        // It is also particularly boilerplatey, so it's the longest code in the whole base (when in reality only thing it does is just repeat itself)
        // (SourceGenerator is way to go with this)
        // ----
        // Because of this, I include samples and things often used (alpha, color, 1D property/ies, position, rotation, scale and shake)
        // (basically a straight port from coroutine tween days where i used to have few shorthands)

        [ThreadStatic]
        private static readonly Random rng = new Random();

        // Transform
        /// <summary>
        /// Shorthand to create a position tween.
        /// </summary>
        /// <param name="transform">Transform property, this can't be <see langword="null"/>.</param>
        /// <param name="setLink">
        /// Whether to call <see cref="BXSTweenContext{TValue}.SetLinkObject{T}(T, TickSuspendAction)"/>
        /// with the target property. Default is <see langword="false"/>, setting it <see langword="true"/>
        /// will kill the tween gracefully if the link object dies.
        /// </param>
        /// <param name="play">Whether to start the tween when the shorthand is created.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static BXSTweenVector3Context Position(
            // Props
            Transform transform, Vector3 target,
            // Settings
            float duration = 1.0f, float delay = 0f, int loopCount = 0,
            EaseType ease = EaseType.QuadInOut, Space space = Space.Self,
            float speed = 1f, bool setLink = false, bool play = true
        )
        {
            if (transform == null)
            {
                throw new ArgumentNullException(nameof(transform));
            }

            var ctx = new BXSTweenVector3Context(duration, delay, loopCount, ease, speed);
            switch (space)
            {
                case Space.World:
                    ctx.SetupContext(() => transform.position, target, (v) => transform.position = v);
                    break;

                case Space.Self:
                default:
                    ctx.SetupContext(() => transform.localPosition, target, (v) => transform.localPosition = v);
                    break;
            }

            if (setLink)
            {
                ctx.SetLinkObject(transform);
            }
            if (play)
            {
                ctx.PlayDelayed();
            }

            return ctx;
        }
        /// <summary>
        /// Shorthand to create a x position tween.
        /// </summary>
        /// <param name="transform">Transform property, this can't be <see langword="null"/>.</param>
        /// <param name="setLink">
        /// Whether to call <see cref="BXSTweenContext{TValue}.SetLinkObject{T}(T, TickSuspendAction)"/>
        /// with the target property. Default is <see langword="false"/>, setting it <see langword="true"/>
        /// will kill the tween gracefully if the link object dies.
        /// </param>
        /// <param name="play">Whether to start the tween when the shorthand is created.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static BXSTweenFloatContext PositionX(
            // Props
            Transform transform, float target,
            // Settings
            float duration = 1.0f, float delay = 0f, int loopCount = 0,
            EaseType ease = EaseType.QuadInOut, Space space = Space.Self,
            float speed = 1f, bool setLink = false, bool play = true
        )
        {
            if (transform == null)
            {
                throw new ArgumentNullException(nameof(transform));
            }

            var ctx = new BXSTweenFloatContext(duration, delay, loopCount, ease, speed);
            switch (space)
            {
                case Space.World:
                    ctx.SetupContext(() => transform.position.x, target, (v) =>
                    {
                        Vector3 p = transform.position;
                        p.x = v;
                        transform.position = p;
                    }); break;

                case Space.Self:
                default:
                    ctx.SetupContext(() => transform.localPosition.x, target, (v) =>
                    {
                        Vector3 p = transform.localPosition;
                        p.x = v;
                        transform.localPosition = p;
                    });
                    break;
            }

            if (setLink)
            {
                ctx.SetLinkObject(transform);
            }
            if (play)
            {
                ctx.PlayDelayed();
            }

            return ctx;
        }
        /// <summary>
        /// Shorthand to create a y position tween.
        /// </summary>
        /// <param name="transform">Transform property, this can't be <see langword="null"/>.</param>
        /// <param name="setLink">
        /// Whether to call <see cref="BXSTweenContext{TValue}.SetLinkObject{T}(T, TickSuspendAction)"/>
        /// with the target property. Default is <see langword="false"/>, setting it <see langword="true"/>
        /// will kill the tween gracefully if the link object dies.
        /// </param>
        /// <param name="play">Whether to start the tween when the shorthand is created.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static BXSTweenFloatContext PositionY(
            // Props
            Transform transform, float target,
            // Settings
            float duration = 1.0f, float delay = 0f, int loopCount = 0,
            EaseType ease = EaseType.QuadInOut, Space space = Space.Self,
            float speed = 1f, bool setLink = false, bool play = true
        )
        {
            if (transform == null)
            {
                throw new ArgumentNullException(nameof(transform));
            }

            var ctx = new BXSTweenFloatContext(duration, delay, loopCount, ease, speed);
            switch (space)
            {
                case Space.World:
                    ctx.SetupContext(() => transform.position.x, target, (v) =>
                    {
                        Vector3 p = transform.position;
                        p.y = v;
                        transform.position = p;
                    }); break;

                case Space.Self:
                default:
                    ctx.SetupContext(() => transform.localPosition.x, target, (v) =>
                    {
                        Vector3 p = transform.localPosition;
                        p.y = v;
                        transform.localPosition = p;
                    });
                    break;
            }
            if (setLink)
            {
                ctx.SetLinkObject(transform);
            }
            if (play)
            {
                ctx.PlayDelayed();
            }

            return ctx;
        }
        /// <summary>
        /// Shorthand to create a z position tween.
        /// </summary>
        /// <param name="transform">Transform property, this can't be <see langword="null"/>.</param>
        /// <param name="setLink">
        /// Whether to call <see cref="BXSTweenContext{TValue}.SetLinkObject{T}(T, TickSuspendAction)"/>
        /// with the target property. Default is <see langword="false"/>, setting it <see langword="true"/>
        /// will kill the tween gracefully if the link object dies.
        /// </param>
        /// <param name="play">Whether to start the tween when the shorthand is created.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static BXSTweenFloatContext PositionZ(
            // Props
            Transform transform, float target,
            // Settings
            float duration = 1.0f, float delay = 0f, int loopCount = 0,
            EaseType ease = EaseType.QuadInOut, Space space = Space.Self,
            float speed = 1f, bool setLink = false, bool play = true
        )
        {
            if (transform == null)
            {
                throw new ArgumentNullException(nameof(transform));
            }

            var ctx = new BXSTweenFloatContext(duration, delay, loopCount, ease, speed);
            switch (space)
            {
                case Space.World:
                    ctx.SetupContext(() => transform.position.x, target, (v) =>
                    {
                        Vector3 p = transform.position;
                        p.z = v;
                        transform.position = p;
                    }); break;

                case Space.Self:
                default:
                    ctx.SetupContext(() => transform.localPosition.x, target, (v) =>
                    {
                        Vector3 p = transform.localPosition;
                        p.z = v;
                        transform.localPosition = p;
                    });
                    break;
            }
            if (setLink)
            {
                ctx.SetLinkObject(transform);
            }
            if (play)
            {
                ctx.PlayDelayed();
            }

            return ctx;
        }
        /// <summary>
        /// Shorthand to create a rotation tween.
        /// </summary>
        /// <param name="transform">Transform property, this can't be <see langword="null"/>.</param>
        /// <param name="setLink">
        /// Whether to call <see cref="BXSTweenContext{TValue}.SetLinkObject{T}(T, TickSuspendAction)"/>
        /// with the target property. Default is <see langword="false"/>, setting it <see langword="true"/>
        /// will kill the tween gracefully if the link object dies.
        /// </param>
        /// <param name="play">Whether to start the tween when the shorthand is created.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static BXSTweenQuaternionContext Rotate(
            // Props
            Transform transform, Quaternion target,
            // Settings
            float duration = 1.0f, float delay = 0f, int loopCount = 0,
            EaseType ease = EaseType.QuadInOut, Space space = Space.Self,
            float speed = 1f, bool setLink = false, bool useSlerp = false, bool play = true
        )
        {
            if (transform == null)
            {
                throw new ArgumentNullException(nameof(transform));
            }

            var ctx = new BXSTweenQuaternionContext(duration, delay, loopCount, ease, speed);
            switch (space)
            {
                case Space.World:
                    ctx.SetupContext(() => transform.rotation, target, (v) => transform.rotation = v);
                    break;

                case Space.Self:
                default:
                    ctx.SetupContext(() => transform.localRotation, target, (v) => transform.localRotation = v);
                    break;
            }

            ctx.SetUseSlerp(useSlerp);
            if (setLink)
            {
                ctx.SetLinkObject(transform);
            }
            if (play)
            {
                ctx.PlayDelayed();
            }

            return ctx;
        }
        /// <summary>
        /// Shorthand to create an euler rotation tween.
        /// </summary>
        /// <param name="transform">Transform property, this can't be <see langword="null"/>.</param>
        /// <param name="setLink">
        /// Whether to call <see cref="BXSTweenContext{TValue}.SetLinkObject{T}(T, TickSuspendAction)"/>
        /// with the target property. Default is <see langword="false"/>, setting it <see langword="true"/>
        /// will kill the tween gracefully if the link object dies.
        /// </param>
        /// <param name="play">Whether to start the tween when the shorthand is created.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static BXSTweenVector3Context RotateEuler(
            // Props
            Transform transform, Vector3 target,
            // Settings
            float duration = 1.0f, float delay = 0f, int loopCount = 0,
            EaseType ease = EaseType.QuadInOut, Space space = Space.Self,
            float speed = 1f, bool setLink = false, bool play = true
        )
        {
            if (transform == null)
            {
                throw new ArgumentNullException(nameof(transform));
            }

            var ctx = new BXSTweenVector3Context(duration, delay, loopCount, ease, speed);
            switch (space)
            {
                case Space.World:
                    ctx.SetupContext(() => transform.eulerAngles, target, (v) => transform.eulerAngles = v);
                    break;

                case Space.Self:
                default:
                    ctx.SetupContext(() => transform.eulerAngles, target, (v) => transform.eulerAngles = v);
                    break;
            }

            if (setLink)
            {
                ctx.SetLinkObject(transform);
            }
            if (play)
            {
                ctx.PlayDelayed();
            }

            return ctx;
        }

        /// <summary>
        /// Shorthand to create a scale tween.
        /// </summary>
        /// <param name="transform">Transform property, this can't be <see langword="null"/>.</param>
        /// <param name="setLink">
        /// Whether to call <see cref="BXSTweenContext{TValue}.SetLinkObject{T}(T, TickSuspendAction)"/>
        /// with the target property. Default is <see langword="false"/>, setting it <see langword="true"/>
        /// will kill the tween gracefully if the link object dies.
        /// </param>
        /// <param name="play">Whether to start the tween when the shorthand is created.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static BXSTweenVector3Context Scale(
            // Props
            Transform transform, Vector3 target,
            // Settings
            float duration = 1.0f, float delay = 0f, int loopCount = 0,
            EaseType ease = EaseType.QuadInOut, Space space = Space.Self,
            float speed = 1f, bool setLink = false, bool play = true
        )
        {
            if (transform == null)
            {
                throw new ArgumentNullException(nameof(transform));
            }

            var ctx = new BXSTweenVector3Context(duration, delay, loopCount, ease, speed);
            switch (space)
            {
                case Space.World:
                    ctx.SetupContext(() => transform.lossyScale, target, (v) =>
                    {
                        Vector3 globalScale = transform.lossyScale;
                        // ratio scale to set by our current absolute scale, computed by the parent transforms.
                        v.x /= globalScale.x;
                        v.y /= globalScale.y;
                        v.z /= globalScale.z;

                        transform.localScale = v;
                    });
                    break;

                case Space.Self:
                default:
                    ctx.SetupContext(() => transform.localScale, target, (v) => transform.localScale = v);
                    break;
            }

            if (setLink)
            {
                ctx.SetLinkObject(transform);
            }
            if (play)
            {
                ctx.PlayDelayed();
            }

            return ctx;
        }
        /// <summary>
        /// Shorthand to create a x scale tween.
        /// </summary>
        /// <param name="transform">Transform property, this can't be <see langword="null"/>.</param>
        /// <param name="setLink">
        /// Whether to call <see cref="BXSTweenContext{TValue}.SetLinkObject{T}(T, TickSuspendAction)"/>
        /// with the target property. Default is <see langword="false"/>, setting it <see langword="true"/>
        /// will kill the tween gracefully if the link object dies.
        /// </param>
        /// <param name="play">Whether to start the tween when the shorthand is created.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static BXSTweenFloatContext ScaleX(
            // Props
            Transform transform, float target,
            // Settings
            float duration = 1.0f, float delay = 0f, int loopCount = 0,
            EaseType ease = EaseType.QuadInOut, Space space = Space.Self,
            float speed = 1f, bool setLink = false, bool play = true
        )
        {
            if (transform == null)
            {
                throw new ArgumentNullException(nameof(transform));
            }

            var ctx = new BXSTweenFloatContext(duration, delay, loopCount, ease, speed);
            switch (space)
            {
                case Space.World:
                    ctx.SetupContext(() => transform.lossyScale.x, target, (v) =>
                    {
                        Vector3 localScale = transform.localScale;
                        // ratio scale to set by our current absolute scale, computed by the parent transforms.
                        localScale.x = v / transform.lossyScale.x;

                        transform.localScale = localScale;
                    });
                    break;

                case Space.Self:
                default:
                    ctx.SetupContext(() => transform.localScale.x, target, (v) =>
                    {
                        Vector3 localScale = transform.localScale;
                        localScale.x = v;

                        transform.localScale = localScale;
                    });
                    break;
            }

            if (setLink)
            {
                ctx.SetLinkObject(transform);
            }
            if (play)
            {
                ctx.PlayDelayed();
            }

            return ctx;
        }
        /// <summary>
        /// Shorthand to create a y scale tween.
        /// </summary>
        /// <param name="transform">Transform property, this can't be <see langword="null"/>.</param>
        /// <param name="setLink">
        /// Whether to call <see cref="BXSTweenContext{TValue}.SetLinkObject{T}(T, TickSuspendAction)"/>
        /// with the target property. Default is <see langword="false"/>, setting it <see langword="true"/>
        /// will kill the tween gracefully if the link object dies.
        /// </param>
        /// <param name="play">Whether to start the tween when the shorthand is created.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static BXSTweenFloatContext ScaleY(
            // Props
            Transform transform, float target,
            // Settings
            float duration = 1.0f, float delay = 0f, int loopCount = 0,
            EaseType ease = EaseType.QuadInOut, Space space = Space.Self,
            float speed = 1f, bool setLink = false, bool play = true
        )
        {
            if (transform == null)
            {
                throw new ArgumentNullException(nameof(transform));
            }

            var ctx = new BXSTweenFloatContext(duration, delay, loopCount, ease, speed);
            switch (space)
            {
                case Space.World:
                    ctx.SetupContext(() => transform.lossyScale.y, target, (v) =>
                    {
                        Vector3 localScale = transform.localScale;
                        // ratio scale to set by our current absolute scale, computed by the parent transforms.
                        localScale.y = v / transform.lossyScale.y;

                        transform.localScale = localScale;
                    });
                    break;

                case Space.Self:
                default:
                    ctx.SetupContext(() => transform.localScale.y, target, (v) =>
                    {
                        Vector3 localScale = transform.localScale;
                        localScale.y = v;

                        transform.localScale = localScale;
                    });
                    break;
            }

            if (setLink)
            {
                ctx.SetLinkObject(transform);
            }
            if (play)
            {
                ctx.PlayDelayed();
            }

            return ctx;
        }
        /// <summary>
        /// Shorthand to create a z position tween.
        /// </summary>
        /// <param name="transform">Transform property, this can't be <see langword="null"/>.</param>
        /// <param name="setLink">
        /// Whether to call <see cref="BXSTweenContext{TValue}.SetLinkObject{T}(T, TickSuspendAction)"/>
        /// with the target property. Default is <see langword="false"/>, setting it <see langword="true"/>
        /// will kill the tween gracefully if the link object dies.
        /// </param>
        /// <param name="play">Whether to start the tween when the shorthand is created.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static BXSTweenFloatContext ScaleZ(
            // Props
            Transform transform, float target,
            // Settings
            float duration = 1.0f, float delay = 0f, int loopCount = 0,
            EaseType ease = EaseType.QuadInOut, Space space = Space.Self,
            float speed = 1f, bool setLink = false, bool play = true
        )
        {
            if (transform == null)
            {
                throw new ArgumentNullException(nameof(transform));
            }

            var ctx = new BXSTweenFloatContext(duration, delay, loopCount, ease, speed);
            switch (space)
            {
                case Space.World:
                    ctx.SetupContext(() => transform.lossyScale.z, target, (v) =>
                    {
                        Vector3 localScale = transform.localScale;
                        // ratio scale to set by our current absolute scale, computed by the parent transforms.
                        localScale.z = v / transform.lossyScale.z;

                        transform.localScale = localScale;
                    });
                    break;

                case Space.Self:
                default:
                    ctx.SetupContext(() => transform.localScale.z, target, (v) =>
                    {
                        Vector3 localScale = transform.localScale;
                        localScale.z = v;

                        transform.localScale = localScale;
                    });
                    break;
            }
            if (setLink)
            {
                ctx.SetLinkObject(transform);
            }
            if (play)
            {
                ctx.PlayDelayed();
            }

            return ctx;
        }
        /// <summary>
        /// Shorthand to create a shake tween.
        /// <br>This creates a progress only tween, where the start value is 0f and end is 1f and the shake generates a list of points for setter.</br>
        /// <br>
        /// Because of this, seemingly change runs this tween (unless in non-deterministic mode which in that case, the previous assumption is true)
        /// But since the intervals are tiny, change does technically move this tween. The points which there are <paramref name="iters"/> count
        /// wrap. With the <paramref name="iterDensity"/>, how fast does it wrap can be controlled.
        /// </br>
        /// <br>The shake immediately switches position as opposed to smoothly moving from point A->B and the other intermediate points</br>
        /// <br>It is recommended to reuse this tween if <paramref name="iters"/> &gt; 0, as it allocates more memory than others.</br>
        /// </summary>
        /// <param name="magnitude">
        /// Magnitude of shake per given axis. For 2D, you can pass <see cref="Vector2"/>.one.normalized and vice versa.
        /// Negative axis shakes do nothing expect for inverting the shake, if deterministic.
        /// <br>(note that the shake is generated into the setter)</br>
        /// </param>
        /// <param name="transform">Transform property to shake, this can't be <see langword="null"/>.</param>
        /// <param name="iters">
        /// Amount of randomized values iteration to send into the predicate. 
        /// If this is zero or less than zero, the tween has a non-deterministic shake.
        /// </param>
        /// <param name="iterDensity">
        /// How much the tween should elapse before completing (and repeating) the shake list.
        /// <br>Only relevant if <paramref name="iters"/> is &gt; 0</br>
        /// <br>Value is limited between 0.001f and 1f.</br>
        /// </param>
        /// <param name="returnToInitial">
        /// Returns to initial position if <see langword="true"/>.
        /// </param>
        /// <param name="setLink">
        /// Whether to call <see cref="BXSTweenContext{TValue}.SetLinkObject{T}(T, TickSuspendAction)"/>
        /// with the target property. Default is <see langword="false"/>, setting it <see langword="true"/>
        /// will kill the tween gracefully if the link object dies.
        /// </param>
        /// <param name="play">Whether to start the tween when the shorthand is created.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static BXSTweenFloatContext Shake(
            // Props
            Transform transform, Vector3 magnitude, int iters = 32, float iterDensity = 0.33f, bool returnToInitial = true,
            // Settings
            float duration = 1.0f, float delay = 0f, int loopCount = 0,
            EaseType ease = EaseType.Linear, Space space = Space.Self,
            float speed = 1f, bool setLink = false, bool play = true
        )
        {
            // TODO : Ability to get setter for shake. Though with predefined types.
            if (transform == null)
            {
                throw new ArgumentNullException(nameof(transform));
            }

            iterDensity = Math.Clamp(iterDensity, 0.001f, 1f);

            // I could add "maxJumpDistance", if the magnitude is a little bit too large.
            var ctx = new BXSTweenFloatContext(duration, delay, loopCount, ease, speed);
            if (iters > 0)
            {
                float[] shakes = new float[iters * 3];

                for (int i = 0; i < shakes.Length; i++)
                {
                    shakes[i] = (((float)rng.NextDouble()) - 0.5f) * 2f;
                }

                switch (space)
                {
                    case Space.World:
                        {
                            Vector3 startPos = transform.position;
                            ctx.SetupContext(0f, 1f, (v) =>
                            {
                                if (returnToInitial && v >= 1f)
                                {
                                    transform.position = startPos;
                                    return;
                                }

                                Vector3 pos = transform.position;
                                int i1 = ((int)((v * iters * 3) / iterDensity)) % shakes.Length;
                                int i2 = (i1 + 1) % shakes.Length;
                                int i3 = (i2 + 1) % shakes.Length;
                                Vector3 shakeMag = new Vector3(shakes[i1], shakes[i2], shakes[i3]);

                                pos.x = startPos.x + (magnitude.x * shakeMag.x);
                                pos.y = startPos.y + (magnitude.y * shakeMag.y);
                                pos.z = startPos.z + (magnitude.z * shakeMag.z);

                                transform.position = pos;
                            });
                            break;
                        }
                    default:
                    case Space.Self:
                        {
                            Vector3 startPos = transform.localPosition;
                            ctx.SetupContext(0f, 1f, (v) =>
                            {
                                if (returnToInitial && v >= 1f)
                                {
                                    transform.localPosition = startPos;
                                    return;
                                }

                                Vector3 pos = transform.localPosition;
                                int i1 = ((int)((v * iters * 3) / iterDensity)) % shakes.Length;
                                int i2 = (i1 + 1) % shakes.Length;
                                int i3 = (i2 + 1) % shakes.Length;
                                Vector3 shakeMag = new Vector3(shakes[i1], shakes[i2], shakes[i3]);

                                pos.x = startPos.x + (magnitude.x * shakeMag.x);
                                pos.y = startPos.y + (magnitude.y * shakeMag.y);
                                pos.z = startPos.z + (magnitude.z * shakeMag.z);

                                transform.localPosition = pos;
                            });
                            break;
                        }
                }
            }
            else
            {
                switch (space)
                {
                    case Space.World:
                        {
                            Vector3 startPos = transform.position;
                            ctx.SetupContext(0f, 1f, (v) =>
                            {
                                if (returnToInitial && v >= 1f)
                                {
                                    transform.position = startPos;
                                    return;
                                }

                                Vector3 pos = transform.position;
                                pos.x = startPos.x + (magnitude.x * (((float)rng.NextDouble()) - 0.5f) * 2f);
                                pos.y = startPos.y + (magnitude.y * (((float)rng.NextDouble()) - 0.5f) * 2f);
                                pos.z = startPos.z + (magnitude.z * (((float)rng.NextDouble()) - 0.5f) * 2f);

                                transform.position = pos;
                            });
                            break;
                        }
                    default:
                    case Space.Self:
                        {
                            Vector3 startPos = transform.localPosition;
                            ctx.SetupContext(0f, 1f, (v) =>
                            {
                                if (returnToInitial && v >= 1f)
                                {
                                    transform.localPosition = startPos;
                                    return;
                                }

                                Vector3 pos = transform.localPosition;
                                pos.x = startPos.x + (magnitude.x * (((float)rng.NextDouble()) - 0.5f) * 2f);
                                pos.y = startPos.y + (magnitude.y * (((float)rng.NextDouble()) - 0.5f) * 2f);
                                pos.z = startPos.z + (magnitude.z * (((float)rng.NextDouble()) - 0.5f) * 2f);

                                transform.localPosition = pos;
                            });
                            break;
                        }
                }
            }

            if (setLink)
            {
                ctx.SetLinkObject(transform);
            }
            if (play)
            {
                ctx.PlayDelayed();
            }

            return ctx;
        }

        // AnchoredTransform
        /// <summary>
        /// Shorthand to create an anchored position tween.
        /// </summary>
        /// <param name="transform">Transform property, this can't be <see langword="null"/>.</param>
        /// <param name="setLink">
        /// Whether to call <see cref="BXSTweenContext{TValue}.SetLinkObject{T}(T, TickSuspendAction)"/>
        /// with the target property. Default is <see langword="false"/>, setting it <see langword="true"/>
        /// will kill the tween gracefully if the link object dies.
        /// </param>
        /// <param name="play">Whether to start the tween when the shorthand is created.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static BXSTweenVector2Context AnchoredPosition(
            // Props
            RectTransform transform, Vector2 target,
            // Settings
            float duration = 1.0f, float delay = 0f, int loopCount = 0,
            EaseType ease = EaseType.QuadInOut,
            float speed = 1f, bool setLink = false, bool play = true
        )
        {
            if (transform == null)
            {
                throw new ArgumentNullException(nameof(transform));
            }

            var ctx = new BXSTweenVector2Context(duration, delay, loopCount, ease, speed);
            ctx.SetupContext(() => transform.anchoredPosition, target, (v) => transform.anchoredPosition = v);

            if (setLink)
            {
                ctx.SetLinkObject(transform);
            }
            if (play)
            {
                ctx.PlayDelayed();
            }

            return ctx;
        }
        /// <summary>
        /// Shorthand to create an anchored position tween.
        /// </summary>
        /// <param name="transform">Transform property, this can't be <see langword="null"/>.</param>
        /// <param name="setLink">
        /// Whether to call <see cref="BXSTweenContext{TValue}.SetLinkObject{T}(T, TickSuspendAction)"/>
        /// with the target property. Default is <see langword="false"/>, setting it <see langword="true"/>
        /// will kill the tween gracefully if the link object dies.
        /// </param>
        /// <param name="play">Whether to start the tween when the shorthand is created.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static BXSTweenFloatContext AnchoredPositionX(
            // Props
            RectTransform transform, float target,
            // Settings
            float duration = 1.0f, float delay = 0f, int loopCount = 0,
            EaseType ease = EaseType.QuadInOut,
            float speed = 1f, bool setLink = false, bool play = true
        )
        {
            if (transform == null)
            {
                throw new ArgumentNullException(nameof(transform));
            }

            var ctx = new BXSTweenFloatContext(duration, delay, loopCount, ease, speed);
            ctx.SetupContext(() => transform.anchoredPosition.x, target, (v) =>
            {
                Vector2 p = transform.anchoredPosition;
                p.x = v;
                transform.anchoredPosition = p;
            });

            if (setLink)
            {
                ctx.SetLinkObject(transform);
            }
            if (play)
            {
                ctx.PlayDelayed();
            }

            return ctx;
        }
        /// <summary>
        /// Shorthand to create an anchored position tween.
        /// </summary>
        /// <param name="transform">Transform property, this can't be <see langword="null"/>.</param>
        /// <param name="setLink">
        /// Whether to call <see cref="BXSTweenContext{TValue}.SetLinkObject{T}(T, TickSuspendAction)"/>
        /// with the target property. Default is <see langword="false"/>, setting it <see langword="true"/>
        /// will kill the tween gracefully if the link object dies.
        /// </param>
        /// <param name="play">Whether to start the tween when the shorthand is created.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static BXSTweenFloatContext AnchoredPositionY(
            // Props
            RectTransform transform, float target,
            // Settings
            float duration = 1.0f, float delay = 0f, int loopCount = 0,
            EaseType ease = EaseType.QuadInOut,
            float speed = 1f, bool setLink = false, bool play = true
        )
        {
            if (transform == null)
            {
                throw new ArgumentNullException(nameof(transform));
            }

            var ctx = new BXSTweenFloatContext(duration, delay, loopCount, ease, speed);
            ctx.SetupContext(() => transform.anchoredPosition.y, target, (v) =>
            {
                Vector2 p = transform.anchoredPosition;
                p.y = v;
                transform.anchoredPosition = p;
            });

            if (setLink)
            {
                ctx.SetLinkObject(transform);
            }
            if (play)
            {
                ctx.PlayDelayed();
            }

            return ctx;
        }

        // UGUI
        /// <summary>
        /// Shorthand to create a color tween.
        /// </summary>
        /// <param name="graphic">The target. This can't be <see langword="null"/>.</param>
        /// <param name="setLink">
        /// Whether to call <see cref="BXSTweenContext{TValue}.SetLinkObject{T}(T, TickSuspendAction)"/>
        /// with the target property. Default is <see langword="false"/>, setting it <see langword="true"/>
        /// will kill the tween gracefully if the link object dies.
        /// </param>
        /// <param name="play">Whether to start the tween when the shorthand is created.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static BXSTweenColorContext Color(
            // Props
            Graphic graphic, Color target,
            // Settings
            float duration = 1.0f, float delay = 0f, int loopCount = 0,
            EaseType ease = EaseType.QuadInOut,
            float speed = 1f, bool setLink = false, bool play = true
        )
        {
            if (graphic == null)
            {
                throw new ArgumentNullException(nameof(graphic));
            }

            var ctx = new BXSTweenColorContext(duration, delay, loopCount, ease, speed);
            ctx.SetupContext(() => graphic.color, target, (v) => graphic.color = v);

            if (setLink)
            {
                ctx.SetLinkObject(graphic);
            }
            if (play)
            {
                ctx.PlayDelayed();
            }

            return ctx;
        }
        /// <summary>
        /// Shorthand to create an alpha tween.
        /// </summary>
        /// <param name="graphic">The target. This can't be <see langword="null"/>.</param>
        /// <param name="setLink">
        /// Whether to call <see cref="BXSTweenContext{TValue}.SetLinkObject{T}(T, TickSuspendAction)"/>
        /// with the target property. Default is <see langword="false"/>, setting it <see langword="true"/>
        /// will kill the tween gracefully if the link object dies.
        /// </param>
        /// <param name="play">Whether to start the tween when the shorthand is created.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static BXSTweenFloatContext Alpha(
            // Props
            Graphic graphic, float target,
            // Settings
            float duration = 1.0f, float delay = 0f, int loopCount = 0,
            EaseType ease = EaseType.QuadInOut,
            float speed = 1f, bool setLink = false, bool play = true
        )
        {
            if (graphic == null)
            {
                throw new ArgumentNullException(nameof(graphic));
            }

            var ctx = new BXSTweenFloatContext(duration, delay, loopCount, ease, speed);
            ctx.SetupContext(() => graphic.color.a, target, (v) =>
            {
                Color c = graphic.color;
                c.a = v;
                graphic.color = c;
            });

            if (setLink)
            {
                ctx.SetLinkObject(graphic);
            }
            if (play)
            {
                ctx.PlayDelayed();
            }

            return ctx;
        }
        /// <summary>
        /// Shorthand to create an alpha tween.
        /// </summary>
        /// <param name="group">The target. This can't be <see langword="null"/>.</param>
        /// <param name="setLink">
        /// Whether to call <see cref="BXSTweenContext{TValue}.SetLinkObject{T}(T, TickSuspendAction)"/>
        /// with the target property. Default is <see langword="false"/>, setting it <see langword="true"/>
        /// will kill the tween gracefully if the link object dies.
        /// </param>
        /// <param name="play">Whether to start the tween when the shorthand is created.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static BXSTweenFloatContext Alpha(
            // Props
            CanvasGroup group, float target,
            // Settings
            float duration = 1.0f, float delay = 0f, int loopCount = 0,
            EaseType ease = EaseType.QuadInOut,
            float speed = 1f, bool setLink = false, bool play = true
        )
        {
            if (group == null)
            {
                throw new ArgumentNullException(nameof(group));
            }

            var ctx = new BXSTweenFloatContext(duration, delay, loopCount, ease, speed);
            ctx.SetupContext(() => group.alpha, target, (v) => group.alpha = v);

            if (setLink)
            {
                ctx.SetLinkObject(group);
            }
            if (play)
            {
                ctx.PlayDelayed();
            }

            return ctx;
        }

        // Renderer/Material
        /// <summary>
        /// Shorthand to create a lerp tween for materials.
        /// </summary>
        /// <param name="material">Material property, this can't be <see langword="null"/>.</param>
        /// <param name="setLink">
        /// Whether to call <see cref="BXSTweenContext{TValue}.SetLinkObject{T}(T, TickSuspendAction)"/>
        /// with the target property. Default is <see langword="false"/>, setting it <see langword="true"/>
        /// will kill the tween gracefully if the link object dies.
        /// </param>
        /// <param name="play">Whether to start the tween when the shorthand is created.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static BXSTweenFloatContext Lerp(
            // Props
            Material material, Material target,
            // Settings
            float duration = 1.0f, float delay = 0f, int loopCount = 0,
            EaseType ease = EaseType.QuadInOut,
            float speed = 1f, bool setLink = false, bool play = true
        )
        {
            if (material == null)
            {
                throw new ArgumentNullException(nameof(material));
            }

            var ctx = new BXSTweenFloatContext(duration, delay, loopCount, ease, speed);
            ctx.SetupContext(0f, 1f, (v) => material.Lerp(material, target, v));

            if (setLink)
            {
                ctx.SetLinkObject(material);
            }
            if (play)
            {
                ctx.PlayDelayed();
            }

            return ctx;
        }
        /// <summary>
        /// Shorthand to create a lerp tween for float property on material.
        /// </summary>
        /// <param name="material">Material property, this can't be <see langword="null"/>.</param>
        /// <param name="setLink">
        /// Whether to call <see cref="BXSTweenContext{TValue}.SetLinkObject{T}(T, TickSuspendAction)"/>
        /// with the target property. Default is <see langword="false"/>, setting it <see langword="true"/>
        /// will kill the tween gracefully if the link object dies.
        /// </param>
        /// <param name="play">Whether to start the tween when the shorthand is created.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static BXSTweenFloatContext Property(
            // Props
            Material material, float target, string propertyName,
            // Settings
            float duration = 1.0f, float delay = 0f, int loopCount = 0,
            EaseType ease = EaseType.QuadInOut,
            float speed = 1f, bool setLink = false, bool play = true
        )
        {
            if (material == null)
            {
                throw new ArgumentNullException(nameof(material));
            }
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            var ctx = new BXSTweenFloatContext(duration, delay, loopCount, ease, speed);

            int hash = Shader.PropertyToID(propertyName);
            ctx.SetupContext(() => material.GetFloat(hash), target, (v) => material.SetFloat(hash, v));

            if (setLink)
            {
                ctx.SetLinkObject(material);
            }
            if (play)
            {
                ctx.PlayDelayed();
            }

            return ctx;
        }
        /// <summary>
        /// Shorthand to create a color tween.
        /// </summary>
        /// <param name="material">Material property, this can't be <see langword="null"/>.</param>
        /// <param name="setLink">
        /// Whether to call <see cref="BXSTweenContext{TValue}.SetLinkObject{T}(T, TickSuspendAction)"/>
        /// with the target property. Default is <see langword="false"/>, setting it <see langword="true"/>
        /// will kill the tween gracefully if the link object dies.
        /// </param>
        /// <param name="play">Whether to start the tween when the shorthand is created.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static BXSTweenColorContext Color(
            // Props
            Material material, Color target,
            // Settings
            float duration = 1.0f, float delay = 0f, int loopCount = 0,
            EaseType ease = EaseType.QuadInOut, string propertyName = null,
            float speed = 1f, bool setLink = false, bool play = true
        )
        {
            if (material == null)
            {
                throw new ArgumentNullException(nameof(material));
            }

            var ctx = new BXSTweenColorContext(duration, delay, loopCount, ease, speed);
            if (string.IsNullOrEmpty(propertyName))
            {
                ctx.SetupContext(() => material.color, target, (v) => material.color = v);
            }
            else
            {
                int hash = Shader.PropertyToID(propertyName);
                ctx.SetupContext(() => material.GetColor(hash), target, (v) => material.SetColor(hash, v));
            }

            if (setLink)
            {
                ctx.SetLinkObject(material);
            }
            if (play)
            {
                ctx.PlayDelayed();
            }

            return ctx;
        }
        /// <summary>
        /// Shorthand to create a color tween.
        /// </summary>
        /// <param name="renderer">Renderer property, this can't be <see langword="null"/>.</param>
        /// <param name="setLink">
        /// Whether to call <see cref="BXSTweenContext{TValue}.SetLinkObject{T}(T, TickSuspendAction)"/>
        /// with the target property. Default is <see langword="false"/>, setting it <see langword="true"/>
        /// will kill the tween gracefully if the link object dies.
        /// </param>
        /// <param name="play">Whether to start the tween when the shorthand is created.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static BXSTweenColorContext Color(
            // Props
            Renderer renderer, Color target,
            // Settings
            float duration = 1.0f, float delay = 0f, int loopCount = 0,
            EaseType ease = EaseType.QuadInOut, string propertyName = null,
            float speed = 1f, bool setLink = false, bool useShared = false, bool play = true
        )
        {
            if (renderer == null)
            {
                throw new ArgumentNullException(nameof(renderer));
            }

            return Color(
                useShared ? renderer.sharedMaterial : renderer.material,
                target, duration: duration, delay: delay, loopCount: loopCount,
                ease: ease, propertyName: propertyName,
                speed: speed, setLink: setLink, play: play
            );
        }
        /// <summary>
        /// Shorthand to create an alpha tween (targeting a color property).
        /// </summary>
        /// <param name="material">Material property, this can't be <see langword="null"/>.</param>
        /// <param name="setLink">
        /// Whether to call <see cref="BXSTweenContext{TValue}.SetLinkObject{T}(T, TickSuspendAction)"/>
        /// with the target property. Default is <see langword="false"/>, setting it <see langword="true"/>
        /// will kill the tween gracefully if the link object dies.
        /// </param>
        /// <param name="play">Whether to start the tween when the shorthand is created.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static BXSTweenFloatContext Alpha(
            // Props
            Material material, float target,
            // Settings
            float duration = 1.0f, float delay = 0f, int loopCount = 0,
            EaseType ease = EaseType.QuadInOut, string propertyName = null,
            float speed = 1f, bool setLink = false, bool play = true
        )
        {
            if (material == null)
            {
                throw new ArgumentNullException(nameof(material));
            }

            var ctx = new BXSTweenFloatContext(duration, delay, loopCount, ease, speed);
            if (string.IsNullOrEmpty(propertyName))
            {
                ctx.SetupContext(() => material.color.a, target, (v) =>
                {
                    Color c = material.color;
                    c.a = v;
                    material.color = c;
                });
            }
            else
            {
                int hash = Shader.PropertyToID(propertyName);
                ctx.SetupContext(() => material.GetColor(hash).a, target, (v) =>
                {
                    Color c = material.GetColor(hash);
                    c.a = v;
                    material.SetColor(hash, c);
                });
            }

            if (setLink)
            {
                ctx.SetLinkObject(material);
            }
            if (play)
            {
                ctx.PlayDelayed();
            }

            return ctx;
        }
        /// <summary>
        /// Shorthand to create a color tween (targeting a color property).
        /// </summary>
        /// <param name="renderer">Renderer property, this can't be <see langword="null"/>.</param>
        /// <param name="setLink">
        /// Whether to call <see cref="BXSTweenContext{TValue}.SetLinkObject{T}(T, TickSuspendAction)"/>
        /// with the target property. Default is <see langword="false"/>, setting it <see langword="true"/>
        /// will kill the tween gracefully if the link object dies.
        /// </param>
        /// <param name="play">Whether to start the tween when the shorthand is created.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static BXSTweenFloatContext Alpha(
            // Props
            Renderer renderer, float target,
            // Settings
            float duration = 1.0f, float delay = 0f, int loopCount = 0,
            EaseType ease = EaseType.QuadInOut, string propertyName = null,
            float speed = 1f, bool setLink = false, bool useShared = false, bool play = true
        )
        {
            if (renderer == null)
            {
                throw new ArgumentNullException(nameof(renderer));
            }

            return Alpha(
                useShared ? renderer.sharedMaterial : renderer.material, target,
                duration: duration, delay: delay, loopCount: loopCount,
                ease: ease, propertyName: propertyName,
                speed: speed, setLink: setLink, play: play
            );
        }

        // Light
        /// <summary>
        /// Shorthand to create a color tween.
        /// </summary>
        /// <param name="light">The target. This can't be <see langword="null"/>.</param>
        /// <param name="setLink">
        /// Whether to call <see cref="BXSTweenContext{TValue}.SetLinkObject{T}(T, TickSuspendAction)"/>
        /// with the target property. Default is <see langword="false"/>, setting it <see langword="true"/>
        /// will kill the tween gracefully if the link object dies.
        /// </param>
        /// <param name="play">Whether to start the tween when the shorthand is created.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static BXSTweenColorContext Color(
            // Props
            Light light, Color target,
            // Settings
            float duration = 1.0f, float delay = 0f, int loopCount = 0,
            EaseType ease = EaseType.QuadInOut,
            float speed = 1f, bool setLink = false, bool play = true
        )
        {
            if (light == null)
            {
                throw new ArgumentNullException(nameof(light));
            }

            var ctx = new BXSTweenColorContext(duration, delay, loopCount, ease, speed);
            ctx.SetupContext(() => light.color, target, (v) => light.color = v);

            if (setLink)
            {
                ctx.SetLinkObject(light);
            }
            if (play)
            {
                ctx.PlayDelayed();
            }

            return ctx;
        }
        /// <summary>
        /// Shorthand to create a color temprature tween.
        /// </summary>
        /// <param name="light">The target. This can't be <see langword="null"/>.</param>
        /// <param name="setLink">
        /// Whether to call <see cref="BXSTweenContext{TValue}.SetLinkObject{T}(T, TickSuspendAction)"/>
        /// with the target property. Default is <see langword="false"/>, setting it <see langword="true"/>
        /// will kill the tween gracefully if the link object dies.
        /// </param>
        /// <param name="play">Whether to start the tween when the shorthand is created.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static BXSTweenFloatContext ColorTemprature(
            // Props
            Light light, float target,
            // Settings
            float duration = 1.0f, float delay = 0f, int loopCount = 0,
            EaseType ease = EaseType.QuadInOut,
            float speed = 1f, bool setLink = false, bool play = true
        )
        {
            if (light == null)
            {
                throw new ArgumentNullException(nameof(light));
            }

            var ctx = new BXSTweenFloatContext(duration, delay, loopCount, ease, speed);
            ctx.SetupContext(() => light.colorTemperature, target, (v) => light.colorTemperature = v);

            if (setLink)
            {
                ctx.SetLinkObject(light);
            }
            if (play)
            {
                ctx.PlayDelayed();
            }

            return ctx;
        }
        /// <summary>
        /// Shorthand to create an intensity tween.
        /// </summary>
        /// <param name="light">The target. This can't be <see langword="null"/>.</param>
        /// <param name="setLink">
        /// Whether to call <see cref="BXSTweenContext{TValue}.SetLinkObject{T}(T, TickSuspendAction)"/>
        /// with the target property. Default is <see langword="false"/>, setting it <see langword="true"/>
        /// will kill the tween gracefully if the link object dies.
        /// </param>
        /// <param name="play">Whether to start the tween when the shorthand is created.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static BXSTweenFloatContext Fade(
            // Props
            Light light, float target,
            // Settings
            float duration = 1.0f, float delay = 0f, int loopCount = 0,
            EaseType ease = EaseType.QuadInOut,
            float speed = 1f, bool setLink = false, bool play = true
        )
        {
            if (light == null)
            {
                throw new ArgumentNullException(nameof(light));
            }

            var ctx = new BXSTweenFloatContext(duration, delay, loopCount, ease, speed);
            ctx.SetupContext(() => light.intensity, target, (v) => light.intensity = v);

            if (setLink)
            {
                ctx.SetLinkObject(light);
            }
            if (play)
            {
                ctx.PlayDelayed();
            }

            return ctx;
        }

        // AudioSource
        /// <summary>
        /// Shorthand to create a volume tween.
        /// </summary>
        /// <param name="source">The target. This can't be <see langword="null"/>.</param>
        /// <param name="setLink">
        /// Whether to call <see cref="BXSTweenContext{TValue}.SetLinkObject{T}(T, TickSuspendAction)"/>
        /// with the target property. Default is <see langword="false"/>, setting it <see langword="true"/>
        /// will kill the tween gracefully if the link object dies.
        /// </param>
        /// <param name="play">Whether to start the tween when the shorthand is created.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static BXSTweenFloatContext Volume(
            // Props
            AudioSource source, float target,
            // Settings
            float duration = 1.0f, float delay = 0f, int loopCount = 0,
            EaseType ease = EaseType.QuadInOut,
            float speed = 1f, bool setLink = false, bool play = true
        )
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var ctx = new BXSTweenFloatContext(duration, delay, loopCount, ease, speed);
            ctx.SetupContext(() => source.volume, target, (v) => source.volume = v);

            if (setLink)
            {
                ctx.SetLinkObject(source);
            }
            if (play)
            {
                ctx.PlayDelayed();
            }

            return ctx;
        }
    }
}
#endif
