using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMode : MonoBehaviour
{
    public int game_type = 1;
    public GameObject about_panel;
    public GameManager gm_script;

    void Start()
    {
        DontDestroyOnLoad(transform.gameObject);

    }


    public void ActivateGameMode(int n)
    {
        game_type = n;
        SceneManager.LoadScene(1);
    }

    public void ShowAbout()
    {
        about_panel.SetActive(true);
    }

    public void CloseAbout()
    {
        about_panel.SetActive(false);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
        Destroy(this.gameObject);
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(0);
        SceneManager.LoadScene(1);
    }
}
