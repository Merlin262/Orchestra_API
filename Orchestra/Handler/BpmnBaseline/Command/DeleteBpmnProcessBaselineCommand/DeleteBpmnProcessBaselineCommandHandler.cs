using MediatR;
using Orchestra.Data.Context;

namespace Orchestra.Handler.BpmnBaseline.Command.DeleteBpmnProcessBaselineCommand
{
    public class DeleteBpmnProcessBaselineCommandHandler : IRequestHandler<DeleteBpmnProcessBaselineCommand, bool>
    {
        private readonly ApplicationDbContext _context;

        public DeleteBpmnProcessBaselineCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteBpmnProcessBaselineCommand request, CancellationToken cancellationToken)
        {
            var process = await _context.BpmnProcess.FindAsync(new object[] { request.Id }, cancellationToken);

            if (process == null)
                return false;

            _context.BpmnProcess.Remove(process);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
