using UnityEngine;

public class BillboardYAxis : MonoBehaviour
{
    private GameObject player;

    private void Awake()
    {
        player = GameObject.FindWithTag("MainCamera");
    }
    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("MainCamera");
        }

        Vector3 targetPosition = player.transform.position;

        targetPosition.y = transform.position.y;

        transform.LookAt(targetPosition);
    }
}
