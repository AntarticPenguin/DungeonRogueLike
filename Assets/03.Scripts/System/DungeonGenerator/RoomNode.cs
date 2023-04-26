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
    public Vector2 SpaceCenter => _spaceSize.center;
    public Vector2Int BottomLeftAnchor => new Vector2Int(_spaceSize.xMin, _spaceSize.yMin);
    public Vector2Int TopRightAnchor => new Vector2Int(_spaceSize.xMax, _spaceSize.yMax);
    public ref RectInt RoomSize => ref _roomSize;

    public bool IsLeaf => (null == _left) && (null == _right);
    public bool HasLeaf { get; set; }
    public string RoomName { get; set; }

    public Vector3 DoorPosition { get; private set; }
    public Quaternion DoorRotation { get; private set; }

    public RoomNode(RectInt size)
    {
        _spaceSize = size;
    }

    public void InitRoomSizeBySpace(int minPadding, int maxPadding)
    {
        int deltaX = Random.Range(minPadding, Mathf.FloorToInt(_spaceSize.width / (float)maxPadding));
        int deltaY = Random.Range(minPadding, Mathf.FloorToInt(_spaceSize.height / (float)maxPadding));
        int x = _spaceSize.x + deltaX;
        int y = _spaceSize.y + deltaY;
        int w = _spaceSize.width - deltaX;      //이동한 좌표만큼 빼줘서 길이를 맞춰준다.
        int h = _spaceSize.height - deltaY;

        w -= Random.Range(minPadding, w / maxPadding);
        h -= Random.Range(minPadding, h / maxPadding);

        //홀,짝수 길이 보정
        int roomWidth = (w % 2 == 0) ? w - 1 : w;
        int roomHeight = (h % 2 == 0) ? h - 1 : h;

        _roomSize = new RectInt(x, y, roomWidth, roomHeight);
    }

    public void CalculateDoorPosition(Vector2 from, Vector2 to)
    {
        if(from.x < to.x)
        {
            //왼쪽 방: 오른쪽 모서리에 배치. 문은 왼쪽방향
            DoorPosition = new Vector3(RoomSize.xMax, 0, to.y);
            DoorRotation = Quaternion.Euler(0f, -90f, 0f);
        }
        else if(from.x > to.x)
        {
            //오른쪽 방: 왼쪽 모서리에 배치. 문은 오른쪽 방향
            DoorPosition = new Vector3(RoomSize.xMin, 0, to.y);
            DoorRotation = Quaternion.Euler(0f, 90f, 0f);
        }
        else if(from.y < to.y)
        {
            //아래쪽 방: 위쪽 모서리에 배치. 문은 아래 방향
            DoorPosition = new Vector3(to.x, 0, RoomSize.yMax);
            DoorRotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else if(from.y > to.y)
        {
            //위쪽 방: 아래쪽 모서리에 배치. 문은 위쪽 방향
            DoorPosition = new Vector3(to.x, 0, RoomSize.yMin);
            DoorRotation = Quaternion.identity;
        }
    }
}