using Application.Interfaces.Repositories;
using FluentValidation;

namespace Application.Features.Clients.Commands.UpdateClient
{
	public class UpdateClientCommandValidator : AbstractValidator<UpdateClientCommand>
	{
		private readonly IClientRepositoryAsync _repository;

		public UpdateClientCommandValidator(IClientRepositoryAsync repository)
		{
			_repository = repository;

			RuleFor(p => p.ClientId)
				.NotEqual(Guid.Empty).WithMessage("{PropertyName} must be a valid GUID.")
				.MustAsync(IsContains).WithMessage("Client with {PropertyName} does not exists.");
		}
		private async Task<bool> IsContains(Guid id, CancellationToken cancellationToken)
		{
			var server = await _repository.GetByIdAsync(id);
			if (server != null) return true;
			return false;
		}
	}
}
