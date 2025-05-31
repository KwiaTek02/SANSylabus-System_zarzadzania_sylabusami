using System;
using System.Collections.Generic;

namespace SylabusAPI.Models;

public partial class koordynatorzy_sylabusu
{
    public int id { get; set; }

    public int sylabus_id { get; set; }

    public int uzytkownik_id { get; set; }

    public virtual sylabusy sylabus { get; set; } = null!;

    public virtual uzytkownicy uzytkownik { get; set; } = null!;
}
