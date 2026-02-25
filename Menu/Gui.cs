using BepInEx;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Violet.Menu;
using Violet.Utilities;

namespace VioletPaid.Menu
{
    internal class Gui : MonoBehaviour
    {
        public void Update()
        {
            if (UnityInput.Current.GetKeyDown(KeyCode.Insert))
            {
                ArrayListShown = !ArrayListShown;
            }
        }

        public void OnGUI()
        {
            buttonTexture = CreateTexture(buttonColor);
            buttonHoverTexture = CreateTexture(buttonHoverColor);
            buttonClickTexture = CreateTexture(buttonEnabledColor);
            arrayListTexture = CreateTexture(Variables.ButtonColorOff, 17, 17);

            GUIStyle labelStyle = new GUIStyle(GUI.skin.box)
            {
                fontSize = 32,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = false,
                normal = { background = arrayListTexture, textColor = colorMaterial.color }
            };


            if (ArrayListShown)
            {
                IOrderedEnumerable<ButtonHandler.Button> orderedButtons = from b in ModButtons.buttons
                                                                          orderby b.buttonText.Length descending
                                                                          select b;
                GUIStyle buttonLabelStyle = new GUIStyle(GUI.skin.box)
                {
                    fontSize = 25,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter,
                    wordWrap = false,
                    normal = { background = arrayListTexture, textColor = Color.white }
                };

                foreach (ButtonHandler.Button button in orderedButtons)
                {
                    if (button.Enabled)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(5.5f);
                        GUILayout.BeginVertical();
                        GUILayout.Space(5.5f);
                        GUILayout.Label(
                            button.buttonText,
                            buttonLabelStyle,
                            GUILayout.Width(buttonLabelStyle.CalcSize(new GUIContent(button.buttonText)).x + 23.5f),
                            GUILayout.Height(33.5f)
                        );
                        GUILayout.EndVertical();
                        GUILayout.EndHorizontal();
                    }
                }
            }
        }

        public static Texture2D CreateTexture(Color color, int width = 30, int height = 30)
        {
            Texture2D texture = new Texture2D(width, height);
            Color[] pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }

        public static bool ArrayListShown = true;
        public static string NameOfMenu = "Violet";
        public static string VersionOfMenu = string.Format("{0}", "1.0"[0]);
        public Color32 buttonColor = ColorLib.DarkGrey;
        public Color32 buttonHoverColor = new Color32(48, 48, 48, byte.MaxValue);
        public Color32 buttonEnabledColor = new Color32(35, 35, 35, byte.MaxValue);
        public static Texture2D buttonTexture = new Texture2D(2, 2);
        public static Texture2D buttonHoverTexture = new Texture2D(2, 2);
        public static Texture2D buttonClickTexture = new Texture2D(2, 2);
        public static Texture2D arrayListTexture = new Texture2D(2, 2);
        public static Material colorMaterial = new Material(Shader.Find("GUI/Text Shader"))
        {
            color = Color.Lerp(Color.gray * 1.3f, Color.grey * 1.6f, Mathf.PingPong(Time.time, 1.5f))
        };
    }
}