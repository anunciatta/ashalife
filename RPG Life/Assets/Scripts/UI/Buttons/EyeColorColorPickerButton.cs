public class EyeColorColorPickerButton : GenericColorPickerButton
{
    public override void OnButtonPressed()
    {
        Bus<EyesColorPickedEvent>.CallEvent(new EyesColorPickedEvent(colorIndex));
    }
}
