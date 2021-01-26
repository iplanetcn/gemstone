using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Gemstone : MonoBehaviour
{
    public int rowIndex;

    public int columnIndex;

    public ISelectListener selectListener;

    public bool isSelect;

    private List<Sprite> _spriteList;

    private SpriteRenderer _spriteRenderer;

    private int _gemstoneType;

    private GameObject _outline;

    private Vector3 _targetPosition;

    private void Awake()
    {
        isSelect = false;
        selectListener = GameObject.Find("GameController").GetComponent<GameController>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _outline = transform.Find("outline").gameObject;
    }

    private void Start()
    {
        _gemstoneType = Random.Range(0, _spriteList.Count);
        Debug.Log("gemstoneType: " + _gemstoneType);
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
        _targetPosition = new Vector3(columnIndex, rowIndex, 0);
    }

    IEnumerator DelayAndInitialTargetPosition()
    {
        yield return new WaitForSeconds(1);
        _targetPosition = new Vector3(columnIndex, rowIndex , 0);
    }

    private void OnMouseDown()
    {
        selectListener.Select(this);
    }

    public void SetSpriteList(List<Sprite> sprites)
    {
        _spriteList = sprites;
    }
    
    public int GemstoneType() => _gemstoneType;

    /// <summary>
    /// 消除gemstone
    /// </summary>
    public void CrushGemstone()
    {
        StartCoroutine(DestroyGemstone());
    }

    private IEnumerator DestroyGemstone()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}