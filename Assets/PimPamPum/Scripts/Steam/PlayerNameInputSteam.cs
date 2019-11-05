using Steamworks;

namespace PimPamPum
{
    public class PlayerNameInputSteam : PlayerNameInput
    {
        protected override void Start()
        {
            if (SteamManager.Initialized)
            {
                Input.text = SteamFriends.GetPersonaName();
            }
        }
    }
}