using System;

namespace MedinaDokter.Api.Infrastructure.Persistence.Entities;

public class User
{
     public ulong Id { get; set; }

     public string Username { get; set; } = string.Empty;

     public string Password { get; set; } = string.Empty;

     public ulong? PegawaiId { get; set; }

     public DateTime? CreatedAt { get; set; }

     public DateTime? UpdatedAt { get; set; }

     // Navigation Property
     public Pegawai? Pegawai { get; set; }
}
