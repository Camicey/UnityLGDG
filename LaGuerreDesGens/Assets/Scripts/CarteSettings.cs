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
    public int IdPouvoirVar;
    public string Pouvoir;
    public string PouvoirVar;
    public float CoutPouvoir;
    public float CoutPouvoirVar;
    public string Famille;
    public Sprite FamilleImage;
    public string Type;
    public Sprite TypeImage;
    public List<CarteSettings> liens = new List<CarteSettings>();
    public List<CarteSettings> liensvar = new List<CarteSettings>();

}