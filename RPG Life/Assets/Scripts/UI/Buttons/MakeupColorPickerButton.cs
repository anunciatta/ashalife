public class MakeupColorPickerButton : GenericColorPickerButton
{
    public override void OnButtonPressed()
    {
        Bus<MakeupColorPickedEvent>.CallEvent(new MakeupColorPickedEvent(colorIndex));
    }
}

