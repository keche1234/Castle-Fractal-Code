using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Pause Info")]
    [SerializeField] protected bool paused = false;
    [SerializeField] protected float prevTimeScale = 1;
    [SerializeField] protected Image blackFade;
    [SerializeField] protected List<Canvas> pauseMenus; //0 is continue/quit, 1 is inventory
    [SerializeField] protected int lastMenu = 0;

    // Start is called before the first frame update
    void Start()
    {
        
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
     * Opens a specific menu
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
        }
        else //unpause the game!
        {
            Time.timeScale = prevTimeScale;

            //hide menus
            blackFade.enabled = false;
            Cursor.visible = false;
            EventSystem.current.SetSelectedGameObject(null);
            foreach (Canvas menu in pauseMenus)
                menu.gameObject.SetActive(false);
        }
    }

    /*
     * Ends a run
     */
    public void EndRun()
    {
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
}
