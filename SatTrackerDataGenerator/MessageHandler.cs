using System;
using System.Collections.Generic;
using System.Text;

namespace SatTrackerDataGenerator
{
    public class MessageHandler : IDisposable
    {
        private List<List<string>> _workChunks;
        public bool running = false;
        private SendMessage sendMessage;
        public MessageHandler(List<List<string>> workChunks)
        {
            _workChunks = workChunks;
            sendMessage = new SendMessage();
        }

        public void RunMessages()
        {
            running = true;
            foreach(var chunk in _workChunks)
            {
                sendMessage.ProcessMessages(chunk);
            }
            running = false;
        }

        public void updateChunks(List<List<string>> workChunks)
        {
            _workChunks = workChunks;
        }

        public void Dispose()
        {
            sendMessage.Disconnect();
        }
    }
}
