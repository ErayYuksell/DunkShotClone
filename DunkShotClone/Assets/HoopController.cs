using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoopController : MonoBehaviour
{
    public Transform hoopBottom; // Potan�n alt k�sm�
    Vector3 hoopBottomInitialScale;
    private Vector3 touchStartPos;
    private Vector3 touchEndPos;
    private bool isDragging = false;
    public float maxStretch = 2f; // Potan�n alt k�sm�n�n maksimum uzama miktar�
    private Rigidbody2D ballRb;
    public LineRenderer trajectoryLineRenderer; // �izim i�in LineRenderer

    private void Start()
    {
        hoopBottomInitialScale = hoopBottom.localScale;
        ballRb = FindObjectOfType<BallController>().GetComponent<Rigidbody2D>();
        trajectoryLineRenderer.positionCount = 0; // Ba�lang��ta �izgiyi gizle
        trajectoryLineRenderer.useWorldSpace = true; // D�nya koordinatlar�n� kullan
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            touchStartPos.z = 0;
            isDragging = true;
            ballRb.velocity = Vector2.zero; // Topun hareketini durdur
            ballRb.isKinematic = true; // Fizik etkilerini ge�ici olarak devre d��� b�rak
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            Vector3 currentTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentTouchPos.z = 0;

            Vector3 direction = touchStartPos - currentTouchPos; // Y�n� do�ru hesapla
            float distance = Mathf.Clamp(direction.magnitude, 0, maxStretch);

            // Potan�n alt k�sm�n� uzat
            hoopBottom.localScale = new Vector3(hoopBottomInitialScale.x, hoopBottomInitialScale.y + distance, hoopBottomInitialScale.z);

            // Potan�n y�n�n� de�i�tir
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            hoopBottom.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90)); // 90 derece ��karmam�z�n sebebi, ba�lang�� rotasyonunun yukar�y� g�stermesini sa�lamak

            // Topun pozisyonunu ve rotasyonunu ayarla
            ballRb.transform.position = hoopBottom.position;
            ballRb.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

            // Tahmini hareketi �iz
            Vector3 force = direction * 10; // G�c� ayarla
            DrawTrajectory(ballRb, force);
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;

            // Potan�n alt k�sm�n� s�f�rla
            hoopBottom.localScale = hoopBottomInitialScale;
            hoopBottom.rotation = Quaternion.identity;

            // Topa kuvvet uygula
            touchEndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            touchEndPos.z = 0;
            Vector3 force = (touchStartPos - touchEndPos) * 10;

            ballRb.isKinematic = false;
            ballRb.AddForce(force, ForceMode2D.Impulse); // �arpan� ayarlayabilirsiniz

            // �izgiyi gizle
            trajectoryLineRenderer.positionCount = 0;
        }
    }

    void DrawTrajectory(Rigidbody2D rb, Vector3 force)
    {
        Vector3[] points = new Vector3[30]; // 30 noktay� hesaplayal�m
        Vector2 startingPosition = rb.position;
        Vector2 startingVelocity = force / rb.mass * Time.fixedDeltaTime;

        for (int i = 0; i < points.Length; i++)
        {
            float t = i / (float)points.Length; // Zaman ad�m�
            points[i] = startingPosition + startingVelocity * t + 0.5f * Physics2D.gravity * t * t;
            points[i].z = 0;
        }

        trajectoryLineRenderer.positionCount = points.Length;
        trajectoryLineRenderer.SetPositions(points);
    }
}
