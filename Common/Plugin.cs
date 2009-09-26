using System;
using System.Collections.Generic;
using System.Text;

namespace Temnenkov.SJB.Common
{
    public class Plugin
    {
        protected ITranslator Translator { get; private set; }
        public Plugin(ITranslator translator)
        {
            Translator = translator;
        }
    }
}
