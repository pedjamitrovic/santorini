using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayMenu : MonoBehaviour
{
    public Dropdown player1dropdown;
    public Dropdown player2dropdown;
    // Use this for initialization
    void Start()
    {
        PlayerPrefs.SetInt("Player1Type", 0);
        PlayerPrefs.SetInt("Player2Type", 0);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Game");
    }
    // Update is called once per frame
    void Update()
    {
        var dropdownlist1 = GameObject.Find("Player1Dropdown/Dropdown List");
        int i = 0;
        if(dropdownlist1 != null)
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
    }
    public void OnDropdownChange()
    {
        GameObject.Find("Player1Dropdown/Label").GetComponent<TextMeshProUGUI>().text = player1dropdown.options[player1dropdown.value].text;
        GameObject.Find("Player2Dropdown/Label").GetComponent<TextMeshProUGUI>().text = player2dropdown.options[player2dropdown.value].text;
        PlayerPrefs.SetInt("Player1Type", player1dropdown.value);
        PlayerPrefs.SetInt("Player2Type", player2dropdown.value);
    }
}
