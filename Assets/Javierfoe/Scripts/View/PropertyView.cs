using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Bang
{
    public class PropertyView : CardView
    {
        protected override void Start()
        {
            base.Start();
            drop = Drop.Properties;
        }
    }
}