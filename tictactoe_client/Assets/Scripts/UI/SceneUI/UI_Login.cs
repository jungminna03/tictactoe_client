using System.Collections;
using System.Collections.Generic;
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
        _enteredEmail = _emailInputField.text;
        _enteredPassword = _passwordInputField.text;

        Debug.Log(_enteredEmail);
        Debug.Log(_enteredPassword);
    }

    public void PressSignInButton()
    {

    }
}
