using System;
using UnityEngine;

public class BackGroundLooper : MonoBehaviour
{
    [SerializeField] private Transform backGroundContainer;
    [SerializeField] private Transform[] backgrounds = new Transform[2];
    [SerializeField] private float backgroundSizeX;
    private Transform left;
    private Transform right;

    private void Reset()
    {
        Init();
    }

    private void Awake()
    {
        left = backgrounds[0];
        right = backgrounds[1];
        foreach (var background in backgrounds)
        {
            background.position += 0.5f * backgroundSizeX * Vector3.right;
        }
    }

    private void Init()
    {
        backGroundContainer = transform.GetChild(0);
        for (int i = 0; i < backGroundContainer.childCount; i++)
        {
            backgrounds[i] = backGroundContainer.GetChild(i).transform;
        }

        backgroundSizeX = MathF.Abs(backgrounds[0].localPosition.x);
        
        left = backgrounds[0];
        right = backgrounds[1];
    }

    private void FixedUpdate()
    {
        if (PlayerManager.Instance().LocalPlayer.IsLocked || CameraManager.Instance().CinemachineCamera != CameraManager.Instance().PlayerCamera)
        {
            return;
        }
        
        if (left.position.x < transform.position.x - backgroundSizeX)
        {
            left.position = right.position + Vector3.right * backgroundSizeX;
            Swap();
        }
        else if (right.position.x > transform.position.x + backgroundSizeX)
        {
            right.position = left.position - Vector3.right * backgroundSizeX;
            Swap();
        }
    }

    private void Swap()
    {
        (backgrounds[0], backgrounds[1]) = (backgrounds[1], backgrounds[0]);

        left = backgrounds[0];
        right = backgrounds[1];
    }
}
