using System.Reflection;

namespace LibMapperObjRemote.Interface;

public interface IMapperObj
{
  TTarget Map<TSource, TTarget>(TSource source)
    where TTarget : new();

  TTarget Map<TSource, TTarget>(TSource source, Func<TTarget> factory);

  TDto MapToDto<TSource, TDto>(TSource source, TDto dto);

  TSource MapToObj<TDto, TSource>(TDto dto, TSource source);

  void MapTo<TSource, TTarget>(TSource source, TTarget target);

  TDto MapToDto<TSource, TDto>(TSource source, TDto dto, Func<PropertyInfo, bool>? propertyFilter);

  TSource MapToObj<TDto, TSource>(TDto dto, TSource source, Func<PropertyInfo, bool> propertyFilter);

  TDto MapToDto<TSource, TDto>(TSource source, TDto dto, List<string> excludePropertyNames);

  TSource MapToObj<TDto, TSource>(TDto dto, TSource source, List<string> excludePropertyNames);
}
