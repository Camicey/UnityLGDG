using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;

public class MainMenu : NetworkBehaviour
{
    public JoueurManager JoueurManager;
    public GameManager JeuEnCours;
    public GameObject Ecran;
    public GameObject MenuPauseUI;
    public Button BoutonNext;
    public GameObject ImageGrandePartie;
    public static bool JeuEnPause = false;


    // Start is called before the first frame update
    void Start()
    {
        if (MenuPauseUI != null) { MenuPauseUI.SetActive(false); }
        if (BoutonNext != null) { BoutonNext.interactable = false; }
        if (JeuEnCours != null) { JeuEnCours.FamillesChoisies.Clear(); }
    }

    public void Familles()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void Jouer()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        JoueurManager = networkIdentity.GetComponent<JoueurManager>();
        JoueurManager.CmdInstancier();
    }


    public void InitialiserLonguePartie()
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

    public void InitialiserPetitePartie()
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

    public void PlacerDeck(Joueur joueur)
    {
        for (int i = 0; i < 5; i++)
        { joueur.Deck[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(-892 + i * 182, -485); }
    }

    public void Quitter() { Application.Quit(); }

    public void RetourAuMenu() { SceneManager.LoadScene("Menu"); }

    public void MAJ()
    {
        if (JeuEnCours.FamillesChoisies.Count == 2 && JeuEnCours.TypePartie != "")
        { BoutonNext.interactable = true; }
        else { BoutonNext.interactable = false; }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (JeuEnPause) { Reprendre(); }
            else { Pause(); }
        }
    }

    public void Reprendre()
    {
        MenuPauseUI.SetActive(false);
        JeuEnPause = false;
    }
    public void Pause()
    {
        MenuPauseUI.SetActive(true);
        JeuEnPause = true;
    }

}