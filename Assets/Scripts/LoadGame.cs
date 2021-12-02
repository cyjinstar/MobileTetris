using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGame : MonoBehaviour
{
    public string sceneName = "game";

    public void ClickedStart()
    {
        SceneManager.LoadScene("game");
    }
}