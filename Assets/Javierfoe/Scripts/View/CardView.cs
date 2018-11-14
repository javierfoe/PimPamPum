using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Bang
{
    public class CardView : DropView, ICardView, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        private readonly string[] SUITS = { "", "S", "H", "D", "C" };
        private readonly string[] RANKS = { "", "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

        [SerializeField] private Text cardName = null, suit = null, rank = null;

        private int index;
        private bool draggable, discardable;
        private DropView currentDropView;
        private PlayerView currentPlayerView;

        public IPlayerView PlayerView
        {
            private get; set;
        }

        public void Playable(bool value)
        {
            draggable = value;
        }

        public void Discardable(bool value)
        {
            discardable = value;
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

        public void SetRank(ERank rank)
        {
            this.rank.text = RANKS[(int)rank];
        }

        public void SetSuit(ESuit suit)
        {
            this.suit.text = SUITS[(int)suit];
        }

        public void Empty()
        {
            SetName("", Color.black);
            SetRank(0);
            SetSuit(0);
        }

        public int GetPlayerIndex()
        {
            return PlayerView.GetPlayerIndex();
        }

        public void SetPlayerView(IPlayerView playerView)
        {
            PlayerView = playerView;
        }

        public override int GetDropEnum()
        {
            return index;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!draggable && !discardable) return;
            if (draggable)
            {
                PlayerController.LocalPlayer.BeginCardDrag(index);
            }
            Highlight(true);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!draggable && !discardable) return;

            DropView drop = null;
            PlayerView pv = null;
            List<GameObject> hovered = eventData.hovered;
            GameObject hover;
            for (int i = 0; i < hovered.Count && drop == null; i++)
            {
                hover = hovered[i];
                drop = hover.GetComponent<DropView>();
                drop = (drop != null && drop.Droppable) ? drop : null;
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

            if (drop != null && drop.Droppable)
            {
                //Debug.Log("Drag: " + drop.gameObject.name, drop.gameObject);
                currentDropView = drop;
                drop.Highlight(true);
            }
            if (pv != null)
            {
                currentPlayerView = pv;
                pv.Highlight(true);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!draggable && !discardable) return;
            Highlight(false);
            if (draggable && currentPlayerView != null)
            {
                PlayerController.LocalPlayer.PlayCard(currentPlayerView.GetPlayerIndex(), currentDropView.GetDropEnum());
            }
            else if (discardable)
            {
                PlayerController.LocalPlayer.DiscardCard(index);
            }
            PlayerController.LocalPlayer.EndCardDrag();
            if (currentDropView != null)
            {
                currentDropView.Highlight(false);
                currentDropView = null;
            }
            if (currentPlayerView != null)
            {
                currentPlayerView.Highlight(false);
                currentPlayerView = null;
            }
        }
    }
}