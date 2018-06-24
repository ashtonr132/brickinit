﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

    [SerializeField]
    GameObject ExitBut, TutBut, LBut, BBut;
    // Use this for initialization
    void Start() {
        if (SceneManager.GetActiveScene().name.Contains("Main Menu"))
        {
            TutBut.GetComponent<Button>().onClick.AddListener(delegate { MenuButtons(TutBut.name); });
            LBut.GetComponent<Button>().onClick.AddListener(delegate { MenuButtons(LBut.name); });
            BBut.GetComponent<Button>().onClick.AddListener(delegate { MenuButtons(BBut.name); });
        }
        print("exitbutton added");
        ExitBut.GetComponent<Button>().onClick.AddListener(delegate { MenuButtons(ExitBut.name); });
    }
    private void Update()
    {
        if (Time.timeScale != 0 && SceneManager.GetActiveScene().name.Contains("Main Menu"))
        {
            Time.timeScale = 1;
        }
    }
    private void MenuButtons(string buttonname)
    {
        print("button");
        switch (buttonname)
        {
            case "Tut":
                SceneManager.LoadScene("Tutorial Level");
                break;
            case "Level 1":
                SceneManager.LoadScene("Level 01");
                break;
            case "Boss":
                SceneManager.LoadScene("Boss Fight");
                break;
            case "Exit":
                print("exit");
                if (SceneManager.GetActiveScene().name.Contains("Main Menu"))
                {
                    Application.Quit();
                }
                else
                {
                    SceneManager.LoadScene("Main Menu");
                }
                break;
        }
    }

}
