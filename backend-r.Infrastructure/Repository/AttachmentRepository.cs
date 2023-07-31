namespace backend_r.Infrastructure.Repository
{
    public class AttachmentRepository : BaseRepository<Attachment> , IAttachmentRepository
    {
        private readonly JoshuaContext _context;
        public AttachmentRepository(JoshuaContext context) : base(context)
        {
            _context = context;
        }
    }
}
