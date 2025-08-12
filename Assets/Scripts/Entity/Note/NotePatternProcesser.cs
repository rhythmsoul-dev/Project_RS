using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NotePatternProcesser : MonoBehaviour
{
    private List<NotePatternInfo> infos = new List<NotePatternInfo>();
    private Note[] curPatternNotes;

    private Coroutine processCoroutine;

    public bool IsProcessing { get; private set; }

    public bool IsInterrupted { get; set; }

    public void AddPattern(NotePatternInfo info)
    {
        infos.Add(info);
    }

    public void RemovePattern(NotePatternInfo info)
    {
        infos.Remove(info);
    }

    public void RemovePattern(int index)
    {
        infos.RemoveAt(index);
    }

    public void StartPatterns()
    {
        if (processCoroutine != null)
        {
            StopCoroutine(processCoroutine);
            processCoroutine = null;
        }

        IsInterrupted = false;
        IsProcessing = true;
        processCoroutine = StartCoroutine(ProcessPatterns());
    }

    private IEnumerator ProcessPatterns()
    {
        yield return new WaitForSeconds(0.3f);
        while (gameObject.activeSelf)
        {
            if (IsInterrupted)
            {
                yield break;
            }

            for (int i = 0; i < infos.Count; i++)
            {
                if (IsInterrupted)
                {
                    yield break;
                }

                int[] ids = infos[i].NoteInfos.Select(info => info.Id).ToArray();
                int[] indexes = infos[i].NoteInfos.Select(info => info.Index).ToArray();
                int[] delays = infos[i].NoteInfos.Select(info => info.Delay).ToArray();

                curPatternNotes = NoteSystem.Instance().MakeNotes(ids, indexes, delays);

                yield return new WaitUntil(() => curPatternNotes.Length == infos[i].NoteInfos.Length);
                yield return new WaitUntil(() => curPatternNotes.Where(n => n.gameObject.activeSelf).All(n => n.IsEnded));

                yield return new WaitForSeconds(infos[i].Delay * 0.001f);
            }

            yield return null;
        }

        yield break;
    }

    public void StopAllPatterns()
    {
        StopAllCoroutines();
        processCoroutine = null;
        IsProcessing = false;
        IsInterrupted = true;

        infos.Clear();
        if (curPatternNotes != null)
        {
            for (int i = 0; i < curPatternNotes.Length; i++)
            {
                curPatternNotes[i].End();
            }

            curPatternNotes = null;
        }
    }

    public int GetActiveNoteCount()
    {
        int count = 0;
        for (int i = 0; i < curPatternNotes.Length; i++)
        {
            if (curPatternNotes[i].gameObject.activeSelf)
            {
                count++;
            }
        }

        return count;
    }
}
