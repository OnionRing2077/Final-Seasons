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
}
