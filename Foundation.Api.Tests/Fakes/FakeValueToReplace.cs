﻿using AutoBogus;
using Foundation.Api.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Foundation.Api.Tests.Fakes
{
    // or replace 'AutoFaker' with 'Faker' if you don't want all fields to be auto faked
    public class FakeValueToReplace : AutoFaker<ValueToReplace>
    {
        public FakeValueToReplace()
        {
            // leaving the first 49 for potential special use cases in startup builds that need explicit values
            RuleFor(lambdaInitialsToReplace => lambdaInitialsToReplace.ValueToReplaceId, lambdaInitialsToReplace => lambdaInitialsToReplace.Random.Number(50, 100000));
        }
    }
}
