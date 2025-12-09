using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SubExplore.ViewModels
{
    /// <summary>
    /// ViewModel de base pour tous les ViewModels de l'application.
    /// Fournit des fonctionnalités communes comme IsBusy, Title, etc.
    /// </summary>
    public abstract partial class BaseViewModel : ObservableObject
    {
        /// <summary>
        /// Titre de la page/vue
        /// </summary>
        [ObservableProperty]
        private string title = string.Empty;

        /// <summary>
        /// Indique si le ViewModel est en train de charger des données
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        private bool isBusy;

        /// <summary>
        /// Inverse de IsBusy, utile pour les bindings UI
        /// </summary>
        public bool IsNotBusy => !IsBusy;

        /// <summary>
        /// Méthode appelée lors de l'apparition de la vue
        /// </summary>
        public virtual Task OnAppearingAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Méthode appelée lors de la disparition de la vue
        /// </summary>
        public virtual Task OnDisappearingAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Méthode utilitaire pour exécuter des tâches avec gestion automatique de IsBusy
        /// </summary>
        protected async Task ExecuteAsync(Func<Task> operation)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                await operation();
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
