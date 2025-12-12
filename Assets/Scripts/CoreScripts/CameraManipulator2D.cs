using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraManipulator2D : MonoBehaviour
{
    MapArrayScript M;
    CreateStuffAdvancedFunctions A;
    CreateStuffSimpleFunctions S;
    public MapGenerationFunctions G;
    public CreateStuff C;

    public float speed = 10.0f;
    public float xLowerBoundary = 0f;
    public float yLowerBoundary = 0f;

    private CreateStuff createStuffScript;
    private int gameBoundary;

    public GameObject player; // reference to the player object

    void Start()
    {
        GameObject mapMaker = GameObject.Find("MapMaker");


        A = mapMaker.GetComponent<CreateStuffAdvancedFunctions>();
        M = mapMaker.GetComponent<MapArrayScript>();
        S = mapMaker.GetComponent<CreateStuffSimpleFunctions>();
        G = mapMaker.GetComponent<MapGenerationFunctions>();
        C = mapMaker.GetComponent<CreateStuff>();

        gameBoundary = GameManager.Instance.mapSize;
    }

    void Update()
    {
        if (transform.position.x < xLowerBoundary)
        {
            transform.position = new Vector3(xLowerBoundary, transform.position.y, transform.position.z);
        }
        if (transform.position.x > gameBoundary)
        {
            transform.position = new Vector3(gameBoundary, transform.position.y, transform.position.z);
        }
        if (transform.position.y < yLowerBoundary)
        {
            transform.position = new Vector3(transform.position.x, yLowerBoundary, transform.position.z);
        }
        if (transform.position.y > gameBoundary)
        {
            transform.position = new Vector3(transform.position.x, gameBoundary, transform.position.z);
        }

        // update the camera position to match the player's position
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);

        // you can still use your existing input code to move the camera manually
        float horizontalInput = Input.GetAxis("Horizontal");
        transform.Translate(Vector3.right * horizontalInput * Time.deltaTime * speed);
        float verticalInput = Input.GetAxis("Vertical");
        transform.Translate(Vector3.up * verticalInput * Time.deltaTime * speed);
    }
}