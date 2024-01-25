using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceDeck : MonoBehaviour
{
    public RectTransform rectTransform;
    public bool availableCarteSlots;

    void Start()
    {
        availableCarteSlots = true;
        rectTransform = GetComponent<RectTransform>();
    }

}
