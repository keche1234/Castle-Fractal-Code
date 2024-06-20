using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Pause Info")]
    [SerializeField] protected bool paused = false;
    [SerializeField] protected float prevTimeScale = 1;
    [SerializeField] protected Image blackFade;
    [SerializeField] protected List<Canvas> pauseMenus; //0 is continue/quit, 1 is inventory
    [SerializeField] protected int lastMenu = 0;
    [SerializeField] protected bool gameOver = false;

    [SerializeField] protected PlayerController player;
    [SerializeField] protected GameObject screenBars;

    [Header("Event Manager")]
    [SerializeField] protected EventSystem eventSystem;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
     * Generic Pause function
     */
    public void Pause()
    {
        Pause(lastMenu);
    }

    /*
     * Opens a specific menu, or closes the current menu
     */
    public void Pause(int m)
    {
        paused = !paused;
        if (paused) //pause the game!
        {
            prevTimeScale = Time.timeScale;
            Time.timeScale = 0;

            //show menu i
            blackFade.enabled = true;
            Cursor.visible = true;
            pauseMenus[m].gameObject.SetActive(true);

            Button[] buttons = pauseMenus[m].GetComponentsInChildren<Button>(true);
            if (player.GetActionInputDevice("main attack") == Keyboard.current)
                eventSystem.SetSelectedGameObject(buttons[0].gameObject);
            else
                eventSystem.SetSelectedGameObject(null);
        }
        else //unpause the game!
        {
            Time.timeScale = prevTimeScale;

            //hide menus
            blackFade.enabled = false;
            Cursor.visible = false;
            eventSystem.SetSelectedGameObject(null);
            foreach (Canvas menu in pauseMenus)
                menu.gameObject.SetActive(false);
        }

        if (paused && m == 2)
        {
            gameOver = true;
            screenBars.SetActive(false);
        }
    }

    /*
     * Restarts a run
     */
    public void RestartRun()
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    /*
     * Ends a run
     */
    public void EndRun()
    {
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false; //TEMP CODE
    }

    /*
     * For the sake of spacebars
     */
    public void DeselectButton()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    /*
     * Determines if game is paused
     */
    public bool IsPaused()
    {
        return paused;
    }

    /*
     * Determines if the game is over
     */
    public bool GameIsOver()
    {
        return gameOver;
    }
}
