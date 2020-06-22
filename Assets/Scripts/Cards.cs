using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Card", menuName ="Card")]
public class Cards : ScriptableObject
{
    public string Cardname;
    public int hp;
    public string[] Strength;
    public string[] weakness;
    public Sprite Artwork;

}
