using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayParticle : MonoBehaviour
{
    [SerializeField] private GamePlayAnimation gamePlayAnimation;

    [SerializeField] private Transform[] dices;

    [SerializeField] private int diceIndex_1;
    [SerializeField] private int diceIndex_2;

    [SerializeField] private float speed = 2;

    [SerializeField] private ParticleSystem particleSystem_1;
    [SerializeField] private ParticleSystem particleSystem_2;

    public bool isSwapping;
    
    public void SwapDices()
    {
        if (!isSwapping)
        {
            isSwapping = true;
            Transform dice_1 = dices[diceIndex_1].GetChild(0);
            Transform dice_2 = dices[diceIndex_2].GetChild(0);
            dice_2.SetParent(dices[diceIndex_1]);
            dice_1.SetParent(dices[diceIndex_2]);

            StartCoroutine(Swap(dice_1.GetComponent<SpriteRenderer>(), dice_2.GetComponent<SpriteRenderer>()));
        }
    }

    IEnumerator Swap(SpriteRenderer dice_1, SpriteRenderer dice_2)
    {
        bool dice_1_particle = false;

        bool dice_2_particle = false;
        
        bool dice_1_animation = false;

        bool dice_2_animation = false;

        dice_1.sortingOrder = 6;

        dice_2.sortingOrder = 6;

        while (dice_1.transform.localPosition != Vector3.zero && dice_2.transform.localPosition != Vector3.zero)
        {
            dice_1.transform.localPosition = Vector3.MoveTowards(dice_1.transform.localPosition, Vector3.zero,Time.deltaTime * speed * 100);

            if (!dice_1_particle && Vector3.Distance(dice_1.transform.localPosition,Vector3.zero) < 0.1f)
            {
                dice_1_particle = true;
                particleSystem_1.transform.position = dice_1.transform.position;
                particleSystem_1.Play();
                StartCoroutine(gamePlayAnimation.FlowAnimation(0,1,dice_1.transform, DiceAnime_1));
            }

            dice_2.transform.localPosition = Vector3.MoveTowards(dice_2.transform.localPosition, Vector3.zero,Time.deltaTime * speed * 100);

            if (!dice_2_particle && Vector3.Distance(dice_2.transform.localPosition, Vector3.zero) < 0.1f)
            {
                dice_2_particle = true;
                particleSystem_2.transform.position = dice_2.transform.position;
                particleSystem_2.Play();
                StartCoroutine(gamePlayAnimation.FlowAnimation(0, 1, dice_2.transform, DiceAnime_2));
            }

            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        dice_1.sortingOrder = 4;

        dice_2.sortingOrder = 4;

        isSwapping = false;

        void DiceAnime_1()
        {
            dice_1_animation = true;
        }

        void DiceAnime_2()
        {
            dice_2_animation = true;
        }
    }
}
