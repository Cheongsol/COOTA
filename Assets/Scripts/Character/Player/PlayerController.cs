﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player 조작을 관리하는 클래스
[RequireComponent(typeof(Player))]
public class PlayerController : Singleton<PlayerController>
{
    [Header("Character")]
    [SerializeField] private Player player;
 
    private bool IsInteracting { get { return goInteract != null; } }
    
    [Header("InteractObject")]
    [SerializeField] private GameObject goInteract = null;    
    [SerializeField] private Dictionary<string, Action> dicInteractionDelegate = new Dictionary<string, Action>();

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
        player = GetComponent<Player>();
        dicInteractionDelegate.Clear();

        DontDestroyOnLoad(this.gameObject);
    }

    private void UpdateMovement() // 입력을 받아 이동, 점프 명령을 내림
    {
        if (IsInteracting || !player.CanMove)
        {
            player.Stop();
            return;
        }

        Vector2 moveVector = Vector2.zero;

        if (Input.GetKey(Global.KeyRight))
        {
            moveVector.x = 1.0f;
        }        
        else if (Input.GetKey(Global.KeyLeft))
        {
            moveVector.x = -1.0f;
        }
        else if (Input.GetKey(Global.KeyUp))
        {
            moveVector.y = 1.0f;
        }
        else if (Input.GetKey(Global.KeyDown))
        {
            moveVector.y = -1.0f;
        }

        if (Input.GetKey(Global.KeyJump))
        {
            player.Jump();
        }

        player.Move(moveVector);
    }
    private void UpdateInteraction() // 입력을 받아 상호작용 명령을 내림
    {
        if (!player.IsAlive)
        {
            DisableInteractionObject();
            return;
        }

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

    private void DoInteraction() // 오브젝트 상호작용
    {
        if (dicInteractionDelegate.Count == 0 || IsInteracting) return;

        // 첫 번째 스위치만 작동함
        foreach (var InterActionCoroutine in dicInteractionDelegate)
        {
            InterActionCoroutine.Value();
            break;
        }
    }
    public void AddInteraction(string switchName, Action InterActionCoroutine) // 상호작용 가능한 스위치의 작동범위 내 도달하면 호출됨
    {
        dicInteractionDelegate.Add(switchName, InterActionCoroutine);
#if MODE_DEBUG
        Debug.Log("Add Interaction: " + switchName);
#endif
    }
    public void RemoveInteraction(string switchName) // 상호작용 가능한 스위치의 작동범위를 벗어나면 호출됨
    {
        dicInteractionDelegate.Remove(switchName);
#if MODE_DEBUG
        Debug.Log("Remove Interaction: " + switchName);
#endif
    }
    public void EnableInteractionObject(GameObject go) // 상호작용할 오브젝트 활성화
    {
        if (IsInteracting) return;

        goInteract = go;
        goInteract.SetActive(true);
#if MODE_DEBUG
        Debug.Log("Start Interaction: " + goInteract.name);
#endif
    }
    private void DisableInteractionObject() // 상호작용할 오브젝트 비활성화
    {
        if (!IsInteracting) return;

#if MODE_DEBUG
        Debug.Log("Termiante Interaction: " + goInteract.name);
#endif
        goInteract.SetActive(false);
        goInteract = null;
    }
}
