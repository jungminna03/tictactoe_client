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

    private void Awake()
    {
        _loginButton.onClick.RemoveAllListeners();
        _signinButton.onClick.RemoveAllListeners();
        _loginButton.onClick.AddListener(PressLogInButton);
        _signinButton.onClick.AddListener(PressSignInButton);
    }

    public void PressLogInButton()
    {
        if (Util.CheckEmailValid(_emailInputField.text))
        {
            byte[] byteData = Util.ClassToByte<LoginReqest>(new LoginReqest()
            {
                email = _emailInputField.text,
                password = _passwordInputField.text
            },
                                                    Define.ClientMsg.Login);

            ServerManager.GetInst().Send(byteData);
        }
        else
        {
            Debug.Log($"{_emailInputField.text} was not valied");

            // TODO : °æ°í UI ¶ç¿ì±â 
        }

        _emailInputField.text = "";
        _passwordInputField.text = "";
    }

    public void PressSignInButton()
    {

    }
}
