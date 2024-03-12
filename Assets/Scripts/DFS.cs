using System;
using System.Collections.Generic;
// using System.Diagnostics;
using UnityEngine;

class DFSGrid {
    private readonly int rows;
    private readonly int cols;
    private readonly LevelInfo level;
    private bool[,] visited;
    
    private bool solved = false;
    public bool Solved { get { return solved; }}
    private List<Vector2Int> path;
    public List<Vector2Int> Path { get { return path; }}

    public DFSGrid(LevelInfo levelInfo) {
        level = levelInfo;
        rows = levelInfo.levelSize.x;
        cols = levelInfo.levelSize.y;
        path = new List<Vector2Int>();
        visited = new bool[rows+2*level.maxOffset, cols+2*level.maxOffset];

        Vector2Int startNode = level.mouseStartPos;
        Vector2Int targetNode = level.cheesePos;

        if (IsValidCell(startNode.x, startNode.y)) {
            DFS_Recursive(startNode, targetNode);
        }
    }

    // returns true if we've reached the targetNode
    private void DFS_Recursive(Vector2Int currentNode, Vector2Int targetNode) {
        if (CheckVisited(currentNode, visited)) { return; }
        SetVisited(currentNode, visited);
        if (currentNode == targetNode) { 
            solved = true;
            return; 
        }
        foreach (Vector2Int neighbor in GetNeighbors(currentNode)) {
            if (CheckVisited(neighbor, visited)) { continue;}
            bool isValid = IsValidCell(neighbor.x, neighbor.y);
            if (!isValid) { continue; }
            //move mouse to neighbor tile
            path.Add(neighbor);

            DFS_Recursive(neighbor, targetNode);
            if (solved)  {
                return;
            } else {
                //return mouse to current tile
                path.Add(currentNode);
            }
        }
        return;
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
        //check out of bounds
        int maxOffset = level.maxOffset;
        if (OutOfBounds(row, -maxOffset, rows+maxOffset)) return false;
        if (OutOfBounds(col, -maxOffset, cols+maxOffset)) return false;

        // get color at (row, col)
        RGB tileColor = RGB.BLACK;
        foreach (LabyrinthInfo lab in level.labyrinths) {
            //what tile in the labyrinth should we check, considering its offset
            Vector2Int tileToCheck = new Vector2Int(row - (int)lab.Offset.x, col - (int)lab.Offset.y);
            //if the tile is out of bounds, there is no color there
            if (OutOfBounds(tileToCheck.x, 0, rows)) { continue;}
            if (OutOfBounds(tileToCheck.y, 0, cols)) { continue;}
            //check if the matrix has a tile there
            bool[,] matrix = lab.LevelMatrix;
            if (matrix[tileToCheck.x, tileToCheck.y]) {
                //add color to tileColor
                tileColor = ColorUtil.AddColors(tileColor, lab.rgbColor);
            }
        }
        //check if mouse can walk there
        foreach (RGB possibleColor in level.mouseColors)
        {
            if (tileColor == possibleColor) return true;
        }
        return false;

        static bool OutOfBounds(int i, int lowerbound, int upperbound) {
            if (i < lowerbound) return true; 
            if (i >= upperbound) return true;
            return false;
        }
    }

    private bool CheckVisited(Vector2Int node, bool[,] visited) {
        //because the tiles we can visit is bigger than the size of the labyrinths, 
        //the visited matrix is padded by the maxOffset 
        return visited[level.maxOffset+node.x, level.maxOffset+node.y];
    }
    private void SetVisited(Vector2Int node, bool[,] visited) {
        visited[level.maxOffset+node.x, level.maxOffset+node.y] = true;
    }
}
