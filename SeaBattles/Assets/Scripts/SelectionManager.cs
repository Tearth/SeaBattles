﻿using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;

public class SelectionManager : MonoBehaviour
{
    public Camera Camera;
    public Transform Ships;
    public GameObject SelectPrefab;
    public Transform Selections;
    public int SelectionThickness;
    public Color SelectionBckgColor;
    public Color SelectionBorderColor;

    private GameObject _preSelect;
    private Vector3 _mouseClickPoint;
    private Texture2D _texture;

    // Start is called before the first frame update
    void Start()
    {
        _preSelect = Instantiate(SelectPrefab, Vector3.zero, Quaternion.identity, transform);
        _preSelect.GetComponent<SelectIndicatorEntity>().SetAsPreselect();
        _preSelect.SetActive(false);

        _texture = new Texture2D(1, 1);
        _texture.SetPixel(0, 0, Color.white);
        _texture.Apply();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Cursor.visible)
        {
            return; 
        }

        if (Input.GetMouseButtonDown(0))
        {
            _mouseClickPoint = Input.mousePosition;
        }

        if (Physics.Raycast(Camera.ScreenPointToRay(Input.mousePosition), out var hit, float.MaxValue))
        {
            var selectable = hit.collider.gameObject.GetComponent<ISelectable>();
            if (selectable != null)
            {
                if (!selectable.Selected)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        SelectTarget(selectable, hit.collider.transform);
                        HidePreselect();
                    }
                    else
                    {
                        ShowPreselect(hit.collider.transform);
                    }
                }
            }
            else
            {
                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    RemoveAllSelections();
                }

                HidePreselect();
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftShift))
            {
                RemoveAllSelections();
            }

            HidePreselect();
        }
    }

    void OnGUI()
    {
        if (Input.GetMouseButton(0))
        {
            var minX = Mathf.Min(Input.mousePosition.x, _mouseClickPoint.x);
            var maxX = Mathf.Max(Input.mousePosition.x, _mouseClickPoint.x);
            var minY = Mathf.Min(Input.mousePosition.y, _mouseClickPoint.y);
            var maxY = Mathf.Max(Input.mousePosition.y, _mouseClickPoint.y);

            var rect = Rect.MinMaxRect(minX, minY, maxX, maxY);

            // Background
            DrawRectangle(rect.position, new Vector2(rect.size.x, -rect.size.y), SelectionBckgColor);

            // Top
            DrawRectangle(rect.position, new Vector2(rect.width, SelectionThickness), SelectionBorderColor);

            // Bottom
            DrawRectangle(rect.position + new Vector2(0, rect.height + SelectionThickness), new Vector2(rect.width, SelectionThickness), SelectionBorderColor);

            // Left
            DrawRectangle(rect.position, new Vector2(SelectionThickness, -rect.height), SelectionBorderColor);

            // Right
            DrawRectangle(rect.position + new Vector2(rect.width - SelectionThickness, 0), new Vector2(SelectionThickness, -rect.height), SelectionBorderColor);

            foreach (Transform ship in Ships)
            {
                var selectable = ship.GetComponent<ISelectable>();
                if (rect.Contains(Camera.WorldToScreenPoint(ship.position)))
                {
                    if (!selectable.Selected)
                    {
                        SelectTarget(selectable, ship.transform);
                    }
                }
                else
                {
                    if (selectable.Selected)
                    {
                        UnselectTarget(selectable, ship.transform);
                    }
                }
            }
        }
    }

    private void DrawRectangle(Vector2 position, Vector2 size, Color color)
    {
        var invertedPosition = new Vector3(position.x, Camera.main.pixelHeight - position.y);

        GUI.color = color;
        GUI.DrawTexture(new Rect(invertedPosition, size), _texture);
        GUI.color = Color.white;
    }

    private void SelectTarget(ISelectable selectable, Transform target)
    {
        var createdSelection = Instantiate(SelectPrefab, Vector3.zero, Quaternion.identity, Selections);
        createdSelection.GetComponent<SelectIndicatorEntity>().Target = target;
        createdSelection.GetComponent<SelectIndicatorEntity>().ForceUpdatePosition();
        selectable.Selected = true;
    }

    private void UnselectTarget(ISelectable selectable, Transform target)
    {
        target.GetComponent<ISelectable>().Selected = false;
        foreach (Transform selection in Selections)
        {
            if (selection.GetComponent<SelectIndicatorEntity>().Target == target)
            {
                Destroy(selection.gameObject);
            }
        }
    }

    private void RemoveAllSelections()
    {
        foreach (Transform child in Selections)
        {
            var target = child.GetComponent<SelectIndicatorEntity>().Target;
            target.GetComponent<ISelectable>().Selected = false;

            Destroy(child.gameObject);
        }
    }

    private void ShowPreselect(Transform target)
    {
        _preSelect.SetActive(true);
        _preSelect.GetComponent<SelectIndicatorEntity>().Target = target;
        _preSelect.GetComponent<SelectIndicatorEntity>().ForceUpdatePosition();
    }

    private void HidePreselect()
    {
        _preSelect.SetActive(false);
    }
}
