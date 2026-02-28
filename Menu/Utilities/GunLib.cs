using BepInEx;
using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using Violet.Menu;

namespace Violet.Menu.Utilities
{
    internal class GunLib : MonoBehaviour
    {
        public static Vector3 currentPointerPos = Vector3.zero;
        public static Vector3 currentLineEnd = Vector3.zero;
        public static GameObject pointer = null;
        public static LineRenderer Line = null;
        public static RaycastHit raycastHit;
        public static bool hand = false;
        public static bool hand1 = false;
        public static VRRig LockedRig;
        public static bool IsLocking = false;

        private static readonly int ParticleCount = 48;
        private class ParticleState
        {
            public GameObject go;
            public Transform t;
            public Renderer rend;
            public float angle;
            public float radius;
            public float along;
            public float orbitSpeed;
            public float drift;
            public float convergeSpeed;
            public bool active;
        }
        private static ParticleState[] particles = null;

        private enum ParticlePhase { Idle, Charging, Converging, Returning }
        private static ParticlePhase particlePhase = ParticlePhase.Idle;
        private static float particleTubeRadius = 0.12f;
        private static float particleFloatSpeed = 0.04f; // very slow drift
        private static float particleConvergeBaseSpeed = 18f; // speed when converging
        private static float particleThreshold = 0.08f;

        private static float chargeStartTime = 0f;
        private static float chargeDuration = 0.65f;
        private static float chargingOrbitMultiplier = 3.5f; 

        private static Material sharedParticleMaterial = null;
        private static Material sharedLineMaterial = null;
        private static Material sharedTrailMaterial = null;
        private static MaterialPropertyBlock mpb = new MaterialPropertyBlock();

