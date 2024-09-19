using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoixDeFamille : MonoBehaviour
{
    public bool EstActive = false;
    public string Famille;
    public MainMenu Menu;

    void Start() // Start is called before the first frame update
    {
        EstActive = false;
    }

    public void SelectionnerFamille() //Quand je sélectionne une famille
    {
        var colors = GetComponent<Image>().color;

        if (EstActive == false)
        {
            EstActive = true;
            Menu.JeuEnCours.FamillesChoisies.Add(Famille);
            colors = Color.grey;
        }
        else
        {
            EstActive = false;
            Menu.JeuEnCours.FamillesChoisies.Remove(Famille);
            colors = Color.white;
        }
        GetComponent<Image>().color = colors;
        Menu.MAJ();
    }
    public void SelectionnerTypePartie()
    {
        var colors = GetComponent<Image>().color;

        if (EstActive == false && Menu.JeuEnCours.TypePartie == "")
        {
            EstActive = true;
            Menu.JeuEnCours.TypePartie = Famille;
            colors = Color.grey;
        }
        else if (EstActive == true && Menu.JeuEnCours.TypePartie == Famille)
        {
            EstActive = false;
            Menu.JeuEnCours.TypePartie = "";
            colors = Color.white;
        }
        GetComponent<Image>().color = colors;
        Menu.MAJ();
    }
}
