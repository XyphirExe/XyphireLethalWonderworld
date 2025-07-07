using BepInEx.Configuration;
using System.Linq;

namespace XyphireLethalWonderworld
{
    class Config
    {
        public readonly ConfigEntry<int> dogtoyRarity;
        public readonly ConfigEntry<string> dogtoyValue;
        public readonly ConfigEntry<float> dogtoyInterestTime;
        public (int, int) dogtoyValueParsed;

        public Config(ConfigFile cfg)
        {
            cfg.SaveOnConfigSet = false;
            dogtoyRarity = cfg.Bind("Dog Toy", "Rarity", 25, "The spawn chance of the Dog Toy item.");
            dogtoyValue = cfg.Bind("Dog Toy", "Scrap value", "10,40", "The min,max scrap value of the Dog Toy item. (the final value will be randomized between these 2 numbers)");
            dogtoyInterestTime = cfg.Bind("Dog Toy", "Interest time", 10f, "The time in seconds the Dog Toy will keep the interest of Eyeless Dogs when thrown on the ground.");
            cfg.Save();
            cfg.SaveOnConfigSet = true;
        }

        public void SetupCustomConfigs()
        {
            ParseValues();
        }

        private void ParseValues()
        {
            if (dogtoyValue.Value == "")
            { dogtoyValueParsed = (-1, -1); return; }
            var valueTab = dogtoyValue.Value.Split(',').Select(s => s.Trim()).ToArray();
            if (valueTab.Count() != 2)
            { dogtoyValueParsed = (-1, -1); return; }
            if (!int.TryParse(valueTab[0], out var minV) || !int.TryParse(valueTab[1], out var maxV))
            { dogtoyValueParsed = (-1, -1); return; }
            if (minV > maxV)
            { dogtoyValueParsed = (-1, -1); return; }
            dogtoyValueParsed = (minV, maxV);
        }
    }
}
