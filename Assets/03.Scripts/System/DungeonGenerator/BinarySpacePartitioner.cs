using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public enum eSplitDirection
{
    NONE= 0,
    HORIZONTAL = 1,
    VERTICAL = 2,
}

public class BinarySpacePartitioner
{
    private RoomNode _rootNode;
    private int _maxIterations;

    private float minDivideRatio = 0.45f;
    private float maxDivideRatio = 0.65f;

    public BinarySpacePartitioner(RectInt size)
    {
        _rootNode = new RoomNode(size);
    }

    public RoomNode CreateRoomTree(int maxIterations)
    {
        _maxIterations = maxIterations;
        DivideSpace(_rootNode, 0);

        return _rootNode;
    }

    private void DivideSpace(RoomNode node, int iterations = 0)
    {
        if (iterations == _maxIterations)
            return;

        eSplitDirection splitDireciton = (node.SpaceWidth > node.SpaceHeight) ? eSplitDirection.VERTICAL : eSplitDirection.HORIZONTAL;
        node.SplittedDirection = splitDireciton;


        if (eSplitDirection.HORIZONTAL == splitDireciton)
        {
            //가로
            int dividedHeight = Mathf.RoundToInt(UnityEngine.Random.Range(node.SpaceHeight * minDivideRatio, node.SpaceHeight * maxDivideRatio));

            RectInt topRoomSize = new RectInt(
                node.BottomLeftPosition.x, node.BottomLeftPosition.y + dividedHeight,
                node.SpaceWidth, node.SpaceHeight - dividedHeight
            );

            RectInt bottomRoomSize = new RectInt(
                node.BottomLeftPosition.x, node.BottomLeftPosition.y,
                node.SpaceWidth, dividedHeight
            );

            GameObject go = Resources.Load<GameObject>("TestLineRenderer");
            GameObject instance = GameObject.Instantiate(go);
            LineRenderer line = instance.GetComponent<LineRenderer>();
            line.SetPosition(0, new Vector3(node.BottomLeftPosition.x, 0.1f, node.BottomLeftPosition.y + dividedHeight));
            line.SetPosition(1, new Vector3(node.TopRightPosition.x, 0.1f, node.BottomLeftPosition.y + dividedHeight));

            RoomNode leftNode = new RoomNode(topRoomSize);
            RoomNode rightNode = new RoomNode(bottomRoomSize);

            node.Left = leftNode;
            node.Right = rightNode;
        }
        else
        {
            //세로
            int dividedWidth = Mathf.RoundToInt(UnityEngine.Random.Range(node.SpaceWidth * minDivideRatio, node.SpaceWidth * maxDivideRatio));

            RectInt leftRoomSize = new RectInt(
                node.BottomLeftPosition.x, node.BottomLeftPosition.y,
                dividedWidth, node.SpaceHeight
            );

            RectInt rightRoomSize = new RectInt(
                node.BottomLeftPosition.x + dividedWidth, node.BottomLeftPosition.y,
                node.SpaceWidth - dividedWidth, node.SpaceHeight
            );

            GameObject go = Resources.Load<GameObject>("TestLineRenderer");
            GameObject instance = GameObject.Instantiate(go);
            LineRenderer line = instance.GetComponent<LineRenderer>();
            line.SetPosition(0, new Vector3(node.BottomLeftPosition.x + dividedWidth, 0.1f, node.BottomLeftPosition.y));
            line.SetPosition(1, new Vector3(node.BottomLeftPosition.x + dividedWidth, 0.1f, node.TopRightPosition.y));

            RoomNode leftNode = new RoomNode(leftRoomSize);
            RoomNode rightNode = new RoomNode(rightRoomSize);

            node.Left = leftNode;
            node.Right = rightNode;
        }

        DivideSpace(node.Left, iterations + 1);
        DivideSpace(node.Right, iterations + 1);
    }
}
