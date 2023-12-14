using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 10;
    [SerializeField]
    private float ratateSpeed = 10;
    private Vector3 moveDirection;

    [SerializeField]
    private Rigidbody rigidbody;

    private void Update()
    {
        rigidbody.AddRelativeForce(0, 0, moveSpeed * Input.GetAxisRaw("Vertical"), ForceMode.Force);

        transform.Rotate(0, ratateSpeed * Input.GetAxisRaw("Horizontal") * Time.deltaTime, 0, Space.Self);
    }
}
