using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateIndіcator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _indicator;

    public void SetColor(Color color)
    {
        if(_indicator.color == color) return; 
        _indicator.color = color;
    }
}
