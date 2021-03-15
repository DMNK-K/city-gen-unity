using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField]
    private float speed = 10;
    [SerializeField]
    private float speedMult = 5;

    CharacterController cc;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        Vector3 move = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            move.z = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            move.z = -1;
        }

        if (Input.GetKey(KeyCode.D))
        {
            move.x = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            move.x = -1;
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            move.y = -1;
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            move.y = 1;
        }

        Vector3 velocity = (transform.forward * move.z + transform.up * move.y + transform.right * move.x) * speed; 
        if (Input.GetKey(KeyCode.LeftShift))
        {
            velocity *= speedMult;
        }
        cc.Move(velocity * Time.deltaTime);
    }
}
