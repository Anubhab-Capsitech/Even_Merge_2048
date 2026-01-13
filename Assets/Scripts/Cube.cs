using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Cube : MonoBehaviour
{
    private static int staticID = 0;

    [SerializeField] private TMP_Text[] numbersText;

    private MeshRenderer cubeMeshRenderer;
    private TrailRenderer trailRenderer;

    [HideInInspector] public int CubeID;
    [HideInInspector] public Color CubeColor;
    [HideInInspector] public int CubeNumber;
    [HideInInspector] public Rigidbody CubeRigidbody;
    [HideInInspector] public bool IsMainCube;
    [HideInInspector] public bool HasBeenLaunched = false;


    private void Awake()
    {
        CubeID = staticID++;

        cubeMeshRenderer = GetComponent<MeshRenderer>();
        CubeRigidbody = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();

        if (trailRenderer != null)
        {
            trailRenderer.enabled = false;
        }

        if (numbersText == null || numbersText.Length == 0)
        {
            Debug.LogWarning("Cube {CubeID}: NumbersText array is not assigned ", this);
        }
    }

    private void OnDisable()
    {
        DisableTrail();
    }

    private void OnDestroy()
    {
        trailRenderer = null; 
    }

    #region Color Management
    public void SetColor(Color color)
    {
        CubeColor = color;

        if (cubeMeshRenderer != null)
        {
            MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
            cubeMeshRenderer.GetPropertyBlock(propBlock);
            propBlock.SetColor("_Color", color);
            cubeMeshRenderer.SetPropertyBlock(propBlock);
        }
        else
        {
            Debug.LogWarning("Cube {CubeID}: MeshRenderer not found ", this);
        }

        SetupTrailColor(color);
    }

    private void SetupTrailColor(Color color)
    {
        if (trailRenderer == null) return;

        trailRenderer.startColor = color;
        trailRenderer.endColor = new Color(color.r, color.g, color.b, 0f);

        if (trailRenderer.material != null)
        {
            trailRenderer.material.color = color;
        }
    }
    #endregion

    #region Number Management
    public void SetNumber(int number)
    {
        CubeNumber = number;
        string numberString = number.ToString();

        if (numbersText != null && numbersText.Length > 0)
        {
            foreach (var text in numbersText)
            {
                if (text != null)
                    text.text = numberString;
            }
        }
        else
        {
            Debug.LogWarning("Cube {CubeID}: NumbersText array is empty or null!", this);
        }
    }

    public string GetNumberText() => CubeNumber.ToString();
    #endregion

    #region Trail Management
    public void EnableTrail()
    {
        if (trailRenderer == null) return;

        try
        {
            trailRenderer.enabled = true;
            trailRenderer.Clear();
        }
        catch (MissingReferenceException) { }
    }

    public void DisableTrail()
    {
        if (trailRenderer == null) return;

        try
        {
            trailRenderer.enabled = false;
            trailRenderer.Clear();
        }
        catch (MissingReferenceException) { }
    }

    public void SetTrailWidth(float width)
    {
        if (trailRenderer == null) return;

        try
        {
            trailRenderer.startWidth = width;
            trailRenderer.endWidth = width * 0.5f;
        }
        catch (MissingReferenceException) { }
    }
    #endregion

    #region Cube State Management
    public void ResetCube()
    {
        if (CubeRigidbody != null)
        {
            CubeRigidbody.linearVelocity = Vector3.zero;
            CubeRigidbody.angularVelocity = Vector3.zero;
        }

        transform.rotation = Quaternion.identity;
        DisableTrail();

        HasBeenLaunched = false;
        IsMainCube = false;
    }

    public void MakeKinematic(bool kinematic)
    {
        if (CubeRigidbody != null)
        {
            CubeRigidbody.isKinematic = kinematic;
        }
    }
    #endregion

    #region Public Getters
    public bool IsMoving()
    {
        if (CubeRigidbody == null) return false;
        return CubeRigidbody.linearVelocity.magnitude > 0.1f;
    }

    public bool IsStationary()
    {
        if (CubeRigidbody == null) return true;
        return CubeRigidbody.linearVelocity.magnitude < 0.1f;
    }

    public Vector3 GetVelocity()
    {
        return CubeRigidbody != null ? CubeRigidbody.linearVelocity : Vector3.zero;
    }
    #endregion
}
