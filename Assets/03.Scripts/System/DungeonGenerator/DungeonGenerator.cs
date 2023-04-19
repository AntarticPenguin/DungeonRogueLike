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
    public GameObject _testRoomPrefab;
    [SerializeField] GameObject _tilePrefab;
    [SerializeField] GameObject _wallPrefab;
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

        //만들어진 트리를 이용해 룸 생성
        CreateRooms(treeNode);

        //방 연결
        DrawCorridor(treeNode);
    }

    private void CreateRooms(RoomNode treeNode)
    {
        List<RoomNode> leafNodes = new List<RoomNode>();

        TraverseAndGetLeafNodes(treeNode, leafNodes);

        for (int i = 0; i < leafNodes.Count; i++)
        {
            leafNodes[i].InitRoomSizeBySpace(_minRoomPadding, _maxRoomPadding);

            //Test Room Render
            //{
            //    GameObject instance = Instantiate(_testRoomPrefab);
            //    instance.transform.position = new Vector3(leafNodes[i].RoomSize.xMin, 0, leafNodes[i].RoomSize.yMin);
            //    instance.transform.localScale = new Vector3(leafNodes[i].RoomSize.width, leafNodes[i].RoomSize.height, 1.0f);
            //}

            RectInt roomSize = leafNodes[i].RoomSize;
            int tileSize = 2;

            for (int y = roomSize.yMin; y < roomSize.yMax; y += tileSize)
            {
                for (int x = roomSize.xMin; x < roomSize.xMax; x += tileSize)
                {
                    GameObject instance = Instantiate(_tilePrefab);
                    instance.transform.position = new Vector3(x, 0, y);
                    instance.transform.localScale = Vector3.one;
                }
            }

            CreateWalls(roomSize);
        }
    }

    private void CreateWalls(RectInt room)
    {
        int wallWidth = 1;
        int roomWidth = (room.width % 2 == 0) ? room.width - 1 : room.width;
        int roomHeight = (room.height % 2 == 0) ? room.height - 1 : room.height;

        //Max좌표 보정
        room.xMax = room.xMin + roomWidth;
        room.yMax = room.yMin + roomHeight;

        Vector2 offset = new Vector2(1, 1);

        Debug.Log($"Width:{roomWidth}, Height: {roomHeight}");

        //Bottom edge
        for (int i = 1; i < roomWidth; i++)
        {
            GameObject instance = Instantiate(_wallPrefab);
            instance.transform.position = new Vector3(room.xMin - offset.x + i * wallWidth, 0, room.yMin - offset.y);
            instance.transform.localScale = Vector3.one;
        }

        //Right side edge
        for (int i = 1; i < roomHeight; i++)
        {
            GameObject instance = Instantiate(_wallPrefab);
            instance.transform.position = new Vector3(room.xMax, 0, room.yMin - offset.y + i * wallWidth);
            instance.transform.eulerAngles = new Vector3(0, -90, 0);
            instance.transform.localScale = Vector3.one;
        }

        //Top edge
        for (int i = 1; i < roomWidth; i++)
        {
            GameObject instance = Instantiate(_wallPrefab);
            instance.transform.position = new Vector3(room.xMin + i * wallWidth, 0, room.yMax);
            instance.transform.eulerAngles = new Vector3(0, -180, 0);
            instance.transform.localScale = Vector3.one;
        }

        //Left side edge
        for (int i = 1; i < roomHeight; i++)
        {
            GameObject instance = Instantiate(_wallPrefab);
            instance.transform.eulerAngles = new Vector3(0, 90, 0);
            instance.transform.position = new Vector3(room.xMin - offset.x, 0, room.yMin + i * wallWidth);
            instance.transform.localScale = Vector3.one;
        }
    }

    private void TraverseAndGetLeafNodes(RoomNode node, List<RoomNode> outList)
    {
        if (null == node)
            return;

        TraverseAndGetLeafNodes(node.Left, outList);

        if (node.IsLeaf)
        {
            outList.Add(node);
        }
            
        TraverseAndGetLeafNodes(node.Right, outList);
    }

    private void DrawCorridor(RoomNode node)
    {
        if(node.IsLeaf)
        {
            return;
        }

        //TODO: 룸을 가로지르는 복도는 어떻게 지울건가

        //TODO: 게임 에셋으로 교체할 부분
        GameObject go = Resources.Load<GameObject>("TestLineRenderer");
        GameObject instance = Instantiate(go);
        LineRenderer line = instance.GetComponent<LineRenderer>();
        line.SetPosition(0, new Vector3(node.Left.SpaceCenter.x, 0.0f, node.Left.SpaceCenter.y));
        line.SetPosition(1, new Vector3(node.Right.SpaceCenter.x, 0.0f, node.Right.SpaceCenter.y));
        line.startWidth = 1.5f;
        line.endWidth= 1.5f;

        DrawCorridor(node.Left);
        DrawCorridor(node.Right);
    }
}