        public static void MakeGun(bool gunLock, Action onLockCallback, Action onUnLockCallback = null)
        {
            try
            {
                Transform arm = GorillaLocomotion.GTPlayer.Instance.GetControllerTransform(false);
                hand = ControllerInputPoller.instance.rightGrab || Mouse.current?.rightButton?.isPressed == true;
                hand1 = ControllerInputPoller.TriggerFloat(XRNode.RightHand) > 0.5f || Mouse.current?.leftButton?.isPressed == true;

                Color targetColor;
                switch (GunSettings.PointerColorMode)
                {
                    case PointerColorMode.Normal:
                        targetColor = hand1 ? Color.green : Color.red;
                        break;
                    case PointerColorMode.Red:
                        targetColor = Color.red;
                        break;
                    case PointerColorMode.Green:
                        targetColor = Color.green;
                        break;
                    case PointerColorMode.Blue:
                        targetColor = Color.blue;
                        break;
                    case PointerColorMode.Yellow:
                        targetColor = Color.yellow;
                        break;
                    case PointerColorMode.White:
                        targetColor = Color.white;
                        break;
                    case PointerColorMode.Magenta:
                        targetColor = Color.magenta;
                        break;
                    case PointerColorMode.Custom:
                        targetColor = GunSettings.PointerColor;
                        break;
                    default:
                        targetColor = GunSettings.PointerColor;
                        break;
                }

                if (!hand)
                {
                    Cleanup();
                    return;
                }

                bool hit = false;
                if (XRSettings.isDeviceActive)
                {
                    hit = Physics.Raycast(arm.position, -arm.up, out raycastHit);
                }
                else
                {
                    var cam = Main.thirdPersonCamera.GetComponentInParent<Camera>();
                    if (cam != null)
                        hit = Physics.Raycast(cam.ScreenPointToRay(Mouse.current.position.ReadValue()), out raycastHit, 512f, InvisLayerMaskFix());
                }

                if (pointer == null)
                {
                    pointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    if (pointer.TryGetComponent(out Collider col)) Destroy(col);
                    if (pointer.TryGetComponent(out Rigidbody rb)) Destroy(rb);
                }

                Vector3 targetPoint = IsLocking && LockedRig ? LockedRig.transform.position : (hit ? raycastHit.point : arm.position + arm.forward * 2f);
                if (currentPointerPos == Vector3.zero) currentPointerPos = targetPoint;

                if (GunSettings.EnablePointerAnimation && Time.deltaTime > 0f)
                    currentPointerPos = Vector3.Lerp(currentPointerPos, targetPoint, Time.deltaTime * 15f);
                else
                    currentPointerPos = targetPoint;

                pointer.transform.position = currentPointerPos;
                pointer.transform.localScale = Vector3.one * GunSettings.PointerScale;
                SetMaterialColor(pointer, targetColor, true); // immediate set

                var trail = pointer.GetComponent<TrailRenderer>();
                if (GunSettings.GunOrbTrail)
                {
                    if (trail == null)
                    {
                        trail = pointer.AddComponent<TrailRenderer>();
                        trail.time = 0.3f;
                        trail.startWidth = GunSettings.PointerScale * 0.5f;
                        trail.endWidth = 0f;
                        if (sharedTrailMaterial == null) sharedTrailMaterial = new Material(Shader.Find(GunSettings.ShaderName));
                        trail.material = sharedTrailMaterial;
                        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                        trail.receiveShadows = false;
                        trail.autodestruct = false;
                    }
                    else
                    {
                        trail.enabled = true;
                        trail.startWidth = GunSettings.PointerScale * 0.5f;
                    }

                    trail.startColor = trail.endColor = targetColor;
                }
                else
                {
                    if (trail != null)
                    {
                        trail.enabled = false;
                    }
                }

                UpdateLineRenderer(arm, targetColor, pointer.transform.position);

                if (hand1)
                {
                    if (gunLock)
                    {
                        if (!IsLocking)
                        {
                            if (raycastHit.collider != null)
                            {
                                var candidate = raycastHit.collider.GetComponentInParent<VRRig>();
                                if (candidate != null)
                                {
                                    IsLocking = true;
                                    LockedRig = candidate;
                                }
                            }
                        }
                        else
                        {
                            onLockCallback?.Invoke();
                        }
                    }
                    RunMethod(gunLock, onLockCallback, onUnLockCallback);
                }
                else
                {
                    onUnLockCallback?.Invoke();
                    LockedRig = null;
                    IsLocking = false;
                }

                Vector3 startPos = arm.position;
                Vector3 endPos = pointer != null ? pointer.transform.position : (startPos + arm.forward * 1f);
                Vector3 dir = endPos - startPos;
                float len = Mathf.Max(0.0001f, dir.magnitude);
                Vector3 dirNorm = dir / len;
                Vector3 arbitrary = Vector3.up;
                if (Mathf.Abs(Vector3.Dot(arbitrary, dirNorm)) > 0.9f) arbitrary = Vector3.forward;
                Vector3 right = Vector3.Cross(dirNorm, arbitrary).normalized;
                Vector3 up = Vector3.Cross(right, dirNorm).normalized;

                if (particles == null)
                {
                    particles = new ParticleState[ParticleCount];
                    System.Random rnd = new System.Random();
                    if (sharedParticleMaterial == null) sharedParticleMaterial = new Material(Shader.Find(GunSettings.ShaderName));
                    for (int i = 0; i < ParticleCount; i++)
                    {
                        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        float scale = GunSettings.PointerScale * (0.06f + (float)rnd.NextDouble() * 0.12f);
                        go.transform.localScale = Vector3.one * scale;
                        if (go.TryGetComponent(out Collider c)) UnityEngine.Object.Destroy(c);
                        if (go.TryGetComponent(out Rigidbody r)) UnityEngine.Object.Destroy(r);
                        var rend = go.GetComponent<Renderer>();
                        rend.sharedMaterial = sharedParticleMaterial;
                        rend.sharedMaterial.color = Color.Lerp(sharedParticleMaterial.color, targetColor, 0.2f);

                        float angle = (float)(i * Mathf.PI * 2f / ParticleCount) + (float)rnd.NextDouble() * 0.5f;
                        float radius = particleTubeRadius * (0.6f + (float)rnd.NextDouble() * 0.9f);
                        float along = (float)rnd.NextDouble();

                        Vector3 tubePos = startPos + dirNorm * (along * len);
                        Vector3 radial = (Mathf.Cos(angle) * right + Mathf.Sin(angle) * up) * radius;
                        go.transform.position = tubePos + radial; 

                        var st = new ParticleState
                        {
                            go = go,
                            t = go.transform,
                            rend = rend,
                            angle = angle,
                            radius = radius,
                            along = along,
                            orbitSpeed = 0.6f + (float)rnd.NextDouble() * 1.4f,
                            drift = particleFloatSpeed * (0.3f + (float)rnd.NextDouble() * 1.2f),
                            convergeSpeed = particleConvergeBaseSpeed * (0.9f + (float)rnd.NextDouble() * 0.6f),
                            active = true
                        };
                        particles[i] = st;
                    }
                }

                if (hand1)
                {
                    if (particlePhase == ParticlePhase.Idle || particlePhase == ParticlePhase.Returning)
                    {
                        particlePhase = ParticlePhase.Charging;
                        chargeStartTime = Time.time;
                        ResetParticles(startPos, arm); 
                    }
                }
                else
                {
                    if (particlePhase == ParticlePhase.Charging || particlePhase == ParticlePhase.Converging)
                    {
                        particlePhase = ParticlePhase.Returning;
                        ResetParticles(startPos, arm); 
                    }
                }

                if (particlePhase == ParticlePhase.Charging)
                {
                    if (Time.time - chargeStartTime >= chargeDuration)
                    {
                        particlePhase = ParticlePhase.Converging;
                    }
                }

                bool anyActive = false;
                int plen = particles?.Length ?? 0;
                for (int i = 0; i < plen; i++)
                {
                    var st = particles[i];
                    if (st == null || st.go == null) continue;

                    if (st.rend != null)
                    {
                        mpb.Clear();
                        mpb.SetColor("_Color", targetColor);
                        st.rend.SetPropertyBlock(mpb);
                    }

                    Vector3 tubePos = startPos + dirNorm * (st.along * len);
                    Vector3 radial = (Mathf.Cos(st.angle) * right + Mathf.Sin(st.angle) * up) * st.radius;
                    Vector3 tubeWorld = tubePos + radial;

                    if (particlePhase == ParticlePhase.Idle)
                    {
                        st.angle += st.orbitSpeed * Time.deltaTime * 0.4f;
                        st.along += st.drift * Time.deltaTime;
                        if (st.along > 1f) st.along -= 1f;
                        if (st.along < 0f) st.along += 1f;

                        tubePos = startPos + dirNorm * (st.along * len);
                        radial = (Mathf.Cos(st.angle) * right + Mathf.Sin(st.angle) * up) * st.radius;
                        tubeWorld = tubePos + radial;

                        st.t.position = tubeWorld;
                        st.go.SetActive(true);
                        st.active = true;
                        anyActive = true;
                    }
                    else if (particlePhase == ParticlePhase.Charging)
                    {
                        st.angle += st.orbitSpeed * chargingOrbitMultiplier * Time.deltaTime;
                        st.along += st.drift * Time.deltaTime;
                        if (st.along > 1f) st.along -= 1f;
                        if (st.along < 0f) st.along += 1f;

                        tubePos = startPos + dirNorm * (st.along * len);
                        radial = (Mathf.Cos(st.angle) * right + Mathf.Sin(st.angle) * up) * st.radius;
                        tubeWorld = tubePos + radial;

                        Vector3 pulse = radial.normalized * (st.radius * 0.25f) * Mathf.Sin((Time.time - chargeStartTime) * 8f);
                        st.t.position = tubeWorld + pulse;
                        st.go.SetActive(true);
                        st.active = true;
                        anyActive = true;
                    }
                    else if (particlePhase == ParticlePhase.Converging)
                    {
                        Vector3 cur = st.t.position;
                        Vector3 target = endPos;
                        float speed = st.convergeSpeed;
                        float bias = 0.5f + 0.5f * (1f - st.along);
                        Vector3 next = Vector3.MoveTowards(cur, target, speed * bias * Time.deltaTime);
                        st.t.position = next;

                        float d = Vector3.Distance(next, target);
                        float scaleFactor = Mathf.Clamp01(d / 1.0f);
                        st.t.localScale = Vector3.one * (GunSettings.PointerScale * Mathf.Lerp(0.02f, 0.12f, scaleFactor));

                        if (d <= particleThreshold)
                        {
                            st.go.SetActive(false);
                            st.active = false;
                        }
                        else
                        {
                            anyActive = true;
                        }
                    }
                    else if (particlePhase == ParticlePhase.Returning)
                    {
                        if (!st.go.activeSelf)
                        {
                            st.go.SetActive(true);
                        }

                        st.t.position = tubeWorld;
                        st.t.localScale = Vector3.one * (GunSettings.PointerScale * 0.12f);
                        st.active = true;
                    }
                }

                if (particlePhase == ParticlePhase.Returning)
                {
                    bool allBack = true;
                    for (int i = 0; i < plen; i++)
                    {
                        var st = particles[i];
                        if (st == null || st.go == null) continue;
                        Vector3 tube = startPos + dirNorm * (st.along * len);
                        Vector3 rad = (Mathf.Cos(st.angle) * right + Mathf.Sin(st.angle) * up) * st.radius;
                        Vector3 desired = tube + rad;
                        if (Vector3.Distance(st.t.position, desired) > 0.04f)
                        {
                            allBack = false;
                            break;
                        }
                    }
                    if (allBack)
                    {
                        particlePhase = ParticlePhase.Idle;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GunLib] Error in MakeGun: {ex}");
            }
        }


        public static void RunMethod(bool gunLock, Action onLockCallback, Action onUnLockCallback = null)
        {
            if (gunLock)
            {
                onLockCallback?.Invoke();
            }
            else
            {
                onUnLockCallback?.Invoke();
            }
        }

        private static void SetMaterialColor(GameObject obj, Color color, bool immediate = false)
        {
            try
            {
                var renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    if (renderer.sharedMaterial == null)
                        renderer.sharedMaterial = new Material(Shader.Find(GunSettings.ShaderName));
                    renderer.sharedMaterial.shader = Shader.Find(GunSettings.ShaderName);

                    if (immediate || Time.deltaTime <= 0f)
                    {
                        renderer.sharedMaterial.color = color;
                    }
                    else
                    {
                        mpb.Clear();
                        mpb.SetColor("_Color", Color.Lerp(renderer.sharedMaterial.color, color, Time.deltaTime * 5f));
                        renderer.SetPropertyBlock(mpb);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[GunLib] Failed to set material color: {e}");
            }
        }

        private static void UpdateLineRenderer(Transform arm, Color targetColor, Vector3 targetPos)
        {
            try
            {
                const int pointCount = 32;

                if (Line == null)
                {
                    GameObject lineObj = new GameObject("Line");
                    Line = lineObj.AddComponent<LineRenderer>();
                    if (sharedLineMaterial == null) sharedLineMaterial = new Material(Shader.Find(GunSettings.ShaderName));
                    Line.material = sharedLineMaterial;
                    Line.startWidth = GunSettings.LineWidth;
                    Line.endWidth = GunSettings.LineWidth;
                    Line.positionCount = pointCount;
                    Line.useWorldSpace = true;
                    Line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    Line.receiveShadows = false;
                }

                Color lineColor;
                switch (GunSettings.LineColorMode)
                {
                    case LineColorMode.Normal:
                        lineColor = hand1 ? Color.green : Color.red;
                        break;
                    case LineColorMode.Red:
                        lineColor = Color.red;
                        break;
                    case LineColorMode.Green:
                        lineColor = Color.green;
                        break;
                    case LineColorMode.Blue:
                        lineColor = Color.blue;
                        break;
                    case LineColorMode.Yellow:
                        lineColor = Color.yellow;
                        break;
                    case LineColorMode.White:
                        lineColor = Color.white;
                        break;
                    case LineColorMode.Magenta:
                        lineColor = Color.magenta;
                        break;
                    case LineColorMode.Custom:
                        lineColor = GunSettings.LineColor;
                        break;
                    default:
                        lineColor = GunSettings.LineColor;
                        break;
                }

                Line.startColor = lineColor;
                Line.endColor = lineColor;

                if (currentLineEnd == Vector3.zero)
                    currentLineEnd = targetPos;

                Vector3 velocity = Vector3.zero;
                if (Time.deltaTime > 0f)
                    velocity = (targetPos - currentLineEnd) / Time.deltaTime;
                float speed = velocity.magnitude;

                if (GunSettings.EnableLineAnimation && Time.deltaTime > 0f)
                    currentLineEnd = Vector3.Lerp(currentLineEnd, targetPos, Time.deltaTime * 12f);
                else
                    currentLineEnd = targetPos;

                bool shouldBend = speed > 0.05f;
                Vector3 bendDir = shouldBend ? -velocity.normalized : Vector3.zero;
                float bendStrength = shouldBend ? Mathf.Clamp(speed * 0.01f, 0f, 0.4f) : 0f;

                Vector3 start = arm.position;
                Vector3 end = targetPos;

                for (int i = 0; i < pointCount; i++)
                {
                    float t = i / (float)(pointCount - 1);
                    float smoothT = Mathf.SmoothStep(0f, 1f, t);
                    Vector3 baseLine = Vector3.Lerp(start, end, smoothT);
                    float sineOffset = Mathf.Sin(smoothT * Mathf.PI);
                    Vector3 offset = bendDir * bendStrength * sineOffset;
                    Line.SetPosition(i, baseLine + offset);
                }

                Line.startWidth = Line.endWidth = GunSettings.LineWidth;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GKongGunTemp] Error updating line renderer: {ex}");
            }
        }

        private static void Cleanup()
        {
            try
            {
                LockedRig = null;
                IsLocking = false;

                if (pointer != null)
                {
                    Destroy(pointer);
                    pointer = null;
                    currentPointerPos = Vector3.zero;
                }

                if (Line != null)
                {
                    Destroy(Line.gameObject);
                    Line = null;
                    currentLineEnd = Vector3.zero;
                }

                CleanupParticles();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GKongGunTemp] Cleanup failed: {ex}");
            }
        }

