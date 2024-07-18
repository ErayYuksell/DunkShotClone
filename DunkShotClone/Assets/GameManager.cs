using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] GameObject hoopPrefab;
    [SerializeField] List<GameObject> hoops = new List<GameObject>();

    [SerializeField] GameObject hoopWrapper;

    int removeCount = -1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }
    }
    public void SetLocation(GameObject hoop)
    {
        float newY = hoops[0].transform.position.y + Random.Range(2f, 3f); // 0 dememin nedeni zaten kesin 2 pota olucak listede ve her zaman 1 indisli eleman degisecek 
        Vector3 location = new Vector3(Random.Range(1.5f, -1.5f), newY, 0);
        hoop.transform.position = location;
    }
    public void CreateHoop()
    {
        if (hoops.Count < 2) // Eðer 2'den az pota varsa
        {
            GameObject hoopObj = Instantiate(hoopPrefab, hoopWrapper.transform);
            hoops.Add(hoopObj);
            SetLocation(hoopObj);
        }
    }

    public void RemoveHoopFromList()
    {
        hoops[0].SetActive(false);
        hoops.Remove(hoops[0]);
        CreateHoop(); // Pota sayýsýný kontrol et ve gerekirse yeni pota ekle
    }

    public bool GetHoopScoredInfo()
    {
        return hoops[1].GetComponent<HoopController>().hasScored;
    }
}
