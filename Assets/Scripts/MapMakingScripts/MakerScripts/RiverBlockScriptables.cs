using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[CreateAssetMenu(fileName = "RiverBlock", menuName = "ScriptableObjects/RiverBlock", order = 1)]
public class RiverBlock : ScriptableObject
{
    [JsonIgnore]
    public Sprite sprite;
    public Quaternion rotation;

    public CreateStuffSimpleFunctions.Direction previousRiverDirection;
    public CreateStuffSimpleFunctions.Direction nextRiverDirection;
    public CreateStuffSimpleFunctions.Direction firstMergeDirection;
    public CreateStuffSimpleFunctions.Direction secondMergeDirection;

    public int riverNumber;
    public int internalRiverNumber;
    public int x;
    public int y;

    public int[] ToArray()
    {
        return new int[]
        {
            x,
            y,
            (int)previousRiverDirection, // Assuming Direction is an enum
            (int)nextRiverDirection,     // Convert enum to int
            (int)firstMergeDirection,
            (int)secondMergeDirection,
            riverNumber,
            internalRiverNumber
        };
    }

    // Method to populate RiverBlock data from an array
    public void FromArray(int[] arrayData)
    {
        x = arrayData[0];
        y = arrayData[1];
        previousRiverDirection = (CreateStuffSimpleFunctions.Direction)arrayData[2]; // Convert int back to enum
        nextRiverDirection = (CreateStuffSimpleFunctions.Direction)arrayData[3];
        firstMergeDirection = (CreateStuffSimpleFunctions.Direction)arrayData[4];
        secondMergeDirection = (CreateStuffSimpleFunctions.Direction)arrayData[5];
        riverNumber = arrayData[6];
        internalRiverNumber = arrayData[7];
    }

    public void DetermineSprite()
    {
        if(secondMergeDirection != CreateStuffSimpleFunctions.Direction.NoDirection)
        {
            sprite = Resources.Load<Sprite>("RiverSprites/RiverQuad");
            rotation = Quaternion.Euler(0, 0, 270);
        }
        else
        {
            if (firstMergeDirection != CreateStuffSimpleFunctions.Direction.NoDirection)
            {
                RiverMerger();
            }
            else 
            { 
                if (previousRiverDirection == CreateStuffSimpleFunctions.Direction.NoDirection)
                {
                    RiverStart();
                }
                else if (nextRiverDirection == CreateStuffSimpleFunctions.Direction.NoDirection)
                {
                    RiverEnd();
                }
                else
                {
                    RiverStandard();
                }
            }
        }
    }

    public void RiverStart()
    {
        if (nextRiverDirection == CreateStuffSimpleFunctions.Direction.left)
        {
            sprite = Resources.Load<Sprite>("RiverSprites/RiverStart");
            rotation = Quaternion.Euler(0, 0, 270);
        }
        else if (nextRiverDirection == CreateStuffSimpleFunctions.Direction.right)
        {
            sprite = Resources.Load<Sprite>("RiverSprites/RiverStart");
            rotation = Quaternion.Euler(0, 0, 90);
        }
        else if (nextRiverDirection == CreateStuffSimpleFunctions.Direction.up)
        {
            sprite = Resources.Load<Sprite>("RiverSprites/RiverStart");
            rotation = Quaternion.Euler(0, 0, 180);
        }
        else if (nextRiverDirection == CreateStuffSimpleFunctions.Direction.down)
        {
            sprite = Resources.Load<Sprite>("RiverSprites/RiverStart");
            rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            Debug.LogError("start river sprite error");
        }
    }

    public void RiverEnd()
    {
        if (previousRiverDirection == CreateStuffSimpleFunctions.Direction.left)
        {
            sprite = Resources.Load<Sprite>("RiverSprites/RiverEnd");
            rotation = Quaternion.Euler(0, 0, 270);
        }
        else if (previousRiverDirection == CreateStuffSimpleFunctions.Direction.right)
        {
            sprite = Resources.Load<Sprite>("RiverSprites/RiverEnd");
            rotation = Quaternion.Euler(0, 0, 90);
        }
        else if (previousRiverDirection == CreateStuffSimpleFunctions.Direction.up)
        {
            sprite = Resources.Load<Sprite>("RiverSprites/RiverEnd");
            rotation = Quaternion.Euler(0, 0, 180);
        }
        else if (previousRiverDirection == CreateStuffSimpleFunctions.Direction.down)
        {
            sprite = Resources.Load<Sprite>("RiverSprites/RiverEnd");
            rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            Debug.LogError("end river sprite error");
        }
    }

