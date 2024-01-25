using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public List<Carte> Pioche = new List<Carte>();
    public PlaceDeck PlacePioche;
    public Joueur J1;
    public Joueur J2;
    public Joueur JoueurActif;
    public Joueur JoueurPassif { get; set; }
    public int Tour { get; set; }
    public List<PlaceDeck> SlotsCartes = new List<PlaceDeck>();
    public List<CarteSettings> CartesMontrable = new List<CarteSettings>();
    public GrosseCarte grosseCarte;
    public bool CarteMontreeEnCours;

    //public List<BoutonCache> BoutonsDerriere = new List<BoutonCache>();


    void Start()
    {
        JoueurActif = J1;
        JoueurPassif = J2;
        Tour = 0;
        CarteMontreeEnCours = false;
        //SlotsCartes = JoueurActif.PlaceDeck;
        foreach (Carte carte in Pioche)
        {
            carte.PlaceOccupee = PlacePioche;
        }
    }


    public void Piocher()
    {
        if (Pioche.Count >= 1)
        {
            Carte randCarte = Pioche[Random.Range(0, Pioche.Count)];

            foreach (PlaceDeck slot in SlotsCartes)
            {
                if (slot.availableCarteSlots == true)
                {
                    randCarte.gameObject.SetActive(true);
                    randCarte.rectTransform.anchoredPosition = slot.rectTransform.anchoredPosition;
                    slot.availableCarteSlots = false;
                    randCarte.PositionBase = slot.rectTransform.anchoredPosition;
                    randCarte.PlaceOccupee = slot;
                    Pioche.Remove(randCarte);
                    JoueurActif.CartesPossedees.Add(randCarte);
                    return;
                }
            }
        }
    }

    public void ChangerDeTour()
    {

        Debug.Log("On change de tour");
        Debug.Log(Tour);

        /*
        foreach (PlaceDeck slot in carteSlots)
        {
            Vector2 descendre = new Vector2(0, 500);
            // this.gameObject.transform.Translate(90, 180, 0f); //Ne marche surement
            slot.rectTransform.anchoredPosition = slot.rectTransform.anchoredPosition - descendre;
        }
        */

        if (Tour % 2 == 0)
        {
            JoueurActif = J2;
            JoueurPassif = J1;
        }
        else
        {
            JoueurActif = J1;
            JoueurPassif = J2;
        }
        Tour++;
        //carteSlots = JoueurActif.placeDeck;
        /*
        foreach (Deck slot in carteSlots)
        {
            Vector2 monter = new Vector2(0, 500);
            slot.rectTransform.anchoredPosition = slot.rectTransform.anchoredPosition + monter;
        }
        */
    }

}
