namespace Aliencube.Web.Http.Extended
{
    /// <summary>
    /// This identifies the authorisatino status, which is based on HTTP status code.
    /// </summary>
    public enum AuthorizationStatus
    {
        /// <summary>
        /// Determines the authorisation request is valid and accepted.
        /// </summary>
        Accepted = 202,

        /// <summary>
        /// Determines the authorisation request is invalid.
        /// </summary>
        Unauthorized = 401,

        /// <summary>
        /// Determines the authorisation request is valid but not enough for further processing.
        /// </summary>
        Forbidden = 403,
    }
}