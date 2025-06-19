using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace PolMedUMG.Controls
{

    public class CalendarNoUia : Calendar
    {
        protected override AutomationPeer OnCreateAutomationPeer() => null;
    }
}