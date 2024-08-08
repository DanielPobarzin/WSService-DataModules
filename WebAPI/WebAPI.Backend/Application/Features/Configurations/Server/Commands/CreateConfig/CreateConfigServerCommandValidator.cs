using Application.Interfaces.Repositories;
using FluentValidation;

namespace Application.Features.Configurations.Server.Commands.CreateConfig
{
	public class CreateConfigServerCommandValidator : AbstractValidator<CreateConfigServerCommand>
	{
		private readonly IServerConfigRepositoryAsync _repository;
		public CreateConfigServerCommandValidator(IServerConfigRepositoryAsync repository)
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
			RuleFor(p => p.NotifyHubMethod)
			.NotEmpty().WithMessage("{PropertyName} is required.")
			.NotNull();
			RuleFor(p => p.AlarmHubMethod)
			.NotEmpty().WithMessage("{PropertyName} is required.")
			.NotNull();
			RuleFor(p => p.NotifyDelayMilliseconds)
			.GreaterThanOrEqualTo(0).WithMessage("{PropertyName} must be a non-negative integer.").NotNull();
			RuleFor(p => p.Port)
			.NotEmpty().GreaterThanOrEqualTo(0).WithMessage("{PropertyName} must be a non-negative integer.").NotNull();
			RuleFor(p => p.AlarmDelayMilliseconds)
			.GreaterThanOrEqualTo(0).WithMessage("{PropertyName} must be a non-negative integer.").NotNull();
			RuleFor(p => p.AllowedOrigins).NotNull();
			RuleFor(p => p.AlarmTargetClients).NotNull();
			RuleFor(p => p.NotifyTargetClients).NotNull();
			RuleFor(p => p.Urls)
			.NotEmpty().WithMessage("{PropertyName} is required.")
			.NotNull();
			RuleFor(p => p.PolicyName).NotNull();
			RuleFor(p => p.RouteAlarm).NotEmpty().WithMessage("{PropertyName} is required.")
			.NotNull(); 
			RuleFor(p => p.RouteNotify).NotEmpty().WithMessage("{PropertyName} is required.")
			.NotNull(); 
			RuleFor(p => p.SystemId).NotEmpty().WithMessage("{PropertyName} is required.")
			.NotEqual(Guid.Empty).WithMessage("{PropertyName} must be a valid GUID.")
			.MustAsync(IsUniqueNumber).WithMessage("{PropertyName} already exists.").NotNull();

		}
		private async Task<bool> IsUniqueNumber(Guid id, CancellationToken cancellationToken)
		{
			var config = await _repository.GetByIdAsync(id);
			if (config == null) return true;
			return false;
		}
	}
}
		
