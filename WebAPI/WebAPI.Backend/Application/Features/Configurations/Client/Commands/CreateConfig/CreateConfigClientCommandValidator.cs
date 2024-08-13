using Application.Interfaces.Repositories;
using Domain.Enums;
using FluentValidation;

namespace Application.Features.Configurations.Client.Commands.CreateConfig
{
	public class CreateConfigClientCommandValidator : AbstractValidator<CreateConfigClientCommand>
	{
		private readonly IServerConfigRepositoryAsync _repository;
		public CreateConfigClientCommandValidator(IServerConfigRepositoryAsync repository)
		{
			_repository = repository;

			RuleFor(p => p.AlarmDB)
			.NotEmpty().WithMessage("{PropertyName} is required.")
			.NotNull();
			RuleFor(p => p.NotificationDB)
			.NotEmpty().WithMessage("{PropertyName} is required.")
			.NotNull();
			RuleFor(p => p.DB)
			.NotEmpty().WithMessage("{PropertyName} is required.")
			.NotNull();
			RuleFor(p => p.AlarmUrl)
			.NotEmpty().WithMessage("{PropertyName} is required.")
			.NotNull();
			RuleFor(p => p.NotifyUrl)
			.NotEmpty().WithMessage("{PropertyName} is required.")
			.NotNull();
			RuleFor(p => p.Mode)
			.NotEmpty().WithMessage("{PropertyName} is required.").NotNull();
			RuleFor(p => p.SystemId).NotEmpty().WithMessage("{PropertyName} is required.")
			.NotEqual(Guid.Empty).WithMessage("{PropertyName} must be a valid GUID.")
			.MustAsync(IsUniqueNumber).WithMessage("{PropertyName} already exists.").NotNull();
			RuleFor(p => p.ConsumerBootstrapServer).NotEmpty().WithMessage("{PropertyName} is required.")
			.NotNull();
			RuleFor(p => p.ProducerBootstrapServer).NotEmpty().WithMessage("{PropertyName} is required.")
			.NotNull();
		}
		private async Task<bool> IsUniqueNumber(Guid id, CancellationToken cancellationToken)
		{
			var config = await _repository.GetByIdAsync(id);
			if (config == null) return true;
			return false;
		}
	}
}
