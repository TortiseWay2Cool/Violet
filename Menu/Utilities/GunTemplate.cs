using BepInEx;
using ExitGames.Client.Photon;
using Pathfinding;
using Photon.Pun;
using Photon.Realtime;
using POpusCodec.Enums;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using Violet.Menu;

namespace Violet.Utilities
{
    public class GunTemplate : MonoBehaviour
    {
        public static readonly Color purple = new Color(0.5f, 0f, 0.5f);
        public static int LineCurve = 500;
        public const float PointerScale = 0.4f;
        public const float PulseSpeed = 3f;
        public const float PulseAmplitude = 0.04f;
        public static readonly Color Violet = new Color(0.56f, 0.0f, 1.0f);
        public static GameObject spherepointer;
        public static GameObject lineRen;
        public static LineRenderer lineRenderer;
        public static Color lineRendererColor1 = Color.Lerp(ColorLib.Black, ColorLib.Violet, Mathf.PingPong(Time.time, 2f));
        public static Color lineRendererColor2 = Color.Lerp(ColorLib.Violet, ColorLib.Black, Mathf.PingPong(Time.time, 2f));
        public static VRRig lockedPlayer;
        public static Vector3 lr;
        public static readonly Color32 PointerColor = Violet;
        public static bool lineEnabled = true;
        public static int CurrentThemeCycle = 1;
        public static Color32 CurrentBackgroundColor = ColorLib.Violet;
        public static Color32 CurrentLineColor = ColorLib.Violet;
        public static Color32 CurrentPointerColor = ColorLib.Violet;
        public static LineStyle CurrentLineStyle = LineStyle.Curvy;
        public static float CurrentLineWidth = 0.03f;
        public static int CurrentLineWidthCycle = 1;
        public static int CurrentLineStyleCycle = 1;

        public enum LineStyle
        {
            Curvy,
            Straight,
            Wavy,
            ZigZag,
            Spiral,
            Pulse,
            WavePulse,
            Twist,
            fastTwist,
            Jagged,
            Ripple,
            Fractal
        }

