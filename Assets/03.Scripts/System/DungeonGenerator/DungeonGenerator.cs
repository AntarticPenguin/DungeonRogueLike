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
        RoomNode rootNode = bsp.MakeRoomTree(maxIterations);

        //만들어진 트리를 이용해 룸 생성

        //방 연결
    }
}
