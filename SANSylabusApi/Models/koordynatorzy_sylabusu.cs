using System;
using System.Collections.Generic;

namespace SylabusAPI.Models;

public partial class koordynatorzy_sylabusu
{
    public int id { get; set; }

    public int sylabus_id { get; set; }

    public int uzytkownik_id { get; set; }

    // Nawigacja do powiązanego sylabusa (relacja wiele-do-jednego)
    public virtual sylabusy sylabus { get; set; } = null!;

    // Nawigacja do powiązanego użytkownika (koordynatora)
    public virtual uzytkownicy uzytkownik { get; set; } = null!;
}
