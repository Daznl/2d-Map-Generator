using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class RiverMakerScript : MonoBehaviour
{
    private GameObject mapMaker;
    private void Start()
    {
        FindMapMaker();
        M = mapMaker.GetComponent<MapArrayScript>();

        destroyRiverMaker = false;

        riverMakerPosition.X = (int)transform.position.x;
        riverMakerPosition.Y = (int)transform.position.y;

        internalRiverCounter = 0;
        riverAltitude = M.altitudeArray[riverMakerPosition.X, riverMakerPosition.Y];
        flowDirections = UpdateDirections(riverFixedFlowDirection);
        flowPositions = UpdatePositions(flowDirections, flowPositions, riverMakerPosition);
    }
    public static bool isUpdating = false;

    private void FindMapMaker()
    {
        GameObject worldCreator = GameObject.Find("WorldCreator");
        if (worldCreator != null)
        {
            Transform mapMakerTransform = worldCreator.transform.Find("MapMaker");
            if (mapMakerTransform != null)
            {
                mapMaker = mapMakerTransform.gameObject;
            }
            else
            {
                Debug.LogError("MapMaker not found.");
            }
        }
        else
        {
            Debug.LogError("WorldCreator not found.");
        }
    }
    private void Update()
    {

        RollChangeFlowDirection();

        //update river block
        riverMakerPosition.X = (int)transform.position.x;
        riverMakerPosition.Y = (int)transform.position.y;

        riverAltitude = M.altitudeArray[riverMakerPosition.X, riverMakerPosition.Y];
        flowDirections = UpdateDirections(riverFixedFlowDirection);
        flowPositions = UpdatePositions(flowDirections, flowPositions, riverMakerPosition);

        //Debug.Log("BigCheck:FlowDirections straight:" + flowDirections.Straight + ", left: " + flowDirections.Left + ", right:  " + flowDirections.Right + ", Currect coords:" + x +", " + y +", FlowPostions: straight coords: " + flowPositions.Straight.X + ", " + flowPositions.Straight.Y + ", right coords" + flowPositions.Right.X + ", " + flowPositions.Right.Y + ", left coords " + flowPositions.Left.X + ", " + flowPositions.Left.Y);
        if (isUpdating)
            return;

        isUpdating = true;

        riverMoved = false;

        riverMoved = RiverMerger(flowPositions, flowDirections, riverMakerPosition);

        if (!riverMoved)
        {
            riverMoved = RiverNextToWater(flowPositions, riverMakerPosition);
        }

        

        if (!riverMoved)
        {
            canMoveDirections = UpdateCanMoveDirections(canMoveDirections, flowDirections, riverMakerPosition);

            ChooseMove(canMoveDirections, flowDirections, riverMakerPosition);
        }

        if (destroyRiverMaker == true)
        {
            Destroy(this.gameObject);
        }

        // After we are done updating, release the lock
        isUpdating = false;
    }

    /*if block in the direction is a riverBlock of a different number merge with it*/
    public bool RiverMerger(FlowPositions currentFlowPositions, FlowDirections currentFlowDirections, Position riverMakerPostion)
    {
        bool didMove = false;
        /*block in direction straight is a riverBlock of different number*/
        //Debug.Log("straight: (" + currentFlowPositions.Straight.X + ", " + currentFlowPositions.Straight.Y + "), (" + M.riverNumberArray[currentFlowPositions.Straight.X, currentFlowPositions.Straight.Y] + "),Left: (" + currentFlowPositions.Left.X + ", " + currentFlowPositions.Left.Y + "), (" + M.riverNumberArray[currentFlowPositions.Left.X, currentFlowPositions.Left.Y] + "),right: (" + currentFlowPositions.Right.X + ", " + currentFlowPositions.Right.Y + ")" + ", (" + M.riverNumberArray[currentFlowPositions.Right.X, currentFlowPositions.Right.Y] + ")");
        if (M.riverNumberArray[currentFlowPositions.Straight.X, currentFlowPositions.Straight.Y] != -1 && M.riverNumberArray[currentFlowPositions.Straight.X, currentFlowPositions.Straight.Y] != riverNumber)
        {
            Merge(currentFlowDirections.Straight, currentFlowPositions.Straight.X, currentFlowPositions.Straight.Y, riverMakerPostion);
            didMove = true;
        }
        /*block in direction left is a riverBlock of different number*/
        else if (M.riverNumberArray[currentFlowPositions.Left.X, currentFlowPositions.Left.Y] != -1 && M.riverNumberArray[currentFlowPositions.Left.X, currentFlowPositions.Left.Y] != riverNumber)
        {
            Merge(currentFlowDirections.Left, currentFlowPositions.Left.X, currentFlowPositions.Left.Y, riverMakerPostion);
            didMove = true;
        }
        /*block in direction right is a riverBlock of different number*/
        else if (M.riverNumberArray[currentFlowPositions.Right.X, currentFlowPositions.Right.Y] != -1 && M.riverNumberArray[currentFlowPositions.Right.X, currentFlowPositions.Right.Y] != riverNumber)
        {
            Merge(currentFlowDirections.Right, currentFlowPositions.Right.X, currentFlowPositions.Right.Y, riverMakerPostion);
            didMove = true;
        }
        return didMove;
    }

    /*logic to pick what direction to move in*/
    public void ChooseMove(CanMoveDirections canMoveDirections, FlowDirections flowDirections, Position riverMakerPosition)

    {
        CreateStuffSimpleFunctions.Direction noDirection = CreateStuffSimpleFunctions.Direction.NoDirection;

        CreateStuffSimpleFunctions.Direction movedDirection;

        int riverBranchChance = 5;

        /*if all direction can be moved in*/
        if (canMoveDirections.Left && canMoveDirections.Right && canMoveDirections.Straight)
        {
            /*chance to move in each direction, straight is higher chance*/
            int choice = Random.Range(0, 10);
            if (choice <= 2)
            {
                Move(flowDirections.Left, riverMakerPosition);
                movedDirection = flowDirections.Left;
            }  
            else if (choice <= 5)
            {
                Move(flowDirections.Right, riverMakerPosition);
                movedDirection = flowDirections.Right;
            }   
            else
            {
                Move(flowDirections.Straight, riverMakerPosition);
                movedDirection = flowDirections.Straight;
            }
            /*chance to create another branch to the river*/
            ThreeWayRiverDiverge(riverBranchChance, riverMakerPosition, movedDirection, flowDirections.Left, flowDirections.Right, flowDirections.Straight);
        }
        /*can move left and straight*/
        else if (canMoveDirections.Left && canMoveDirections.Straight)
        {
            /*equal chance to move in both directions*/
            int choice = Random.Range(0, 2);
            if (choice == 0)
            { 
                Move(flowDirections.Left, riverMakerPosition);
            }
            else
            {
                Move(flowDirections.Straight, riverMakerPosition);
            }
            /*chance to create another branch to the river*/
            TwoWayRiverDiverge(riverBranchChance, riverMakerPosition, flowDirections.Left, flowDirections.Straight);
        }
        /*can move right and straight*/
        else if (canMoveDirections.Right && canMoveDirections.Straight)
        {
            /*equal chance to move in both directions*/
            int choice = Random.Range(0, 2);
            if (choice == 0)
            { 
                Move(flowDirections.Right, riverMakerPosition); 
            }
            else
            {
                Move(flowDirections.Straight, riverMakerPosition);
            }
            /*chance to create another branch to the river*/
            TwoWayRiverDiverge(riverBranchChance, riverMakerPosition, flowDirections.Right, flowDirections.Straight);
        }
        /*can move right and left*/
        else if (canMoveDirections.Right && canMoveDirections.Left)
        {
            /*equal chance to move in both directions*/
            int choice = Random.Range(0, 2);
            if (choice == 0)
            {
                Move(flowDirections.Right, riverMakerPosition);
            }
            else
            {
                Move(flowDirections.Left, riverMakerPosition);
            }
            /*chance to create another branch to the river*/
            TwoWayRiverDiverge(riverBranchChance, riverMakerPosition, flowDirections.Right, flowDirections.Left);
        }
        else
        {
            /*go through individual directions to see which one is avaliable to move towards*/
            if (canMoveDirections.Straight)
            {
                Move(flowDirections.Straight, riverMakerPosition);
            }
            else if (canMoveDirections.Right)
            {
                Move(flowDirections.Right, riverMakerPosition);
            }
            else if (canMoveDirections.Left)
            {
                Move(flowDirections.Left, riverMakerPosition);
            } 
            else if (canMoveDirections.Back)
            {
                RiverForcedChangeDirection(flowDirections, riverMakerPosition, canMoveDirections);
            }
            else
            {
                /*all of the locations are not possible to move in*/
                CreateRiverBlock(riverMakerPosition, noDirection);
                destroyRiverMaker = true;
            }
        }
    }
}