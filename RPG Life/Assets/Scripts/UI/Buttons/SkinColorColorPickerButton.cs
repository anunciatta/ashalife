

public class SkinColorColorPickerButton : GenericColorPickerButton
{
    public override void OnButtonPressed()
    {
        Bus<SkinColorPickedEvent>.CallEvent(new SkinColorPickedEvent(colorIndex));
    }
}





