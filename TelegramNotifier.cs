using System;
using System.Net.Http;
using System.Threading.Tasks;
using Divine.Plugin;
using Divine.Update;
using Divine.Game;

namespace TelegramNotifier
{
    [Plugin("TG Notifier")]
    public class TelegramNotifier : PluginBootstrapper
    {
        private const string TgToken = "8452444419:AAE-Hz3cenJ6C0rEuKmIL2C9xTE1fGY4VoM";
        private const string ChatId = "-1003448981729";
        
        private static readonly HttpClient Client = new HttpClient();
        private GameState _lastGameState = GameState.Undefined;

        protected override void OnActivate()
        {
            UpdateManager.Update += OnUpdate;
            SendTelegramMessageAsync("✅ Divine: Скрипт запущен! Жду поиск игры.");
        }

        protected override void OnDeactivate()
        {
            UpdateManager.Update -= OnUpdate;
        }

        private void OnUpdate()
        {
            var currentState = GameManager.GameState;
            
            if (currentState != _lastGameState)
            {
                if (currentState == GameState.WaitingForPlayersToLoad)
                {
                    SendTelegramMessageAsync("🎮 ИГРА НАЙДЕНА! Принимаю и гружусь...");
                }
                else if (currentState == GameState.PreGame || currentState == GameState.GameInProgress)
                {
                    if (_lastGameState == GameState.WaitingForPlayersToLoad || _lastGameState == GameState.HeroSelection)
                    {
                        SendTelegramMessageAsync("🚀 МАТЧ НАЧАЛСЯ!");
                    }
                }
                else if (currentState == GameState.PostGame)
                {
                    SendTelegramMessageAsync("🏁 КАТКА ЗАКОНЧИЛАСЬ.");
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
            catch (Exception) { }
        }
    }
}
