using UnityEngine;

public class TriggerFunctions : MonoBehaviour
{
    public void DisplayMessage(string message)
    {
        AppData.MessageDisplayer.DisplayMessage(message);
    }
}

