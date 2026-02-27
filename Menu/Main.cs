using BepInEx;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Violet.GUI;
using Violet.Initialization;
using Violet.Menu;
using Violet.Menu.Utilities;
using Violet.Utilities;
using VioletPaid.Utilities;
using static Violet.Menu.ButtonHandler;
using static Violet.Menu.Main;
using static Violet.Menu.Optimizations;
using static Violet.Utilities.ColorLib;
using static Violet.Utilities.Variables;
using static VioletPaid.Initialization.PluginInfo;

namespace Violet.Menu
{
    public class Main : MonoBehaviour
    {
        private const string ConfigFilePath = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Gorilla Tag\\output.txt";

        private static Material colorMaterial;
        private static bool isPCMenuOpen;
        private static bool isInPcCondition;
        private static bool isInMenuCondition;
        private static GameObject thirdPersonCamera;
        private static GameObject cm;

        public void Update()
        {
            UpdateNotificationsAndButtons();
            HandleInput();
        }

        private void UpdateNotificationsAndButtons()
        {
            NotificationLib.Instance?.UpdateNotifications();

            if (ModButtons.buttons != null)
            {
                foreach (var button in ModButtons.buttons)
                {
                    if (button.Enabled && button.onEnable != null)
                    {
                        button.onEnable.Invoke();
                    }
                }
            }
        }

        private void HandleInput()
        {
            if (UnityInput.Current.GetKeyDown(PCMenuKey))
            {
                isPCMenuOpen = !isPCMenuOpen;
            }

            bool openMenu = rightHandedMenu
                ? ControllerInputPoller.instance.rightControllerSecondaryButton
                : ControllerInputPoller.instance.leftControllerSecondaryButton;

            if (isPCMenuOpen && !isInMenuCondition && !pollerInstance.leftControllerPrimaryButton && !pollerInstance.rightControllerPrimaryButton && !openMenu)
            {
                EnterPcMenuMode();
                HandlePcMenuInteraction();
            }
            else if (menuObj != null && isInPcCondition)
            {
                ExitPcMenuMode();
            }

            if (openMenu && !isInPcCondition)
            {
                EnterControllerMenuMode();
            }
            else if (menuObj != null && isInMenuCondition)
            {
                ExitControllerMenuMode();
            }
        }

        private void EnterPcMenuMode()
        {
            isInPcCondition = true;
            cm?.SetActive(false);

            if (menuObj == null)
            {
                Draw();
                AddButtonClicker(thirdPersonCamera?.transform);
            }
            else
            {
                AddButtonClicker(thirdPersonCamera?.transform);
                PositionMenuForKeyboard();
                AddTitleAndFPSCounter();
            }
        }

        private void HandlePcMenuInteraction()
        {
            if (Mouse.current.leftButton.isPressed)
            {
                if (thirdPersonCamera == null) return;

                Ray ray = thirdPersonCamera.GetComponent<Camera>().ScreenPointToRay(Mouse.current.position.ReadValue());
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    var btnCollider = hit.collider?.GetComponent<BtnCollider>();
                    if (btnCollider != null && clickerObj != null)
                    {
                        btnCollider.OnTriggerEnter(clickerObj.GetComponent<Collider>());
                    }
                }
            }
            else if (clickerObj != null)
            {
                DestroyObject(clickerObj);
            }
        }

        public static IEnumerator OnStartDelayed()
        {
            yield return new WaitForSeconds(0.1f);
            CustomBoards.Init();
        }

        private void ExitPcMenuMode()
        {
            isInPcCondition = false;
            CleanupMenu(0);
            cm?.SetActive(true);
        }

        private void EnterControllerMenuMode()
        {
            isInMenuCondition = true;

            if (menuObj == null)
            {
                Draw();
                AddRigidbodyToMenu();
                AddButtonClicker(rightHandedMenu ? taggerInstance.leftHandTransform : taggerInstance.rightHandTransform);
            }
            else
            {
                AddTitleAndFPSCounter();
                PositionMenuForHand();
            }
        }

        private void ExitControllerMenuMode()
        {
            isInMenuCondition = false;
            if (Variables.Rigidbody)
            {
                AddRigidbodyToMenu();

                Vector3 currentVelocity = rightHandedMenu
                    ? playerInstance.GetHandVelocityTracker(false).GetAverageVelocity(true, 0f, false)
                    : playerInstance.GetHandVelocityTracker(true).GetAverageVelocity(true, 0f, false);

                if (Vector3.Distance(currentVelocity, previousVelocity) > velocityThreshold)
                {
                    currentMenuRigidbody.velocity = currentVelocity;
                    previousVelocity = currentVelocity;
                }

                CleanupMenu(1);
            }
            else
            {
                CleanupMenu(0);
            }
        }

        public static IEnumerator StartMenuDelayed()
        {
            yield return new WaitForSeconds(0.1f);
            VioletGUI.StartMenu();
        }

        public void Awake()
        {
            ResourceLoader.LoadResources();
            StartCoroutine(StartMenuDelayed());
            //Board();
            StartCoroutine(OnStartDelayed());
            taggerInstance = GorillaTagger.Instance;
            playerInstance = GorillaLocomotion.GTPlayer.Instance;
            pollerInstance = ControllerInputPoller.instance;
            thirdPersonCamera = GameObject.Find("Player Objects/Third Person Camera/Shoulder Camera");
            cm = GameObject.Find("Player Objects/Third Person Camera/Shoulder Camera/CM vcam1");
        }

