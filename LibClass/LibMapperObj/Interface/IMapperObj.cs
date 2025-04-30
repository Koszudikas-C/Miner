using System.Reflection;
using System.Collections.Generic;

namespace LibMapperObj.Interface;

public interface IMapperObj
{
    TDto MapToDto<TSource, TDto>(TSource source, TDto dto);
    
    TSource MapToObj<TDto, TSource>(TDto dto, TSource source);

    TDto MapToDto<TSource, TDto>(TSource source, TDto dto, Func<PropertyInfo, bool>? propertyFilter);

    TSource MapToObj<TDto, TSource>(TDto dto, TSource source, Func<PropertyInfo, bool> propertyFilter);

    TDto MapToDto<TSource, TDto>(TSource source, TDto dto, List<string> excludePropertyNames);

    TSource MapToObj<TDto, TSource>(TDto dto, TSource source, List<string> excludePropertyNames);
}