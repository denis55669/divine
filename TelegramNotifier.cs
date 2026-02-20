using System;
using System.Net.Http;
using System.Threading.Tasks;
using Divine.Plugin;
using Divine.Game;
using Divine.Update;
using Divine.Menu;
using Divine.Menu.Items;

namespace TelegramNotifier
{
    [Plugin("TG Notifier")]
    public class TelegramNotifier : PluginBootstrapper
    {
        private Menu _menu;
        private const string TgToken = "8452444419:AAE-Hz3cenJ6C0rEuKmIL2C9xTE1fGY4VoM";
        private const string ChatId = "-1003448981729";
        private static readonly HttpClient Client = new HttpClient();
        private GameState _lastState = GameState.Undefined;

        protected override void OnActivate()
        {
            // Создаем меню самым надежным методом Divine
            _menu = MenuManager.CreateMenu("TG Notifier", "tg_notifier");
            var testBtn = _menu.CreateButton("Test Telegram Connection");
            
            testBtn.MouseClick += (s, e) => 
            {
                Task.Run(() => SendTg("🚀 ТЕСТ DIVINE: Кнопка нажата, всё работает!"));
            };

            UpdateManager.Update += OnUpdate;
            Task.Run(() => SendTg("✅ Divine: Плагин загружен в игру и ждет поиск!"));
        }

        protected override void OnDeactivate()
        {
            UpdateManager.Update -= OnUpdate;
            MenuManager.RemoveMenu(_menu);
        }

        private void OnUpdate()
        {
            var state = GameManager.GameState;
            if (state != _lastState)
            {
                if (state == GameState.WaitingForPlayersToLoad)
                    Task.Run(() => SendTg("🎮 ИГРА НАЙДЕНА! Принимаю..."));
                else if (state == GameState.PreGame || state == GameState.GameInProgress)
                {
                    if (_lastState == GameState.WaitingForPlayersToLoad || _lastState == GameState.HeroSelection)
                        Task.Run(() => SendTg("🚀 МАТЧ НАЧАЛСЯ!"));
                }
                else if (state == GameState.PostGame)
                    Task.Run(() => SendTg("🏁 КАТКА ЗАКОНЧИЛАСЬ."));

                _lastState = state;
            }
        }

        private async Task SendTg(string text)
        {
            try
            {
                string url = $"https://api.telegram.org/bot{TgToken}/sendMessage?chat_id={ChatId}&text={Uri.EscapeDataString(text)}";
                await Client.GetAsync(url);
            }
            catch (Exception) { }
        }
    }
}
