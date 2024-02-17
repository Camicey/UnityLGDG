using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoixDeFamille : MonoBehaviour
{
    public bool EstActive = false;
    public string Famille;

    public MainMenu Menu;
    // Start is called before the first frame update
    void Start()
    {
        EstActive = false;
    }

    public void Selectionner()
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

}
