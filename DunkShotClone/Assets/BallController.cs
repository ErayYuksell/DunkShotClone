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

    [SerializeField] private bool hitOnlyCircleCollider = true; // Deliksiz giriþ kontrolü

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Topun herhangi bir þeyle çarpýþtýðýný iþaretle
        hitOnlyCircleCollider = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
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
                UIManager.Instance.IncreaseScore(2); // Deliksiz giriþte ekstra puan ver
            }
            else
            {
                UIManager.Instance.IncreaseScore(1); // Normal puan ver
            }

            hoopController.hasScored = true;
            hoopController.CloseTheHoop();

            // Skor verildikten sonra bayraðý sýfýrla
            hitOnlyCircleCollider = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Hoop")
        {
            hoop = null;
            inTheAir = true;

            // Potadan çýkarken deliksiz giriþi sýfýrla
            hitOnlyCircleCollider = true;
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
