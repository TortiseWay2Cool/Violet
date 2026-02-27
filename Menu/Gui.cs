using BepInEx;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using Violet.Menu;

namespace Violet.GUI
{
    class SwishButton
    {
        public string text;
        public float startTime;
        public bool isEntering;
        public const float AnimationDuration = 0.5f;
        public float targetY;
        public float currentY;

        public SwishButton(string text, bool entering)
        {
            this.text = text;
            this.isEntering = entering;
            this.startTime = Time.time;
            this.targetY = 0f;
            this.currentY = 0f;
        }

        public float AnimationProgress => Mathf.Clamp01((Time.time - startTime) / AnimationDuration);
        public bool IsDone => (Time.time - startTime) > AnimationDuration;
    }

    [BepInPlugin("Violet.GUI", "VioletGUI", "1.0.0")]
    public class VioletGUI : BaseUnityPlugin
    {
        public static bool guiEnabled = true;


        private const int GradientHeight = 600;
        private const float WiggleAmplitude = 10f;
        private const float WiggleSpeed = 1f;
        private const float DisplayDuration = 2f;
        private const float MoveDuration = 1.5f;
        private const float LineAnimSpeed = 300f;
        private const string ImageUrl = "https://i.ibb.co/Q35CRZpP/image-Photoroom-1.png";

        private Texture2D rgbGradientTex;
        private static Texture2D downloadedImage;
        private Texture2D backgroundTexture;
        private Texture2D cardTexture;
        private readonly Dictionary<int, Texture2D> roundedTextureCache = new Dictionary<int, Texture2D>();

        private GUIStyle playerBoxStyle;
        private GUIStyle windowStyle;
        private GUIStyle cardStyle;
        private GUIStyle labelStyle;
        private bool stylesInitialized = false;

        private float gradientHue = 0f;
        private static bool showStartMenu = false;
        private static bool isMovingToFinalPosition = false;
        private static float imageDisplayStartTime;
        private static float moveProgress = 0f;
        private static Rect bigImageRect;
        private static Rect finalImageRect;

        private readonly Dictionary<string, SwishButton> activeSwishButtons = new Dictionary<string, SwishButton>();
        private readonly List<string> visibleFinalButtons = new List<string>();
        private float currentLineTopY = -1f;
        private float currentLineBottomY = -1f;
        private float lineX = -1f;
        private float overlayAlpha = 0f;

        private float lineExtensionCompletedTime = -1f;
        private float lineFullyExtendedSince = -1f;
        private string animatingButton = null;

        private void Start()
        {
            Debug.Log("GUI initialized!");
            StartCoroutine(DownloadImages());

            backgroundTexture = CreateRoundedTexture(128, 128, 24, new Color(0.1f, 0.1f, 0.1f, 0.85f));
            cardTexture = CreateRoundedTexture(128, 128, 20, new Color(0.2f, 0.2f, 0.2f, 0.95f));
        }

        private IEnumerator DownloadImages()
        {
            // download logo
            yield return DownloadTexture(ImageUrl, result => {
                downloadedImage = result;
                // recompute start menu rects if start menu is active so sizes use the real image
                if (showStartMenu)
                {
                    StartMenu();
                }
            });
        }

        private IEnumerator DownloadTexture(string url, Action<Texture2D> onSuccess)
        {
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
            {
                yield return request.SendWebRequest();


                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Download error for {url}: {request.error}");
                }
                else
                {
                    var texture = DownloadHandlerTexture.GetContent(request);
                    if (texture != null)
                    {
                        Debug.Log($"Downloaded {url}: {texture.width}x{texture.height}");
                        onSuccess?.Invoke(texture);
                    }
                }
            }
        }

        private void Update()
        {
            UpdateRGBGradientTexture();

            if (Keyboard.current != null && Keyboard.current.f8Key.wasPressedThisFrame)
            {
                guiEnabled = !guiEnabled;

                if (guiEnabled)
                {
                    StartMenu();
                }
            }
        }



        private void UpdateRGBGradientTexture()
        {
            if (rgbGradientTex == null || rgbGradientTex.height != GradientHeight)
            {
                rgbGradientTex = new Texture2D(1, GradientHeight)
                {
                    wrapMode = TextureWrapMode.Clamp,
                    filterMode = FilterMode.Bilinear
                };
            }

            for (int y = 0; y < GradientHeight; y++)
            {
                float t = (float)y / GradientHeight;
                float localHue = (gradientHue + t * 0.2f) % 1f;
                rgbGradientTex.SetPixel(0, y, Color.HSVToRGB(localHue, 0.8f, 1f));
            }

            rgbGradientTex.Apply();
            gradientHue = (gradientHue + Time.deltaTime * 0.04f) % 1f;
        }

