using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pouvoir : MonoBehaviour
{
    public int Id;
    public GameManager Jeu;
    /*
        public IEnumerator ChangerPouvoir()
        {
            Jeu.EstEnTrainDAttaquer = true;
            Jeu.EnleverBonBoutons();
            Jeu.Warning("Choisissez avec qui va perdre son pouvoir.");
            yield return new WaitUntil(() => Jeu.CarteCiblee != null);
            Jeu.CarteCiblee.Stats.Pouvoir = "Plus de pouvoir.";
            Jeu.CarteCiblee.Stats.IdPouvoir = 0;
            Jeu.CarteMontree.AUtilisePouvoir = true;
            Jeu.CarteCiblee = null;
            Jeu.EstEnTrainDAttaquer = false;
        }
        public void ChangerDePouvoir()
        { StartCoroutine(ChangerPouvoir()); }

        IEnumerator Cible(string p, int degat, bool reccurence)
        {
            yield return new WaitUntil(() => Jeu.CarteCiblee != null);
        }
        IEnumerator Imobilise(int nbTour)
        { yield return new WaitUntil(() => Jeu.CarteCiblee != null); }

        IEnumerator CouperLien()
        { yield return new WaitUntil(() => Jeu.CarteCiblee != null); }
        IEnumerator CreerLien()
        {
            yield return new WaitUntil(() => Jeu.CarteCiblee != null);
            yield return new WaitUntil(() => Jeu.CarteCiblee != null);
        }
        IEnumerator Teleporter()
        {
            yield return new WaitUntil(() => Jeu.TerrainCiblee != null);
        }
        IEnumerator MourirCiblee()
        { yield return new WaitUntil(() => Jeu.CarteCiblee != null); }
        public void RetournerTous()
        { }
        IEnumerator Retourner(string type) //Famille ou Type
        { yield return new WaitUntil(() => Jeu.CarteCiblee != null); }
        public void RevenirPioche()
        { }*/

}