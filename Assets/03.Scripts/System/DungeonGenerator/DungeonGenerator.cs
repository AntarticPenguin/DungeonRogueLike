using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// 던전 생성 클래스
/// 방을 만들고, 처음 공간을 분할하고, 복도를 연결해주는 클래스
/// </summary>
public class DungeonGenerator : MonoBehaviour
{
    [Header("Dungeon Size")]
    [SerializeField] int _mapSize;

    [Header("Settings")]
    [SerializeField, Range(1, 6)]
    private int _maxIterations = 4;

    [SerializeField, Range(0.3f, 0.7f)]
    private float _divideRatio = 0.45f;

    [SerializeField] int _minRoomPadding = 1;
    [SerializeField] int _maxRoomPadding = 3;

    [Header("Assets")]
    [SerializeField] GameObject _tilePrefab;
    [SerializeField] GameObject _wallPrefab;
    [SerializeField] GameObject _doorwayPrefab;
    [SerializeField] GameObject _backfillPrefab;
    [SerializeField] GameObject _corridor1mPrefab;

    private float _minDivideRatio;
    private float _maxDivideRatio;
    private List<RoomNode> _leafNodes = new List<RoomNode>();    //리프노드 = 실제 생성될 룸노드

    private readonly int _DOOR_LENGTH = 6;

    private void Start()
    {
        GenerateDungeon();
    }

    private void GenerateDungeon()
    {
        RectInt dungeonSize = new RectInt(0, 0, _mapSize, _mapSize);
        _minDivideRatio = Mathf.Min(_divideRatio, 1 - _divideRatio);
        _maxDivideRatio = 1 - _minDivideRatio;

        //BSP를 이용한 공간 분할
        BinarySpacePartitioner bsp = new BinarySpacePartitioner(dungeonSize, _minDivideRatio, _maxDivideRatio);
        RoomNode treeNode = bsp.CreateRoomTree(_maxIterations);

        InitRoomInfo(treeNode);

        //만들어진 트리를 이용해 룸 생성
        CreateRooms();

        //복도 생성(방 연결)
        for (int i = 0; i < _leafNodes.Count; i++)
        {
            DrawCorridor(_leafNodes[i]);
        }
    }

    private void InitRoomInfo(RoomNode treeNode)
    {
        TraverseNode(treeNode);

        for (int i = 0; i < _leafNodes.Count; i++)
        {
            _leafNodes[i].InitRoomSizeBySpace(_minRoomPadding, _maxRoomPadding);
            _leafNodes[i].RoomName = string.Format("{0} Node", i);
        }

        for (int i = 0; i < _leafNodes.Count; i++)
        {
            FindAndAddNeighborNode(_leafNodes[i]);
        }

        ConnectRooms();
    }

    private void FindAndAddNeighborNode(RoomNode curNode)
    {
        var nodesByDirection = new List<Tuple<RoomNode, float>>[4];      //상하좌우
        for(int i = 0; i < 4; i++)
        {
            nodesByDirection[i] = new List<Tuple<RoomNode, float>>();
        }

        RectInt from = curNode.RoomSize;

        foreach (RoomNode node in _leafNodes)
        {
            if(curNode == node)
            {
                continue;
            }

            RectInt to = node.RoomSize;
            eRelativeRectDirection relativeDirection = to.DistinguishRectPosition(from);
            switch (relativeDirection)
            {
                case eRelativeRectDirection.LEFT:
                    nodesByDirection[0].Add(new Tuple<RoomNode, float>(node, Mathf.Abs(to.xMax - from.xMin)));
                    break;
                case eRelativeRectDirection.RIGHT:
                    nodesByDirection[1].Add(new Tuple<RoomNode, float>(node, Mathf.Abs(to.xMin - from.xMax)));
                    break;
                case eRelativeRectDirection.DOWN:
                    nodesByDirection[2].Add(new Tuple<RoomNode, float>(node, Mathf.Abs(to.yMax - from.yMin)));
                    break;
                case eRelativeRectDirection.UP:
                    nodesByDirection[3].Add(new Tuple<RoomNode, float>(node, Mathf.Abs(to.yMin - from.yMax)));
                    break;
                case eRelativeRectDirection.NONE:
                default:
                    break;
            }
        }

        foreach(List<Tuple<RoomNode, float>> neighborNodes in nodesByDirection)
        {
            neighborNodes.Sort((lhs, rhs)=>
            {
                return lhs.Item2.CompareTo(rhs.Item2);
            });

            if (0 != neighborNodes.Count && CanMakeDoor(from, neighborNodes[0].Item1.RoomSize, _DOOR_LENGTH))
            {
                curNode.NeighborNodes.Add(neighborNodes[0].Item1);
            }
        }
    }

    private bool CanMakeDoor(RectInt from, RectInt to, int doorLength)
    {
        int doorXMin = Mathf.Max(from.xMin, to.xMin);
        int doorXMax = Mathf.Min(from.xMax, to.xMax);
        int doorYMin = Mathf.Max(from.yMin, to.yMin);
        int doorYMax = Mathf.Min(from.yMax, to.yMax);

        bool canMakeDoor = false;

        eRelativeRectDirection relative = from.DistinguishRectPosition(to);
        if (eRelativeRectDirection.LEFT == relative || eRelativeRectDirection.RIGHT == relative)
        {
            canMakeDoor = Mathf.Abs(doorYMax - doorYMin) >= doorLength;
        }
        else if (eRelativeRectDirection.UP == relative || eRelativeRectDirection.DOWN == relative)
        {
            canMakeDoor = Mathf.Abs(doorXMax - doorXMin) >= doorLength;
        }

        return canMakeDoor;
    }