        public static void StartMenu()
        {
            showStartMenu = true;
            isMovingToFinalPosition = false;
            moveProgress = 0f;
            imageDisplayStartTime = Time.time;

            const float bigScale = 1f;
            const float smallScale = 0.4f;

            if (downloadedImage != null)
            {
                float bigWidth = downloadedImage.width * bigScale;
                float bigHeight = downloadedImage.height * bigScale;
                float centerX = (Screen.width - bigWidth) / 2f;
                float centerY = (Screen.height - bigHeight) / 2f;
                bigImageRect = new Rect(centerX, centerY, bigWidth, bigHeight);

                float imgWidth = downloadedImage.width * smallScale;
                float imgHeight = downloadedImage.height * smallScale;
                float x = Screen.width - imgWidth - 10f;
                float y = 10f;
                finalImageRect = new Rect(x, y, imgWidth, imgHeight);
            }
            else
            {
                float fallbackBig = Mathf.Min(Screen.width, Screen.height) * 0.4f;
                float centerX = (Screen.width - fallbackBig) / 2f;
                float centerY = (Screen.height - fallbackBig) / 2f;
                bigImageRect = new Rect(centerX, centerY, fallbackBig, fallbackBig);

                float fallbackSmall = Mathf.Min(Screen.width, Screen.height) * 0.12f;
                float x = Screen.width - fallbackSmall - 10f;
                float y = 10f;
                finalImageRect = new Rect(x, y, fallbackSmall, fallbackSmall);
            }
        }

        private void OnGUI()
        {
            if (!guiEnabled) return;

            UpdateButtonStates();
            DrawGUIElements();
        }


        private void UpdateButtonStates()
        {
            IOrderedEnumerable<ButtonHandler.Button> orderedButtons = from b in ModButtons.buttons
                                                                      orderby b.buttonText.Length descending
                                                                      select b;
            var currentEnabled = new HashSet<string>();
            foreach (ButtonHandler.Button button in orderedButtons)
            {
                if (button.Enabled)
                {
                    currentEnabled.Add(button.buttonText);
                    if (!activeSwishButtons.ContainsKey(button.buttonText))
                    {
                        activeSwishButtons[button.buttonText] = new SwishButton(button.buttonText, true);
                    }
                }
            }

            var toRemove = new List<string>();
            foreach (var kvp in activeSwishButtons)
            {
                if (!currentEnabled.Contains(kvp.Key))
                {
                    if (kvp.Value.isEntering)
                    {
                        kvp.Value.startTime = Time.time;
                        kvp.Value.isEntering = false;
                    }
                    else if (kvp.Value.IsDone)
                    {
                        toRemove.Add(kvp.Key);
                    }
                }
            }

            foreach (var key in toRemove)
            {
                activeSwishButtons.Remove(key);
            }
        }

        private void DrawGUIElements()
        {
            if (!stylesInitialized)
            {
                InitGUIStyles();
                stylesInitialized = true;
            }

            var guistyle = new GUIStyle(UnityEngine.GUI.skin.label)
            {
                font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf"),
                alignment = TextAnchor.MiddleLeft,
                fontSize = 16,
                wordWrap = false,
                richText = false,
                clipping = TextClipping.Clip
            };

            var boxStyle = new GUIStyle(UnityEngine.GUI.skin.box)
            {
                normal = { background = MakeTex(2, 2, new Color(0.2f, 0.2f, 0.2f, 0.5f)) },
                border = new RectOffset(0, 0, 0, 0)
            };

            DrawButtons(guistyle, boxStyle);
            DrawImage();
        }

