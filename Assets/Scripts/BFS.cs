using System;
using System.Collections.Generic;
// using System.Diagnostics;
using UnityEngine;


class BFSGrid {
    private bool[,] grid;
    private int rows;
    private int cols;

    public BFSGrid(bool[,] matrix) {
        grid = matrix;
        rows = grid.GetLength(0);
        cols = grid.GetLength(1);
    }

    public List<(int, int)> FindShortestPath((int, int) startNode, (int, int) targetNode) {
        bool[,] visited = new bool[rows, cols];
        Dictionary<(int, int), (int, int)> parentMap = new Dictionary<(int, int), (int, int)>();
        Queue<(int, int)> queue = new Queue<(int, int)>();
        List<(int, int)> path = new List<(int, int)>();

        queue.Enqueue(startNode);
        visited[startNode.Item1, startNode.Item2] = true;

        while (queue.Count != 0) {
            var currentNode = queue.Dequeue();

            if (currentNode == targetNode) {
                path = ConstructPath(parentMap, startNode, targetNode);
                break;
            }

            foreach (var neighbor in GetNeighbors(currentNode)) {
                int row = neighbor.Item1;
                int col = neighbor.Item2;

                if (IsValidCell(row, col) && grid[row, col] && !visited[row, col]) {
                    queue.Enqueue(neighbor);
                    visited[row, col] = true;
                    parentMap[neighbor] = currentNode;
                }
            }
        }

        return path;
    }

    private List<(int, int)> ConstructPath(Dictionary<(int, int), (int, int)> parentMap, (int, int) startNode, (int, int) targetNode) {
        List<(int, int)> path = new List<(int, int)>();
        var currentNode = targetNode;

        while (currentNode != startNode) {
            path.Insert(0, currentNode);
            currentNode = parentMap[currentNode];
        }

        path.Insert(0, startNode);

        return path;
    }

    private List<(int, int)> GetNeighbors((int, int) node) {
        int row = node.Item1;
        int col = node.Item2;

        List<(int, int)> neighbors = new List<(int, int)>();

        // Add all four neighbors: top, bottom, left, right
        neighbors.Add((row - 1, col));
        neighbors.Add((row + 1, col));
        neighbors.Add((row, col - 1));
        neighbors.Add((row, col + 1));

        return neighbors;
    }

    private bool IsValidCell(int row, int col) {
        return row >= 0 && row < rows && col >= 0 && col < cols;
    }
}
