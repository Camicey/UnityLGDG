using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (EstActive == false)
        {
            EstActive = true;
            Menu.JeuEnCours.FamillesChoisies.Add(Famille);
            //GetComponent<Image>().color = Color.blue;
        }
        else
        {
            EstActive = false;
            Menu.JeuEnCours.FamillesChoisies.Remove(Famille);
            //GetComponent<Image>().color = Color.white;
        }
        Menu.MAJ();
    }

}
