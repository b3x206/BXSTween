# BXSTween

BXSTween is a **slightly<sup>\^1</sup>** simpler tweening engine, mainly built for Unity Engine but 
_can be adapted to other C# projects that doesn't depend on Unity Engine.<sup>\^2,3</sup>_

It is compatible with Unity Engine &gt;=2021.3 or .NET Standard 2.1 <sup>(or with .NET Standard 2.0 with package provided `System.Numerics`)</sup>

<sup>\^</sup>1 : Most expected functionality exists, but some are implemented _somewhat naively_.
The demos show how I use my own system, which is usually to schedule and do simpler animations.

<sup>\^</sup>2 : Only the [BXSTween/Core directory](./Core) is eligible.
[Editor tooling](./Editor) can be ported but can take significantly more time, if the framework is unsuitable (for example, no IMGUI or generic serialization interface).
Also, a timed loop (delta time/frame) is required and explicit nullability is disabled (for this project) for running the tweener. <br/>

<sup>\^</sup>3 : `System.Numerics`, `System.Collections`, `System.Collections.Generic` and some `System` namespaced libraries are the dependencies. Only one that may not exist in your context could be `System.Numerics`, but it is unlikely.

---

BXSTween powers the UI animations and some other animations in both of my "games", [Flag Race](https://b3x.itch.io/flag-race) and [Fall Xtra](https://b3x.itch.io/fall-xtra). <br/>
It was also used in game jams that I contributed in (though usually only by me).

<sup>However, the below demo scripts could _arguably_ be more impressive than these games \:P</sup>

## Demo

Examples shown here assume you use Unity Engine, but it can be adapted to other frameworks.

Basic object move:

```cs
using UnityEngine;
using BX.Tweening;

public class BasicMove : MonoBehaviour
{
    private void Start()
    {
        // Bounce this object to some random point within unit sphere.
        BXSTween.Position(transform, transform.position + (Random.insideUnitSphere * 3f), duration: 1.2f, ease: EaseType.BounceOut);
    }
}
```

Sequence via coroutine:
```cs
using UnityEngine;
using BX.Tweening;
using System.Collections;

public class TweenCoroutine : MonoBehaviour
{
    public Transform target;
    public Vector3 posStart = new Vector3(-3f, -2f, -1f), posEnd = new Vector3(3f, 2f, 1f);

    private void Start()
    {
        StartCoroutine(SequenceTweens());
    }

    private IEnumerator SequenceTweens()
    {
        if (target == null)
        {
            yield break;
        }

        yield return new BXSWaitForTween(BXSTween.Position(target, posStart, duration: 2f));
        yield return new BXSWaitForTween(BXSTween.Position(target, posEnd, duration: 0.5f));
    }
}
```
<sup>^ the same can be done with `System.Threading.Task` using `await BXSTweenable.WaitUntilDone()`, however if you were to run this on a seperate thread; the start/end values should be get on the main thread (this means creating the tween on the main thread) or your library should be thread safe. If the setter loop is in the main thread, it is safe for the other functionality (except for the getter/setter generally).</sup> <br/>
The same can also be done with `BXSTweenSequence`.

Create an infinitely rotating and moving transform object:
```cs
using UnityEngine;
using BX.Tweening;

public class RotateAndMove : MonoBehaviour
{
    public Transform rotateAndMove;
    public Vector2 moveRange = new Vector2(-3f, 3f);
    public BXSTweenFloatContext moveAnim = new BXSTweenFloatContext(0.8f);
    public BXSTweenQuaternionContext rotateAnim = new BXSTweenQuaternionContext(0.5f);

    private void Start()
    {
        if (rotateAndMove == null)
        {
            rotateAndMove = transform;
        }

        moveAnim.SetupContext(moveRange.x, moveRange.y, (v) =>
        {
            Vector3 pos = rotateAndMove.position;
            pos.x = v;
            rotateAndMove.position = pos;
        }).SetLoopCount(-1).SetLoopType(LoopType.Yoyo);

        rotateAnim.SetupContext(() => rotateAndMove.rotation, Random.rotation, (q) =>
        {
            rotateAndMove.rotation = q;
        }).SetLoopCount(-1)
            // When the loop repeats, we set the starting value from the getter supplied.
            .SetLoopType(LoopType.Reset)
            .SetLoopRepeatAction(() => rotateAnim.SetStartValue().SetEndValue(Random.rotation));

        moveAnim.Play();
        rotateAnim.Play();
    }
}
```
![Moving Box Preview](https://github.com/b3x206/BXSTween/blob/resource/resource/move-box.gif?raw=true)

Button object that moves away from the cursor when clicked:
```cs
using BX.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EscapeButton : MonoBehaviour
{
    public Button button;
    public BXSTweenVector2Context escapeAnim = new BXSTweenVector2Context(0.6f);

    public void Start()
    {
        // This condition checks whether if the tween is setup.
        if (!escapeAnim.IsValid)
        {
            escapeAnim
                .SetStartValue(() => button.transform.localPosition)
                .SetSetter((v) =>
                    button.transform.localPosition = new Vector3(v.x, v.y, button.transform.localPosition.z)
                );
        }

        button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        Canvas parentCanvas = button.GetComponentInParent<Canvas>();
        RectTransform rt = button.GetComponent<RectTransform>();
        Vector2 btnSize = rt.rect.size;
        Vector2 btnPos = button.transform.position;
        Vector2 canvasSize = parentCanvas.GetComponent<RectTransform>().rect.size * parentCanvas.transform.lossyScale;
        float escapeFactor = Random.Range(1f, 2f);

        // While somewhat wrong and jumps too much or too little, this works for some reason.
        // If you understand RectTransform and it's many dimensions of "seperate coordinate spaces" better you can fix this.
        // ---
        // To stay within the screen, decide whether if we will escape negative or positive
        int escapeSignX;
        if ((btnPos.x - (btnSize.x * escapeFactor)) <= 0f)
        {
            escapeSignX = 1;
        }
        else if ((btnPos.x + (btnSize.x * escapeFactor)) >= canvasSize.x)
        {
            escapeSignX = -1;
        }
        else
        {
            // random
            escapeSignX = Random.Range(0f, 1f) >= 0.5f ? 1 : -1;
        }
        int escapeSignY;
        if ((btnPos.y - (btnSize.y * escapeFactor)) <= 0f)
        {
            escapeSignY = 1;
        }
        else if ((btnPos.y + (btnSize.y * escapeFactor)) >= canvasSize.y)
        {
            escapeSignY = -1;
        }
        else
        {
            // random
            escapeSignY = Random.Range(0f, 1f) >= 0.5f ? 1 : -1;
        }

        escapeAnim.SetStartValue().SetEndValue(new Vector2(btnSize.x * escapeSignX * escapeFactor, btnSize.y * escapeSignY * escapeFactor));
        // Calling play during play will cancel the previous if the tween was running.
        escapeAnim.Play();
    }
}
```
![Escaping Button Preview](https://github.com/b3x206/BXSTween/blob/resource/resource/escape-btn.gif?raw=true)

Wavy hue rainbow cubes:
```cs
using UnityEngine;
using BX.Tweening;

public class WavyCubes : MonoBehaviour
{
    public BXSTweenFloatContext boxesAnim = new(1.5f);
    public BXSTweenColorContext boxesColorAnim = new(1.5f);
    public Vector2Int boxesCount = new Vector2Int(48, 48);
    public Vector2 boxesWaveRange = new Vector2(-2f, 2f);

    private void Start()
    {
        for (int y = 0; y < boxesCount.y; y++)
        {
            for (int x = 0; x < boxesCount.x; x++)
            {
                Vector3 pos = new Vector3(y - (boxesCount.y / 2f), boxesWaveRange.x, x - (boxesCount.x / 2f));

                GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Transform trs = obj.transform;
                trs.position = pos;
                Material mat = obj.GetComponent<Renderer>().material;

                float animDelay = ((x + y) * 3f) / boxesCount.magnitude;
                BXSTweenFloatContext moveAnim = boxesAnim.AsCopy<BXSTweenFloatContext>();
                moveAnim.SetupContext(boxesWaveRange.x, boxesWaveRange.y, (v) =>
                {
                    Vector3 p = trs.position;
                    p.y = v;
                    trs.position = p;
                }).SetLoopCount(-1)
                    .SetDelay(animDelay)
                    .SetLinkObject(obj)
                    .SetTag("BoxesMove")
                    .SetWaitDelayOnLoop(false)
                    .SetLoopType(LoopType.Yoyo)
                    .Play();

                float hueStart = 0.8f * ((x + y) / (float)(boxesCount.x + boxesCount.y)), hueEnd = hueStart + 0.19f;
                // Select a random starting hue then interpolate to it
                // a smaller range is chosen to not make it look ugly (like noise), but you can also use x and y as interpolation parameters
                // then interpolate the range to show the full hue with some offsets.
                BXSTweenColorContext colorContext = boxesColorAnim.AsCopy<BXSTweenColorContext>();
                colorContext.SetupContext(() => mat.color, Random.ColorHSV(hueStart, hueEnd, 0.8f, 1f, 1f, 1f), (c) => mat.color = c)
                    .SetLoopCount(-1)
                    .SetDelay(animDelay)
                    .SetLinkObject(obj)
                    .SetTag("BoxesColor")
                    .SetWaitDelayOnLoop(false)
                    // The tween values can be not swapped as we set new start value when the loop repeats.
                    .SetLoopType(LoopType.Reset)
                    .SetLoopRepeatAction(() => colorContext.SetStartValue().SetEndValue(Random.ColorHSV(hueStart, hueEnd, 0.8f, 1f, 1f, 1f)))
                    .Play();
            }
        }

        // With tags, you can batch manage tweens:
        // ex: BXSTween.FindByTag("BoxesMove").Stop();
        //     ^ stops all tweens with tag "BoxesMove"
        //     with your own custom task scheduler or routine (or by leeching a BXSTween context) you can time it to stop after a while.
    }
}
```
![Cubes Preview](https://github.com/b3x206/BXSTween/blob/resource/resource/wavy-boxes.gif?raw=true)

---

Embed BXSTween to another framework (`Raylib_cs` shown as an example)

Note that even if included with your project, the [Editor directory](./Editor) and some unity specific code won't compile unless `UNITY_EDITOR` is defined. <br/>
Also, [shorthands](./Core/Utility/Unity/BXSTweenShorthand.cs) have to be defined for your framework, this is why I usually avoid using them.

Because some things were abstracted away in the unity way or are required to be done in that way (like lazy initialization), this may look ugly.
```cs
using Raylib_cs;
using BX.Tweening;
using BX.Tweening.Interop;
using System.Numerics;

namespace Example;

// This is assuming you are writing this on a .NET 8+ project
// If not, just change the incompatible constructors.
public sealed class BXSTweenRunner : IBXSTweenLoop
{
    private readonly List<BXSTweenable> m_RunningTweens = [];
    public List<BXSTweenable> RunningTweens => m_RunningTweens;
    private readonly BXSTweenTaskDeferrer<BXSTweenable> m_TaskDeferrer = new();
    public BXSTweenTaskDeferrer<BXSTweenable> TaskDeferrer => m_TaskDeferrer;

    private readonly IBXSTweenLogger m_Logger = new BXSTweenConsoleLogger();
    public IBXSTweenLogger Logger => m_Logger;

    // If you have a time manager you can set these to it's corresponding value.
    private int m_FrameCount = 0;
    public int ElapsedTickCount => m_FrameCount;
    private float m_DeltaTime = 0f;
    public float UnscaledDeltaTime => m_DeltaTime;
    public float TimeScale => 1f;

    // If you have a "tick" callback/function that is deterministically done 
    // N times in a second, set this true and create FixedTick the same way as Tick
    public bool SupportsFixedTick => false;
    public float FixedUnscaledDeltaTime => throw new NotImplementedException();

    public event Action<IBXSTweenLoop>? OnInit;
    public event Action<IBXSTweenLoop>? OnTick;
    public event Action<IBXSTweenLoop>? OnFixedTick;
    public event IBXSTweenLoop.ExitAction? OnExit;

    public BXSTweenRunner()
    {
        OnInit?.Invoke(this);
    }

    public void Tick(float dt)
    {
        // Minimal implementation
        m_DeltaTime = dt;
        OnTick?.Invoke(this);
        unchecked { m_FrameCount++; }
    }

    public void Kill()
    {
        OnExit?.Invoke(this, false);
    }
    public void Quit()
    {
        OnExit?.Invoke(this, true);
    }
}

// Demo Program
// Draws a cube and animates it.
// ---
public sealed class Cube
{
    public Vector3 position = Vector3.Zero;
    public Vector3 size = Vector3.One;
    public Color color = Color.Red;
    // Because i'm noob i will use euler
    // If you use Quaternion it is better, though it gets translated to render matrix anyways
    public Vector3 rotation = Vector3.Zero;

    public void Draw()
    {
        Rlgl.PushMatrix();

        Rlgl.Translatef(position.X, position.Y, position.Z);
        // Z, X, Y
        Rlgl.Rotatef(rotation.Z, 0f, 0f, 1f);
        Rlgl.Rotatef(rotation.X, 1f, 0f, 0f);
        Rlgl.Rotatef(rotation.Y, 0f, 1f, 0f);

        Raylib.DrawCube(Vector3.Zero, size.X, size.Y, size.Z, color);

        Rlgl.PopMatrix();
    }
}

class Program
{
    // Utility code
    private static readonly Random rand = new();
    private static Vector3 RandomEuler()
    {
        // also noob way of doing this
        return Vector3.Normalize(new(rand.NextSingle(), rand.NextSingle(), rand.NextSingle())) * 360f;
    }

    public static int Wrap(int value, int min, int max)
    {
        int range = max - min;
        if (range == 0)
        {
            return min;
        }
        return min + ((((value - min) % range) + range) % range);
    }

    public static Color FromRGB(float r, float g, float b) => new((byte)((r % 1f) * byte.MaxValue), (byte)((g % 1f) * byte.MaxValue), (byte)((b % 1f) * byte.MaxValue));
    public static Color HueCycle(float hue)
    {
        float r, g, b;
        hue = Math.Clamp(hue, 0f, 360f);

        float hue2, back, fwd, hueDelta;
        long hueIndex;

        hue2 = hue;
        if (hue2 >= 360.0f)
        {
            hue2 = 0.0f;
        }

        hue2 /= 60.0f;
        hueIndex = (long)hue2;
        hueDelta = hue2 - hueIndex;
        back = 1.0f - hueDelta;
        fwd = hueDelta;

        switch (hueIndex)
        {
            case 0:
                r = 0.8f;
                g = fwd;
                b = 0.2f;
                break;
            case 1:
                r = back;
                g = 0.8f;
                b = 0.2f;
                break;
            case 2:
                r = 0.2f;
                g = 0.8f;
                b = fwd;
                break;
            case 3:
                r = 0.2f;
                g = back;
                b = 0.8f;
                break;
            case 4:
                r = fwd;
                g = 0.2f;
                b = 0.8f;
                break;
            // case 5..:
            default:
                r = 0.8f;
                g = 0.2f;
                b = back;
                break;
        }

        return FromRGB(r, g, b);
    }

    public static BXSTweenFloatContext moveXAnim = new(1.4f);
    public static BXSTweenVector3Context rotateRandomAnim = new(0.6f);
    public static BXSTweenFloatContext hueAnim = new(4f);
    public static Vector2 moveRange = new(-3.5f, 3.5f);
    public static Cube cube = new();

    public const float CamRotateSpeed = 0.1f;
    public static Camera3D cam = new(new(0f, 10f, 10f), Vector3.Zero, Vector3.UnitY, 50f, CameraProjection.Perspective);

    static void Main()
    {
        // Config
        Raylib.SetConfigFlags(ConfigFlags.Msaa4xHint);
        Raylib.InitWindow(800, 480, "Anim");

        // Subsystems
        BXSTweenRunner runner = new();
        BXSTween.Initialize(() => runner); // for lazy initialization

        // Start
        moveXAnim
            .SetupContext(moveRange.X, moveRange.Y, v => cube.position.X = v)
            .SetLoopCount(-1)
            .SetEase(EaseType.QuadInOut)
            .Play();
        rotateRandomAnim
            .SetupContext(() => cube.rotation, RandomEuler(), v => cube.rotation = v)
            .SetLoopCount(-1)
            .SetLoopRepeatAction(() => rotateRandomAnim.SetStartValue().SetEndValue(RandomEuler()))
            .SetLoopType(LoopType.Reset)
            .Play();
        hueAnim
            .SetupContext(0f, 360f, v => cube.color = HueCycle(v))
            .SetLoopCount(-1)
            .Play();

        // Loop
        float time = 0f;
        float initialDistanceX = cam.Position.Y;
        float initialDistanceZ = cam.Position.Z;

        while (!Raylib.WindowShouldClose())
        {
            // Tick
            runner.Tick(Raylib.GetFrameTime());
            time += Raylib.GetFrameTime();

            if (Raylib.IsKeyPressed(KeyboardKey.Up))
            {
                moveXAnim.SetEase((EaseType)Wrap((int)moveXAnim.Ease + 1, 0, (int)EaseType.ExponentialInOut));
            }
            if (Raylib.IsKeyPressed(KeyboardKey.Down))
            {
                moveXAnim.SetEase((EaseType)Wrap((int)moveXAnim.Ease - 1, 0, (int)EaseType.ExponentialInOut));
            }

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.White);

            Raylib.DrawText($"Up, Down : Set move tween ease\nCurrent : {moveXAnim.Ease} ({(int)moveXAnim.Ease})", 20, 20, 19, Color.Black);

            Raylib.BeginMode3D(cam);
            // Rotate around
            cam.Position = new(MathF.Cos(time * CamRotateSpeed * MathF.PI) * initialDistanceX, cam.Position.Y, MathF.Sin(time * CamRotateSpeed * MathF.PI) * initialDistanceZ);

            cube.Draw();
            Raylib.DrawGrid(10, 1.0f);

            Raylib.EndMode3D();

            Raylib.EndDrawing();
        }

        runner.Quit();
        Raylib.CloseWindow();
    }
}
```
![Raylib Preview](https://github.com/b3x206/BXSTween/blob/resource/resource/raylib-cs-demo.gif?raw=true)

### TODO (there are few)
* [ ] Fix `BXSTweenSequence` implementation to be better.
* [ ] Write tests, test the code more and fix existing bugs.
* [ ] Generate DocFX
