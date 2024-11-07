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
        StopPlacement(); // 셀 인디케이터와 그리드 보이기 초기화
        previewRenderer = CellIndicator.GetComponentInChildren<Renderer>();
    }
    void Update()
    {
        Vector3 mousePosition = InputManagerObj.GetSelectedMapPosition(); // 이 마우스 포즈는 절대 내 영역 밖을 못나간다.
        Vector3Int gridPosition = Grid.WorldToCell(mousePosition);

        previewRenderer.material.color = CanPlaceCardTo(gridPosition) ? Color.green : Color.red; // 셀 인디케이터의 색깔

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
        if (GameManager.Instance.phase != GameManager.Phase.Preparation) // 준비단계가 아니면 즉시 리턴;
        {
            StopPlacement();
            return;
        }
        selectedCardIndex = DataBase.CardDataList.FindIndex(CardData => CardData.Id == Id); // Id로 낼 카드의 인덱스 서칭

        if (selectedCardIndex < 0)
        {
            Debug.Log("NoFound");
            return;
        }

        GridVisualization.SetActive(true);
        CellIndicator.SetActive(true);
        InputManagerObj.OnClicked += PlaceCard; // 마우스 클릭에 카드를 놓을 수 있는 기능 부여
        InputManagerObj.OnExit += StopPlacement;
    }
    void PlaceCard() // 마우스를 전장에 클릭할 때, 영체가 배치되도록 하는 메서드. OnClicked가 작동하면 같이 작동하는 함수.
    {
        if (!InputManagerObj.IsPointerOverField()) // 이상한 곳에 배치하려 하면 리턴
        {
            StopPlacement();
            return;
        }
        if (GameManager.Instance.phase != GameManager.Phase.Preparation) // 준비단계가 아니면 즉시 리턴;
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
        go.transform.GetChild(0).tag = "Ally"; // 태그 설정
        go.transform.GetChild(0).GetComponent<AstralBody>().TargetTag = "Enemy";
        GameManager.Instance.PlayerAstralBody.Add(go.transform.GetChild(0).gameObject); // 게임 메니저에 생성한 게임오브젝트를 기록.
        go.transform.position = Grid.CellToWorld(gridPosition);
        AddCardTo(gridPosition); // 배치한 그리드 표시
        GameManager.Instance.CurrentEssence -= DataBase.CardDataList[selectedCardIndex].Cost; // 플레이어 코스트 차감
        Player.Instance.CurrentEssence -= DataBase.CardDataList[selectedCardIndex].Cost;
        CardManager.Instance.inventoryCount--; // 목록 개수 줄이기.
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
        if (GameManager.Instance.phase != GameManager.Phase.Preparation) // 준비단계가 아니면 즉시 리턴;
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

        if (!CanPlaceCardTo(gridPosition)) // 배치할 수 없다면 원래자리로 돌려보내고 재배치 끝내고 리턴
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
        grapedCard = null; // 필요없는건 초기화해주는게 맞다.
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