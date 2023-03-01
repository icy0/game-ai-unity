using System.Collections;
using UnityEngine;

public class AmmunitionSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject ammoBox;
    private Transform[] spawnPointTransforms;

	void Start ()
    {
        spawnPointTransforms = GetComponentsInChildren<Transform>();
        StartCoroutine("SpawnAmmoBox");
    }

    public IEnumerator SpawnAmmoBox()
    {
        while(true)
        {
            Transform spawnPoint = spawnPointTransforms[Random.Range(0, spawnPointTransforms.Length)];
            Vector3 position = spawnPoint.position;
            position += new Vector3(0, 20, 0);
            Instantiate(ammoBox, position, Quaternion.identity);

            yield return new WaitForSecondsRealtime(Random.Range(8, 10));
        }
    }
}
