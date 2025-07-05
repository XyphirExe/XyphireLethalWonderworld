using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using BepInEx;
using LethalLib.Modules;

namespace XyphireLethalWonderworld
{
    [BepInPlugin(GUID,NAME,VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        const string GUID = "Xyphire.XyphireLethalWonderworld";
        const string NAME = "Xyphire Lethal Wonderworld";
        const string VERSION = "0.1.1";

        public static Plugin instance;

        void Awake()
        {
            instance = this;

            string assetDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "xyphirelethalwonderworld");
            AssetBundle bundle = AssetBundle.LoadFromFile(assetDir);

            var dogtoy = bundle.LoadAsset<Item>("Assets/MyItems/Baton/SO_Baton.asset");

            NetworkPrefabs.RegisterNetworkPrefab(dogtoy.spawnPrefab);

            Utilities.FixMixerGroups(dogtoy.spawnPrefab);

            Items.RegisterScrap(dogtoy, 25, Levels.LevelTypes.All);

            Logger.LogInfo(NAME + " loaded.");
        }
    }
}
