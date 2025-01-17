﻿using UnityEngine;

public class SelectIndicatorEntity : MonoBehaviour
{
    public GameObject ElementPrefab;
    public int ElementsCount;
    public float Radius;
    public float RotateSpeed;
    public Transform Target;
    
    void Awake()
    {
        var angleStep = 2 * Mathf.PI / ElementsCount;
        var currentAngle = 0f;

        for (var i = 0; i < ElementsCount; i++)
        {
            var elementPosition = new Vector3(Mathf.Sin(currentAngle), 0, Mathf.Cos(currentAngle)) * Radius;
            var elementRotation = new Vector3(0, currentAngle * 360 / (2 * Mathf.PI));
            currentAngle += angleStep;

            var element = Instantiate(ElementPrefab, elementPosition, Quaternion.identity, transform);
            element.transform.localEulerAngles = elementRotation;
        }
    }

    void Update()
    {
        ForceUpdatePosition();
    }
    
    void FixedUpdate()
    {
        transform.eulerAngles += new Vector3(0, RotateSpeed, 0);
    }

    public void ForceUpdatePosition()
    {
        if (Target != null)
        {
            transform.position = new Vector3(Target.transform.position.x, 2, Target.transform.position.z);
        }
    }

    public void SetAsPreselect()
    {
        SetTransparency(0.5f);
    }

    public void SetAsSelect()
    {
        SetTransparency(1f);
    }

    public void SetTransparency(float value)
    {
        foreach (Transform child in transform)
        {
            child.GetComponent<MeshRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, value);
        }
    }
}
