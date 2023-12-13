using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    private float moveSpeed;
    private Vector3 moveDirection;

    [SerializeField]
    private Rigidbody rigidbody;

    private void Update()
    {
        rigidbody.AddRelativeForce(0, 0, 10 * Input.GetAxisRaw("Vertical"), ForceMode.Force);

        transform.Rotate(0, 5 * Input.GetAxisRaw("Horizontal") * Time.deltaTime, 0, Space.Self);
    }
}
