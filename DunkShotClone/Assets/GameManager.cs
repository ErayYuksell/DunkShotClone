using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] GameObject hoopPrefab;
    [SerializeField] List<GameObject> hoopstypes = new List<GameObject>();
    [SerializeField] List<GameObject> hoops = new List<GameObject>();
    [SerializeField] GameObject hoopWrapper;

    int creationAmount = 0;
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
        float newX;

        if (hoops[0].transform.position.x < 0)
        {
            newX = Random.Range(0, 1.5f);
        }
        else
        {
            newX = Random.Range(-1.5f, 0);
        }

        Vector3 location = new Vector3(newX, newY, 0);
        hoop.transform.position = location;
    }
    public void CreateHoop()
    {
        if (hoops.Count < 2) // Eðer 2'den az pota varsa
        {
            creationAmount++;
            GameObject type;

            if (creationAmount < 4) // 3 kere pota olusturduktan sonra 1 kere star li pota olustur
            {
                type = hoopstypes[0];
            }
            else
            {
                type = hoopstypes[1];
                creationAmount = 0;
            }

            GameObject hoopObj = Instantiate(type, hoopWrapper.transform);
            hoops.Add(hoopObj);
            SetLocation(hoopObj);
        }
    }
    // 1. potaya top girdiginde surekli 0. eleman siliniyor artik 1. pota 0. pota haline gelirken yeni eklenen pota 1. pota oluyor bu sekilde dongu ile ilerliyor 
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
