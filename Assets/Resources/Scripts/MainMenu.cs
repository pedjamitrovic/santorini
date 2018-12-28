using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace etf.santorini.mp150608d
{
    public class MainMenu : MonoBehaviour
    {
        public void QuitGame()
        {
            Debug.Log("QUIT");
            Application.Quit();
        }
    }
}
