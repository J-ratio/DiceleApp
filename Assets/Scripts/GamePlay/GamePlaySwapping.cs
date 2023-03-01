using System;
using System.Collections;
using UnityEngine;

public enum ColorType
{
    Yellow,
    White,
    Green
}
public class GamePlaySwapping : MonoBehaviour
{
    [SerializeField] private GamePlayAnimation gamePlayAnimation;

    [SerializeField] private Transform[] dices;

    [SerializeField] private float speed = 2;

    [SerializeField] private ParticleSystem particleSystem_1;
    [SerializeField] private ParticleSystem particleSystem_2;

    public bool isSwapping;
    [SerializeField] private Color yellowColor;
    [SerializeField] private Color whiteColor;
    [SerializeField] private Color greenColor;
    public void SwapDices(Dice dice_1, Dice dice_2, Action callBack, int intialSortOrder, int finalSortOrder)
    {
        if (!isSwapping)
        {
            isSwapping = true;
            Transform dice = dice_1.transform.parent;
            dice_1.transform.SetParent(dice_2.transform.parent);
            dice_2.transform.SetParent(dice);

            StartCoroutine(Swap(dice_1, dice_2, callBack, intialSortOrder, finalSortOrder));
        }
    }

    IEnumerator Swap(Dice dice_1, Dice dice_2, Action callBack, int intialSortOrder, int finalSortOrder)
    {
        bool dice_1_particle = false;

        bool dice_2_particle = false;

        bool dice_1_animation = false;

        bool dice_2_animation = false;

        dice_1.spriteRenderer.sortingOrder = intialSortOrder;

        dice_2.spriteRenderer.sortingOrder = intialSortOrder;

        while (dice_1.transform.localPosition != Vector3.zero || dice_2.transform.localPosition != Vector3.zero)
        {
            dice_1.transform.localPosition = Vector3.MoveTowards(dice_1.transform.localPosition, Vector3.zero, Time.deltaTime * speed * 100);

            if (!dice_1_particle && Vector3.Distance(dice_1.transform.localPosition, Vector3.zero) < 0.1f)
            {
                dice_1_particle = true;
                if (SetParticleColor(particleSystem_1, dice_1.colorType))
                {
                    particleSystem_1.transform.position = dice_1.transform.position;

                    particleSystem_1.Play();
                }
                StartCoroutine(gamePlayAnimation.FlowAnimation(0, 1, dice_1.transform, 0, DiceAnime_1));
            }

            dice_2.transform.localPosition = Vector3.MoveTowards(dice_2.transform.localPosition, Vector3.zero, Time.deltaTime * speed * 100);

            if (!dice_2_particle && Vector3.Distance(dice_2.transform.localPosition, Vector3.zero) < 0.1f)
            {
                dice_2_particle = true;
                if (SetParticleColor(particleSystem_2, dice_2.colorType))
                {
                    particleSystem_2.transform.position = dice_2.transform.position;
                    //SetParticleColor(particleSystem_2, dice_2.colorType);
                    particleSystem_2.Play();
                }
                StartCoroutine(gamePlayAnimation.FlowAnimation(0, 1, dice_2.transform, 0, DiceAnime_2));
            }

            yield return null;
        }
        Debug.Log("swapped");
        dice_1.spriteRenderer.sortingOrder = finalSortOrder;

        dice_2.spriteRenderer.sortingOrder = finalSortOrder;

        isSwapping = false;

        callBack?.Invoke();


        void DiceAnime_1()
        {
            dice_1_animation = true;
        }

        void DiceAnime_2()
        {
            dice_2_animation = true;
        }
    }

    IEnumerator Swap_2(Dice dice_1, Dice dice_2, Action callBack, int intialSortOrder, int finalSortOrder)
    {
        bool dice_1_particle = false;

        bool dice_2_particle = false;

        bool dice_1_animation = false;

        bool dice_2_animation = false;

        dice_1.spriteRenderer.sortingOrder = intialSortOrder;

        dice_2.spriteRenderer.sortingOrder = intialSortOrder;

        while (dice_1.transform.localPosition != Vector3.zero || dice_2.transform.localPosition != Vector3.zero)
        {
            //dice_1.transform.localPosition = Vector3.MoveTowards(dice_1.transform.localPosition, Vector3.zero, Time.deltaTime * speed * 100);
            dice_1.transform.localPosition = Vector3.zero;
            if (!dice_1_particle && Vector3.Distance(dice_1.transform.localPosition, Vector3.zero) < 0.1f)
            {
                dice_1_particle = true;
                PlayParticle(particleSystem_1, dice_1.colorType, dice_1.transform.position);
            }

            //dice_2.transform.localPosition = Vector3.MoveTowards(dice_2.transform.localPosition, Vector3.zero, Time.deltaTime * speed * 100);
            dice_2.transform.localPosition = Vector3.zero;
            if (!dice_2_particle && Vector3.Distance(dice_2.transform.localPosition, Vector3.zero) < 0.1f)
            {
                dice_2_particle = true;
                PlayParticle(particleSystem_2, dice_2.colorType, dice_2.transform.position);
            }

            yield return null;
        }

        dice_1.spriteRenderer.sortingOrder = finalSortOrder;

        dice_2.spriteRenderer.sortingOrder = finalSortOrder;

        isSwapping = false;

        dice_1.ResetScale();
        dice_2.ResetScale();

        callBack?.Invoke();


        void DiceAnime_1()
        {
            dice_1_animation = true;
        }

        void DiceAnime_2()
        {
            dice_2_animation = true;
        }
    }

    void PlayParticle(ParticleSystem particle, ColorType colorType, Vector3 pos)
    {
        if (colorType == ColorType.Green)
        {
            particle.transform.position = pos;
            particle.startColor = greenColor;
            particle.Play();
        }
        //else if (colorType == ColorType.Yellow) particle.startColor = yellowColor;
        //else particle.startColor = whiteColor;
    }
    bool SetParticleColor(ParticleSystem particle, ColorType colorType)
    {
        if (colorType == ColorType.Green)
        {
            particle.startColor = greenColor;
            return true;
        }
        return false;
        //else if (colorType == ColorType.Yellow) particle.startColor = yellowColor;
        //else particle.startColor = whiteColor;
    }
}
