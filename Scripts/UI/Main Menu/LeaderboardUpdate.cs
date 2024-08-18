using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardUpdate : MonoBehaviour
{
    [SerializeField] protected List<Text> scoreTexts;

    private void Awake()
    {
        Leaderboard leaderboard = new Leaderboard();
        string scoreJSON = PlayerPrefs.GetString("Leaderboard");

        if (!string.IsNullOrEmpty(scoreJSON))
        {
            int emptyScore = 0;
            JsonUtility.FromJsonOverwrite(scoreJSON, leaderboard);
            if (leaderboard.thirdPlace > 0)
            {
                scoreTexts[2].text = string.Format("{0:D6} pts.", leaderboard.thirdPlace);
                emptyScore++;

                if (leaderboard.secondPlace > 0)
                {
                    scoreTexts[1].text = string.Format("{0:D6} pts.", leaderboard.secondPlace);
                    emptyScore++;

                    if (leaderboard.firstPlace > 0)
                    {
                        scoreTexts[0].text = string.Format("{0:D6} pts.", leaderboard.firstPlace);
                        emptyScore++;
                    }
                }
            }

            for (int i = emptyScore; i < scoreTexts.Count; i++)
                scoreTexts[i].text = "------ pts.";
        }
        else
        {
            for (int i = 0; i < scoreTexts.Count; i++)
                scoreTexts[i].text = "------ pts.";
        }

        PlayerPrefs.SetString("Leaderboard", JsonUtility.ToJson(leaderboard));
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
