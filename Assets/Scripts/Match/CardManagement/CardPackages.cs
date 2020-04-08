using System.Collections.Generic;

namespace Assets.Scripts.Match.CardManagement
{
    public class CardPackages
    {
        public static Dictionary<string, CardPack> Packages = new Dictionary<string, CardPack>
        {
            { "KitchenPack", new CardPack("KitchenPack", 2, 2) },
            { "ArtPack", new CardPack("ArtPack", 5, 6) },
            { "OceanPack", new CardPack("OceanPack", 5, 6) },
        };
    }
}
