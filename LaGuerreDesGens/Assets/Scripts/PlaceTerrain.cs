using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class PlaceTerrain : MonoBehaviour, IDropHandler, IPointerDownHandler
{
    public Carte CartePlacee;
    public int Id;
    public GameManager JeuEnCours;
    public Joueur Appartenance;


    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");
        if (eventData.pointerDrag != null &&
        Appartenance == JeuEnCours.JoueurActif &&
        eventData.pointerDrag.GetComponent<Carte>().EstEnJeu == false &&
        eventData.pointerDrag.GetComponent<Carte>().Appartenance == JeuEnCours.JoueurActif &&
        JeuEnCours.PMEnCours >= 1)
        {
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
            eventData.pointerDrag.GetComponent<Carte>().EstEnJeu = true; //Si je pose la carte sur la place, on pose un true
            CartePlacee = eventData.pointerDrag.GetComponent<Carte>();
            eventData.pointerDrag.GetComponent<Carte>().PlaceTerrainOccupee = this;
            if (JeuEnCours.JoueurActif.Terrain[0] != null && this == JeuEnCours.JoueurActif.Terrain[0]) { CartePlacee.EstCachee = false; }
            else { CartePlacee.EstCachee = true; }
            JeuEnCours.PMEnCours -= 1;
            JeuEnCours.AfficherPM();
        }
        else { Debug.Log("Oh oh problem for you"); }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (JeuEnCours.EstEnTrainDeSeDeplacer == true && CartePlacee == null && JeuEnCours.VerifierTerrainACote(this))
        {
            JeuEnCours.TerrainCiblee = this;
        }
        else if (JeuEnCours.EstEnTrainDeSeDeplacer == true && CartePlacee != null)
        { JeuEnCours.Warning("Il y a déjà quelqu'un ici !"); }
        else if (!JeuEnCours.VerifierTerrainACote(this)) { JeuEnCours.Warning("Ce terrain est trop loin !"); }
    }

}