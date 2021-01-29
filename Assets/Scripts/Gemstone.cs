using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Gemstone : MonoBehaviour
{
    public int rowIndex;
    public int columnIndex;
    public GameController gameController;
    public bool isSelect;
    public bool isCrushed;
    public int gemstoneType;

    private Guid _guid;
    private GameObject _outline;
    private Vector3 _targetPosition;
    private Vector2 _offset;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        isSelect = false;
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        _outline = transform.Find("outline").gameObject;
        _guid = Guid.NewGuid();
    }

    private void Start()
    {
        isCrushed = false;
        StartCoroutine(DelayAndInitialTargetPosition());
    }

    private void Update()
    {
        _spriteRenderer.sprite = gameController.spriteList[gemstoneType];
        _outline.SetActive(isSelect);

        if (Math.Abs(Vector3.Distance(_targetPosition, transform.localPosition)) > 0f)
        {
            var diff = Vector3.MoveTowards(transform.localPosition, _targetPosition, 8f * Time.deltaTime);
            transform.localPosition = diff;
        }

        if (isCrushed)
        {
            gameObject.SetActive(false);
        }
    }

    public void ActiveWithGemstoneInfo(GemstoneInfo gi)
    {
        gemstoneType = gi.GemstoneType;
        transform.position = gi.Position;
        gameObject.SetActive(true);
        isCrushed = false;
    }

    public void Move()
    {
        _targetPosition = new Vector3(columnIndex - _offset.x, rowIndex - _offset.y, 0);
    }

    private IEnumerator DelayAndInitialTargetPosition()
    {
        yield return new WaitForSeconds(1);
        Move();
    }

    private void OnMouseDown()
    {
        gameController.Select(this);
    }

    public Guid GetGuid() => _guid;

    public void SetOffset(Vector2 offset) => _offset = offset;

    /// <summary>
    /// 消除gemstone
    /// </summary>
    public void CrushGemstone()
    {
        StartCoroutine(DoCrushGemstone());
    }

    private IEnumerator DoCrushGemstone()
    {
        yield return new WaitForSeconds(0.5f);
        isCrushed = true;
    }
}