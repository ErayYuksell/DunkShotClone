using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Sahne y�netimi i�in ekleyin

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform ball; // Topun Transform bile�eni
    private float initialY; // Kameran�n ba�lang��taki y ekseni pozisyonu
    [SerializeField] float loseThreshold = -5f; // Kaybetme durumu i�in y ekseni e�i�i
    [SerializeField] float yDistanceToLose = 10f; // Topun yukar�dan a�a��ya kaybetmek i�in ne kadar d��mesi gerekti�i

    private bool isFalling = false; // Topun d���p d��medi�ini kontrol eder
    private float highestPoint; // Topun ula�t��� en y�ksek nokta

    void Start()
    {
        if (ball != null)
        {
            initialY = transform.position.y;
            highestPoint = ball.position.y;
        }
    }

    void LateUpdate()
    {
        if (ball != null)
        {
            Vector3 newPosition = transform.position;
            newPosition.y = ball.position.y;
            transform.position = newPosition;

            // Topun ula�t��� en y�ksek noktay� g�ncelle
            if (ball.position.y > highestPoint)
            {
                highestPoint = ball.position.y;
            }

            // Topun d��meye ba�lad���n� kontrol et
            if (ball.position.y < highestPoint - yDistanceToLose)
            {
                isFalling = true;
            }

            // Kaybetme durumunu kontrol et
            CheckLoseCondition();
        }
    }

    void CheckLoseCondition()
    {
        if (isFalling && ball.position.y < loseThreshold)
        {
            // Oyunu kaybetti�inde yap�lacak i�lemler
            Debug.Log("Oyun kaybedildi!");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Mevcut sahneyi yeniden y�kleyerek oyunu ba�tan ba�lat�r
        }
    }
}
