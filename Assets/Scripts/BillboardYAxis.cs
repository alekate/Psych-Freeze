using UnityEngine;

public class BillboardYAxis : MonoBehaviour
{
    private GameObject player;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
    }
    void Update()
    {
        if (player == null) return;

        Vector3 targetPosition = player.transform.position;

        targetPosition.y = transform.position.y;

        transform.LookAt(targetPosition);
    }
}
