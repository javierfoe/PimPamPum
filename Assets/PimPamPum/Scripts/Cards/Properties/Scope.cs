using System.Collections;

namespace PimPamPum
{
    public class Scope : Property
    {
        public override void BeginCardDrag(PlayerController pc)
        {
            base.BeginCardDrag(pc);
            pc.SelfTargetPropertyCard<Scope>();
        }

        public override void AddPropertyEffect(PlayerController pc)
        {
            pc.EquipScope();
        }

        public override void RemovePropertyEffect(PlayerController pc)
        {
            pc.UnequipScope();
        }

        protected override IEnumerator EquipTrigger(PlayerController pc)
        {
            yield return pc.Equip<Scope>(this);
        }
        
        public override IEnumerator CardUsed(PlayerController pc)
        {
            yield return GameController.Instance.UsedCard<Scope>(pc);
        }

        public override string ToString()
        {
            return "Binoculars";
        }
    }
}