using System;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Card package parameters
    /// </summary>
    [Serializable]
    public class CardPack
    {
        public string Name;

        public int MaxWidth;

        public int MaxHeigth;

        public CardPack(string name, int maxWidth, int maxHeigth)
        {
            Name = name;
            MaxWidth = maxWidth;
            MaxHeigth = maxHeigth;
        }

        public override bool Equals(object obj)
        {
            if (obj is CardPack cardPack)
                return cardPack.Name == Name;

            return base.Equals(obj);
        }
    }
}
