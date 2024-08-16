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
        // Client �� Server�� Connect �Ǿ��� �� �ҷ������� �ݹ� �Լ�

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

    [ClientCallback]    // Ŭ���̾�Ʈ�� ������ ������ ��
    private void OnDestroy()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        onMessage -= NewMessage;

    }


    // Client RPC ������ ���
    // [ClientRPC] ��ɾ� -> [Command] ��ɾ�(����) -> [client] ��ɾ�

    [Client] // ���� Ŭ���̾�Ʈ�� �ൿ�� �޼ҵ�
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


    [Command] // ���� ������ �޼ҵ带 �����ϱ� ���� �޼ҵ�
    private void cmd_SendMessage(string message)
    {
        RPCHandle_Message($"[{connectionToClient.connectionId}] : {message}");
    }

    [ClientRpc] // �������� ��� �޼ҵ带 �����϶�� �� �� ���ϴ� �޼ҵ�
    private void RPCHandle_Message(string m)
    {
        onMessage?.Invoke($"\n{m}");
    }


}
