using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.EKS;
using Amazon.CDK.AWS.IAM;
using Constructs;

namespace DotnetCdkKubernetes;

public record AwsLoadBalancerControllerProps(ICluster EksCluster, IVpc Vpc);

public class AwsLoadBalancerController : Construct
{
    public AwsLoadBalancerController(Construct scope, string id, AwsLoadBalancerControllerProps props) : base(scope, id)
    {
        var serviceAccount = props.EksCluster.AddServiceAccount(
            "aws-load-balancer-controller",
            new ServiceAccountOptions()
            {
                Name = "aws-load-balancer-controller",
                Namespace = "kube-system"
            });

        var lbAcmPolicyStatements = new PolicyStatement(new PolicyStatementProps()
        {
            Effect = Effect.ALLOW,
            Actions = new[]
            {
                "acm:DescribeCertificate",
                "acm:ListCertificates",
                "acm:GetCertificate"
            },
            Resources = new[] {"*"}
        });

        var lbEc2PolicyStatements = new PolicyStatement(new PolicyStatementProps()
        {
            Effect = Effect.ALLOW,
            Actions = new[]
            {
                "ec2:AuthorizeSecurityGroupIngress",
                "ec2:CreateSecurityGroup",
                "ec2:CreateTags",
                "ec2:DeleteTags",
                "ec2:DeleteSecurityGroup",
                "ec2:DescribeAccountAttributes",
                "ec2:DescribeAddresses",
                "ec2:DescribeInstances",
                "ec2:DescribeInstanceStatus",
                "ec2:DescribeInternetGateways",
                "ec2:DescribeNetworkInterfaces",
                "ec2:DescribeSecurityGroups",
                "ec2:DescribeSubnets",
                "ec2:DescribeTags",
                "ec2:DescribeVpcs",
                "ec2:ModifyInstanceAttribute",
                "ec2:ModifyNetworkInterfaceAttribute",
                "ec2:RevokeSecurityGroupIngress",
                "ec2:DescribeAvailabilityZones"
            },
            Resources = new[] {"*"}
        });
        var lbElbPolicyStatements = new PolicyStatement(new PolicyStatementProps()
        {
            Effect = Effect.ALLOW,
            Actions = new[]
            {
                "elasticloadbalancing:AddListenerCertificates",
                "elasticloadbalancing:AddTags",
                "elasticloadbalancing:CreateListener",
                "elasticloadbalancing:CreateLoadBalancer",
                "elasticloadbalancing:CreateRule",
                "elasticloadbalancing:CreateTargetGroup",
                "elasticloadbalancing:DeleteListener",
                "elasticloadbalancing:DeleteLoadBalancer",
                "elasticloadbalancing:DeleteRule",
                "elasticloadbalancing:DeleteTargetGroup",
                "elasticloadbalancing:DeregisterTargets",
                "elasticloadbalancing:DescribeListenerCertificates",
                "elasticloadbalancing:DescribeListeners",
                "elasticloadbalancing:DescribeLoadBalancers",
                "elasticloadbalancing:DescribeLoadBalancerAttributes",
                "elasticloadbalancing:DescribeRules",
                "elasticloadbalancing:DescribeSSLPolicies",
                "elasticloadbalancing:DescribeTags",
                "elasticloadbalancing:DescribeTargetGroups",
                "elasticloadbalancing:DescribeTargetGroupAttributes",
                "elasticloadbalancing:DescribeTargetHealth",
                "elasticloadbalancing:ModifyListener",
                "elasticloadbalancing:ModifyLoadBalancerAttributes",
                "elasticloadbalancing:ModifyRule",
                "elasticloadbalancing:ModifyTargetGroup",
                "elasticloadbalancing:ModifyTargetGroupAttributes",
                "elasticloadbalancing:RegisterTargets",
                "elasticloadbalancing:RemoveListenerCertificates",
                "elasticloadbalancing:RemoveTags",
                "elasticloadbalancing:SetIpAddressType",
                "elasticloadbalancing:SetSecurityGroups",
                "elasticloadbalancing:SetSubnets",
                "elasticloadbalancing:SetWebAcl"
            },
            Resources = new[] {"*"}
        });

        var lbIamPolicyStatements = new PolicyStatement(new PolicyStatementProps()
        {
            Effect = Effect.ALLOW,
            Actions = new[]
            {
                "iam:CreateServiceLinkedRole",
                "iam:GetServerCertificate",
                "iam:ListServerCertificates"
            },
            Resources = new[] {"*"}
        });

        var lbCognitoPolicyStatements = new PolicyStatement(new PolicyStatementProps()
        {
            Effect = Effect.ALLOW,
            Actions = new[]
            {
                "cognito-idp:DescribeUserPoolClient"
            },
            Resources = new[] {"*"}
        });

        var lbWafRegPolicyStatements = new PolicyStatement(new PolicyStatementProps()
        {
            Effect = Effect.ALLOW,
            Actions = new[]
            {
                "waf-regional:GetWebACLForResource",
                "waf-regional:GetWebACL",
                "waf-regional:AssociateWebACL",
                "waf-regional:DisassociateWebACL"
            },
            Resources = new[] {"*"}
        });

        var lbTagPolicyStatements = new PolicyStatement(new PolicyStatementProps()
        {
            Effect = Effect.ALLOW,
            Actions = new[]
            {
                "tag:GetResources", "tag:TagResources"
            },
            Resources = new[] {"*"}
        });

        var lbWafPolicyStatements = new PolicyStatement(new PolicyStatementProps()
        {
            Effect = Effect.ALLOW,
            Actions = new[]
            {
                "waf:GetWebACL"
            },
            Resources = new[] {"*"}
        });

        var lbWafv2PolicyStatements = new PolicyStatement(new PolicyStatementProps()
        {
            Effect = Effect.ALLOW,
            Actions = new[]
            {
                "wafv2:GetWebACL",
                "wafv2:GetWebACLForResource",
                "wafv2:AssociateWebACL",
                "wafv2:DisassociateWebACL"
            },
            Resources = new[] {"*"}
        });

        var lbShieldPolicyStatements = new PolicyStatement(new PolicyStatementProps()
        {
            Effect = Effect.ALLOW,
            Actions = new[]
            {
                "shield:DescribeProtection",
                "shield:GetSubscriptionState",
                "shield:DeleteProtection",
                "shield:CreateProtection",
                "shield:DescribeSubscription",
                "shield:ListProtections"
            },
            Resources = new[] {"*"}
        });

        serviceAccount.AddToPrincipalPolicy(lbAcmPolicyStatements);
        serviceAccount.AddToPrincipalPolicy(lbEc2PolicyStatements);
        serviceAccount.AddToPrincipalPolicy(lbElbPolicyStatements);
        serviceAccount.AddToPrincipalPolicy(lbIamPolicyStatements);
        serviceAccount.AddToPrincipalPolicy(
            lbCognitoPolicyStatements
        );
        serviceAccount.AddToPrincipalPolicy(
            lbWafRegPolicyStatements
        );
        serviceAccount.AddToPrincipalPolicy(lbTagPolicyStatements);
        serviceAccount.AddToPrincipalPolicy(lbWafPolicyStatements);
        serviceAccount.AddToPrincipalPolicy(lbWafv2PolicyStatements);
        serviceAccount.AddToPrincipalPolicy(
            lbShieldPolicyStatements
        );

        var stack = Stack.Of(this);

        props.EksCluster.AddHelmChart("aws-load-balancer-controller", new HelmChartOptions()
        {
            Chart = "aws-load-balancer-controller",
            Repository = "https://aws.github.io/eks-charts",
            Namespace = "kube-system",
            Values = new Dictionary<string, object>()
            {
                {"clusterName", props.EksCluster.ClusterName},
                {"region", stack.Region},
                {"vpcId", props.Vpc.VpcId},
                {
                    "serviceAccount", new Dictionary<string, object>()
                    {
                        {"create", false},
                        {"name", "aws-load-balancer-controller"}
                    }
                }
            }
        });
    }
}