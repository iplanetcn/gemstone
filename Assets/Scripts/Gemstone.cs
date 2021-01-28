using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Gemstone : MonoBehaviour
{
    private Guid _guid;
        
    public int rowIndex;

    public int columnIndex;

    public ISelectListener selectListener;

    public bool isSelect;

    private List<Sprite> _spriteList;

    private SpriteRenderer _spriteRenderer;

    private int _gemstoneType;

    private GameObject _outline;

    private Vector3 _targetPosition;

    private Vector2 _offset;

    public bool isCrushed;

    private void Awake()
    {
        isSelect = false;
        selectListener = GameObject.Find("GameController").GetComponent<GameController>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _outline = transform.Find("outline").gameObject;
        _guid = Guid.NewGuid();
    }

    private void Start()
    {
        isCrushed = false;
        _gemstoneType = Random.Range(0, _spriteList.Count);
        StartCoroutine(DelayAndInitialTargetPosition());
    }

    private void Update()
    {
        _spriteRenderer.sprite = _spriteList[_gemstoneType];
        _outline.SetActive(isSelect);
        
        if (Math.Abs(Vector3.Distance(_targetPosition, transform.localPosition)) > 0f)
        {
            var diff = Vector3.MoveTowards(transform.localPosition, _targetPosition, 8f * Time.deltaTime);
            transform.localPosition = diff;   
        }
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
        selectListener.Select(this);
    }

    public void SetSpriteList(List<Sprite> sprites)
    {
        _spriteList = sprites;
    }
    
    public int GetGemstoneType() => _gemstoneType;

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
        gameObject.SetActive(false);
    }

    public bool IsSameGuid(Gemstone other)
    {
        return !(other is null) && _guid.Equals(other._guid);
    }
}