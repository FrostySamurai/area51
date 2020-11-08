using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameHelper : MonoBehaviour
{
    public void EndTheGame()
    {
        AppData.MessageDisplayer.DisplayAfterword();
    }
}

