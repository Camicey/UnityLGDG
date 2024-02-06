using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FonctionsInutiles : MonoBehaviour
{
    /*
    public Carte ChercherCarteANous(int id)
    {
        foreach (Carte carteCherchee in JoueurActif.CartesPossedees)
        {
            if (carteCherchee.Id == id)
            {
                return carteCherchee;
            }
        }
        Debug.Log("Ah pas de possession");
        return JoueurActif.CartesPossedees[0];
    }
    
    public CarteSettings ChercherCarteSettings(int id)
    {
        foreach (CarteSettings carteCherchee in JeuEnCours.CartesMontrable)
        {
            if (carteCherchee.Id == id)
            {
                return carteCherchee;
            }
        }
        return JeuEnCours.CartesMontrable[0];
    }
     IEnumerator AttendreCiblee(Carte attaquant, Carte receveur)
    {
        Warning("Choisissez avec qui " + attaquant.Stats.Prenom + " va se battre.");
        yield return new WaitUntil(() => receveur != null);
        FondColore.gameObject.transform.Translate(0, 50, 0f);
        if (receveur.Appartenance == JoueurActif) // Vérifier que la carte est à côtée // Vérifier que la carte n'est pas liée
        {
            Warning("Cette carte vous appartient !");
        }
        else if (!VerifierTerrainACote())
        {
            Warning("Cette carte est trop loin !");
        }
        else
        {
            PMEnCours--;
            receveur.GetComponent<UnityEngine.UI.Image>().sprite = receveur.ImageOriginale;
            receveur.EstCachee = false;
            attaquant.EstCachee = false;
            receveur.Stats.PVar = receveur.Stats.PVar - attaquant.Stats.PA;
            Warning(attaquant.Stats.Prenom + " a infligé " + attaquant.Stats.PA.ToString() + " a " + receveur.Stats.Prenom);
            if (receveur.Stats.PVar > 0)
            {
                attaquant.Stats.PVar = attaquant.Stats.PVar - receveur.Stats.PA;
                Warning(receveur.Stats.Prenom + " a infligé " + receveur.Stats.PA.ToString() + " en retour.");
                if (attaquant.Stats.PVar <= 0)
                {
                    attaquant.Mourir();
                    Warning(attaquant.Stats.Prenom + " est mort(e).");
                }
            }
            else
            {
                Warning("Vous avez tué " + receveur.Stats.Prenom + " avec " + attaquant.Stats.Prenom);
                receveur.Mourir();
            }

        }
        receveur = null;
        EstEnTrainDAttaquer = false;
        CacherGrandeCarte();
        AfficherPM();
    }

    */
}