using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace etf.santorini.mp150608d
{
    public class GameController : MonoBehaviour
    {
        public PrefabSpawner prefabSpawner;
        public UserInterface UI;
        public SemaphoreSlim semaphore;
        public GameObject selectedField;
        public GameObject selectedFigure;
        public Dictionary<string, GameObject> fields;
        public List<GameObject> playerFigures;
        public Player first;
        public Player second;
        public Player onTurn;
        public GameState gameState;
        private bool endGame;
        private Logger log;


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
            InstantiatePlayers();
            log = new Logger(first, second, "proba" + ".txt");
            onTurn = first;
            gameState = new GameState();
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
            log.LogStartingPosition(onTurn, playerFigures[0].GetComponent<PlayerFigure>().position, playerFigures[1].GetComponent<PlayerFigure>().position);
            SwitchTurns();
            await onTurn.PickStartingField(semaphore);
            InstantiatePlayerFigure(Resources.Load<Material>("Materials/SecondPlayerFigureMaterial"));
            await onTurn.PickStartingField(semaphore);
            InstantiatePlayerFigure(Resources.Load<Material>("Materials/SecondPlayerFigureMaterial"));
            log.LogStartingPosition(onTurn, playerFigures[2].GetComponent<PlayerFigure>().position, playerFigures[3].GetComponent<PlayerFigure>().position);
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
                await onTurn.CalculateNextMove();
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
            UI.onTurnText.text = "";
            UI.nextMoveText.text = "";
            UI.gameOverText.text = "GAME OVER - " + onTurn.Id() + " WINS";
            UI.finishButton.gameObject.SetActive(true);
            selectedFigure = null;
            DisableAllFigures();
            DisableAllFields();
            log.WriteToFile();
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
            playerFigures.Add(prefabSpawner.SpawnPlayerFigure(selectedField, material));
            gameState[selectedField.GetComponent<Field>().position] = (onTurn == first ? new GameState.GameField(0, -1) : new GameState.GameField(0, -2));
            selectedField = null;
        }
        void InstantiatePlayers()
        {
            int player1Type = PlayerPrefs.GetInt("Player1Type");
            switch (player1Type)
            {
                case 0:
                    first = new Human("PLAYER 1");
                    break;
                case 1:
                    first = new MinimaxBot("PLAYER 1");
                    break;
                case 2:
                    first = new AlphaBetaBot("PLAYER 1");
                    break;
                default:
                    first = new Human("PLAYER 1");
                    break;
            }
            int player2Type = PlayerPrefs.GetInt("Player2Type");
            switch (player2Type)
            {
                case 0:
                    second = new Human("PLAYER 2");
                    break;
                case 1:
                    second = new MinimaxBot("PLAYER 2");
                    break;
                case 2:
                    second = new AlphaBetaBot("PLAYER 1");
                    break;
                default:
                    second = new Human("PLAYER 2");
                    break;
            }
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
            string[] neighbours = fields[selectedFigure.GetComponent<PlayerFigure>().position].GetComponent<Field>().neighbours;
            for (int i = 0; i < neighbours.Length; i++)
            {
                if (fields[neighbours[i]].GetComponent<Field>().enabled && fields[neighbours[i]].GetComponent<Field>().level != 4)
                {
                    fields[neighbours[i]].GetComponent<Field>().GetComponent<Renderer>().material = fields[neighbours[i]].GetComponent<Field>().selectMaterial;
                    fields[neighbours[i]].GetComponent<Field>().paused = true;
                }
            }
            selectedFigure.GetComponent<PlayerFigure>().Disable();
            DisableAllFigures();
            DisableAllFields();
        }
        void MoveFigure()
        {
            string previousFigurePosition = selectedFigure.GetComponent<PlayerFigure>().position;
            string nextFigurePosition = selectedField.GetComponent<Field>().position;
            selectedFigure.transform.position = new Vector3(selectedField.transform.position.x, selectedField.transform.position.y + 1.25f, selectedField.transform.position.z);
            selectedFigure.GetComponent<PlayerFigure>().position = selectedField.GetComponent<Field>().position;
            selectedFigure.GetComponent<PlayerFigure>().level = selectedField.GetComponent<Field>().level + 1;
            if (selectedFigure.GetComponent<PlayerFigure>().level == 4) endGame = true;
            selectedField = null;
            gameState[nextFigurePosition].PlayerFigure = gameState[previousFigurePosition].PlayerFigure;
            gameState[previousFigurePosition].PlayerFigure = 0;
            log.LogGameMove(onTurn, previousFigurePosition, nextFigurePosition, previousFigurePosition);
        }
        void BuildNewLevel()
        {
            if (selectedField.GetComponent<Field>().level > 3) return;
            if (selectedField.GetComponent<Field>().level < 3)
            {
                prefabSpawner.SpawnField(selectedField);
            }
            else
            {
                prefabSpawner.SpawnHemisphere(selectedField);
            }
            selectedField.GetComponent<Field>().level++;
            gameState[selectedField.GetComponent<Field>().position].FieldLevel = selectedField.GetComponent<Field>().level;
            gameState.ChangeTurns();
            log.AlterLastBuildPosition(onTurn, selectedField.GetComponent<Field>().position);
            selectedField = null;
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
        public GameObject FetchMyFigure(Player p, int index)
        {
            if (p == first) return playerFigures[index];
            else if (p == second) return playerFigures[index + 2];
            return null;
        }
        public GameObject FetchFigure(string currentPosition)
        {
            foreach(var figure in playerFigures)
            {
                if (figure.GetComponent<PlayerFigure>().position == currentPosition) return figure;
            }
            return null;
        }
    }
}