using UnityEngine;

public class GemSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] gems;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       // GemSpawn();
    }

   public void GemSpawn()
    {
        int index = Random.Range(0, gems.Length);
        print(index);
      //  if (index < gems.Length)
        {
            for (int i = 0; i < gems.Length; i++)
            {
                if (i == index)
                {
                    gems[i].SetActive(true);
                    print("spawned");
                }
              //  gems[i].SetActive(false);
            }
        }
    }
}
