using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace MyProject
{
	public class DatabaseUIManager : MonoBehaviour
	{
		public GameObject loginPanel;
		public GameObject registerPanel;
		public GameObject errorPanel;

		[Space(10)]
		public InputField emailInput;
		public InputField pwInput;
		public Button signUpButton;
		public Button loginButton;
		public Button registerButton;
		

		[Space(10)]
		public InputField register_EmailInput;
		public InputField register_PWInput;
		public InputField register_ClassInput;

		[Space(10)]
		public Button register_Register_Button;
		public Button register_Cancel_Button;

		[Space(10)]
		public Text errorText;
		public Button error_Cancel_Button;



		private UserData userData;

        private void Awake()
        {
			loginButton.onClick.AddListener(LoginButtonClick);
			registerButton.onClick.AddListener(RegisterButtonClick);

			register_Register_Button.onClick.AddListener(OnRegisterRegisterButtonClick);
			register_Cancel_Button.onClick.AddListener(OnCancelButtonClick);

			error_Cancel_Button.onClick.AddListener(ErrorCancelButtonClick);

        }

		public void ErrorCancelButtonClick()
        {
			//if (loginPanel.activeSelf)
			//{
			//	loginPanel.SetActive(false);
			//}
			//else if (registerPanel.activeSelf)
			//         {
			//	registerPanel.SetActive(false);
			//         }

			errorPanel.SetActive(false);
			loginPanel.SetActive(true);

		}

		public void ErrorPanelOn()
        {
			loginPanel.SetActive(false);
			errorPanel.SetActive(true);
		}




		public void RegisterButtonClick()
        {
			loginPanel.SetActive(false);
			registerPanel.SetActive(true);
        }

		private void OnCancelButtonClick()
		{
			registerPanel.SetActive(false);
			loginPanel.SetActive(true);
		}

		
		private void OnRegisterRegisterButtonClick()
		{
			DatabaseManager.Instance.Register(register_EmailInput.text, register_PWInput.text, (CharClass)int.Parse(register_ClassInput.text), OnRegisterSuccess, OnRegisterFailure);
		}

		public void LoginButtonClick()
        {
			DatabaseManager.Instance.Login(emailInput.text, pwInput.text, OnLoginSuccess, OnLoginFailure);
        }

		
		private void OnLoginSuccess(UserData data)
        {
			
			

			if (data.is_online)
            {
				ErrorPanelOn();
            }
			else
            {
				print("로그인 성공");

				DataManager.Instance.LoadData(data);

				SceneManager.LoadScene("Playground");
			}
		}

		private void OnLoginFailure()
		{
			print("로그인 실패");
		}

		private void OnRegisterSuccess()
        {
			print("회원가입 성공");
        }

		private void OnRegisterFailure()
        {
			print("회원가입 실패");
		}

	}
}
