using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviourPun
{
    public GameObject playerPrefab;

    private void Start()
    {
        Vector3 randomPos = new Vector3(Random.Range(-5, 5), 1, Random.Range(-5, 5));
        PhotonNetwork.Instantiate(playerPrefab.name, randomPos, Quaternion.identity);
    }
}
