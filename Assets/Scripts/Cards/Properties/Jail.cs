using System.Collections;

namespace PimPamPum
{
    public class Jail : Property
    {
        public static bool CheckCondition(Card c)
        {
            return c.Suit == Suit.Hearts;
        }

        public override void BeginCardDrag(PlayerController pc)
        {
            base.BeginCardDrag(pc);
            pc.JailBeginCardDrag();
        }

        public override void AddPropertyEffect(PlayerController pc)
        {
            pc.EquipJail();
        }

        public override void RemovePropertyEffect(PlayerController pc)
        {
            pc.UnequipJail();
        }

        protected override IEnumerator EquipTrigger(PlayerController pc)
        {
            yield return pc.Equip<Jail>(this);
        }

        public override string ToString()
        {
            return "Jail";
        }
    }
}