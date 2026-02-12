public static class Bus<T> where T : IEvent
{
    public delegate void Event(T args);
    public static event Event OnEvent;
    public static void CallEvent(T _event) => OnEvent?.Invoke(_event);

}
