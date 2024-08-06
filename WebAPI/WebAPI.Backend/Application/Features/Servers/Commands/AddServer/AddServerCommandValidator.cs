﻿using Application.Features.Connections.Commands.AddServer;
using Application.Interfaces.Repositories;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Servers.Commands.AddServer
{
	public class AddServerCommandValidator : AbstractValidator<AddServerCommand>
	{
		private readonly IServerRepositoryAsync _repository;
		public AddServerCommandValidator(IServerRepositoryAsync repository)
		{
			_repository = repository;

			RuleFor(p => p.WorkStatus)
				.NotEmpty().WithMessage("{PropertyName} is required.").NotNull();

			RuleFor(p => p.ServerId)
				.NotEmpty().WithMessage("{PropertyName} is required.").NotNull()
				.Must(BeValidGuid).WithMessage("{PropertyName} must be a valid GUID.")
				.MustAsync(IsUniqueNumber).WithMessage("{PropertyName} already exists.");

			RuleFor(p => p.ConnectionStatus)
				.NotEmpty().WithMessage("{PropertyName} is required.").NotNull();

			RuleFor(p => p.CountListeners).NotEmpty().WithMessage("{PropertyName} is required.").NotNull()
				.GreaterThanOrEqualTo(0).WithMessage("{PropertyName} must be a non-negative integer.");
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
