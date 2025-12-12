using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapGenerationFunctions : MonoBehaviour
{
    public MapArrayScript M;
    public CreateStuffSimpleFunctions S;
    public CreateStuffAdvancedFunctions A;
    public GameObject landPrefab;
    public CreateStuff C;

    /*public int deepWaterElevation = -3;
    public int waterElevation = -2;
    public int shallowWaterElevation = -1;
    public int lowlandElevation = -1;
    public int landElevation = 0;
    public int sandElevation = 0;
    public int hillElevation = 1;
    public int highlandElevation = 2;
    public int mountainElevation = 3;
    public int peakElevation = 4;
    public int drylandElevation = 1;
    public int swampElevation = 0;
    public int swampLowlandElevation = -1;*/

   

    //Static Chances
    public int waterInitialChance = 100;
    public int waterChance = 20;
    public int shallowWaterChance = 20;
    public int shallowWaterInitialChance = 100;
    public int mountainMakerChance = 20;
    public int highlandMakerInitialChance = 90;
    public int highlandMakerChance = 40;
    public int hillMakerInitialChance = 100;
    public int hillMakerInitialLandChance = 5;
    public int hillMakerChance = 35;
    public int sandMakerInitialChance = 8;
    public int sandMakerChance = 80;
    public int lowlandMakerChance = 55;
    public int lowlandMakerInitialChance = 5;

    public void Grow(MapArrayScript.Blocktype mainBlock, MapArrayScript.Blocktype otherBlock, MapArrayScript.Blocktype otherOtherBlock, MapArrayScript.Blocktype fillBlock, GameObject FillBlock, int elevation, int repeat)
    {
        int spawnedFrom;
        bool mountainArray = false;
        while (repeat > 0)    //Repeat coordinate reroll until free spot if found or rerolls exceed 100 attempts
        {
            for (int x = 3; x < GameManager.Instance.mapSize - 1; x++)
            {
                for (int y = 3; y < GameManager.Instance.mapSize - 1; y++)
                {
                    if (fillBlock == MapArrayScript.Blocktype.Water | fillBlock == MapArrayScript.Blocktype.Shallowwater) { spawnedFrom = -2; }
                    else if (mainBlock == MapArrayScript.Blocktype.Nothing) { spawnedFrom = -1; }
                    else { spawnedFrom = S.SpawnedFrom(x, y); 
                    if (fillBlock == MapArrayScript.Blocktype.Mountain | fillBlock == MapArrayScript.Blocktype.Highland)
                        {
                            mountainArray = true;
                        }
                    }

                    if (S.Self(x, y) == mainBlock

                        & (S.Left(x, y) == otherBlock | S.Left(x, y) == otherOtherBlock)
                        & (S.Right(x, y) == otherBlock | S.Right(x, y) == otherOtherBlock)
                        & (S.Down(x, y) == otherBlock | S.Down(x, y) == otherOtherBlock))
                    {
                        S.CreateBlock(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                        if(mountainArray == true) { M.peakSpawnedFrom[x, y] = M.peakSpawnedFrom[x, y - 1];}
                    }
                    if (S.Self(x, y) == mainBlock

                        & (S.Up(x, y) == otherBlock | S.Up(x, y) == otherOtherBlock)
                        & (S.Down(x, y) == otherBlock | S.Down(x, y) == otherOtherBlock)
                        & (S.Right(x, y) == otherBlock | S.Right(x, y) == otherOtherBlock))
                    {
                        S.CreateBlock(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                        if (mountainArray == true) { M.peakSpawnedFrom[x, y] = M.peakSpawnedFrom[x + 1, y]; }
                    }
                    if (S.Self(x, y) == mainBlock

                        & (S.Up(x, y) == otherBlock | S.Up(x, y) == otherOtherBlock)
                        & (S.Right(x, y) == otherBlock | S.Right(x, y) == otherOtherBlock)
                        & (S.Left(x, y) == otherBlock | S.Left(x, y) == otherOtherBlock))
                    {
                        S.CreateBlock(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                        if (mountainArray == true) { M.peakSpawnedFrom[x, y] = M.peakSpawnedFrom[x, y + 1]; }
                    }
                    if (S.Self(x, y) == mainBlock

                        & (S.Up(x, y) == otherBlock | S.Up(x, y) == otherOtherBlock)
                        & (S.Left(x, y) == otherBlock | S.Left(x, y) == otherOtherBlock)
                        & (S.Down(x, y) == otherBlock | S.Down(x, y) == otherOtherBlock))
                    {
                        S.CreateBlock(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                        if (mountainArray == true) { M.peakSpawnedFrom[x, y] = M.peakSpawnedFrom[x - 1, y]; }
                    }
                }
            }
            repeat--;
        }
    }
    public void Fill(MapArrayScript.Blocktype mainBlock, MapArrayScript.Blocktype otherBlock, MapArrayScript.Blocktype otherOtherBlock, MapArrayScript.Blocktype fillBlock, GameObject FillBlock, int elevation, int repeat)
    {
        int spawnedFrom;
        bool mountainArray = false;
        while (repeat > 0)    //Repeat coordinate reroll until free spot if found or rerolls exceed 100 attempts
        {
            
            for (int x = 3; x < GameManager.Instance.mapSize - 3; x++)
            {
                for (int y = 3; y < GameManager.Instance.mapSize - 3; y++)
                {
                    if (fillBlock == MapArrayScript.Blocktype.Water | fillBlock == MapArrayScript.Blocktype.Shallowwater) 
                    { spawnedFrom = -2; }
                    else if (mainBlock == MapArrayScript.Blocktype.Nothing) { spawnedFrom = -1;}
                    else { spawnedFrom = S.SpawnedFrom(x, y);
                        if (fillBlock == MapArrayScript.Blocktype.Mountain | fillBlock == MapArrayScript.Blocktype.Highland)
                        {
                            mountainArray = true;
                        }
                    }

                    // fill lone spot
                    if (S.Self(x, y) == mainBlock)
                    {
                        if ((S.Right(x, y) == otherBlock | S.Right(x, y) == otherOtherBlock)
                            & (S.Left(x, y) == otherBlock | S.Left(x, y) == otherOtherBlock)
                            & (S.Up(x, y) == otherBlock | S.Up(x, y) == otherOtherBlock)
                            & (S.Down(x, y) == otherBlock | S.Down(x, y) == otherOtherBlock))
                        {
                            S.CreateBlock(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                            if (mountainArray == true) { M.peakSpawnedFrom[x, y] = M.peakSpawnedFrom[x - 1, y]; }
                        }

                        //if S.Self and below is alone, fill
                        if (S.Down(x, y) == mainBlock

                            & (S.Up(x, y) == otherBlock | S.Up(x, y) == otherOtherBlock)
                            & (S.Right(x, y) == otherBlock | S.Right(x, y) == otherOtherBlock)
                            & (S.RightDown(x, y) == otherBlock | S.RightDown(x, y) == otherOtherBlock)
                            & (S.Down2(x, y) == otherBlock | S.Down2(x, y) == otherOtherBlock)
                            & (S.LeftDown(x, y) == otherBlock | S.LeftDown(x, y) == otherOtherBlock)
                            & (S.Left(x, y) == otherBlock | S.Left(x, y) == otherOtherBlock))
                        {
                            S.CreateBlock(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                            S.CreateBlockDown(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                            if (mountainArray == true)
                            {
                                M.peakSpawnedFrom[x, y] = M.peakSpawnedFrom[x, y + 1];
                                M.peakSpawnedFrom[x, S.Down(y)] = M.peakSpawnedFrom[x, y - 2];
                            }
                        }

                        //if S.Self and left is alone, fill
                        if (S.Left(x, y) == mainBlock

                            & (S.Up(x, y) == otherBlock | S.Up(x, y) == otherBlock)
                            & (S.LeftDown(x, y) == otherBlock | S.LeftDown(x, y) == otherOtherBlock)
                            & (S.Left2(x, y) == otherBlock | S.Left2(x, y) == otherOtherBlock)
                            & (S.Down(x, y) == otherBlock | S.Down(x, y) == otherOtherBlock)
                            & (S.LeftUp(x, y) == otherBlock | S.LeftUp(x, y) == otherOtherBlock)
                            & (S.Right(x, y) == otherBlock | S.Right(x, y) == otherOtherBlock))
                        {
                            S.CreateBlock(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                            S.CreateBlockLeft(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                            if (mountainArray == true)
                            {
                                M.peakSpawnedFrom[x, y] = M.peakSpawnedFrom[x + 1, y];
                                M.peakSpawnedFrom[S.Left(x), y] = M.peakSpawnedFrom[x - 2, y];
                            }
                        }

                        //if 3 right angle, up and right is alone       
                        if (S.Up(x, y) == mainBlock
                             & S.Right(x, y) == mainBlock

                             & (S.Up2(x, y) == otherBlock | S.Up2(x, y) == otherOtherBlock)
                             & (S.RightUp(x, y) == otherBlock | S.RightUp(x, y) == otherOtherBlock)
                             & (S.Right2(x, y) == otherBlock | S.Right2(x, y) == otherOtherBlock)
                             & (S.RightDown(x, y) == otherBlock | S.RightDown(x, y) == otherOtherBlock)
                             & (S.Down(x, y) == otherBlock | S.Down(x, y) == otherOtherBlock)
                             & (S.Left(x, y) == otherBlock | S.Left(x, y) == otherOtherBlock)
                             & (S.LeftUp(x, y) == otherBlock | S.LeftUp(x, y) == otherOtherBlock))
                        {
                            S.CreateBlock(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                            S.CreateBlockUp(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                            S.CreateBlockRight(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                            if (mountainArray == true)
                            {
                                M.peakSpawnedFrom[x, y] = M.peakSpawnedFrom[x - 1, y];
                                M.peakSpawnedFrom[x, S.Up(y)] = M.peakSpawnedFrom[x, y + 2];
                                M.peakSpawnedFrom[S.Right(x), y] = M.peakSpawnedFrom[x + 2, y];
                            }
                        }
                        //if right angle, right and down is alone
                        if (S.Right(x, y) == mainBlock
                            & S.Down(x, y) == mainBlock

                            & (S.Up(x, y) == otherBlock | S.Up(x, y) == otherBlock)
                            & (S.RightUp(x, y) == otherBlock | S.RightUp(x, y) == otherOtherBlock)
                            & (S.Right2(x, y) == otherBlock | S.Right2(x, y) == otherOtherBlock)
                            & (S.RightDown(x, y) == otherBlock | S.RightDown(x, y) == otherOtherBlock)
                            & (S.Down2(x, y) == otherBlock | S.Down2(x, y) == otherOtherBlock)
                            & (S.LeftDown(x, y) == otherBlock | S.LeftDown(x, y) == otherOtherBlock)
                            & (S.Left(x, y) == otherBlock | S.Left(x, y) == otherOtherBlock))
                        {
                            S.CreateBlock(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                            S.CreateBlockRight(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                            S.CreateBlockDown(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                            if (mountainArray == true)
                            {
                                M.peakSpawnedFrom[x, y] = M.peakSpawnedFrom[x, y + 1];
                                M.peakSpawnedFrom[S.Right(x), y] = M.peakSpawnedFrom[x + 2, y];
                                M.peakSpawnedFrom[x, S.Down(y)] = M.peakSpawnedFrom[x, y - 2];
                            }
                        }

                        //if right angle, left and down is alone
                        if (S.Left(x, y) == mainBlock
                            & S.Down(x, y) == mainBlock

                            & (S.Up(x, y) == otherBlock | S.Up(x, y) == otherOtherBlock)
                            & (S.LeftUp(x, y) == otherBlock | S.LeftUp(x, y) == otherOtherBlock)
                            & (S.Right(x, y) == otherBlock | S.Right(x, y) == otherOtherBlock)
                            & (S.RightDown(x, y) == otherBlock | S.RightDown(x, y) == otherOtherBlock)
                            & (S.Down2(x, y) == otherBlock | S.Down2(x, y) == otherOtherBlock)
                            & (S.LeftDown(x, y) == otherBlock | S.LeftDown(x, y) == otherOtherBlock)
                            & (S.Left2(x, y) == otherBlock | S.Left2(x, y) == otherOtherBlock))
                        {
                            S.CreateBlock(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                            S.CreateBlockLeft(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                            S.CreateBlockDown(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                            if (mountainArray == true)
                            {
                                M.peakSpawnedFrom[x, y] = M.peakSpawnedFrom[x + 1, y];
                                M.peakSpawnedFrom[S.Left(x), y] = M.peakSpawnedFrom[x - 2, y];
                                M.peakSpawnedFrom[x, S.Down(y)] = M.peakSpawnedFrom[x, y - 2];
                            }
                        }
                        //if right angle, left and up is alone
                        if (S.Up(x, y) == mainBlock
                            & S.Left(x, y) == mainBlock

                            & (S.Up2(x, y) == otherBlock | S.Up2(x, y) == otherOtherBlock)
                            & (S.RightUp(x, y) == otherBlock | S.RightUp(x, y) == otherOtherBlock)
                            & (S.Right(x, y) == otherBlock | S.Right(x, y) == otherOtherBlock)
                            & (S.Down(x, y) == otherBlock | S.Down(x, y) == otherOtherBlock)
                            & (S.LeftDown(x, y) == otherBlock | S.LeftDown(x, y) == otherOtherBlock)
                            & (S.Left2(x, y) == otherBlock | S.Left2(x, y) == otherOtherBlock)
                            & (S.LeftUp(x, y) == otherBlock | S.LeftUp(x, y) == otherOtherBlock))
                        {
                            S.CreateBlock(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                            S.CreateBlockUp(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                            S.CreateBlockLeft(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                            if (mountainArray == true)
                            {
                                M.peakSpawnedFrom[x, y] = M.peakSpawnedFrom[x, y - 1];
                                M.peakSpawnedFrom[x, S.Up(y)] = M.peakSpawnedFrom[x, y + 2];
                                M.peakSpawnedFrom[S.Left(x), y] = M.peakSpawnedFrom[x - 2, y];
                            }
                        }

                        //if S.Self above and below
                        if (S.Up(x, y) == mainBlock
                            & S.Down(x, y) == mainBlock

                            & (S.Up2(x, y) == otherBlock | S.Up2(x, y) == otherOtherBlock)
                            & (S.RightUp(x, y) == otherBlock | S.RightUp(x, y) == otherOtherBlock)
                            & (S.Right(x, y) == otherBlock | S.Right(x, y) == otherOtherBlock)
                            & (S.RightDown(x, y) == otherBlock | S.RightDown(x, y) == otherOtherBlock)
                            & (S.Down2(x, y) == otherBlock | S.Down2(x, y) == otherOtherBlock)
                            & (S.LeftDown(x, y) == otherBlock | S.LeftDown(x, y) == otherOtherBlock)
                            & (S.Left(x, y) == otherBlock | S.Left(x, y) == otherOtherBlock)
                            & (S.LeftUp(x, y) == otherBlock | S.LeftUp(x, y) == otherOtherBlock))
                        {
                            S.CreateBlock(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                            S.CreateBlockUp(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                            S.CreateBlockDown(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                            if (mountainArray == true)
                            {
                                M.peakSpawnedFrom[x, y] = M.peakSpawnedFrom[x - 1, y];
                                M.peakSpawnedFrom[x, S.Up(y)] = M.peakSpawnedFrom[x, y + 2];
                                M.peakSpawnedFrom[x, S.Down(y)] = M.peakSpawnedFrom[x, y - 2];
                            }
                        }
                        //if S.Self left and right
                        if (S.Right(x, y) == mainBlock
                            & S.Left(x, y) == mainBlock

                            & (S.LeftUp(x, y) == otherBlock | S.LeftUp(x, y) == otherOtherBlock)
                            & (S.Up(x, y) == otherBlock | S.Up(x, y) == otherOtherBlock)
                            & (S.RightUp(x, y) == otherBlock | S.RightUp(x, y) == otherOtherBlock)
                            & (S.Right2(x, y) == otherBlock | S.Right2(x, y) == otherOtherBlock)
                            & (S.RightDown(x, y) == otherBlock | S.RightDown(x, y) == otherOtherBlock)
                            & (S.Down(x, y) == otherBlock | S.Down(x, y) == otherOtherBlock)
                            & (S.LeftDown(x, y) == otherBlock | S.LeftDown(x, y) == otherOtherBlock)
                            & (S.Left2(x, y) == otherBlock | S.Left2(x, y) == otherOtherBlock))
                        {
                            S.CreateBlock(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                            S.CreateBlockRight(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                            S.CreateBlockLeft(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                            if (mountainArray == true)
                            {
                                M.peakSpawnedFrom[x, y] = M.peakSpawnedFrom[x, y + 1];
                                M.peakSpawnedFrom[S.Right(x), y] = M.peakSpawnedFrom[x + 2, y];
                                M.peakSpawnedFrom[S.Left(x), y] = M.peakSpawnedFrom[x - 2, y];
                            }
                        }

                        //if 2x2 block
                        if (S.Down(x, y) == mainBlock
                            & S.LeftDown(x, y) == mainBlock
                            & S.Left(x, y) == mainBlock

                            & (S.Up(x, y) == otherBlock | S.Up(x, y) == otherOtherBlock)
                            & (S.LeftUp(x, y) == otherBlock | S.LeftUp(x, y) == otherOtherBlock)
                            & (S.Right(x, y) == otherBlock | S.Right(x, y) == otherOtherBlock)
                            & (S.RightDown(x, y) == otherBlock | S.RightDown(x, y) == otherOtherBlock)
                            & (S.Down2(x, y) == otherBlock | S.Down2(x, y) == otherOtherBlock)
                            & (S.LeftDown2(x, y) == otherBlock | S.LeftDown2(x, y) == otherOtherBlock)
                            & (S.Left2(x, y) == otherBlock | S.Left2(x, y) == otherOtherBlock)
                            & (S.Left2Down(x, y) == otherBlock | S.Left2Down(x, y) == otherOtherBlock))
                        {
                            S.CreateBlock(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                            S.CreateBlockDown(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                            S.CreateBlockLeftDown(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                            S.CreateBlockLeft(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                            if(mountainArray == true)
                            {
                                M.peakSpawnedFrom[x, y] = M.peakSpawnedFrom[x - 1, y];
                                M.peakSpawnedFrom[x, S.Down(y)] = M.peakSpawnedFrom[x - 2, y];
                                M.peakSpawnedFrom[S.Left(x), S.Down(y)] = M.peakSpawnedFrom[x - 2, y];
                                M.peakSpawnedFrom[S.Left(x), y] = M.peakSpawnedFrom[x - 1, y + 1];
                            }
                        }

                        //2x3 verticle
                        if (S.Up(x, y) == mainBlock
                            & S.RightUp(x, y) == mainBlock
                            & S.Down(x, y) == mainBlock
                            & S.RightDown(x, y) == mainBlock
                            & S.Right(x, y) == mainBlock

                            & (S.Up2(x, y) == otherBlock | S.Up2(x, y) == otherOtherBlock)
                            & (S.RightUp2(x, y) == otherBlock | S.RightUp2(x, y) == otherOtherBlock)
                            & (S.Right2(x, y) == otherBlock | S.Right2(x, y) == otherOtherBlock)
                            & (S.Right2Up(x, y) == otherBlock | S.Right2Up(x, y) == otherOtherBlock)
                            & (S.Right2Down(x, y) == otherBlock | S.Right2Down(x, y) == otherOtherBlock)
                            & (S.RightDown2(x, y) == otherBlock | S.RightDown2(x, y) == otherOtherBlock)
                            & (S.Down2(x, y) == otherBlock | S.Down2(x, y) == otherOtherBlock)
                            & (S.LeftDown(x, y) == otherBlock | S.LeftDown(x, y) == otherOtherBlock)
                            & (S.Left(x, y) == otherBlock | S.Left(x, y) == otherOtherBlock)
                            & (S.LeftUp(x, y) == otherBlock | S.LeftUp(x, y) == otherOtherBlock))
                        {
                            S.CreateBlock(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                            S.CreateBlockUp(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                            S.CreateBlockRightUp(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                            S.CreateBlockDown(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                            S.CreateBlockRightDown(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                            S.CreateBlockRight(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                            if (mountainArray == true)
                            {
                                M.peakSpawnedFrom[x, y] = M.peakSpawnedFrom[x - 1, y];
                                M.peakSpawnedFrom[x, S.Up(y)] = M.peakSpawnedFrom[x, y + 2];
                                M.peakSpawnedFrom[S.Right(x), S.Up(y)] = M.peakSpawnedFrom[x + 1, y + 2];
                                M.peakSpawnedFrom[x, S.Down(y)] = M.peakSpawnedFrom[x, y - 2];
                                M.peakSpawnedFrom[S.Right(x), S.Down(y)] = M.peakSpawnedFrom[x + 1, y - 2];
                                M.peakSpawnedFrom[S.Right(x), y] = M.peakSpawnedFrom[x + 2, y];
                            }
                        }
                        //2x3 horizontal
                        if (S.Right(x, y) == mainBlock
                            & S.RightDown(x, y) == mainBlock
                            & S.Left(x, y) == mainBlock
                            & S.LeftDown(x, y) == mainBlock
                            & S.Down(x, y) == mainBlock

                            & (S.Up(x, y) == otherBlock | S.Up(x, y) == otherOtherBlock)
                            & (S.RightUp(x, y) == otherBlock | S.RightUp(x, y) == otherOtherBlock)
                            & (S.LeftUp(x, y) == otherBlock | S.LeftUp(x, y) == otherOtherBlock)
                            & (S.Right2(x, y) == otherBlock | S.Right2(x, y) == otherOtherBlock)
                            & (S.Right2Down(x, y) == otherBlock | S.Right2Down(x, y) == otherOtherBlock)
                            & (S.Down2(x, y) == otherBlock | S.Down2(x, y) == otherOtherBlock)
                            & (S.LeftDown2(x, y) == otherBlock | S.LeftDown2(x, y) == otherOtherBlock)
                            & (S.RightDown2(x, y) == otherBlock | S.RightDown2(x, y) == otherOtherBlock)
                            & (S.Left2(x, y) == otherBlock | S.Left2(x, y) == otherOtherBlock)
                            & (S.Left2Down(x, y) == otherBlock | S.Left2Down(x, y) == otherOtherBlock))
                        {
                            S.CreateBlock(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                            S.CreateBlockRight(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                            S.CreateBlockRightDown(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                            S.CreateBlockLeft(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                            S.CreateBlockLeftDown(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                            S.CreateBlockDown(fillBlock, FillBlock, x, y, elevation, spawnedFrom);
                            if (mountainArray == true) 
                            { M.peakSpawnedFrom[x, y] = M.peakSpawnedFrom[x, y + 1];
                            M.peakSpawnedFrom[S.Right(x), y] = M.peakSpawnedFrom[x + 2, y]; 
                            M.peakSpawnedFrom[S.Right(x), S.Down(y)] = M.peakSpawnedFrom[x + 2, y - 1]; 
                            M.peakSpawnedFrom[S.Left(x), y] = M.peakSpawnedFrom[x - 2, y]; 
                            M.peakSpawnedFrom[S.Left(x), S.Down(y)] = M.peakSpawnedFrom[x - 2, y - 1]; 
                            M.peakSpawnedFrom[x, S.Down(y)] = M.peakSpawnedFrom[x, y - 2]; }
                        }
                    }
                }
            }
            repeat--;
        }
    }
    public void SpawnNextTo(GameObject SpawnBlock, MapArrayScript.Blocktype mainBlock, MapArrayScript.Blocktype otherBlock, MapArrayScript.Blocktype spawnBlock, int elevation, int blockChance)
    {
        int spawnedFrom;
        bool mountainArray = false;
        for (int x = 1; x < GameManager.Instance.mapSize - 1; x++)
        {
            for (int y = 1; y < GameManager.Instance.mapSize - 1; y++)
            {
                if (spawnBlock == MapArrayScript.Blocktype.Water | spawnBlock == MapArrayScript.Blocktype.Shallowwater) { spawnedFrom = -2; } 
                else { spawnedFrom = S.SpawnedFrom(x, y); 
                    if (spawnBlock == MapArrayScript.Blocktype.Highland)
                    {
                        mountainArray = true;
                    }
                }
                if (S.Self(x, y) == mainBlock)
                {
                    int roll = Random.Range(0, 100);
                    if (roll < blockChance
                        & S.Left(x, y) == otherBlock)
                    {
                        S.CreateBlock(spawnBlock, SpawnBlock, S.Left(x), y, elevation, spawnedFrom);
                        if (mountainArray == true) { M.peakSpawnedFrom[x - 1, y] = M.peakSpawnedFrom[x, y]; }
                    }
                    roll = Random.Range(0, 100);
                    if (roll < blockChance
                        & S.Right(x, y) == otherBlock)
                    {
                        S.CreateBlock(spawnBlock, SpawnBlock, S.Right(x), y, elevation, spawnedFrom);
                        if (mountainArray == true) { M.peakSpawnedFrom[x + 1, y] = M.peakSpawnedFrom[x, y]; }
                    }
                    roll = Random.Range(0, 100);
                    if (roll < blockChance
                        & S.Up(x, y) == otherBlock)
                    {
                        S.CreateBlock(spawnBlock, SpawnBlock, x, S.Up(y), elevation, spawnedFrom);
                        if (mountainArray == true) { M.peakSpawnedFrom[x, y + 1] = M.peakSpawnedFrom[x, y]; }
                    }
                    roll = Random.Range(0, 100);
                    if (roll < blockChance
                        & S.Down(x, y) == otherBlock)
                    {
                        S.CreateBlock(spawnBlock, SpawnBlock, x, S.Down(y), elevation, spawnedFrom);
                        if (mountainArray == true) { M.peakSpawnedFrom[x, y - 1] = M.peakSpawnedFrom[x, y]; }
                    }
                }
            }
        }
    }
    public void EmptySpaceFill(GameObject DeepWaterMaker, int elevation)
    {
        for (int x = 0; x < GameManager.Instance.mapSize; x++)
        {
            for (int y = 0; y < GameManager.Instance.mapSize; y++)
            {
                if (S.Self(x, y) == 0)
                {
                    //deepWaterCounter++;
                    S.CreateBlock(MapArrayScript.Blocktype.Deepwater, DeepWaterMaker, x, y, elevation, -2);
                }
            }
        }
    }
    public void SpawnInOpenspace(GameObject SpawnBlock, MapArrayScript.Blocktype mainBlock, MapArrayScript.Blocktype spawnBlock, int elevation, int blockChance)
    {
        for (int x = 2; x < GameManager.Instance.mapSize - 2; x++)
        {
            for (int y = 2; y < GameManager.Instance.mapSize - 2; y++)
            {

                //spawn on land
                if (S.Self(x, y) == mainBlock
                    & (S.Up(x, y) == mainBlock)
                    & (S.Up2(x, y) == mainBlock)
                    & (S.LeftUp(x, y) == mainBlock)
                    & (S.RightUp(x, y) == mainBlock)
                    & (S.Up2Right2(x, y) == mainBlock)
                    & (S.Left2Up2(x, y) == mainBlock)
                    & (S.LeftUp2(x, y) == mainBlock)
                    & (S.RightUp2(x, y) == mainBlock)
                    & (S.Down(x, y) == mainBlock)
                    & (S.Down2(x, y) == mainBlock)
                    & (S.RightDown(x, y) == mainBlock)
                    & (S.LeftDown(x, y) == mainBlock)
                    & (S.Right2Down2(x, y) == mainBlock)
                    & (S.Right2Down2(x, y) == mainBlock)
                    & (S.RightDown2(x, y) == mainBlock)
                    & (S.LeftDown2(x, y) == mainBlock)
                    & (S.Left(x, y) == mainBlock)
                    & (S.Left2(x, y) == mainBlock)
                    & (S.Left2Up(x, y) == mainBlock)
                    & (S.Right2Down(x, y) == mainBlock)
                    & (S.Up(x, y) == mainBlock)
                    & (S.Right2(x, y) == mainBlock)
                    & (S.Right2Down(x, y) == mainBlock)
                    & (S.Right2Up(x, y) == mainBlock))
                {
                    int roll = Random.Range(0, 1000);
                    if (roll < blockChance)
                    {
                        S.CreateBlock(spawnBlock, SpawnBlock, x, y, elevation, S.SpawnedFrom(x, y));
                        S.CreateBlockRight(spawnBlock, SpawnBlock, x, y, elevation, S.SpawnedFrom(x, y));
                        S.CreateBlockLeft(spawnBlock, SpawnBlock, x, y, elevation, S.SpawnedFrom(x, y));
                        S.CreateBlockUp(spawnBlock, SpawnBlock, x, y, elevation, S.SpawnedFrom(x, y));
                        S.CreateBlockDown(spawnBlock, SpawnBlock, x, y, elevation, S.SpawnedFrom(x, y));
                    }
                }
            }
        }
    }
}
