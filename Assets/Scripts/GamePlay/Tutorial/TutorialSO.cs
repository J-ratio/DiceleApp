using UnityEngine;

[CreateAssetMenu()]
public class TutorialSO : ScriptableObject
{
    public int level;
    public bool tutorialStart;
    public TutorialSwapType[] tutorialTypes;
    public TutorialStartType tutorialFinishType;
}

[System.Serializable]
public class TutorialSwapType
{
    public int dice_1;
    public int dice_2;
    public string message;
    public Sprite icon;
}
[System.Serializable]
public class TutorialStartType
{
    public string message;
    public Sprite icon;
}