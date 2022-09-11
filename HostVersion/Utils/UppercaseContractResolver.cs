using Newtonsoft.Json.Serialization;

namespace HostVersion.Utils
{
    public class UppercaseContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            if (propertyName.Length == 1)
                return propertyName[0].ToString().ToUpper();
            return propertyName[0].ToString().ToUpper() + propertyName.Substring(1);
        }
    }
}