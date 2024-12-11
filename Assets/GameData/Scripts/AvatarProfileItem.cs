using UnityEngine;

public class AvatarProfileItem : MonoBehaviour
{
    public GameObject selectionObj;
    int myIndex = - 1;

    private void Start()
    {
        myIndex = int.Parse(this.gameObject.name);
    }

    public void SelectProfileClick()
    {
        HomeUIHandler.inst.avatarSelectionScript.currentSelectedAvatarProfile = myIndex;
        for (int i = 0; i < HomeUIHandler.inst.avatarSelectionScript.selectImageAllAvatars.Length; i++)
        {
            HomeUIHandler.inst.avatarSelectionScript.selectImageAllAvatars[i].UpdateSelection();
        }
        HomeUIHandler.inst.avatarSelectionProfileImage.sprite = DataHandler.inst.userSprites[myIndex];
        SoundHandler.Instance.PlayButtonClip();
    }

    public void UpdateSelection()
    {
        if (myIndex == -1)
        {
            myIndex = int.Parse(this.gameObject.name);
        }
        selectionObj.SetActive(myIndex == HomeUIHandler.inst.avatarSelectionScript.currentSelectedAvatarProfile);
    }
}
