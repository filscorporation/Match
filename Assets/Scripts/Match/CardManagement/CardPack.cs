using System;
using System.Collections.Generic;

namespace Assets.Scripts.Match.CardManagement
{
    /// <summary>
    /// Card package parameters
    /// </summary>
    [Serializable]
    public class CardPack
    {
        public readonly string Name;

        public readonly int MaxWidth;

        public readonly int MaxHeight;

        public CardPack(string name, int maxWidth, int maxHeight)
        {
            Name = name;
            MaxWidth = maxWidth;
            MaxHeight = maxHeight;
        }

        public override bool Equals(object obj)
        {
            if (obj is CardPack cardPack)
                return cardPack.Name == Name;

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            var hashCode = -312013121;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + MaxWidth.GetHashCode();
            hashCode = hashCode * -1521134295 + MaxHeight.GetHashCode();
            return hashCode;
        }
    }
}
