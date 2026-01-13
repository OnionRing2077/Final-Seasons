using UnityEngine;
using Photon.Pun;

public class PlayerIdentity : MonoBehaviourPun
{
    public string playerName;
    public Color playerColor = Color.white;

    public SpriteRenderer body;
    public NameTagFollow nameTag;

    public void Setup(string name)
    {
        if (photonView.IsMine)
        {
            Color c = Random.ColorHSV(0,1, 0.6f,1f, 0.6f,1f); // สุ่มสีสวย
            photonView.RPC("RPC_Setup", RpcTarget.AllBuffered, name, c.r, c.g, c.b);
        }
    }

    [PunRPC]
    void RPC_Setup(string name, float r, float g, float b)
    {
        playerName = name;
        playerColor = new Color(r, g, b);

        if (nameTag != null)
            nameTag.SetName(name);

        if (body != null)
            body.color = playerColor;
        
        ApplyColor();
    }
    void ApplyColor()
{
    foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
    {
        sr.color = playerColor;
    }
}
}