using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JoinByCodeHandler : MonoBehaviour
{
    public TMP_InputField JoinCode;
    public LobbyController _LobbyController;
    public MenuHandler _MenuHandler;

    public void JoinClicked()
    {
        _LobbyController.JoinLobbyByCode(JoinCode.text);
    }
}
