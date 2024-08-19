using Application.Interfaces.Repositories;
using FluentValidation;

namespace Application.Features.Configurations.Client.Commands.UpdateConfig
{
	public class UpdateConfigClientCommandValidator : AbstractValidator<UpdateConfigClientCommand>
	{
		private readonly IClientConfigRepositoryAsync _repository;
		public UpdateConfigClientCommandValidator(IClientConfigRepositoryAsync repository)
		{
			_repository = repository;

			RuleFor(p => p.SystemId)
				.NotEqual(Guid.Empty).WithMessage("{PropertyName} must be a valid GUID.")
				.MustAsync(IsContains).WithMessage("Client with {PropertyName} does not exists.");
		}
		private async Task<bool> IsContains(Guid SystemId, CancellationToken cancellationToken)
		{
			var config = await _repository.GetByIdAsync(SystemId);
			if (config == null) return true;
			return false;
		}
	}
}