using BepInEx;
using BepInEx.Logging;
using LethalLib.Modules;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace XyphireLethalWonderworld
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        const string GUID = "Xyphire.XyphireLethalWonderworld";
        const string NAME = "Xyphire Lethal Wonderworld";
        const string VERSION = "0.1.1";

        public static Plugin instance;
        public static ManualLogSource logger;
        internal static Config config { get; private set; } = null!;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051")]
        void Awake()
        {
            instance = this;
            logger = Logger;
            config = new Config(Config);
            config.SetupCustomConfigs();

            string assetDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "xyphirelethalwonderworld");
            AssetBundle bundle = AssetBundle.LoadFromFile(assetDir);

            var dogtoy = bundle.LoadAsset<Item>("Assets/MyItems/Baton/SO_Baton.asset");

            NetworkPrefabs.RegisterNetworkPrefab(dogtoy.spawnPrefab);

            Utilities.FixMixerGroups(dogtoy.spawnPrefab);

            if (config.dogtoyValueParsed.Item1 != -1)
            {
                dogtoy.minValue = (int)(config.dogtoyValueParsed.Item1 * 2.5f);
                dogtoy.maxValue = (int)(config.dogtoyValueParsed.Item2 * 2.5f);
            }

            Items.RegisterScrap(dogtoy, config.dogtoyRarity.Value, Levels.LevelTypes.All);

            logger.LogInfo(NAME + " loaded.");
        }
    }
}
