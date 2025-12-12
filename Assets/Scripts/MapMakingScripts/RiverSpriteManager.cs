using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using static MapArrayScript;

public class RiverSpriteManager : MonoBehaviour
{
    public Sprite riverStartSprite;
    public Sprite riverStartSpriteLeft;
    public Sprite riverStartSpriteUp;
    public Sprite riverStartSpriteRight;

    public Sprite riverEndSprite;
    public Sprite riverEndSpriteLeft;
    public Sprite riverEndSpriteUp;
    public Sprite riverEndSpriteRight;

    public Sprite riverStraightSprite;
    public Sprite riverStraightSpriteHorizontal;

    public Sprite riverBendSprite;
    public Sprite riverBendSpriteDL;
    public Sprite riverBendSpriteLU;
    public Sprite riverBendSpriteUR;

    public Sprite riverTripleSprite;
    public Sprite riverTripleSprite1;
    public Sprite riverTripleSprite2;
    public Sprite riverTripleSprite3;

    public Sprite riverQuadSprite;
    public Sprite DetermineSprite(RiverBlockData riverData)
    {
        Sprite riverSprite;
        if (riverData.secondMergeDirection != CreateStuffSimpleFunctions.Direction.NoDirection)
        {
            riverSprite = riverQuadSprite;
        }
        else
        {
            if (riverData.firstMergeDirection != CreateStuffSimpleFunctions.Direction.NoDirection)
            {
                riverSprite = RiverMerger(riverData);
            }
            else
            {
                if (riverData.previousRiverDirection == CreateStuffSimpleFunctions.Direction.NoDirection)
                {
                    riverSprite = RiverStart(riverData);
                }
                else if (riverData.nextRiverDirection == CreateStuffSimpleFunctions.Direction.NoDirection)
                {
                    riverSprite = RiverEnd(riverData);
                }
                else
                {
                    riverSprite = RiverStandard(riverData);
                }
            }
        }

        return riverSprite;
    }

    public Sprite RiverStart(RiverBlockData riverData)
    {
        Sprite riverSprite;
        if (riverData.nextRiverDirection == CreateStuffSimpleFunctions.Direction.left)
        {
            riverSprite = riverStartSpriteRight;
        }
        else if (riverData.nextRiverDirection == CreateStuffSimpleFunctions.Direction.right)
        {
            riverSprite = riverStartSpriteLeft;
        }
        else if (riverData.nextRiverDirection == CreateStuffSimpleFunctions.Direction.up)
        {
            riverSprite = riverStartSpriteUp;
        }
        else if (riverData.nextRiverDirection == CreateStuffSimpleFunctions.Direction.down)
        {
            riverSprite = riverStartSprite;
        }
        else
        {
            Debug.LogError("start river sprite error");
            riverSprite = null;
        }
        return riverSprite;
    }

    public Sprite RiverEnd(RiverBlockData riverData)
    {
        Sprite riverSprite;
        if (riverData.previousRiverDirection == CreateStuffSimpleFunctions.Direction.left)
        {
            riverSprite = riverEndSpriteRight;
        }
        else if (riverData.previousRiverDirection == CreateStuffSimpleFunctions.Direction.right)
        {
            riverSprite = riverEndSpriteLeft;
        }
        else if (riverData.previousRiverDirection == CreateStuffSimpleFunctions.Direction.up)
        {
            riverSprite = riverEndSpriteUp;
        }
        else if (riverData.previousRiverDirection == CreateStuffSimpleFunctions.Direction.down)
        {
            riverSprite = riverEndSprite;
        }
        else
        {
            Debug.LogError("end river sprite error");
            riverSprite = null;
        }
        return riverSprite;
    }