        public static void ExampleOnHowToUseGunLib()
        {
            MakeGun(false, () =>
            {
                try
                {
                    var bug = GameObject.Find("Floating Bug Holdable");
                    if (bug != null && pointer != null)
                        bug.transform.position = pointer.transform.position;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[GKongGunTemp] Example callback failed: {ex}");
                }
            });
        }

        public static int InvisLayerMaskFix()
        {
            return ~(1 << TransparentFX | 1 << IgnoreRaycast | 1 << Zone |
                     1 << GorillaTrigger | 1 << GorillaBoundary | 1 << GorillaCosmetics | 1 << GorillaParticle);
        }

        public static readonly int TransparentFX = LayerMask.NameToLayer("TransparentFX");
        public static readonly int IgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
        public static readonly int Zone = LayerMask.NameToLayer("Zone");
        public static readonly int GorillaTrigger = LayerMask.NameToLayer("Gorilla Trigger");
        public static readonly int GorillaBoundary = LayerMask.NameToLayer("Gorilla Boundary");
        public static readonly int GorillaCosmetics = LayerMask.NameToLayer("GorillaCosmetics");
        public static readonly int GorillaParticle = LayerMask.NameToLayer("GorillaParticle");

        private static void CleanupParticles()
        {
            if (particles != null)
            {
                for (int i = 0; i < particles.Length; i++)
                {
                    if (particles[i] != null && particles[i].go != null)
                        UnityEngine.Object.Destroy(particles[i].go);
                    particles[i] = null;
                }
                particles = null;
            }
            particlePhase = ParticlePhase.Idle;
        }

