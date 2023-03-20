using TMPro;
using UnityEngine;

public class PlayerEntryHandler : MonoBehaviour
{
    public TMP_Text PlayerName;
    private string _PlayerName;

    public void InitializeEntry(string playerName)
    {
        _PlayerName = playerName;
        PlayerName.text = _PlayerName;
    }
    public void EntryClicked()
    {

    }
}
