using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[Serializable]
public class Recipe
{
    public BlockType[] Blocks;
}

public class Test
{
    const int localX = 10;
    const int localY = 20;

    BlockType[,] blockTypes = new BlockType[localX, localY];

}

public enum BlockType
{
    None,
    Filled
}