        public static void SaveConfigs()
        {
            if (ModButtons.buttons == null) return;

            var enabledButtonTexts = ModButtons.buttons
                .Where(b => b.Enabled)
                .Select(b => b.buttonText)
                .ToArray();

            SaveStringArrayToFile(enabledButtonTexts);
        }

        public static void SaveStringArrayToFile(string[] data)
        {
            try
            {
                File.WriteAllLines(ConfigFilePath, data ?? Array.Empty<string>());
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to save config file: {ex.Message}");
            }
        }

        public static void LoadConfigs()
        {
            try
            {
                if (!File.Exists(ConfigFilePath)) return;

                string[] contents = File.ReadAllLines(ConfigFilePath);
                if (contents == null || contents.Length == 0) return;

                foreach (var button in ModButtons.buttons)
                {
                    if (contents.Contains(button.buttonText))
                    {
                        if (!button.Enabled)
                        {
                            button.Enabled = true;
                            button.onEnable?.Invoke();
                        }
                    }
                    else
                    {
                        if (button.Enabled)
                        {
                            button.Enabled = false;
                            button.onDisable?.Invoke();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load config file: {ex.Message}");
            }
        }

        public static bool TargetPlrTagger;
        public static string PlayerInfo
        {
            get
            {
                var info = new StringBuilder();
                foreach (var player in PhotonNetwork.PlayerList)
                {
                    string masterTag = player.IsMasterClient ? " (Master)" : string.Empty;
                    var rig = RigManager.GetVRRigFromPlayer(player);
                    int fps = rig != null ? rig.fps : 0;
                    info.AppendLine($"{player.NickName} (Actor: {player.ActorNumber}){masterTag} (FPS: {fps})");
                }

                return info.ToString();
            }
        }

        private static Material originalMat1;
        private static Material originalMat2;
        private static Material originalMat3;
        private static Color32 _cachedRgbColor;

        public static void Board()
        {
            try
            {
                Color32 BoardColor = Color.violet;

                colorMaterial ??= new Material(Shader.Find("GorillaTag/UberShader"));
                colorMaterial.color = ButtonColorOff;
                colorMaterial.SetFloat("_Mode", 2f);

                var mat = new Material(Shader.Find("GorillaTag/UberShader")) { color = colorMaterial.color };

                var wallMonitorPath = "Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomBoundaryStones/BoundaryStoneSet_Forest/wallmonitorforestbg";
                var wallMonitor = GameObject.Find(wallMonitorPath);
                if (wallMonitor != null)
                {
                    var r = wallMonitor.GetComponent<Renderer>();
                    if (r != null) r.material = mat;
                }

                const string motdBodyPath = "Environment Objects/LocalObjects_Prefab/TreeRoom/motdBodyText";
                const string motdHeadingPath = "Environment Objects/LocalObjects_Prefab/TreeRoom/motdHeadingText";
                const string cocHeadingPath = "Environment Objects/LocalObjects_Prefab/TreeRoom/CodeOfConductHeadingText";

                if (PhotonNetwork.InRoom)
                {
                    var motdObj = GameObject.Find(motdBodyPath);
                    if (motdObj != null)
                    {
                        var motdText = motdObj.GetComponent<TextMeshPro>();
                        if (motdText != null)
                        {
                            motdText.text = $"Hey guys its tortise if you want to support me give me money.\n now here are the details of your lobby\n \n" +
                                            $"Is Master {PhotonNetwork.IsMasterClient}\nPlayer Count: {PhotonNetwork.CurrentRoom.PlayerCount} / 10 \nRegion: {PhotonNetwork.CloudRegion}";
                            var meshRenderer = motdObj.GetComponent<MeshRenderer>();
                            if (meshRenderer != null) meshRenderer.material.color = BoardColor;
                        }
                    }

                    var cocHeadingObj = GameObject.Find(cocHeadingPath);
                    if (cocHeadingObj != null)
                    {
                        var cocHeading = cocHeadingObj.GetComponent<TextMeshPro>();
                        if (cocHeading != null) cocHeading.text = "Violet Made By: Tortise& RaiseEvent201";
                    }
                }
                else
                {
                    var motdObj = GameObject.Find(motdBodyPath);
                    if (motdObj != null)
                    {
                        var motdText = motdObj.GetComponent<TextMeshPro>();
                        if (motdText != null)
                        {
                            motdText.text = "not in room join one....";
                            var meshRenderer = motdObj.GetComponent<MeshRenderer>();
                            if (meshRenderer != null) meshRenderer.material.color = BoardColor;
                        }
                    }

                    var motdHeadingObj = GameObject.Find(motdHeadingPath);
                    if (motdHeadingObj != null)
                    {
                        var motdHeadingText = motdHeadingObj.GetComponent<TextMeshPro>();
                        if (motdHeadingText != null)
                        {
                            motdHeadingText.text = $"Violet V{VioletPaid.Initialization.PluginInfo.menuVersion}";
                            var meshRenderer = motdHeadingObj.GetComponent<MeshRenderer>();
                            if (meshRenderer != null) meshRenderer.material.color = BoardColor;
                        }
                    }

                    var cocHeadingObj = GameObject.Find(cocHeadingPath);
                    if (cocHeadingObj != null)
                    {
                        var cocHeading = cocHeadingObj.GetComponent<TextMeshPro>();
                        if (cocHeading != null)
                        {
                            cocHeading.text = "Violet Made By: Tortise & RaiseEvent201";
                            var meshRenderer = cocHeadingObj.GetComponent<MeshRenderer>();
                            if (meshRenderer != null) meshRenderer.material.color = BoardColor;
                        }
                    }
                }

                var gamemodeTitle = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/GameModes Title Text");
                if (gamemodeTitle != null)
                {
                    var mesh = gamemodeTitle.GetComponent<MeshRenderer>();
                    if (mesh != null) mesh.material.color = BoardColor;
                    var txt = gamemodeTitle.GetComponent<TextMeshPro>();
                    if (txt != null) txt.text = "Violet";
                }

                var mater = new Material(Shader.Find("GorillaTag/UberShader")) { color = colorMaterial.color };
                Tools.ChangeBoardMaterial("Environment Objects/LocalObjects_Prefab/TreeRoom", 5, mater, ref originalMat1);
                Tools.ChangeBoardMaterial("Environment Objects/LocalObjects_Prefab/Forest", 13, mater, ref originalMat2);
                Tools.ChangeBoardMaterial("Environment Objects/LocalObjects_Prefab/Forest/Terrain", 11, mater, ref originalMat3);

                var monitorRendererObj = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/GorillaComputerObject/ComputerUI/monitor/monitorScreen");
                if (monitorRendererObj != null)
                {
                    var monitorRenderer = monitorRendererObj.GetComponent<MeshRenderer>();
                    if (monitorRenderer != null)
                    {
                        monitorRenderer.material = mat;
                        monitorRenderer.material.color = colorMaterial.color;
                    }
                }

                const string cocBodyPath = "Environment Objects/LocalObjects_Prefab/TreeRoom/COCBodyText_TitleData";
                var cocBodyObj = GameObject.Find(cocBodyPath);
                if (cocBodyObj != null)
                {
                    var cocText = cocBodyObj.GetComponent<TextMeshPro>();
                    if (cocText != null)
                    {
                        cocText.color = BoardColor;
                        cocText.fontStyle = FontStyles.Bold;
                        cocText.alignment = TextAlignmentOptions.Top;
                        cocText.fontSize = 75;
                        cocText.text = PhotonNetwork.InRoom ? "\n \n" + PlayerInfo : "\nNOT CONNECTED TO A ROOM\n";
                    }
                }

                const string cocTextPath = "Environment Objects/LocalObjects_Prefab/TreeRoom/COC Text";
                var cocTextObj2 = GameObject.Find(cocTextPath);
                if (cocTextObj2 != null)
                {
                    var cocText2 = cocTextObj2.GetComponent<TextMeshPro>();
                    if (cocText2 != null)
                    {
                        cocText2.alignment = TextAlignmentOptions.Top;
                        cocText2.color = BoardColor;
                    }
                }

                var gmTitleObj = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/GameModes Title Text");
                if (gmTitleObj != null)
                {
                    var gmText = gmTitleObj.GetComponent<TextMeshPro>();
                    if (gmText != null) gmText.text = "Violet";
                    var gmMesh = gmTitleObj.GetComponent<MeshRenderer>();
                    if (gmMesh != null) gmMesh.material.color = BoardColor;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Board() exception: {ex.Message}");
            }
        }

        public static void Draw()
        {
            if (menuObj != null)
            {
                ClearMenuObjects();
                return;
            }

            CreateMenuObject();
            CreateBackground();
            CreateMenuCanvasAndTitle();
            if (toggledisconnectButton) AddDisconnectButton();
            AddReturnButton();
            AddPageButton(">");
            AddPageButton("<");
            if (SideCatagorys) DrawCategoryTabs();
            if (Variables.PlayerTab) DrawPlayerTabs();

            ButtonPool.ResetPool();
            var buttonsToDraw = GetButtonInfoByPage(currentPage).Skip(currentCategoryPage * ButtonsPerPage).Take(ButtonsPerPage).ToArray();
            for (int i = 0; i < buttonsToDraw.Length; i++)
            {
                AddModButton(i * 0.09f, buttonsToDraw[i]);
            }
        }

        private static void CreateMenuObject()
        {
            menuObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(menuObj.GetComponent<Renderer>());
            menuObj.name = "menu";
            menuObj.transform.localScale = new Vector3(0.1f, 0.3f, 0.3825f);
            menuObj.AddComponent<BoxCollider>();
        }

        private static void CreateBackground()
        {
            background = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(background.GetComponent<Rigidbody>());
            Destroy(background.GetComponent<BoxCollider>());
            Outline(background, violet);
            background.GetComponent<MeshRenderer>().material.color = MenuColor;
            background.transform.parent = menuObj.transform;
            background.transform.rotation = Quaternion.identity;
            background.transform.localScale = new Vector3(0.11f, 0.97f, 1.03f);
            background.name = "menucolor";
            background.transform.position = new Vector3(0.05f, 0f, -0f);
        }

        private static void CreateMenuCanvasAndTitle()
        {
            canvasObj = new GameObject { name = "canvas" };
            canvasObj.transform.parent = menuObj.transform;
            var canvas = canvasObj.AddComponent<Canvas>();
            var canvasScaler = canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvasScaler.dynamicPixelsPerUnit = 1000;

            var titleObj = new GameObject { transform = { parent = canvasObj.transform, localScale = new Vector3(0.875f, 0.875f, 1f) } };
            title = titleObj.AddComponent<Text>();
            title.font = ResourceLoader.ArialFont;
            title.fontStyle = FontStyle.Bold;
            title.color = Theme == 3 ? Black : White;
            title.fontSize = 7;
            title.alignment = TextAnchor.MiddleCenter;
            title.resizeTextForBestFit = true;
            title.resizeTextMinSize = 0;
            var titleTransform = title.GetComponent<RectTransform>();
            titleTransform.localPosition = Vector3.zero;
            titleTransform.position = new Vector3(0.07f, 0f, 0.17f);
            titleTransform.rotation = Quaternion.Euler(180f, 90f, 90f);
            titleTransform.sizeDelta = new Vector2(0.21f, 0.065f);
        }

        private static void AddDisconnectButton()
        {
            if (!PhotonNetwork.InRoom)
            {
                Destroy(disconnectButton);
                return;
            }

            disconnectButton = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(disconnectButton.GetComponent<Rigidbody>());
            disconnectButton.GetComponent<BoxCollider>().isTrigger = true;
            Outline(disconnectButton, Indigo);
            disconnectButton.transform.parent = menuObj.transform;
            disconnectButton.transform.rotation = Quaternion.identity;
            disconnectButton.transform.localScale = new Vector3(0.09f, 0.9f, 0.08f);
            disconnectButton.transform.localPosition = new Vector3(0.56f, 0f, 0.588f);
            disconnectButton.AddComponent<BtnCollider>().clickedButton = new ButtonHandler.Button("DisconnectButton", Category.Home, false, false, null, null);
            disconnectButton.GetComponent<Renderer>().material.color = DisconnectColor;

            var discontext = new GameObject { transform = { parent = canvasObj.transform } }.AddComponent<Text>();
            discontext.text = "Disconnect";
            discontext.font = Font.CreateDynamicFontFromOSFont("Anton", 1);
            discontext.fontStyle = FontStyle.Bold;
            discontext.color = Theme == 3 ? Black : White;
            discontext.alignment = TextAnchor.MiddleCenter;
            discontext.resizeTextForBestFit = true;
            discontext.resizeTextMinSize = 0;
            var rectt = discontext.GetComponent<RectTransform>();
            rectt.localScale = new Vector3(0.9f, 0.9f, 0.9f);
            rectt.sizeDelta = new Vector2(0.13f, 0.023f);
            rectt.localPosition = new Vector3(0.064f, 0f, 0.2295f);
            rectt.rotation = Quaternion.Euler(180f, 90f, 90f);
        }

        private static void AddReturnButton()
        {
            var backToStartButton = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(backToStartButton.GetComponent<Rigidbody>());
            backToStartButton.GetComponent<BoxCollider>().isTrigger = true;
            Outline(backToStartButton, violet);
            backToStartButton.transform.parent = menuObj.transform;
            backToStartButton.transform.rotation = Quaternion.identity;
            backToStartButton.transform.localScale = new Vector3(0.09f, 0.30625f, 0.08f);
            backToStartButton.transform.localPosition = new Vector3(0.56f, 0f, -0.435f);
            backToStartButton.AddComponent<BtnCollider>().clickedButton = new ButtonHandler.Button("ReturnButton", Category.Home, false, false, null, null);
            backToStartButton.GetComponent<Renderer>().material.color = ButtonColorOff;

            Outline(backToStartButton, violet);

            var titleObj = new GameObject { transform = { parent = canvasObj.transform, localScale = new Vector3(0.9f, 0.9f, 0.9f), localPosition = new Vector3(0.85f, 0.85f, 0.85f) } };
            var title = titleObj.AddComponent<Text>();
            title.font = ResourceLoader.ArialFont;
            title.fontStyle = FontStyle.Bold;
            title.text = "Home";
            title.color = Theme == 3 ? Black : White;
            title.fontSize = 3;
            title.alignment = TextAnchor.MiddleCenter;
            title.resizeTextForBestFit = true;
            title.resizeTextMinSize = 0;
            var titleTransform = title.GetComponent<RectTransform>();
            titleTransform.localPosition = Vector3.zero;
            titleTransform.sizeDelta = new Vector2(0.25f, 0.025f);
            titleTransform.localPosition = new Vector3(0.064f, 0f, -0.165f);
            titleTransform.rotation = Quaternion.Euler(180f, 90f, 90f);
        }

        private static void AddPageButton(string buttonText)
        {
            PageButtons = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(PageButtons.GetComponent<Rigidbody>());
            Outline(PageButtons, DarkerGrey);
            PageButtons.GetComponent<BoxCollider>().isTrigger = true;
            PageButtons.transform.parent = menuObj.transform;
            PageButtons.transform.rotation = Quaternion.identity;
            PageButtons.transform.localScale = new Vector3(0.09f, 0.25f, 0.079f);
            PageButtons.transform.localPosition = new Vector3(0.56f, buttonText.Contains("<") ? 0.2925f : -0.2925f, -0.435f);
            PageButtons.GetComponent<Renderer>().material.color = ButtonColorOff;
            PageButtons.AddComponent<BtnCollider>().clickedButton = new ButtonHandler.Button(buttonText, Category.Home, false, false, null, null);


            var titleObj = new GameObject { transform = { parent = canvasObj.transform, localScale = new Vector3(0.9f, 0.9f, 0.9f) } };
            var title = titleObj.AddComponent<Text>();
            title.font = ResourceLoader.ArialFont;
            title.color = Theme == 3 ? Black : White;
            title.fontSize = 5;
            title.fontStyle = FontStyle.Normal;
            title.alignment = TextAnchor.MiddleCenter;
            title.resizeTextForBestFit = true;
            title.resizeTextMinSize = 0;
            title.text = buttonText.Contains("<") ? "<<<" : ">>>";
            var titleTransform = title.GetComponent<RectTransform>();
            titleTransform.localPosition = Vector3.zero;
            titleTransform.sizeDelta = new Vector2(0.3f, 0.04f);
            titleTransform.position = new Vector3(0.064f, buttonText.Contains("<") ? 0.087f : -0.087f, -0.165f);
            titleTransform.rotation = Quaternion.Euler(180f, 90f, 90f);
        }

        public static int PgNumber = 0;
        private static void DrawCategoryTabs()
        {
            /*Catagorybackground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(Catagorybackground.GetComponent<Rigidbody>());
            Destroy(Catagorybackground.GetComponent<BoxCollider>());
            Outline(Catagorybackground, violet);
            Catagorybackground.GetComponent<MeshRenderer>().material.color = MenuColor;
            Catagorybackground.transform.parent = menuObj.transform;
            Catagorybackground.transform.rotation = Quaternion.identity;
            Catagorybackground.transform.localScale = new Vector3(0.11f, 0.36f, 1.03f);
            Catagorybackground.name = "menucolor";
            Catagorybackground.transform.position = new Vector3(0.05f, -0.21f, -0f);

            if (PgNumber == 0)
            {
                DrawCategoryTab(0f, 0f, "Settings", ()=> ChangePage(Category.Settings));
                DrawCategoryTab(1f, 0.037f, "Room", () => ChangePage(Category.Room));
                DrawCategoryTab(2f, 0.074f, "Movement", () => ChangePage(Category.Movement));
                DrawCategoryTab(3f, 0.111f, "Safety", () => ChangePage(Category.Saftey));
                DrawCategoryTab(4f, 0.148f, "Player", () => ChangePage(Category.Player));
                DrawCategoryTab(5f, 0.187f, "World", () => ChangePage(Category.World));
                DrawCategoryTab(6f, 0.224f, "Visual", () => ChangePage(Category.Visuals));
                DrawCategoryTab(7f, 0.263f, "Projectiles", () => ChangePage(Category.Projectiles));
                DrawCategoryTab(8f, 0.301f, "Master", () => ChangePage(Category.Master));
                DrawCategoryTab(9f, 0.339f, "Next Page", () => PgNumber = 1);
            }

            else if (PgNumber == 1)
            {
                DrawCategoryTab(0f, 0f, "Guardian", () => ChangePage(Category.Guardian));
                DrawCategoryTab(1f, 0.037f, "Critters", () => ChangePage(Category.Critters));
                DrawCategoryTab(2f, 0.074f, "Overpowered", () => ChangePage(Category.Overpowered));
                DrawCategoryTab(3f, 0.111f, "Stone", () => ChangePage(Category.StoneNetworking));
                DrawCategoryTab(4f, 0.148f, "SoundBoard", () => ChangePage(Category.SoundBoard));
                DrawCategoryTab(9f, 0.339f, "Back Page", () => PgNumber = 0);
            }*/
            
        }

        private static void DrawCategoryTab(float multiplier, float textOffset, string name, Action action)
        {
            float positionOffset = multiplier * 0.1f;

            var btn = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(btn.GetComponent<Rigidbody>());
            btn.GetComponent<BoxCollider>().isTrigger = true;
            btn.transform.parent = menuObj.transform;
            btn.transform.rotation = Quaternion.identity;
            btn.transform.localScale = new Vector3(0.09f, 0.3f, 0.08f);
            btn.transform.localPosition = new Vector3(0.56f, catsSwitch ? -0.705f : 0.705f, 0.447f - positionOffset);
            btn.AddComponent<BtnCollider>().clickedButton = new ButtonHandler.Button("CatButton", Category.Home, false, false, action, null);
            btn.GetComponent<Renderer>().material.color = ButtonColorOff;

            var titleObj = new GameObject { transform = { parent = canvasObj.transform, localScale = new Vector3(0.9f, 0.9f, 0.9f), localPosition = new Vector3(0.85f, 0.67f, 0.85f) } };
            var title = titleObj.AddComponent<Text>();
            title.fontStyle = FontStyle.BoldAndItalic;
            title.font = ResourceLoader.MSGothicFont;
            title.text = name;
            title.color = White;
            title.alignment = TextAnchor.MiddleCenter;
            title.resizeTextForBestFit = true;
            title.resizeTextMinSize = 0;
            var titleTransform = title.GetComponent<RectTransform>();
            titleTransform.localPosition = Vector3.zero;
            titleTransform.sizeDelta = new Vector2(0.14f, 0.014f);
            titleTransform.localPosition = new Vector3(0.064f, catsSwitch ? -0.215f : 0.215f, 0.17f - textOffset);
            titleTransform.rotation = Quaternion.Euler(180f, 90f, 90f);
        }


        public static int index = 0;
        public static int selectedActorNumber;

        private static void PlayerTab(float multiplier, float textOffset, Category category, Player plr)
        {
            /*float positionOffset = multiplier * 0.1f;

            // Create button
            var btn = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(btn.GetComponent<Rigidbody>());
            btn.GetComponent<BoxCollider>().isTrigger = true;
            btn.transform.SetParent(menuObj.transform, false);
            btn.transform.rotation = Quaternion.identity;
            btn.transform.localScale = new Vector3(0.09f, 0.3f, 0.08f);
            btn.transform.localPosition = new Vector3(0.56f, 0.705f, 0.447f - positionOffset);
            btn.GetComponent<Renderer>().material.color = ButtonColorOff;

            int actorNumber = plr.ActorNumber;
            Action action = () =>
            {
                selectedActorNumber = actorNumber; 
                ChangePage(category);
                ButtonHandler.ChangeButtonText($"Lag Player", "Lag Player ["+ PhotonNetwork.CurrentRoom.GetPlayer(selectedActorNumber).NickName+ "]");
                ButtonHandler.ChangeButtonText($"Stutter Player", "Stutter Player [" + PhotonNetwork.CurrentRoom.GetPlayer(selectedActorNumber).NickName + "]");
                ButtonHandler.ChangeButtonText($"Freeze Player", "Freeze Player [" + PhotonNetwork.CurrentRoom.GetPlayer(selectedActorNumber).NickName + "]");
                ButtonHandler.ChangeButtonText("Tag Player", "Tag Player [" +PhotonNetwork.CurrentRoom.GetPlayer(selectedActorNumber).NickName + "]");
                ButtonHandler.ChangeButtonText("Crash Player", "Crash Player [" + PhotonNetwork.CurrentRoom.GetPlayer(selectedActorNumber).NickName + "]");
                ButtonHandler.ChangeButtonText("UnTag Player", "Untag Player [" + PhotonNetwork.CurrentRoom.GetPlayer(selectedActorNumber).NickName + "]" + " [<color=red>M</color>]");
                ButtonHandler.ChangeButtonText("Slow Player", "Slow Player [" + PhotonNetwork.CurrentRoom.GetPlayer(selectedActorNumber).NickName + "]" + " [<color=red>M</color>]");
                ButtonHandler.ChangeButtonText("Vibrate Player", "Vibrate Player [" + PhotonNetwork.CurrentRoom.GetPlayer(selectedActorNumber).NickName + "]" + " [<color=red>M</color>]");
                ButtonHandler.ChangeButtonText("Infection Particle Player", "Infection Particle Player [" + PhotonNetwork.CurrentRoom.GetPlayer(selectedActorNumber).NickName + "]" + " [<color=red>M</color>]");
                ButtonHandler.ChangeButtonText("Fling Player", "Fling Player [" + PhotonNetwork.CurrentRoom.GetPlayer(selectedActorNumber).NickName + "]");
            };

            btn.AddComponent<BtnCollider>().clickedButton = new ButtonHandler.Button("Temp Player", Category.Home, false, false, action, null);

            // Create title text
            var titleObj = new GameObject("PlayerNameText");
            titleObj.transform.SetParent(canvasObj.transform, false);
            titleObj.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);

            var title = titleObj.AddComponent<Text>();
            title.fontStyle = FontStyle.Bold;
            title.font = ResourceLoader.ArialFont;
            title.text = plr.NickName ?? $"Player {index + 1}";
            title.color = Color.white;
            title.alignment = TextAnchor.MiddleCenter;
            title.resizeTextForBestFit = true;
            title.resizeTextMinSize = 0;

            var titleTransform = title.GetComponent<RectTransform>();
            titleTransform.sizeDelta = new Vector2(0.155f, 0.0155f);
            titleTransform.localPosition = new Vector3(0.064f, 0.215f, 0.17f - textOffset);
            titleTransform.rotation = Quaternion.Euler(180f, 90f, 90f);*/
        }

        private static void DrawPlayerTabs()
        {
            if (PhotonNetwork.InRoom)
            {
                /*GameObject yuh = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Destroy(yuh.GetComponent<Rigidbody>());
                Destroy(yuh.GetComponent<BoxCollider>());
                Outline(yuh, violet);
                yuh.GetComponent<MeshRenderer>().material.color = MenuColor;
                yuh.transform.parent = menuObj.transform;
                yuh.transform.rotation = Quaternion.identity;
                yuh.transform.localScale = new Vector3(0.11f, 0.36f, 1.03f);
                yuh.name = "menucolor";
                yuh.transform.position = new Vector3(0.05f, 0.21f, -0f);*/
            }

            index = 0;
            if (PhotonNetwork.PlayerList == null) return;

            for (int i = 0; i < PhotonNetwork.PlayerList.Length && i < 10; i++)
            {
                var plr = PhotonNetwork.PlayerList[i];
                float multiplier = i;
                float textOffset = i * 0.037f;
                Category category = Category.Player;

                PlayerTab(multiplier, textOffset, category, plr);
                index++;
            }
        }


        private static void AddModButton(float offset, ButtonHandler.Button button)
        {
            var modButton = ButtonPool.GetButton();
            Destroy(modButton.GetComponent<Rigidbody>());
            var btnCollider = modButton.GetComponent<BoxCollider>();
            if (btnCollider != null) btnCollider.isTrigger = true;
            modButton.transform.SetParent(menuObj.transform, false);
            modButton.transform.rotation = Quaternion.identity;
            modButton.transform.localScale = new Vector3(0.09f, 0.9f, 0.08f);
            modButton.transform.localPosition = new Vector3(0.56f, 0f, 0.32f - offset);
            var btnColScript = modButton.GetComponent<BtnCollider>() ?? modButton.AddComponent<BtnCollider>();
            btnColScript.clickedButton = button;

            var titleObj = TextPool.GetTextObject();
            titleObj.transform.SetParent(canvasObj.transform, false);
            titleObj.transform.localScale = new Vector3(0.95f, 0.95f, 1f);
            var title = titleObj.GetComponent<Text>();
            title.text = button.buttonText;
            title.font = ResourceLoader.ArialFont;
            title.fontStyle = FontStyle.Bold;
            title.color = Theme == 3 ? Black : White;
            var titleTransform = title.GetComponent<RectTransform>();
            titleTransform.localPosition = new Vector3(0.064f, 0, 0.126f - offset / 2.6f);
            titleTransform.rotation = Quaternion.Euler(180f, 90f, 90f);
            titleTransform.sizeDelta = new Vector2(0.21f, 0.02225f);


            var btnRenderer = modButton.GetComponent<Renderer>();
            if (btnRenderer != null)
            {
                btnRenderer.material.color = button.Enabled ? ButtonColorOn : ButtonColorOff;
            }
        }

        private static void AddTitleAndFPSCounter()
        {
            fps = Time.deltaTime > 0 ? Mathf.RoundToInt(1f / Time.deltaTime) : 0;
            title.fontStyle = FontStyle.Bold;
            title.text = true
                ? $"{menuName}\nFPS: {fps} | Version: {menuVersion}"
                : $"{menuName} [Page: {currentCategoryPage + 1}]\nFPS: {fps} | Version: {menuVersion}";
        }

        public static void Outline(GameObject obj, Color color)
        {
            var outlineObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(outlineObj.GetComponent<Rigidbody>());
            Destroy(outlineObj.GetComponent<BoxCollider>());
            outlineObj.transform.parent = obj.transform;
            outlineObj.transform.rotation = Quaternion.identity;
            outlineObj.transform.localPosition = obj.transform.localPosition;
            outlineObj.transform.localScale = obj.transform.localScale + new Vector3(-0.01f, 0.0145f, 0.0145f);
            outlineObj.GetComponent<MeshRenderer>().material.color = outColor;

        }

        private static void PositionMenuForHand()
        {
            if (bark)
            {
                menuObj.transform.position = GorillaTagger.Instance.headCollider.transform.position +
                                            GorillaTagger.Instance.headCollider.transform.forward * 0.5f +
                                            GorillaTagger.Instance.headCollider.transform.up * -0.1f;
                menuObj.transform.LookAt(GorillaTagger.Instance.headCollider.transform);
                var rotModify = menuObj.transform.rotation.eulerAngles + new Vector3(-90f, 0f, -90f);
                menuObj.transform.rotation = Quaternion.Euler(rotModify);
            }
            else if (rightHandedMenu)
            {
                menuObj.transform.position = taggerInstance.rightHandTransform.position;
                var rotation = taggerInstance.rightHandTransform.rotation.eulerAngles + new Vector3(0f, 0f, 180f);
                menuObj.transform.rotation = Quaternion.Euler(rotation);
            }
            else
            {
                menuObj.transform.position = taggerInstance.leftHandTransform.position;
                menuObj.transform.rotation = taggerInstance.leftHandTransform.rotation;
            }
        }

        private static void PositionMenuForKeyboard()
        {
            if (thirdPersonCamera == null) return;

            thirdPersonCamera.transform.position = new Vector3(-65.8373f, 21.6568f, -80.9763f);
            thirdPersonCamera.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            menuObj.transform.SetParent(thirdPersonCamera.transform, true);

            var headPosition = thirdPersonCamera.transform.position;
            var headRotation = thirdPersonCamera.transform.rotation;
            float offsetDistance = 0.65f;
            var offsetPosition = headPosition + headRotation * Vector3.forward * offsetDistance;
            menuObj.transform.position = offsetPosition;

            var directionToHead = headPosition - menuObj.transform.position;
            menuObj.transform.rotation = Quaternion.LookRotation(directionToHead, Vector3.up);
            menuObj.transform.Rotate(Vector3.up, -90f);
            menuObj.transform.Rotate(Vector3.right, -90f);
        }

        public static void AddButtonClicker(Transform parentTransform)
        {
            if (clickerObj == null)
            {
                clickerObj = new GameObject("buttonclicker");
                var clickerCollider = clickerObj.AddComponent<BoxCollider>();
                clickerCollider.isTrigger = true;
                var meshFilter = clickerObj.AddComponent<MeshFilter>();
                meshFilter.mesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
                var clickerRenderer = clickerObj.AddComponent<MeshRenderer>();
                clickerRenderer.material.color = White;
                clickerRenderer.material.shader = Shader.Find("GUI/Text Shader");
                if (parentTransform != null)
                {
                    clickerObj.transform.parent = parentTransform;
                    clickerObj.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
                    clickerObj.transform.localPosition = new Vector3(0f, -0.1f, 0f);
                }
            }
        }

        public static void AddRigidbodyToMenu()
        {
            if (menuObj == null) return;

            currentMenuRigidbody = menuObj.GetComponent<Rigidbody>() ?? menuObj.AddComponent<Rigidbody>();
            currentMenuRigidbody.useGravity = gravity;
            currentMenuRigidbody.mass = 0.5f;
        }


        #region Settings

        public static void ChangeTheme()
        {
            Theme++;
            if (Theme > 5) Theme = 0;

            switch (Theme)
            {
                case 0:
                    SetDefaultTheme();
                    ButtonHandler.ChangeButtonText("Current Menu Theme", "Current Menu Theme [Base]");
                    break;
                case 1:
                    MenuColorT = SkyBlueTransparent;
                    MenuColor = SkyBlue;
                    ButtonColorOff = RoyalBlue;
                    ButtonColorOn = DodgerBlue;
                    outColor = DarkDodgerBlue;
                    DisconnectColor = Crimson;
                    disOut = WineRed;
                    ButtonHandler.ChangeButtonText("Current Menu Theme", "Current Menu Theme [Blue]");
                    break;
                case 2:
                    MenuColorT = FireBrickTransparent;
                    MenuColor = FireBrick;
                    ButtonColorOff = WineRed;
                    ButtonColorOn = IndianRed;
                    outColor = IndianRed;
                    DisconnectColor = Crimson;
                    disOut = WineRed;
                    ButtonHandler.ChangeButtonText("Current Menu Theme", "Current Menu Theme [Red]");
                    break;
                case 3:
                    MenuColorT = new Color32(171, 129, 182, 80);
                    MenuColor = new Color32(171, 129, 182, 255);
                    ButtonColorOff = Plum;
                    ButtonColorOn = MediumOrchid;
                    outColor = DarkSlateBlue;
                    DisconnectColor = ButtonColorOff;
                    disOut = outColor;
                    ButtonHandler.ChangeButtonText("Current Menu Theme", "Current Menu Theme [Lavender]");
                    break;
                case 4:
                    MenuColorT = MediumAquamarineTransparent;
                    MenuColor = MediumAquamarine;
                    ButtonColorOff = MediumSeaGreen;
                    ButtonColorOn = SeaGreen;
                    DisconnectColor = ButtonColorOff;
                    outColor = Lime;
                    disOut = outColor;
                    ButtonHandler.ChangeButtonText("Current Menu Theme", "Current Menu Theme [Green]");
                    break;
                case 5:
                    MenuColorT = BlackTransparent;
                    MenuColor = Black;
                    ButtonColorOff = DarkerGrey;
                    ButtonColorOn = WineRed;
                    DisconnectColor = WineRed;
                    outColor = DarkDodgerBlue;
                    disOut = Maroon;
                    ButtonHandler.ChangeButtonText("Current Menu Theme", "Current Menu Theme [Dark]");
                    break;
            }
            RefreshMenu();
        }

        private static void SetDefaultTheme()
        {
            MenuColor = Black;
            MenuColorT = MenuColor;
            ButtonColorOff = ColorLib.Violet;
            ButtonColorOn = Indigo;
            DisconnectColor = violet;
            outColor = ColorLib.Violet;
            disOut = WineRed;
            NotificationLib.SendNotification("<color=white>[</color><color=blue>Theme</color><color=white>] Violet/Default</color>");
            RefreshMenu();
        }

        public static void ChangeOutlineColor()
        {
            Theme++;
            if (Theme > 8) Theme = 1;

            switch (Theme)
            {
                case 1:
                    RainbowOutline = false;
                    outColor = Navy;
                    ButtonHandler.ChangeButtonText("Current Menu Outline", "Current Menu Outline [Blue]");
                    break;
                case 2:
                    RainbowOutline = false;
                    outColor = WineRed;
                    ButtonHandler.ChangeButtonText("Current Menu Outline", "Current Menu Outline [Red]");
                    break;
                case 3:
                    RainbowOutline = false;
                    outColor = Green;
                    ButtonHandler.ChangeButtonText("Current Menu Outline", "Current Menu Outline [Green]");
                    break;
                case 4:
                    RainbowOutline = false;
                    outColor = Black;
                    ButtonHandler.ChangeButtonText("Current Menu Outline", "Current Menu Outline [Dark]");
                    break;
                case 5:
                    RainbowOutline = false;
                    outColor = Purple;
                    ButtonHandler.ChangeButtonText("Current Menu Outline", "Current Menu Outline [Purple]");
                    break;
                case 6:
                    RainbowOutline = false;
                    outColor = Orange;
                    ButtonHandler.ChangeButtonText("Current Menu Outline", "Current Menu Outline [Orange]");
                    break;
                case 7:
                    RainbowOutline = false;
                    outColor = Yellow;
                    ButtonHandler.ChangeButtonText("Current Menu Outline", "Current Menu Outline [Yellow]");
                    break;
                case 8:
                    RainbowOutline = false;
                    outColor = Pink;
                    ButtonHandler.ChangeButtonText("Current Menu Outline", "Current Menu Outline [Pink]");
                    break;
            }
        }

        public static void ChangeSound()
        {
            SoundIndex++;
            if (SoundIndex > 6) SoundIndex = 1;

            ActualSound = SoundIndex switch
            {
                1 => 66,
                2 => 8,
                3 => 203,
                4 => 50,
                5 => 67,
                6 => 114,
                _ => 66
            };
        }

        public static void EnableGravity(bool enable)
        {
            gravity = enable;
        }
        #endregion
    }

}