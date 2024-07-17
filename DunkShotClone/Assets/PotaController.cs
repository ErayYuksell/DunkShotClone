using UnityEngine;

public class PotaController : MonoBehaviour
{
    public Transform ball; // Pota i�indeki topun referans�
    public Rigidbody2D ballRb;
    public float maxDragDistance = 2f;
    public Transform nextPota; // Bir sonraki potan�n referans�
    private Vector3 dragStartPosition;
    private bool isDragging = false;
    private Quaternion initialRotation;
    private bool isBallInPota = false; // Topun pota i�inde olup olmad���n� takip eder

    void Start()
    {
        initialRotation = transform.rotation; // Potan�n ba�lang��taki d�n���n� kaydet
    }

    void Update()
    {
        if (isBallInPota)
        {
            ball.position = transform.position; // Topu potan�n i�inde tut
        }

        // Ekrana dokundu�umuzda
        if (Input.GetMouseButtonDown(0) && isBallInPota)
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPosition.z = 0; // Z eksenini s�f�rla
            Collider2D hitCollider = Physics2D.OverlapPoint(worldPosition);

            if (hitCollider != null && hitCollider.transform == ball)
            {
                isDragging = true;
                dragStartPosition = ball.position; // Topun ba�lang�� pozisyonunu kaydet
            }
        }

        // Dokunmay� s�rd�rd���m�zde
        if (isDragging && Input.GetMouseButton(0))
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPosition.z = 0; // Z eksenini s�f�rla
            Vector3 dragVector = worldPosition - dragStartPosition;

            if (dragVector.magnitude > maxDragDistance)
            {
                dragVector = dragVector.normalized * maxDragDistance;
            }

            // Potan�n pozisyonunu ve y�n�n� ayarla
            transform.position = dragStartPosition + dragVector / 2;
            ball.position = transform.position;

            float angle = Mathf.Atan2(dragVector.y, dragVector.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + 90); // Potan�n y�n�n� do�ru ayarlamak i�in +90 derece ekle
        }

        // Dokunmay� b�rakt���m�zda
        if (isDragging && Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            Vector3 releasePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            releasePosition.z = 0; // Z eksenini s�f�rla
            Vector3 releaseVector = dragStartPosition - releasePosition;

            ballRb.velocity = Vector2.zero; // �nceki h�zlar� s�f�rlay�n
            ballRb.angularVelocity = 0f; // �nceki a��sal h�zlar� s�f�rlay�n
            ballRb.AddForce(releaseVector * 100); // Kuvveti uygulamak i�in �arpan

            // Potan�n ba�lang�� pozisyonuna ve y�n�ne d�nmesini sa�la
            transform.position = dragStartPosition;
            transform.rotation = initialRotation; // Potan�n ba�lang��taki d�n���n� geri y�kle

            isBallInPota = false; // Top art�k potada de�il
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform == ball)
        {
            isBallInPota = true;
            if (nextPota != null)
            {
                PotaController nextPotaController = nextPota.GetComponent<PotaController>();
                if (nextPotaController != null)
                {
                    nextPotaController.ReceiveBall(ball);
                }
            }
        }
    }

    public void ReceiveBall(Transform ball)
    {
        this.ball = ball;
        ballRb = ball.GetComponent<Rigidbody2D>();
        isBallInPota = true;
    }
}
