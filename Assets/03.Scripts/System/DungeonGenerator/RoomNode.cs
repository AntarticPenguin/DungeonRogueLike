using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNode
{
    private RoomNode _left;
    private RoomNode _right;

    private RectInt _roomSize;

    public RoomNode Left
    {
        get { return _left; }
        set { _left = value; }
    }
    public RoomNode Right
    {
        get { return _right; }
        set { _right = value; }
    }

    public int Width => _roomSize.width;
    public int Height => _roomSize.height;
    public Vector2Int BottomLeftPosition => new Vector2Int(_roomSize.xMin, _roomSize.yMin);
    public Vector2Int TopRightPosition => new Vector2Int(_roomSize.xMax, _roomSize.yMax);

    public eSplitDirection SplittedDirection { get; set; }

    public RoomNode(RectInt size)
    {
        _roomSize = size;
    }
}