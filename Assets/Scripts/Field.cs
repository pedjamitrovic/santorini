using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour {
    public Material defaultMaterial;
    public Material selectMaterial;
    public GameObject prefab;
    public GameObject hemisphere;
    public int level;
    public string position;
    public string[] neighbours;
    public new bool enabled;
    public bool selected;

	// Use this for initialization
	void Start () {
        GetComponent<Renderer>().material = defaultMaterial;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void Upgrade()
    {
        if (level > 3) return;
        if (level < 3)
        {
            GameObject go = Instantiate(prefab) as GameObject;
            go.GetComponent<Field>().enabled = false;
            go.GetComponent<Renderer>().material = defaultMaterial;
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
            transform.localScale = new Vector3(transform.lossyScale.x * 0.8f, transform.lossyScale.y, transform.lossyScale.z * 0.8f);
        }
        else
        {
            GameObject go = Instantiate(hemisphere) as GameObject;
            go.GetComponentInChildren<Renderer>().material = selectMaterial;
            go.transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
            GetComponent<Renderer>().material = defaultMaterial;
            enabled = false;
        }
        level++;
    }

    private void OnMouseDown()
    {
        if (!enabled) return;
        Upgrade();
    }

    private void OnMouseEnter()
    {
        if (!enabled) return;
        GetComponent<Renderer>().material = selectMaterial;
    }

    private void OnMouseExit()
    {
        if (!enabled) return;
        GetComponent<Renderer>().material = defaultMaterial;
    }

}
