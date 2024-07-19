using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Sahne yönetimi için ekleyin

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform ball; // Topun Transform bileþeni
    private float initialY; // Kameranýn baþlangýçtaki y ekseni pozisyonu
    [SerializeField] float loseThreshold = -5f; // Kaybetme durumu için y ekseni eþiði
    [SerializeField] float yDistanceToLose = 10f; // Topun yukarýdan aþaðýya kaybetmek için ne kadar düþmesi gerektiði

    private bool isFalling = false; // Topun düþüp düþmediðini kontrol eder
    private float highestPoint; // Topun ulaþtýðý en yüksek nokta

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

            // Topun ulaþtýðý en yüksek noktayý güncelle
            if (ball.position.y > highestPoint)
            {
                highestPoint = ball.position.y;
            }

            // Topun düþmeye baþladýðýný kontrol et
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
            // Oyunu kaybettiðinde yapýlacak iþlemler
            Debug.Log("Oyun kaybedildi!");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Mevcut sahneyi yeniden yükleyerek oyunu baþtan baþlatýr
        }
    }
}
