using UnityEngine;
using Photon.Pun;

public class PlayerMovement2D : MonoBehaviourPun
{
    public float walkSpeed = 3f;
    public float runSpeed = 6f;

    Rigidbody2D rb;
    Animator anim;

    Vector2 input;
    Vector2 lastMove;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        // อ่าน input
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        input.Normalize();

        if (input != Vector2.zero)
            lastMove = input;

        bool isRun = Input.GetKey(KeyCode.LeftShift);

        // ใช้ velocity จริง → ไม่มีวันสไลด์
        float speed = rb.velocity.magnitude;

        anim.SetFloat("Speed", speed);
        anim.SetBool("IsRun", isRun);
        anim.SetFloat("MoveX", lastMove.x);
        anim.SetFloat("MoveY", lastMove.y);

        // Flip sprite
        if (lastMove.x != 0)
        {
            Vector3 s = transform.localScale;
            s.x = Mathf.Abs(s.x) * (lastMove.x < 0 ? -1 : 1);
            transform.localScale = s;
        }
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        bool isRun = Input.GetKey(KeyCode.LeftShift);
        float speed = isRun ? runSpeed : walkSpeed;

        rb.velocity = input * speed;
    }
}