    private void ConnectRooms()
    {
        for (int i = 0; i < _leafNodes.Count; i++)
        {
            var neighborNodes = _leafNodes[i].NeighborNodes;

            for (int j = 0; j < neighborNodes.Count; j++)
            {
                _leafNodes[i].ConnectRoom(neighborNodes[j]);
            }
        }
    }

    private void CreateRooms()
    {
        for (int i = 0; i < _leafNodes.Count; i++)
        {
            RectInt room = _leafNodes[i].RoomSize;
            int tileSize = 2;
            float tilePivot = 0.5f;

            GameObject parentObject = new GameObject($"{_leafNodes[i].RoomName}");

            for (int y = room.yMin; y < room.yMax; y += tileSize)
            {
                for (int x = room.xMin; x < room.xMax; x += tileSize)
                {
                    GameObject instance = Instantiate(_tilePrefab, parentObject.transform);
                    instance.transform.position = new Vector3(x + tilePivot, 0, y + tilePivot);
                    instance.transform.localScale = Vector3.one;
                }
            }

            CreatRoomEdge(_leafNodes[i], parentObject);
        }
    }

    private void CreatRoomEdge(RoomNode roomNode, GameObject parentObject)
    {
        int wallSize = 1;
        RectInt size = roomNode.RoomSize;
        int width = size.width;
        int height = size.height;

        int xPos = 0;
        int zPos = 0;

        //Top edge
        for (int i = 0; i < width + 1; i++)
        {
            xPos = size.xMin + i * wallSize;
            zPos = size.yMax;

            if (!CheckDoorPosition(roomNode, xPos, zPos, isHorizontal: true))
            {
                CreatWall(xPos, zPos, -180f, parentObject);
            }
        }

        //Bottom edge
        for (int i = 0; i < width + 1; i++)
        {
            xPos = size.xMin + i * wallSize;
            zPos = size.yMin;

            if (!CheckDoorPosition(roomNode, xPos, zPos, isHorizontal: true))
            {
                CreatWall(xPos, zPos, 0f, parentObject);
            }
        }

        //Left side edge
        for (int i = 0; i < height - 1; i++)
        {
            xPos = size.xMin;
            zPos = size.yMin + (i+1) * wallSize;

            if (!CheckDoorPosition(roomNode, xPos, zPos, isHorizontal: false))
            {
                CreatWall(xPos, zPos, 90f, parentObject);
            }
        }

        //Right side edge
        for (int i = 0; i < height - 1; i++)
        {
            xPos = size.xMax;
            zPos = size.yMin + (i+1) * wallSize;

            if (!CheckDoorPosition(roomNode, xPos, zPos, isHorizontal: false))
            {
                CreatWall(xPos, zPos, -90f, parentObject);
            }
        }
    }

    private bool CheckDoorPosition(RoomNode node, int wallXPos, int wallZPos, bool isHorizontal)
    {
        for (int i = 0; i < node.DoorInfos.Count; i++)
        {
            Vector3 doorPos = node.DoorInfos[i]._doorPosition;

            int doorXMin = Mathf.CeilToInt(doorPos.x - 3);
            int doorXMax = Mathf.FloorToInt(doorPos.x + 3);
            int doorZMin = Mathf.CeilToInt(doorPos.z - 3);
            int doorZMax = Mathf.FloorToInt(doorPos.z + 3);

            if (isHorizontal)
            {
                if ((doorXMin < wallXPos && wallXPos < doorXMax) && ((int)doorPos.z == wallZPos))
                {
                    return true;
                }
            }
            else
            {
                if ((doorZMin < wallZPos && wallZPos < doorZMax) && ((int)doorPos.x == wallXPos))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void CreatWall(int xPos, int zPos, float angle, GameObject parentObject)
    {
        GameObject instance = Instantiate(_wallPrefab, parentObject.transform);
        instance.transform.position = new Vector3(xPos, 0, zPos);
        instance.transform.eulerAngles = new Vector3(0, angle, 0);
        instance.transform.localScale = Vector3.one;
    }

    private void TraverseNode(RoomNode node)
    {
        if (null == node)
            return;

        TraverseNode(node.Left);

        if (node.IsLeaf)
        {
            _leafNodes.Add(node);
        }
            
        TraverseNode(node.Right);
    }

    private void DrawCorridor(RoomNode node)
    {
        foreach(DoorInfo doorInfo in node.DoorInfos)
        {
            //복도 문 생성
            GameObject instance = Instantiate(_doorwayPrefab);
            instance.gameObject.name = doorInfo._name;
            instance.transform.position = doorInfo._doorPosition;
            instance.transform.rotation = doorInfo._doorRotation;
            instance.transform.localScale = Vector3.one;

            if (doorInfo._hasCorridor)
            {
                //TODO: 방향을 알아야 포지션 수정함. x? z?
                Vector3 newPos = doorInfo._doorPosition;
                for (int i = 0; i < doorInfo._corridorLength; i++)
                {
                    GameObject corridorInstance = Instantiate(_corridor1mPrefab);
                    switch (doorInfo._doorDirection)
                    { 
                        case eRelativeRectDirection.LEFT:
                            newPos.x -= 1;
                            break;
                        case eRelativeRectDirection.RIGHT:
                            newPos.x += 1;
                            break;
                        case eRelativeRectDirection.DOWN:
                            newPos.z -= 1;
                            break;
                        case eRelativeRectDirection.UP:
                            newPos.z += 1;
                            break;
                        case eRelativeRectDirection.NONE:
                        default:
                            break;
                    }

                    corridorInstance.transform.position = newPos;
                    corridorInstance.transform.rotation = doorInfo._doorRotation;
                    corridorInstance.transform.localScale = Vector3.one;
                }
            }
        }
    }
}
