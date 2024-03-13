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
    [SerializeField] private Vector2Int initialOffset = Vector2Int.zero;
    public Vector2Int InitialOffset { get { return new Vector2Int(initialOffset.y, initialOffset.x); }}

    private Vector2 offset = new Vector2(0,0);
    public Vector2 Offset { get {return offset;} set { offset = value; }}
}

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/LevelInfo", order = 1)]
public class LevelInfo : ScriptableObject
{
    [Header("Start & End")]
    [SerializeField] private Vector2Int mouseStartPos;
    public Vector2Int MouseStartPos { get { return new Vector2Int(mouseStartPos.y, mouseStartPos.x); }}
    [SerializeField] private Vector2Int cheesePos;
    public Vector2Int CheesePos { get { return new Vector2Int(cheesePos.y, cheesePos.x); }}

    [Header("Labyrinth Layout")]
    [SerializeField] private int levelWidth;
    [SerializeField] private int levelHeight;
    public Vector2Int LevelSize { get { return new Vector2Int(levelHeight, levelWidth); }}
    [SerializeField] public LabyrinthInfo[] labyrinths;


    [Header("Other")]
    [SerializeField] public Texture hint = null;
    [SerializeField] public float cameraSize = 4;
    [SerializeField] public int maxOffset;
    [SerializeField] public RGB[] mouseColors;
}