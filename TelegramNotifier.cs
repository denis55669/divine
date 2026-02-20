using System;
using Divine.Plugin;
using Divine.Menu;
using Divine.Menu.Items;

namespace TelegramNotifier
{
    [Plugin("TG Notifier")]
    public class TelegramNotifier : PluginBootstrapper
    {
        private Menu _mainMenu;

        protected override void OnActivate()
        {
            // Пишем в консоль, чтобы проверить, что скрипт вообще стартанул
            Console.WriteLine("[TG Notifier] Попытка загрузки плагина...");

            // Создаем меню
            _mainMenu = MenuManager.CreateRootMenu("TG Notifier Safe");
            var btn = _mainMenu.CreateButton("Test Button (No Internet)");
            
            btn.MouseClick += (sender, e) => 
            {
                Console.WriteLine("[TG Notifier] Кнопка нажата!");
            };

            Console.WriteLine("[TG Notifier] Меню успешно создано!");
        }

        protected override void OnDeactivate()
        {
            if (_mainMenu != null)
            {
                MenuManager.RemoveRootMenu(_mainMenu);
            }
        }
    }
}
