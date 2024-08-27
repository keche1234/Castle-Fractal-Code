using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuHolderUI : MonoBehaviour
{
    [SerializeField] protected List<GameObject> menus;

    public void OpenMenu(GameObject menu)
    {
        if (menus.Contains(menu))
        {
            foreach (GameObject myMenu in menus)
                myMenu.SetActive(false);
            menu.SetActive(true);
        }
    }

    public void Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        PlayerPrefs.DeleteKey("Leaderboard");
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void ResetScores()
    {
        PlayerPrefs.DeleteKey("Leaderboard");
        OpenMenu(menus[0]);
    }

    public void TransitionScene(string scene)
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scene);
    }
}
