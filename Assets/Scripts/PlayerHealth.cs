using UnityEngine;
using Photon.Pun;

public class PlayerHealth : MonoBehaviourPun
{
    public bool IsDead { get; private set; }

    public void Kill()
    {
        if (IsDead) return;

        IsDead = true;

        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;

        GetComponent<Animator>().SetTrigger("Die");
    }
}
