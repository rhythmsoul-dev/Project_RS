using UnityEngine;

public abstract class UIControllerBase : MonoBehaviour
{
    private void Start()
    {
        SetUp();
    }

    protected abstract void SetUp();

    public abstract void Show();
    public abstract void Hide();
}