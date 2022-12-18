## Pre-Reqs for Dev/Build env
- Install cdk `npm install -g aws-cdk`
- Install dotnet SDK


## Build/Deploy:
```
dotnet publish ./SampleApi -c Release -o ./LambdaSource/SampleApi
dotnet publish ./SampleExtension -c Release -o ./LambdaSource/SampleExtension
cdk deploy
```