using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class UI_Login : MonoBehaviour
{
    [SerializeField]
    InputField _emailInputField;
    [SerializeField]
    InputField _passwordInputField;

    [SerializeField]
    Button _loginButton;
    [SerializeField]
    Button _signinButton;

    string _enteredEmail;
    string _enteredPassword;

    private void Awake()
    {
        // 버튼들에게 이벤트 자동 바인드
        _loginButton.onClick.RemoveAllListeners();
        _signinButton.onClick.RemoveAllListeners();
        _loginButton.onClick.AddListener(PressLogInButton);
        _signinButton.onClick.AddListener(PressSignInButton);
    }

    // 로그인 버튼 누를 때 실행할 함수
    public void PressLogInButton()
    {
        // 로그인 정보 메세지팩으로 제작
        byte[] byteData = Util.ClassToByte<LoginReqest>(new LoginReqest()
                                                        {   
                                                            email = _emailInputField.text,
                                                            password = _passwordInputField.text 
                                                        }, 
                                                        Define.ClientMsg.Login);
        // 인풋 필드 비우기
        _emailInputField.text = "";
        _passwordInputField.text = "";

        // 메세지팩 보내기
        ServerManager.GetInst().Send(byteData);
    }

    // 회원가입 버튼 누를 때 실행할 함수
    public void PressSignInButton()
    {

    }
}
