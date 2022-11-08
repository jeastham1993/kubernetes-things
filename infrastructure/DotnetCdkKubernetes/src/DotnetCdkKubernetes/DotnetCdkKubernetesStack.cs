using Amazon.CDK;
using Amazon.CDK.AWS.ECR;
using Amazon.CDK.AWS.EKS;
using Constructs;

namespace DotnetCdkKubernetes
{
    public class DotnetCdkKubernetesStack : Stack
    {
        internal DotnetCdkKubernetesStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            var ecrRepo = new Repository(this, "asp-net-app-repo", new RepositoryProps()
            {
                RepositoryName = "asp-net-app-repo",
                ImageTagMutability = TagMutability.IMMUTABLE,
                ImageScanOnPush = true
            });
            
            var networking = new Networking(this, "dotnet-kubernetes-networking");

            var eksCluster = new FargateEksCluster(this,  "dotnet-kubernetes-stack", new FargateEksClusterProps("dotnet-kubernetes", networking.Vpc));
        }
    }
}
