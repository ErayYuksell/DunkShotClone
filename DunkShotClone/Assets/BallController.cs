using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BallController : MonoBehaviour
{
    Rigidbody2D rb;

    Vector3 touchStartPos;
    Vector3 touchEndPos;
    bool isDragging = false;
    bool inTheAir = false; // havadayken hi�bir �ey yapamayaca��z

    Transform hoop; // Pota
    LineRenderer trajectoryLineRenderer; // �izim i�in LineRenderer

    HoopController hoopController;

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
                trajectoryLineRenderer.positionCount = 2; // �izgiyi etkinle�tir
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
                Vector3[] linePositions = CalculateTrajectory(rb.position, direction.normalized * distance * 5); // Distance ile �arparak �izginin uzunlu�unu ayarla
                trajectoryLineRenderer.positionCount = linePositions.Length;
                trajectoryLineRenderer.SetPositions(linePositions);
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
                trajectoryLineRenderer.positionCount = 0;
            }
        }
        else
        {
            BallRotate();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Hoop")
        {
            inTheAir = false;

            hoop = collision.gameObject.transform.Find("Hoop");
            hoopController = collision.gameObject.GetComponent<HoopController>();

            trajectoryLineRenderer = hoopController.GetComponent<LineRenderer>();
            trajectoryLineRenderer.positionCount = 0; // Ba�lang��ta �izgiyi gizle
            trajectoryLineRenderer.useWorldSpace = true; // D�nya koordinatlar�n� kullan
        }
        // E�er �arp�lan collider bir CircleCollider2D ise
        CircleCollider2D circleCollider = collision as CircleCollider2D;
        if (circleCollider != null && collision.tag == "Hoop" && !hoopController.hasScored)
        {
            // Skoru art�r
            UIManager.Instance.IncreaseScore(1);
            hoopController.hasScored = true;
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

    public void BallRotate()
    {
        // Topu s�rekli d�nd�r
        transform.Rotate(new Vector3(0, 0, 360) * Time.deltaTime);
    }

    // Topun gidece�i yolu hesaplayan fonksiyon
    private Vector3[] CalculateTrajectory(Vector2 startPosition, Vector2 velocity)
    {
        List<Vector3> positions = new List<Vector3>();
        Vector2 gravity = Physics2D.gravity;
        float timeStep = 0.05f; // Zaman aral���n� biraz art�rd�k
        int steps = 20; // Ad�m say�s�n� art�rarak �izgiyi uzat�yoruz

        for (int i = 0; i < steps; i++)
        {
            float t = i * timeStep;
            Vector3 position = startPosition + velocity * t + 0.5f * gravity * t * t;
            positions.Add(position);
        }

        return positions.ToArray();
    }
}
