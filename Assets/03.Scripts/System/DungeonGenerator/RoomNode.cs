using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNode
{
    private RoomNode _left;
    private RoomNode _right;

    private RectInt _spaceSize;
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

    public int SpaceWidth => _spaceSize.width;
    public int SpaceHeight => _spaceSize.height;
    public Vector2Int BottomLeftPosition => new Vector2Int(_spaceSize.xMin, _spaceSize.yMin);
    public Vector2Int TopRightPosition => new Vector2Int(_spaceSize.xMax, _spaceSize.yMax);
    public Vector2Int RoomPosition => _roomSize.position;
    public Vector2Int RoomScale => new Vector2Int(_roomSize.width, _roomSize.height);
    public Vector2 SpaceCenter => _spaceSize.center;

    public bool IsLeaf => (null == _left) && (null == _right);

    public eSplitDirection SplittedDirection { get; set; }

    public RoomNode(RectInt size)
    {
        _spaceSize = size;
    }

    public void InitRoomSizeBySpace()
    {
        int deltaX = Random.Range(1, Mathf.FloorToInt(_spaceSize.width / 3.0f));
        int deltaY = Random.Range(1, Mathf.FloorToInt(_spaceSize.height / 3.0f));
        int x = _spaceSize.x + deltaX;
        int y = _spaceSize.y + deltaY;
        int w = _spaceSize.width - deltaX;      //이동한 좌표만큼 빼줘서 길이를 맞춰준다.
        int h = _spaceSize.height - deltaY;

        w -= Random.Range(1, w / 3);
        h -= Random.Range(1, h / 3);

        _roomSize = new RectInt(x, y, w, h);
    }
}