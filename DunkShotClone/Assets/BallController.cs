using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BallController : MonoBehaviour
{
    Rigidbody2D rb;
    public GameObject redDotPrefab; // Kýrmýzý nokta prefab'i

    Vector3 touchStartPos;
    Vector3 touchEndPos;
    bool isDragging = false;
    bool inTheAir = false; // havadayken hiçbir þey yapamayacaðýz

    Transform hoop; // Pota
    List<GameObject> trajectoryDots = new List<GameObject>(); // Çizim için kullanýlan noktalar

    HoopController hoopController;

    private bool hitOnlyCircleCollider = false; // Deliksiz giriþ kontrolü
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

                Vector3 direction = touchStartPos - currentTouchPos; // Yönü doðru hesapla
                float distance = Mathf.Clamp(direction.magnitude, 0, 2); // Maksimum sürükleme miktarýný ayarla

                // Potanýn yönünü deðiþtir
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                hoop.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90)); // 90 derece çýkarmamýzýn sebebi, baþlangýç rotasyonunun yukarýyý göstermesini saðlamak

                // Topun pozisyonunu ve rotasyonunu ayarla
                rb.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

                // Çizgi çizimini güncelle
                DrawTrajectory(rb.position, direction.normalized * distance * 5); // Distance ile çarparak çizginin uzunluðunu ayarla
            }

            if (Input.GetMouseButtonUp(0) && isDragging)
            {
                isDragging = false;

                // Potanýn alt kýsmýný sýfýrla
                hoop.rotation = Quaternion.identity;

                // Topa kuvvet uygula
                touchEndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                touchEndPos.z = 0;
                Vector3 force = (touchStartPos - touchEndPos) * 5;

                rb.isKinematic = false;
                rb.AddForce(force, ForceMode2D.Impulse);

                // Çizgiyi gizle
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
        if (!firstColliderHit) // Ýlk çarpma kontrolü
        {
            if (collision is CircleCollider2D && collision.tag == "Hoop")
            {
                // Ýlk çarpma CircleCollider2D ile
                hitOnlyCircleCollider = true;
                firstColliderHit = true; // Ýlk çarpma gerçekleþti if in sonuna koysam yildiz ile carpistiginda sikinti cikar 
            }
            else if (collision is EdgeCollider2D && collision.tag == "Hoop")
            {
                // Ýlk çarpma EdgeCollider2D ile
                hitOnlyCircleCollider = false;
                firstColliderHit = true; // Ýlk çarpma gerçekleþti
            }
        }

        if (collision.tag == "Hoop")
        {
            inTheAir = false;

            hoop = collision.gameObject.transform.Find("Hoop");
            hoopController = collision.gameObject.GetComponent<HoopController>();

            ClearTrajectoryDots(); // Çizim noktalarýný temizle
        }

        // Eðer çarpýlan collider bir CircleCollider2D ise
        CircleCollider2D circleCollider = collision as CircleCollider2D;
        if (circleCollider != null && collision.tag == "Hoop" && !hoopController.hasScored)
        {
            // Deliksiz giriþi kontrol et
            if (hitOnlyCircleCollider)
            {
                fireEffect.Play();
                strikeCount++;
                hoopController.hasScored = true;
                hoopController.OpenStrikeText(strikeCount);
                UIManager.Instance.IncreaseScore(strikeCount); // Deliksiz giriþte ekstra puan ver
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

            // Potadan çýkarken deliksiz giriþi sýfýrla
            hitOnlyCircleCollider = false;
            firstColliderHit = false; // Ýlk çarpma kontrolünü sýfýrla
        }
    }


    public void BallRotate()
    {
        // Topu sürekli döndür
        transform.Rotate(new Vector3(0, 0, 360) * Time.deltaTime);
    }

    // Çizim noktalarýný temizleme fonksiyonu
    private void ClearTrajectoryDots()
    {
        foreach (var dot in trajectoryDots)
        {
            Destroy(dot);
        }
        trajectoryDots.Clear();
    }

    // Trajectory çizme fonksiyonu
    private void DrawTrajectory(Vector2 startPosition, Vector2 velocity)
    {
        ClearTrajectoryDots(); // Mevcut noktalarý temizle

        Vector2 gravity = Physics2D.gravity;
        float timeStep = 0.1f;
        int steps = 15; // Adým sayýsýný ayarla

        for (int i = 0; i < steps; i++)
        {
            float t = i * timeStep;
            Vector3 position = startPosition + velocity * t + 0.5f * gravity * t * t;
            GameObject dot = Instantiate(redDotPrefab, position, Quaternion.identity);
            trajectoryDots.Add(dot);
        }
    }
}
