using EliteMonitor.Elite;
using EliteMonitor.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EliteMonitor.Sessions
{
    public class SessionManager
    {

        public enum SessionEvent
        {
            START_SESSION,
            ADD_CREDITS,
            REMOVE_CREDITS,
            PURCHASE_SHIP,
            SELL_SHIP,
            FSD_JUMP,
            COLLECT_MATERIAL,
            BUY_CARGO,
            SELL_CARGO,
            CLAIM_BOUNTY,
            CLAIM_BONDS,
            GET_BOUNTY
        }

        private MainForm main;
        private Logger Logger;

        public SessionManager(MainForm _main)
        {
            this.main = _main;
            this.Logger = new Logger("SessionManager");
        }

        public CommanderSession getCurrentSessionForCommander(Commander c)
        {
            return c.Sessions.Last();
        }

        public CommanderSession startNewSessionForCommander(Commander c)
        {
            CommanderSession session = new CommanderSession();
            c.Sessions.Add(session);
            this.addSessionEvent(c, SessionEvent.START_SESSION);
            return session;
        }

        public CommanderSession addSessionEvent(Commander c, SessionEvent sEvent, params object[] data)
        {
            CommanderSession cSession = this.getCurrentSessionForCommander(c);
            return cSession;
        }

    }
}
