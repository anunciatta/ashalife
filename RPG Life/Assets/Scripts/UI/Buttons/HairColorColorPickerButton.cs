public class HairColorColorPickerButton : GenericColorPickerButton
{
    public override void OnButtonPressed()
    {
        Bus<HairColorPickedEvent>.CallEvent(new HairColorPickedEvent(colorIndex));
    }
}

