using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRot : MonoBehaviour
{
    [SerializeField] float sensX;
    [SerializeField] float sensY;

    [SerializeField] Transform orient;

    private float xRotation;
    private float yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;


        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 60f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orient.rotation = Quaternion.Euler(0, yRotation, 0);

    }
}
