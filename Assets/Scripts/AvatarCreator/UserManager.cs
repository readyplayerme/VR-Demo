using System.Threading.Tasks;
using ReadyPlayerMe.AvatarCreator;
using UnityEngine;

namespace ReadyPlayerMe.XR
{
    public class UserManager : MonoBehaviour
    {
        private const string STORED_SESSION_KEY = "RPM_UserSession";

        private async void Start()
        {
            await LoginUser();
        }

        private void OnApplicationQuit()
        {
            PlayerPrefs.SetString(STORED_SESSION_KEY, JsonUtility.ToJson(AuthManager.UserSession));
        }

        private async Task LoginUser()
        {
            if (PlayerPrefs.HasKey(STORED_SESSION_KEY))
            {
                Debug.Log("Load User from PlayerPrefs");
                AuthManager.SetUser(JsonUtility.FromJson<UserSession>(PlayerPrefs.GetString(STORED_SESSION_KEY)));
            }
            else
            {
                Debug.Log("Create new User");
                await AuthManager.LoginAsAnonymous();
            }
        }
    }
}