        private void DrawButtons(GUIStyle guistyle, GUIStyle boxStyle)
        {
            visibleFinalButtons.RemoveAll(b => !activeSwishButtons.ContainsKey(b));

            if (animatingButton != null && !activeSwishButtons.ContainsKey(animatingButton))
            {
                animatingButton = null;
            }

            float yOffset = finalImageRect.y + finalImageRect.height + 10f;
            float y = yOffset;
            float maxRightX = 0f;
            float targetLineTopY = -1f;
            float targetLineBottomY = -1f;

            var sortedButtons = new List<SwishButton>(activeSwishButtons.Values);
            sortedButtons.Sort((a, b) => b.text.Length.CompareTo(a.text.Length));

            var measured = new List<(SwishButton sw, float textWidth, float textHeight, float targetX, float boxWidth, float boxHeight)>();
            foreach (var sw in sortedButtons)
            {
                Vector2 size = guistyle.CalcSize(new GUIContent(sw.text));
                float textHeight = size.y;
                float textWidth = size.x;
                float boxPaddingX = 22f;
                float boxPaddingY = 10f;
                float boxWidth = textWidth + boxPaddingX;
                float boxHeight = textHeight + boxPaddingY;
                float targetX = Screen.width - boxWidth - 10f;

                sw.targetY = y;

                if (targetLineTopY < 0f) targetLineTopY = sw.targetY;
                targetLineBottomY = sw.targetY + boxHeight;
                maxRightX = Mathf.Max(maxRightX, Screen.width - 10f);

                measured.Add((sw, textWidth, textHeight, targetX, boxWidth, boxHeight));

                y += boxHeight + 5f;
            }

            DrawLine(maxRightX, targetLineTopY, targetLineBottomY);

            float eps = 0.5f;
            bool lineFullyExtended = Mathf.Abs(lineX - maxRightX) < eps
                                     && Mathf.Abs(currentLineTopY - targetLineTopY) < eps
                                     && Mathf.Abs(currentLineBottomY - targetLineBottomY) < eps;

            if (lineFullyExtended)
            {
                if (lineFullyExtendedSince < 0f) lineFullyExtendedSince = Time.time;
            }
            else
            {
                lineFullyExtendedSince = -1f;
            }

            int nextIndex = -1;
            for (int i = 0; i < measured.Count; i++)
            {
                var sw = measured[i].sw;
                if (sw.isEntering && !visibleFinalButtons.Contains(sw.text))
                {
                    nextIndex = i;
                    break;
                }
            }

            if (lineFullyExtendedSince >= 0f && nextIndex >= 0 && animatingButton == null)
            {
                float dwell = 0.06f;
                if (Time.time - lineFullyExtendedSince >= dwell)
                {
                    var sw = measured[nextIndex].sw;
                    animatingButton = sw.text;
                    sw.startTime = Time.time; // start animation now

                    if (!visibleFinalButtons.Contains(sw.text))
                        visibleFinalButtons.Add(sw.text);
                }
            }

            for (int i = 0; i < measured.Count; i++)
            {
                var entry = measured[i];
                var sw = entry.sw;
                float textWidth = entry.textWidth;
                float textHeight = entry.textHeight;
                float targetX = entry.targetX;
                float boxWidth = entry.boxWidth;
                float boxHeight = entry.boxHeight;

                sw.currentY = Mathf.Lerp(sw.currentY, sw.targetY, 12f * Time.deltaTime);

                if (!sw.isEntering)
                {
                    float progress = sw.AnimationProgress;
                    float eased = EaseOutCubic(progress);
                    float drawWidth = Mathf.Lerp(boxWidth, 0f, eased);
                    float drawX = lineX - drawWidth;

                    if (drawWidth > 1f)
                    {
                        UnityEngine.GUI.BeginGroup(new Rect(drawX, sw.currentY, drawWidth, boxHeight));
                        float labelOffset = targetX - drawX;
                        UnityEngine.GUI.Box(new Rect(labelOffset, 0f, boxWidth, boxHeight), "", boxStyle);
                        var textRect = new Rect(labelOffset + boxWidth / 2f - textWidth / 2f, boxHeight / 2f - textHeight / 2f, textWidth, textHeight);
                        guistyle.clipping = TextClipping.Clip;
                        UnityEngine.GUI.Label(textRect, sw.text, guistyle);
                        UnityEngine.GUI.EndGroup();
                    }
                    continue;
                }

                if (visibleFinalButtons.Contains(sw.text))
                {
                    if (animatingButton == sw.text && sw.isEntering)
                    {
                        Color prev = UnityEngine.GUI.color;
                        UnityEngine.GUI.color = new Color(prev.r, prev.g, prev.b, 0f);
                        UnityEngine.GUI.Box(new Rect(targetX, sw.currentY, boxWidth, boxHeight), "", boxStyle);
                        UnityEngine.GUI.color = prev;
                    }
                    else
                    {
                        UnityEngine.GUI.Box(new Rect(targetX, sw.currentY, boxWidth, boxHeight), "", boxStyle);
                        var textRect = new Rect(targetX + boxWidth / 2f - textWidth / 2f, sw.currentY + boxHeight / 2f - textHeight / 2f, textWidth, textHeight);
                        guistyle.clipping = TextClipping.Clip;
                        UnityEngine.GUI.Label(textRect, sw.text, guistyle);
                        continue;
                    }
                }

                if (animatingButton == sw.text)
                {
                    float progress = sw.AnimationProgress;
                    float eased = EaseOutCubic(progress);
                    float drawWidth = Mathf.Lerp(0f, boxWidth, eased);
                    float drawX = lineX - drawWidth;
                    if (drawWidth > 1f)
                    {
                        Color prev = UnityEngine.GUI.color;
                        UnityEngine.GUI.color = new Color(prev.r, prev.g, prev.b, prev.a * Mathf.Clamp01(eased * 1.2f));
                        UnityEngine.GUI.BeginGroup(new Rect(drawX, sw.currentY, drawWidth, boxHeight));
                        float internalX = drawWidth - boxWidth;
                        UnityEngine.GUI.Box(new Rect(internalX, 0f, boxWidth, boxHeight), "", boxStyle);
                        var textRect = new Rect(internalX + boxWidth / 2f - textWidth / 2f, boxHeight / 2f - textHeight / 2f, textWidth, textHeight);
                        guistyle.clipping = TextClipping.Clip;
                        UnityEngine.GUI.Label(textRect, sw.text, guistyle);
                        UnityEngine.GUI.EndGroup();
                        UnityEngine.GUI.color = prev;
                    }
                    if (progress >= 1f)
                    {
                        if (!visibleFinalButtons.Contains(sw.text)) visibleFinalButtons.Add(sw.text);
                        animatingButton = null;
                    }
                    continue;
                }
            }

            if (activeSwishButtons.Count == 0 && visibleFinalButtons.Count == 0)
            {
                lineFullyExtendedSince = -1f;
                animatingButton = null;
            }

            if (animatingButton == null && activeSwishButtons.Count == 0)
            {
                visibleFinalButtons.Clear();
            }
        }

