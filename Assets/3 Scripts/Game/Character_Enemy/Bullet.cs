using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inca;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;
    Vector3 dir;

    private int attack;

    private void OnEnable()
    {
        dir = (IncaData.PlayerPosition - transform.position).normalized;
        // Vector3 desiredVelocity = (IncaData.PlayerPosition - transform.position).normalized * 30;
        // dir = desiredVelocity;
        transform.SetParent(IncaData.PlayerTransform);
    }

    public void Setup(Vector3 dir)
    {
        this.dir = dir.normalized;
    }

    public void SetAttack(int attack) => this.attack = attack;

    private void FixedUpdate()
    {

        // Vector3 steerForce = desiredVelocity - IncaData.PlayerVelocity;

        dir = (IncaData.PlayerPosition - transform.position);
        dir.Normalize();

        if ((IncaData.PlayerPosition - transform.position).sqrMagnitude <= 1)
        {
            Player.Instance.Hit(attack);
            // MemoryPool.Instance(MemoryPoolType.Enemy).DeactivatePoolItem(gameObject);
            Destroy(this.gameObject);
        }

        transform.position += dir * moveSpeed * Time.deltaTime;
    }
}
