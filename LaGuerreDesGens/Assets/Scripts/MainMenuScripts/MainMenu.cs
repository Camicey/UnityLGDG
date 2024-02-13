using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button BoutonNext;
    public GameObject Ecran;
    public GameManager JeuEnCours;

    // Start is called before the first frame update
    void Start()
    {
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

    public void Quitter()
    {
        Application.Quit();
    }

    public void MAJ()
    {
        if (JeuEnCours.FamillesChoisies.Count == 2)
        { BoutonNext.interactable = true; }
        else { BoutonNext.interactable = false; }
    }

}