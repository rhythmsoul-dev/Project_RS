using System.Collections;
using UnityEngine;

public class BlowZone : MonoBehaviour
{
    private void Awake()
    {
        NoteSystem.Instance().BlowZone = this;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Note"))
        {
            Note note = collision.GetComponent<Note>();
            if (note.IsInitialized && !note.IsEnded)
            {
                note.Miss();
            }
        }
    }

    public void SetPosition(Transform parent, float t)
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = new Vector3(parent.position.x - (parent.localScale.x * 0.5f) + 0.6105f, 0f, 0f);

        transform.position = Vector3.Lerp(startPos, targetPos, t);
    }
}
