using Microsoft.EntityFrameworkCore;

namespace DoctorsOffice.Models
{
  public class DoctorsOfficeContext : DbContext
  {
    //Change to class the DB is dealing with
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<DoctorPatient> DoctorPatient { get; set; }
    
    public DoctorsOfficeContext(DbContextOptions options) : base(options) { }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseLazyLoadingProxies();
    }
  }
}