using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FlashScreen : MonoBehaviour
{
    public Image flash_overlay;


	public void Flash(Color color)
    {
        flash_overlay.color = color;
        flash_overlay.enabled = true;
        StartCoroutine("RemoveOverlay");
    }
    public IEnumerator RemoveOverlay()
    {
        yield return 0;
        flash_overlay.enabled = false;
    }

}
