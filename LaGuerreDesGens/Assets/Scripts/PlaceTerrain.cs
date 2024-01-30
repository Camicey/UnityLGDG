using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class PlaceTerrain : MonoBehaviour, IDropHandler
{
    public Carte CartePlacee;
    public GameManager JeuEnCours;
    public Joueur JoueurAppartient;


    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");
        if (eventData.pointerDrag != null && JoueurAppartient == JeuEnCours.JoueurActif && eventData.pointerDrag.GetComponent<Carte>().EstEnJeu == false)
        {
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
            eventData.pointerDrag.GetComponent<Carte>().EstEnJeu = true; //Si je pose la carte sur la place, on pose un true
            CartePlacee = eventData.pointerDrag.GetComponent<Carte>();
            eventData.pointerDrag.GetComponent<Carte>().PlaceTerrainOccupee = this;
        }
    }

}