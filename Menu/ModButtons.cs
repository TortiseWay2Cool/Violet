
using Photon.Pun;
using Violet.Utilities;
using Violet.Utilities.Patches;
using VioletPaid.Utilities;
using static Violet.Menu.ButtonHandler;
using static Violet.Menu.Main;
using static Violet.Utilities.Variables;

using static Violet.Mods.Advantage;
using static Violet.Mods.Overpowered;
using static Violet.Mods.Master;

using UnityEngine;
using Violet.GUI;

namespace Violet.Menu
{
    public enum Category
    {
        Home,
        Settings,
        MenuSettings,
        GunSettings,
        PlayerSettings,

        Room,
        Saftey,
        Movement,
        Player,
        Advantage,
        Visuals,
        Projectiles,
        Overpowered,
        Master,
        SoundBoard,


    }
    public class ModButtons
    {
        public static Button[] buttons =
        {
            #region Main Page | Starting Page
            new Button("Settings", Category.Home, false, false, () => ChangePage(Category.Settings)),
            new Button("Room", Category.Home, false, false, () => ChangePage(Category.Room)),
            new Button("Safety", Category.Home, false, false, () => ChangePage(Category.Saftey)),
            new Button("Movement", Category.Home, false, false, () => ChangePage(Category.Movement)),
            new Button("Player", Category.Home, false, false, () => ChangePage(Category.Player)),
            new Button("Advantage", Category.Home, false, false, () => ChangePage(Category.Advantage)),
            new Button("Visuals", Category.Home, false, false, () => ChangePage(Category.Visuals)),
            new Button("Overpowered", Category.Home, false, false, () => ChangePage(Category.Overpowered)),
            new Button("Projectiles", Category.Home, false, false, () => ChangePage(Category.Projectiles)),
            new Button("Soundboard", Category.Home, false, false, () => ChangePage(Category.SoundBoard)),
            #endregion


            // Settings 
            new Button("Save Configs", Category.Settings, false, false, () => SaveConfigs()),
            new Button("Load Configs", Category.Settings, false, false, () => LoadConfigs()),
            new Button("Menu Setting", Category.Settings, false, false, () => ChangePage(Category.MenuSettings)),
            new Button("Gun Setting", Category.Settings, false, false, () => ChangePage(Category.GunSettings)),
            new Button("Player Setting", Category.Settings, false, false, () => ChangePage(Category.PlayerSettings)),
            
            //menu settings
            new Button("Right-Handed Menu", Category.MenuSettings, true, false, () => rightHandedMenu = false, () => rightHandedMenu = true),
            new Button("Custom Boards", Category.MenuSettings, true, true, () => Main.Board()), // Adding this Later
            new Button("Disable Array List", Category.MenuSettings, true, false, () => VioletGUI.guiEnabled = true, () => VioletGUI.guiEnabled = false),
            new Button("Toggle Disconect Button", Category.MenuSettings, true, false, () => toggledisconnectButton = true, ()=> toggledisconnectButton = false),
            new Button("Toggle Player Catagorys", Category.MenuSettings, true, false, () => SideCatagorys = true, () => SideCatagorys = false),
            new Button("Toggle Player Tabs", Category.MenuSettings, true, false, () => Variables.PlayerTab = true, ()=> Variables.PlayerTab = false),
            new Button("Toggle Rigidbdy", Category.MenuSettings, true, false, () => Variables.Rigidbody = true, ()=> Variables.Rigidbody = false),
            new Button("Toggle Gravity", Category.MenuSettings, true, false, () => Variables.gravity = true, ()=> Variables.gravity = false),
            new Button("Current Menu Theme []", Category.MenuSettings, false, false, ()=> Main.ChangeTheme()),
            new Button("Current Menu Outline []", Category.MenuSettings, false, false, ()=> Main.ChangeOutlineColor()),
            new Button("Current Menu Sound", Category.MenuSettings, false, false, ()=> Main.ChangeSound()),
            
            // Gun Settings
            new Button("Gun Example", Category.GunSettings, true, false, () => GunTemplate.GunExample()),
            new Button("Change Gun Colors []", Category.GunSettings, false, false, () => GunTemplate.ChangeTheme()),
            new Button("Change Line Width []", Category.GunSettings, false, false, () => GunTemplate.ChangeLineWidth()),
            new Button("Change Gun Line []", Category.GunSettings, false, false, () => GunTemplate.ChangeLineStyle()),

            // Room
            new Button("Check If Master []", Category.Room, false, false, ()=> Tools.IsMasterCheck()),

            // Saftey

            // Movement

            // Player

            // Advantage
            new Button("Tag Gun", Category.Advantage, true, false, () => TagGun()),
            new Button("Tag All", Category.Advantage, true, false, () => TagAll()),
            new Button("Tag Self", Category.Advantage, true, false, () => TagSelf()),

            // Visuals

            // Projectiles

            // Overpowered
            new Button("Master", Category.Overpowered, false, false, () => ChangePage(Category.Master)),
            new Button("Scitzo Gun", Category.Overpowered, true, false, () => ScitzoGun()),
            new Button("Reverse Scitzo Gun", Category.Overpowered, true, false, () => ReverseScitzoGun()),
            new Button("Lag All", Category.Overpowered, true, false, () => Lag(0)),

            // Master
            new Button("Back", Category.Master, false, false, () => ChangePage(Category.Overpowered)),
            new Button("Zero Gravity All", Category.Master, true, false, () => ZeroGravityAll(true), ()=> ZeroGravityAll(false)),
            new Button("Zero Gravity Others", Category.Master, true, false, () => ZeroGravityOthers()),
            new Button("Zero Gravity Gun", Category.Master, true, false, () => ZeroGravityGun()),
            new Button("Grey Screen All", Category.Master, true, false, () => GreyScreenAll(), ()=> ZeroGravityAll(false)),
            new Button("Grey Screen Others", Category.Master, true, false, () => GreyScreenOthers()),
            new Button("Grey Screen Gun", Category.Master, true, false, () => GreyScreenGun()),
           
            // SoundBoard Category
            new Button("Play Alarm", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/SoundBoard/main/alarm!!!-made-with-Voicemod.mp3", "alarm!!!-made-with-Voicemod.mp3"), false)),
            new Button("Play Amogus", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/SoundBoard/main/amogus-made-with-Voicemod.mp3", "amogus-made-with-Voicemod.mp3"), false)),
            new Button("Play Augh", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/SoundBoard/main/augh-made-with-Voicemod.mp3", "augh-made-with-Voicemod.mp3"), false)),
            new Button("Play Ben No", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/SoundBoard/main/ben-no-made-with-Voicemod.mp3", "ben-no-made-with-Voicemod.mp3"), false)),
            new Button("Play Bruh", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/SoundBoard/main/bruh-made-with-Voicemod.mp3", "bruh-made-with-Voicemod.mp3"), false)),
            new Button("Play Chipi Chipi Chapa Chapa", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/SoundBoard/main/chipi-chipi-chapa-chapa-made-with-Voicemod.mp3", "chipi-chipi-chapa-chapa-made-with-Voicemod.mp3"), false)),
            new Button("Play Clapping", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/SoundBoard/main/clapping-made-with-Voicemod.mp3", "clapping-made-with-Voicemod.mp3"), false)),
            new Button("Play Czy To Freddy Fazbear", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/SoundBoard/main/czy-to-freddy-fazbear-made-with-Voicemod.mp3", "czy-to-freddy-fazbear-made-with-Voicemod.mp3"), false)),
            new Button("Play Discord In", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/SoundBoard/main/discord-in-made-with-Voicemod.mp3", "discord-in-made-with-Voicemod.mp3"), false)),
            new Button("Play Emotional Damage", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/SoundBoard/main/emotional-damage-made-with-Voicemod.mp3", "emotional-damage-made-with-Voicemod.mp3"), false)),
            new Button("Play Erm What The Sigma", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/SoundBoard/main/erm-what-the-sigma-made-with-Voicemod.mp3", "erm-what-the-sigma-made-with-Voicemod.mp3"), false)),
            new Button("Play Fart", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/SoundBoard/main/fart-made-with-Voicemod.mp3", "fart-made-with-Voicemod.mp3"), false)),
            new Button("Play FBI Open Up", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/SoundBoard/main/fbi-open-up!-(sound-effect)-made-with-Voicemod.mp3", "fbi-open-up!-(sound-effect)-made-with-Voicemod.mp3"), false)),
            new Button("Play Get Out", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/SoundBoard/main/get-out-made-with-Voicemod.mp3", "get-out-made-with-Voicemod.mp3"), false)),
            new Button("Play Giga Chad", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/SoundBoard/main/giga-chad-made-with-Voicemod.mp3", "giga-chad-made-with-Voicemod.mp3"), false)),
            new Button("Play Hawk Tuah", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/SoundBoard/main/hawk-tuah_SRaUp2L.mp3", "hawk-tuah_SRaUp2L.mp3"), false)),
            new Button("Play Holy Moly", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/SoundBoard/main/holy-moly-made-with-Voicemod.mp3", "holy-moly-made-with-Voicemod.mp3"), false)),
            new Button("Play Lego Yoda Death", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/SoundBoard/main/lego-yoda-death-made-with-Voicemod.mp3", "lego-yoda-death-made-with-Voicemod.mp3"), false)),
            new Button("Play Mario Jump", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/SoundBoard/main/mario-jump-made-with-Voicemod.mp3", "mario-jump-made-with-Voicemod.mp3"), false)),
            new Button("Play Metal Pipe Falling", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/SoundBoard/main/metal-pipe-falling-sound-effect-made-with-Voicemod.mp3", "metal-pipe-falling-sound-effect-made-with-Voicemod.mp3"), false)),
            new Button("Play Monkey", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/SoundBoard/main/monkey-made-with-Voicemod.mp3", "monkey-made-with-Voicemod.mp3"), false)),
            new Button("Play Oi Oi Oi", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/SoundBoard/main/oi-oi-oi-made-with-Voicemod.mp3", "oi-oi-oi-made-with-Voicemod.mp3"), false)),
            new Button("Play Rick Roll", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/SoundBoard/main/rick-roll-made-with-Voicemod.mp3", "rick-roll-made-with-Voicemod.mp3"), false)),
            new Button("Play Rizz Sound Effect", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/SoundBoard/main/rizz-made-with-Voicemod.mp3", "rizz-made-with-Voicemod.mp3"), false)),
            new Button("Play Roblox Bye", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/SoundBoard/main/roblox-bye-made-with-Voicemod.mp3", "roblox-bye-made-with-Voicemod.mp3"), false)),
            new Button("Play Spongebob", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/SoundBoard/main/spongebob-made-with-Voicemod.mp3", "spongebob-made-with-Voicemod.mp3"), false)),
            new Button("Play Sus", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/SoundBoard/main/sus-made-with-Voicemod.mp3", "sus-made-with-Voicemod.mp3"), false)),
            new Button("Play Taco Bell Bell", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/SoundBoard/main/taco-bell-bell-made-with-Voicemod.mp3", "taco-bell-bell-made-with-Voicemod.mp3"), false)),
            new Button("Play Two Hours Later", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/SoundBoard/main/two-hours-later-made-with-Voicemod.mp3", "two-hours-later-made-with-Voicemod.mp3"), false)),
            new Button("Play Uhh No", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/SoundBoard/main/uhh-no-made-with-Voicemod.mp3", "uhh-no-made-with-Voicemod.mp3"), false)),
            new Button("Play Uwu", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/SoundBoard/main/uwu-made-with-Voicemod.mp3", "uwu-made-with-Voicemod.mp3"), false)),
            new Button("Play Vine Boom", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/SoundBoard/main/vine-boom-made-with-Voicemod.mp3", "vine-boom-made-with-Voicemod.mp3"), false)),
            new Button("Play What The Sigma", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/SoundBoard/main/what-the-sigma-made-with-Voicemod.mp3", "what-the-sigma-made-with-Voicemod.mp3"), false)),
            new Button("Play Why Are You Gay", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/SoundBoard/main/why-are-you-gay-made-with-Voicemod.mp3", "why-are-you-gay-made-with-Voicemod.mp3"), false)),
            new Button("Play Womp Womp", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/SoundBoard/main/womp-womp-made-with-Voicemod.mp3", "womp-womp-made-with-Voicemod.mp3"), false)),
            new Button("Play Yipee", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/SoundBoard/main/yipee-made-with-Voicemod.mp3", "yipee-made-with-Voicemod.mp3"), false)),
            new Button("Play daisy-bell-slowed", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/GhostSounds/main/daisy-bell-slowed-made-with-Voicemod.mp3", "daisy-bell-slowed-made-with-Voicemod.mp3"), false)),
            new Button("Play daisy09-gorilla-tag", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/GhostSounds/main/daisy09-gorilla-tag-made-with-Voicemod.mp3", "daisy09-gorilla-tag-made-with-Voicemod.mp3"), false)),
            new Button("Play distorted-run-rabbit-run", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/GhostSounds/main/distorted-run-rabbit-run-made-with-Voicemod.mp3", "distorted-run-rabbit-run-made-with-Voicemod.mp3"), false)),
            new Button("Play footsteps-sound-effect", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/GhostSounds/main/footsteps-sound-effect-made-with-Voicemod.mp3", "footsteps-sound-effect-made-with-Voicemod.mp3"), false)),
            new Button("Play j3vu-message", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/GhostSounds/main/j3vu-message-made-with-Voicemod.mp3", "j3vu-message-made-with-Voicemod.mp3"), false)),
            new Button("Play pbbv-warningbot", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/GhostSounds/main/pbbv-warningbot-made-with-Voicemod.mp3", "pbbv-warningbot-made-with-Voicemod.mp3"), false)),
            new Button("Play run-rabbit-run", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/GhostSounds/main/run-rabbit-run-made-with-Voicemod.mp3", "run-rabbit-run-made-with-Voicemod.mp3"), false)),
            new Button("Play run-run-run", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/GhostSounds/main/run-run-run-made-with-Voicemod.mp3", "run-run-run-made-with-Voicemod.mp3"), false)),
            new Button("Play t774-bells", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/GhostSounds/main/t774-bells-made-with-Voicemod.mp3", "t774-bells-made-with-Voicemod.mp3"), false)),
            new Button("Play t774-speech-1", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/GhostSounds/main/t774-speech-made-with-Voicemod (1).mp3", "t774-speech-made-with-Voicemod (1).mp3"), false)),
            new Button("Play t774-speech-2", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/GhostSounds/main/t774-speech-made-with-Voicemod.mp3", "t774-speech-made-with-Voicemod.mp3"), false)),
            new Button("Play tip-toe-warning-bot", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/GhostSounds/main/tip-toe-warning-bot-made-with-Voicemod.mp3", "tip-toe-warning-bot-made-with-Voicemod.mp3"), false)),
            new Button("Play virus-twin1-sound", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/GhostSounds/main/virus-twin1-sound-made-with-Voicemod.mp3", "virus-twin1-sound-made-with-Voicemod.mp3"), false)),
            new Button("Play wi-crash-sound", Category.SoundBoard, false, false, () => AudioManager.PlaySoundThroughMicrophone(AudioManager.LoadSoundFromURL("https://raw.githubusercontent.com/TortiseWay2Cool/GhostSounds/main/wi-crash-sound-(better-&-louder)-made-with-Voicemod.mp3", "wi-crash-sound-(better-&-louder)-made-with-Voicemod.mp3"), false)),



            };
    }
}
