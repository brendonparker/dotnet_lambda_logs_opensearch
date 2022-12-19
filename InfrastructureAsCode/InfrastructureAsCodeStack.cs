using Amazon.CDK;
using Constructs;
using Lambda = Amazon.CDK.AWS.Lambda;
using Firehose = Amazon.CDK.AWS.KinesisFirehose.Alpha;
using Destinations = Amazon.CDK.AWS.KinesisFirehose.Destinations.Alpha;
using S3 = Amazon.CDK.AWS.S3;

namespace InfrastructureAsCode;

public class InfrastructureAsCodeStack : Stack
{
    internal InfrastructureAsCodeStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
    {
        var bucket = new S3.Bucket(this, "Bucket");
        var deliveryStream = new Firehose.DeliveryStream(this, "DeliveryStream", new Firehose.DeliveryStreamProps
        {
            Destinations = new[]
            {
                new Destinations.S3Bucket(bucket, new Destinations.S3BucketProps
                {
                    BufferingSize = Size.Mebibytes(1),
                    BufferingInterval = Duration.Seconds(60),
                    Compression = Destinations.Compression.GZIP
                }),
            },
        });

        var lambda = new Lambda.Function(this, "Lambda", new Lambda.FunctionProps
        {
            Runtime = Lambda.Runtime.DOTNET_6,
            MemorySize = 1024,
            Handler = "SampleApi",
            Code = new Lambda.AssetCode("./LambdaSource/SampleApi"),
            Layers = new[]
            {
                new Lambda.LayerVersion(this, "CustomLayer", new Lambda.LayerVersionProps
                {
                    CompatibleRuntimes = new[] { Lambda.Runtime.DOTNET_6 },
                    Code = new Lambda.AssetCode("./LambdaSource/SampleExtension"),
                    RemovalPolicy = RemovalPolicy.DESTROY
                })
            },
            Environment = new Dictionary<string, string>
            {
                ["DELIVERY_STREAM_NAME"] = deliveryStream.DeliveryStreamName
            }
        });

        deliveryStream.GrantPutRecords(lambda);

        lambda.Role.DenyCloudWatch();

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
