## Pre-Reqs for Dev/Build env
- Install cdk `npm install -g aws-cdk`
- Install dotnet SDK


## Build/Deploy:
```
dotnet publish ./SampleApi -c Release -o ./LambdaSource/SampleApi
dotnet publish ./SampleExtension -c Release -o ./LambdaSource/SampleExtension
cdk deploy
```

## Additional Resources
* [Introducing AWS Lambda Extensions](https://aws.amazon.com/blogs/compute/introducing-aws-lambda-extensions-in-preview/)
* [Building Extensions for AWS Lambda](https://aws.amazon.com/blogs/compute/building-extensions-for-aws-lambda-in-preview/)
* [Using AWS Lambda extensions to send logs to custom destinations](https://aws.amazon.com/blogs/compute/using-aws-lambda-extensions-to-send-logs-to-custom-destinations/)

## Things I learned / Outstanding Questions

1. You MUST_ do the long polling to get the next event from the lambda. Without that, the lambda runtime never gets invoked. Is there a way to consume logs without needing to do this?
2. The L2 constructs for Firehose Destination is in Alpha for the CDK.
3. There are only L1 constructs for OpenSearch. So getting those wired in via the CDK will take a little more work. Hopefully L2 constructs for OpenSearch and the ability to add it as a destination for Firehose are coming soon?
