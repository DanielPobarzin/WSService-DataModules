namespace Application.Features.Clients.Queries.GetClient.GetAll
{
	/// <summary>
	/// Represents the view model for a list of clients
	/// </summary>
	/// /// <remarks>
	/// The client is such an <see cref="Domain.Entities.Client"/> entity
	/// The presentation model looks like a list of received clients
	/// </remarks>
	public class ClientListViewModel
	{
		/// <summary>
		/// Gets or sets the list of <see cref="ClientLookupDTO"/> clients.
		/// </summary>
		public IList<ClientLookupDTO> Clients { get; set; }
	}
}