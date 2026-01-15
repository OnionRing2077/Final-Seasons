using UnityEngine;
using Photon.Pun;

public class PlayerMovement2D : MonoBehaviourPunCallbacks, IPunObservable
{
    public float walkSpeed = 3f;
    public float runSpeed = 6f;

    Rigidbody2D rb;
    Animator anim;

    Vector2 input;
    Vector2 lastMove;      // ของ local player
    Vector2 netLastMove;   // ที่มาจาก network

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");
            input.Normalize();

            if (input.sqrMagnitude > 0.01f)
                lastMove = input;

            bool isRun = Input.GetKey(KeyCode.LeftShift);

            // isometric direction
            float isoX = lastMove.x - lastMove.y;
            float isoY = (lastMove.x + lastMove.y) * 0.5f;

            anim.SetFloat("MoveX", isoX);
            anim.SetFloat("MoveY", isoY);
            anim.SetFloat("Speed", input.magnitude);
            anim.SetBool("IsRun", isRun);

            // flip only when pressing left/right
            if (Mathf.Abs(lastMove.x) > 0.05f)
            {
                Vector3 s = transform.localScale;
                s.x = Mathf.Abs(s.x) * (lastMove.x > 0 ? 1 : -1);
                transform.localScale = s;
            }
        }
        else
        {
            // remote player animation
            float isoX = netLastMove.x - netLastMove.y;
            float isoY = (netLastMove.x + netLastMove.y) * 0.5f;

            anim.SetFloat("MoveX", isoX);
            anim.SetFloat("MoveY", isoY);
        }
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        bool isRun = Input.GetKey(KeyCode.LeftShift);
        float speed = isRun ? runSpeed : walkSpeed;

        rb.velocity = input * speed;
    }

    // ===== NETWORK SYNC =====
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(lastMove);
            stream.SendNext(transform.localScale.x);
        }
        else
        {
            netLastMove = (Vector2)stream.ReceiveNext();
            float sx = (float)stream.ReceiveNext();

            Vector3 s = transform.localScale;
            s.x = sx;
            transform.localScale = s;
        }
    }
}
