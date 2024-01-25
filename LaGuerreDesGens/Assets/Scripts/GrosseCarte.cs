using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrosseCarte : MonoBehaviour
{
    public CarteSettings Carte;

    public Text IdT;
    public Text PrenomT;

    public Image ImageT;

    public Text PMT;
    public Text PVT;
    public Text PAT;

    public Text PouvoirT;
    public Text CoutPouvoirT;
    public Image FamilleImageT;
    public Image TypeImageT;
    public Text LiensT;

    // Start is called before the first frame update
    public void Montrer()
    {
        IdT.text = Carte.Id.ToString();
        PrenomT.text = Carte.Prenom;
        ImageT.sprite = Carte.Image;
        PMT.text = Carte.PM.ToString();
        PVT.text = Carte.PVar.ToString();
        PAT.text = Carte.PA.ToString();
        PouvoirT.text = Carte.Pouvoir;
        CoutPouvoirT.text = Carte.CoutPouvoir.ToString() + "PM";
        FamilleImageT.sprite = Carte.FamilleImage;
        TypeImageT.sprite = Carte.TypeImage;
        LiensT.text = MontrerLiens();
    }

    public string MontrerLiens()
    {
        string description = " ";
        foreach (CarteSettings lien in Carte.liens)
        {
            description += lien.Prenom + "\n";
        }

        if (description == " ") { description = "Personne"; }

        return description;
    }

    public void SeDecaler(CarteSettings carteMontree, bool montree) //Montre la carte
    {
        if (montree == true)
        {
            Carte = carteMontree;
            Montrer();
            Debug.Log(Carte.Prenom);
            gameObject.transform.Translate(940, 0, 0f); // -618
        }
        else
        {
            gameObject.transform.Translate(-940, 0, 0f);
        }
    }
    public void Start()
    {
        Montrer();
    }




}

