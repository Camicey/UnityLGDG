using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameManager JeuEnCours;
    public GameObject Ecran;
    public GameObject MenuPauseUI;
    public Button BoutonNext;
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
        JeuEnCours.NouvellePartie();
        Ecran.SetActive(false);
    }

    public void Quitter() { Application.Quit(); }

    public void RetourAuMenu() { SceneManager.LoadScene("Menu"); }

    public void MAJ()
    {
        if (JeuEnCours.FamillesChoisies.Count == 2)
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