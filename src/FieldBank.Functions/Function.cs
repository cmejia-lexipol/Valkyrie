using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace FieldBank.Functions
{
    public class Function
    {
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input">The event for the Lambda function handler to process.</param>
        /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
        /// <returns></returns>
        public string FunctionHandler(string input, ILambdaContext context)
        {
            context.Logger.LogInformation($"Processing input: {input}");
            
            // TODO: Add database operations here when needed
            // For now, just return the uppercase version
            var result = input.ToUpper();
            
            context.Logger.LogInformation($"Result: {result}");
            return result;
        }
    }
}   
