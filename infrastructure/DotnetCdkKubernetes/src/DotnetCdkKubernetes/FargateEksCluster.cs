using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.EKS;
using Amazon.CDK.AWS.IAM;
using Constructs;

namespace DotnetCdkKubernetes
{
    public record FargateEksClusterProps(string ClusterName, IVpc Vpc);

    public class FargateEksCluster : Construct
    {
        public FargateEksCluster(Construct scope, string id, FargateEksClusterProps props): base(scope, id)
        {
            var clusterMasterRole = new Role(this, "cluster-master-role", new RoleProps()
            {
                AssumedBy = new AccountRootPrincipal()
            });

            var cluster = new FargateCluster(this, "dotnet-kubernetes", new FargateClusterProps()
            {
                Version = KubernetesVersion.V1_23,
                MastersRole = clusterMasterRole,
                ClusterName = props.ClusterName,
                OutputClusterName = true,
                EndpointAccess = EndpointAccess.PUBLIC_AND_PRIVATE,
                Vpc = props.Vpc,
                VpcSubnets = new ISubnetSelection[1]
                {
                    new SubnetSelection()
                    {
                        SubnetType = SubnetType.PRIVATE_WITH_EGRESS
                    }
                },
            });
            
            var albController = new AwsLoadBalancerController(this, "alb-controller",
                new AwsLoadBalancerControllerProps(cluster, props.Vpc));
        }
    }
}