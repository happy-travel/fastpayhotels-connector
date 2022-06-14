using Microsoft.EntityFrameworkCore;

namespace HappyTravel.FastpayhotelsConnector.Data;
public class FastpayhotelsContext : DbContext
{
    public FastpayhotelsContext()
    { }

    public FastpayhotelsContext(DbContextOptions<FastpayhotelsContext> options) : base(options)
    { }
}
