using System.Collections;

namespace PimPamPum
{
    public class Dynamite : Property, ICondition
    {
        public override void BeginCardDrag(PlayerController pc)
        {
            base.BeginCardDrag(pc);
            pc.SelfTargetPropertyCard<Dynamite>();
        }

        public override void AddPropertyEffect(PlayerController pc)
        {
            pc.EquipDynamite();
        }

        public override void RemovePropertyEffect(PlayerController pc)
        {
            pc.UnequipDynamite();
        }

        protected override IEnumerator EquipTrigger(PlayerController pc)
        {
            yield return pc.Equip<Dynamite>(this);
        }

        public bool CheckCondition(Card c)
        {
            return c.Suit == Suit.Spades && c.Rank <= Rank.Nine && c.Rank >= Rank.Two;
        }

        public override string ToString()
        {
            return "Dynamite";
        }
    }
}