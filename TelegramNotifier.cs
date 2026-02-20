using System;
using System.Net.Http;
using System.Threading.Tasks;
using Divine.Plugin;
using Divine.Update;
using Divine.Game;
using Divine.Menu;
using Divine.Menu.Items;

namespace TelegramNotifier
{
    [Plugin("TG Notifier")]
    public class TelegramNotifier : PluginBootstrapper
    {
        private const string TgToken = "8452444419:AAE-Hz3cenJ6C0rEuKmIL2C9xTE1fGY4VoM";
        private const string ChatId = "-1003448981729";
        
        private static readonly HttpClient Client = new HttpClient();
        private GameState _lastGameState = GameState.Undefined;

        private Menu _mainMenu;
        private MenuButton _testButton;

        protected override void OnActivate()
        {
            // Создаем меню в самом чите Divine
            _mainMenu = MenuManager.CreateRootMenu("TG Notifier");
            _testButton = _mainMenu.CreateButton("Test Telegram Connection");
            
            // Действие при нажатии на кнопку теста
            _testButton.MouseClick += (sender, e) => 
            {
                Task.Run(() => SendTelegramMessageAsync("🚀 ТЕСТ DIVINE: Кнопка в меню нажата, связь есть!"));
            };

            UpdateManager.Update += OnUpdate;
            
            // Отправляем сообщение при старте асинхронно, чтобы не крашить Доту
            Task.Run(() => SendTelegramMessageAsync("✅ Divine: Меню создано! Скрипт готов к поиску игры."));
        }

        protected override void OnDeactivate()
        {
            UpdateManager.Update -= OnUpdate;
            MenuManager.RemoveRootMenu(_mainMenu);
        }

        private void OnUpdate()
        {
            var currentState = GameManager.GameState;
            
            if (currentState != _lastGameState)
            {
                if (currentState == GameState.WaitingForPlayersToLoad)
                {
                    Task.Run(() => SendTelegramMessageAsync("🎮 ИГРА НАЙДЕНА! Загружаемся..."));
                }
                else if (currentState == GameState.PreGame || currentState == GameState.GameInProgress)
                {
                    if (_lastGameState == GameState.WaitingForPlayersToLoad || _lastGameState == GameState.HeroSelection)
                    {
                        Task.Run(() => SendTelegramMessageAsync("🚀 МАТЧ НАЧАЛСЯ!"));
                    }
                }
                else if (currentState == GameState.PostGame)
                {
                    Task.Run(() => SendTelegramMessageAsync("🏁 КАТКА ЗАКОНЧИЛАСЬ."));
                }

                _lastGameState = currentState;
            }
        }

        private async Task SendTelegramMessageAsync(string text)
        {
            try
            {
                string url = $"https://api.telegram.org/bot{TgToken}/sendMessage?chat_id={ChatId}&text={Uri.EscapeDataString(text)}";
                await Client.GetAsync(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TG Notifier] Ошибка: {ex.Message}");
            }
        }
    }
}
