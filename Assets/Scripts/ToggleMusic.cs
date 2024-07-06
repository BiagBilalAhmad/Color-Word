using UnityEngine;
using UnityEngine.UI;

public class ToggleMusic : MonoBehaviour
{
    public Toggle toggle;

    public void Togglemusic()
    {
        SoundManager.instance.ToggleMusic(toggle);
    }
    public void ToggleVolume()
    {
        SoundManager.instance.ToggleVolume(toggle);
    }
}
