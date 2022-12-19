using Constructs;
using IAM = Amazon.CDK.AWS.IAM;

namespace InfrastructureAsCode;

public static class RoleExtensions
{
    public static void DenyCloudWatch(this IAM.IRole role)
    {
        if (role.Node.Scope is Construct scope)
        {
            role.AttachInlinePolicy(new IAM.Policy(scope, "CloudWatchDeny", new IAM.PolicyProps
            {
                PolicyName = "CloudWatchDeny",
                Document = new IAM.PolicyDocument(new IAM.PolicyDocumentProps
                {
                    Statements = new[]
                    {
                        new IAM.PolicyStatement(new IAM.PolicyStatementProps
                        {
                            Effect = IAM.Effect.DENY,
                            Actions = new[]
                            {
                                "logs:CreateLogGroup",
                                "logs:CreateLogStream",
                                "logs:PutLogEvents",
                            },
                            Resources = new[] { "arn:aws:logs:*:*:*" }
                        })
                    }
                })
            }));
        }
    }
}