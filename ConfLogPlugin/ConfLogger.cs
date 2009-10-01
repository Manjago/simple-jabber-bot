using Temnenkov.SJB.Common;
using Temnenkov.SJB.ConfLogPlugin.Business;

namespace Temnenkov.SJB.ConfLogPlugin
{
    public sealed class ConfLogger : Plugin
    {
        private PersistentLineDataLayer _dal;

        public ConfLogger(ITranslator translator)
            : base(translator)
        {
            Translator.RoomPublicMessage += Translator_RoomPublicMessage;
            Translator.RoomDelayPublicMessage += Translator_RoomDelayPublicMessage;
            Translator.ChangeSubject += Translator_ChangeSubject;
            Translator.ChangeSubjectDelay += Translator_ChangeSubjectDelay;
            Translator.RoomLeaveJoin += Translator_RoomLeaveJoin;
        }

        private void Translator_RoomLeaveJoin(object sender, RoomLeaveJoinEventArgs e)
        {
            new LeaveJoinLine(e.RoomJid, e.Who, e.IsJoin, e.Date).Save(_dal);
        }

        public override void Init()
        {
            base.Init();
            _dal = new PersistentLineDataLayer("ConfLog");
            _dal.Check();
        }

        void Translator_RoomDelayPublicMessage(object sender, RoomDelayMessageEventArgs e)
        {
            new ProtocolDelayLine(e.RoomJid, e.From, e.Message, e.Date).Save(_dal);
        }

        void Translator_RoomPublicMessage(object sender, RoomMessageEventArgs e)
        {
            new ProtocolLine(e.RoomJid, e.From, e.Message, e.Date).Save(_dal);
        }

        void Translator_ChangeSubject(object sender, ChangeSubjectEventArgs e)
        {
            new ChangeSubjectLine(e.RoomJid, e.Who, e.Subject, e.Date).Save(_dal);
        }

        void Translator_ChangeSubjectDelay(object sender, ChangeSubjectDelayEventArgs e)
        {
            new ChangeSubjectDelayLine(e.RoomJid, e.Who, e.Subject, e.Date).Save(_dal);
        }

    }
}
