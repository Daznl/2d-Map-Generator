using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class KeyInput : MonoBehaviour
{
    public GameManager gameManager;
    //public CreateRaces createRaces;
    public MapArrayScript M;
    public Json J;

    public Camera mainGameCamera;
    public Camera mapGenerationCamera;

    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            StartCoroutine(gameManager.createStuff.GenerateMap());
            Debug.Log("Keypress M");
            mainGameCamera.gameObject.SetActive(false);
            mapGenerationCamera.gameObject.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Keypress P");
            //createRaces.CreateRacesStart();
        }
    }

    //public void CreateRaces()
    //{
    //    createRaces.CreateRacesStart();
    //}
}
