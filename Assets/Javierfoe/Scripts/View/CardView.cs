using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Bang
{
    public class CardView : DropView, ICardView, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        private readonly string[] Suits = { "", "S", "H", "D", "C" };
        private readonly string[] Ranks = { "", "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

        [SerializeField] private Text cardName = null, suit = null, rank = null;

        protected int index;
        private bool draggable;
        private DropView currentDropView;
        private PlayerView currentPlayerView;

        public override int GetDropIndex()
        {
            return index;
        }
        
        public void Playable(bool value)
        {
            draggable = value;
        }

        public void SetIndex(int index)
        {
            this.index = index;
        }

        public void SetName(string name, Color color)
        {
            cardName.color = color;
            cardName.text = name;
        }

        public void SetRank(Rank rank)
        {
            this.rank.text = Ranks[(int)rank];
        }

        public void SetSuit(Suit suit)
        {
            this.suit.text = Suits[(int)suit];
        }

        protected override void Start()
        {
            base.Start();
            drop = Drop.Hand;
        }

        public void Empty()
        {
            SetName("", Color.black);
            SetRank(0);
            SetSuit(0);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!draggable) return;
            PlayerController.LocalPlayer.BeginCardDrag(index);
            Highlight(true);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!draggable) return;

            DropView drop = null;
            PlayerView pv = null;
            List<GameObject> hovered = eventData.hovered;
            GameObject hover;
            for (int i = 0; i < hovered.Count && drop == null; i++)
            {
                hover = hovered[i];
                drop = hover.GetComponent<DropView>();
                drop = ( drop != null && drop != this && drop.Droppable) ? drop : null;
            }
            if (drop != null)
            {
                for (int i = 0; i < hovered.Count && pv == null; i++)
                {
                    hover = hovered[i];
                    pv = hover.GetComponent<PlayerView>();
                }
            }

            if (currentDropView != null && drop != currentDropView)
            {
                currentDropView.Highlight(false);
            }
            if (currentPlayerView != null && pv != currentPlayerView)
            {
                currentPlayerView.Highlight(false);
            }

            currentDropView = null;
            if (drop != null && drop.Droppable)
            {
                currentDropView = drop;
                drop.Highlight(true);
            }
            currentPlayerView = null;
            if (pv != null)
            {
                currentPlayerView = pv;
                pv.Highlight(true);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!draggable) return;
            Highlight(false);
            int player = -1;
            Drop drop = Drop.Nothing;
            int targetIndex = -1;
            if (currentDropView != null)
            {
                drop = currentDropView.GetDropEnum();
                targetIndex = currentDropView.GetDropIndex();
                currentDropView.Highlight(false);
            }
            if (currentPlayerView != null)
            {
                player = currentPlayerView.GetPlayerIndex();
                currentPlayerView.Highlight(false);
            }
            PlayerController.LocalPlayer.UseCard(index, player, drop, targetIndex);
            currentPlayerView = null;
            currentDropView = null;
        }
    }
}