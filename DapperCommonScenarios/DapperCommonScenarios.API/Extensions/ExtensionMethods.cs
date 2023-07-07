namespace DapperCommonScenarios.API.Extensions
{
    public static class ExtensionMethods
    {
        public static int ToInteger(this string value)
        {
            int outValue = 0;
            int.TryParse(value, out outValue);
            return outValue;
        }
    }
}
