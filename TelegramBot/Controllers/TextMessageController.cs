using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Services;

namespace TelegramBot.Controllers
{
    class TextMessageController
    {
        private readonly IStorage _memoryStorage;
        private readonly ITelegramBotClient _telegramClient;

        public TextMessageController(ITelegramBotClient telegramBotClient, IStorage memoryStorage)
        {
            _telegramClient = telegramBotClient;
            _memoryStorage = memoryStorage;
        }

        public async Task Handle(Message message, CancellationToken ct)
        {
            Console.WriteLine(_memoryStorage.GetSession(message.Chat.Id).SelectedButton);
            switch (message.Text)
            {
                case "/start":

                    // Объект, представляющий кноки
                    var buttons = new List<InlineKeyboardButton[]>();
                    buttons.Add(new[]
                    {
                        InlineKeyboardButton.WithCallbackData($" Длина строки" , $"stringLength"),
                        InlineKeyboardButton.WithCallbackData($" Сумма чисел" , $"sumDigits")
                    });

                    // передаем кнопки вместе с сообщением (параметр ReplyMarkup)
                    await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"<b>  Наш бот может посчитать длину строки или сумму чисел. (по умолчанию - длина строки)</b> {Environment.NewLine}" +
                        $"{Environment.NewLine}Выберите вариант из кнопок ниже:  {Environment.NewLine}", cancellationToken: ct, parseMode: ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(buttons));

                    break;
                default:
                    switch (_memoryStorage.GetSession(message.Chat.Id).SelectedButton)
                    {
                        case "stringLength":
                            await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"В вашем сообщении {message.Text.Length} символов", cancellationToken: ct);
                            break;
                        case "sumDigits":
                            if(!CheckMessage(message.Text))
                            {
                                await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"Ошибка! В вашем сообщении содержатся не только цифры", cancellationToken: ct);
                            }
                            else
                            {
                                await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"Сумма чисел в строке: {SumDigits(message.Text)}", cancellationToken: ct);
                            }
                            break;
                        default:
                            await _telegramClient.SendTextMessageAsync(message.Chat.Id, $"Получено сообщение не поддерживаемого формата", cancellationToken: ct);
                            break;
                    }
                    break;
            }
        }

        private bool CheckMessage(string message)
        {
            bool allDigits = Regex.IsMatch(message, @"^[\d\s]+$");

            if (!message.Contains(" ")) { allDigits = false; }

            return allDigits;
        }

        private int SumDigits(string message)
        {
            string[] substrings = message.Split(' ');
            int sum = 0;

            foreach (string digit in substrings)
            {
                sum += Convert.ToInt32(digit);
            }

            return sum;
        }
    }
}
