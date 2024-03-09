using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/LevelInfo", order = 1)]
public class LevelInfo : ScriptableObject
{
    [SerializeField] public Vector2Int levelSize;
    [SerializeField] public TextAsset redLabyrinthTextFile;
    [SerializeField] public TextAsset greenLabyrinthTextFile;
    [SerializeField] public TextAsset blueLabyrinthTextFile;
    [SerializeField] public Vector2Int mouseStartPos;
    [SerializeField] public Vector2Int cheesePos;
    [SerializeField] public int maxOffset;
}