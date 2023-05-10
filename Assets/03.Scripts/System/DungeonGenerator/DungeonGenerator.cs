using System;
using System.Collections;
using System.Collections.Generic;
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

    private float _minDivideRatio;
    private float _maxDivideRatio;
    private List<RoomNode> leafNodes = new List<RoomNode>();    //리프노드 = 실제 생성될 룸노드

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
        ConnectRooms();

        //복도 생성(방 연결)
        for (int i = 0; i < leafNodes.Count; i++)
        {
            DrawCorridor(leafNodes[i]);
        }
    }

    private void InitRoomInfo(RoomNode treeNode)
    {
        TraverseNode(treeNode);

        for (int i = 0; i < leafNodes.Count; i++)
        {
            leafNodes[i].InitRoomSizeBySpace(_minRoomPadding, _maxRoomPadding);
            leafNodes[i].RoomName = string.Format("{0} Node", i);
        }

        for (int i = 0; i < leafNodes.Count; i++)
        {
            FindAndAddNeighborNode(leafNodes[i]);
        }
    }

    private void FindAndAddNeighborNode(RoomNode node)
    {
        List<KeyValuePair<RoomNode, float>> distanceWithNodeList = new List<KeyValuePair<RoomNode, float>>();
        List<RoomNode> neighborNodes = new List<RoomNode>();

        //룸 상하좌우 위치 비교해서 가장 짧은거리가 _mapSize * Mathf.pow(_minDivi, _maxIter) 보다 작을 경우
        float limitDistance = _mapSize * Mathf.Pow(_maxDivideRatio, _maxIterations);
        Debug.Log($"limit distance: {limitDistance}");

        for (int i = 0; i < leafNodes.Count; i++)
        {
            if (leafNodes[i] == node)
                continue;

            float distance = node.RoomSize.GetShortestDistance(leafNodes[i].RoomSize);
            Debug.Log($"{leafNodes[i].RoomName}, {node.RoomName}'s distance: {distance}");

            if (distance <= limitDistance)
            {
                neighborNodes.Add(leafNodes[i]);
            }
        }

        if(neighborNodes.Count == 0)
        {
            Debug.LogError("이웃 노드가 없음!!");
        }
        else
        {
            foreach (RoomNode neighbor in neighborNodes)
            {
                node.AddNeighborNode(neighbor);
            }
        }
    }

    private void CreateRooms()
    {
        for (int i = 0; i < leafNodes.Count; i++)
        {
            RectInt room = leafNodes[i].RoomSize;
            int tileSize = 2;
            float tilePivot = 0.5f;

            GameObject parentObject = new GameObject($"{leafNodes[i].RoomName}");

            for (int y = room.yMin; y < room.yMax; y += tileSize)
            {
                for (int x = room.xMin; x < room.xMax; x += tileSize)
                {
                    GameObject instance = Instantiate(_tilePrefab, parentObject.transform);
                    instance.transform.position = new Vector3(x + tilePivot, 0, y + tilePivot);
                    instance.transform.localScale = Vector3.one;
                }
            }

            CreatRoomEdge(leafNodes[i]);
        }
    }

    private void ConnectRooms()
    {
        for (int i = 0; i < leafNodes.Count; i++)
        {
            var neighborNodes = leafNodes[i].NeighborNodes;

            for (int j = 0; j < neighborNodes.Count; j++)
            {
                leafNodes[i].ConnectRoom(neighborNodes[j]);
            }
        }
    }

    private bool CheckDoorPosition(RoomNode node, int wallXPos, int wallZPos, bool isHorizontal)
    {
        for(int i = 0; i < node.DoorInfos.Count; i++)
        {
            Vector3 doorPos = node.DoorInfos[i]._doorPosition;

            int doorXMin = Mathf.CeilToInt(doorPos.x - 3);
            int doorXMax = Mathf.FloorToInt(doorPos.x + 3);
            int doorZMin = Mathf.CeilToInt(doorPos.z - 3);
            int doorZMax = Mathf.FloorToInt(doorPos.z + 3);
            
            if(isHorizontal)
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

    private void CreatRoomEdge(RoomNode roomNode)
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
                CreatWall(xPos, zPos, -180f);
            }
        }

        //Bottom edge
        for (int i = 0; i < width + 1; i++)
        {
            xPos = size.xMin + i * wallSize;
            zPos = size.yMin;

            if (!CheckDoorPosition(roomNode, xPos, zPos, isHorizontal: true))
            {
                CreatWall(xPos, zPos, 0f);
            }
        }

        //Left side edge
        for (int i = 0; i < height - 1; i++)
        {
            xPos = size.xMin;
            zPos = size.yMin + (i+1) * wallSize;

            if (!CheckDoorPosition(roomNode, xPos, zPos, isHorizontal: false))
            {
                CreatWall(xPos, zPos, 90f);
            }
        }

        //Right side edge
        for (int i = 0; i < height - 1; i++)
        {
            xPos = size.xMax;
            zPos = size.yMin + (i+1) * wallSize;

            if (!CheckDoorPosition(roomNode, xPos, zPos, isHorizontal: false))
            {
                CreatWall(xPos, zPos, -90f);
            }
        }
    }

    private void CreatWall(int xPos, int zPos, float angle)
    {
        GameObject instance = Instantiate(_wallPrefab);
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
            leafNodes.Add(node);
        }
            
        TraverseNode(node.Right);
    }

    private void DrawCorridor(RoomNode node)
    {
        //복도 문 생성
        for(int i = 0; i < node.DoorInfos.Count; i++)
        {
            GameObject instance = Instantiate(_doorwayPrefab);
            instance.gameObject.name = node.DoorInfos[i]._name;
            instance.transform.position = node.DoorInfos[i]._doorPosition;
            instance.transform.rotation = node.DoorInfos[i]._doorRotation;
            instance.transform.localScale = Vector3.one;
        }
        
        //TODO: 게임 에셋으로 교체할 부분
    }
}
