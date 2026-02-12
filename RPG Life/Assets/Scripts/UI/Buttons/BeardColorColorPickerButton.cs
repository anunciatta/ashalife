public class BeardColorColorPickerButton : GenericColorPickerButton
{
    public override void OnButtonPressed()
    {
        Bus<BeardColorPickedEvent>.CallEvent(new BeardColorPickedEvent(colorIndex));
    }
}



