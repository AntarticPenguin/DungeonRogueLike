using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 던전 룸 생성 클래스
/// 방은 만들고, 처음 공간을 분할하고, 복도를 연결해주는 클래스
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

        GameObject room = Resources.Load<GameObject>("TestRoom");

        for (int i = 0; i < leafNodes.Count; i++)
        {
            leafNodes[i].InitRoomSizeBySpace(_minRoomPadding, _maxRoomPadding);
            GameObject instance = Instantiate(room);
            instance.transform.position = new Vector3(leafNodes[i].RoomPosition.x, 0, leafNodes[i].RoomPosition.y);
            instance.transform.localScale = new Vector3(leafNodes[i].RoomScale.x, leafNodes[i].RoomScale.y);
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

        //TODO: 룸을 가로지르는 복도는 어떻게 지울건가?

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
