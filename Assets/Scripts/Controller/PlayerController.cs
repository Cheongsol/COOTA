﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player 클래스를 관리하는 클래스
public class PlayerController : Singleton<PlayerController>
{
    [Header("Player Object")]
    [SerializeField] private Player player;

    [Header("Status")]
    [SerializeField] private bool canMove = true;
    [SerializeField] private bool onGround = true;

    private bool IsInteracting { get { return goInteract != null; } }

    [Header("InteractingObject")]
    [SerializeField] private GameObject goInteract = null;    
    [SerializeField] private Dictionary<string, IEnumerator> dicInteractionCoroutine = new Dictionary<string, IEnumerator>();

    [Header("Component")]
    [SerializeField] private Collider2D playerCollider2D = null;

    public bool CanMove { private set { this.canMove = value; }  get { return canMove; } }

    void Start()
    {
        Initialization();
    }
    void Update()
    {
        UpdateInteraction();
        UpdateMovement();
    }

    private void Initialization() // 초기화
    {
        player = Player.Instance;
        playerCollider2D = GetComponent<Collider2D>();

        canMove = true;
        onGround = true;
    }

    private void UpdateMovement() // 입력을 받아 이동, 점프 명령을 내림
    {
        canMove = !IsInteracting;

        if (!canMove)
        { 
            player.Stop();
            return; 
        }

        Vector2 moveHorizontal = Vector2.zero;

        if (Input.GetKey(Global.KeyLeft))
        {
            moveHorizontal.x = -1.0f;
        }
        else if (Input.GetKey(Global.KeyRight))
        {
            moveHorizontal.x = 1.0f;
        }

        if (onGround && Input.GetKey(Global.KeyJump))
        {
            onGround = false;
            player.Jump();
        }

        player.Move(moveHorizontal);        
    }
    private void UpdateInteraction() // 입력을 받아 상호작용 명령을 내림
    {
        if (Input.GetKeyDown(Global.KeyInteract))
        {
#if MODE_DEBUG
            Debug.Log("Press InterAction Key");
#endif
            DoInteraction();
        }
        else if (Input.GetKeyDown(Global.KeyCancel))
        {
#if MODE_DEBUG
            Debug.Log("Press Cancel Key");
#endif
            DisableInteractionObject();
        }
    }

    private void DoInteraction() // 상호작용 수행
    {        
        if (dicInteractionCoroutine.Count == 0 || IsInteracting) return;

        // 첫 번째 스위치만 작동함
        foreach (var InterActionCoroutine in dicInteractionCoroutine)
        {
            Debug.Log(InterActionCoroutine.Value);
            StartCoroutine(InterActionCoroutine.Value);
            break;
        }
    }    
    public void AddInteraction(string switchName, IEnumerator InterActionCoroutine) // 상호작용 가능한 스위치의 작동범위 내 도달하면 호출됨
    {
        dicInteractionCoroutine.Add(switchName, InterActionCoroutine);
#if MODE_DEBUG
        Debug.Log("Add Interaction: " + switchName);
#endif
    }
    public void RemoveInteraction(string switchName) // 상호작용 가능한 스위치의 작동범위를 벗어나면 호출됨
    {
        dicInteractionCoroutine.Remove(switchName);
#if MODE_DEBUG
        Debug.Log("Remove Interaction: " + switchName);
#endif
    }
    public void EnableInteractionObject(GameObject go) // 상호작용할 오브젝트 활성화 ( 주로 퍼즐 )
    {
        if (IsInteracting) return;

        goInteract = go;
        goInteract.SetActive(true);
#if MODE_DEBUG
        Debug.Log("Start Interaction: " + goInteract.name);
#endif
    }
    private void DisableInteractionObject() // 상호작용할 오브젝트 비활성화 ( 주로 퍼즐 )
    {
        if (!IsInteracting) return;

#if MODE_DEBUG
        Debug.Log("Termiante Interaction: " + goInteract.name);
#endif
        goInteract.SetActive(false);
        goInteract = null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            onGround = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            onGround = false;
        }
    }
}