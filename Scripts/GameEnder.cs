using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameEnder : MonoBehaviour
{
    protected PlayerController player;

    [Header("Pieces")]
    [SerializeField] protected Text gameText;
    [SerializeField] protected Text overText;
    [SerializeField] protected GameObject scoreHolder;
    [SerializeField] protected ButtonColorManipulation tryAgainButton;
    [SerializeField] protected ButtonColorManipulation quitButton;

    [Header("Positions")]
    [SerializeField] protected Vector2 gameTextFinalPosition;
    [SerializeField] protected Vector2 overTextFinalPosition;
    protected Vector2 gameTextInitialPosition;
    protected Vector2 overTextInitialPosition;

    //[Header("Score Texts")]
    [SerializeField] protected List<Text> scoreTexts; //first, second, third, currentRun
    [SerializeField] protected GameObject screenBars;
    protected FloorScoreTimeManager scoreManager;

    protected const float PLAYER_ROTATE_SPEED = 2880;
    protected const float GAME_OVER_DELAY = 1f;
    //protected float delayTimer = 0;
    protected const float PIECE_TRANSITION_TIME = 60f / 84;
    protected float transitionTimer = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        scoreManager = FindObjectOfType<FloorScoreTimeManager>();

        gameTextInitialPosition = gameText.rectTransform.anchoredPosition;
        overTextInitialPosition = overText.rectTransform.anchoredPosition;

        tryAgainButton.GetComponent<Button>().interactable = false;
        tryAgainButton.transform.localScale = Vector3.zero;
        quitButton.GetComponent<Button>().interactable = false;
        quitButton.transform.localScale = Vector3.zero;
    }

    public void BeginGameOverSequence()
    {
        StartCoroutine("GameOverSequence");
    }

    protected IEnumerator GameOverSequence()
    {
        //Debug.Log("um");
        scoreManager.PauseTime();
        scoreManager.gameObject.SetActive(false);
        screenBars.gameObject.SetActive(false);

        /***********************
         * Stun player for a bit
         ***********************/
        float t = 0;
        player.OverrideInvincibility(0);
        while (t < GAME_OVER_DELAY)
        {
            player.transform.Rotate(0, PLAYER_ROTATE_SPEED * Time.unscaledDeltaTime, 0);
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        /********************
         * Shrink the player
         ********************/
        t = 0;
        while (t < PIECE_TRANSITION_TIME * 2)
        {
            t += Time.unscaledDeltaTime;
            player.transform.Rotate(0, PLAYER_ROTATE_SPEED * Time.unscaledDeltaTime, 0);
            player.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t / (PIECE_TRANSITION_TIME * 2));
            yield return null;
        }

        /****************
         * gameText slide
         ****************/
        t = 0;
        while (t < PIECE_TRANSITION_TIME)
        {
            t += Time.unscaledDeltaTime;
            gameText.rectTransform.anchoredPosition = Vector2.Lerp(gameTextInitialPosition, gameTextFinalPosition, t / PIECE_TRANSITION_TIME);
            //Debug.Log("Game Text: " + gameText.rectTransform.anchoredPosition);
            yield return null;
        }
        //yield return new WaitForSecondsRealtime(PIECE_TRANSITION_TIME);

        /****************
         * overText slide
         ****************/
        t = 0;
        while (t < PIECE_TRANSITION_TIME)
        {
            t += Time.unscaledDeltaTime;
            overText.rectTransform.anchoredPosition = Vector2.Lerp(overTextInitialPosition, overTextFinalPosition, t / PIECE_TRANSITION_TIME);
            //Debug.Log("Over Text: " + overText.rectTransform.anchoredPosition);
            yield return null;
        }
        //yield return new WaitForSecondsRealtime(PIECE_TRANSITION_TIME);

        /***************
         * update scores
         ***************/
        // Read from JSON
        int currentScore = scoreManager.GetScore();
        Leaderboard leaderboard = new Leaderboard();
        string scoreJSON = PlayerPrefs.GetString("Leaderboard");

        if (!string.IsNullOrEmpty(scoreJSON)) //There are scores saved on the leaderboard
        {
            JsonUtility.FromJsonOverwrite(scoreJSON, leaderboard);

            if (leaderboard.GetPlaceOnLeaderboard(currentScore) <= 3)
            {
                scoreTexts[3].text = currentScore > 0 ? "High Score!" : "------ pts.";
                leaderboard.AddToLeaderboard(currentScore);
            }
            else
                scoreTexts[3].text = string.Format("{0:D6} pts.", currentScore);

            scoreTexts[0].text = leaderboard.firstPlace > 0 ? string.Format("{0:D6} pts.", leaderboard.firstPlace) : "------ pts.";
            scoreTexts[1].text = leaderboard.secondPlace > 0 ? string.Format("{0:D6} pts.", leaderboard.secondPlace) : "------ pts.";
            scoreTexts[2].text = leaderboard.thirdPlace > 0 ? string.Format("{0:D6} pts.", leaderboard.thirdPlace) : "------ pts.";
        }
        else // There are no scores saved on the leaderboard
        {
            leaderboard.AddToLeaderboard(currentScore);
            scoreTexts[0].text = currentScore > 0 ? string.Format("{0:D6} pts.", currentScore) : "------ pts.";
            for (int i = 1; i < 3; i++)
                scoreTexts[i].text = "------ pts.";
            scoreTexts[3].text = currentScore > 0 ? "High Score!" : "------ pts.";
        }
        PlayerPrefs.SetString("Leaderboard", JsonUtility.ToJson(leaderboard));
        PlayerPrefs.Save();
        yield return null;

        /******************
         * scoreHolder grow
         ******************/
        t = 0;
        while (t < PIECE_TRANSITION_TIME)
        {
            t += Time.unscaledDeltaTime;
            scoreHolder.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t / PIECE_TRANSITION_TIME);
            yield return null;
        }
        //yield return new WaitForSecondsRealtime(PIECE_TRANSITION_TIME);

        /***********************************
         * restartButton and quitButton grow
         ***********************************/
        t = 0;
        while (t < PIECE_TRANSITION_TIME)
        {
            t += Time.unscaledDeltaTime;
            tryAgainButton.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t / PIECE_TRANSITION_TIME);
            quitButton.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t / PIECE_TRANSITION_TIME);
            yield return null;
        }

        //Unlock restartButton and quitButton
        tryAgainButton.GetComponent<Button>().interactable = true;
        quitButton.GetComponent<Button>().interactable = true;
        Cursor.visible = true;

        yield return null;
    }
}
