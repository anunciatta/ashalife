public class Eyebrows : GenericCharacterPart
{
    public override void Initialize()
    {
        base.Initialize();
        Bus<EyebrowsSpriteChangeEvent>.OnEvent += ChangeSprite;
    }

    private void OnDestroy()
    {
        Bus<EyebrowsSpriteChangeEvent>.OnEvent -= ChangeSprite;
    }

    public void ChangeSprite(EyebrowsSpriteChangeEvent _event)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = _event.Icon;
        }
    }


}
