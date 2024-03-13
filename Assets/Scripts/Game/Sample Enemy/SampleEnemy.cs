using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inca;

public class SampleEnemy : MonoBehaviour
{
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private float shootingTime = 0.5f;

    private void Start()
    {
        StartCoroutine(ShootRoutine());
    }

    void Update()
    {
        transform.LookAt(IncaData.PlayerPosition, Vector3.up);
    }

    private IEnumerator ShootRoutine()
    {
        yield return new WaitForSeconds(0.1f);
        while (true)
        {
            Instantiate(bulletPrefab, transform.position, transform.rotation);
            yield return new WaitForSeconds(shootingTime);
        }
    }
}
