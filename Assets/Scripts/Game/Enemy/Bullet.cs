using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inca;

public class Bullet : MonoBehaviour
{

    [SerializeField]
    private float moveSpeed;

    void Update()
    {
        Vector3 dir = IncaData.PlayerPosition - transform.position;
        if (dir.sqrMagnitude <= 1)
        {
            Player.Instance?.Hit();
            Destroy(this.gameObject);
        }

        dir.Normalize();
        transform.position += dir * moveSpeed * Time.deltaTime;
    }
}
