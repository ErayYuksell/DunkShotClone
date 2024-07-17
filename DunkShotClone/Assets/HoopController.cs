using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoopController : MonoBehaviour
{
    public Transform hoopBottom; // Potanýn alt kýsmý
    Vector3 hoopBottomInitialScale;
    private Vector3 touchStartPos;
    private Vector3 touchEndPos;
    private bool isDragging = false;
    public float maxStretch = 2f; // Potanýn alt kýsmýnýn maksimum uzama miktarý
    private Rigidbody2D ballRb;
    public LineRenderer trajectoryLineRenderer; // Çizim için LineRenderer

    private void Start()
    {
        hoopBottomInitialScale = hoopBottom.localScale;
        ballRb = FindObjectOfType<BallController>().GetComponent<Rigidbody2D>();
        trajectoryLineRenderer.positionCount = 0; // Baþlangýçta çizgiyi gizle
        trajectoryLineRenderer.useWorldSpace = true; // Dünya koordinatlarýný kullan
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            touchStartPos.z = 0;
            isDragging = true;
            ballRb.velocity = Vector2.zero; // Topun hareketini durdur
            ballRb.isKinematic = true; // Fizik etkilerini geçici olarak devre dýþý býrak
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            Vector3 currentTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentTouchPos.z = 0;

            Vector3 direction = touchStartPos - currentTouchPos; // Yönü doðru hesapla
            float distance = Mathf.Clamp(direction.magnitude, 0, maxStretch);

            // Potanýn alt kýsmýný uzat
            hoopBottom.localScale = new Vector3(hoopBottomInitialScale.x, hoopBottomInitialScale.y + distance, hoopBottomInitialScale.z);

            // Potanýn yönünü deðiþtir
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            hoopBottom.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90)); // 90 derece çýkarmamýzýn sebebi, baþlangýç rotasyonunun yukarýyý göstermesini saðlamak

            // Topun pozisyonunu ve rotasyonunu ayarla
            ballRb.transform.position = hoopBottom.position;
            ballRb.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

            // Tahmini hareketi çiz
            Vector3 force = direction * 10; // Gücü ayarla
            DrawTrajectory(ballRb, force);
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;

            // Potanýn alt kýsmýný sýfýrla
            hoopBottom.localScale = hoopBottomInitialScale;
            hoopBottom.rotation = Quaternion.identity;

            // Topa kuvvet uygula
            touchEndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            touchEndPos.z = 0;
            Vector3 force = (touchStartPos - touchEndPos) * 10;

            ballRb.isKinematic = false;
            ballRb.AddForce(force, ForceMode2D.Impulse); // Çarpaný ayarlayabilirsiniz

            // Çizgiyi gizle
            trajectoryLineRenderer.positionCount = 0;
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
}
