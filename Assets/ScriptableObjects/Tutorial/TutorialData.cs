using System;
using UnityEngine;

[CreateAssetMenu(fileName = "TutorialSO", menuName = "Scriptable Objects/Tutorial Data")]
public class TutorialData : ScriptableObject
{
    public Tutorial[] tutorials;

    private void Reset()
    {
        int tutorialCount = Enum.GetValues(typeof(TutorialType)).Length;
        tutorials = new Tutorial[tutorialCount];
        for (int i = 0; i < tutorialCount; i++)
        {
            TutorialType type = (TutorialType)i;
            tutorials[i] = new Tutorial(type);
        }
    }
}