        public static void ChangeTheme()
        {
            CurrentThemeCycle++;
            if (CurrentThemeCycle > 10)
            {
                CurrentThemeCycle = 1;
            }

            switch (CurrentThemeCycle)
            {
                case 1:
                    CurrentBackgroundColor = ColorLib.DeepVioletTransparent;
                    CurrentLineColor = ColorLib.DeepVioletTransparent;
                    CurrentPointerColor = ColorLib.DeepVioletTransparent;
                    ButtonHandler.ChangeButtonText("Change Gun Colors", "Change Gun Colors [Violet]");
                    break;
                case 2:
                    CurrentBackgroundColor = ColorLib.RedTransparent;
                    CurrentLineColor = ColorLib.RedTransparent;
                    CurrentPointerColor = ColorLib.RedTransparent;
                    ButtonHandler.ChangeButtonText("Change Gun Colors", "Change Gun Colors [Red]");
                    break;
                case 3:
                    CurrentBackgroundColor = ColorLib.GreenTransparent;
                    CurrentLineColor = ColorLib.GreenTransparent;
                    CurrentPointerColor = ColorLib.GreenTransparent;
                    ButtonHandler.ChangeButtonText("Change Gun Colors", "Change Gun Colors [Green]");
                    break;
                case 4:
                    CurrentBackgroundColor = ColorLib.BlueTransparent;
                    CurrentLineColor = ColorLib.BlueTransparent;
                    CurrentPointerColor = ColorLib.BlueTransparent;
                    ButtonHandler.ChangeButtonText("Change Gun Colors", "Change Gun Colors [Blue]");
                    break;
                case 5:
                    CurrentBackgroundColor = ColorLib.YellowTransparent;
                    CurrentLineColor = ColorLib.YellowTransparent;
                    CurrentPointerColor = ColorLib.YellowTransparent;
                    ButtonHandler.ChangeButtonText("Change Gun Colors", "Change Gun Colors [Yellow]");
                    break;
                case 6:
                    CurrentBackgroundColor = ColorLib.OrangeTransparent;
                    CurrentLineColor = ColorLib.OrangeTransparent;
                    CurrentPointerColor = ColorLib.OrangeTransparent;
                    ButtonHandler.ChangeButtonText("Change Gun Colors", "Change Gun Colors [Orange]");
                    break;
                case 7:
                    CurrentBackgroundColor = ColorLib.PurpleTransparent;
                    CurrentLineColor = ColorLib.PurpleTransparent;
                    CurrentPointerColor = ColorLib.PurpleTransparent;
                    ButtonHandler.ChangeButtonText("Change Gun Colors", "Change Gun Colors [Purple]");
                    break;
                case 8:
                    CurrentBackgroundColor = ColorLib.PinkTransparent;
                    CurrentLineColor = ColorLib.PinkTransparent;
                    CurrentPointerColor = ColorLib.PinkTransparent;
                    ButtonHandler.ChangeButtonText("Change Gun Colors", "Change Gun Colors [Pink]");
                    break;
                case 9:
                    CurrentBackgroundColor = ColorLib.CyanTransparent;
                    CurrentLineColor = ColorLib.CyanTransparent;
                    CurrentPointerColor = ColorLib.CyanTransparent;
                    ButtonHandler.ChangeButtonText("Change Gun Colors", "Change Gun Colors [Cyan]");
                    break;
                case 10:
                    CurrentBackgroundColor = ColorLib.BlackTransparent;
                    CurrentLineColor = ColorLib.BlackTransparent;
                    CurrentPointerColor = ColorLib.BlackTransparent;
                    ButtonHandler.ChangeButtonText("Change Gun Colors", "Change Gun Colors [Black]");
                    break;
                default:
                    CurrentBackgroundColor = ColorLib.DeepVioletTransparent;
                    CurrentLineColor = ColorLib.DeepVioletTransparent;
                    CurrentPointerColor = ColorLib.DeepVioletTransparent;
                    ButtonHandler.ChangeButtonText("Change Gun Colors", "Change Gun Colors [Violet]");
                    break;
            }
        }

        public static void ChangeLineStyle()
        {
            CurrentLineStyleCycle++;
            if (CurrentLineStyleCycle > 11)
            {
                CurrentLineStyleCycle = 1;
            }

            switch (CurrentLineStyleCycle)
            {
                case 1:
                    CurrentLineStyle = LineStyle.Curvy;
                    ButtonHandler.ChangeButtonText("Change Gun Line", "Change Gun Line [Curvy]");
                    break;
                case 2:
                    CurrentLineStyle = LineStyle.Straight;
                    ButtonHandler.ChangeButtonText("Change Gun Line", "Change Gun Line [Straight]");
                    break;
                case 3:
                    CurrentLineStyle = LineStyle.Wavy;
                    ButtonHandler.ChangeButtonText("Change Gun Line", "Change Gun Line [Wavy]");
                    break;
                case 4:
                    CurrentLineStyle = LineStyle.ZigZag;
                    ButtonHandler.ChangeButtonText("Change Gun Line", "Change Gun Line [ZigZag]");
                    break;
                case 5:
                    CurrentLineStyle = LineStyle.Spiral;
                    ButtonHandler.ChangeButtonText("Change Gun Line", "Change Gun Line [Spiral]");
                    break;
                case 6:
                    CurrentLineStyle = LineStyle.Pulse;
                    ButtonHandler.ChangeButtonText("Change Gun Line", "Change Gun Line [Pulse]");
                    break;
                case 7:
                    CurrentLineStyle = LineStyle.WavePulse;
                    ButtonHandler.ChangeButtonText("Change Gun Line", "Change Gun Line [WavePulse]");
                    break;
                case 8:
                    CurrentLineStyle = LineStyle.Twist;
                    ButtonHandler.ChangeButtonText("Change Gun Line", "Change Gun Line [Twist]");
                    break;
                case 9:
                    CurrentLineStyle = LineStyle.fastTwist;
                    ButtonHandler.ChangeButtonText("Change Gun Line", "Change Gun Line [Big Twist]");
                    break;
                case 10:
                    CurrentLineStyle = LineStyle.Jagged;
                    ButtonHandler.ChangeButtonText("Change Gun Line", "Change Gun Line [Jagged]");
                    break;
                case 11:
                    CurrentLineStyle = LineStyle.Ripple;
                    ButtonHandler.ChangeButtonText("Change Gun Line", "Change Gun Line [Ripple]");
                    break;
                case 12:
                    CurrentLineStyle = LineStyle.Fractal;
                    ButtonHandler.ChangeButtonText("Change Gun Line", "Change Gun Line [Fractal]");
                    break;
                default:
                    CurrentLineStyle = LineStyle.Curvy;
                    ButtonHandler.ChangeButtonText("Change Gun Line", "Change Gun Line [Curvy]");
                    break;
            }
        }

