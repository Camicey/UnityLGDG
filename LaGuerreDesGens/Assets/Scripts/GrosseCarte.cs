using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrosseCarte : MonoBehaviour
{
    public CarteSettings Carte;

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

    public void Start()
    { gameObject.SetActive(true); }
    public void Montrer() //Cette fonction permet de montrer la carte et de mettre à jour ses informations
    {
        PrenomT.text = Carte.Prenom;
        ImageT.sprite = Carte.Image;
        PMT.text = Carte.PM.ToString();
        PVT.text = Carte.PVar.ToString();
        PAT.text = Carte.PA.ToString();
        PouvoirT.text = Carte.PouvoirVar;
        CoutPouvoirT.text = Carte.CoutPouvoirVar.ToString() + "PM";
        FamilleImageT.sprite = Carte.FamilleImage;
        TypeImageT.sprite = Carte.TypeImage;
        LiensT.text = MontrerLiens();
    }
    public string MontrerLiens()
    {
        string description = " ";
        foreach (CarteSettings lien in Carte.liensVar)
        {
            description += lien.Prenom + "\n";
        }
        if (description == " ") { description = "Personne"; }
        return description;
    }
    public void SeDecaler(CarteSettings carteMontree, bool montrer) //Montre ou cache la carte en fonction du bool
    {
        if (montrer == true)
        {
            Carte = carteMontree;
            Montrer();
            GetComponent<RectTransform>().anchoredPosition = new Vector2(-660, 0); //Montrer la grande carte
        }
        else
        {
            GetComponent<RectTransform>().anchoredPosition = new Vector2(-1600, 0); //Cacher la grande carte
        }
    }

}

