using System.Text;
using UnityEngine;
using System;

//Author : MERFOUD Kelyan

namespace Tooling
{
    [Serializable]
    public class Translable<T> : BaseTranslable, ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector] internal T[] ValueArray = new T[Language.Languages.Length];
        public T Value { get; private set; }
        public readonly static bool isVoice;

        static Translable() => isVoice = typeof(T) == typeof(AudioClip) || typeof(T).IsSubclassOf(typeof(AudioClip));

        public Translable() : base()
        {
            SetTranslationToCurrentLanguage(); 
        }

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            SetTranslationToCurrentLanguage();
        }

        public override bool IsVoice() => isVoice;
        internal override void SetTranslation(int pIndex) => Value = ValueArray[pIndex];
        public override void SetTranslationToCurrentLanguage() => Value = ValueArray[isVoice ? Language.CurrentVoiceLanguage : Language.CurrentLanguage];

        public override string ToString()
        {
            StringBuilder lBuilder = new StringBuilder();

            for (int i = 0; i < ValueArray.Length; i++)
            {
                lBuilder.Append('[');
                lBuilder.Append(Language.Languages[i]);
                lBuilder.Append(", ");
                lBuilder.Append(ValueArray[i].ToString());
                lBuilder.Append("] ");
            }

            return lBuilder.ToString();
        }
    }
}