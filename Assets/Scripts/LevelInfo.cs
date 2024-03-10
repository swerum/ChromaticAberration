using UnityEngine;
using System;

[Serializable]
public class LabyrinthInfo
{
    public TextAsset labyrinthTextFile;
    public Color color = Color.white;
    public RGB rgbColor;
}

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/LevelInfo", order = 1)]
public class LevelInfo : ScriptableObject
{
    [SerializeField] public Vector2Int levelSize;
    [SerializeField] public LabyrinthInfo[] labyrinths;
    [SerializeField] public Vector2Int mouseStartPos;
    [SerializeField] public Vector2Int cheesePos;
    [SerializeField] public int maxOffset;
}