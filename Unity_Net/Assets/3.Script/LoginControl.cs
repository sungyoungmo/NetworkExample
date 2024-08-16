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
            log.text = "아이디와 비밀번호를 입력해주세요.";
            return;
        }

        if (SQLManager.instance.Login(id_input.text, Pass_input.text))
        {
            // 로그인 성공

            UserInfo info = SQLManager.instance.info;
            Debug.Log($"{info.UserName} | {info.User_Password} | {info.User_PhoneNum}");

            checker.Start_Client();
            transform.parent.gameObject.SetActive(false);
        }
        else
        {
            // 로그인 실패
            log.text = "아이디와 비밀번호를 확인해주세요.";

        }
    }
}
