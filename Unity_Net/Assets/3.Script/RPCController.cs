using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class RPCController : NetworkBehaviour
{
    [SerializeField]
    private TMP_Text chat_Text;

    [SerializeField]
    private TMP_InputField input;

    [SerializeField]
    private GameObject Chat_UI;

    private static event Action<string> onMessage;

    public override void OnStartAuthority()
    {
        // Client 가 Server에 Connect 되었을 때 불러와지는 콜백 함수

        if (isLocalPlayer)
        {
            Chat_UI.SetActive(true);
        }

        onMessage += NewMessage;
    }

    private void NewMessage(string m)
    {
        chat_Text.text += m;
    }

    [ClientCallback]    // 클라이언트가 서버를 나갔을 떄
    private void OnDestroy()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        onMessage -= NewMessage;

    }


    // Client RPC 보내는 방법
    // [ClientRPC] 명령어 -> [Command] 명령어(서버) -> [client] 명령어

    [Client] // 실제 클라이언트가 행동할 메소드
    public void Send()
    {
        if (!Input.GetKeyDown(KeyCode.Return))
        {
            return;
        }

        if (string.IsNullOrEmpty(input.text))
        {
            return;
        }

        cmd_SendMessage(input.text);
        input.text = string.Empty;
    }


    [Command] // 실제 서버가 메소드를 실행하기 위한 메소드
    private void cmd_SendMessage(string message)
    {
        RPCHandle_Message($"[{connectionToClient.connectionId}] : {message}");
    }

    [ClientRpc] // 서버한테 어떠한 메소드를 실행하라고 할 때 말하는 메소드
    private void RPCHandle_Message(string m)
    {
        onMessage?.Invoke($"\n{m}");
    }


}
