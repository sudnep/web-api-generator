﻿namespace Foundation.Api.Validators.ValueToReplace
{
    using FluentValidation;
    using Foundation.Api.Models.ValueToReplace;
    using System;

    public class ValueToReplaceForManipulationDtoValidator<T> : AbstractValidator<T> where T : ValueToReplaceForManipulationDto
    {
        public ValueToReplaceForManipulationDtoValidator()
        {
            RuleFor(lambdaInitialsToReplace => lambdaInitialsToReplace.ValueToReplaceTextField1)
                .NotEmpty();
            RuleFor(lambdaInitialsToReplace => lambdaInitialsToReplace.ValueToReplaceIntField1)
                .GreaterThanOrEqualTo(0);
            RuleFor(lambdaInitialsToReplace => lambdaInitialsToReplace.ValueToReplaceDateField1)
                .LessThanOrEqualTo(DateTime.UtcNow);
        }
    }
}
