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

    BlockType[,] blockTypes = new BlockType[localX, localY]; //이게 초기값이어서 변수를 지정하면 안된다. 상수로 해야한다.

 
}

public enum BlockType
{
    None,
    Filled
}