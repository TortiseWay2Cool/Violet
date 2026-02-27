using GorillaNetworking;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Violet.Menu.Utilities
{
    internal class CustomBoards : MonoBehaviour
    {
        public const int StumpLeaderboardIndex = 3;
        public const int ForestLeaderboardIndex = 3;

        private static Material boardMaterial;

        private static readonly Color purpleA = new Color32(70, 35, 110, 255);
        private static readonly Color purpleB = new Color32(95, 50, 140, 255);
        private static readonly Color purpleC = new Color32(55, 25, 90, 255);

        private static readonly Color[] colors = { purpleA, purpleB, purpleC };

        private static int currentIndex = 0;
        private static int nextIndex = 1;

        private static float lerpTime = 0f;
        private static float lerpSpeed = 0.35f;

        private static GameObject motdTitle;
        private static GameObject motdText;
        private static List<TextMeshPro> textMeshPro = new List<TextMeshPro>();
        private static string motdTemplate = "Welcome to Violet! \nThis Menu is being developed by Tortise and RaiseEvent200! \nThis Menu is 100% Free and Open Source. \nJoin the Discord for Updates!";

        public static void Init()
        {
            var shader = Shader.Find("GorillaTag/UberShader");
            if (shader == null)
            {
                Debug.LogError("CustomBoards.Init: Shader 'GorillaTag/UberShader' not found (null)");
                return;
            }

            boardMaterial = new Material(shader);
            boardMaterial.color = colors.Length > 0 ? colors[0] : Color.magenta;

            ApplyMaterialOnce(boardMaterial);
            ApplyBoardText();
        }

        public static void Update()
        {
            if (boardMaterial == null)
            {
                return;
            }

            if (colors == null || colors.Length == 0)
            {
                return;
            }

            lerpTime += Time.deltaTime * lerpSpeed;
            boardMaterial.color = Color.Lerp(
                colors[currentIndex],
                colors[nextIndex],
                lerpTime
            );

            if (lerpTime >= 1f)
            {
                lerpTime = 0f;
                currentIndex = nextIndex;
                nextIndex = (nextIndex + 1) % colors.Length;
            }
        }

        public static void ApplyBoardText()
        {
            try
            {
                if (motdTitle == null)
                {
                    GameObject originalHeading = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/motdHeadingText");
                    if (originalHeading != null)
                    {
                        motdTitle = Object.Instantiate(originalHeading, originalHeading.transform.parent);
                        originalHeading.SetActive(false);
                    }
                    else
                    {
                        Debug.LogWarning("CustomBoards.ApplyBoardText: originalHeading GameObject not found (motdHeadingText)");
                    }
                }

                if (motdText == null)
                {
                    GameObject originalBody = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/motdBodyText");
                    if (originalBody != null)
                    {
                        motdText = Object.Instantiate(originalBody, originalBody.transform.parent);
                        originalBody.SetActive(false);

                        Component[] comps = motdText.GetComponents(typeof(Component));
                        foreach (var c in comps)
                        {
                            if (c == null) continue;
                            if (c.GetType().Name == "PlayFabTitleDataTextDisplay")
                            {
                                if (c is Behaviour b)
                                    b.enabled = false;
                                break;
                            }
                        }

                        motdText.SetActive(true);
                    }
                    else
                    {
                        Debug.LogWarning("CustomBoards.ApplyBoardText: originalBody GameObject not found (motdBodyText)");
                    }
                }

                if (motdTitle != null)
                {
                    TextMeshPro heading = motdTitle.GetComponent<TextMeshPro>();
                    if (heading != null)
                    {
                        heading.text = "VIOLET";

                        if (!textMeshPro.Contains(heading))
                            textMeshPro.Add(heading);
                    }
                    else
                    {
                        Debug.LogWarning("CustomBoards.ApplyBoardText: motdTitle exists but GetComponent<TextMeshPro>() returned null");
                    }
                }

                if (motdText != null)
                {
                    TextMeshPro body = motdText.GetComponent<TextMeshPro>();
                    if (body != null)
                    {
                        string formatted = string.Format(motdTemplate);
                        body.text = formatted;

                        try
                        {
                            body.fontSize = body.fontSize * 1.25f;
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogWarning($"CustomBoards.ApplyBoardText: failed to change body.fontSize: {e}");
                        }

                        if (!textMeshPro.Contains(body))
                            textMeshPro.Add(body);
                    }
                    else
                    {
                        Debug.LogWarning("CustomBoards.ApplyBoardText: motdText exists but GetComponent<TextMeshPro>() returned null");
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"CustomBoards.ApplyBoardText: Exception while applying board text: {e}");
            }
        }
        private static void ApplyMaterialOnce(Material mat)
        {
            if (mat == null)
            {
                Debug.LogError("CustomBoards.ApplyMaterialOnce: provided material is null");
                return;
            }

            ApplyToBoard(
                "Environment Objects/LocalObjects_Prefab/TreeRoom",
                StumpLeaderboardIndex,
                mat
            );

            ApplyToBoard(
                "Environment Objects/LocalObjects_Prefab/Forest",
                ForestLeaderboardIndex,
                mat
            );

            if (PhotonNetworkController.Instance == null)
            {
                Debug.LogWarning("CustomBoards.ApplyMaterialOnce: PhotonNetworkController.Instance is null");
            }
            else if (PhotonNetworkController.Instance.allJoinTriggers == null)
            {
                Debug.LogWarning("CustomBoards.ApplyMaterialOnce: PhotonNetworkController.Instance.allJoinTriggers is null");
            }
            else
            {
                foreach (GorillaNetworkJoinTrigger jt in PhotonNetworkController.Instance.allJoinTriggers)
                {
                    if (jt == null)
                    {
                        Debug.LogWarning("CustomBoards.ApplyMaterialOnce: encountered null GorillaNetworkJoinTrigger in allJoinTriggers");
                        continue;
                    }

                    try
                    {
                        var ui = jt.ui;
                        if (ui == null)
                        {
                            continue;
                        }

                        JoinTriggerUITemplate t = ui.template;
                        if (t == null)
                        {
                            continue;
                        }

                        t.ScreenBG_AbandonPartyAndSoloJoin = mat;
                        t.ScreenBG_AlreadyInRoom = mat;
                        t.ScreenBG_ChangingGameModeSoloJoin = mat;
                        t.ScreenBG_Error = mat;
                        t.ScreenBG_InPrivateRoom = mat;
                        t.ScreenBG_LeaveRoomAndGroupJoin = mat;
                        t.ScreenBG_LeaveRoomAndSoloJoin = mat;
                        t.ScreenBG_NotConnectedSoloJoin = mat;

                        GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/GorillaComputerObject/ComputerUI/monitor/monitorScreen").GetComponent<Renderer>().material = mat;
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"CustomBoards.ApplyMaterialOnce: exception while applying material to join trigger {jt?.name}: {e}");
                    }
                }

                try
                {
                    PhotonNetworkController.Instance.UpdateTriggerScreens();
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"CustomBoards.ApplyMaterialOnce: failed to call UpdateTriggerScreens: {e}");
                }
            }
        }

        private static void ApplyToBoard(string path, int index, Material mat)
        {
            Transform parent = GameObject.Find(path)?.transform;
            if (parent == null)
            {
                Debug.LogWarning($"CustomBoards.ApplyToBoard: parent Transform not found for path '{path}'");
                return;
            }

            List<Transform> boards = new List<Transform>();
            for (int i = 0; i < parent.childCount; i++)
                boards.Add(parent.GetChild(i));

            boards = boards.Where(x => x.name.Contains("UnityTempFile")).ToList();
            if (index < 0 || index >= boards.Count)
            {
                Debug.LogWarning($"CustomBoards.ApplyToBoard: index {index} out of range for boards count {boards.Count} at path '{path}'");
                return;
            }

            Renderer r = boards[index].GetComponent<Renderer>();
            if (r != null)
                r.material = mat;
            else
                Debug.LogWarning($"CustomBoards.ApplyToBoard: Renderer not found on board at index {index} path '{path}'");
        }

    }
}
