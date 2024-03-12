using UnityEngine;
using System;
using UnityEngine.UI;

[Serializable]
public class LabyrinthInfo
{
    public TextAsset labyrinthTextFile;
    public RGB rgbColor;
    private bool[,] levelMatrix;
    public bool[,] LevelMatrix { get {return levelMatrix;} set { levelMatrix = value; }}

    private Vector2 offset = new Vector2(0,0);
    public Vector2 Offset { get {return offset;} set { offset = value; }}
}

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/LevelInfo", order = 1)]
public class LevelInfo : ScriptableObject
{
    [SerializeField] public Vector2Int cheesePos;
    [SerializeField] public RawImage hint;

    [Header("Labyrinth Layout")]
    [SerializeField] public Vector2Int levelSize;
    [SerializeField] public int maxOffset;
    [SerializeField] public LabyrinthInfo[] labyrinths;


    [Header("Mouse Info")]
    [SerializeField] public Vector2Int mouseStartPos;
    [SerializeField] public RGB[] mouseColors;
}