        private static bool AllParticlesAtTarget(Vector3 target)
        {
            if (particles == null) return true;
            for (int i = 0; i < particles.Length; i++)
            {
                var p = particles[i];
                if (p != null && p.go != null && p.go.activeSelf && Vector3.Distance(p.go.transform.position, target) > particleThreshold)
                    return false;
            }
            return true;
        }

        private static void ResetParticles(Vector3 gunPos, Transform arm)
        {
            if (particles == null) return;

            Vector3 startPos = arm.position;
            Vector3 endPos = startPos + arm.forward * 1f;
            Vector3 dir = endPos - startPos;
            float len = Mathf.Max(0.0001f, dir.magnitude);
            Vector3 dirNorm = dir / len;
            Vector3 arbitrary = Vector3.up;
            if (Mathf.Abs(Vector3.Dot(arbitrary, dirNorm)) > 0.9f) arbitrary = Vector3.forward;
            Vector3 right = Vector3.Cross(dirNorm, arbitrary).normalized;
            Vector3 up = Vector3.Cross(right, dirNorm).normalized;

            System.Random rnd = new System.Random();
            for (int i = 0; i < particles.Length; i++)
            {
                var st = particles[i];
                if (st == null || st.go == null) continue;
                st.along = (float)rnd.NextDouble();
                st.angle = (float)(i * Mathf.PI * 2f / particles.Length) + (float)rnd.NextDouble() * 0.5f;
                st.radius = particleTubeRadius * (0.6f + (float)rnd.NextDouble() * 0.9f);
                Vector3 tubePos = startPos + dirNorm * (st.along * len);
                Vector3 radial = (Mathf.Cos(st.angle) * right + Mathf.Sin(st.angle) * up) * st.radius;
                st.t.position = tubePos + radial;
                st.go.SetActive(true);
                st.active = true;
            }
        }
        internal enum PointerColorMode
        {
            Normal,
            Red,
            Green,
            Blue,
            Yellow,
            White,
            Magenta,
            Custom
        }

