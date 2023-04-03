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

    private float _minDivideRatio;
    private float _maxDivideRatio;

    public BinarySpacePartitioner(RectInt size, float minDivideRatio, float maxDivideRatio)
    {
        _rootNode = new RoomNode(size);
        _minDivideRatio = minDivideRatio;
        _maxDivideRatio = maxDivideRatio;
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

        if (eSplitDirection.HORIZONTAL == splitDireciton)
        {
            //가로
            int dividedHeight = Mathf.RoundToInt(UnityEngine.Random.Range(node.SpaceHeight * _minDivideRatio, node.SpaceHeight * _maxDivideRatio));

            RectInt topRoomSize = new RectInt(
                node.BottomLeftAnchor.x, node.BottomLeftAnchor.y + dividedHeight,
                node.SpaceWidth, node.SpaceHeight - dividedHeight
            );

            RectInt bottomRoomSize = new RectInt(
                node.BottomLeftAnchor.x, node.BottomLeftAnchor.y,
                node.SpaceWidth, dividedHeight
            );

            //TODO: 게임 에셋으로 교체할 부분
            GameObject go = Resources.Load<GameObject>("TestLineRenderer");
            GameObject instance = GameObject.Instantiate(go);
            LineRenderer line = instance.GetComponent<LineRenderer>();
            line.SetPosition(0, new Vector3(node.BottomLeftAnchor.x, 0.1f, node.BottomLeftAnchor.y + dividedHeight));
            line.SetPosition(1, new Vector3(node.TopRightAnchor.x, 0.1f, node.BottomLeftAnchor.y + dividedHeight));

            RoomNode leftNode = new RoomNode(topRoomSize);
            RoomNode rightNode = new RoomNode(bottomRoomSize);

            node.Left = leftNode;
            node.Right = rightNode;
        }
        else
        {
            //세로
            int dividedWidth = Mathf.RoundToInt(UnityEngine.Random.Range(node.SpaceWidth * _minDivideRatio, node.SpaceWidth * _maxDivideRatio));

            RectInt leftRoomSize = new RectInt(
                node.BottomLeftAnchor.x, node.BottomLeftAnchor.y,
                dividedWidth, node.SpaceHeight
            );

            RectInt rightRoomSize = new RectInt(
                node.BottomLeftAnchor.x + dividedWidth, node.BottomLeftAnchor.y,
                node.SpaceWidth - dividedWidth, node.SpaceHeight
            );

            //TODO: 게임 에셋으로 교체할 부분
            GameObject go = Resources.Load<GameObject>("TestLineRenderer");
            GameObject instance = GameObject.Instantiate(go);
            LineRenderer line = instance.GetComponent<LineRenderer>();
            line.SetPosition(0, new Vector3(node.BottomLeftAnchor.x + dividedWidth, 0.1f, node.BottomLeftAnchor.y));
            line.SetPosition(1, new Vector3(node.BottomLeftAnchor.x + dividedWidth, 0.1f, node.TopRightAnchor.y));

            RoomNode leftNode = new RoomNode(leftRoomSize);
            RoomNode rightNode = new RoomNode(rightRoomSize);

            node.Left = leftNode;
            node.Right = rightNode;
        }

        DivideSpace(node.Left, iterations + 1);
        DivideSpace(node.Right, iterations + 1);
    }
}
