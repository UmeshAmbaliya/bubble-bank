using UnityEngine;

public class SoundHandler : MonoBehaviour
{
    public static SoundHandler Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource clickAu;


    [Header("Clips")]
    [SerializeField] private AudioClip clickClip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (Instance!= this)
            {
                Destroy(this.gameObject);
            }
        }
    }

    public void PlayButtonClip()
    {
        if (DataHandler.inst.Sound == 1)
        {
            clickAu.PlayOneShot(clickClip);
        }
    } 
}