        public static void ChangeLineWidth()
        {
            CurrentLineWidthCycle++;
            if (CurrentLineWidthCycle > 5)
            {
                CurrentLineWidthCycle = 1;
            }

            switch (CurrentLineWidthCycle)
            {
                case 1:
                    CurrentLineWidth = 0.02f;
                    ButtonHandler.ChangeButtonText("Change Line Width", "Change Line Width [Thin]");
                    break;
                case 2:
                    CurrentLineWidth = 0.03f;
                    ButtonHandler.ChangeButtonText("Change Line Width", "Change Line Width [Normal]");
                    break;
                case 3:
                    CurrentLineWidth = 0.04f;
                    ButtonHandler.ChangeButtonText("Change Line Width", "Change Line Width [Medium]");
                    break;
                case 4:
                    CurrentLineWidth = 0.07f;
                    ButtonHandler.ChangeButtonText("Change Line Width", "Change Line Width [Thick]");
                    break;
                case 5:
                    CurrentLineWidth = 0.09f;
                    ButtonHandler.ChangeButtonText("Change Line Width", "Change Line Width [Very Thick]");
                    break;
                default:
                    CurrentLineWidth = 0.03f;
                    ButtonHandler.ChangeButtonText("Change Line Width", "Change Line Width [Normal]");
                    break;
            }
        }

        public static void StartVrGun(Action action, bool LockOn)
        {
            if (ControllerInputPoller.instance.rightGrab)
            {
                Physics.Raycast(GorillaTagger.Instance.rightHandTransform.position, -GorillaTagger.Instance.rightHandTransform.up, out nray, float.MaxValue);
                if (GunTemplate.spherepointer == null)
                {
                    GunTemplate.spherepointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    GunTemplate.spherepointer.AddComponent<Renderer>();
                    GunTemplate.spherepointer.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    GunTemplate.spherepointer.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    GameObject.Destroy(GunTemplate.spherepointer.GetComponent<BoxCollider>());
                    GameObject.Destroy(GunTemplate.spherepointer.GetComponent<Rigidbody>());
                    GameObject.Destroy(GunTemplate.spherepointer.GetComponent<Collider>());
                    GunTemplate.lr = GorillaTagger.Instance.offlineVRRig.rightHandTransform.position;
                }

                if (GunTemplate.lockedPlayer == null)
                {
                    GunTemplate.spherepointer.transform.position = GunTemplate.nray.point;
                    GunTemplate.spherepointer.GetComponent<Renderer>().material.color = CurrentPointerColor;
                }
                else
                {
                    GunTemplate.spherepointer.transform.position = GunTemplate.lockedPlayer.transform.position;
                }

                lr = Vector3.Lerp(GunTemplate.lr, (GorillaTagger.Instance.rightHandTransform.position + GunTemplate.spherepointer.transform.position) / 2f, Time.deltaTime * 6f);
                GameObject gameObject = new GameObject("Line");
                lineRenderer = gameObject.AddComponent<LineRenderer>();
                lineRenderer.startWidth = CurrentLineWidth;
                lineRenderer.endWidth = CurrentLineWidth;
                lineRenderer.startColor = CurrentLineColor;
                lineRenderer.endColor = CurrentLineColor;
                lineRenderer.useWorldSpace = true;
                lineRenderer.material = new Material(Shader.Find("GUI/Text Shader"));
                gameObject.AddComponent<GunTemplate>().StartCoroutine(GunTemplate.StartLineRenderer(lineRenderer, GorillaTagger.Instance.rightHandTransform.position, GunTemplate.lr, GunTemplate.spherepointer.transform.position));
                GameObject.Destroy(lineRenderer, Time.deltaTime);
                if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.5f)
                {
                    if (LockOn)
                    {
                        if (GunTemplate.lockedPlayer == null)
                        {
                            GunTemplate.lockedPlayer = GunTemplate.nray.collider.GetComponentInParent<VRRig>();
                        }
                        if (GunTemplate.lockedPlayer != null)
                        {
                            GunTemplate.spherepointer.transform.position = GunTemplate.lockedPlayer.transform.position;
                            action();
                        }
                    }
                    else
                    {
                        action();
                    }
                }
                else if (GunTemplate.lockedPlayer != null)
                {
                    GunTemplate.lockedPlayer = null;
                }
            }
            else if (GunTemplate.spherepointer != null)
            {
                GameObject.Destroy(GunTemplate.spherepointer);
                GunTemplate.spherepointer = null;
                GunTemplate.lockedPlayer = null;
            }
        }