    public void RiverMerger()
    {
        //rivermerger
        if (previousRiverDirection != CreateStuffSimpleFunctions.Direction.right &&
            nextRiverDirection != CreateStuffSimpleFunctions.Direction.right &&
            firstMergeDirection != CreateStuffSimpleFunctions.Direction.right)
        {
            sprite = Resources.Load<Sprite>("RiverSprites/RiverTriple");
            rotation = Quaternion.Euler(0, 0, 180);
        }
        else if (previousRiverDirection != CreateStuffSimpleFunctions.Direction.left &&
            nextRiverDirection != CreateStuffSimpleFunctions.Direction.left &&
            firstMergeDirection != CreateStuffSimpleFunctions.Direction.left)
        {
            sprite = Resources.Load<Sprite>("RiverSprites/RiverTriple");
            rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (previousRiverDirection != CreateStuffSimpleFunctions.Direction.up &&
            nextRiverDirection != CreateStuffSimpleFunctions.Direction.up &&
            firstMergeDirection != CreateStuffSimpleFunctions.Direction.up)
        {
            sprite = Resources.Load<Sprite>("RiverSprites/RiverTriple");
            rotation = Quaternion.Euler(0, 0, 270);
        }
        else if (previousRiverDirection != CreateStuffSimpleFunctions.Direction.down &&
            nextRiverDirection != CreateStuffSimpleFunctions.Direction.down &&
            firstMergeDirection != CreateStuffSimpleFunctions.Direction.down)
        {
            sprite = Resources.Load<Sprite>("RiverSprites/RiverTriple");
            rotation = Quaternion.Euler(0, 0, 90);
        }
        else
        {
            Debug.LogError("merge river sprite error");
        }
    }

    public void RiverStandard()
    {
        //river straight
        if (previousRiverDirection == CreateStuffSimpleFunctions.Direction.right && nextRiverDirection == CreateStuffSimpleFunctions.Direction.left
        || previousRiverDirection == CreateStuffSimpleFunctions.Direction.left && nextRiverDirection == CreateStuffSimpleFunctions.Direction.right)
        {
            sprite = Resources.Load<Sprite>("RiverSprites/RiverStraight");
            rotation = Quaternion.Euler(0, 0, 90);
        }
        else if (previousRiverDirection == CreateStuffSimpleFunctions.Direction.up && nextRiverDirection == CreateStuffSimpleFunctions.Direction.down
         || previousRiverDirection == CreateStuffSimpleFunctions.Direction.down && nextRiverDirection == CreateStuffSimpleFunctions.Direction.up)
        {
            sprite = Resources.Load<Sprite>("RiverSprites/RiverStraight");
            rotation = Quaternion.Euler(0, 0, 0);
        }
        //river Bend
        else if (previousRiverDirection == CreateStuffSimpleFunctions.Direction.up && nextRiverDirection == CreateStuffSimpleFunctions.Direction.right ||
                 previousRiverDirection == CreateStuffSimpleFunctions.Direction.right && nextRiverDirection == CreateStuffSimpleFunctions.Direction.up)
        {
            sprite = Resources.Load<Sprite>("RiverSprites/RiverBend");
            rotation = Quaternion.Euler(0, 0, 90);
            // Debug.Log("up right");
        }
        else if (previousRiverDirection == CreateStuffSimpleFunctions.Direction.up && nextRiverDirection == CreateStuffSimpleFunctions.Direction.left ||
                 previousRiverDirection == CreateStuffSimpleFunctions.Direction.left && nextRiverDirection == CreateStuffSimpleFunctions.Direction.up)
        {
            sprite = Resources.Load<Sprite>("RiverSprites/RiverBend");
            rotation = Quaternion.Euler(0, 0, 180);
            // Debug.Log("up left");
        }
        else if (previousRiverDirection == CreateStuffSimpleFunctions.Direction.down && nextRiverDirection == CreateStuffSimpleFunctions.Direction.left ||
                 previousRiverDirection == CreateStuffSimpleFunctions.Direction.left && nextRiverDirection == CreateStuffSimpleFunctions.Direction.down)
        {
            sprite = Resources.Load<Sprite>("RiverSprites/RiverBend");
            rotation = Quaternion.Euler(0, 0, 270);
            //Debug.Log("down left");
        }
        else if (previousRiverDirection == CreateStuffSimpleFunctions.Direction.down && nextRiverDirection == CreateStuffSimpleFunctions.Direction.right ||
                 previousRiverDirection == CreateStuffSimpleFunctions.Direction.right && nextRiverDirection == CreateStuffSimpleFunctions.Direction.down)
        {

            sprite = Resources.Load<Sprite>("RiverSprites/RiverBend");
            //Debug.Log("down right");
        }
        else
        {
            Debug.LogError("standard river sprite error");
        }
    }
}