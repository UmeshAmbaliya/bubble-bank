using TMPro;
using UnityEngine;

public class InfoScreenScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI contentTaxt;
    [SerializeField] private TextMeshProUGUI titleText;

    public void ShowPopUp(string title, string content)
    {
        titleText.text = title;
        contentTaxt.text = content;
        this.gameObject.SetActive(true);
        SoundHandler.Instance.PlayButtonClip();
    }

    public void HidePopUp()
    {
        this.gameObject.SetActive(false);
        SoundHandler.Instance.PlayButtonClip();
    }
}
