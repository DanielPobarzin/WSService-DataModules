using Application.Interfaces.Repositories;
using FluentValidation;

namespace Application.Features.Clients.Commands.AddClient
{
	public class AddClientCommandValidator : AbstractValidator<AddClientCommand>
	{
		private readonly IClientRepositoryAsync _repository;
		public AddClientCommandValidator(IClientRepositoryAsync repository)
		{
			_repository = repository;

			RuleFor(p => p.WorkStatus)
				.NotEmpty().WithMessage("{PropertyName} is required.").NotNull();

			RuleFor(p => p.ClientId)
				.NotEmpty().WithMessage("{PropertyName} is required.").NotNull()
				.Must(BeValidGuid).WithMessage("{PropertyName} must be a valid GUID.")
				.MustAsync(IsUniqueNumber).WithMessage("{PropertyName} already exists.");

			RuleFor(p => p.ConnectionStatus)
				.NotEmpty().WithMessage("{PropertyName} is required.").NotNull();
		}

		private bool BeValidGuid(Guid serverId)
		{
			return serverId != Guid.Empty; 
		}
		private async Task<bool> IsUniqueNumber(Guid id, CancellationToken cancellationToken)
		{
			var server = await _repository.GetByIdAsync(id);
			if (server == null) return true;
			return false;
		}
	}
}
