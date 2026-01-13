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

    input.x = Input.GetAxisRaw("Horizontal");
    input.y = Input.GetAxisRaw("Vertical");
    input.Normalize();

    if (input != Vector2.zero)
        lastMove = input;   // ⭐ ใช้ตัวนี้เป็นทิศจริง

    bool isRun = Input.GetKey(KeyCode.LeftShift);

    float speed = rb.velocity.magnitude;

    anim.SetFloat("Speed", speed);
    anim.SetBool("IsRun", isRun);
}


    void LateUpdate()
{
    if (!photonView.IsMine) return;

    if (rb.velocity.sqrMagnitude < 0.01f) return;

    Vector2 v = rb.velocity.normalized;

    // แปลง world movement → isometric facing
    float isoX = v.x - v.y;
    float isoY = (v.x + v.y) * 0.5f;

    // Flip sprite
    if (Mathf.Abs(isoX) > 0.01f)
    {
        Vector3 s = transform.localScale;
        s.x = Mathf.Abs(s.x) * (isoX > 0 ? 1 : -1);
        transform.localScale = s;
    }

    anim.SetFloat("MoveX", isoX);
    anim.SetFloat("MoveY", isoY);
}



    void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        bool isRun = Input.GetKey(KeyCode.LeftShift);
        float speed = isRun ? runSpeed : walkSpeed;

        rb.velocity = input * speed;
    }
}
