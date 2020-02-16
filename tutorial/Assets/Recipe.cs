using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[Serializable]
public class Recipe
{
    public int Row;
    public int Column;

    public BlockType[] Blocks;


}

public class Test
{
    int localX = 10;
    int localY = 20;

    BlockType[,] blockTypes = new BlockType[localX, localY];

}

public enum BlockType
{
    None,
    Filled
}