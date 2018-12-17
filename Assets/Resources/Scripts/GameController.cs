using etf.santorini.mp150608d;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    public TextMeshProUGUI onTurnText;
    public TextMeshProUGUI nextMoveText;
    public SemaphoreSlim semaphore;
    public GameObject selectedField;
    public GameObject selectedFigure;
    public GameObject prefabSpawner;
    private Dictionary<string, GameObject> fields;
    private List<GameObject> playerFigures;
    private Player first;
    private Player second;
    private Player onTurn;
    private bool endGame;

    // Use this for initialization
    void Start()
    {
        semaphore = new SemaphoreSlim(0, 1);
        fields = new Dictionary<string, GameObject>();
        playerFigures = new List<GameObject>();
        GameObject[] fieldObjects = GameObject.FindGameObjectsWithTag("Field");
        for (int i = 0; i < fieldObjects.Length; i++)
        {
            fields.Add(fieldObjects[i].GetComponent<Field>().position, fieldObjects[i]);
        }
        first = new Human("PLAYER 1");
        second = new Human("PLAYER 2");
        onTurn = first;
        StartGame();
    }

    // Update is called once per frame
    void Update()
    {

    }

    async void StartGame()
    {
        await onTurn.PickStartingField(semaphore);
        InstantiatePlayerFigure(Resources.Load<Material>("Materials/FirstPlayerFigureMaterial"));
        await onTurn.PickStartingField(semaphore);
        InstantiatePlayerFigure(Resources.Load<Material>("Materials/FirstPlayerFigureMaterial"));
        SwitchTurns();
        await onTurn.PickStartingField(semaphore);
        InstantiatePlayerFigure(Resources.Load<Material>("Materials/SecondPlayerFigureMaterial"));
        await onTurn.PickStartingField(semaphore);
        InstantiatePlayerFigure(Resources.Load<Material>("Materials/SecondPlayerFigureMaterial"));
        SwitchTurns();
        while (true)
        {
            if (NoMovesLeft())
            {
                SwitchTurns();
                break;
            }
            DisableAllFields();
            ActivatePlayerFigures();
            await onTurn.SelectFigure(semaphore);
            EnableAllFields();
            if (ShowPossibleMoves() == 0)
            {
                selectedFigure.GetComponent<PlayerFigure>().Deselect();
                selectedFigure = null;
                continue;
            }
            DisableAllFigures();
            DisableAllFields();
            await onTurn.MoveFigure(semaphore);
            if (selectedFigure == null)
            {
                continue;
            }
            MoveFigure();
            if (endGame) break;
            EnableAllFields();
            DisableAllFigures();
            ShowPossibleBuilds();
            await onTurn.BuildNewLevel(semaphore);
            BuildNewLevel();
            if (endGame) break;
            SwitchTurns();
        }
        FinishGame();
    }
    void FinishGame()
    {
        nextMoveText.text = "GAME OVER - " + onTurn.Id() + " WINS";
        DisableAllFigures();
        DisableAllFields();
    }
    void SwitchTurns()
    {
        if (onTurn == first)
        {
            onTurn = second;
        }
        else if (onTurn == second)
        {
            onTurn = first;
        }
        ActivatePlayerFigures();
    }
    void InstantiatePlayerFigure(Material material)
    {
        playerFigures.Add(prefabSpawner.GetComponent<PrefabSpawner>().SpawnPlayerFigure(selectedField, material));
        selectedField = null;
    }
    int ShowPossibleMoves()
    {
        int count = 0;
        string currentPosition = selectedFigure.GetComponent<PlayerFigure>().position;
        string[] neighbours = fields[currentPosition].GetComponent<Field>().neighbours;
        for (int i = 0; i < neighbours.Length; i++)
        {
            if (fields[neighbours[i]].GetComponent<Field>().enabled && fields[neighbours[i]].GetComponent<Field>().level - 1 <= fields[currentPosition].GetComponent<Field>().level)
            {
                fields[neighbours[i]].GetComponent<Field>().GetComponent<Renderer>().material = fields[neighbours[i]].GetComponent<Field>().selectMaterial;
                fields[neighbours[i]].GetComponent<Field>().paused = true;
                count++;
            }
        }
        return count;
    }
    int CountPossibleMoves(string currentPosition)
    {
        EnableAllFields();
        int count = 0;
        string[] neighbours = fields[currentPosition].GetComponent<Field>().neighbours;
        for (int i = 0; i < neighbours.Length; i++)
        {
            if (fields[neighbours[i]].GetComponent<Field>().enabled && fields[neighbours[i]].GetComponent<Field>().level - 1 <= fields[currentPosition].GetComponent<Field>().level)
            {
                count++;
            }
        }
        DisableAllFields();
        return count;
    }
    bool NoMovesLeft()
    {
        bool val = false;
        if (onTurn == first)
        {
            val = (CountPossibleMoves(playerFigures[0].GetComponent<PlayerFigure>().position) == 0 && CountPossibleMoves(playerFigures[1].GetComponent<PlayerFigure>().position) == 0);
        }
        else if (onTurn == second)
        {
            val = (CountPossibleMoves(playerFigures[2].GetComponent<PlayerFigure>().position) == 0 && CountPossibleMoves(playerFigures[3].GetComponent<PlayerFigure>().position) == 0);
        }
        return val;
    }
    void ShowPossibleBuilds()
    {
        string firstPosition = "", secondPosition = "";
        if (onTurn == first)
        {
            firstPosition = playerFigures[0].GetComponent<PlayerFigure>().position;
            secondPosition = playerFigures[1].GetComponent<PlayerFigure>().position;
        }
        else
        {
            firstPosition = playerFigures[2].GetComponent<PlayerFigure>().position;
            secondPosition = playerFigures[3].GetComponent<PlayerFigure>().position;
        }
        string[] neighbours = fields[firstPosition].GetComponent<Field>().neighbours;
        for (int i = 0; i < neighbours.Length; i++)
        {
            if (fields[neighbours[i]].GetComponent<Field>().enabled && fields[neighbours[i]].GetComponent<Field>().level != 4)
            {
                fields[neighbours[i]].GetComponent<Field>().GetComponent<Renderer>().material = fields[neighbours[i]].GetComponent<Field>().selectMaterial;
                fields[neighbours[i]].GetComponent<Field>().paused = true;
            }
        }
        neighbours = fields[secondPosition].GetComponent<Field>().neighbours;
        for (int i = 0; i < neighbours.Length; i++)
        {
            if (fields[neighbours[i]].GetComponent<Field>().enabled && fields[neighbours[i]].GetComponent<Field>().level != 4)
            {
                fields[neighbours[i]].GetComponent<Field>().GetComponent<Renderer>().material = fields[neighbours[i]].GetComponent<Field>().selectMaterial;
                fields[neighbours[i]].GetComponent<Field>().paused = true;
            }
        }
        DisableAllFigures();
        selectedFigure = playerFigures[0]; // dummy
        DisableAllFields();
        selectedFigure = null;
    }
    void MoveFigure()
    {
        selectedFigure.transform.position = new Vector3(selectedField.transform.position.x, selectedField.transform.position.y + 1.25f, selectedField.transform.position.z);
        selectedFigure.GetComponent<PlayerFigure>().position = selectedField.GetComponent<Field>().position;
        selectedFigure.GetComponent<PlayerFigure>().level = selectedField.GetComponent<Field>().level + 1;
        if (selectedFigure.GetComponent<PlayerFigure>().level == 4) endGame = true;
        selectedField = null;
        selectedFigure = null;
    }
    void BuildNewLevel()
    {
        if (selectedField.GetComponent<Field>().level > 3) return;
        if (selectedField.GetComponent<Field>().level < 3)
        {
            prefabSpawner.GetComponent<PrefabSpawner>().SpawnField(selectedField);
        }
        else
        {
            prefabSpawner.GetComponent<PrefabSpawner>().SpawnHemisphere(selectedField);
        }
        selectedField.GetComponent<Field>().level++;
    }
    void EnableAllFields()
    {
        foreach (var field in fields)
        {
            if (field.Value.GetComponent<Field>().level < 4) field.Value.GetComponent<Field>().Enable();
        }
        foreach (var playerFigure in playerFigures)
        {
            fields[playerFigure.GetComponent<PlayerFigure>().position].GetComponent<Field>().Disable();
        }
    }
    void DisableAllFields()
    {
        foreach (var field in fields)
        {
            if (!field.Value.GetComponent<Field>().paused || selectedFigure == null) field.Value.GetComponent<Field>().Disable();
        }
    }
    void EnableAllFigures()
    {
        foreach (var figure in playerFigures)
        {
            figure.GetComponent<PlayerFigure>().Enable();
        }
    }
    void DisableAllFigures()
    {
        foreach (var figure in playerFigures)
        {
            if (selectedFigure == figure) continue;
            if (!figure.GetComponent<PlayerFigure>().paused) figure.GetComponent<PlayerFigure>().Disable();
        }
    }
    void ActivatePlayerFigures()
    {
        if (onTurn == first)
        {
            if (playerFigures.Count > 0 && CountPossibleMoves(playerFigures[0].GetComponent<PlayerFigure>().position) > 0) playerFigures[0].GetComponent<PlayerFigure>().Enable();
            if (playerFigures.Count > 1 && CountPossibleMoves(playerFigures[1].GetComponent<PlayerFigure>().position) > 0) playerFigures[1].GetComponent<PlayerFigure>().Enable();
            if (playerFigures.Count > 2) playerFigures[2].GetComponent<PlayerFigure>().Disable();
            if (playerFigures.Count > 3) playerFigures[3].GetComponent<PlayerFigure>().Disable();
        }
        else if (onTurn == second)
        {
            if (playerFigures.Count > 0) playerFigures[0].GetComponent<PlayerFigure>().Disable();
            if (playerFigures.Count > 1) playerFigures[1].GetComponent<PlayerFigure>().Disable();
            if (playerFigures.Count > 2 && CountPossibleMoves(playerFigures[2].GetComponent<PlayerFigure>().position) > 0) playerFigures[2].GetComponent<PlayerFigure>().Enable();
            if (playerFigures.Count > 3 && CountPossibleMoves(playerFigures[3].GetComponent<PlayerFigure>().position) > 0) playerFigures[3].GetComponent<PlayerFigure>().Enable();
        }
    }
}
