using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace etf.santorini.mp150608d
{
    public class PrefabSpawner : MonoBehaviour
    {
        public GameObject playerFigure;
        public GameObject field;
        public GameObject hemisphere;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public GameObject SpawnPlayerFigure(GameObject onField, Material material)
        {
            GameObject go = Instantiate(playerFigure, GameObject.Find("PlayerFigures").transform) as GameObject;
            go.transform.position = new Vector3(onField.transform.position.x, onField.transform.position.y + 1.25f, onField.transform.position.z);
            go.GetComponent<PlayerFigure>().defaultMaterial = material;
            go.GetComponent<Renderer>().material = material;
            go.GetComponent<PlayerFigure>().position = onField.GetComponent<Field>().position;
            go.GetComponent<PlayerFigure>().Disable();
            onField.GetComponent<Field>().Disable();
            return go;
        }
        public GameObject SpawnField(GameObject onField)
        {
            GameObject go = Instantiate(onField) as GameObject;
            go.GetComponent<Field>().enabled = false;
            onField.GetComponent<Field>().transform.position = new Vector3(onField.GetComponent<Field>().transform.position.x, onField.GetComponent<Field>().transform.position.y + 0.5f, onField.GetComponent<Field>().transform.position.z);
            onField.GetComponent<Field>().transform.localScale = new Vector3(onField.GetComponent<Field>().transform.lossyScale.x * 0.8f, onField.GetComponent<Field>().transform.lossyScale.y, onField.GetComponent<Field>().transform.lossyScale.z * 0.8f);
            go.transform.parent = onField.transform;
            return go;
        }
        public GameObject SpawnHemisphere(GameObject onField)
        {
            GameObject go = Instantiate(hemisphere) as GameObject;
            go.transform.position = new Vector3(onField.GetComponent<Field>().transform.position.x, onField.GetComponent<Field>().transform.position.y + 0.5f, onField.GetComponent<Field>().transform.position.z);
            onField.GetComponent<Field>().enabled = false;
            go.transform.parent = onField.transform;
            return go;
        }
    }
}
