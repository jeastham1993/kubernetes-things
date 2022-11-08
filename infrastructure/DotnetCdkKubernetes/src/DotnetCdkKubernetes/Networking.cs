using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Constructs;

namespace DotnetCdkKubernetes;

public class Networking : Construct
{
    public IVpc Vpc { get; private set; }
    
    public Networking(Construct scope, string id): base(scope, id)
    {
        Vpc = new Vpc(this, "kubernetes-vpc", new VpcProps()
        {
            NatGateways = 1,
            MaxAzs = 3,
            SubnetConfiguration = new ISubnetConfiguration[3]
            {
                new SubnetConfiguration()
                {
                    Name = "private-subnet-1",
                    SubnetType = SubnetType.PRIVATE_WITH_EGRESS,
                    CidrMask = 24
                },
                new SubnetConfiguration()
                {
                    Name = "public-subnet-1",
                    SubnetType = SubnetType.PUBLIC,
                    CidrMask = 24
                },
                new SubnetConfiguration()
                {
                    Name = "isolated-subnet-1",
                    SubnetType = SubnetType.PRIVATE_ISOLATED,
                    CidrMask = 28
                }
            }
        });
    }
}