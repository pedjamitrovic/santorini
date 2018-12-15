using etf.santorini.mp150608d;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public TextMeshProUGUI onTurnText;
    public TextMeshProUGUI nextMoveText;
    private Dictionary<string, GameObject> fields;
    private Player first;
    private Player second;
    private Player onTurn;

    // Use this for initialization
    void Start () {
        fields = new Dictionary<string, GameObject>();
        GameObject[] fieldObjects = GameObject.FindGameObjectsWithTag("Field");
        for(int i = 0; i < fieldObjects.Length; i++)
        {
            fields.Add(fieldObjects[i].GetComponent<Field>().position, fieldObjects[i]);
        }
        first = new Human(this);
        second = new Human(this);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartGame()
    {
        onTurnText.text = "ON TURN: PLAYER 1";
        first.PickStartingFields();

        onTurnText.text = "ON TURN: PLAYER 2";
        second.PickStartingFields();

        onTurn = first;

        while (!EndOfGame())
        {
            onTurn.NextMove();
            SwitchTurns();
        }
    }
    public bool EndOfGame()
    {
        return false;
    }
    public void FinishGame()
    {

    }
    public void SwitchTurns()
    {
        if(onTurn == first)
        {
            onTurn = second;
            onTurnText.text = "ON TURN: PLAYER 2";
        }
        else if (onTurn == second)
        {
            onTurn = first;
            onTurnText.text = "ON TURN: PLAYER 1";
        }
    }
}
