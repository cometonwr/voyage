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

public enum BlockType
{
    Blank, 
    Fire,
    Water
}