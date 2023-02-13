using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GamePlayAnimation : MonoBehaviour
{
    [SerializeField] private FlowTransforms[] horizontalFlow;
    [SerializeField] private FlowTransforms[] verticalFlow;

    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float time = 0.5f;
    [SerializeField] private float bounceHeight = 40;
    [SerializeField] private int bounce = 1;

    public bool isFinished = true;

    private void Start()
    {
        isFinished = true;
    }
    public void AnimateVertical()
    {
        if (isFinished)
        {
            isFinished = false;
            for (int i = 0; i < verticalFlow.Length; i++)
            {
                if (i == verticalFlow.Length - 1)
                {
                    AnimateFlow(verticalFlow[i].myTransforms, Finished);
                }
                else
                {
                    AnimateFlow(verticalFlow[i].myTransforms);
                }
            }
        }
    }

    public void AnimateVerticalSingle(int l_column)
    {
        if (isFinished)
        {
            isFinished = false;
            AnimateFlow(verticalFlow[l_column].myTransforms, Finished);
        }
    }

    void Finished()
    {
        //Debug.Log("Finished");
        isFinished = true;
    }

    public void AnimateHorizontalSingle(int l_row)
    {
        if (isFinished)
        {
            isFinished = false;
            AnimateFlow(horizontalFlow[l_row].myTransforms, Finished);
        }
    }
    
    public void AnimateVerticalSingle(int l_column, Action callBack = null)
    {
        if (isFinished)
        {
            isFinished = false;
            AnimateFlow(verticalFlow[l_column].myTransforms, callBack);
        }
    }

    public void AnimateHorizontalSingle(int l_row, Action callBack )
    {
        if (isFinished)
        {
            isFinished = false;
            AnimateFlow(horizontalFlow[l_row].myTransforms, callBack);
        }
    }

    public void AnimateHorizontal()
    {
        if (isFinished)
        {
            isFinished = false;
            for (int i = 0; i < horizontalFlow.Length; i++)
            {
                if (i == horizontalFlow.Length - 1)
                {
                    AnimateFlow(horizontalFlow[i].myTransforms, Finished);
                }
                else
                {
                    AnimateFlow(horizontalFlow[i].myTransforms);
                }
            }
        }
    }

    void AnimateFlow(Transform[] myTransforms, Action callBack = null)
    {
        for (int i = 0; i < myTransforms.Length; i++)
        {
            if (i == myTransforms.Length - 1)
            {
                StartCoroutine(FlowAnimation(((float)(i * time)) / 2, bounce, myTransforms[i], callBack));
            }
            else
            {
                StartCoroutine(FlowAnimation(((float)(i * time)) / 2, bounce, myTransforms[i]));
            }
        }

    }

    public IEnumerator FlowAnimation(float waitTime, int count, Transform objTransform, Action callBack = null)
    {
        float startPointY = objTransform.transform.position.y;
        yield return new WaitForSeconds(waitTime);
        float moveDis = 0;
        float l_bounceHeight = bounceHeight;
        for (int i = 0; i < count; i++)
        {
            while (moveDis < 2)
            {
                moveDis += Time.deltaTime / time;
                objTransform.position = new Vector3(objTransform.position.x, startPointY + curve.Evaluate(moveDis) * l_bounceHeight, objTransform.position.z);
                yield return null;
            }
            moveDis = 0;
            l_bounceHeight /= 2;
        }
        callBack?.Invoke();
    }

}

[Serializable]
public class FlowTransforms
{
    public Transform[] myTransforms;
}