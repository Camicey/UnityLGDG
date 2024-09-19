using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class MainMenu : NetworkBehaviour
{
    public JoueurManager JoueurManager;
    public GameManager JeuEnCours;
    public GameObject Ecran;
    public GameObject MenuPauseUI;
    public Button BoutonNext;
    public GameObject ImageGrandePartie;
    public static bool JeuEnPause = false;
    public TMP_InputField ZoneNomJ1; // Référence à l'InputField UI
    public string NomJ1 = "";
    public TMP_InputField ZoneNomJ2; // Référence à l'InputField UI
    public string NomJ2 = "";

    public void OnTextChanged1() { NomJ1 = ZoneNomJ1.text; MAJ(); }
    public void OnTextChanged2() { NomJ2 = ZoneNomJ2.text; MAJ(); }

    void Start() // Start is called before the first frame update
    {
        if (MenuPauseUI != null) { MenuPauseUI.SetActive(false); }
        if (BoutonNext != null) { BoutonNext.interactable = false; }
        if (JeuEnCours != null) { JeuEnCours.FamillesChoisies.Clear(); }
        NomJ1 = "";
        NomJ2 = "";
    }

    public void Jouer() //Commence le jeu, utilisé qu'une fois, utilise le multijoueur
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        JoueurManager = networkIdentity.GetComponent<JoueurManager>();
        JeuEnCours.J1.Prenom = NomJ1;
        JeuEnCours.J2.Prenom = NomJ2;
        if (JeuEnCours.ToutesLesCartes.Count == 0)
        { JoueurManager.CmdInstancier(); }
    }

    public void InitialiserPetitePartie() //Place tous les terrains correctement en début de petite partie.
    {
        foreach (PlaceTerrain terrain in JeuEnCours.ToutTerrain)
        {
            if (terrain.Id == 0) { terrain.GetComponent<RectTransform>().anchoredPosition = new Vector2(560, -330); }
            if (terrain.Id == 1) { terrain.GetComponent<RectTransform>().anchoredPosition = new Vector2(300, -230); }
            if (terrain.Id == 2) { terrain.GetComponent<RectTransform>().anchoredPosition = new Vector2(820, -230); }
            if (terrain.Id == 3) { terrain.GetComponent<RectTransform>().anchoredPosition = new Vector2(560, 330); }
            if (terrain.Id == 4) { terrain.GetComponent<RectTransform>().anchoredPosition = new Vector2(300, 230); }
            if (terrain.Id == 5) { terrain.GetComponent<RectTransform>().anchoredPosition = new Vector2(820, 230); }
        }
    }
    public void InitialiserLonguePartie() //Pareil pour une grande partie
    {
        ImageGrandePartie.transform.Translate(0, 0, -3f);
        foreach (PlaceTerrain terrain in JeuEnCours.ToutTerrain)
        {
            if (terrain.Id == 0) { terrain.GetComponent<RectTransform>().anchoredPosition = new Vector2(550, -400); }
            if (terrain.Id == 1) { terrain.GetComponent<RectTransform>().anchoredPosition = new Vector2(280, -400); }
            if (terrain.Id == 2) { terrain.GetComponent<RectTransform>().anchoredPosition = new Vector2(820, -400); }
            if (terrain.Id == 3) { terrain.GetComponent<RectTransform>().anchoredPosition = new Vector2(550, 400); }
            if (terrain.Id == 4) { terrain.GetComponent<RectTransform>().anchoredPosition = new Vector2(280, 400); }
            if (terrain.Id == 5) { terrain.GetComponent<RectTransform>().anchoredPosition = new Vector2(820, 400); }
            if (terrain.Id == 6) { terrain.GetComponent<RectTransform>().anchoredPosition = new Vector2(380, -150); }
            if (terrain.Id == 7) { terrain.GetComponent<RectTransform>().anchoredPosition = new Vector2(720, -150); }
            if (terrain.Id == 8) { terrain.GetComponent<RectTransform>().anchoredPosition = new Vector2(380, 150); }
            if (terrain.Id == 9) { terrain.GetComponent<RectTransform>().anchoredPosition = new Vector2(720, 150); }
            if (terrain.Id == 10) { terrain.GetComponent<RectTransform>().anchoredPosition = new Vector2(550, 0); }
        }
    }

    public void PlacerDeck(Joueur joueur) //Place les decks aux bons endroits
    {
        for (int i = 0; i < 5; i++)
        { joueur.Deck[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(-892 + i * 182, -485); }
    }

    public void Reprendre() //Reprendre la partie après une pause
    {
        MenuPauseUI.SetActive(false);
        JeuEnPause = false;
    }
    public void Pause() //Mettre le jeu en pause
    {
        MenuPauseUI.SetActive(true);
        JeuEnPause = true;
    }
    public void RetourAuMenu() { SceneManager.LoadScene("Menu"); }
    public void Familles() { SceneManager.LoadScene("SampleScene"); }
    public void Quitter() { Application.Quit(); }

    public void MAJ() //A chaque fois il vérifie que les conditions pour débuter la partie sont remplie
    {
        if (JeuEnCours.FamillesChoisies.Count == 2 && JeuEnCours.TypePartie != "" && NomJ1 != "" && NomJ2 != "" && NomJ1 != NomJ2)
        { BoutonNext.interactable = true; }
        else { BoutonNext.interactable = false; }
    }

    public void Update() //Fonction qui s'execute à chaque frame
    {
        if (Input.GetKeyDown(KeyCode.Escape)) //Si on appuie sur la touche echap
        {
            if (JeuEnPause) { Reprendre(); }
            else { Pause(); }
        }
    }
}