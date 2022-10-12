using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceSimulator
{
    public class ActionBar
    {
        private DateTime? _timer;
        private ulong? _messageVisibleFor;
        private string _message = "";
        private bool _isNew = false;

        public void Show(string message, ulong milliseconds)
        {
            _timer = DateTime.Now;
            _messageVisibleFor = milliseconds;
            _message = message;
            _isNew = true;
        }

        public void Draw()
        {
            var now = DateTime.Now;
            if (_messageVisibleFor == null || _timer == null || _message.Length == 0)
                return;

            var shouldHide = (now - _timer.Value).TotalMilliseconds >= _messageVisibleFor;

            var y = Console.WindowHeight - 1;
            if (shouldHide || _isNew)
            {
                Console.SetCursorPosition(0, y);
                Console.Write(new string(' ', Console.WindowWidth));
            }

            if (shouldHide)
            {
                _message = "";
                _messageVisibleFor = null;
                _timer = null;
                _isNew = true;
                return;
            }

            if (_isNew)
            {
                _isNew = false;
                Console.SetCursorPosition((Console.WindowWidth - _message.Length) / 2, y);
                Console.Write(_message);
            }
        }
    }
}
