using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class Leaderboard : MonoBehaviour
{
    string url = "https://sid-restapi.onrender.com";
    public GameObject[] positions;
    private bool state = false;
    [SerializeField] GameObject panel;

    public void ShowLeaderboard(bool state)
    {
        if (state)
        {
            StartCoroutine(GetAllUsersScore());
        }
        else
        {
            panel.SetActive(false);

        }

    }
    public IEnumerator GetAllUsersScore()
    {
        string Token = PlayerPrefs.GetString("token");

        UnityWebRequest requestAllUsers = UnityWebRequest.Get(url + "/api/usuarios/");
        requestAllUsers.SetRequestHeader("x-Token", Token);

        yield return requestAllUsers.SendWebRequest();

        if (requestAllUsers.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(requestAllUsers.error);
        }
        else
        {


            if (requestAllUsers.responseCode == 200)
            {
                Debug.Log("GetProfile Successful");
                LeaderboardData leaderboard = JsonUtility.FromJson<LeaderboardData>(requestAllUsers.downloadHandler.text);


                foreach (var item in leaderboard.usuarios)
                {
                    if (item.data == null)
                    {
                        item.data.score = 0;
                    }
                }
                Usuario[] usuariosOrganizados = leaderboard.usuarios.OrderByDescending(user => user.data?.score).Take(10).ToArray();

                for (int i = 0; i < usuariosOrganizados.Length; i++)
                {
                    if (i < positions.Length)
                    {
                        positions[i].gameObject.SetActive(true);
                        positions[i].GetComponent<TMP_Text>().text = $"Username: {usuariosOrganizados[i].username} Score: {usuariosOrganizados[i].data.score}";

                        Debug.Log(usuariosOrganizados[i].username);
                    }

                }
                panel.SetActive(true);
            }
            else
            {
                Debug.Log(requestAllUsers.responseCode + "|" + requestAllUsers.error);

            }
        }
    }
}
