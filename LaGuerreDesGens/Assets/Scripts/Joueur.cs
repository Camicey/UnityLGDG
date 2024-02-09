using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joueur : MonoBehaviour
{
    public int Id;
    public string Prenom;
    public string Email;
    public List<Carte> CartesPossedees = new List<Carte>();
    public List<PlaceDeck> Deck = new List<PlaceDeck>();
    public List<PlaceTerrain> Terrain = new List<PlaceTerrain>();
    public bool APioche;

    public Carte TrouverStratege()
    {
        foreach (Carte carte in CartesPossedees)
        { if (carte.Stratege == true) { return carte; } }
        return null;
    }
    public void Start()
    {
        CartesPossedees.Clear();
        APioche = false;
    }
}

