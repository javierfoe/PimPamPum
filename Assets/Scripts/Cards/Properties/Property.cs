using System.Collections;
using UnityEngine;
namespace PimPamPum
{
    public abstract class Property : Card
    {
        private static Color property = Color.blue;

        public override Color Color => property;

        protected override IEnumerator CardEffect(PlayerController pc, int player, Drop drop, int cardIndex)
        {
            pc.UnequipDraggedCard();
            pc.EquipPropertyTo(player, this);
            yield return EquipTrigger(pc);
        }

        public virtual void EquipProperty(PlayerController pc)
        {
            pc.EquipProperty(this);
            AddPropertyEffect(pc);
        }

        public virtual void AddPropertyEffect(PlayerController pc) { }

        public virtual void RemovePropertyEffect(PlayerController pc) { }

        protected virtual IEnumerator EquipTrigger(PlayerController pc) { yield return null; }
    }
}