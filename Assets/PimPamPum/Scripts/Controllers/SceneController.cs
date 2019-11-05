using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PimPamPum
{
    public class SceneController : MonoBehaviour
    {
        [Scene] [SerializeField] private string localMultiplayer = "";
        [Scene] [SerializeField] private string onlineMultiplayer = "";

        public void LoadLocalMultiplayer()
        {
            SceneManager.LoadScene(localMultiplayer);
        }

        public void LoadOnlineMultiplayer()
        {
            SceneManager.LoadScene(onlineMultiplayer);
        }
    }
}