        internal enum LineColorMode
        {
            Normal,
            Red,
            Green,
            Blue,
            Yellow,
            White,
            Magenta,
            Custom
        }

        internal static class GunSettings
        {
            public static PointerColorMode PointerColorMode { get; set; } = PointerColorMode.Normal;
            public static LineColorMode LineColorMode { get; set; } = LineColorMode.Normal;
            public static Color PointerColor { get; set; } = Color.blue;
            public static Color LineColor { get; set; } = Color.blue;
            public static float PointerScale { get; set; } = 0.07f;
            public static float LineWidth { get; set; } = 0.02f;
            public static bool EnablePointerAnimation { get; set; } = true;
            public static bool EnableLineAnimation { get; set; } = true;

            public static bool GunOrbTrail { get; set; } = true;
            public static string ShaderName { get; set; } = "GUI/Text Shader";

            public static void SetPointerColorMode(PointerColorMode mode)
            {
                PointerColorMode = mode;
            }

            public static void SetLineColorMode(LineColorMode mode)
            {
                LineColorMode = mode;
            }

            public static void SetPointerColor(Color color)
            {
                PointerColor = color;
                PointerColorMode = PointerColorMode.Custom;
            }

            public static void SetLineColor(Color color)
            {
                LineColor = color;
                LineColorMode = LineColorMode.Custom;
            }

