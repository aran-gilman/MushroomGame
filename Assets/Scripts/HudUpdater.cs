using UnityEngine;
using UnityEngine.UI;

public class HudUpdater : MonoBehaviour
{
    public Text scoreText;
    public Text timeText;
    public Text nextMushroomText;

    private string scoreFormat;
    private string timeFormat;
    private string nextMushroomFormat;

    private float lastTimeUpdate;

    private void Start()
    {
        scoreFormat = scoreText.text;
        timeFormat = timeText.text;
        nextMushroomFormat = nextMushroomText.text;
        lastTimeUpdate = -1;
    }

    private void FixedUpdate()
    {
        scoreText.text = string.Format(
            scoreFormat, PlanterManager.PersistentData.totalPoints);
        nextMushroomText.text = string.Format(
            nextMushroomFormat, 0);
        if (Time.time - lastTimeUpdate >= 1)
        {
            timeText.text = string.Format(
                timeFormat, (int)PlanterManager.GameTimeRemaining());
            lastTimeUpdate = Time.time;
        }
    }
}
