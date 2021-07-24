using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup placementGroup;
    [SerializeField]
    private GameObject placeButtonObject;
    [SerializeField]
    private GameObject cancelButtonObject;

    private enum PlacementStatus
    {
        idle,       // placeButton icon, placementGroup hidden
        choosing,   // cancelButton icon, placementGroup showing
        placing     // cancelButton icon, placementGroup hidden
    }

    private PlacementStatus currentStatus;

    private void Start()
    {
        cancelButtonObject.SetActive(false);
        currentStatus = PlacementStatus.idle;
    }

    public void ShowHideButton()
    {
        switch (currentStatus)
        {
            case PlacementStatus.idle:
            case PlacementStatus.placing:
                ShowPlacement();
                break;
            case PlacementStatus.choosing:
                HidePlacement();
                break;
            default:
                break;
        }
    }

    public void ShowPlacement()
    {
        currentStatus = PlacementStatus.choosing;
        cancelButtonObject.SetActive(true);
        placeButtonObject.SetActive(false);
        placementGroup.alpha = 1f;
        placementGroup.interactable = true;
    }

    public void DuringPlacement()
    {
        currentStatus = PlacementStatus.placing;
        cancelButtonObject.SetActive(true);
        placeButtonObject.SetActive(false);
        placementGroup.alpha = 0f;
        placementGroup.interactable = false;
    }

    public void HidePlacement()
    {
        currentStatus = PlacementStatus.idle;
        cancelButtonObject.SetActive(false);
        placeButtonObject.SetActive(true);
        placementGroup.alpha = 0f;
        placementGroup.interactable = false;
    }
}
