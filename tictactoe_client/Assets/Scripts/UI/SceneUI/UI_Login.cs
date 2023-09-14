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

    public void PressLogInButton()
    {
        byte[] byteData = Util.ClassToByte<LoginReqest>(new LoginReqest(_emailInputField.text, _passwordInputField.text), Define.ClientMsg.Login);

        Debug.Log(byteData);

        _emailInputField.text = "";
        _passwordInputField.text = "";
    }

    public void PressSignInButton()
    {

    }
}
