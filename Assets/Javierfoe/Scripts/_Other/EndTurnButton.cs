using UnityEngine;
using UnityEngine.UI;

namespace Bang
{

    public class EndTurnButton : MonoBehaviour
    {

        public bool Active
        {
            set
            {
                gameObject.SetActive(value);
            }
        }

        // Use this for initialization
        void Start()
        {
            Button button = GetComponent<Button>();
            button.onClick.AddListener(() => PlayerController.LocalPlayer.EndTurn());
        }
    }
}