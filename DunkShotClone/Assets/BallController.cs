using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BallController : MonoBehaviour
{
    Rigidbody2D rb;
    public GameObject redDotPrefab; // K�rm�z� nokta prefab'i

    Vector3 touchStartPos;
    Vector3 touchEndPos;
    bool isDragging = false;
    bool inTheAir = false; // havadayken hi�bir �ey yapamayaca��z

    Transform hoop; // Pota
    List<GameObject> trajectoryDots = new List<GameObject>(); // �izim i�in kullan�lan noktalar

    HoopController hoopController;

    private bool hitOnlyCircleCollider = false; // Deliksiz giri� kontrol�
    bool firstColliderHit = false;

    ParticleSystem fireEffect;

    int strikeCount = 0;

   

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        fireEffect = GetComponentInChildren<ParticleSystem>();
    }

    void Update()
    {
        if (!inTheAir)
        {
            if (Input.GetMouseButtonDown(0))
            {
                touchStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                touchStartPos.z = 0;
                isDragging = true;
                rb.velocity = Vector2.zero; // Topun hareketini durdur
                ClearTrajectoryDots();
            }

            if (Input.GetMouseButton(0) && isDragging)
            {
                Vector3 currentTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                currentTouchPos.z = 0;

                Vector3 direction = touchStartPos - currentTouchPos; // Y�n� do�ru hesapla
                float distance = Mathf.Clamp(direction.magnitude, 0, 2); // Maksimum s�r�kleme miktar�n� ayarla

                // Potan�n y�n�n� de�i�tir
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                hoop.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90)); // 90 derece ��karmam�z�n sebebi, ba�lang�� rotasyonunun yukar�y� g�stermesini sa�lamak

                // Topun pozisyonunu ve rotasyonunu ayarla
                rb.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

                // �izgi �izimini g�ncelle
                DrawTrajectory(rb.position, direction.normalized * distance * 5); // Distance ile �arparak �izginin uzunlu�unu ayarla
            }

            if (Input.GetMouseButtonUp(0) && isDragging)
            {
                isDragging = false;

                // Potan�n alt k�sm�n� s�f�rla
                hoop.rotation = Quaternion.identity;

                // Topa kuvvet uygula
                touchEndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                touchEndPos.z = 0;
                Vector3 force = (touchStartPos - touchEndPos) * 5;

                rb.isKinematic = false;
                rb.AddForce(force, ForceMode2D.Impulse);

                // �izgiyi gizle
                ClearTrajectoryDots();
            }
        }
        else
        {
            BallRotate();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!firstColliderHit) // �lk �arpma kontrol�
        {
            if (collision is CircleCollider2D && collision.tag == "Hoop")
            {
                // �lk �arpma CircleCollider2D ile
                hitOnlyCircleCollider = true;
                firstColliderHit = true; // �lk �arpma ger�ekle�ti if in sonuna koysam yildiz ile carpistiginda sikinti cikar 
            }
            else if (collision is EdgeCollider2D && collision.tag == "Hoop")
            {
                // �lk �arpma EdgeCollider2D ile
                hitOnlyCircleCollider = false;
                firstColliderHit = true; // �lk �arpma ger�ekle�ti
            }
        }

        if (collision.tag == "Hoop")
        {
            inTheAir = false;

            hoop = collision.gameObject.transform.Find("Hoop");
            hoopController = collision.gameObject.GetComponent<HoopController>();

            ClearTrajectoryDots(); // �izim noktalar�n� temizle
        }

        // E�er �arp�lan collider bir CircleCollider2D ise
        CircleCollider2D circleCollider = collision as CircleCollider2D;
        if (circleCollider != null && collision.tag == "Hoop" && !hoopController.hasScored)
        {
            // Deliksiz giri�i kontrol et
            if (hitOnlyCircleCollider)
            {
                fireEffect.Play();
                strikeCount++;
                hoopController.hasScored = true;
                hoopController.OpenStrikeText(strikeCount);
                UIManager.Instance.IncreaseScore(strikeCount); // Deliksiz giri�te ekstra puan ver
            }
            else
            {
                fireEffect.Stop();
                UIManager.Instance.IncreaseScore(1); // Normal puan ver
                hoopController.hasScored = true;
                hoopController.OpenStrikeText(1);
                strikeCount = 0;
            }

            hoopController.CloseTheHoop();

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Hoop" && hoopController.hasScored)
        {
            hoop = null;
            inTheAir = true;

            // Potadan ��karken deliksiz giri�i s�f�rla
            hitOnlyCircleCollider = false;
            firstColliderHit = false; // �lk �arpma kontrol�n� s�f�rla
        }
    }


    public void BallRotate()
    {
        // Topu s�rekli d�nd�r
        transform.Rotate(new Vector3(0, 0, 360) * Time.deltaTime);
    }

    // �izim noktalar�n� temizleme fonksiyonu
    private void ClearTrajectoryDots()
    {
        foreach (var dot in trajectoryDots)
        {
            Destroy(dot);
        }
        trajectoryDots.Clear();
    }

    // Trajectory �izme fonksiyonu
    private void DrawTrajectory(Vector2 startPosition, Vector2 velocity)
    {
        ClearTrajectoryDots(); // Mevcut noktalar� temizle

        Vector2 gravity = Physics2D.gravity;
        float timeStep = 0.1f;
        int steps = 15; // Ad�m say�s�n� ayarla

        for (int i = 0; i < steps; i++)
        {
            float t = i * timeStep;
            Vector3 position = startPosition + velocity * t + 0.5f * gravity * t * t;
            GameObject dot = Instantiate(redDotPrefab, position, Quaternion.identity);
            trajectoryDots.Add(dot);
        }
    }
}
