using System;
using System.Collections.Generic;
// using System.Diagnostics;
using UnityEngine;


class DFSGrid {
    private bool[,] grid;
    private int rows;
    private int cols;
    private LevelInfo level;
    private GameObject mouseGameObject;

    public DFSGrid(LevelInfo levelInfo, GameObject mouse) {
        level = levelInfo;
        rows = levelInfo.levelSize.x;
        cols = levelInfo.levelSize.y;
        mouseGameObject = mouse;
    }

    public List<Vector2Int> FindShortestPath() {
        Vector2Int startNode = level.mouseStartPos;
        Vector2Int targetNode = level.cheesePos;
        bool[,] visited = new bool[rows, cols];
        
        Dictionary<Vector2Int, Vector2Int> parentMap = new();
        Stack<Vector2Int> queue = new();
        List<Vector2Int> path = new();

        queue.Push(startNode);
        visited[startNode.x, startNode.y] = true;

        while (queue.Count != 0) {
            var currentNode = queue.Pop();

            // check if we've arrived
            if (currentNode == targetNode) {
                path = ConstructPath(parentMap, startNode, targetNode);
                break;
            }

            //add new neighbors to Stack
            foreach (var neighbor in GetNeighbors(currentNode)) {
                int row = neighbor.x;
                int col = neighbor.y;

                if (IsValidCell(row, col) && grid[row, col] && !visited[row, col]) {
                    queue.Push(neighbor);
                    visited[row, col] = true;
                    parentMap[neighbor] = currentNode;
                }
            }
        }

        Debug.Log(path.ToString());
        return path;
    }

    private List<Vector2Int> ConstructPath(Dictionary<Vector2Int, Vector2Int> parentMap, Vector2Int startNode, Vector2Int targetNode) {
        List<Vector2Int> path = new();
        var currentNode = targetNode;

        while (currentNode != startNode) {
            path.Insert(0, currentNode);
            currentNode = parentMap[currentNode];
        }

        path.Insert(0, startNode);

        return path;
    }

    private List<Vector2Int> GetNeighbors(Vector2Int node) {
        int row = node.x;
        int col = node.y;

        List<Vector2Int> neighbors = new()
        {
            // Add all four neighbors: top, bottom, left, right
            new Vector2Int(row - 1, col),
            new Vector2Int(row + 1, col),
            new Vector2Int(row, col - 1),
            new Vector2Int(row, col + 1)
        };

        return neighbors;
    }

    // TODO: Big logic for checking if the mouse can walk here
    private bool IsValidCell(int row, int col) {
        return row >= 0 && row < rows && col >= 0 && col < cols;
    }
}
