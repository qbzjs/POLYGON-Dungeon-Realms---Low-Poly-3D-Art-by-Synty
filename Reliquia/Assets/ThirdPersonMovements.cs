using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovements : MonoBehaviour
{
    
    public CharacterController controller;
    public float speed = 6f;
    public float speedAim = 3f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public Transform cam;

    private void Update()
    {

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        Vector3 move = transform.right * horizontal + transform.forward * vertical;

        if (direction.magnitude >= 0.1f && this.gameObject.GetComponent<Lighting_Gabriel>().aim == false)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }
        else if (direction.magnitude >= 0.1f && this.gameObject.GetComponent<Lighting_Gabriel>().aim == true)
        {
            controller.Move(move * speedAim * Time.deltaTime);
        }


    }
}
