using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayAnimation : MonoBehaviour
{
    private List<FlowTransforms> horizontalFlow = new List<FlowTransforms>();
    private List<FlowTransforms> verticalFlow = new List<FlowTransforms>();

    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float finishedWaitTime = 0.5f;
    [SerializeField] private float time = 0.5f;
    [SerializeField] private float bounceHeight = 40;
    [SerializeField] private int bounce = 1;

    //bool isFinished = true;

    //private void Start() => isFinished = true;

    internal void CheckAndPlayFinalHorizontalAnimation(Action callback)
    {
        Debug.Log("CheckAndPlayFinalHorizontalAnimation");
        //if (isFinished)
        //{
        //    isFinished = false;
            for (int i = 0; i < horizontalFlow.Count; i++)
            {
                if (i == horizontalFlow.Count - 1)
                    AnimateHorizontalSingle(i, callback, finishedWaitTime);
                else
                    AnimateHorizontalSingle(i, false);
            }
        //}
    }

    internal void CheckAndPlayFinalVerticalAnimation(Action callback)
    {
        //if (isFinished)
        //{
        //    isFinished = false;
            for (int i = 0; i < horizontalFlow.Count; i++)
            {
                if (i == horizontalFlow.Count - 1)
                    AnimateVerticalSingle(i, callback, finishedWaitTime);
                else
                    AnimateVerticalSingle(i, false);
            }
        //}
    }
    internal void CheckAndPlayHorizontalAnimation(Action callback,int rowIndex)
    {
        Debug.Log("CheckAndPlayHorizontalAnimation");
        AnimateHorizontalSingle(rowIndex, callback);
    }
    
    internal void CheckAndPlayVerticalAnimation(Action callback,int rowIndex)
    {
        Debug.Log("CheckAndPlayHorizontalAnimation");
        AnimateVerticalSingle(rowIndex, callback);
    }
    
    public void CheckAndPlayHorizontalAnimation(int rowIndex)
    {
        Debug.Log("CheckAndPlayHorizontalAnimation");
        AnimateHorizontalSingle(rowIndex, null);
    }
    
    internal void CheckAndPlayVerticalAnimation(int rowIndex)
    {
        Debug.Log("CheckAndPlayHorizontalAnimation");
        AnimateVerticalSingle(rowIndex, null);
    }

    internal void AddDice(List<Dice> diceList, int rowColumnCount)
    {
        horizontalFlow.Clear();
        verticalFlow.Clear();

        int rowIndex = 0;

        for (int i = 0; i < rowColumnCount; i++)
        {
            horizontalFlow.Add(new FlowTransforms());
            verticalFlow.Add(new FlowTransforms());
            for (int j = 0; j < rowColumnCount; j++)
            {
                horizontalFlow[rowIndex].myTransforms.Add(diceList[rowColumnCount * i + j].transform.parent);
                verticalFlow[rowIndex].myTransforms.Add(diceList[i + rowColumnCount * j].transform.parent);
            }
            rowIndex++;
        }
    }

    private void Finished()
    {
        Debug.Log("Finished");
        //isFinished = true;
    }

    private void AnimateHorizontalSingle(int l_row, bool call) => AnimateFlow(horizontalFlow[l_row].myTransforms);

    private void AnimateHorizontalSingle(int l_row, Action callBack, float l_finishedWaitTime = 0) => AnimateFlow(horizontalFlow[l_row].myTransforms, callBack, l_finishedWaitTime);

    private void AnimateVerticalSingle(int l_row, bool call) => AnimateFlow(verticalFlow[l_row].myTransforms);

    private void AnimateVerticalSingle(int l_row, Action callBack, float l_finishedWaitTime = 0) => AnimateFlow(verticalFlow[l_row].myTransforms, callBack, l_finishedWaitTime);

    private void AnimateFlow(List<Transform> myTransforms, Action callBack = null, float l_finishedWaitTime = 0)
    {
        float reduceTimeby = 2;
        for (int i = 0; i < myTransforms.Count; i++)
        {
            if (i == myTransforms.Count - 1)
                StartCoroutine(FlowAnimation(((float)(i * time)) / reduceTimeby, bounce, myTransforms[i].transform, l_finishedWaitTime, callBack));
            else
                StartCoroutine(FlowAnimation(((float)(i * time)) / reduceTimeby, bounce, myTransforms[i].transform, l_finishedWaitTime, null));
        }
    }

    internal IEnumerator FlowAnimation(float waitTime, int count, Transform objTransform, float l_finishedWaitTime = 0, Action callBack = null)
    {
        float startPointY = objTransform.transform.position.y;
        yield return new WaitForSeconds(waitTime);
        float moveDis = 0;
        float l_bounceHeight = bounceHeight;
        float reducebounceby = 2;
        float moveDisMax = 2;

        for (int i = 0; i < count; i++)
        {
            while (moveDis < moveDisMax)
            {
                moveDis += Time.deltaTime / time;
                objTransform.position = new Vector3(objTransform.position.x, startPointY + curve.Evaluate(moveDis) * l_bounceHeight, objTransform.position.z);
                yield return null;
            }
            moveDis = 0;
            l_bounceHeight /= reducebounceby;
        }
        yield return new WaitForSeconds(l_finishedWaitTime);
        if (callBack != null)
        {
            Finished();
        }
        callBack?.Invoke();
    }

}

[Serializable]
public class FlowTransforms
{
    public List<Transform> myTransforms = new List<Transform>();
}