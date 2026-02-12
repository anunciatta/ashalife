public class ConnectionFailurePanel : Panel
{

    public override void Initialize()
    {
        base.Initialize();
        Bus<ConnectionFailureEvent>.OnEvent += OnConnectionFailure;

    }

    void OnDestroy()
    {
        Bus<ConnectionFailureEvent>.OnEvent -= OnConnectionFailure;
    }

    private void OnConnectionFailure(ConnectionFailureEvent _event)
    {
        //Debug.LogWarning(_event.Message);
        panel.SetActive(true);
    }
}
