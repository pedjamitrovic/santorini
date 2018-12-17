using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFigure : MonoBehaviour
{
    public Material defaultMaterial;
    public Material selectMaterial;
    public GameController gameController;
    public new bool enabled;
    public bool selected;
    public bool paused;
    public string position;
    public int level;
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

    private void OnMouseDown()
    {
        if (!enabled) return;
        if (!selected)
        {
            GetComponent<Renderer>().material = selectMaterial;
            gameController.selectedFigure = this.gameObject;
            gameController.semaphore.Release(1);
        }
        else
        {
            Enable();
            gameController.selectedFigure = null;
            gameController.semaphore.Release(1);
            return;
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
