using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateStuffSimpleFunctions : MonoBehaviour
{
    public MapArrayScript M;
    public enum Direction
    {
        NoDirection, right, left, up, down,
    };
    public Direction SelfD(int x, int y, Direction[,] blockType)
    {
        Direction theBlock;
        theBlock = blockType[x, y];
        return theBlock;
    }
    public MapArrayScript.Blocktype Self(int x, int y)
    {
        MapArrayScript.Blocktype theBlock;
        theBlock = M.blockType[x, y];
        return theBlock;

    }
    public MapArrayScript.Blocktype Right(int x, int y)
    {
        MapArrayScript.Blocktype theBlock;
        theBlock = M.blockType[Right(x), y];
        return theBlock;
    }
    public MapArrayScript.Blocktype Left(int x, int y)
    {
        MapArrayScript.Blocktype theBlock;
        theBlock = M.blockType[Left(x), y];
        return theBlock;
    }
    public MapArrayScript.Blocktype Up(int x, int y)
    {
        MapArrayScript.Blocktype theBlock;
        theBlock = M.blockType[x, Up(y)];
        return theBlock;
    }
    public MapArrayScript.Blocktype Down(int x, int y)
    {
        MapArrayScript.Blocktype theBlock;
        theBlock = M.blockType[x, Down(y)];
        return theBlock;
    }
    public MapArrayScript.Blocktype RightUp(int x, int y)
    {
        MapArrayScript.Blocktype theBlock;
        theBlock = M.blockType[Right(x), Up(y)];
        return theBlock;
    }
    public MapArrayScript.Blocktype LeftUp(int x, int y)
    {
        MapArrayScript.Blocktype theBlock;
        theBlock = M.blockType[Left(x), Up(y)];
        return theBlock;
    }
    public MapArrayScript.Blocktype RightDown(int x, int y)
    {
        MapArrayScript.Blocktype theBlock;
        theBlock = M.blockType[Right(x), Down(y)];
        return theBlock;
    }
    public MapArrayScript.Blocktype LeftDown(int x, int y)
    {
        MapArrayScript.Blocktype theBlock;
        theBlock = M.blockType[Left(x), Down(y)];
        return theBlock;
    }
    public MapArrayScript.Blocktype Right2(int x, int y)
    {
        MapArrayScript.Blocktype theBlock;
        theBlock = M.blockType[Right2(x), y];
        return theBlock;
    }
    public MapArrayScript.Blocktype Left2(int x, int y)
    {
        MapArrayScript.Blocktype theBlock;
        theBlock = M.blockType[Left2(x), y];
        return theBlock;
    }
    public MapArrayScript.Blocktype Up2(int x, int y)
    {
        MapArrayScript.Blocktype theBlock;
        theBlock = M.blockType[x, Up2(y)];
        return theBlock;
    }
    public MapArrayScript.Blocktype Down2(int x, int y)
    {
        MapArrayScript.Blocktype theBlock;
        theBlock = M.blockType[x, Down2(y)];
        return theBlock;
    }
    public MapArrayScript.Blocktype Up2Right2(int x, int y)
    {
        MapArrayScript.Blocktype theBlock;
        theBlock = M.blockType[Right2(x), Up2(y)];
        return theBlock;
    }
    public MapArrayScript.Blocktype Left2Up2(int x, int y)
    {
        MapArrayScript.Blocktype theBlock;
        theBlock = M.blockType[Left2(x), Up2(y)];
        return theBlock;
    }
    public MapArrayScript.Blocktype Right2Down2(int x, int y)
    {
        MapArrayScript.Blocktype theBlock;
        theBlock = M.blockType[Right2(x), Down2(y)];
        return theBlock;
    }
    public MapArrayScript.Blocktype Left2Down2(int x, int y)
    {
        MapArrayScript.Blocktype theBlock;
        theBlock = M.blockType[Left2(x), Down2(y)];
        return theBlock;
    }
    public MapArrayScript.Blocktype Right2Up(int x, int y)
    {
        MapArrayScript.Blocktype theBlock;
        theBlock = M.blockType[Right2(x), Up(y)];
        return theBlock;
    }
    public MapArrayScript.Blocktype Right2Down(int x, int y)
    {
        MapArrayScript.Blocktype theBlock;
        theBlock = M.blockType[Right2(x), Down(y)];
        return theBlock;
    }
    public MapArrayScript.Blocktype Left2Up(int x, int y)
    {
        MapArrayScript.Blocktype theBlock;
        theBlock = M.blockType[Left2(x), Up(y)];
        return theBlock;
    }
    public MapArrayScript.Blocktype Left2Down(int x, int y)
    {
        MapArrayScript.Blocktype theBlock;
        theBlock = M.blockType[Left2(x), Down(y)];
        return theBlock;
    }
    public MapArrayScript.Blocktype RightUp2(int x, int y)
    {
        MapArrayScript.Blocktype theBlock;
        theBlock = M.blockType[Right(x), Up2(y)];
        return theBlock;
    }
    public MapArrayScript.Blocktype LeftUp2(int x, int y)
    {
        MapArrayScript.Blocktype theBlock;
        theBlock = M.blockType[Left(x), Up2(y)];
        return theBlock;
    }
    public MapArrayScript.Blocktype RightDown2(int x, int y)
    {
        MapArrayScript.Blocktype theBlock;
        theBlock = M.blockType[Right(x), Down2(y)];
        return theBlock;
    }
    public MapArrayScript.Blocktype LeftDown2(int x, int y)
    {
        MapArrayScript.Blocktype theBlock;
        theBlock = M.blockType[Left(x), Down2(y)];
        return theBlock;
    }

    //Get position relative to block
    public int Right(int x)
    {
        int right = x + 1;
        return right;
    }
    public int Left(int x)
    {
        int left = x - 1;
        return left;
    }
    public int Up(int y)
    {
        int up = y + 1;
        return up;
    }
    public int Down(int y)
    {
        int down = y - 1;
        return down;
    }
    public int Right2(int x)
    {
        int right = x + 2;
        return right;
    }
    public int Left2(int x)
    {
        int left = x - 2;
        return left;
    }
    public int Up2(int y)
    {
        int up = y + 2;
        return up;
    }
    public int Down2(int y)
    {
        int down = y - 2;
        return down;
    }
    public int SpawnedFrom(int x, int y)
    {
        int spawnedFrom = M.spawnedFrom[x, y];
        return spawnedFrom;
    }

    //CreateBlocks
    public void CreateBlock(MapArrayScript.Blocktype fillBlock, GameObject Block, int x, int y, int elevation, int spawnedFrom)
    {
        Instantiate(Block, new Vector3(x, y, elevation), Quaternion.identity);
        M.blockType[x, y] = fillBlock;
        M.spawnedFrom[x, y] = spawnedFrom;
    }
    public void CreateBlockRight(MapArrayScript.Blocktype fillBlock, GameObject Block, int x, int y, int elevation, int spawnedFrom)
    {
        Instantiate(Block, new Vector3(Right(x), y, elevation), Quaternion.identity);
        M.blockType[Right(x), y] = fillBlock;
        M.spawnedFrom[Right(x), y] = spawnedFrom;
    }
    public void CreateBlockLeft(MapArrayScript.Blocktype fillBlock, GameObject Block, int x, int y, int elevation, int spawnedFrom)
    {
        Instantiate(Block, new Vector3(Left(x), y, elevation), Quaternion.identity);
        M.blockType[Left(x), y] = fillBlock;
        M.spawnedFrom[Left(x), y] = spawnedFrom;
    }
    public void CreateBlockUp(MapArrayScript.Blocktype fillBlock, GameObject Block, int x, int y, int elevation, int spawnedFrom)
    {
        Instantiate(Block, new Vector3(x, Up(y), elevation), Quaternion.identity);
        M.blockType[x, Up(y)] = fillBlock;
        M.spawnedFrom[x, Up(y)] = spawnedFrom;
    }
    public void CreateBlockDown(MapArrayScript.Blocktype fillBlock, GameObject Block, int x, int y, int elevation, int spawnedFrom)
    {
        Instantiate(Block, new Vector3(x, Down(y), elevation), Quaternion.identity);
        M.blockType[x, Down(y)] = fillBlock;
        M.spawnedFrom[x, Down(y)] = spawnedFrom;
    }
    public void CreateBlockRightUp(MapArrayScript.Blocktype fillBlock, GameObject Block, int x, int y, int elevation, int spawnedFrom)
    {
        Instantiate(Block, new Vector3(Right(x), Up(y), elevation), Quaternion.identity);
        M.blockType[Right(x), Up(y)] = fillBlock;
        M.spawnedFrom[Right(x), Up(y)] = spawnedFrom;
    }
    public void CreateBlockRightDown(MapArrayScript.Blocktype fillBlock, GameObject Block, int x, int y, int elevation, int spawnedFrom)
    {
        Instantiate(Block, new Vector3(Right(x), Down(y), elevation), Quaternion.identity);
        M.blockType[Right(x), Down(y)] = fillBlock;
        M.spawnedFrom[Right(x), Down(y)] = spawnedFrom;
    }
    public void CreateBlockLeftUp(MapArrayScript.Blocktype fillBlock, GameObject Block, int x, int y, int elevation, int spawnedFrom)
    {
        Instantiate(Block, new Vector3(Left(x), Up(y), elevation), Quaternion.identity);
        M.blockType[Left(x), Up(y)] = fillBlock;
        M.spawnedFrom[Left(x), Up(y)] = spawnedFrom;
    }
    public void CreateBlockLeftDown(MapArrayScript.Blocktype fillBlock, GameObject Block, int x, int y, int elevation, int spawnedFrom)
    {
        Instantiate(Block, new Vector3(Left(x), Down(y), elevation), Quaternion.identity);
        M.blockType[Left(x), Down(y)] = fillBlock;
        M.spawnedFrom[Left(x), Down(y)] = spawnedFrom;
    }

}

