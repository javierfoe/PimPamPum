using UnityEngine;
using System.Collections;
namespace Bang
{
    public class WeaponView : CardView
    {
        protected override void Start()
        {
            base.Start();
            eDrop = EDrop.WEAPON;
        }
    }
}
