using UnityEngine;
using Photon.Pun;

public class PlayerMovement2D : MonoBehaviourPun, IPunObservable
{
    public float walkSpeed = 2.5f;
    public float runSpeed = 5f;

    Rigidbody2D rb;
    Animator anim;

    Vector2 input;
    Vector2 lastMove = Vector2.down;

    // ===== network =====
    Vector3 netPos;
    Vector2 netLastMove;
    bool netIsMoving;
    bool netIsRun;

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

            bool isMoving = input.magnitude > 0.1f;
            bool isRun = Input.GetKey(KeyCode.LeftShift);

            if (isMoving)
                lastMove = input;

            UpdateAnimator(lastMove, isMoving, isRun);
        }
        else
        {
            // Smooth movement for remote player
            transform.position = Vector3.Lerp(transform.position, netPos, Time.deltaTime * 10f);

            UpdateAnimator(netLastMove, netIsMoving, netIsRun);
        }
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        rb.velocity = input * speed;
    }

    void UpdateAnimator(Vector2 move, bool isMoving, bool isRun)
    {
        // isometric facing
        float isoX = move.x - move.y;
        float isoY = (move.x + move.y) * 0.5f;

        anim.SetFloat("MoveX", isoX);
        anim.SetFloat("MoveY", isoY);
        anim.SetBool("IsMoving", isMoving);
        anim.SetBool("IsRun", isRun);

        // Flip ONLY when pressing left/right
        if (Mathf.Abs(move.x) > 0.01f)
        {
            Vector3 s = transform.localScale;
            s.x = Mathf.Abs(s.x) * (move.x > 0 ? 1 : -1);
            transform.localScale = s;
        }
    }

    // ===== Photon Sync =====
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(lastMove);
            stream.SendNext(input.magnitude > 0.1f);
            stream.SendNext(Input.GetKey(KeyCode.LeftShift));
        }
        else
        {
            netPos = (Vector3)stream.ReceiveNext();
            netLastMove = (Vector2)stream.ReceiveNext();
            netIsMoving = (bool)stream.ReceiveNext();
            netIsRun = (bool)stream.ReceiveNext();
        }
    }
}
