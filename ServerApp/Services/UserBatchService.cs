using ServerApp.DataAccessLayer;
using Microsoft.EntityFrameworkCore;


namespace ServerApp.Services
{
    public class UserBatchService
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;

        public UserBatchService(AppDbContext appDbContext, EmailService emailService )
        {
            _context = appDbContext;
            _emailService = emailService;
        }
        public async Task ProcessBatch()
        {
            Console.WriteLine("   /// Loading Users List: \n");
            List<User> users = await _context.Users.ToListAsync();
            Console.WriteLine("\n   ///  Successful \n\n");

            foreach (var user in users)
            {
                _emailService.SendEmail(user.Email, "Weekly Newsletter", "This is your weekly newsletter content!");

                user.Name = "_" + user.Name;

                Console.WriteLine($"Changed Name For: | {user.Name} |\n");

            }

            await _context.SaveChangesAsync();
        }
    }
}
