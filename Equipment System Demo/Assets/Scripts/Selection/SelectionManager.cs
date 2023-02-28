using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{

    private ISelectable currentSelection;

    private ISelectionResponse selectionResponse;
    private IRayProvider rayProvider;
    private ISelectionDetermination selector;

    private void Awake()
    {
        selectionResponse = GetComponent<ISelectionResponse>();
        rayProvider = GetComponent<IRayProvider>();
        selector = GetComponent<ISelectionDetermination>();
    }
    // Update is called once per frame
    private void Update()
    {
        selectionResponse.OnDeselect(currentSelection);

        selector.CheckSelection(rayProvider.CreateRay());
        currentSelection = selector.GetSelection();

        selectionResponse.OnSelect(currentSelection);

    }
}
