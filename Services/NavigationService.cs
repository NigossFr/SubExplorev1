namespace SubExplore.Services
{
    /// <summary>
    /// Implémentation du service de navigation utilisant Shell
    /// </summary>
    public class NavigationService : INavigationService
    {
        /// <inheritdoc/>
        public Task NavigateToAsync(string route, bool animate = true)
        {
            return Shell.Current.GoToAsync(route, animate);
        }

        /// <inheritdoc/>
        public Task NavigateToAsync(string route, IDictionary<string, object> parameters, bool animate = true)
        {
            return Shell.Current.GoToAsync(route, animate, parameters);
        }

        /// <inheritdoc/>
        public Task GoBackAsync(bool animate = true)
        {
            return Shell.Current.GoToAsync("..", animate);
        }

        /// <inheritdoc/>
        public Task GoToRootAsync(bool animate = true)
        {
            return Shell.Current.GoToAsync("//", animate);
        }
    }
}
