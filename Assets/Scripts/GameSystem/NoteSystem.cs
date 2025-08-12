using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NoteSystem : GameSystem<NoteSystem>
{
    public HitZone HitZone { get; set; }
    public BlowZone BlowZone { get; set; }

    public NotePatternProcesser PatternProcesser { get; private set; }

    private void Awake()
    {
        if (PatternProcesser == null)
        {
            PatternProcesser = new GameObject("NotePatternProcesser").AddComponent<NotePatternProcesser>();
            PatternProcesser.transform.SetParent(transform);
        }
    }

    public void StartPatterns(int[] patternIds)
    {
        for (int i = 0; i < patternIds.Length; i++)
        {
            PatternProcesser.AddPattern(EntityManager.Instance().GetNotePatternInfo(patternIds[i]));
        }

        PatternProcesser.StartPatterns();
    }

    public void StopAllPatterns()
    {
        PatternProcesser.StopAllPatterns();
    }

    public Note MakeNote(int id, int index, int delay)
    {
        Note note = ObjectPoolManager.Instance().Get("Note").GetComponent<Note>();
        NoteInfo info = EntityManager.Instance().GetNoteInfo(id);
        //note.LoadSprite(info.SpritePath);

        Vector3 scale = info.SizeType switch
        {
            NoteSizeType.Small => (Vector3.one * 0.8f).WithZ(1f),
            NoteSizeType.Normal => Vector3.one,
            NoteSizeType.Big => (Vector3.one * 1.3f).WithZ(1f),
            _ => Vector3.zero
        };
        note.transform.localScale = scale;

        float cameraDistance = -CameraManager.Instance().MainCamera.transform.position.z;
        Vector3 rightEdgePos = CameraManager.Instance().MainCamera.ViewportToWorldPoint(new Vector3(1f, 0.5f, cameraDistance)).WithY(0f); // 카메라 기준 왼쪽 중앙 -> 월드 포지션 기준 y좌표 0으로 맞추기
        int slotLength = 8;
        float[] slotWidths = new float[slotLength];     // 오른쪽 끝 Index: 0, 왼쪽 끝 Index: 7
        float[] slotPositions = new float[slotLength];  // 오른쪽 끝 Index: 0, 왼쪽 끝 Index: 7
        float currentX = rightEdgePos.x;

        for (int i = 0; i < slotWidths.Length; i++)
        {
            float width = i == index ? note.transform.localScale.x : 1.25f;
            slotWidths[i] = width;
        }

        for (int i = 0; i < slotLength; i++)
        {
            float width = slotWidths[i];
            float halfWidth = width * 0.5f;
            float spacing = 0.5f;

            currentX -= halfWidth;
            slotPositions[i] = currentX;
            currentX -= halfWidth + spacing;
        }

        int reversedIndex = slotLength - index - 1; // 왼쪽 끝 Index가 0이 되도록 반전
        note.transform.position = new Vector3(slotPositions[reversedIndex], rightEdgePos.y, 0f);

        //Vector3 notePos = rightEdgePos.WithX(rightEdgePos.x - (scale.x * 0.5f));
        //note.transform.position = notePos;

        note.Init(info, delay);

        return note;
    }

    public Note[] MakeNotes(int[] ids, int[] indexes, int[] delays)
    {
        Note[] notes = new Note[ids.Length];
        NoteInfo[] noteInfos = new NoteInfo[ids.Length];
        for (int i = 0; i < notes.Length; i++)
        {
            noteInfos[i] = EntityManager.Instance().GetNoteInfo(ids[i]);
        }

        for (int i = 0; i < notes.Length; i++)
        {
            notes[i] = ObjectPoolManager.Instance().Get("Note").GetComponent<Note>();
            //notes[i].LoadSprite(noteInfos[i].SpritePath);

            Vector3 scale = noteInfos[i].SizeType switch
            {
                NoteSizeType.Small => (Vector3.one * 0.8f).WithZ(1f),
                NoteSizeType.Normal => Vector3.one,
                NoteSizeType.Big => (Vector3.one * 1.5f).WithZ(1f),
                _ => Vector3.zero
            };

            notes[i].transform.localScale = scale;
        }

        float cameraDistance = -CameraManager.Instance().MainCamera.transform.position.z;
        // 카메라 기준 왼쪽 중앙, 월드 포지션 기준 y좌표 0으로 맞추기
        Vector3 rightEdgePos = CameraManager.Instance().MainCamera.ViewportToWorldPoint(new Vector3(1f, 0.5f, cameraDistance)).WithY(0f);
        int slotLength = 8;
        float[] slotWidths = new float[slotLength];     // 오른쪽 끝 Index: 0, 왼쪽 끝 Index: 7
        float[] slotPositions = new float[slotLength];  // 오른쪽 끝 Index: 0, 왼쪽 끝 Index: 7
        float currentX = rightEdgePos.x;

        for (int i = 0; i < slotWidths.Length; i++)
        {
            slotWidths[i] = 1.25f;
        }

        for (int i = 0; i < notes.Length && i < indexes.Length; i++)
        {
            int slotIndex = indexes[i];
            slotWidths[slotIndex] = notes[i].transform.localScale.x;
        }

        for (int i = 0; i < slotLength; i++)
        {
            float width = slotWidths[i];
            float halfWidth = width * 0.5f;
            float spacing = 0.5f;

            currentX -= halfWidth;
            slotPositions[i] = currentX;
            currentX -= halfWidth + spacing;
        }

        for (int i = 0; i < notes.Length && i < indexes.Length; i++)
        {
            int slotIndex = indexes[i];
            int reversedIndex = slotLength - slotIndex - 1; // 왼쪽 끝 Index가 0이 되도록 반전
            notes[i].transform.position = new Vector3(slotPositions[reversedIndex], rightEdgePos.y, 0f);
        }

        for (int i = 0; i < notes.Length; i++)
        {
            notes[i].Init(noteInfos[i], delays[i]);
        }

        return notes;
    }
}
