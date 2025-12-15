using UnityEngine;
using Photon.Pun;

public class PlayerMovement3D : MonoBehaviourPun
{
    public float moveSpeed = 5f;
    public Rigidbody rb;
    public Transform cameraTransform;
    public Animator animator;

    private Vector3 movement;

    void Update()
    {
        // ✅ ควบคุมเฉพาะ Player ของเราเท่านั้น
        //if (!photonView.IsMine) return;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        Vector3 inputDirection = new Vector3(moveX, 0f, moveZ).normalized;

        if (inputDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            movement = moveDir.normalized;

            // หมุนตามทิศทางที่เดิน
            //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, targetAngle, 0f), Time.deltaTime * 10f);
        }
        else
        {
            movement = Vector3.zero;
        }

        animator.SetFloat("Speed", movement.magnitude * moveSpeed);
    }

    void FixedUpdate()
    {
        //if (!photonView.IsMine) return;
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}
