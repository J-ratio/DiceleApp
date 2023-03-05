using UnityEngine;

public class HandAnimation : MonoBehaviour
{
    [SerializeField] public Transform waypoint_1;

    [SerializeField] public Transform waypoint_2;

    [SerializeField] private Animator handAnimation;

    [SerializeField] private float speed = 2;

    private int handDownAnimation = Animator.StringToHash("HandDownAnimation");

    private int handUpAnimation = Animator.StringToHash("HandUpAnimation");

    private float nextTime;

    [SerializeField] private float reEnableTime = 2;

    Dice dice_1;

    Dice dice_2;
    
    internal void Init(Dice l_dice_1,Dice l_dice_2)
    {
        waypoint_1.position = l_dice_1.transform.parent.position;
        waypoint_2.position = l_dice_2.transform.parent.position;
        dice_1 = l_dice_1;
        dice_2 = l_dice_2;
        ResetMovement();
        Show();
    }

    public void StopAnimation() => transform.GetChild(0).gameObject.SetActive(false);

    public void StartAnimation() => transform.GetChild(0).gameObject.SetActive(true);

    internal void Hide() => gameObject.SetActive(false);

    internal void Show()
    {
        gameObject.SetActive(true);
        nextTime = Time.time + reEnableTime;
        StartAnimation();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!CheckDices()) return; 

            if (dice_1.select_tag || dice_2.select_tag)
            StopAnimation();
        }
        if (Input.GetMouseButton(0))
        {
            if (!CheckDices()) return;

            if (dice_1.select_tag || dice_2.select_tag)
            {
                nextTime = Time.time + reEnableTime;
                return;
            }
        }
        if (nextTime < Time.time)
        {
            StartAnimation();
        }
        if (handAnimation.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            transform.position = Vector3.MoveTowards(transform.position, waypoint_2.position, Time.deltaTime * speed);

        if (transform.position == waypoint_2.position)
        {
            if (handAnimation.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                handAnimation.Play(handUpAnimation, 0, 0);

        }
    }

    public void ResetMovement()
    {
        transform.position = waypoint_1.position;
        handAnimation.Play(handDownAnimation, 0, 0);
    }

    bool CheckDices()
    {
        if (dice_1 == null) return false;
        if (dice_2 == null) return false;

        return true;
    }
}
