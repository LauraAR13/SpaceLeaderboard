using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class AuthHandler : MonoBehaviour
{
    [SerializeField] TMP_Text usernameLabel;

    string url = "https://sid-restapi.onrender.com";
    public string Token { get; set; }

    public string Username { get; set; }

    public GameObject PanelAuth;

    public UnityEvent OnLogin;

    void Start()
    {
        Token = PlayerPrefs.GetString("token");

        if (string.IsNullOrEmpty(Token))
        {
            Debug.Log("There is no token");
            PanelAuth.SetActive(true);
        }
        else
        {
            Username = PlayerPrefs.GetString("username");
            Debug.Log(Username);
            StartCoroutine("GetProfile");
        }
    }

    public void SendRegistration()
    {
        AuthData data = new AuthData();
        data.username = GameObject.Find("InputFieldUsername").GetComponent<TMP_InputField>().text;
        data.password = GameObject.Find("InputFieldPassword").GetComponent<TMP_InputField>().text;
        StartCoroutine("SignUp", JsonUtility.ToJson(data));
    }

    public void SendLogin()
    {
        AuthData data = new AuthData();
        data.username = GameObject.Find("InputFieldUsername").GetComponent<TMP_InputField>().text;
        data.password = GameObject.Find("InputFieldPassword").GetComponent<TMP_InputField>().text;

        StartCoroutine("Login", JsonUtility.ToJson(data));
    }

    IEnumerator SignUp(string json)
    {
        UnityWebRequest request = UnityWebRequest.Put(url + "/api/usuarios", json);
        request.method = UnityWebRequest.kHttpVerbPOST;
        request.SetRequestHeader("Content-Type", "application/json");
        Debug.Log("Send Request SignUp");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log(request.downloadHandler.text);
            Debug.Log(request.GetResponseHeader("Content-Type"));

            if (request.responseCode == 200)
            {
                Debug.Log("Signup Successful");
                StartCoroutine("Login", json);

            }
            else
            {

                Debug.Log(request.responseCode + "|" + request.error);
                PanelAuth.SetActive(true);
            }
        }
    }

    IEnumerator Login(string json)
    {
        UnityWebRequest request = UnityWebRequest.Put(url + "/api/auth/login", json);
        request.method = UnityWebRequest.kHttpVerbPOST;
        request.SetRequestHeader("Content-Type", "application/json");
        Debug.Log("Send Request Login");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log(request.downloadHandler.text);
            Debug.Log(request.GetResponseHeader("Content-Type"));

            if (request.responseCode == 200)
            {
                OnLogin?.Invoke();

                AuthData data = JsonUtility.FromJson<AuthData>(request.downloadHandler.text);

                Username = data.usuario.username;
                Token = data.token;

                PlayerPrefs.SetString("token", Token);
                PlayerPrefs.SetString("username", Username);
                usernameLabel.text = usernameLabel.text.Replace("@username", Username);
            }
            else
            {
                Debug.Log(request.responseCode + "|" + request.error);
            }
        }
    }

    IEnumerator GetProfile()
    {
        UnityWebRequest request = UnityWebRequest.Get(url + "/api/usuarios/" + Username);
        request.SetRequestHeader("x-Token", Token);

        Debug.Log("Send Request GetProfile");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {


            if (request.responseCode == 200)
            {
                Debug.Log("GetProfile Successful");

                AuthData data = JsonUtility.FromJson<AuthData>(request.downloadHandler.text);
                Username = data.usuario.username;
                OnLogin?.Invoke();
                usernameLabel.text = usernameLabel.text.Replace("@username", Username);

            }
            else
            {
                Debug.Log(request.responseCode + "|" + request.error);

            }
        }


    }

    public void LogOut()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(0);
    }
}

[System.Serializable]

public class AuthData
{
    public string username;
    public string password;
    public Usuario usuario;
    public string token;
}

[System.Serializable]
public class Usuario
{
    public string _id;
    public string username;
    public UserDataApi data;

}
[System.Serializable]

public class UserDataApi
{
    public int score;
    public string appID;
}

[System.Serializable]

public class LeaderboardData
{
    public Usuario[] usuarios;
}