using LibClassManagerOptions.Entities.Enum;

namespace LibClassManagerOptions.Entities;

public class ParamsManagerOptionsResponse
{
    public object ParamsForProcessResponse { private get; set; } = new();
    
    public string?  TypeName { get; set; }
    public TypeManagerOptionsResponse TypeManagerOptionsResponse { get; set; }
    
    public T GetParamsForProcessResponse<T>() where T : class
    {
        return ParamsForProcessResponse as T
               ?? throw new InvalidCastException($"The object is not of expected type:{typeof(T).Name}");
    }
}