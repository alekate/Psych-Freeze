using System.Collections;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    private GameObject player;
    private PickupCounter pickupCounter;
    private Transform playerPos;

    private bool followPlayer = false;
    public bool hasBeenCollected = false;

    private void Start()
    {
        TryAssignReferences();
    }

    private void Update()
    {
        if (player == null || pickupCounter == null || playerPos == null)
        {
            TryAssignReferences();
        }
    }

    private void TryAssignReferences()
    {
        player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            pickupCounter = player.GetComponent<PickupCounter>();
            playerPos = player.transform;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasBeenCollected)
        {
            hasBeenCollected = true;
            followPlayer = true;

            if (pickupCounter != null)
                pickupCounter.currentPickups++;

            StartCoroutine(FollowAndDestroy());
        }
    }

    private IEnumerator FollowAndDestroy()
    {
        float followDuration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < followDuration)
        {
            if (playerPos != null)
                transform.position = Vector3.Lerp(transform.position, playerPos.position, Time.deltaTime * 10f);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
