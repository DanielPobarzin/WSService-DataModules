﻿using FluentValidation.Results;

namespace Application.Exceptions
{
	public class ValidationException : Exception
	{
		public List<string> Errors { get; }
		public ValidationException() : base("One or more validation failures have occurred.")
		{
			Errors = new List<string>();
		}

		public ValidationException(IEnumerable<ValidationFailure> failures)
			: this()
		{
			foreach (var failure in failures)
			{
				Errors.Add(failure.ErrorMessage);
			}
		}

		public ValidationException(string message) : base(message)
		{
		}

		public ValidationException(string message, Exception Exception) : base(message, Exception)
		{
		}
	}
}
