using System.Collections;
using System.Reflection;
using LibMapperObj.Interface;

namespace LibMapperObj.Service;

public class MapperObjService : IMapperObj
{
    private static readonly Dictionary<(Type, Type), PropertyInfo[]> PropertyCache = new();
    
    public TTarget Map<TSource, TTarget>(TSource source)
        where TTarget : new()
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        
        var target = new TTarget();
        MapTo(source, target);
        return target;
    }
    
    public TTarget Map<TSource, TTarget>(TSource source, Func<TTarget> factory)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        
        var target = factory();
        MapTo(source, target);
        return target;
    }
    
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
                => p.Name == sourceProp.Name);

            if (dtoProp is null) continue;

            var value = dtoProp.GetValue(dto);

            if (value is null) continue;

            sourceProp.SetValue(source, value);
        }

        return source;
    }

    public void MapTo<TSource, TTarget>(TSource source, TTarget target)
    {
        if (source == null || target == null) return;

        var sourceType = typeof(TSource);
        var targetType = typeof(TTarget);

        var props = GetMappedProperties(sourceType, targetType);

        foreach (var (sourceProp, targetProp) in props)
        {
            var sourceValue = sourceProp.GetValue(source);
            if (sourceValue == null) continue;
            
            if (targetProp.PropertyType == sourceProp.PropertyType || IsSimpleType(targetProp.PropertyType))
            {
                targetProp.SetValue(target, sourceValue);
            }
            
            else if (IsList(sourceProp.PropertyType, out var sourceItemType) &&
                     IsList(targetProp.PropertyType, out var targetItemType))
            {
                var listSource = (IEnumerable)sourceValue;
                var listTarget = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(targetItemType))!;
                foreach (var item in listSource)
                {
                    var mappedItem = MapDynamic(item, sourceItemType, targetItemType);
                    listTarget.Add(mappedItem);
                }
                targetProp.SetValue(target, listTarget);
            }
            else
            {
                Activator.CreateInstance(targetProp.PropertyType);
                var mappedNested = MapDynamicConstructor(sourceValue, sourceProp.PropertyType, targetProp.PropertyType);
                targetProp.SetValue(target, mappedNested);
            }
        }
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
                p.Name == sourceProp.Name);

            if (dtoProp is null) continue;

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

    private static bool IsSimpleType(Type type)
    {
        return type.IsPrimitive
               || type.IsEnum
               || type == typeof(string)
               || type == typeof(decimal)
               || type == typeof(DateTime)
               || type == typeof(Guid)
               || type == typeof(DateTimeOffset)
               || type == typeof(TimeSpan);
    }
    
    private static bool IsList(Type type, out Type itemType)
    {
        if (type.IsGenericType && typeof(IEnumerable).IsAssignableFrom(type))
        {
            itemType = type.GetGenericArguments()[0];
            return true;
        }
        itemType = typeof(object);
        return false;
    }
    
    private object MapDynamic(object source, Type sourceType, Type targetType)
    {
        var method = GetType()
            .GetMethod(nameof(Map), BindingFlags.Public | BindingFlags.Instance)!
            .MakeGenericMethod(sourceType, targetType);
    
        return method.Invoke(this, [source])!;
    }
    
    private object MapDynamicConstructor(object source, Type sourceType, Type targetType)
    {

        var methods = GetType()
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.Name == nameof(Map)
                        && m.IsGenericMethodDefinition
                        && m.GetGenericArguments().Length == 2
                        && m.GetParameters().Length == 1
                        && m.GetParameters()[0].ParameterType.IsGenericParameter)
            .ToArray();

        if (methods.Length != 1)
            throw new InvalidOperationException("Não foi possível determinar unicamente o método genérico 'Map<TSource,TTarget>(TSource)'.");

        var method = methods[0].MakeGenericMethod(sourceType, targetType);

        return method.Invoke(this, new[] { source })!;
    }


    private static List<(PropertyInfo source, PropertyInfo target)> GetMappedProperties(Type sourceType, Type targetType)
    {
        var key = (sourceType, targetType);
        if (PropertyCache.TryGetValue(key, out var cached))
        {
            return cached.Select(p => (p, targetType.GetProperty(p.Name)!)).ToList();
        }

        var sourceProps = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
        var targetProps = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic)
            .Where(p => p.CanWrite).ToDictionary(p => p.Name);

        var matches = sourceProps
            .Where(p => targetProps.ContainsKey(p.Name))
            .Select(p => (p, targetProps[p.Name]))
            .ToList();

        PropertyCache[key] = matches.Select(m => m.p).ToArray();
        return matches;
    }
}