        private void DrawLine(float targetLineX, float targetLineTopY, float targetLineBottomY)
        {
            if (activeSwishButtons.Count == 0 && visibleFinalButtons.Count == 0) return;

            if (currentLineTopY < 0f || currentLineBottomY < 0f)
            {
                float mid = (targetLineTopY + targetLineBottomY) / 2f;
                currentLineTopY = currentLineBottomY = mid;
                lineX = targetLineX;
            }

            currentLineTopY = Mathf.MoveTowards(currentLineTopY, targetLineTopY, LineAnimSpeed * Time.deltaTime);
            currentLineBottomY = Mathf.MoveTowards(currentLineBottomY, targetLineBottomY, LineAnimSpeed * Time.deltaTime);
            lineX = Mathf.MoveTowards(lineX, targetLineX, LineAnimSpeed * Time.deltaTime);

            float height = currentLineBottomY - currentLineTopY;
            if (height > 0 && rgbGradientTex != null)
            {
                UnityEngine.GUI.DrawTexture(new Rect(lineX, currentLineTopY, 2f, height), rgbGradientTex);
            }
        }

        private void DrawImage()
        {
            Texture2D imageToDraw = downloadedImage ?? Texture2D.whiteTexture;

            float fadeProgress = 0f;
            if (showStartMenu)
            {
                if (!isMovingToFinalPosition)
                {
                    fadeProgress = 0f;
                }
                else
                {
                    fadeProgress = moveProgress;
                }
                overlayAlpha = 1f - Mathf.Clamp01(fadeProgress);
            }
            else
            {
                overlayAlpha = 0f;
            }

            if (overlayAlpha > 0f)
            {
                Color prevColor = UnityEngine.GUI.color;
                UnityEngine.GUI.color = new Color(0f, 0f, 0f, overlayAlpha);
                UnityEngine.GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
                UnityEngine.GUI.color = prevColor;
            }

            if (showStartMenu)
            {
                if (!isMovingToFinalPosition)
                {
                    UnityEngine.GUI.DrawTexture(bigImageRect, imageToDraw, ScaleMode.ScaleToFit, true);
                    if (Time.time - imageDisplayStartTime > DisplayDuration)
                    {
                        isMovingToFinalPosition = true;
                        moveProgress = 0f;
                        imageDisplayStartTime = Time.time;
                    }
                }
                else
                {
                    moveProgress = Mathf.Clamp01(moveProgress + Time.deltaTime / MoveDuration);
                    if (moveProgress >= 1f) showStartMenu = false;

                    float x = Mathf.Lerp(bigImageRect.x, finalImageRect.x, moveProgress);
                    float y = Mathf.Lerp(bigImageRect.y, finalImageRect.y, moveProgress);
                    float width = Mathf.Lerp(bigImageRect.width, finalImageRect.width, moveProgress);
                    float height = Mathf.Lerp(bigImageRect.height, finalImageRect.height, moveProgress);

                    UnityEngine.GUI.DrawTexture(new Rect(x, y, width, height), imageToDraw, ScaleMode.ScaleToFit, true);
                }
            }
            else
            {
                float wiggleOffset = Mathf.Sin(Time.time * WiggleSpeed) * WiggleAmplitude;
                var wiggleRect = new Rect(finalImageRect.x + wiggleOffset, finalImageRect.y, finalImageRect.width, finalImageRect.height);
                UnityEngine.GUI.DrawTexture(wiggleRect, imageToDraw, ScaleMode.ScaleToFit, true);
            }
        }

