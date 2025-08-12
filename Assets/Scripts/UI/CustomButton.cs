using UnityEngine.UI;

public class CustomButton : Button
{
    public new bool IsHighlighted()
    {
        return currentSelectionState == SelectionState.Highlighted;
    }
}