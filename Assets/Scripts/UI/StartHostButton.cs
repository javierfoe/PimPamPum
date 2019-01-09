namespace Bang
{
    public class StartHostButton : NetworkManagerButton
    {
        protected override void NetworkManagerAction()
        {
            networkManager.StartHost();
        }
    }
}