using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Mirror;

public class PlaceTerrain : NetworkBehaviour, IDropHandler, IPointerDownHandler
{
    public int Id;
    public Carte CartePlacee;
    public GameManager JeuEnCours;
    public Joueur Appartenance;

    public void Start() { CartePlacee = null; }

    public void Initialiser()
    {
        CartePlacee = null;
        if (JeuEnCours.TypePartie == "Longue")
        { GetComponent<RectTransform>().sizeDelta = new Vector2(168, 258); }
        else
        { GetComponent<RectTransform>().sizeDelta = new Vector2(224, 344); }
    }

    public void OnDrop(PointerEventData eventData) //Quand une carte est lâchée sur le terrain
    {
        Carte objetDeplace = eventData.pointerDrag.GetComponent<Carte>(); //Rend le code plus lisible
        if (eventData.pointerDrag != null && Appartenance == JeuEnCours.JoueurActif &&
        objetDeplace.EstEnJeu == false && objetDeplace.Appartenance == JeuEnCours.JoueurActif && JeuEnCours.PMEnCours >= 1)
        {
            objetDeplace.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
            objetDeplace.EstEnJeu = true; //Si je pose la carte sur la place, on pose un true
            CartePlacee = objetDeplace;
            objetDeplace.PlaceDeTerrain = this;
            if (JeuEnCours.JoueurActif.Terrain[0] != null && this == JeuEnCours.JoueurActif.Terrain[0])
            { CartePlacee.EstCachee = false; }
            else { CartePlacee.EstCachee = true; }
            if (JeuEnCours.TypePartie == "Longue") // La carte se met à la taille de la case
            { CartePlacee.gameObject.transform.localScale = Vector3.one * 0.75f; }
            JeuEnCours.PMEnCours -= 1;
            JeuEnCours.AfficherPM();
        }
    }
    public void OnPointerDown(PointerEventData eventData) //Quand je clique dessus
    {
        if (JeuEnCours.EnTrainCibleTerrain == true) { JeuEnCours.TerrainCiblee = this; }
    }
}