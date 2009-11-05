using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using Temnenkov.SJB.Common;

namespace Temnenkov.SJB.Bot
{
    partial class BotAsService : ServiceBase
    {
        private Bot _bot;

        public BotAsService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Logger.Log(LogType.Info, "Starting in service mode");

            _bot = new Bot();
            if (_bot.Connect())
            {
                Logger.Log(LogType.Info, "Successfully connected");

                if (!_bot.JoinRoom(string.Format("{0}/{1}", Settings.RoomJid, Settings.NameInRoom)))
                {
                    Logger.Log(LogType.Error, "Fail join room");
                    Stop();
                }
            }
            else
            {
                Logger.Log(LogType.Error, "Fail connect");
                Stop();
            }
        }

        protected override void OnStop()
        {
            _bot.Disconnect();
        }
    }
}
