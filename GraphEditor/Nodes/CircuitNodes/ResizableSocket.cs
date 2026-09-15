using NodeGraphControl.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.Nodes.CircuitNodes
{
    internal class ResizableSocket<S, T> where S : AbstractSocket
    {
        readonly AbstractNode _parentNode;
        readonly List <S> _sockets = new List <S>();
        readonly int _defaultSize = 1;
        readonly bool _startAtZero = true;
        readonly string _socketNameFormat = "";

        int _size = 0;

        public int Size
        {
            get => _size;
            set => SetSize(value);
        }

        public ResizableSocket(AbstractNode parentNode, string socketNameFormat, int defaultSize, bool startAtZero = true)
        {
            _parentNode = parentNode;
            _socketNameFormat = socketNameFormat;
            _defaultSize = defaultSize;
            _startAtZero = startAtZero;

            SetSize(_defaultSize);
        }

        private string GetSocketName(int idx)
        {
            return string.Format(_socketNameFormat, _startAtZero ? idx : idx + 1);
        }

        private void SetSize(int size)
        {
            if (size == _size)
                return;

            _size = size;

            while (_sockets.Count > _size)
            {
                S socket = _sockets[_sockets.Count - 1];
                _parentNode.Sockets.Remove(socket);
                _sockets.Remove(socket);
            }

            while (_sockets.Count < _size)
            {
                if (typeof(S) == typeof(SocketIn))
                {
                    SocketIn socketIn = new SocketIn(typeof(T), GetSocketName(_sockets.Count), _parentNode, true);
                    _parentNode.Sockets.Add(socketIn);
                    _sockets.Add(socketIn as S);
                }
                else if (typeof(S) == typeof(SocketOut))
                {
                    SocketOut socketOut = new SocketOut(typeof(T), GetSocketName(_sockets.Count), _parentNode);
                    _parentNode.Sockets.Add(socketOut);
                    _sockets.Add(socketOut as S);
                }
            }
        }
    }
}
