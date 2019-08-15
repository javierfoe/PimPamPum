using System.Collections;

namespace PimPamPum
{
    public class Barrel : Property, ICondition
    {
        public override void BeginCardDrag(PlayerController pc)
        {
            base.BeginCardDrag(pc);
            pc.SelfTargetPropertyCard<Barrel>();
        }

        public override void AddPropertyEffect(PlayerController pc)
        {
            pc.EquipBarrel();
        }

        public override void RemovePropertyEffect(PlayerController pc)
        {
            pc.UnequipBarrel();
        }

        public bool CheckCondition(Card c)
        {
            return c.Suit == Suit.Hearts;
        }

        protected override IEnumerator EquipTrigger(PlayerController pc)
        {
            yield return pc.Equip<Barrel>(this);
        }

        public override string ToString()
        {
            return "Barrel";
        }
    }
}