using Application.Features.Clients.Commands.DeleteClient;
using Application.Interfaces.Repositories;
using FluentValidation;

namespace Application.Features.Configurations.Client.Commands.DeleteConfig
{
	public class DeleteConfigClientCommandValidator : AbstractValidator<DeleteClientCommand>
	{
		private readonly IClientConfigRepositoryAsync _repository;
		public DeleteConfigClientCommandValidator(IClientConfigRepositoryAsync repository)
		{
			_repository = repository;

			RuleFor(d => d.ClientId)
				.NotEqual(Guid.Empty).WithMessage("{PropertyName} must be a valid GUID.")
				.MustAsync(IsExists).WithMessage("{PropertyName} does not exists.");
		}
		private async Task<bool> IsExists(Guid id, CancellationToken cancellationToken)
		{
			var server = await _repository.GetByIdAsync(id);
			if (server != null) return true;
			return false;
		}
	}
}
