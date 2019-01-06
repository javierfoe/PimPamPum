using UnityEngine;
using UnityEngine.UI;

public class EndGamePanelView : MonoBehaviour {

    private Text endGameText;

	// Use this for initialization
	void Awake () {
        endGameText = GetComponentInChildren<Text>();
	}

    public void Win()
    {
        SetText("You WIN", Color.green);
    }

    public void Lose()
    {
        SetText("You LOSE", Color.red);
    }

    private void SetText(string text, Color color)
    {
        gameObject.SetActive(true);
        endGameText.text = text;
        endGameText.color = color;
    }
}
