using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginReqest
{
    public string email;
    public string password;

    public LoginReqest(string _email, string _password)
    {
        email = _email;
        password = _password;
    }
}