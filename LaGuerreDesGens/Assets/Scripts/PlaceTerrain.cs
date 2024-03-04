using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class PlaceTerrain : MonoBehaviour, IDropHandler, IPointerDownHandler
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
    }

    public void OnDrop(PointerEventData eventData) //Quand une carte est lâchée sur le terrain
    {
        if (eventData.pointerDrag != null &&
        Appartenance == JeuEnCours.JoueurActif &&
        eventData.pointerDrag.GetComponent<Carte>().EstEnJeu == false &&
        eventData.pointerDrag.GetComponent<Carte>().Appartenance == JeuEnCours.JoueurActif &&
        JeuEnCours.PMEnCours >= 1)
        {
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
            eventData.pointerDrag.GetComponent<Carte>().EstEnJeu = true; //Si je pose la carte sur la place, on pose un true
            CartePlacee = eventData.pointerDrag.GetComponent<Carte>();
            eventData.pointerDrag.GetComponent<Carte>().PlaceDeTerrain = this;
            if (JeuEnCours.JoueurActif.Terrain[0] != null && this == JeuEnCours.JoueurActif.Terrain[0]) { CartePlacee.EstCachee = false; }
            else { CartePlacee.EstCachee = true; }
            CartePlacee.rectTransform.sizeDelta = GetComponent<RectTransform>().sizeDelta; //La carte se met à la taille de la case
            JeuEnCours.PMEnCours -= 1;
            JeuEnCours.AfficherPM();
        }
        else
        {
            Debug.Log(eventData.pointerDrag + (Appartenance == JeuEnCours.JoueurActif).ToString() +
            eventData.pointerDrag.GetComponent<Carte>().EstEnJeu +
            (eventData.pointerDrag.GetComponent<Carte>().Appartenance == JeuEnCours.JoueurActif).ToString() + (JeuEnCours.PMEnCours >= 1).ToString());
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (JeuEnCours.EnTrainCibleTerrain == true) { JeuEnCours.TerrainCiblee = this; }
    }
}