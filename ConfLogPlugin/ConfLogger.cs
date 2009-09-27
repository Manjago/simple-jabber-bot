using System;
using System.Collections.Generic;
using System.Text;
using Temnenkov.SJB.Common;
using Temnenkov.SJB.ConfLogPlugin.Business;

namespace Temnenkov.SJB.ConfLogPlugin
{
    public sealed class ConfLogger : Plugin
    {
        private Database.Database _db;

        public ConfLogger(ITranslator translator)
            : base(translator)
        {
            Translator.RoomPublicMessage += Translator_RoomPublicMessage;
            Translator.RoomDelayPublicMessage += Translator_RoomDelayPublicMessage;
            Translator.ChangeSubject += Translator_ChangeSubject;
            Translator.ChangeSubjectDelay += Translator_ChangeSubjectDelay;
        }

        public override void Init()
        {
            base.Init();
            _db = new Temnenkov.SJB.Database.Database("ConfLog");
            PersistentLine.Check(_db);
        }

        void Translator_RoomDelayPublicMessage(object sender, RoomDelayMessageEventArgs e)
        {
            new ProtocolDelayLine(e.RoomJid, e.From, e.Message, e.Date).Save(_db);
        }

        void Translator_RoomPublicMessage(object sender, RoomMessageEventArgs e)
        {
            new ProtocolLine(e.RoomJid, e.From, e.Message, e.Date).Save(_db);
        }

        void Translator_ChangeSubject(object sender, ChangeSubjectEventArgs e)
        {
            new ChangeSubjectLine(e.RoomJid, e.Who, e.Subject, e.Date).Save(_db);
        }

        void Translator_ChangeSubjectDelay(object sender, ChangeSubjectDelayEventArgs e)
        {
            new ChangeSubjectDelayLine(e.RoomJid, e.Who, e.Subject, e.Date).Save(_db);
        }

    }
}
