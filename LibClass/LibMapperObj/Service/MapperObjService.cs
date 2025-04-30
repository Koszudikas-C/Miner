using System.Reflection;
using LibMapperObj.Interface;
using System.Collections.Generic;

namespace LibMapperObj.Service;

public class MapperObjService : IMapperObj
{
    public TDto MapToDto<TSource, TDto>(TSource source, TDto dto)
    {
        ValidateObj(source, dto);
        
        var sourceType = typeof(TSource);
        var dtoType = typeof(TDto);
        
        var sourceProps = sourceType.GetProperties(BindingFlags.Public |
                                                   BindingFlags.Instance | BindingFlags.NonPublic);
        
        var dtoProps = dtoType.GetProperties(BindingFlags.Public |
                                             BindingFlags.Instance | BindingFlags.NonPublic);

        foreach (var dtoProp in dtoProps)
        {
            if (!dtoProp.CanWrite) continue;
            
            var sourceProp = sourceProps.FirstOrDefault(p
                => p.Name == dtoProp.Name && p.PropertyType == dtoProp.PropertyType);
            
            if (sourceProp == null) continue;
            
            var value = sourceProp.GetValue(source);
            
            dtoProp.SetValue(dto, value);
        }
        return dto;
    }

    public TSource MapToObj<TDto, TSource>(TDto dto, TSource source)
    {
        ValidateObj(source, dto);
        
        var sourceType = typeof(TSource);
        var dtoType = typeof(TDto);
        
        var sourceProps = sourceType.GetProperties(BindingFlags.Public |
                                                   BindingFlags.Instance | BindingFlags.NonPublic);
        
        var dtoProps = dtoType.GetProperties(BindingFlags.Public |
                                             BindingFlags.Instance | BindingFlags.NonPublic);

        foreach (var sourceProp in sourceProps)
        {
            if (!sourceProp.CanWrite) continue;
            
            var dtoProp = dtoProps.FirstOrDefault(p
                => p.Name == sourceProp.Name && p.PropertyType == sourceProp.PropertyType);
            
            if (dtoProp == null) continue;
            
            var value = dtoProp.GetValue(dto);
            
            if(value is null) continue;
            
            sourceProp.SetValue(source, value);
        }
        
        return source;
    }
    
    public TDto MapToDto<TSource, TDto>(TSource source, TDto dto, Func<PropertyInfo, bool>? propertyFilter)
    {
        ValidateObj(source, dto);

        var sourceProps = typeof(TSource).GetProperties(
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
        
        var dtoProps = typeof(TDto).GetProperties(
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

        foreach (var dtoProp in dtoProps.Where(p => p.CanWrite && (propertyFilter?.Invoke(p) ?? true)))
        {
            var sourceProp = sourceProps.FirstOrDefault(p =>
                p.Name == dtoProp.Name && p.PropertyType == dtoProp.PropertyType);

            if (sourceProp == null) continue;

            var value = sourceProp.GetValue(source);
            dtoProp.SetValue(dto, value);
        }

        return dto;
    }

    public TSource MapToObj<TDto, TSource>(TDto dto, TSource source, Func<PropertyInfo, bool> propertyFilter)
    {
        ValidateObj(dto, source);

        var sourceProps = typeof(TSource).GetProperties(
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
        
        var dtoProps = typeof(TDto).GetProperties(
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

        foreach (var sourceProp in sourceProps.Where(p => p.CanWrite && (propertyFilter?.Invoke(p) ?? true)))
        {
            var dtoProp = dtoProps.FirstOrDefault(p =>
                p.Name == sourceProp.Name && p.PropertyType == sourceProp.PropertyType);

            if (dtoProp == null) continue;

            var value = dtoProp.GetValue(dto);
            sourceProp.SetValue(source, value);
        }

        return source;
    }

    public TDto MapToDto<TSource, TDto>(TSource source, TDto dto, List<string> excludePropertyNames)
    {
        return MapToDto(source, dto, p => !excludePropertyNames.Contains(p.Name));
    }

    public TSource MapToObj<TDto, TSource>(TDto dto, TSource source, List<string> excludePropertyNames)
    {
        return MapToObj(dto, source, p => !excludePropertyNames.Contains(p.Name));
    }

    private static void ValidateObj<TSource, TDto>(TSource source, TDto dto)
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source), "Source object cannot be null.");
        if (dto is null)
            throw new ArgumentNullException(nameof(dto), "DTO object cannot be null.");
    }
}