using Application.Features.Clients.Commands.AddClient;
using Application.Interfaces.Repositories;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Clients.Commands.DeleteClient
{
	public class DeleteClientCommandValidator : AbstractValidator<DeleteClientCommand>
	{
		private readonly IClientRepositoryAsync _repository;

		public DeleteClientCommandValidator(IClientRepositoryAsync repository)
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
