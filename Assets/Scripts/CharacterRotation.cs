using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRotation : MonoBehaviour
{
    [SerializeField]
    private Camera cam;

    [SerializeField]
    private float sensitivity = 5f;

    private float rotY = 0;

    void Start()
    {
        
    }

    void Update()
    {
        float rotX = Input.GetAxisRaw("Mouse X") * sensitivity;
        rotY += -Input.GetAxisRaw("Mouse Y") * sensitivity;

        transform.Rotate(Vector3.up * rotX);

        if (cam != null)
        {
            cam.transform.localRotation = Quaternion.Euler(rotY, 0, 0);
        }
    }
}
