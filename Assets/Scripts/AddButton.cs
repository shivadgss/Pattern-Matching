using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddButton : MonoBehaviour
{
    [SerializeField] private Transform puzzleField;
    [SerializeField] private GameObject btn;
    [SerializeField] private int numberOfRowsToAdd = 2;

    private void Awake()
    {
        int totalButtons = 4 * (numberOfRowsToAdd + 1); // Calculate the total number of buttons (including the new row)

        for (int i = 0; i < totalButtons; i++)
        {
            GameObject button = Instantiate(btn);
            button.name = "" + i;
            button.transform.SetParent(puzzleField, false);
        }
    }

}
