using EndlessSpacePilot;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class AuthScore : MonoBehaviour
{
    string url = "https://sid-restapi.onrender.com";
    public string Token { get; set; }

    public string Username { get; set; }

    public Usuario user;
    private void Start()
    {
        //StartCoroutine(GetAllUsersScore());

    }
    private void OnEnable()
    {
        GameoverManager.onGameOver += SetScore;
    }
    private void OnDisable()
    {
        GameoverManager.onGameOver -= SetScore;

    }
    public void SetScore(int score)
    {
        Token = PlayerPrefs.GetString("token");

        if (!string.IsNullOrEmpty(Token))
        {
            var userPatch = new Usuario();
            Username = PlayerPrefs.GetString("username");
            userPatch.username = Username;
            userPatch.data = new UserDataApi();
            userPatch.data.score = score;
            var userJson = JsonUtility.ToJson(userPatch);
            Debug.Log(userJson);

            StartCoroutine(SetUserScore(userJson));

        }
    }


    public IEnumerator SetUserScore(string userJson)
    {

        string token = PlayerPrefs.GetString("token");
        UnityWebRequest request = UnityWebRequest.Put(url + "/api/usuarios", userJson);
        request.method = "PATCH";
        request.SetRequestHeader("x-Token", token);
        request.SetRequestHeader("Content-Type", "application/json");
        Debug.Log("Send Request Score");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            if (request.responseCode == 200)
            {
                Debug.Log("Score Set");
            }
            else
            {
                Debug.Log(request.result);

            }
        }
    }
}
