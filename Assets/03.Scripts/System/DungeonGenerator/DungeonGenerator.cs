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
    public int mapWidth;
    public int mapSize;
    public int maxIterations;

    private void Start()
    {
        GenerateDungeon();
    }

    private void GenerateDungeon()
    {
        RectInt dungeonSize = new RectInt(0, 0, mapWidth, mapSize);

        //BSP를 이용한 공간 분할
        BinarySpacePartitioner bsp = new BinarySpacePartitioner(dungeonSize);
        RoomNode treeNode = bsp.CreateRoomTree(maxIterations);

        //만들어진 트리를 이용해 룸 생성
        CreateRooms(treeNode);

        //방 연결
    }

    private void CreateRooms(RoomNode treeNode)
    {
        List<RoomNode> leafNodes = new List<RoomNode>();

        TraverseAndGetLeafNodes(treeNode, leafNodes);

        GameObject room = Resources.Load<GameObject>("TestRoom");

        for (int i = 0; i < leafNodes.Count; i++)
        {
            leafNodes[i].InitRoomSizeBySpace();
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
}