            public static void SetPointerScale(float scale)
            {
                PointerScale = Mathf.Clamp(scale, 0.01f, 1f);
            }

            public static void SetLineWidth(float width)
            {
                LineWidth = Mathf.Clamp(width, 0.001f, 0.5f);
            }

            public static void SetPointerAnimation(bool enabled)
            {
                EnablePointerAnimation = enabled;
            }

            public static void SetLineAnimation(bool enabled)
            {
                EnableLineAnimation = enabled;
            }

            public static void SetShader(string shaderName)
            {
                ShaderName = shaderName;
            }

            public static void BigPointer() => PointerScale = 0.15f;
            public static void NormalPointer() => PointerScale = 0.07f;
            public static void SmallPointer() => PointerScale = 0.03f;

            public static void PointerRed() { PointerColor = Color.red; PointerColorMode = PointerColorMode.Red; }
            public static void PointerGreen() { PointerColor = Color.green; PointerColorMode = PointerColorMode.Green; }
            public static void PointerBlue() { PointerColor = Color.blue; PointerColorMode = PointerColorMode.Blue; }
            public static void PointerYellow() { PointerColor = Color.yellow; PointerColorMode = PointerColorMode.Yellow; }
            public static void PointerWhite() { PointerColor = Color.white; PointerColorMode = PointerColorMode.White; }
            public static void PointerMagenta() { PointerColor = Color.magenta; PointerColorMode = PointerColorMode.Magenta; }
            public static void PointerNormal() { PointerColorMode = PointerColorMode.Normal; }

            public static void LineRed() { LineColor = Color.red; LineColorMode = LineColorMode.Red; }
            public static void LineGreen() { LineColor = Color.green; LineColorMode = LineColorMode.Green; }
            public static void LineBlue() { LineColor = Color.blue; LineColorMode = LineColorMode.Blue; }
            public static void LineYellow() { LineColor = Color.yellow; LineColorMode = LineColorMode.Yellow; }
            public static void LineWhite() { LineColor = Color.white; LineColorMode = LineColorMode.White; }
            public static void LineMagenta() { LineColor = Color.magenta; LineColorMode = LineColorMode.Magenta; }
            public static void LineNormal() { LineColorMode = LineColorMode.Normal; }

            public static void ThinLine() => LineWidth = 0.01f;
            public static void NormalLine() => LineWidth = 0.02f;
            public static void ThickLine() => LineWidth = 0.05f;

            public static void EnableAllAnimations()
            {
                EnablePointerAnimation = true;
                EnableLineAnimation = true;
            }
            public static void DisableAllAnimations()
            {
                EnablePointerAnimation = false;
                EnableLineAnimation = false;
            }
            public static void OnlyPointerAnimation()
            {
                EnablePointerAnimation = true;
                EnableLineAnimation = false;
            }
            public static void OnlyLineAnimation()
            {
                EnablePointerAnimation = false;
                EnableLineAnimation = true;
            }

            public static void UseDefaultShader() => ShaderName = "GUI/Text Shader";
            public static void UseUnlitColorShader() => ShaderName = "Unlit/Color";
            public static void UseStandardShader() => ShaderName = "Standard";
        }
    }
}