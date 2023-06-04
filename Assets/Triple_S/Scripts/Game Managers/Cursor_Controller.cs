using UnityEngine;

public class Cursor_Controller : MonoBehaviour
{
    [SerializeField] private Texture2D cursorHover;
    [SerializeField] private Texture2D cursorClick;
    [SerializeField] private Texture2D cursorDefault;

    private void Start()
    {
        Cursor.SetCursor(cursorDefault, Vector2.zero, CursorMode.Auto);
    }

    private void AudioButtonHover()
    {
        SFX_Controller.instance.PlaySound(SFX_Controller.SoundType.ButtonHover);
    }

    private void AudioButtonClick()
    {
        SFX_Controller.instance.PlaySound(SFX_Controller.SoundType.ButtonClick);
    }

    // Cuando el ratón pasa por encima del boton
    private void PointerEnter()
    {
        Cursor.SetCursor(cursorHover, Vector2.zero, CursorMode.Auto);
    }

    // Cuando el ratón sale del botón
    private void PointerExit()
    {
        Cursor.SetCursor(cursorDefault, Vector2.zero, CursorMode.Auto);
    }

    // Cuando se presiona click en el boton
    private void PointerUp()
    {
        Cursor.SetCursor(cursorDefault, Vector2.zero, CursorMode.Auto);
    }

    // Cuando se suelta click en el boton
    private void PointerDown()
    {
        Cursor.SetCursor(cursorClick, Vector2.zero, CursorMode.Auto);
    }
}
