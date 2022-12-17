using Amazon.CDK;
using Constructs;
using Lambda = Amazon.CDK.AWS.Lambda; 
namespace InfrastructureAsCode;

public class InfrastructureAsCodeStack : Stack
{
    internal InfrastructureAsCodeStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
    {
        var lambda = new Lambda.Function(this, "Lambda", new Lambda.FunctionProps
        {
            Runtime = Lambda.Runtime.DOTNET_6,
            MemorySize = 1024,
            Handler = "SampleApi",
            Code = new Lambda.AssetCode("./LambdaSource/SampleApi")
        });

        var functionUrl = lambda.AddFunctionUrl(new Lambda.FunctionUrlOptions
        {
            AuthType = Lambda.FunctionUrlAuthType.NONE
        });

        new CfnOutput(this, "FunctionUrl", new CfnOutputProps
        {
            ExportName = "SampleApiFunctionUrl",
            Value = functionUrl.Url
        });
    }
}