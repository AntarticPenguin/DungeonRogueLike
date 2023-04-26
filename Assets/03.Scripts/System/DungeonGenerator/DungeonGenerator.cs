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
    [SerializeField] int _mapWidth;
    [SerializeField] int _mapSize;

    [Header("Settings")]
    [SerializeField, Range(1, 6)]
    int _maxIterations = 4;

    [SerializeField, Range(0.3f, 0.7f)]
    float _minDivideRatio = 0.45f;

    [SerializeField] int _minRoomPadding = 1;
    [SerializeField] int _maxRoomPadding = 3;

    [Header("Assets")]
    [SerializeField] GameObject _tilePrefab;
    [SerializeField] GameObject _wallPrefab;
    [SerializeField] GameObject _doorwayPrefab;
    [SerializeField] GameObject _backfillPrefab;

    private void Start()
    {
        GenerateDungeon();
    }

    private void GenerateDungeon()
    {
        RectInt dungeonSize = new RectInt(0, 0, _mapWidth, _mapSize);

        //BSP를 이용한 공간 분할
        BinarySpacePartitioner bsp = new BinarySpacePartitioner(dungeonSize, _minDivideRatio, 1 - _minDivideRatio);
        RoomNode treeNode = bsp.CreateRoomTree(_maxIterations);

        InitRoomInfo(treeNode);

        //만들어진 트리를 이용해 룸 생성
        CreateRooms();

        //복도 생성(방 연결)
        DrawCorridor(treeNode);
    }


    List<RoomNode> leafNodes = new List<RoomNode>();    //리프노드 = 실제 생성될 룸노드
    List<RoomNode> doorNodes = new List<RoomNode>();    //룸노드 중, 방을 연결해주는 노드(복도와 복도 아님)
    private void InitRoomInfo(RoomNode treeNode)
    {
        TraverseNode(treeNode);

        for(int i = 0; i < leafNodes.Count; i++)
        {
            leafNodes[i].InitRoomSizeBySpace(_minRoomPadding, _maxRoomPadding);
        }

        for(int i = 0; i < doorNodes.Count; i++)
        {
            RoomNode node = doorNodes[i];
            node.Left.CalculateDoorPosition(node.Left.SpaceCenter, node.Right.SpaceCenter);
            node.Right.CalculateDoorPosition(node.Right.SpaceCenter, node.Left.SpaceCenter);
        }
    }

    private void CreateRooms()
    {
        for (int i = 0; i < leafNodes.Count; i++)
        {
            RectInt room = leafNodes[i].RoomSize;
            int tileSize = 2;
            float tilePivot = 0.5f;

            for (int y = room.yMin; y < room.yMax; y += tileSize)
            {
                for (int x = room.xMin; x < room.xMax; x += tileSize)
                {
                    GameObject instance = Instantiate(_tilePrefab);
                    instance.transform.position = new Vector3(x + tilePivot, 0, y + tilePivot);
                    instance.transform.localScale = Vector3.one;
                }
            }

            CreateWalls(leafNodes[i]);
        }
    }

    private void CreateWalls(RoomNode roomNode)
    {
        int wallWidth = 1;
        RectInt size = roomNode.RoomSize;
        int width = size.width;
        int height = size.height;

        int xPos = 0;
        int zPos = 0;
        int doorXMin = Mathf.CeilToInt(roomNode.DoorPosition.x - 3);
        int doorXMax = Mathf.FloorToInt(roomNode.DoorPosition.x + 3);
        int doorZMin = Mathf.CeilToInt(roomNode.DoorPosition.z - 3);
        int doorZMax = Mathf.FloorToInt(roomNode.DoorPosition.z + 3);

        //Top edge
        for (int i = 0; i <= width; i++)
        {
            xPos = size.xMin + i * wallWidth;
            zPos = size.yMax;

            if((doorXMin < xPos && xPos < doorXMax) && ((int)roomNode.DoorPosition.z == zPos))
            {
                continue;
            }

            GameObject instance = Instantiate(_wallPrefab);
            instance.transform.position = new Vector3(xPos, 0, size.yMax);
            instance.transform.eulerAngles = new Vector3(0, -180, 0);
            instance.transform.localScale = Vector3.one;
        }

        //Bottom edge
        for (int i = 0; i <= width; i++)
        {
            xPos = size.xMin + i * wallWidth;
            zPos = size.yMin;

            if ((doorXMin < xPos && xPos < doorXMax) && ((int)roomNode.DoorPosition.z == zPos))
            {
                continue;
            }

            GameObject instance = Instantiate(_wallPrefab);
            instance.transform.position = new Vector3(xPos, 0, zPos);
            instance.transform.localScale = Vector3.one;
        }

        //Left side edge
        for (int i = height - 1; i > 0; i--)
        {
            xPos = size.xMin;
            zPos = size.yMin + i * wallWidth;

            if ((doorZMin < zPos && zPos < doorZMax) && ((int)roomNode.DoorPosition.x == xPos))
            {
                continue;
            }

            GameObject instance = Instantiate(_wallPrefab);
            instance.transform.position = new Vector3(xPos, 0, zPos);
            instance.transform.eulerAngles = new Vector3(0, 90, 0);
            instance.transform.localScale = Vector3.one;
        }

        //Right side edge
        for (int i = height - 1; i > 0; i--)
        {
            xPos = size.xMax;
            zPos = size.yMin + i * wallWidth;

            if ((doorZMin < zPos && zPos < doorZMax) && ((int)roomNode.DoorPosition.x == xPos))
            {
                continue;
            }

            GameObject instance = Instantiate(_wallPrefab);
            instance.transform.position = new Vector3(xPos, 0, zPos);
            instance.transform.eulerAngles = new Vector3(0, -90, 0);
            instance.transform.localScale = Vector3.one;
        }
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

        if(node.HasLeaf)
        {
            doorNodes.Add(node);
        }
            
        TraverseNode(node.Right);
    }

    private void DrawCorridor(RoomNode node)
    {
        if(node.IsLeaf)
        {
            return;
        }

        //복도 문 생성
        if (node.HasLeaf)
        {
            {
                GameObject instance = Instantiate(_doorwayPrefab);
                instance.transform.position = node.Left.DoorPosition;
                instance.transform.rotation = node.Left.DoorRotation;
                instance.transform.localScale = Vector3.one;
            }
            {
                GameObject instance = Instantiate(_doorwayPrefab);
                instance.transform.position = node.Right.DoorPosition;
                instance.transform.rotation = node.Right.DoorRotation;
                instance.transform.localScale = Vector3.one;
            }
        }

        //TODO: 게임 에셋으로 교체할 부분
        //복도 생성
        {
            GameObject go = Resources.Load<GameObject>("TestLineRenderer");
            GameObject instance = Instantiate(go);
            LineRenderer line = instance.GetComponent<LineRenderer>();

            Material newMat = new Material(Shader.Find("Unlit/Color"));
            newMat.color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 1.0f);
            line.material = newMat;
            line.SetPosition(0, new Vector3(node.Left.SpaceCenter.x, 0.0f, node.Left.SpaceCenter.y));
            line.SetPosition(1, new Vector3(node.Right.SpaceCenter.x, 0.0f, node.Right.SpaceCenter.y));
            line.startWidth = 1.5f;
            line.endWidth= 1.5f;
        }

        DrawCorridor(node.Left);
        DrawCorridor(node.Right);
    }
}
