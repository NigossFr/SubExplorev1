namespace SubExplore.Services
{
    /// <summary>
    /// Implémentation du service de dialogues utilisant les APIs MAUI
    /// </summary>
    public class DialogService : IDialogService
    {
        private Page? GetCurrentPage()
        {
            return Application.Current?.Windows?.FirstOrDefault()?.Page;
        }

        /// <inheritdoc/>
        public Task ShowAlertAsync(string title, string message, string cancel = "OK")
        {
            return GetCurrentPage()?.DisplayAlert(title, message, cancel) ?? Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task<bool> ShowConfirmAsync(string title, string message, string accept = "Oui", string cancel = "Non")
        {
            return GetCurrentPage()?.DisplayAlert(title, message, accept, cancel) ?? Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<string?> ShowActionSheetAsync(string title, string cancel, string? destruction = null, params string[] buttons)
        {
            return GetCurrentPage()?.DisplayActionSheet(title, cancel, destruction, buttons) ?? Task.FromResult<string?>(null);
        }

        /// <inheritdoc/>
        public Task ShowLoadingAsync(string message = "Chargement...")
        {
            // Pour le moment, on retourne un Task completé
            // Dans une version future, on pourrait utiliser un popup personnalisé
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task HideLoadingAsync()
        {
            // Pour le moment, on retourne un Task completé
            // Dans une version future, on pourrait fermer le popup personnalisé
            return Task.CompletedTask;
        }
    }
}
