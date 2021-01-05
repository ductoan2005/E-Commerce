using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoAn1.EmailSender
{
    public interface IEmailSender
    {
        void SendEmail(Message message);
    }
}
