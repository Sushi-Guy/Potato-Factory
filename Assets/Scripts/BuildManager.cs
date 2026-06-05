using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public enum EditMode { None, Buy, Delete, Select, Move, Rotate }

    [Header("Current Tool State")]
    public EditMode currentMode = EditMode.None;
    public bool isBuildModeActive = false;

    [Header("Economy Link")]
    [Tooltip("Attach your master PlayerWallet component here to manage global cash.")]
    public PlayerWallet playerWallet; 

    [Header("Grid & Raycast Settings")]
    public float gridSize = 2f; 
    public LayerMask floorLayer; 
    public LayerMask obstacleLayer; 
    public LayerMask machineLayer; 

    [Header("Hologram Materials")]
    public Material validGhostMaterial;
    public Material invalidGhostMaterial;

    [Header("UI References")]
    public GameObject hotbarPanel; 
    public GameObject shopPanel;

    // Dynamic slots managed by your UI buttons
    private GameObject objectToBuild; 
    private GameObject ghostPrefab;   
    private int currentItemCost = 0;   
    private float currentGhostHeightOffset = 0f;

    private GameObject currentGhost;
    private Renderer[] ghostRenderers;
    private bool isValidPlacement = false;
    
    // Selection state trackers
    private GameObject selectedObject;
    private SelectableMachine highlightedMachine;
    private bool isMovingObject = false;

    void Start()
    {
        if (hotbarPanel != null) hotbarPanel.SetActive(true); 
        if (shopPanel != null) shopPanel.SetActive(false);
        isBuildModeActive = false;
        currentMode = EditMode.None;

        // Auto-fallback check if you forgot to drag the wallet into the Inspector slot
        if (playerWallet == null)
        {
            playerWallet = Object.FindFirstObjectByType<PlayerWallet>();
        }
    }

    void Update()
    {
       if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleBuildSystem();
        }

        // FIX: Moving the shop shortcut out of the restriction box! 
        // Now you can tap 2 to browse your blueprints whenever you want.
        if (Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            // Automatically wake up build mode so you can place items right after buying!
            if (!isBuildModeActive) ToggleBuildSystem(); 
            
            ToggleShopMenu(); 
        }

        // 2. These tools still require you to press 'E' first to use them
        if (isBuildModeActive)
        {
            if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeMode(3); // 1 = Select Mode
            if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeMode(2); // 3 = Delete Mode
            if (Input.GetKeyDown(KeyCode.Alpha4)) ChangeMode(4); // 4 = Move Mode
            if (Input.GetKeyDown(KeyCode.Alpha5)) ChangeMode(5); // 5 = Rotate Mode
        }

    if (!isBuildModeActive) return;
        // 1. BUY / PLACE BRUSH LOGIC
        if (currentMode == EditMode.Buy)
        {
            if (currentGhost == null) SpawnGhost();
            UpdateGhostPosition();
            CheckPlacementValidity();

            if (Input.GetMouseButtonDown(0) && isValidPlacement)
            {
                PlaceObject();
            }
        }
        // 2. MOVE BRUSH LOGIC (Glides perfectly over the floor layer)
        // 2. FIXED MOVE BRUSH LOGIC (Ignores machine collision + center-tile offset!)
        // 2. FIXED MOVE BRUSH LOGIC (Keeps grid alignment AND fixes sinking!)
        else if (currentMode == EditMode.Move && selectedObject != null)
        {
            DestroyGhost(); 
            isMovingObject = true;

            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, 100f, floorLayer))
            {
                float snappedX = Mathf.Round(hit.point.x / gridSize) * gridSize;
                float snappedZ = Mathf.Round(hit.point.z / gridSize) * gridSize;
                
                snappedX += gridSize / 2f;
                snappedZ += gridSize / 2f;
                
                // FIX: Instead of snapping exactly to the floor surface (hit.point.y), 
                // we can add an offset, OR use your custom currentGhostHeightOffset variable 
                // so it lifts back up to its proper standing position!
                float targetY = hit.point.y + currentGhostHeightOffset;
                
                selectedObject.transform.position = new Vector3(snappedX, targetY, snappedZ);
            }

            // Click again to confirm new position
            if (Input.GetMouseButtonDown(0))
            {
                isMovingObject = false;
                ClearSelection(); 
                currentMode = EditMode.Select; 
            }
        }
        else
        {
            if (!isMovingObject) DestroyGhost();
        }

        // 3. INTERACTIVE MOUSE CLICK ACTIONS
        if (Input.GetMouseButtonDown(0) && currentMode != EditMode.Buy && !isMovingObject)
        {
            HandleMachineInteraction();
        }
    }

    public void ChangeMode(int modeIndex)
    {
        if (isMovingObject) isMovingObject = false;
        currentMode = (EditMode)modeIndex;
        Debug.Log("Switched tool to: " + currentMode.ToString());
    }

    public void SelectItemToBuy(GameObject realPrefab, GameObject blueprintPrefab, int cost, float heightOffset)
    {
        ClearSelection();
        DestroyGhost(); 

        objectToBuild = realPrefab;
        ghostPrefab = blueprintPrefab;
        currentItemCost = cost;
        currentGhostHeightOffset = heightOffset;

        currentMode = EditMode.Buy; 
        
        // FIX: Force the shop flag to FALSE so your camera unfreezes instantly!
        IsShopOpen = false; 
        
        if (shopPanel != null) shopPanel.SetActive(false);
        
        // Re-lock the mouse to the center so first-person looking works
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // GLOBAL STATIC FLAG: Any script in the entire game can read this variable instantly!
    public static bool IsShopOpen = false;

    public void ToggleShopMenu()
    {
        if (!isBuildModeActive) return;
        
        if (shopPanel != null)
        {
            bool isOpening = !shopPanel.activeSelf;
            shopPanel.SetActive(isOpening);

            // Update our global camera blocker flag
            IsShopOpen = isOpening;

            if (isOpening)
            {
                Cursor.lockState = CursorLockMode.None; 
                Cursor.visible = true;                  
                if (hotbarPanel != null) hotbarPanel.SetActive(false);
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked; 
                Cursor.visible = false;
                if (hotbarPanel != null) hotbarPanel.SetActive(true);
            }
        }
    }

    void ToggleBuildSystem()
    {
        isBuildModeActive = !isBuildModeActive;
        //if (hotbarPanel != null) hotbarPanel.SetActive(isBuildModeActive);

        if (!isBuildModeActive)
        {
            ClearSelection();
            currentMode = EditMode.None;
            DestroyGhost();
            if (shopPanel != null) shopPanel.SetActive(false);
            
            IsShopOpen = false; // <--- ADD THIS LINE: Reset camera tracking if building is turned off
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            hotbarPanel.SetActive(true);
        }
        else
        {
            currentMode = EditMode.Select; 
            hotbarPanel.SetActive(true);
        }
    }

    void HandleMachineInteraction()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, machineLayer))
        {
            GameObject targetMachine = hit.transform.root.gameObject;

            if (currentMode == EditMode.Select)
            {
                ClearSelection(); 

                selectedObject = targetMachine;
                highlightedMachine = selectedObject.GetComponent<SelectableMachine>();

                if (highlightedMachine != null)
                {
                    highlightedMachine.SetHighlight(true); 
                }
                Debug.Log("Highlighted: " + selectedObject.name);
            }

            if (currentMode == EditMode.Rotate)
            {
                if (targetMachine == selectedObject)
                {
                    targetMachine.transform.Rotate(0, 90, 0);
                }
            }

            if (currentMode == EditMode.Delete)
            {
                // Refunds half cash back to your global wallet!
                if (playerWallet != null) playerWallet.AddGlobalCash(currentItemCost / 2); 
                Destroy(targetMachine);
                ClearSelection();
            }
        }
        else
        {
            if (currentMode == EditMode.Select) ClearSelection();
        }
    }

    void ClearSelection()
    {
        if (highlightedMachine != null)
        {
            highlightedMachine.SetHighlight(false); 
        }
        selectedObject = null;
        highlightedMachine = null;
    }

    void SpawnGhost()
    {
        if (ghostPrefab != null)
        {
            currentGhost = Instantiate(ghostPrefab);
            ghostRenderers = currentGhost.GetComponentsInChildren<Renderer>();
            hotbarPanel.SetActive(true);
        }
    }

    void DestroyGhost()
    {
        if (currentGhost != null) Destroy(currentGhost);
        hotbarPanel.SetActive(true);
    }

    void UpdateGhostPosition()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, floorLayer))
        {
            // 1. Find the intersection grid line point
            float snappedX = Mathf.Round(hit.point.x / gridSize) * gridSize;
            float snappedZ = Mathf.Round(hit.point.z / gridSize) * gridSize;
            
            // FIX: Add half of the grid size (1.0f if gridSize is 2) to push the model 
            // away from the lines and right into the center of the floor tile!
            snappedX += gridSize / 2f;
            snappedZ += gridSize / 2f;

            currentGhost.transform.position = new Vector3(snappedX, hit.point.y + currentGhostHeightOffset, snappedZ);
        }
    }

    void CheckPlacementValidity()
    {
        if (currentGhost == null) return;
        Vector3 boxHalfExtents = new Vector3(gridSize * 0.45f, 0.5f, gridSize * 0.45f);
        
        int ghostLayerIndex = LayerMask.NameToLayer("Ghost");
        int layerMaskToCheck = ~(1 << ghostLayerIndex);

        bool isColliding = Physics.CheckBox(currentGhost.transform.position + Vector3.up, boxHalfExtents, currentGhost.transform.rotation, layerMaskToCheck);
        
        // Economy check linked directly to the upgraded global master bank script
        bool hasEnoughMoney = playerWallet != null && playerWallet.CanAfford(currentItemCost);
        isValidPlacement = !isColliding && hasEnoughMoney;

        Material targetMat = isValidPlacement ? validGhostMaterial : invalidGhostMaterial;
        foreach (Renderer r in ghostRenderers)
        {
            if (r != null) r.sharedMaterial = targetMat; 
        }
    }

    void PlaceObject()
    {
        if (playerWallet != null) playerWallet.SpendCash(currentItemCost);
        GameObject spawned = Instantiate(objectToBuild, currentGhost.transform.position, currentGhost.transform.rotation);
        spawned.layer = (int)Mathf.Log(machineLayer.value, 2);
        
        if (spawned.GetComponent<SelectableMachine>() == null)
        {
            spawned.AddComponent<SelectableMachine>();
            hotbarPanel.SetActive(true);
        }
    }
}