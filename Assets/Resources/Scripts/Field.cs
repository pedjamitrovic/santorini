using UnityEngine;

namespace etf.santorini.mp150608d
{
    public class Field : MonoBehaviour
    {
        public Material defaultMaterial;
        public Material selectMaterial;
        public GameController gameController;
        public int level;
        public string position;
        public string[] neighbours;
        public new bool enabled;
        public bool selected;
        public bool paused;


        // Use this for initialization
        void Start()
        {
            gameController = GameObject.Find("GameController").GetComponent<GameController>();
            GetComponent<Renderer>().material = defaultMaterial;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnMouseDown()
        {
            if (!enabled) return;
            if (!selected)
            {
                GetComponent<Renderer>().material = selectMaterial;
                gameController.GetComponent<GameController>().selectedField = this.gameObject;
                gameController.GetComponent<GameController>().semaphore.Release(1);
            }
            else
            {
                GetComponent<Renderer>().material = defaultMaterial;
            }
            selected = !selected;
        }

        private void OnMouseEnter()
        {
            if (!enabled) return;
            if (!selected) GetComponent<Renderer>().material = selectMaterial;
        }

        private void OnMouseExit()
        {
            if (!enabled || paused) return;
            if (!selected) GetComponent<Renderer>().material = defaultMaterial;
        }

        public void Enable()
        {
            enabled = true;
            paused = false;
            selected = false;
            GetComponent<Renderer>().material = defaultMaterial;
        }

        public void Disable()
        {
            selected = false;
            enabled = false;
            GetComponent<Renderer>().material = defaultMaterial;
        }

        public void Deselect()
        {
            selected = false;
            GetComponent<Renderer>().material = defaultMaterial;
        }

    }
}
