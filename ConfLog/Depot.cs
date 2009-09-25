using System;
using System.Collections.Generic;
using System.Text;

namespace Temnenkov.SJB.ConfLog
{
    internal class Depot
    {
        private List<ProtocolLine> _printed;
        private List<ProtocolLine> _notPrinted;

        internal DepotState State { get; private set; }

        internal Depot()
        {
            State = DepotState.Empty;
            _printed = new List<ProtocolLine>();
            _notPrinted = new List<ProtocolLine>();
        }

        internal void Clear()
        {
            throw new NotImplementedException();
        }


        internal List<ProtocolLine> GetNotPrinted()
        {
            throw new NotImplementedException();
        }

        internal void Add(ProtocolLine line)
        {
            // здесь у нас только отложенные строки!
            if (!line.IsDelay)
                throw new ArgumentException("bad arg - must be delayed");

            // если есть что-то непечатавшееся - то добавляем туда
            if (_notPrinted.Count != 0)
                _notPrinted.Add(line);
            else // иначе - стоит подумать
            {
                if (_printed.Count == 0)
                { // было ли что-то раньше - и с таким же хэшем?
                    if (line.Twin != null)
                        _printed.Add(line);
                    else
                        _notPrinted.Add(line);
                }
                else
                {  // уже есть бывшая - продолжаем сравнивать
                    
                    // а было ли что-то раньше - и с таким же хэшем?
                    if (line.Twin == null)
                        _notPrinted.Add(line);
                    else
                    { // мы продолжаем цепочку?
                        if (line.Twin == _printed[_printed.Count - 1].NextTwin)
                            _printed.Add(line);
                        else
                            _notPrinted.Add(line);
                    }
                }
            }            
        }
    }

    internal enum DepotState
    {
        Empty, // пустой
        PrintedChain, // цепочка, которая уже печаталась
        NonPrinted // есть и что-то непечатавшееся
    }
}
