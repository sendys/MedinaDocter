using System;

namespace MedinaDokter.Api.Infrastructure.Persistence.Entities;

public class Pegawai
{
     public ulong Id { get; set; }

     public string Nama { get; set; } = string.Empty;
     public string? Flag { get; set; }

     public ICollection<User> Users { get; set; } = new List<User>();
}