        private void InitGUIStyles()
        {
            windowStyle = new GUIStyle(UnityEngine.GUI.skin.window)
            {
                normal = { background = backgroundTexture, textColor = Color.white },
                padding = new RectOffset(16, 16, 40, 16),
                fontSize = 16,
                alignment = TextAnchor.UpperLeft,
                border = new RectOffset(24, 24, 24, 24)
            };

            cardStyle = new GUIStyle(UnityEngine.GUI.skin.box)
            {
                normal = { background = cardTexture, textColor = Color.white },
                margin = new RectOffset(8, 8, 8, 8),
                padding = new RectOffset(14, 14, 14, 14),
                fontSize = 15,
                border = new RectOffset(20, 20, 20, 20)
            };

            labelStyle = new GUIStyle(UnityEngine.GUI.skin.label)
            {
                fontSize = 14,
                richText = true,
                normal = { textColor = new Color(0.95f, 0.95f, 0.95f, 1f) }
            };

            playerBoxStyle = new GUIStyle(UnityEngine.GUI.skin.box)
            {
                normal = { background = backgroundTexture, textColor = Color.white },
                margin = new RectOffset(2, 2, 2, 2),
                padding = new RectOffset(8, 8, 4, 4),
                fontSize = 14,
                border = new RectOffset(20, 20, 20, 20)
            };
        }

        private Texture2D GetCachedRoundedTexture(int radius)
        {
            if (!roundedTextureCache.TryGetValue(radius, out var tex))
            {
                tex = CreateRoundedTexture(128, 128, radius, new Color(0.18f, 0.18f, 0.18f, 1f));
                roundedTextureCache[radius] = tex;
            }
            return tex;
        }

        private static Texture2D CreateRoundedTexture(int width, int height, int radius, Color color)
        {
            int maxRadius = Mathf.Min(width, height) / 2;
            radius = Mathf.Clamp(radius, 0, maxRadius);

            var tex = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Bilinear
            };

            Color transparent = new Color(0, 0, 0, 0);
            float radiusSquared = radius * radius;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool isCorner = false;

                    // Topleft corner
                    if (x < radius && y < radius)
                    {
                        float dx = radius - x;
                        float dy = radius - y;
                        isCorner = (dx * dx + dy * dy) > radiusSquared;
                    }
                    // Topright corner
                    else if (x >= width - radius && y < radius)
                    {
                        float dx = x - (width - radius - 1);
                        float dy = radius - y;
                        isCorner = (dx * dx + dy * dy) > radiusSquared;
                    }
                    // Bottomleft corner
                    else if (x < radius && y >= height - radius)
                    {
                        float dx = radius - x;
                        float dy = y - (height - radius - 1);
                        isCorner = (dx * dx + dy * dy) > radiusSquared;
                    }
                    // Bottomright corner
                    else if (x >= width - radius && y >= height - radius)
                    {
                        float dx = x - (width - radius - 1);
                        float dy = y - (height - radius - 1);
                        isCorner = (dx * dx + dy * dy) > radiusSquared;
                    }

                    tex.SetPixel(x, y, isCorner ? transparent : color);
                }
            }

            tex.Apply();
            return tex;
        }

        private static Texture2D MakeTex(int width, int height, Color col)
        {
            var result = new Texture2D(width, height);
            var colors = new Color[width * height];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = col;

            result.SetPixels(colors);
            result.Apply();
            return result;
        }

        private static float EaseOutCubic(float t) => 1f - Mathf.Pow(1f - t, 3f);
    }
}