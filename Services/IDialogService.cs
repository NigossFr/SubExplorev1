namespace SubExplore.Services
{
    /// <summary>
    /// Service pour afficher des dialogues et alertes à l'utilisateur
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// Affiche une alerte simple avec un message
        /// </summary>
        /// <param name="title">Titre de l'alerte</param>
        /// <param name="message">Message de l'alerte</param>
        /// <param name="cancel">Texte du bouton d'annulation</param>
        Task ShowAlertAsync(string title, string message, string cancel = "OK");

        /// <summary>
        /// Affiche une confirmation avec deux boutons
        /// </summary>
        /// <param name="title">Titre de la confirmation</param>
        /// <param name="message">Message de la confirmation</param>
        /// <param name="accept">Texte du bouton d'acceptation</param>
        /// <param name="cancel">Texte du bouton d'annulation</param>
        /// <returns>True si l'utilisateur a accepté, False sinon</returns>
        Task<bool> ShowConfirmAsync(string title, string message, string accept = "Oui", string cancel = "Non");

        /// <summary>
        /// Affiche un dialogue avec plusieurs options
        /// </summary>
        /// <param name="title">Titre du dialogue</param>
        /// <param name="cancel">Texte du bouton d'annulation</param>
        /// <param name="destruction">Texte du bouton destructif</param>
        /// <param name="buttons">Liste des boutons disponibles</param>
        /// <returns>Le texte du bouton sélectionné ou null si annulé</returns>
        Task<string?> ShowActionSheetAsync(string title, string cancel, string? destruction = null, params string[] buttons);

        /// <summary>
        /// Affiche un indicateur de chargement
        /// </summary>
        /// <param name="message">Message à afficher pendant le chargement</param>
        Task ShowLoadingAsync(string message = "Chargement...");

        /// <summary>
        /// Cache l'indicateur de chargement
        /// </summary>
        Task HideLoadingAsync();
    }
}
