using UnityEngine;

[CreateAssetMenu(fileName = "DialogueLine", menuName = "VisualNovel/Dialogue Line")]
public class DialogueLine : ScriptableObject
{
    public string characterName;
    [TextArea(3, 10)]
    public string dialogueText;
    public EnumCity actualCity;
    public DialogueChoice[] choices;
}