        private static void RenderLine(LineRenderer lineRenderer, Vector3 start, Vector3 mid, Vector3 end)
        {
            if (CurrentLineStyle == LineStyle.Curvy)
            {
                lineRenderer.positionCount = LineCurve;
                float springAmplitude = 0.135f;
                float springFrequency = 18f;

                for (int i = 0; i < LineCurve; i++)
                {
                    float t = (float)i / (LineCurve - 1);
                    Vector3 basePoint = Vector3.Lerp(start, end, t);
                    float tPlus = t + 0.01f;
                    Vector3 nextPoint = Vector3.Lerp(start, end, tPlus);
                    Vector3 tangent = (nextPoint - basePoint).normalized;
                    Vector3 normal = Vector3.Cross(tangent, Vector3.up).normalized;
                    Vector3 binormal = Vector3.Cross(tangent, normal).normalized;
                    float angle = i * Mathf.PI * springFrequency / LineCurve;
                    Vector3 coilOffset = normal * Mathf.Sin(angle) * springAmplitude;
                    lineRenderer.SetPosition(i, basePoint + coilOffset);
                }
            }
            else if (CurrentLineStyle == LineStyle.Straight)
            {
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, start);
                lineRenderer.SetPosition(1, end);
            }
            else if (CurrentLineStyle == LineStyle.Wavy)
            {
                lineRenderer.positionCount = LineCurve;
                float waveAmplitude = 0.05f;
                float waveFrequency = 10f;
                for (int i = 0; i < LineCurve; i++)
                {
                    float t = (float)i / (LineCurve - 1);
                    Vector3 basePoint = Vector3.Lerp(start, end, t);
                    Vector3 tangent = (end - start).normalized;
                    Vector3 normal = Vector3.Cross(tangent, Vector3.up).normalized;
                    float angle = i * Mathf.PI * waveFrequency / LineCurve;
                    Vector3 waveOffset = normal * Mathf.Sin(angle) * waveAmplitude;
                    lineRenderer.SetPosition(i, basePoint + waveOffset);
                }
            }
            else if (CurrentLineStyle == LineStyle.ZigZag)
            {
                lineRenderer.positionCount = LineCurve;
                float zigZagAmplitude = 0.1f;
                float zigZagFrequency = 15f;
                for (int i = 0; i < LineCurve; i++)
                {
                    float t = (float)i / (LineCurve - 1);
                    Vector3 basePoint = Vector3.Lerp(start, end, t);
                    Vector3 tangent = (end - start).normalized;
                    Vector3 normal = Vector3.Cross(tangent, Vector3.up).normalized;
                    float angle = i * Mathf.PI * zigZagFrequency / LineCurve;
                    Vector3 zigZagOffset = normal * (Mathf.PingPong(angle, 1f) - 0.5f) * zigZagAmplitude * 2f;
                    lineRenderer.SetPosition(i, basePoint + zigZagOffset);
                }
            }
            else if (CurrentLineStyle == LineStyle.Spiral)
            {
                lineRenderer.positionCount = LineCurve;
                float spiralRadius = 0.05f;
                float spiralFrequency = 12f;
                for (int i = 0; i < LineCurve; i++)
                {
                    float t = (float)i / (LineCurve - 1);
                    Vector3 basePoint = Vector3.Lerp(start, end, t);
                    Vector3 tangent = (end - start).normalized;
                    Vector3 normal = Vector3.Cross(tangent, Vector3.up).normalized;
                    Vector3 binormal = Vector3.Cross(tangent, normal).normalized;
                    float angle = i * Mathf.PI * spiralFrequency / LineCurve;
                    Vector3 spiralOffset = (normal * Mathf.Cos(angle) + binormal * Mathf.Sin(angle)) * spiralRadius;
                    lineRenderer.SetPosition(i, basePoint + spiralOffset);
                }
            }
            else if (CurrentLineStyle == LineStyle.Pulse)
            {
                lineRenderer.positionCount = LineCurve;
                float pulseAmplitude = 0.02f;
                float pulseFrequency = 8f;
                float[] widths = new float[LineCurve];
                for (int i = 0; i < LineCurve; i++)
                {
                    float t = (float)i / (LineCurve - 1);
                    Vector3 basePoint = Vector3.Lerp(start, end, t);
                    lineRenderer.SetPosition(i, basePoint);
                    float pulse = Mathf.Sin(i * Mathf.PI * pulseFrequency / LineCurve + Time.time * PulseSpeed) * pulseAmplitude + CurrentLineWidth;
                    widths[i] = pulse;
                }
                lineRenderer.widthCurve = new AnimationCurve(widths.Select((w, i) => new Keyframe((float)i / (LineCurve - 1), w)).ToArray());
            }
            else if (CurrentLineStyle == LineStyle.WavePulse)
            {
                lineRenderer.positionCount = LineCurve;
                float waveAmplitude = 0.05f;
                float waveFrequency = 10f;
                Color startColor = CurrentLineColor;
                Color[] colors = new Color[LineCurve];
                for (int i = 0; i < LineCurve; i++)
                {
                    float t = (float)i / (LineCurve - 1);
                    Vector3 basePoint = Vector3.Lerp(start, end, t);
                    Vector3 tangent = (end - start).normalized;
                    Vector3 normal = Vector3.Cross(tangent, Vector3.up).normalized;
                    float angle = i * Mathf.PI * waveFrequency / LineCurve;
                    Vector3 waveOffset = normal * Mathf.Sin(angle + Time.time * PulseSpeed) * waveAmplitude;
                    lineRenderer.SetPosition(i, basePoint + waveOffset);
                    float alpha = Mathf.Sin(i * Mathf.PI * waveFrequency / LineCurve + Time.time * PulseSpeed) * 0.5f + 0.5f;
                    colors[i] = new Color(startColor.r, startColor.g, startColor.b, alpha);
                }
                lineRenderer.startColor = colors[0];
                lineRenderer.endColor = colors[LineCurve - 1];
            }
            else if (CurrentLineStyle == LineStyle.fastTwist)
            {
                lineRenderer.positionCount = LineCurve;
                float twistRadius = 0.12f;
                float twistFrequency = 8f;
                for (int i = 0; i < LineCurve; i++)
                {
                    float t = (float)i / (LineCurve - 1);
                    Vector3 basePoint = Vector3.Lerp(start, end, t);
                    Vector3 tangent = (end - start).normalized;
                    Vector3 normal = Vector3.Cross(tangent, Vector3.up).normalized;
                    Vector3 binormal = Vector3.Cross(tangent, normal).normalized;
                    float angle = i * Mathf.PI * twistFrequency / LineCurve;
                    Vector3 twistOffset = (normal * Mathf.Cos(angle + Time.time) + binormal * Mathf.Sin(angle + Time.time)) * twistRadius;
                    lineRenderer.SetPosition(i, basePoint + twistOffset);
                }
            }
            else if (CurrentLineStyle == LineStyle.Twist)
            {
                lineRenderer.positionCount = LineCurve;
                float twistRadius = 0.06f;
                float twistFrequency = 8f;
                for (int i = 0; i < LineCurve; i++)
                {
                    float t = (float)i / (LineCurve - 1);
                    Vector3 basePoint = Vector3.Lerp(start, end, t);
                    Vector3 tangent = (end - start).normalized;
                    Vector3 normal = Vector3.Cross(tangent, Vector3.up).normalized;
                    Vector3 binormal = Vector3.Cross(tangent, normal).normalized;
                    float angle = i * Mathf.PI * twistFrequency / LineCurve;
                    Vector3 twistOffset = (normal * Mathf.Cos(angle + Time.time) + binormal * Mathf.Sin(angle + Time.time)) * twistRadius;
                    lineRenderer.SetPosition(i, basePoint + twistOffset);
                }
            }
            else if (CurrentLineStyle == LineStyle.Jagged)
            {
                lineRenderer.positionCount = LineCurve;
                float jaggedAmplitude = 0.08f;
                float jaggedFrequency = 15f;
                System.Random rand = new System.Random(42);
                for (int i = 0; i < LineCurve; i++)
                {
                    float t = (float)i / (LineCurve - 1);
                    Vector3 basePoint = Vector3.Lerp(start, end, t);
                    Vector3 tangent = (end - start).normalized;
                    Vector3 normal = Vector3.Cross(tangent, Vector3.up).normalized;
                    float randomOffset = (float)(rand.NextDouble() * 2 - 1) * jaggedAmplitude;
                    Vector3 jaggedOffset = normal * randomOffset * Mathf.Sin(i * Mathf.PI * jaggedFrequency / LineCurve);
                    lineRenderer.SetPosition(i, basePoint + jaggedOffset);
                }
            }
            else if (CurrentLineStyle == LineStyle.Ripple)
            {
                lineRenderer.positionCount = LineCurve;
                float rippleAmplitude = 0.07f;
                float rippleFrequency = 6f;
                for (int i = 0; i < LineCurve; i++)
                {
                    float t = (float)i / (LineCurve - 1);
                    Vector3 basePoint = Vector3.Lerp(start, end, t);
                    Vector3 tangent = (end - start).normalized;
                    Vector3 normal = Vector3.Cross(tangent, Vector3.up).normalized;
                    Vector3 binormal = Vector3.Cross(tangent, normal).normalized;
                    float distance = t * Vector3.Distance(start, end);
                    float ripple = Mathf.Sin(distance * rippleFrequency + Time.time * PulseSpeed) * rippleAmplitude * (1 - t);
                    Vector3 rippleOffset = (normal * Mathf.Cos(distance * rippleFrequency) + binormal * Mathf.Sin(distance * rippleFrequency)) * ripple;
                    lineRenderer.SetPosition(i, basePoint + rippleOffset);
                }
            }
            else if (CurrentLineStyle == LineStyle.Fractal)
            {
                lineRenderer.positionCount = LineCurve;
                float fractalAmplitude = 0.1f;
                float fractalFrequency = 10f;
                System.Random rand = new System.Random(42);
                for (int i = 0; i < LineCurve; i++)
                {
                    float t = (float)i / (LineCurve - 1);
                    Vector3 basePoint = Vector3.Lerp(start, end, t);
                    Vector3 tangent = (end - start).normalized;
                    Vector3 normal = Vector3.Cross(tangent, Vector3.up).normalized;
                    float noise = 0f;
                    for (int level = 1; level <= 3; level++)
                    {
                        float scale = Mathf.Pow(2f, level);
                        noise += (float)(rand.NextDouble() * 2 - 1) * fractalAmplitude / scale * Mathf.Sin(i * Mathf.PI * fractalFrequency * scale / LineCurve);
                    }
                    Vector3 fractalOffset = normal * noise;
                    lineRenderer.SetPosition(i, basePoint + fractalOffset);
                }
            }
        }

        public static IEnumerator StartLineRenderer(LineRenderer lineRenderer, Vector3 start, Vector3 mid, Vector3 end)
        {
            while (true)
            {
                RenderLine(lineRenderer, start, mid, end);
                yield return null;
            }
        }

        public static RaycastHit nray;

        public static void StartPcGun(Action action, bool LockOn)
        {
            Ray ray = GameObject.Find("Shoulder Camera").activeSelf ? GameObject.Find("Shoulder Camera").GetComponent<Camera>().ScreenPointToRay(UnityInput.Current.mousePosition) : GorillaTagger.Instance.mainCamera.GetComponent<Camera>().ScreenPointToRay(UnityInput.Current.mousePosition);

            if (Mouse.current.rightButton.isPressed)
            {
                if (Physics.Raycast(ray.origin, ray.direction, out nray, float.PositiveInfinity, -32777) && spherepointer == null)
                {
                    if (spherepointer == null)
                    {
                        spherepointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        spherepointer.AddComponent<Renderer>();
                        spherepointer.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                        spherepointer.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                        GameObject.Destroy(spherepointer.GetComponent<BoxCollider>());
                        GameObject.Destroy(spherepointer.GetComponent<Rigidbody>());
                        GameObject.Destroy(spherepointer.GetComponent<Collider>());
                        lr = GorillaTagger.Instance.offlineVRRig.rightHandTransform.position;
                    }
                }
                if (lockedPlayer == null)
                {
                    spherepointer.transform.position = nray.point;
                    spherepointer.GetComponent<Renderer>().material.color = CurrentPointerColor;
                }
                else
                {
                    spherepointer.transform.position = lockedPlayer.transform.position;
                }
                lr = Vector3.Lerp(lr, (GorillaTagger.Instance.rightHandTransform.position + spherepointer.transform.position) / 2f, Time.deltaTime * 6f);

                GameObject gameObject = new GameObject("Linee");
                lineRenderer = gameObject.AddComponent<LineRenderer>();
                lineRenderer.startWidth = CurrentLineWidth;
                lineRenderer.endWidth = CurrentLineWidth;
                lineRenderer.startColor = CurrentLineColor;
                lineRenderer.endColor = CurrentLineColor;
                lineRenderer.useWorldSpace = true;
                lineRenderer.material = new Material(Shader.Find("GUI/Text Shader"));
                gameObject.AddComponent<GunTemplate>().StartCoroutine(StartLineRenderer(lineRenderer, GorillaTagger.Instance.rightHandTransform.position, lr, spherepointer.transform.position));
                GameObject.Destroy(lineRenderer, Time.deltaTime);
                if (Mouse.current.leftButton.isPressed)
                {
                    spherepointer.GetComponent<Renderer>().material.color = CurrentPointerColor;
                    if (LockOn)
                    {
                        if (lockedPlayer == null)
                        {
                            lockedPlayer = nray.collider.GetComponentInParent<VRRig>();
                        }
                        if (lockedPlayer != null)
                        {
                            spherepointer.transform.position = lockedPlayer.transform.position;
                            action();
                        }
                        return;
                    }
                    action();
                    return;
                }
                else if (lockedPlayer != null)
                {
                    lockedPlayer = null;
                    return;
                }
            }
            else if (spherepointer != null)
            {
                GameObject.Destroy(spherepointer);
                spherepointer = null;
                lockedPlayer = null;
            }
        }

        public static void StartBothGuns(Action action, bool locko)
        {
            if (XRSettings.isDeviceActive)
            {
                StartVrGun(action, locko);
            }
            if (!XRSettings.isDeviceActive)
            {
                StartPcGun(action, locko);
            }
        }

  
        public static void GunExample()
        {
            StartBothGuns(() =>
            {

            }, true);
        }
    }
}
