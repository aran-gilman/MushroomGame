using UnityEngine;
using UnityEngine.UI;

public class HighScoreTracker : MonoBehaviour
{

    public Text bestScoreText;
    public Text lastScoreText;

    public static int LastScore
    {
        get => lastScore;
        set
        {
            lastScore = value;
            if (lastScore > BestScore)
            {
                BestScore = lastScore;
            }
        }
    }

    // Store the best score as a playerpref for simplicity's sake
    public static int BestScore
    {
        get => PlayerPrefs.GetInt("BestScore", 0);
        private set
        {
            PlayerPrefs.SetInt("BestScore", value);
            PlayerPrefs.Save();
        }
    }

    private static int lastScore = 0;

    private void OnEnable()
    {
        bestScoreText.text = string.Format(bestScoreText.text, BestScore);
        lastScoreText.text = string.Format(lastScoreText.text, LastScore);
    }
}
