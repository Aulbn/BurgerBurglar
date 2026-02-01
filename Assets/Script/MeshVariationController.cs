using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class MeshVariationController : MonoBehaviour
{
    public GameObject[] RandomMeshes;

    private void Start()
    {
        int chosenIndex = Random.Range(0, RandomMeshes.Length);
        for (int i = 0; i < RandomMeshes.Length; i++)
        {
            RandomMeshes[i].SetActive(i == chosenIndex);
        }
    }
}
