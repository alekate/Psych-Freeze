using Clase10;
using UnityEngine;

public class RaycastShoot_ObjectController : MonoBehaviour
{

    //CONFIGURACIÓN//

    [Header("Raycast Settings")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float rayDistance = 100f;
    [SerializeField] private KeyCode shootKey = KeyCode.Mouse0;
    [SerializeField] private KeyCode returnKey = KeyCode.F;

    [Header("Control Settings")]
    [SerializeField] private PlayerFSM currentController;
    [SerializeField] private GameObject playerGameObject;

    private bool isPlayer = true;


    //UPDATE//
    private void Update()
    {
        if (Input.GetKeyDown(shootKey))
            ShootRaycast();

        if (Input.GetKeyDown(returnKey) && !isPlayer)
            ReturnToPlayer();

        isPlayer = currentController != null && currentController.gameObject.CompareTag("Player");
    }


    //CAMBIO DE OBJETO//
    private void ShootRaycast()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            if (!hit.collider.CompareTag("Controllable Object") && !hit.collider.CompareTag("Player"))
                return;

            GameObject newObj = hit.collider.gameObject;

            ClearCurrentController();

            PlayerFSM newController = GetOrAddController(newObj);
            SetupCamera(newObj, newController);
            SetupRigidbody(newObj, newController);
            UpdateFlashlight(newController);

            currentController = newController;
        }
    }

    private void ClearCurrentController()
    {
        if (currentController != null)
        {
            Rigidbody oldRb = currentController.GetComponent<Rigidbody>();
            if (oldRb != null)
                oldRb.isKinematic = true;

            Destroy(currentController);
            currentController = null;
        }
    }

    private PlayerFSM GetOrAddController(GameObject obj)
    {
        PlayerFSM controller = obj.GetComponent<PlayerFSM>();
        if (controller == null)
            controller = obj.AddComponent<PlayerFSM>();

        return controller;
    }

    private void SetupCamera(GameObject obj, PlayerFSM controller)
    {
        Transform newPivot = FindChildByName(obj.transform, "Camera Pivot");

        if (newPivot == null)
        {
            Debug.LogError($"'{obj.name}' no tiene un hijo llamado 'Camera Pivot'");
            return;
        }

        cameraTransform.SetParent(newPivot);
        cameraTransform.localPosition = controller.CompareTag("Player") ? new Vector3(0f, 0.5f, 0f) : Vector3.zero;
        cameraTransform.localRotation = Quaternion.identity;

        controller.cameraPivot = cameraTransform;
    }

    private void SetupRigidbody(GameObject obj, PlayerFSM controller)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb == null)
            rb = obj.AddComponent<Rigidbody>();

        rb.isKinematic = false;

        controller.SetRigidbody(rb);
        controller.enabled = true;
    }

    private void UpdateFlashlight(PlayerFSM controller)
    {
        FlashlightController flashlight = controller.GetComponentInChildren<FlashlightController>();
        if (flashlight != null)
            flashlight.SetIsPlayer(controller.CompareTag("Player"));
    }


    //RETORNAR PLAYER//
    private void ReturnToPlayer()
    {
        if (playerGameObject == null)
        {
            playerGameObject = GameObject.FindGameObjectWithTag("Player");
            if (playerGameObject == null)
            {
                Debug.LogWarning("No se encontró ningún GameObject con el tag 'Player'.");
                return;
            }
        }

        ClearCurrentController();

        PlayerFSM playerController = GetOrAddController(playerGameObject);
        SetupCameraToPlayer(playerGameObject, playerController);
        SetupPlayerRigidbody(playerGameObject, playerController);
        UpdateFlashlight(playerController);

        currentController = playerController;
        isPlayer = true;

        Debug.Log("Control retornado al jugador.");
    }

    private void SetupCameraToPlayer(GameObject player, PlayerFSM controller)
    {
        Transform pivot = player.transform.Find("Camera Pivot");
        if (pivot == null)
        {
            Debug.LogWarning("No se encontró 'Camera Pivot'. Usando el transform del jugador.");
            pivot = player.transform;
        }

        cameraTransform.SetParent(pivot);
        cameraTransform.localPosition = new Vector3(0f, 0.5f, 0f);
        cameraTransform.localRotation = Quaternion.identity;

        controller.cameraPivot = cameraTransform;
    }

    private void SetupPlayerRigidbody(GameObject player, PlayerFSM controller)
    {
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb == null)
            rb = player.AddComponent<Rigidbody>();

        rb.isKinematic = false;
        rb.useGravity = true;

        controller.SetRigidbody(rb);
        controller.enabled = true;
    }


    //MÉTODOS AUXILIARES//
    private Transform FindChildByName(Transform parent, string name)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == name)
                return child;
        }
        return null;
    }
}
