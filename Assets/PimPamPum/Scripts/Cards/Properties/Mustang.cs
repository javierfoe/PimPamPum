using System.Collections;
namespace PimPamPum
{
    public class Mustang : Property
    {
        public override void BeginCardDrag(PlayerController pc)
        {
            base.BeginCardDrag(pc);
            pc.SelfTargetPropertyCard<Mustang>();
        }

        public override void AddPropertyEffect(PlayerController pc)
        {
            pc.EquipMustang();
        }

        public override void RemovePropertyEffect(PlayerController pc)
        {
            pc.UnequipMustang();
        }

        protected override IEnumerator EquipTrigger(PlayerController pc)
        {
            yield return pc.Equip<Mustang>(this);
        }

        public override IEnumerator CardUsed(PlayerController pc)
        {
            yield return GameController.UsedCard<Mustang>(pc);
        }

        public override string ToString()
        {
            return "Mustang";
        }
    }
}