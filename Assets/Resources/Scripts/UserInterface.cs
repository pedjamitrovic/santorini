using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace etf.santorini.mp150608d
{
    public class UserInterface : MonoBehaviour
    {
        public TextMeshProUGUI onTurnText;
        public TextMeshProUGUI nextMoveText;
        public TextMeshProUGUI gameOverText;
        public Button finishButton;
        public GameObject textPrefab;
        public ScrollRect possibleMoves;
        public GameObject possibleMovesContent;
        private List<KeyValuePair<Logger.GameMove, float>> possibleMovesList;
        public GameController gc;
        // Use this for initialization
        void Start()
        {
            possibleMovesList = new List<KeyValuePair<Logger.GameMove, float>>();
            possibleMoves.gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnFinishButtonClick()
        {
            SceneManager.LoadScene("Menu");
        }
        public void ShowPossibleMoves()
        {
            SortMovesByRetValue();
            for (int i = 0; i < possibleMovesList.Count; i++)
            {
                var move = possibleMovesList[i];
                string moveString = "( " + move.Key.PreviousFigurePosition + " " + move.Key.NextFigurePosition + " " + move.Key.NewLevelBuildPosition + " ) [" + move.Value + "]";
                if (gc.onTurn is MinimaxBot)
                {
                    MinimaxBot mb = (MinimaxBot)gc.onTurn;
                    if (mb.nextGameMove == move.Key)
                    {
                        moveString = "*" + moveString;
                    } 
                }
                if (gc.onTurn is AlphaBetaBot)
                {
                    AlphaBetaBot mb = (AlphaBetaBot)gc.onTurn;
                    if (mb.nextGameMove == move.Key)
                    {
                        moveString = "*" + moveString;
                    }
                }
                GameObject go = Instantiate(textPrefab, possibleMovesContent.transform);
                go.transform.localPosition = new Vector3(0, i*(-30), 0);
                go.GetComponent<TextMeshProUGUI>().text = moveString;
            }
            possibleMovesContent.GetComponent<RectTransform>().sizeDelta = new Vector2(250, possibleMovesList.Count * 30);
            possibleMoves.gameObject.SetActive(true);
        }
        public void AddPossibleMove(Logger.GameMove move, float retValue)
        {
            possibleMovesList.Add(new KeyValuePair<Logger.GameMove, float>(move, retValue));
        }
        public void ClearPossibleMoves()
        {
            possibleMoves.gameObject.SetActive(false);
            foreach (Transform child in possibleMovesContent.transform)
            {
                Destroy(child.gameObject);
            }
            possibleMovesList.Clear();
        }
        void SortMovesByRetValue()
        {
            possibleMovesList.Sort((x, y) => y.Value.CompareTo(x.Value));
        }
    }
}
