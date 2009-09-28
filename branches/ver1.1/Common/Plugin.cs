namespace Temnenkov.SJB.Common
{
    public class Plugin
    {
        protected ITranslator Translator { get; private set; }
        public Plugin(ITranslator translator)
        {
            Translator = translator;
        }
        public virtual void Init()
        {
        }
    }
}
