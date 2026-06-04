using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public enum EditMode { None, Buy, Delete, Select, Move, Rotate }

    [Header("Current Tool State")]
    public EditMode currentMode = EditMode.None;
    public bool isBuildModeActive = false;

    [Header("Economy Link")]
    public PotatoCollector potatoCollector; 

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

    private GameObject currentGhost;
    private Renderer[] ghostRenderers;
    private bool isValidPlacement = false;
    private GameObject selectedObject;

    void Start()
    {
        if (hotbarPanel != null) hotbarPanel.SetActive(false); 
        if (shopPanel != null) shopPanel.SetActive(false);
        
        isBuildModeActive = false;
        currentMode = EditMode.None;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleBuildSystem();
        }

        if (isBuildModeActive)
        {
            if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeMode(3); // 1 = Select Mode
            if (Input.GetKeyDown(KeyCode.Alpha1)) ToggleShopMenu(); // 2 = Shop
            if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeMode(2); // 3 = Delete Mode
            if (Input.GetKeyDown(KeyCode.Alpha4)) ChangeMode(4); // 4 = Move Mode
            if (Input.GetKeyDown(KeyCode.Alpha5)) ChangeMode(5); // 5 = Rotate Mode
        }

        if (!isBuildModeActive) return;

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
        else
        {
            DestroyGhost();
        }

        if (Input.GetMouseButtonDown(0) && currentMode != EditMode.Buy)
        {
            HandleMachineInteraction();
        }
    }

    public void ChangeMode(int modeIndex)
    {
        currentMode = (EditMode)modeIndex;
        Debug.Log("Switched tool to: " + currentMode.ToString());
    }

    public void SelectItemToBuy(GameObject realPrefab, GameObject blueprintPrefab, int cost)
    {
        DestroyGhost(); 

        objectToBuild = realPrefab;
        ghostPrefab = blueprintPrefab;
        currentItemCost = cost;

        currentMode = EditMode.Buy; 
        
        if (shopPanel != null) shopPanel.SetActive(false);
        
        // Re-lock cursor so placement tracking wakes up!
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        if (hotbarPanel != null) hotbarPanel.SetActive(true);
    }

    public void ToggleShopMenu()
    {
        if (!isBuildModeActive) return;
        
        if (shopPanel != null)
        {
            bool isOpening = !shopPanel.activeSelf;
            shopPanel.SetActive(isOpening);

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
        
        if (hotbarPanel != null) hotbarPanel.SetActive(isBuildModeActive);

        if (!isBuildModeActive)
        {
            currentMode = EditMode.None;
            DestroyGhost();
            if (shopPanel != null) shopPanel.SetActive(false);
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            currentMode = EditMode.Select; 
        }
    }

    void HandleMachineInteraction()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, machineLayer))
        {
            GameObject targetMachine = hit.transform.gameObject;

            switch (currentMode)
            {
                case EditMode.Delete:
                    // Find the topmost root parent object so we don't just delete a single sub-mesh!
                    GameObject objectToDestroy = targetMachine;
                    
                    // If this object is part of a parent group, target the parent instead
                    if (targetMachine.transform.parent != null)
                    {
                        objectToDestroy = targetMachine.transform.root.gameObject; 
                        // Note: transform.root goes to the absolute top. 
                        // If your machines are grouped under an empty 'Map' folder, use this instead:
                        // objectToDestroy = targetMachine.transform.parent.gameObject;
                    }

                    // Vaporize the whole machine group and refund half cash!
                    if (potatoCollector != null) potatoCollector.SpendCash(-currentItemCost / 2); 
                    
                    Destroy(objectToDestroy);
                    break;

                case EditMode.Rotate:
                    targetMachine.transform.Rotate(0, 90, 0);
                    break;

                case EditMode.Select:
                    selectedObject = targetMachine;
                    Debug.Log("Selected: " + selectedObject.name);
                    break;
            }
        }
    }

    void SpawnGhost()
    {
        if (ghostPrefab != null)
        {
            currentGhost = Instantiate(ghostPrefab);
            ghostRenderers = currentGhost.GetComponentsInChildren<Renderer>();
        }
    }

    void DestroyGhost()
    {
        if (currentGhost != null) Destroy(currentGhost);
    }

    void UpdateGhostPosition()
    {
        // Viewport center raycast handles locked-cursor tracking flawlessly
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, floorLayer))
        {
            float snappedX = Mathf.Round(hit.point.x / gridSize) * gridSize;
            float snappedZ = Mathf.Round(hit.point.z / gridSize) * gridSize;
            
            // Placed squarely on the ground hit coordinate
            // Change this line at the bottom of UpdateGhostPosition():
            currentGhost.transform.position = new Vector3(snappedX, hit.point.y + 0.2f, snappedZ);
        }
    }

    void CheckPlacementValidity()
    {
        if (currentGhost == null) return;
        
        Vector3 boxHalfExtents = new Vector3(gridSize * 0.45f, 0.5f, gridSize * 0.45f);
        bool isColliding = Physics.CheckBox(currentGhost.transform.position + Vector3.up, boxHalfExtents, currentGhost.transform.rotation, obstacleLayer);

        bool hasEnoughMoney = potatoCollector != null && potatoCollector.CanAfford(currentItemCost);
        isValidPlacement = !isColliding && hasEnoughMoney;

        // Forces a clean material recalculation on every frame
        Material targetMat = isValidPlacement ? validGhostMaterial : invalidGhostMaterial;
        foreach (Renderer r in ghostRenderers)
        {
            if (r != null) r.sharedMaterial = targetMat; 
        }
    }

    void PlaceObject()
    {
        if (potatoCollector != null) potatoCollector.SpendCash(currentItemCost);
        
        GameObject spawned = Instantiate(objectToBuild, currentGhost.transform.position, currentGhost.transform.rotation);
        spawned.layer = (int)Mathf.Log(machineLayer.value, 2);
    }
}