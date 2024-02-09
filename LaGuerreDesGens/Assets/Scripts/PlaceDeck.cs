using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceDeck : MonoBehaviour
{
    public GameManager JeuEnCours;
    public Joueur Appartenance;
    public bool availableCarteSlots;
    public Carte CartePlacee = null;
    public RectTransform rectTransform;


    public void Start()
    {
        availableCarteSlots = true; //Ne pas enlever, bug sinon
        rectTransform = GetComponent<RectTransform>();
        CartePlacee = null;
    }

}
