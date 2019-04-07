namespace PimPamPum
{
    public class StartHostButton : NetworkManagerButton
    {
        protected override void NetworkManagerAction()
        {
            networkManager.StartHost();
        }
    }
}