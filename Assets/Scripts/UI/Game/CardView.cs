using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PimPamPum
{
    public class CardView : DropView, ICardView, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        private readonly string[] Suits = { "", "S", "H", "D", "C" };
        private readonly string[] Ranks = { "", "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
        public static CardView CurrentCardView;
        private static RectTransform canvas;

        [SerializeField] private Color playable = new Color();
        [SerializeField] private Text cardName = null, suit = null, rank = null;

        protected int index;
        private bool selectable;
        private DropView currentDropView;
        private GameObject ghostCard;
        private MaskableGraphic[] maskableGraphics;

        public override int DropIndex => index;

        protected bool Draggable
        {
            get; set;
        }

        public void Selectable(bool value)
        {
            base.EnableClick(value);
            Playable(value);
            if (value)
            {
                SetBackgroundColor(click);
            }
            selectable = true;
            Draggable = false;
        }

        public override void EnableClick(bool value)
        {
            base.EnableClick(value);
            SetBackgroundColor(value ? click : idle);
        }

        public override void Click()
        {
            if (selectable)
                PlayerController.LocalPlayer.ChooseCard(index);
            else
                PlayerController.LocalPlayer.PhaseOneDecision(Decision.Player, PlayerNumber, DropEnum, DropIndex);
        }

        public virtual void Playable(bool value)
        {
            PlayableColor(value);
            Draggable = value;
        }

        protected override Color GetColor(bool value)
        {
            return value ? this.highlight : Droppable ? target : Draggable ? playable : idle;
        }

        private void PlayableColor(bool value)
        {
            SetBackgroundColor(value ? playable : idle);
        }

        public void SetIndex(int index)
        {
            this.index = index;
        }

        public void SetCard(CardStruct cs)
        {
            SetName(cs.name, cs.color);
            SetRank(cs.rank);
            SetSuit(cs.suit);
        }

        private void SetName(string name, Color color)
        {
            cardName.color = color;
            cardName.text = name;
        }

        private void SetRank(Rank rank)
        {
            this.rank.text = Ranks[(int)rank];
        }

        private void SetSuit(Suit suit)
        {
            this.suit.text = Suits[(int)suit];
        }

        private void SetVisibility(bool value)
        {
            foreach (MaskableGraphic t in maskableGraphics) t.enabled = value;
        }

        protected override void Awake()
        {
            base.Awake();
            drop = Drop.Hand;
            if (!canvas) canvas = FindObjectOfType<Canvas>().transform as RectTransform;
            maskableGraphics = GetComponentsInChildren<MaskableGraphic>();
        }

        public void Empty()
        {
            SetName("", Color.black);
            SetRank(0);
            SetSuit(0);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!Draggable) return;
            CurrentCardView = this;
            PlayerController.LocalPlayer.BeginCardDrag(index);
            PlayableColor(false);
            CreateGhostCard(eventData);
            SetVisibility(false);
        }

        private void CreateGhostCard(PointerEventData eventData)
        {
            ghostCard = Instantiate(gameObject, canvas.transform);

            RectTransform rt = ghostCard.transform as RectTransform;
            rt.sizeDelta = (transform as RectTransform).sizeDelta;

            MaskableGraphic[] graphics = ghostCard.GetComponentsInChildren<MaskableGraphic>();
            foreach (MaskableGraphic mg in graphics) mg.raycastTarget = false;

            ghostCard.transform.SetAsLastSibling();

            SetDraggedPosition(eventData);
        }

        private void SetDraggedPosition(PointerEventData data)
        {
            var rt = ghostCard.GetComponent<RectTransform>();
            Vector3 globalMousePos;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas, data.position, data.pressEventCamera, out globalMousePos))
            {
                rt.position = globalMousePos;
                rt.rotation = canvas.rotation;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!Draggable) return;

            DropView drop = null;
            List<GameObject> hovered = eventData.hovered;
            GameObject hover;
            for (int i = 0; i < hovered.Count && drop == null; i++)
            {
                hover = hovered[i];
                drop = hover.GetComponent<DropView>();
                drop = (drop != null && drop != this && drop.Droppable) ? drop : null;
            }

            if (currentDropView != null && drop != currentDropView)
            {
                currentDropView.Highlight(false);
            }

            currentDropView = null;
            if (drop != null && drop.Droppable)
            {
                currentDropView = drop;
                drop.Highlight(true);
            }

            SetDraggedPosition(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!Draggable) return;
            OnEndDrag();
        }

        public void OnEndDrag()
        {
            int player = -1;
            Drop drop = Drop.Nothing;
            int targetIndex = -1;
            if (currentDropView != null)
            {
                drop = currentDropView.DropEnum;
                targetIndex = currentDropView.DropIndex;
                currentDropView.Highlight(false);
                player = currentDropView.PlayerNumber;
            }

            PlayableColor(true);

            PlayerController.LocalPlayer.UseCard(index, player, drop, targetIndex);
            currentDropView = null;

            SetVisibility(true);
            Destroy(ghostCard);
            CurrentCardView = null;
        }
    }
}