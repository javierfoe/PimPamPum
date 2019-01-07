
public class PlayerNameInput : MultiplayerLocal.InputField
{
    private const string nameKey = "PlayerName";

    protected override void Awake()
    {
        key = nameKey;
        base.Awake();
    }
}
