﻿namespace Foundation.Api.Services.ValueToReplace
{
    using Foundation.Api.Data.Entities;
    using Foundation.Api.Models.Pagination;
    using Foundation.Api.Models.ValueToReplace;
    using System.Threading.Tasks;

    public interface IValueToReplaceRepository
    {
        PagedList<ValueToReplace> GetValueToReplaces(ValueToReplaceParametersDto valueToReplaceParameters);
        Task<ValueToReplace> GetValueToReplaceAsync(int valueToReplaceId);
        ValueToReplace GetValueToReplace(int valueToReplaceId);
        void AddValueToReplace(ValueToReplace valueToReplace);
        void DeleteValueToReplace(ValueToReplace valueToReplace);
        void UpdateValueToReplace(ValueToReplace valueToReplace);
        bool Save();
    }
}
