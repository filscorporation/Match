using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Match.CardManagement
{
    public class CardPackages
    {
        public static Dictionary<string, CardPack> Packages = new Dictionary<string, CardPack>
        {
            { "KitchenPack", new CardPack("KitchenPack", 5, 6, Color.white) },
            { "ArtPack", new CardPack("ArtPack", 5, 6, Color.black) },
            { "OceanPack", new CardPack("OceanPack", 5, 6, Color.white) },
            { "WeaponPack", new CardPack("WeaponPack", 5, 6, Color.black) },
        };
    }
}