    public Sprite RiverMerger(RiverBlockData riverData)
    {
        Sprite riverSprite;
        if (riverData.previousRiverDirection != CreateStuffSimpleFunctions.Direction.right &&
            riverData.nextRiverDirection != CreateStuffSimpleFunctions.Direction.right &&
            riverData.firstMergeDirection != CreateStuffSimpleFunctions.Direction.right)
        {
            riverSprite = riverTripleSprite2;
        }
        else if (riverData.previousRiverDirection != CreateStuffSimpleFunctions.Direction.left &&
            riverData.nextRiverDirection != CreateStuffSimpleFunctions.Direction.left &&
            riverData.firstMergeDirection != CreateStuffSimpleFunctions.Direction.left)
        {
            riverSprite = riverTripleSprite;
        }
        else if (riverData.previousRiverDirection != CreateStuffSimpleFunctions.Direction.up &&
            riverData.nextRiverDirection != CreateStuffSimpleFunctions.Direction.up &&
            riverData.firstMergeDirection != CreateStuffSimpleFunctions.Direction.up)
        {
            riverSprite = riverTripleSprite3;
        }
        else if (riverData.previousRiverDirection != CreateStuffSimpleFunctions.Direction.down &&
            riverData.nextRiverDirection != CreateStuffSimpleFunctions.Direction.down &&
            riverData.firstMergeDirection != CreateStuffSimpleFunctions.Direction.down)
        {
            riverSprite = riverTripleSprite1;
        }
        else
        {
            Debug.LogError("merge river sprite error");
            riverSprite = null;
        }
        return riverSprite;
    }

    public Sprite RiverStandard(RiverBlockData riverData)
    {
        Sprite riverSprite;
        if (riverData.previousRiverDirection == CreateStuffSimpleFunctions.Direction.right && riverData.nextRiverDirection == CreateStuffSimpleFunctions.Direction.left
        || riverData.previousRiverDirection == CreateStuffSimpleFunctions.Direction.left && riverData.nextRiverDirection == CreateStuffSimpleFunctions.Direction.right)
        {
            riverSprite = riverStraightSpriteHorizontal;
        }
        else if (riverData.previousRiverDirection == CreateStuffSimpleFunctions.Direction.up && riverData.nextRiverDirection == CreateStuffSimpleFunctions.Direction.down
         || riverData.previousRiverDirection == CreateStuffSimpleFunctions.Direction.down && riverData.nextRiverDirection == CreateStuffSimpleFunctions.Direction.up)
        {
            riverSprite = riverStraightSprite;
        }
        //river Bend
        else if (riverData.previousRiverDirection == CreateStuffSimpleFunctions.Direction.up && riverData.nextRiverDirection == CreateStuffSimpleFunctions.Direction.right ||
                 riverData.previousRiverDirection == CreateStuffSimpleFunctions.Direction.right && riverData.nextRiverDirection == CreateStuffSimpleFunctions.Direction.up)
        {
            riverSprite = riverBendSpriteDL;
            // Debug.Log("up right");
        }
        else if (riverData.previousRiverDirection == CreateStuffSimpleFunctions.Direction.up && riverData.nextRiverDirection == CreateStuffSimpleFunctions.Direction.left ||
                 riverData.previousRiverDirection == CreateStuffSimpleFunctions.Direction.left && riverData.nextRiverDirection == CreateStuffSimpleFunctions.Direction.up)
        {
            riverSprite = riverBendSpriteLU;
            // Debug.Log("up left");
        }
        else if (riverData.previousRiverDirection == CreateStuffSimpleFunctions.Direction.down && riverData.nextRiverDirection == CreateStuffSimpleFunctions.Direction.left ||
                 riverData.previousRiverDirection == CreateStuffSimpleFunctions.Direction.left && riverData.nextRiverDirection == CreateStuffSimpleFunctions.Direction.down)
        {
            riverSprite = riverBendSpriteUR;
            //Debug.Log("down left");
        }
        else if (riverData.previousRiverDirection == CreateStuffSimpleFunctions.Direction.down && riverData.nextRiverDirection == CreateStuffSimpleFunctions.Direction.right ||
                 riverData.previousRiverDirection == CreateStuffSimpleFunctions.Direction.right && riverData.nextRiverDirection == CreateStuffSimpleFunctions.Direction.down)
        {

            riverSprite = riverBendSprite;
            //Debug.Log("down right");
        }
        else
        {
            Debug.LogError("standard river sprite error");
            riverSprite = null;
        }
        return riverSprite;
    }
}
