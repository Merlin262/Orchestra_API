using HotChocolate.Types;
using Orchestra.Models.Orchestra.Models;

namespace Orchestra.GraphQLConfig
{
    public class BpmnProcessInstanceType : ObjectType<BpmnProcessInstance>
    {
        protected override void Configure(IObjectTypeDescriptor<BpmnProcessInstance> descriptor)
        {
            descriptor.Field(f => f.Id);
            descriptor.Field(f => f.Name);
            descriptor.Field(f => f.XmlContent);
            descriptor.Field(f => f.CreatedAt);
            descriptor.Field(f => f.BpmnProcessBaselineId);
            descriptor.Field(f => f.BpmnProcessBaseline);
            descriptor.Field(f => f.PoolNames);
            descriptor.Field(f => f.Status);
        }
    }
}
