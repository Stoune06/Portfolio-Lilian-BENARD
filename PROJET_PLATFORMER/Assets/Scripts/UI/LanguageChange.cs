using Tooling;
using UnityEngine;
using UnityEngine.UI;

public class LanguageChange : MonoBehaviour
{
    [SerializeField] private Translable<Sprite> _Flags;
    private Sprite _Sprite;

    private void Start()
    {
        GetComponent<Image>().sprite = _Flags.Value;
        Language.OnLanguageChanged += OnChange;
    }

    public void ChangeLanguage()
    {
        Debug.Log("Changed "+ Language.CurrentLanguage);
        
        if(Language.CurrentLanguage == Language.Languages.Length-1)
        {
            Language.CurrentLanguage = 0;
        }
        else
        {
            Language.CurrentLanguage += 1;
        }
    }
    
    private void OnChange()
    {
        GetComponent<Image>().sprite = _Flags.Value;
    }

    private void OnDestroy()
    {
        Language.OnLanguageChanged -= OnChange;
    }
}
