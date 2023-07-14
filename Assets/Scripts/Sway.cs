using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sway : MonoBehaviour
{
    public float swaySmooth;
    public float swayMultiplier;
    private void Update()
    {
        float inputX = Input.GetAxis("Mouse X") * swayMultiplier;
        float inputY = Input.GetAxis("Mouse Y") * swayMultiplier;

        Quaternion rotationX = Quaternion.AngleAxis(-inputY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(inputX, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, swaySmooth * Time.deltaTime);
    }
}
