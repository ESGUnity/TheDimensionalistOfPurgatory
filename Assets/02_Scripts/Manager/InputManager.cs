using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public Camera PlayerCamera;
    public LayerMask placementLayerMask;
    public LayerMask astralBodyLayerMask;
    public GameObject CardPrefab;
    public CopyDataBase DataBase;

    Vector3 lastPosition; // 지정한 공간(전장)에서의 마지막 마우스 커서 위치
    GameObject SelectedAstralBody = null; // 이미 전장에 배치한 영체를 옮길 때 사용할 오브젝트

    public event Action OnClicked, OnStay, OnRelease, OnExit;

    private void Update()
    {
        GetSelectedAstralBody();
        if (SelectedAstralBody != null)
        {
            CardPrefab.SetActive(true);
            //CardPrefab.GetComponentInChildren<Button>().enabled = false;
            int Id = SelectedAstralBody.GetComponent<AstralBody>().Id;
            CardData card = DataBase.CardDataList[DataBase.CardDataList.FindIndex(CardData => CardData.Id == Id)];
            CardPrefab.GetComponent<SetCardInfoManager>().cardData = card;
            CardPrefab.GetComponent<SetCardInfoManager>().SetCardInfoAndGenerate();
        }
        else
        {
            CardPrefab.SetActive(false);
        }

        if (Input.GetMouseButtonDown(0))
        {
            OnClicked?.Invoke();
            if (SelectedAstralBody != null && SelectedAstralBody.tag == "Ally" && GameManager.Instance.phase == GameManager.Phase.Preparation)
            {
                PlacementManager.Instance.StartRePlacement(SelectedAstralBody);
            }
        }
        if (Input.GetMouseButton(0))
        {
            OnStay?.Invoke();
        }
        if (Input.GetMouseButtonUp(0))
        {
            OnRelease?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnExit?.Invoke();
        }
    }
    public void ClearOnStay() // 모든 OnStay에 추가된 함수 해제
    {
        OnStay = null;
    }
    public bool IsPointerOverField()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = PlayerCamera.nearClipPlane;
        Ray ray = PlayerCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, placementLayerMask))
        {
            return true;
        }
        return false;
    }

    public Vector3 GetSelectedMapPosition() // 지정한 공간 내에서(placementLayerMask) 입력만 받게 만드려는 함수
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = PlayerCamera.nearClipPlane;
        Ray ray = PlayerCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, placementLayerMask))
        {
            lastPosition = hit.point;
        }
        return lastPosition; // 지정한 공간 내의 마우스가 위치한 실제 월드 좌표를 반환한다.
    }

    public GameObject GetSelectedAstralBody() // 이거 null 안줘서 3시간 고생;
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = PlayerCamera.nearClipPlane;
        Ray ray = PlayerCamera.ScreenPointToRay(mousePos);
        
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, astralBodyLayerMask))
        {
            SelectedAstralBody = hit.collider.gameObject;
        }
        else
        {
            SelectedAstralBody = null;
        }


        return SelectedAstralBody;
    }
}
