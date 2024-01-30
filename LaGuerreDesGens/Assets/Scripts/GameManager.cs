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
    public Joueur JoueurPassif;
    public int Tour { get; set; }
    public List<PlaceDeck> SlotsCartes = new List<PlaceDeck>();
    public List<CarteSettings> CartesMontrable = new List<CarteSettings>();
    public GrosseCarte grosseCarte;
    public bool CarteMontreeEnCours = false;
    public bool EstEnTrainDeChoisirStratège = false;

    //public List<BoutonCache> BoutonsDerriere = new List<BoutonCache>();


    void Start()
    {
        //Demander les infos des joueurs
        //Demander quelles familles ils veulent jouer
        NouvellePartie(); //Normalement on aura des choix donc il changera de place
    }

    protected void NouvellePartie() // On récupèrera les joueurs et familles
    {
        JoueurActif = J1; //Ce sera une création de joueur à la place
        JoueurPassif = J2;
        Tour = 1;
        foreach (PlaceDeck slot in JoueurActif.Deck) //On cache le deck qu'on a
        {
            slot.gameObject.SetActive(true);
        }
        foreach (PlaceDeck slot in JoueurPassif.Deck) //On cache le deck qu'on a
        {
            slot.gameObject.SetActive(false);
        }
        SlotsCartes = JoueurActif.Deck;

        foreach (Carte carte in Pioche) //Ca donne les endroits du deck pour le jeu
        {
            carte.PlaceOccupee = PlacePioche;
        }

        // Mettre toutes les cartes choisies dans
        /*
        foreach(carte in carteSettings TOTALE)
        if(famille == famille choisie)
        {
            CartesMontrables.Add()
        }
        foreach(carte in carte TOTALE)
        if(famille == famille choisie)
        {
            Pioche.Add()
        }
        */

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
                    slot.CartePlacee = randCarte;
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

        Debug.Log("On change de tour : " + Tour);
        Debug.Log(JoueurPassif);
        Tour++;

        foreach (PlaceDeck slot in JoueurActif.Deck) //On cache le deck qu'on a
        {
            slot.gameObject.SetActive(false);
            if (slot.CartePlacee != null) { slot.CartePlacee.gameObject.SetActive(false); }
        }

        foreach (PlaceDeck slot in JoueurPassif.Deck) // On montre le deck de l'autre joueur
        {
            slot.gameObject.SetActive(true);
            if (slot.CartePlacee != null) { slot.CartePlacee.gameObject.SetActive(true); }
        }

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

        SlotsCartes = JoueurActif.Deck;
    }

}
