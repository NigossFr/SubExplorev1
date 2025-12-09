namespace SubExplore.Services
{
    /// <summary>
    /// Service de navigation pour gérer les transitions entre pages
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Navigue vers une page spécifiée par sa route
        /// </summary>
        /// <param name="route">Route de la page cible</param>
        /// <param name="animate">Indique si la navigation doit être animée</param>
        Task NavigateToAsync(string route, bool animate = true);

        /// <summary>
        /// Navigue vers une page avec des paramètres
        /// </summary>
        /// <param name="route">Route de la page cible</param>
        /// <param name="parameters">Dictionnaire de paramètres à passer</param>
        /// <param name="animate">Indique si la navigation doit être animée</param>
        Task NavigateToAsync(string route, IDictionary<string, object> parameters, bool animate = true);

        /// <summary>
        /// Retourne à la page précédente
        /// </summary>
        /// <param name="animate">Indique si le retour doit être animé</param>
        Task GoBackAsync(bool animate = true);

        /// <summary>
        /// Retourne à la racine de la navigation
        /// </summary>
        /// <param name="animate">Indique si le retour doit être animé</param>
        Task GoToRootAsync(bool animate = true);
    }
}
