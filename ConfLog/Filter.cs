using System;
using System.Collections.Generic;
using System.Text;

namespace Temnenkov.SJB.ConfLog
{
    internal class Filter
    {
        private Depot _depot;
        private List<ProtocolLine> _lines;

        internal Filter()
        {
            _depot = new Depot();
            _lines = new List<ProtocolLine>();
        }

        internal bool Approve(ProtocolLine line)
        {
            _lines = null;
            if (!line.IsDelay)
            {
                switch (_depot.State)
                {
                    case DepotState.Empty:
                        return true;
                    case DepotState.PrintedChain:
                        _depot.Clear();
                        return true;
                    case DepotState.NonPrinted:
                        _lines = _depot.GetNotPrinted();
                        _depot.Clear();
                        _lines.Add(line);
                        return false;
                    default:
                        return true;
                }
            }
            else
            {
                _depot.Add(line);
                return false;
            }
        }

        internal IEnumerable<ProtocolLine> GetLines()
        {
            return _lines;
        }

        internal void Stop()
        {
            _lines = null;
            if (_depot.State == DepotState.NonPrinted)
            {
                _lines = _depot.GetNotPrinted();
                _depot.Clear();
            }
        }
    }
}
