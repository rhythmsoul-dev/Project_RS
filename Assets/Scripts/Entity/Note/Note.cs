using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Note : PoolObject
{
    private Rigidbody2D rigid;
    private CapsuleCollider2D capsuleCollider;
    private SpriteRenderer spriteRenderer;
    private TrailRenderer trailRenderer;

    public event Action OnMissed;
    //[SerializeField] private NoteData data;
    public NoteInfo Info { get; private set; }

    public bool IsInitialized { get; private set; }
    private Vector3 initialPosition;

    public bool IsEnded { get; private set; }

    private Sequence sequence;
    private Tween curTween;

    private Coroutine moveCoroutine;

    private int delay;

    public void Init(NoteInfo info, int delay)
    {
        Info = info;
        this.delay = delay;

        StartCoroutine(ProcessInit());
    }

    private IEnumerator ProcessInit()
    {
        //if (Info.Id == 9999)
        //{
        //    spriteRenderer.color = new Color(0.5f, 0.08f, 0.08f);
        //}

        //LoadSprite(sprtie => spriteRenderer.sprite = sprtie);

        trailRenderer.enabled = false;
        spriteRenderer.enabled = false;

        yield return StartCoroutine(LoadSpriteCoroutine());

        capsuleCollider.enabled = true;
        spriteRenderer.enabled = true;

        initialPosition = transform.position;

        switch (Info.SpawnType)
        {
            case NoteSpawnType.Fade:
                spriteRenderer.color = spriteRenderer.color.WithA(0f);
                yield return spriteRenderer.DOFade(1f, 1f);
                break;
            case NoteSpawnType.RightToLeft:
                transform.position = transform.position.WithX(initialPosition.x + 10f);
                yield return transform.DOMoveX(initialPosition.x, 1f);
                break;
            case NoteSpawnType.UpToDown:
                transform.position = transform.position.WithY(initialPosition.y + 10f);
                yield return transform.DOMoveY(initialPosition.y, 1f);
                break;
            case NoteSpawnType.DownToUp:
                transform.position = transform.position.WithY(initialPosition.y - 10f);
                yield return transform.DOMoveY(initialPosition.y, 1f);
                break;
        }

        yield return new WaitForSeconds(delay * 0.001f);

        IsInitialized = true;
        moveCoroutine = StartCoroutine(MoveToTarget());

        yield break;
    }

    public override void OnGet()
    {
        IsEnded = false;
    }

    //public override void OnReleased()
    //{
    //    StopMove();
    //}

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
        //sequence = DOTween.Sequence().SetAutoKill(false).Pause();
    }

    private void Update()
    {
        if (PlayerManager.Instance().LocalPlayer == null || !PlayerManager.Instance().LocalPlayer.IsAlive || CombatSystem.Instance().CurEnemy == null || !CombatSystem.Instance().CurEnemy.IsAlive)
        {
            End();
        }
    }

    //private void OnDestroy()
    //{
    //    End();
    //}

    private IEnumerator MoveToTarget()
    {
        yield return new WaitUntil(() => IsInitialized);
        yield return new WaitForSeconds(0.1f);

        if (Info.TargetPositions == null || Info.TargetPositions.Length == 0)
        {
            yield break;
        }

        trailRenderer.enabled = true;
        switch (Info.MoveType)
        {
            case NoteMoveType.Linear:
                {
                    //float sequenceTime = 0f;
                    //sequence = DOTween.Sequence().SetAutoKill(false);
                    for (int i = 0; i < Info.TargetPositions.Length; i++)
                    {
                        Vector2 startPos = rigid.position;
                        Vector2 targetPos = Info.TargetPositions[i];
                        float duration = Mathf.Max(0.1f, Vector2.Distance(startPos, targetPos) / Info.Speed);

                        // 처음과 마지막에만 감속 적용
                        //Ease ease;
                        //if (Info.TargetPositions.Length == 1)
                        //{
                        //    ease = Ease.InOutSine;
                        //}
                        //else
                        //{
                        //    ease = i switch
                        //    {
                        //        0 => Ease.InSine,
                        //        int l when l == Info.TargetPositions.Length - 1 => Ease.OutSine,
                        //        _ => Ease.Linear
                        //    };
                        //}

                        curTween = DOTween.To(() => 0f, t =>
                        {
                            rigid.MovePosition(Vector2.Lerp(startPos, targetPos, t));
                        }, 1f, duration).SetUpdate(UpdateType.Fixed).SetEase(Ease.InOutSine).SetAutoKill(false);

                        yield return curTween.WaitForCompletion();

                        curTween.Kill();
                        curTween = null;

                        //sequence.Insert(sequenceTime, tween);
                        //sequenceTime += duration;

                        //sequence.Append(tween);
                    }

                    //yield return sequence.WaitForCompletion(); // 왜케 오래 기다리는거임?
                    //yield return new WaitForSeconds(sequenceTime);

                    //sequence.Kill();
                    //sequence = null;
                }
                break;
            case NoteMoveType.Curve:
                {
                    List<BezierSegment> segments = new List<BezierSegment>();
                    Vector2 p1 = rigid.position;

                    for (int i = 0; i < Info.TargetPositions.Length; i++)
                    {
                        Vector2 p3 = Info.TargetPositions[i];
                        Vector2 p2 = (p1 + p3) * 0.5f;

                        BezierSegment segment = new BezierSegment(p1, p2, p3);
                        segments.Add(segment);

                        p1 = p3;
                    }

                    //float sequenceTime = 0f;
                    //sequence = DOTween.Sequence().SetAutoKill(false);
                    for (int i = 0; i < segments.Count; i++)
                    {
                        BezierSegment segment = segments[i];
                        float duration = Mathf.Max(0.1f, segment.Length / Info.Speed);

                        // 처음과 마지막에만 감속 적용
                        Ease ease = i switch
                        {
                            0 => Ease.InCirc,
                            int l when l == segments.Count - 1 => Ease.OutCirc,
                            _ => Ease.Linear
                        };

                        Vector2 startPos = rigid.position;
                        curTween = DOTween.To(() => 0f, t =>
                        {
                            Vector2 bezier = VectorUtility.BezierCurves(segment.P1, segment.P2, segment.P3, t);
                            rigid.MovePosition(Vector2.Lerp(startPos, bezier, t));
                        }, 1f, duration).SetUpdate(UpdateType.Fixed).SetEase(ease).SetAutoKill(false);

                        //sequence.Insert(sequenceTime, tween);
                        //sequenceTime += duration * 0.78f; // wtf

                        yield return new WaitForSeconds(duration * 0.76f); // wtf
                        //yield return curTween.WaitForCompletion();

                        curTween.Kill();
                        curTween = null;
                    }

                    //yield return sequence.WaitForCompletion();

                    //sequence.Kill();
                    //sequence = null;
                }
                break;
        }

        //Miss();
        End();

        yield break;
    }

    public void Miss()
    {
        if (CombatSystem.Instance().CurEnemy != null)
        {
            if (IsExecution())
            {
                int index = UnityEngine.Random.Range(0, 2);
                SoundManager.Instance().Play(GameConstants.Sound.TAKE_DAMAGE_SOUNDS[index]);
                CombatSystem.Instance().CurEnemy.OffGroggy();
            }
            else if (IsEncount())
            {
                int index = UnityEngine.Random.Range(0, 2);
                SoundManager.Instance().Play(GameConstants.Sound.ENCOUNTER_FAILED_SOUNDS[index]);
                //CombatSystem.Instance().CurEnemy.StartAttack();
            }
            else
            {
                int index = UnityEngine.Random.Range(0, 2);
                SoundManager.Instance().Play(GameConstants.Sound.TAKE_DAMAGE_SOUNDS[index]);
            }


            int attackDmg = Mathf.RoundToInt(CombatSystem.Instance().CurEnemy.Stats.AttackPower);
            int balanceDmg = Mathf.RoundToInt(CombatSystem.Instance().CurEnemy.Stats.BalanceAttackPower * 0.5f);
            DamagedSystem.Instance().Send(new Damaged
            {
                AttackDamage = attackDmg,
                BalanceDamage = balanceDmg,
                Attacker = CombatSystem.Instance().CurEnemy.gameObject,
                Victim = PlayerManager.Instance().LocalPlayer.gameObject,
                IgnoreDefense = false
            });
        }

        NoteSystem.Instance().HitZone.ResetCombo();
        End();
    }

    public void End()
    {
        if (IsEnded)
        {
            return;
        }

        IsEnded = true;

        if (IsInitialized)
        {
            StopMove();
        }
        else
        {
            StopAllCoroutines();
        }

        capsuleCollider.enabled = false;
        trailRenderer.enabled = false;
        spriteRenderer.enabled = false;
        spriteRenderer.color = new Color(1f, 1f, 1f);

        ObjectPoolManager.Instance().Get("Effects/Spark", transform.position, transform.rotation);

        transform.position = Vector3.one * 10000f;
        IsInitialized = false;

        Invoke(nameof(Release), 0.5f);
    }


    private IEnumerator ProcessEnd()
    {
        if (IsEnded)
        {
            yield break;
        }

        IsEnded = true;

        if (IsInitialized)
        {
            StopMove();
        }
        else
        {
            StopAllCoroutines();
        }

        capsuleCollider.enabled = false;
        trailRenderer.enabled = false;
        //spriteRenderer.enabled = false;
        //spriteRenderer.color = new Color(1f, 1f, 1f);

        ObjectPoolManager.Instance().Get("Effects/Spark", transform.position, transform.rotation);
        yield return spriteRenderer.DOColor(new Color(1f, 1f, 1f, 0f), 0.5f);

        spriteRenderer.enabled = false;
        spriteRenderer.color = new Color(1f, 1f, 1f);

        transform.position = Vector3.one * 10000f;
        IsInitialized = false;

        yield return new WaitForSeconds(0.5f);
        Release();

        yield break;
    }

    public void StopMove()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }

        if (curTween != null)
        {
            curTween.Kill();
            curTween = null;
        }

        if (sequence != null)
        {
            sequence.Kill();
            sequence = null;
        }

        rigid.MovePosition(rigid.position);
        rigid.linearVelocity = Vector3.zero;
    }

    public void OnParried()
    {
        StartCoroutine(ProcessParried());
    }

    private IEnumerator ProcessParried()
    {
        StopMove();
        yield return new WaitForFixedUpdate();

        float randomAngle = UnityEngine.Random.Range(-30f, 30f);
        Vector2 finalDirection = Quaternion.Euler(0f, 0f, randomAngle) * transform.right;

        rigid.AddForce(10f * Info.Speed * finalDirection.normalized, ForceMode2D.Impulse);

        yield return new WaitForSeconds(2.5f);
        End();

        yield break;
    }

    public bool IsExecution()
    {
        return Info.Id == 9999;
    }

    public bool IsEncount()
    {
        return Info.Id >= 9900 && Info.Id <= 9998;
    }

    private IEnumerator LoadSpriteCoroutine()
    {
        AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(Info.SpritePath.Trim());

        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            spriteRenderer.sprite = handle.Result;
        }
        else
        {
            Debug.LogErrorFormat("[Addressable] Cannot Found Path: {0}", Info.SpritePath.Trim());
        }

        Addressables.Release(handle);
    }
    //private void LoadSprite(Action<Sprite> onLoaded = null)
    //{
    //    Addressables.LoadAssetAsync<Sprite>(Info.SpritePath).Completed += (handle) =>
    //    {
    //        if (handle.Status == AsyncOperationStatus.Succeeded)
    //        {
    //            onLoaded?.Invoke(handle.Result);
    //        }
    //        else
    //        {
    //            Debug.LogErrorFormat("[Addressable] Cannot Found Path: {0}", Info.SpritePath);
    //        }
    //    };
    //}

    //public void LoadSprite(string path)
    //{
    //    spriteRenderer.enabled = false;

    //    path = path.Trim();
    //    Addressables.LoadAssetAsync<Sprite>(path).Completed += (handle) =>
    //    {
    //        if (handle.Status == AsyncOperationStatus.Succeeded)
    //        {
    //            spriteRenderer.sprite = handle.Result;
    //        }
    //        else
    //        {
    //            Debug.LogErrorFormat("[Addressable] Cannot Found Path: {0}", path);
    //        }

    //        spriteRenderer.enabled = true;
    //    };
    //}

    //public void GetParried()
    //{
    //    sequence.Kill();
    //    sequence = null;
    //    rigid.linearVelocity = Vector3.zero;
    //    float randomAngle = Random.Range(-30f, 30f);
    //    Vector2 finalDirection = Quaternion.Euler(0, 0, randomAngle) * transform.right;
    //    rigid.AddForce(finalDirection.normalized * 50f, ForceMode2D.Impulse);
    //}

    //private void Move()
    //{
    //    if (curTargetPosition == null)
    //    {
    //        return;
    //    }

    //    switch (Info.MoveType)
    //    {
    //        case NoteMoveType.Linear:
    //            Vector2 dir = (curTargetPosition - rigid.position).normalized;
    //            rigid.AddForce(dir * Info.Speed, ForceMode2D.Force);
    //            break;
    //        case NoteMoveType.Curve:
    //            rigid.MovePosition(curTargetPosition);
    //            break;
    //    }

    //    //Vector2 dir = (curTargetPosition - rigid.position).normalized;
    //    //rigid.AddForce(dir * Info.Speed, ForceMode2D.Force);
    //    //rigid.MovePosition(Vector2.Lerp(rigid.position, curTargetPosition, Time.deltaTime * Info.Speed));
    //}
}
