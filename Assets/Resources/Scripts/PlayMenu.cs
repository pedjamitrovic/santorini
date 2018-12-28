using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace etf.santorini.mp150608d
{
    public class PlayMenu : MonoBehaviour
    {
        public Dropdown player1dropdown;
        public Dropdown player2dropdown;
        public Dropdown player1dropdowndepth;
        public Dropdown player2dropdowndepth;
        // Use this for initialization
        void Start()
        {
            PlayerPrefs.SetInt("LoadGame", 0);
            PlayerPrefs.SetInt("Player1Type", 0);
            PlayerPrefs.SetInt("Player2Type", 0);
            PlayerPrefs.SetInt("Player1Depth", 3);
            PlayerPrefs.SetInt("Player2Depth", 3);
        }

        public void PlayGame()
        {
            PlayerPrefs.SetInt("LoadGame", 0);
            SceneManager.LoadScene("Game");
        }

        public void LoadGame()
        {
            PlayerPrefs.SetInt("LoadGame", 1);
            SceneManager.LoadScene("Game");
        }
        // Update is called once per frame
        void Update()
        {
            var dropdownlist1 = GameObject.Find("Player1Dropdown/Dropdown List");
            int i = 0;
            if (dropdownlist1 != null)
            {
                foreach (var item in dropdownlist1.GetComponentsInChildren<TextMeshProUGUI>())
                {
                    item.text = player1dropdown.options[i++].text;
                }
            }
            var dropdownlist2 = GameObject.Find("Player2Dropdown/Dropdown List");
            i = 0;
            if (dropdownlist2 != null)
            {
                foreach (var item in dropdownlist2.GetComponentsInChildren<TextMeshProUGUI>())
                {
                    item.text = player2dropdown.options[i++].text;
                }
            }
            var dropdownlistdepth1 = GameObject.Find("Player1DropdownDepth/Dropdown List");
            i = 0;
            if (dropdownlistdepth1 != null)
            {
                foreach (var item in dropdownlistdepth1.GetComponentsInChildren<TextMeshProUGUI>())
                {
                    item.text = player1dropdowndepth.options[i++].text;
                }
            }
            var dropdownlistdepth2 = GameObject.Find("Player2DropdownDepth/Dropdown List");
            i = 0;
            if (dropdownlistdepth2 != null)
            {
                foreach (var item in dropdownlistdepth2.GetComponentsInChildren<TextMeshProUGUI>())
                {
                    item.text = player2dropdowndepth.options[i++].text;
                }
            }

            if (player1dropdown.value == 0)
            {
                player1dropdowndepth.gameObject.SetActive(false);
            }
            else
            {
                player1dropdowndepth.gameObject.SetActive(true);
            }
            if (player2dropdown.value == 0)
            {
                player2dropdowndepth.gameObject.SetActive(false);
            }
            else
            {
                player2dropdowndepth.gameObject.SetActive(true);
            }
        }
        public void OnDropdownChange()
        {
            GameObject.Find("Player1Dropdown/Label").GetComponent<TextMeshProUGUI>().text = player1dropdown.options[player1dropdown.value].text;
            GameObject.Find("Player2Dropdown/Label").GetComponent<TextMeshProUGUI>().text = player2dropdown.options[player2dropdown.value].text;
            try
            {
                GameObject.Find("Player1DropdownDepth/Label").GetComponent<TextMeshProUGUI>().text = player1dropdowndepth.options[player1dropdowndepth.value].text;
                PlayerPrefs.SetInt("Player1Depth", player1dropdowndepth.value + 2);
            }
            catch (System.Exception e) { }
            try
            {
                GameObject.Find("Player2DropdownDepth/Label").GetComponent<TextMeshProUGUI>().text = player2dropdowndepth.options[player2dropdowndepth.value].text;
                PlayerPrefs.SetInt("Player2Depth", player2dropdowndepth.value + 2);
            }
            catch (System.Exception e) { }
            PlayerPrefs.SetInt("Player1Type", player1dropdown.value);
            PlayerPrefs.SetInt("Player2Type", player2dropdown.value);
        }
    }
}
