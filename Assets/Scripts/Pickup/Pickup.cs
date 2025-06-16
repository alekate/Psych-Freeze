using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    private PickupCounter pickupCounter;
    private CheatScript cheatScript;
    private Transform playerPos;

    private bool followPlayer = false;
    public bool hasBeenCollected = false; 

    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        cheatScript = FindObjectOfType<CheatScript>();
        pickupCounter = player.GetComponent<PickupCounter>();
        playerPos = player.transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasBeenCollected)
        {
            hasBeenCollected = true; 
            followPlayer = true;
            pickupCounter.currentPickups++;

            StartCoroutine(FollowAndDestroy());
        }
    }

    private void Update()
    {
        InstaWin();
    }

    private IEnumerator FollowAndDestroy()
    {
        float followDuration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < followDuration)
        {
            transform.position = Vector3.Lerp(transform.position, playerPos.position, Time.deltaTime * 10f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
    public void InstaWin()
    {
        if (cheatScript.cheatModeIsActive)
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                hasBeenCollected = true;
                followPlayer = true;
                pickupCounter.currentPickups++;

                StartCoroutine(FollowAndDestroy());
            }
        }
    }
}
