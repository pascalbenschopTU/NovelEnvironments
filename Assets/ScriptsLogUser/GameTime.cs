using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameTime : MonoBehaviour
{
    public static DateTime StartTime = DateTime.Now;
    public static int TotalGameTime = 0;

    private static Scene scene;

    public static void AddGameTime()
    {
        scene = SceneManager.GetActiveScene();

        TotalGameTime += (int)(DateTime.Now - StartTime).TotalSeconds;
        
    }

    public static void RestartGameTime()
    {
        scene = SceneManager.GetActiveScene();

        StartTime = DateTime.Now;
    }
}