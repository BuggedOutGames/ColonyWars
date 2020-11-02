using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBehaviour : MonoBehaviour, ISelectable {

    public GameObject player;
    public GameObject selectionIndicator;

    private PlayerController playerController;

    void Start() {
        playerController = player.GetComponent<PlayerController>();
    }

    void OnBecameVisible() {
        playerController.AddVisibleUnit(this);
    }

    void OnBecameInvisible() {
        playerController.RemoveVisibleUnit(this);
    }

    public bool IsSelected() {
        return selectionIndicator.activeInHierarchy;
    }

    public void SetSelected(bool isSelected) {
        selectionIndicator.SetActive(isSelected);
    }
}