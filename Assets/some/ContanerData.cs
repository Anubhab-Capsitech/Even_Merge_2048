using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "ContanerData", menuName = "ScriptableObjects/ContanerData", order = 1)]
public class ContanerData : ScriptableObject
{
    public List<ColorData> colorList;
}

[System.Serializable]
public class ColorData
{
    public Color wallColor;
    public Color groundColor;
    public Sprite newSprite;
    public Sprite buttonimg;
    public Sprite changebuttonimg;
    public Sprite[] scorePanal;
}
