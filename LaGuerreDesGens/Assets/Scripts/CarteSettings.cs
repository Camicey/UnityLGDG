using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Carte", menuName = "Carte")]
public class CarteSettings : ScriptableObject
{
    public int Id;
    public string Prenom;
    public Sprite Image;
    public float PM;
    public int PV;
    public int PVar;
    public int PA;
    public int IdPouvoir;
    public string Pouvoir;
    public float CoutPouvoir;
    public string Famille;
    public Sprite FamilleImage;
    public string Type;
    public Sprite TypeImage;
    public List<CarteSettings> liens = new List<CarteSettings>();

}