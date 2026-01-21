using Photon.Pun;
using UnityEngine;

public class DeadBody : MonoBehaviourPun
{
    public int ownerActor;      // ใครตาย
    public bool isReported;     // กัน report ซ้ำ

    public void Init(int actorNumber)
    {
        ownerActor = actorNumber;
        isReported = false;
    }

    void Awake()
    {
        if (GetComponent<Collider2D>() == null)
        {
            var col = gameObject.AddComponent<BoxCollider2D>();
            col.isTrigger = true; // Make it a trigger so players can walk over it
            col.size = new Vector2(1f, 1f); // Default size
        }
    }
}
