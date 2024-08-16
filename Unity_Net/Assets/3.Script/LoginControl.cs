using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginControl : MonoBehaviour
{
    public InputField id_input;
    public InputField Pass_input;

    [SerializeField]
    private Text log;

    private ServerChecker checker;

    private void Start()
    {
        checker = FindObjectOfType<ServerChecker>();
    }


    public void Login_btn()
    {
        if(id_input.text.Equals(string.Empty) || Pass_input.text.Equals(string.Empty))
        {
            log.text = "���̵�� ��й�ȣ�� �Է����ּ���.";
            return;
        }

        if (SQLManager.instance.Login(id_input.text, Pass_input.text))
        {
            // �α��� ����

            UserInfo info = SQLManager.instance.info;
            Debug.Log($"{info.UserName} | {info.User_Password} | {info.User_PhoneNum}");

            checker.Start_Client();
            transform.parent.gameObject.SetActive(false);
        }
        else
        {
            // �α��� ����
            log.text = "���̵�� ��й�ȣ�� Ȯ�����ּ���.";

        }
    }
}
