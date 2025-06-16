using Clase10;
using UnityEngine;

public class RaycastShoot_ObjectController : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float rayDistance = 100f;
    [SerializeField] private KeyCode shootKey = KeyCode.Mouse0;

    [SerializeField] private PlayerController currentController;

    [SerializeField] private GameObject playerGameObject;
    private bool isPlayer = true;

    [SerializeField] private KeyCode returnKey = KeyCode.F;

    private void Update()
    {
        if (Input.GetKeyDown(shootKey))
        {
            ShootRaycast();
        }

        if (Input.GetKeyDown(returnKey) && !isPlayer)
        {
            ReturnToPlayer();
        }

        isPlayer = currentController != null && currentController.gameObject.CompareTag("Player");

    }


    private void ShootRaycast()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            if (hit.collider.CompareTag("Controllable Object") || hit.collider.CompareTag("Player"))
            {
                GameObject newObj = hit.collider.gameObject;

                if (currentController != null)
                {
                    Rigidbody oldRb = currentController.GetComponent<Rigidbody>();
                    if (oldRb != null)
                        oldRb.isKinematic = true;

                    Destroy(currentController);
                    currentController = null;
                }

                // Obtener o agregar nuevo controlador
                PlayerController newController = newObj.GetComponent<PlayerController>();
                if (newController == null)
                    newController = newObj.AddComponent<PlayerController>();

                Transform newCameraPivot = newObj.transform;
                cameraTransform.SetParent(newCameraPivot);
                cameraTransform.localPosition = Vector3.zero;
                cameraTransform.localRotation = Quaternion.identity;

                newController.cameraPivot = cameraTransform;

                Rigidbody rb = newObj.GetComponent<Rigidbody>();
                if (rb == null)
                    rb = newObj.AddComponent<Rigidbody>();

                rb.isKinematic = false;

                newController.SetRigidbody(rb);
                newController.enabled = true;
                currentController = newController;

            }
        }
    }

    private void ReturnToPlayer()
    {
        // Si no está asignado, lo buscamos
        if (playerGameObject == null)
        {
            playerGameObject = GameObject.FindGameObjectWithTag("Player");

            if (playerGameObject == null)
            {
                Debug.LogWarning("No se encontró ningún GameObject con el tag 'Player'.");
                return;
            }
        }

        // Desactivar el controlador actual
        if (currentController != null)
        {
            Rigidbody oldRb = currentController.GetComponent<Rigidbody>();
            if (oldRb != null)
                oldRb.isKinematic = true; // Freezar objeto anterior

            Destroy(currentController); // Elimina el script del objeto anterior
            currentController = null;
        }

        // Obtener o agregar el controlador del jugador
        PlayerController playerController = playerGameObject.GetComponent<PlayerController>();
        if (playerController == null)
            playerController = playerGameObject.AddComponent<PlayerController>();

        // Mover la cámara
        Transform newPivot = playerGameObject.transform.Find("Camera Holder");
        if (newPivot == null)
        {
            Debug.LogWarning("No se encontró 'Camera Holder'. Usando el transform del jugador.");
            newPivot = playerGameObject.transform;
        }

        cameraTransform.SetParent(newPivot);
        cameraTransform.localPosition = Vector3.zero;
        cameraTransform.localRotation = Quaternion.identity;

        // Reasignar referencias
        playerController.cameraPivot = cameraTransform;

        Rigidbody rb = playerGameObject.GetComponent<Rigidbody>();
        if (rb == null)
            rb = playerGameObject.AddComponent<Rigidbody>();

        //  Hacemos que el player se pueda mover y caiga
        rb.isKinematic = false;
        rb.useGravity = true;

        playerController.SetRigidbody(rb);
        playerController.enabled = true;

        currentController = playerController;
        isPlayer = true;

        Debug.Log("Control retornado al jugador.");
    }

}

