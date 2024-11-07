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

    Vector3 lastPosition; // ������ ����(����)������ ������ ���콺 Ŀ�� ��ġ
    GameObject SelectedAstralBody = null; // �̹� ���忡 ��ġ�� ��ü�� �ű� �� ����� ������Ʈ

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
    public void ClearOnStay() // ��� OnStay�� �߰��� �Լ� ����
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

    public Vector3 GetSelectedMapPosition() // ������ ���� ������(placementLayerMask) �Է¸� �ް� ������� �Լ�
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = PlayerCamera.nearClipPlane;
        Ray ray = PlayerCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, placementLayerMask))
        {
            lastPosition = hit.point;
        }
        return lastPosition; // ������ ���� ���� ���콺�� ��ġ�� ���� ���� ��ǥ�� ��ȯ�Ѵ�.
    }

    public GameObject GetSelectedAstralBody() // �̰� null ���༭ 3�ð� ���;
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
