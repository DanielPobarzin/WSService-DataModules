using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;
using Serilog;

namespace Interactors.Helpers
{
    public class ConfigValidHelper
    {
        public static bool ValidateConfigurationJson(string Schema, string Config)
        {
            JSchema schema = JSchema.Parse(Schema);
            JObject jsonObject = JObject.Parse(Config);
            if (!jsonObject.IsValid(schema))
            {
                Log.Warning("The configuration file was not found or it is incorrect. The default configuration is used.");
                return false;
            }
            return true;
        }
    }


}