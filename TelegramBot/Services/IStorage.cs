using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBot.Models;

namespace TelegramBot.Services
{
    public interface IStorage
    {
        Session GetSession(long chatId);
    }
}
