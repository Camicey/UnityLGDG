using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlaceDeck : NetworkBehaviour
{
    public int Id;
    public Joueur Appartenance;
    public bool availableCarteSlots;
    public Carte CartePlacee = null;
    public RectTransform rectTransform;

    public void Initialiser()
    {
        availableCarteSlots = true; // Ne pas enlever, bug sinon
        rectTransform = GetComponent<RectTransform>();
        CartePlacee = null;
    }

}
