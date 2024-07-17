using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    Rigidbody2D rb;

    Vector3 touchStartPos;
    Vector3 touchEndPos;
    bool isDragging = false;


    Transform hoop; // Potanýn alt kýsmý
    LineRenderer trajectoryLineRenderer; // Çizim için LineRenderer
    bool inTheAir = false; // havadayken hicbir sey yapamayacagiz

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
            }

            if (Input.GetMouseButton(0) && isDragging)
            {
                Vector3 currentTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                currentTouchPos.z = 0;

                Vector3 direction = touchStartPos - currentTouchPos; // Yönü doðru hesapla
                float distance = Mathf.Clamp(direction.magnitude, 0, 0.2f);


                // Potanýn yönünü deðiþtir
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                hoop.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90)); // 90 derece çýkarmamýzýn sebebi, baþlangýç rotasyonunun yukarýyý göstermesini saðlamak

                // Topun pozisyonunu ve rotasyonunu ayarla
                rb.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

                // Tahmini hareketi çiz
                Vector3 force = direction * 10; // Gücü ayarla
                DrawTrajectory(rb, force);
            }

            if (Input.GetMouseButtonUp(0) && isDragging)
            {
                isDragging = false;

                // Potanýn alt kýsmýný sýfýrla
                hoop.rotation = Quaternion.identity;

                // Topa kuvvet uygula
                touchEndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                touchEndPos.z = 0;
                Vector3 force = (touchStartPos - touchEndPos) * 10;

                rb.isKinematic = false;
                rb.AddForce(force, ForceMode2D.Impulse); // Çarpaný ayarlayabilirsiniz

                // Çizgiyi gizle
                trajectoryLineRenderer.positionCount = 0;
            }
        }
    }

    void DrawTrajectory(Rigidbody2D rb, Vector3 force)
    {
        Vector3[] points = new Vector3[30]; // 30 noktayý hesaplayalým
        Vector2 startingPosition = rb.position;
        Vector2 startingVelocity = force / rb.mass * Time.fixedDeltaTime;

        for (int i = 0; i < points.Length; i++)
        {
            float t = i / (float)points.Length; // Zaman adýmý
            points[i] = startingPosition + startingVelocity * t + 0.5f * Physics2D.gravity * t * t;
            points[i].z = 0;
        }

        trajectoryLineRenderer.positionCount = points.Length;
        trajectoryLineRenderer.SetPositions(points);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Hoop")
        {
            inTheAir = false;
            hoop = collision.gameObject.transform.Find("Hoop");
            HoopController hoopController = collision.gameObject.GetComponent<HoopController>();
            trajectoryLineRenderer = hoopController.GetComponent<LineRenderer>();
            trajectoryLineRenderer.positionCount = 0; // Baþlangýçta çizgiyi gizle
            trajectoryLineRenderer.useWorldSpace = true; // Dünya koordinatlarýný kullan
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Hoop")
        {
            hoop = null;
            inTheAir = true;
        }
    }
}
