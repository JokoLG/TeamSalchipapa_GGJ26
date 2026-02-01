using System;
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "Nuevo Dialogo", menuName = "Text Core / DialogueData")]
public class DialogueData : ScriptableObject
{
    [Serializable]
    public struct DialogueLine
    {
        public string dialogueText;
        public TMP_FontAsset fontAsset;
    }

    [Header("Script Content")] public DialogueLine[] lineas;
}