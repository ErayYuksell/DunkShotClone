using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform ball; // Topun Transform bile�eni
    private float initialY; // Kameran�n ba�lang��taki y ekseni pozisyonu

    void Start()
    {
        if (ball != null)
        {
            initialY = transform.position.y;
        }
    }

    void LateUpdate()
    {
        if (ball != null)
        {
            Vector3 newPosition = transform.position;
            newPosition.y = ball.position.y;
            transform.position = newPosition;
        }
    }
}
