using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloorScoreTimeManager : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] protected Text floorText;
    [SerializeField] protected Text scoreText;
    [SerializeField] protected Text timeText;

    protected int floorNum;

    protected int trueScore;
    protected int displayScore;
    protected float scoreMultiplier = 1;
    protected const float MULTIPLIER_GROWTH = 0.2f;

    protected float time;

    //Score Bonus (when you finish a room, you get a bonus based on how fast you clear it)
    //Score Bonus = Max(0, ((ENEMY_EXPECTED_TIME * enemyCount) - time) * ENEMY_TIME_BONUS)
    protected int enemyCount;
    protected const int ENEMY_EXPECTED_TIME = 10; // time "expected" to defeat enemy * 2
    protected const int ENEMY_TIME_BONUS = 10; // points per second left
    protected const int DISPLAY_ROLL_SPEED = ENEMY_TIME_BONUS * 40;

    protected bool timerPaused = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    //private void Awake()
    //{
    //    DontDestroyOnLoad(this);
    //}

    // Update is called once per frame
    void Update()
    {
        if (!timerPaused)
        {
            time += Time.deltaTime;
            timeText.text = string.Format("{0:D0}:{1:D2}", (int)time / 60, (int)time % 60);
        }

        if (displayScore < trueScore)
        {
            //Debug.Log(trueScore);
            displayScore += Mathf.Max(1, (int)(DISPLAY_ROLL_SPEED * Time.unscaledDeltaTime * (timerPaused ? 0.1f : 1)));
            displayScore = Mathf.Min(displayScore, trueScore);
            scoreText.text = string.Format("{0:D6}", displayScore);
        }
    }

    public void SetEnemyCount(int count)
    {
        enemyCount = count;
    }

    /*********************
     * FLOOR NUM FUNCTIONS
     *********************/
    public void SetFloorNum(int f)
    {
        floorNum = f;
        floorText.text = f.ToString();
    }

    /*****************
     * SCORE FUNCTIONS
     *****************/
    public void AddToScore(int points)
    {
        trueScore += (int)(points * scoreMultiplier);
    }

    public void IncreaseScoreMultiplier()
    {
        scoreMultiplier += MULTIPLIER_GROWTH;
    }

    public void ApplyTimeBonus()
    {
        PauseTime();
        int timeBonus = (int)Mathf.Max(0, (ENEMY_EXPECTED_TIME * enemyCount) - time) * ENEMY_TIME_BONUS;
        Debug.Log("Time Bonus: " + timeBonus);
        AddToScore(timeBonus);
    }

    public int GetScore()
    {
        return trueScore;
    }

    /****************
     * TIME FUNCTIONS
     ****************/
    public void PauseTime()
    {
        timerPaused = true;
    }

    public void UnpauseTime()
    {
        timerPaused = false;
    }

    public void ResetTime(bool pause)
    {
        time = 0;
        timerPaused = pause;
    }
}
