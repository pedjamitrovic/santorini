using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
    public TextMeshProUGUI onTurnText;
    public TextMeshProUGUI nextMoveText;
    public TextMeshProUGUI gameOverText;
    public Button finishButton;
    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnFinishButtonClick()
    {
        SceneManager.LoadScene("Menu");
    }
}
