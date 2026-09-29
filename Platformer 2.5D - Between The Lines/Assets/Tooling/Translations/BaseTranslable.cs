using System;

//Author : MERFOUD Kelyan

namespace Tooling
{
    public abstract class BaseTranslable
    {
        public BaseTranslable() => Language.AddTranslatable(this);
        public abstract bool IsVoice();
        internal abstract void SetTranslation(int pIndex);
        public abstract void SetTranslationToCurrentLanguage();
    }
}