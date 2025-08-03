using EasyTransition;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems; 

public class RaycastShoot_ObjectController : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float rayDistance = 100f;
    [SerializeField] private KeyCode shootKey = KeyCode.Mouse0;
    [SerializeField] private KeyCode returnKey = KeyCode.F;

    [Header("Control Settings")]
    [SerializeField] private PlayerFSM currentController;
    [SerializeField] private GameObject playerGameObject;

    private bool isPlayer = true;
    [SerializeField] private GameObject flashLight;

    [Header("Transition")]
    [SerializeField] TransitionSettings transition;
    TransitionManager transitionManager;


    private Transform currentHighlighted;
    private Outline currentOutline;

    private void Start()
    {
        transitionManager = TransitionManager.Instance();
    }

    private void Update()
    {
        // ----------- Lógica outline hover  -----------
        Ray rayHover = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Physics.Raycast(rayHover, out RaycastHit hitHover, rayDistance))
        {
            Transform hitTransform = hitHover.transform;

            if (hitTransform.CompareTag("Controllable Object"))
            {
                // Si es un nuevo objeto distinto al resaltado actualmente
                if (hitTransform != currentHighlighted)
                {
                    if (currentOutline != null)
                        currentOutline.enabled = false;

                    Outline outline = hitTransform.GetComponent<Outline>();
                    if (outline == null)
                    {
                        outline = hitTransform.gameObject.AddComponent<Outline>();
                        outline.OutlineColor = Color.white;
                        outline.OutlineWidth = 10f;
                    }
                    outline.enabled = true;

                    currentHighlighted = hitTransform;
                    currentOutline = outline;
                }
            }
            else
            {
                if (currentOutline != null)
                    currentOutline.enabled = false;

                currentHighlighted = null;
                currentOutline = null;
            }
        }
        else
        {
            if (currentOutline != null)
                currentOutline.enabled = false;

            currentHighlighted = null;
            currentOutline = null;
        }
        // ---------------------------------------------



        if (Input.GetKeyDown(shootKey))
            StartCoroutine(ShootRaycastCoroutine());

        if (Input.GetKeyDown(returnKey) && !isPlayer)
            StartCoroutine(ReturnToPlayerCoroutine());

        isPlayer = currentController != null && currentController.gameObject.CompareTag("Player");

        if (flashLight == null)
            flashLight = GameObject.FindWithTag("FlashLight");

        if (playerGameObject == null)
            playerGameObject = GameObject.FindWithTag("Player");
    }

    private IEnumerator ShootRaycastCoroutine()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (!Physics.Raycast(ray, out RaycastHit hit, rayDistance))
            yield break;

        if (!hit.collider.CompareTag("Controllable Object"))
            yield break;

        // Transición visual
        transitionManager.Transition(transition, 0f);

        float totalTime = transition.transitionTime;
        yield return new WaitForSeconds(totalTime);

        GameObject newObj = hit.collider.gameObject;

        ClearCurrentController();

        playerGameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;

        PlayerFSM newController = GetOrAddController(newObj);
        SetupCamera(newObj, newController);
        SetupRigidbody(newObj, newController);
        flashLight.SetActive(false);
        currentController = newController;
    }

    private IEnumerator ReturnToPlayerCoroutine()
    {
        if (playerGameObject == null)
        {
            playerGameObject = GameObject.FindGameObjectWithTag("Player");
            if (playerGameObject == null)
            {
                Debug.LogWarning("No se encontró ningún GameObject con el tag 'Player'.");
                yield break;
            }
        }

        transitionManager.Transition(transition, 0f);
        float totalTime = transition.transitionTime;
        yield return new WaitForSeconds(totalTime);

        ClearCurrentController();

        PlayerFSM playerController = GetOrAddController(playerGameObject);
        SetupCameraToPlayer(playerGameObject, playerController);
        SetupPlayerRigidbody(playerGameObject, playerController);

        currentController = playerController;
        isPlayer = true;

        flashLight.SetActive(true);
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
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        controller.SetRigidbody(rb);
        controller.enabled = true;
    }

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
