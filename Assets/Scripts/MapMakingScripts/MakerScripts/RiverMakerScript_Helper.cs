using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class RiverMakerScript
{
    MapArrayScript M;

    public GameObject RiverMaker;
    public struct Position
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    Position riverMakerPosition;
    public struct FlowPositions
    {
        public Position Straight { get; set; }
        public Position Left { get; set; }
        public Position Right { get; set; }
    }

    public FlowPositions flowPositions;
    public struct FlowDirections
    {
        public CreateStuffSimpleFunctions.Direction Straight { get; set; }
        public CreateStuffSimpleFunctions.Direction Left { get; set; }
        public CreateStuffSimpleFunctions.Direction Right { get; set; }
    }

    public FlowDirections flowDirections;

    public struct CanMoveDirections
    {
        public bool Straight { get; set; }
        public bool Left { get; set; }
        public bool Right { get; set; }
        public bool Back { get; set; }
    }

    public CanMoveDirections canMoveDirections;

    public CreateStuffSimpleFunctions.Direction riverFixedFlowDirection;

    public CreateStuffSimpleFunctions.Direction previousDirection;

    public bool riverMoved;
    public bool destroyRiverMaker;

    public int lowThreshold = 80;
    public int highThreshold = 90;

    public int riverNumber;
    public int internalRiverCounter;

    public int riverAltitude;

    public GameObject GenericRiverPrefab;

    /*get directions relative to direction facing*/
    public FlowDirections UpdateDirections(CreateStuffSimpleFunctions.Direction mainDirection)
    {
        FlowDirections flowDirections = new FlowDirections
        {
            Straight = CreateStuffSimpleFunctions.Direction.NoDirection,
            Left = CreateStuffSimpleFunctions.Direction.NoDirection,
            Right = CreateStuffSimpleFunctions.Direction.NoDirection
        };

        switch (mainDirection)
        {
            case CreateStuffSimpleFunctions.Direction.right:
                flowDirections.Straight = CreateStuffSimpleFunctions.Direction.right;
                flowDirections.Left = CreateStuffSimpleFunctions.Direction.up;
                flowDirections.Right = CreateStuffSimpleFunctions.Direction.down;
                break;
            case CreateStuffSimpleFunctions.Direction.left:
                flowDirections.Straight = CreateStuffSimpleFunctions.Direction.left;
                flowDirections.Left = CreateStuffSimpleFunctions.Direction.down;
                flowDirections.Right = CreateStuffSimpleFunctions.Direction.up;
                break;
            case CreateStuffSimpleFunctions.Direction.up:
                flowDirections.Straight = CreateStuffSimpleFunctions.Direction.up;
                flowDirections.Left = CreateStuffSimpleFunctions.Direction.left;
                flowDirections.Right = CreateStuffSimpleFunctions.Direction.right;
                break;
            case CreateStuffSimpleFunctions.Direction.down:
                flowDirections.Straight = CreateStuffSimpleFunctions.Direction.down;
                flowDirections.Left = CreateStuffSimpleFunctions.Direction.right;
                flowDirections.Right = CreateStuffSimpleFunctions.Direction.left;
                break;
        }
        //Debug.Log("update directions: Main Direction: " + mainDirection+ ", Straight: " + flowDirections.Straight + ", Left: " + flowDirections.Left + ", Right: " + flowDirections.Right);
        return flowDirections;
    }

    /*if river is stuck try to get it flowing again by moving it back a spawc if it is free and then check surroundings and change river main flow direction to an availiable direction*/
    public void RiverForcedChangeDirection(FlowDirections flowDirections, Position riverMakerPosition, CanMoveDirections canMoveDirections )
    {

        CreateStuffSimpleFunctions.Direction noDirection = CreateStuffSimpleFunctions.Direction.NoDirection;

        Move(InvertDirection(flowDirections.Straight), riverMakerPosition);
        canMoveDirections = UpdateCanMoveDirections(canMoveDirections, flowDirections, GetNextPosition(riverMakerPosition, InvertDirection(flowDirections.Straight)));
        if (canMoveDirections.Left && canMoveDirections.Right)
        {
            RollChangeFlowDirection();
        }
        else if (canMoveDirections.Left)
        {
            riverFixedFlowDirection = flowDirections.Left;
        }
        else if (canMoveDirections.Right)
        {
            riverFixedFlowDirection = flowDirections.Right;
        }
        else if (canMoveDirections.Back)
        {
            riverFixedFlowDirection = InvertDirection(flowDirections.Straight);
        }
        else
        {
            CreateRiverBlock(riverMakerPosition, noDirection);
            destroyRiverMaker = true;
        }
    }

    public CanMoveDirections UpdateCanMoveDirections(CanMoveDirections canMoveDirections, FlowDirections flowDirections, Position riverMakerPostion)
    {
        canMoveDirections.Left = checkFlow(flowDirections.Left, riverMakerPostion);
        canMoveDirections.Right = checkFlow(flowDirections.Right, riverMakerPostion);
        canMoveDirections.Straight = checkFlow(flowDirections.Straight, riverMakerPostion);
        canMoveDirections.Back = checkFlow(InvertDirection(flowDirections.Straight), riverMakerPostion);
        return canMoveDirections;
    }

    /*checks the block in the input direction to see if it is a river of the same type or a block of higher altitude returning true or false*/
    bool checkFlow(CreateStuffSimpleFunctions.Direction direction, Position riverMakerPostion)
    {
        bool canMoveInDirection = true;
        // get the block's position in direction.
        Position nextPosition = GetNextPosition(riverMakerPostion, direction);
        //Debug.Log("Getnext Postiton,(OverallDriection, direction, currentx, currenty, checkx, checky, rivernumber):(" + riverFixedFlowDirection + ", " + direction + ", " + riverMakerX + ", " + riverMakerY + ", " + nextPosition.X + ", " + nextPosition.Y + ", " + riverNumber + ", ");
        /*if block is a riverBlock of same number*/
        if (M.riverNumberArray[nextPosition.X, nextPosition.Y] == riverNumber)
        {
            canMoveInDirection = false;
        }

        //Check Altitude of block in the direction
        if (M.altitudeArray[nextPosition.X, nextPosition.Y] > riverAltitude)
        {
            canMoveInDirection = false;
        }

        return canMoveInDirection;
    }
    /*get coordinate of block in direction relative to direction facing*/
    public FlowPositions UpdatePositions(FlowDirections flowDirections, FlowPositions flowPositions, Position riverMakerPostion)
    {
        flowPositions.Straight = GetNextPosition(riverMakerPostion, flowDirections.Straight);
        flowPositions.Left = GetNextPosition(riverMakerPostion, flowDirections.Left);
        flowPositions.Right = GetNextPosition(riverMakerPostion, flowDirections.Right);

        return flowPositions;
    }

    /*get the coordinate in the direction relative to rivermakers positions*/
    public Position GetNextPosition(Position riverMakerPostion, CreateStuffSimpleFunctions.Direction direction)
    {
        Position nextPosition = new Position { X = riverMakerPostion.X, Y = riverMakerPostion.Y };
        switch (direction)
        {
            case CreateStuffSimpleFunctions.Direction.right:
                nextPosition.X += 1;
                break;
            case CreateStuffSimpleFunctions.Direction.left:
                nextPosition.X -= 1;
                break;
            case CreateStuffSimpleFunctions.Direction.up:
                nextPosition.Y += 1;
                break;
            case CreateStuffSimpleFunctions.Direction.down:
                nextPosition.Y -= 1;
                break;
        }
        //Debug.Log(", Check Direction:" + direction +", riverMakerX,Y:" + riverMakerX + ", " + riverMakerY + ", NextPostionX,Y: " + nextPosition.X + ", " + nextPosition.Y);
        return nextPosition;
    }

    /*random chance of changing the fixed direction such that three new directions are possible*/
    private void RollChangeFlowDirection()
    {
        int roll = Random.Range(0, 100);
        if (roll == 1) // 1% chance
        {
            int turnRoll = Random.Range(0, 2); // 50% chance for either turn
            if (turnRoll == 0)
            {
                //new riverFixedFlowDirection
                riverFixedFlowDirection = flowDirections.Left;
                // for some reason if you find this message I'm pretty sure I should update the flow directions here and not in the main function
            }
            else
            {
                //new riverFixedFlowDirection
                riverFixedFlowDirection = flowDirections.Right;
            }
        }
    }

    /*give block that riverMaker merged with information that it merged after check if it's done it before*/
    void Merge(CreateStuffSimpleFunctions.Direction direction, int directionX, int directionY, Position riverMakerPostion)
    {
        RiverBlock riverBlock = GetRiverBlockAtPosition(directionX, directionY);
        if (riverBlock.firstMergeDirection == CreateStuffSimpleFunctions.Direction.NoDirection)
        {
            riverBlock.firstMergeDirection = InvertDirection(direction);
        }
        else
        {
            riverBlock.secondMergeDirection = InvertDirection(direction);
        }
        CreateRiverBlock(riverMakerPostion, direction);
        destroyRiverMaker = true;
    }

    /*go throught the riverBlock list and find the block at those coordinates, return it for modification*/
    public RiverBlock GetRiverBlockAtPosition(int x, int y)
    {
        foreach (RiverBlock riverBlock in M.riverBlocks)
        {
            if (riverBlock.x == x && riverBlock.y == y)
            {
                return riverBlock;
            }
        }
        return null; // return null if no RiverBlock is found at the given position
    }

    public bool RiverNextToWater(FlowPositions postions, Position riverMakerPostion)
    {
        int choice = Random.Range(0, 2);    //random chance 50%
        bool destroy = false;
        bool waterMerged = false;

        //Debug.Log("River Next To Water Check. RiverMaker X,Y:"+ riverMakerX + ", " + riverMakerY + ", Postions Left X, Y:"+ postions.Left.X +", "+ postions.Left.Y);

        if (M.blockType[postions.Straight.X,postions.Straight.Y] == MapArrayScript.Blocktype.Shallowwater ||
            M.blockType[postions.Straight.X, postions.Straight.Y] == MapArrayScript.Blocktype.Water)
        {
            CreateRiverBlock(riverMakerPostion, flowDirections.Straight);
            destroy = true;
            waterMerged = true;
            
        }
        else if((M.blockType[postions.Left.X, postions.Left.Y] == MapArrayScript.Blocktype.Shallowwater ||
          M.blockType[postions.Left.X, postions.Left.Y] == MapArrayScript.Blocktype.Water)
         && choice == 0)
            {
            CreateRiverBlock(riverMakerPostion, flowDirections.Left);
            destroy = true;
            waterMerged = true;
            
        }
        else if ((M.blockType[postions.Right.X, postions.Right.Y] == MapArrayScript.Blocktype.Shallowwater ||
          M.blockType[postions.Right.X, postions.Right.Y] == MapArrayScript.Blocktype.Water)
          && choice == 1)
        {
            CreateRiverBlock(riverMakerPostion, flowDirections.Right);
            destroy = true;
            waterMerged = true;
            
        }
        

        if (destroy == true)
        { 
            destroyRiverMaker = true;
        }

        return waterMerged;
    }

    /*move the RiverMakerObject(self) in the given direction after creating the riverBlock on it's current postion*/
    void Move(CreateStuffSimpleFunctions.Direction direction, Position riverMakerPostion)
    {
        // Create the river block at the current location before moving
        CreateRiverBlock(riverMakerPostion, direction);

        // Get the next position based on the given direction
        Position nextPosition = GetNextPosition(riverMakerPostion, direction);

        //update previous direction
        previousDirection = InvertDirection(direction);

        // Move the GameObject to the next position
        transform.position = new Vector3(nextPosition.X, nextPosition.Y, transform.position.z);

    }

    public RiverBlock CreateRiverBlock(Position riverMakerPostion, CreateStuffSimpleFunctions.Direction nextLocation)
    {
        RiverBlock riverBlock = ScriptableObject.CreateInstance<RiverBlock>();

        // Populate the RiverBlock properties.
        riverBlock.sprite = null; // Assign the appropriate sprite.
        riverBlock.previousRiverDirection = previousDirection; // Assign the appropriate direction.
        riverBlock.nextRiverDirection = nextLocation; // Assign the next direction
        riverBlock.internalRiverNumber = internalRiverCounter; //The order of rivers within the rivers.
        riverBlock.x = riverMakerPostion.X; // Assign the x-coordinate
        riverBlock.y = riverMakerPostion.Y; // Assign the y-coordinate
        riverBlock.riverNumber = riverNumber;//assign the riverMaker riverNumber

        //add the block to the riverArray
        M.riverNumberArray[riverMakerPostion.X, riverMakerPostion.Y] = riverNumber;

        internalRiverCounter++;

        // Add this river block to the World's list of river blocks.
        M.riverBlocks.Add(riverBlock);

        return riverBlock;
    }

    /*invert the direction for previous directions because previous direction would be oposite*/
    public CreateStuffSimpleFunctions.Direction InvertDirection(CreateStuffSimpleFunctions.Direction direction)
    {
        switch (direction)
        {
            case CreateStuffSimpleFunctions.Direction.right:
                return CreateStuffSimpleFunctions.Direction.left;
            case CreateStuffSimpleFunctions.Direction.left:
                return CreateStuffSimpleFunctions.Direction.right;
            case CreateStuffSimpleFunctions.Direction.up:
                return CreateStuffSimpleFunctions.Direction.down;
            case CreateStuffSimpleFunctions.Direction.down:
                return CreateStuffSimpleFunctions.Direction.up;
            default:
                return CreateStuffSimpleFunctions.Direction.NoDirection;
        }
    }

    public void ThreeWayRiverDiverge(int riverBranchChance, Position riverMakerPostion, CreateStuffSimpleFunctions.Direction movedDirection, CreateStuffSimpleFunctions.Direction left, CreateStuffSimpleFunctions.Direction right, CreateStuffSimpleFunctions.Direction straight)
    {
        if (riverBranchChance > Random.Range(0, 100))
        {
            if (movedDirection == left)
            {
                if (1 >= Random.Range(0, 2))
                {
                    RiverDiverge(right, riverMakerPostion);
                }
                else
                {
                    RiverDiverge(straight, riverMakerPostion);
                }
            }
            else if (movedDirection == right)
            {
                if (1 >= Random.Range(0, 2))
                {
                    RiverDiverge(straight, riverMakerPostion);
                }
                else
                {
                    RiverDiverge(left, riverMakerPostion);
                }
            }
            else if (movedDirection == straight)
            {
                if (1 >= Random.Range(0, 2))
                {
                    RiverDiverge(left, riverMakerPostion);
                }
                else
                {
                    RiverDiverge(right, riverMakerPostion);
                }
            }
            else
            {
                //don't diverge
            }
        }
    }

    public void TwoWayRiverDiverge(int riverBranchChance, Position riverMakerPostion, CreateStuffSimpleFunctions.Direction directionOne, CreateStuffSimpleFunctions.Direction directionTwo)
    {
        if (riverBranchChance > Random.Range(0, 100))
        {
            if (1 >= Random.Range(0, 2))
            {
                RiverDiverge(directionOne, riverMakerPostion);
            }
            else
            {
                RiverDiverge(directionTwo, riverMakerPostion);
            }
        }
        else
        {
            //don't diverge
        }
    }
    public void RiverDiverge(CreateStuffSimpleFunctions.Direction direction, Position riverMakerPostion)
    {/*
        Position nextPosition = GetNextPosition(riverMakerX, riverMakerY, direction);

        GameObject riverMaker = Instantiate(RiverMaker, new Vector3(nextPosition.X, nextPosition.Y, -1), Quaternion.identity);
        Debug.Log("created a diverged RiverMaker");

        RiverMakerScript riverMakerScript = riverMaker.GetComponent<RiverMakerScript>();
        riverMakerScript.riverNumber = riverNumber;
        riverMakerScript.riverFixedFlowDirection = direction;
        riverMakerScript.internalRiverCounter = 0;
        riverMakerScript.previousDirection = InvertDirection(direction);

        StartCoroutine(WaitOneFrame());

        //get the riverblock
        foreach (RiverBlock riverBlock in M.world.riverBlocks)
        {
            if (riverBlock.x == riverMakerX && riverBlock.y == riverMakerY)
            {
                // Add the RiverBlock to the Territory's rivers list.
                if(riverBlock.firstMergeDirection == CreateStuffSimpleFunctions.Direction.NoDirection)
                {
                    riverBlock.firstMergeDirection = direction;
                    Debug.Log("diverged riverblock a first direction");
                }
                else
                {
                    riverBlock.secondMergeDirection = direction;
                    Debug.Log("diverged riverblock a second direction");
                }
                break;
            }
        }*/
    }

    private IEnumerator WaitOneFrame()
    {
        // This line will cause Unity to wait until the next frame before executing the code after this line.
        yield return null;

        // The rest of your code here will be executed in the next frame.
    }

    /*create the riverBlock taking in all necessary variable for sprite generation*/

}


