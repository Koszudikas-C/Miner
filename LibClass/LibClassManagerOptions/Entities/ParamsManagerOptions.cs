using LibClassManagerOptions.Entities.Enum;
using LibClassProcessOperations.Entities;

namespace LibClassManagerOptions.Entities;

public class ParamsManagerOptions<T>
{
    private T ParamsForProcess { get; set; }
    public TypeManagerOptions TypeManagerOptions { get; private set; }
    
    private static readonly Dictionary<TypeManagerOptions, Type> TypeMap = new()
    {
        { TypeManagerOptions.AuthSocks5, typeof(ParamsSocks5) },
    };


    public ParamsManagerOptions(TypeManagerOptions typeManagerOptions, T paramsForProcess)
    {
        TypeManagerOptions = typeManagerOptions;
        ParamsForProcess = paramsForProcess ?? 
                           throw new ArgumentNullException(nameof(paramsForProcess));

        ValidateParams();
    }

    private void ValidateParams()
    {
        if (!TypeMap.TryGetValue(TypeManagerOptions, out var expectedType))
            throw new ArgumentOutOfRangeException(nameof(TypeManagerOptions), $"No type mapped to{TypeManagerOptions}");
        
        if (!expectedType.IsInstanceOfType(ParamsForProcess))
            throw new ArgumentException($"ParamsForProcess " +
                                        $"should be of the type {expectedType.Name} for the type {TypeManagerOptions}");
    }
    
    public T GetParamsForProcess<T>() where T : class =>
        ParamsForProcess as T ?? throw new InvalidCastException($"It was" +
                                                                $" not possible to convert paramsForProcess to {typeof(T).Name}");
}