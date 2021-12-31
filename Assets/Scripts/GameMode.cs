using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMode : MonoBehaviour
{
    public int game_type = 1;

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }


    public void ActivateGameMode(int n)
    {
        game_type = n;
        SceneManager.LoadScene(1);
    }
}
