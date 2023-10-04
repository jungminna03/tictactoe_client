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
        // ��ư�鿡�� �̺�Ʈ �ڵ� ���ε�
        _loginButton.onClick.RemoveAllListeners();
        _signinButton.onClick.RemoveAllListeners();
        _loginButton.onClick.AddListener(PressLogInButton);
        _signinButton.onClick.AddListener(PressSignInButton);
    }

    // �α��� ��ư ���� �� ������ �Լ�
    public void PressLogInButton()
    {
        // �α��� ���� �޼��������� ����
        byte[] byteData = Util.ClassToByte<LoginReqest>(new LoginReqest()
                                                        {   
                                                            email = _emailInputField.text,
                                                            password = _passwordInputField.text 
                                                        }, 
                                                        Define.ClientMsg.Login);
        // ��ǲ �ʵ� ����
        _emailInputField.text = "";
        _passwordInputField.text = "";

        // �޼����� ������
        ServerManager.GetInst().Send(byteData);
    }

    // ȸ������ ��ư ���� �� ������ �Լ�
    public void PressSignInButton()
    {

    }
}
