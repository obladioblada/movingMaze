using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    public void startGame() {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
}
