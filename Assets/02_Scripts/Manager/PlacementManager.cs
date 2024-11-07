using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class PlacementManager : MonoBehaviour
{
    private static PlacementManager instance;
    public static PlacementManager Instance
    {
        get
        {
            return instance;
        }
        private set
        {
            instance = value;
        }
    }
    public GameObject MouseIndicator, CellIndicator;
    public InputManager InputManagerObj;
    public Grid Grid;
    public CopyDataBase DataBase;
    public GameObject GridVisualization;
    public SetCardInfoManager CurrentSelectedCardPrefab;
    public List<Vector3Int> isCellFilled = new List<Vector3Int>();
    public Stack<GameObject> AliveAllyAstralBody = new Stack<GameObject>();
    public Stack<GameObject> AliveEnemyAstralBody = new Stack<GameObject>();

    int selectedCardIndex = -1;
    GameObject grapedCard = null;
    GameObject grapedCardChild = null;
    Vector3Int originPosition = default;
    Renderer previewRenderer;
    
    private void Awake()
    {
        instance = this;

    }
    private void Start()
    {
        StopPlacement(); // �� �ε������Ϳ� �׸��� ���̱� �ʱ�ȭ
        previewRenderer = CellIndicator.GetComponentInChildren<Renderer>();
    }
    void Update()
    {
        Vector3 mousePosition = InputManagerObj.GetSelectedMapPosition(); // �� ���콺 ����� ���� �� ���� ���� ��������.
        Vector3Int gridPosition = Grid.WorldToCell(mousePosition);

        previewRenderer.material.color = CanPlaceCardTo(gridPosition) ? Color.green : Color.red; // �� �ε��������� ����

        MouseIndicator.transform.position = mousePosition;
        CellIndicator.transform.position = gridPosition; //Grid.CellToWorld(gridPosition);

        if (GameManager.Instance.phase == GameManager.Phase.WaitingBeforePreparation)
        {
            isCellFilled.Clear();
        }

        if (GameManager.Instance.phase == GameManager.Phase.Preparation && AliveAllyAstralBody.Count != 0)
        {
            GameObject go = AliveAllyAstralBody.Pop();
            Vector3Int grid = new Vector3Int(-5, 0, -5);
            System.Random random = new System.Random();
            while (isCellFilled.Contains(grid))
            {
                int randomCol = random.Next(-5, -1);
                int randomRow = random.Next(-5, 5);
                grid = new Vector3Int(randomRow, 0, randomCol);
            }
            go.transform.position = grid;
            go.transform.GetChild(0).localPosition = new Vector3(0.5f, 0, 0.5f);
            go.transform.GetChild(0).eulerAngles = new Vector3(0, 0, 0);
            isCellFilled.Add(grid);
            go = null;
        }
        if (GameManager.Instance.phase == GameManager.Phase.Preparation && AliveEnemyAstralBody.Count != 0)
        {
            GameObject go = AliveEnemyAstralBody.Pop();
            Vector3Int grid = new Vector3Int(-5, 0, 4);
            System.Random random = new System.Random();
            while (Opponent.Instance.FilledCell.Contains(grid))
            {
                int randomCol = random.Next(1, 5);
                int randomRow = random.Next(-5, 5);
                grid = new Vector3Int(randomRow, 0, randomCol);
            }
            go.transform.position = grid;
            go.transform.GetChild(0).localPosition = new Vector3(0.5f, 0, 0.5f);
            go.transform.GetChild(0).eulerAngles = new Vector3(0, 180, 0);
            Opponent.Instance.FilledCell.Add(grid);
            go = null;
        }
    }

    public void StartPlacement(int Id)
    {
        if (GameManager.Instance.phase != GameManager.Phase.Preparation) // �غ�ܰ谡 �ƴϸ� ��� ����;
        {
            StopPlacement();
            return;
        }
        selectedCardIndex = DataBase.CardDataList.FindIndex(CardData => CardData.Id == Id); // Id�� �� ī���� �ε��� ��Ī

        if (selectedCardIndex < 0)
        {
            Debug.Log("NoFound");
            return;
        }

        GridVisualization.SetActive(true);
        CellIndicator.SetActive(true);
        InputManagerObj.OnClicked += PlaceCard; // ���콺 Ŭ���� ī�带 ���� �� �ִ� ��� �ο�
        InputManagerObj.OnExit += StopPlacement;
    }
    void PlaceCard() // ���콺�� ���忡 Ŭ���� ��, ��ü�� ��ġ�ǵ��� �ϴ� �޼���. OnClicked�� �۵��ϸ� ���� �۵��ϴ� �Լ�.
    {
        if (!InputManagerObj.IsPointerOverField()) // �̻��� ���� ��ġ�Ϸ� �ϸ� ����
        {
            StopPlacement();
            return;
        }
        if (GameManager.Instance.phase != GameManager.Phase.Preparation) // �غ�ܰ谡 �ƴϸ� ��� ����;
        {
            StopPlacement();
            return;
        }
        Vector3 mousePosition = InputManagerObj.GetSelectedMapPosition();
        Vector3Int gridPosition = Grid.WorldToCell(mousePosition);

        if (!CanPlaceCardTo(gridPosition))
        {
            return;
        }

        GameObject go = Instantiate(DataBase.CardDataList[selectedCardIndex].Prefab);
        go.transform.GetChild(0).tag = "Ally"; // �±� ����
        go.transform.GetChild(0).GetComponent<AstralBody>().TargetTag = "Enemy";
        GameManager.Instance.PlayerAstralBody.Add(go.transform.GetChild(0).gameObject); // ���� �޴����� ������ ���ӿ�����Ʈ�� ���.
        go.transform.position = Grid.CellToWorld(gridPosition);
        AddCardTo(gridPosition); // ��ġ�� �׸��� ǥ��
        GameManager.Instance.CurrentEssence -= DataBase.CardDataList[selectedCardIndex].Cost; // �÷��̾� �ڽ�Ʈ ����
        Player.Instance.CurrentEssence -= DataBase.CardDataList[selectedCardIndex].Cost;
        CardManager.Instance.inventoryCount--; // ��� ���� ���̱�.
        CurrentSelectedCardPrefab.DestroyThis();
        StopPlacement();
    }
    void StopPlacement()
    {
        selectedCardIndex = -1;
        CurrentSelectedCardPrefab = null;
        GridVisualization.SetActive(false);
        CellIndicator.SetActive(false);
        InputManagerObj.OnClicked -= PlaceCard;
        InputManagerObj.OnExit -= StopPlacement;
    }

    public void StartRePlacement(GameObject obj)
    {
        if (GameManager.Instance.phase != GameManager.Phase.Preparation) // �غ�ܰ谡 �ƴϸ� ��� ����;
        {
            StopRePlacement();
            return;
        }
        grapedCard = obj.transform.parent?.gameObject;
        grapedCardChild = obj;
        grapedCardChild.GetComponent<NavMeshAgent>().enabled = false;

        Vector3 mousePosition = InputManagerObj.GetSelectedMapPosition();
        Vector3Int gridPosition = Grid.WorldToCell(mousePosition);
        RemoveCardPosition(gridPosition);
        originPosition = gridPosition;

        GridVisualization.SetActive(true);
        CellIndicator.SetActive(true);
        InputManagerObj.OnStay += GrapCard;
        InputManagerObj.OnRelease += RePlaceCard;
    }
    void GrapCard()
    {
        grapedCard.transform.position = MouseIndicator.transform.position + new Vector3(0, 0.5f, 0);
        if (GameManager.Instance.phase != GameManager.Phase.Preparation)
        {
            grapedCard.transform.position = originPosition;
            StopRePlacement();
        }
    }
    void RePlaceCard()
    {
        if (!InputManagerObj.IsPointerOverField())
        {
            grapedCard.transform.position = originPosition;
            StopRePlacement();
            return;
        }
        Vector3 mousePosition = InputManagerObj.GetSelectedMapPosition();
        Vector3Int gridPosition = Grid.WorldToCell(mousePosition);

        if (!CanPlaceCardTo(gridPosition)) // ��ġ�� �� ���ٸ� �����ڸ��� ���������� ���ġ ������ ����
        {
            grapedCard.transform.position = originPosition;
            StopRePlacement();
            return;
        }

        grapedCard.transform.position = gridPosition;
        AddCardTo(gridPosition);

        StopRePlacement();
    }
    void StopRePlacement()
    {
        grapedCardChild.GetComponent<NavMeshAgent>().enabled = true;
        grapedCard = null; // �ʿ���°� �ʱ�ȭ���ִ°� �´�.
        grapedCardChild = null;

        GridVisualization.SetActive(false);
        CellIndicator.SetActive(false);
        InputManagerObj.OnStay -= GrapCard;
        InputManagerObj.OnRelease -= RePlaceCard;
        InputManagerObj.ClearOnStay();
    }

    public void AddCardTo(Vector3Int gridPosition)
    {
        if (!isCellFilled.Contains(gridPosition))
        {
            isCellFilled.Add(gridPosition);
        }
    }
    public void RemoveCardPosition(Vector3Int gridPosition)
    {
        isCellFilled.Remove(gridPosition);
    }
    public bool CanPlaceCardTo(Vector3Int gridPosition)
    {
        if (isCellFilled.Contains(gridPosition))
        {
            return false;
        }
        return true;
    }

}