using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private int selectedButtonIndex;
    [SerializeField] GameManager gameManager;
    [SerializeField] Button[] buttons;

    public void OnButtonPress(int buttonIndex) {
        selectedButtonIndex = buttonIndex;
        foreach (Button button in buttons)
        {
            if (button.ButtonIndex != buttonIndex) {
                button.Deselect();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        buttons[0].Select();
        selectedButtonIndex = buttons[0].ButtonIndex;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = Vector2.zero;
        direction.x = Math.Sign(Input.GetAxis("Horizontal"));
        if (direction.x != 0) {
            gameManager.MoveLabyrinth(selectedButtonIndex, direction);
            return;
        }
        direction.y = Math.Sign(Input.GetAxis("Vertical"));
        if (direction.y != 0) {
            gameManager.MoveLabyrinth(selectedButtonIndex, direction);
        }
    }
}
