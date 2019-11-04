using Steamworks;
using UnityEngine;

namespace PimPamPum
{
    public class LobbyController : MonoBehaviour
    {
        public const string GAME_ID = "pimpampum";
        public static LobbyController Instance;

        private Callback<LobbyEnter_t> m_LobbyEntered;
        private Callback<GameLobbyJoinRequested_t> m_GameLobbyJoinRequested;
        private Callback<LobbyChatUpdate_t> m_LobbyChatUpdate;
        private CallResult<LobbyMatchList_t> m_LobbyMatchList;
        private CSteamID steamLobbyId;

        private void Start()
        {
            if (SteamManager.Initialized)
            {
                Instance = this;
                m_LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
                m_LobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
                m_LobbyMatchList = CallResult<LobbyMatchList_t>.Create(OnLobbyMatchList);
            }
        }

        public void CreateLobby(int players)
        {
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, players);
            Debug.Log("Lobby Created: " + players);
        }

        void OnLobbyChatUpdate(LobbyChatUpdate_t pCallback)
        {

        }

        public void JoinLobby(CSteamID lobbyId)
        {
            SteamMatchmaking.JoinLobby(lobbyId);
        }

        public void FindMatch()
        {
            SteamMatchmaking.AddRequestLobbyListStringFilter("game", GAME_ID, ELobbyComparison.k_ELobbyComparisonEqual);
            var call = SteamMatchmaking.RequestLobbyList();
            m_LobbyMatchList.Set(call, OnLobbyMatchList);
        }

        void OnLobbyMatchList(LobbyMatchList_t pCallback, bool bIOFailure)
        {
            uint numLobbies = pCallback.m_nLobbiesMatching;
            Debug.Log("Available lobbies: " + numLobbies);
            if (numLobbies > 0)
            {
                Debug.Log("Joining lobby");
                CSteamID lobby = SteamMatchmaking.GetLobbyByIndex(0);
                JoinLobby(lobby);
            }
        }

        void OnLobbyEntered(LobbyEnter_t pCallback)
        {
            Debug.Log("Lobby Joined");
            steamLobbyId = new CSteamID(pCallback.m_ulSteamIDLobby);
            CSteamID hostUserId = SteamMatchmaking.GetLobbyOwner(steamLobbyId);
            CSteamID me = SteamUser.GetSteamID();
            Debug.Log(hostUserId + " " + me);
            if (hostUserId.Equals(me))
            {
                SteamMatchmaking.SetLobbyData(steamLobbyId, "game", GAME_ID);
                Debug.Log("Set GameID");
            }
            else
            {
                NetworkManager.singleton.networkAddress = hostUserId.m_SteamID.ToString();
                NetworkManager.singleton.StartClient();
            }
        }
    }
}